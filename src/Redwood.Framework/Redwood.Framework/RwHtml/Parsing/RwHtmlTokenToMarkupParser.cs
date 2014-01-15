using Redwood.Framework.RwHtml.Markup;
using Redwood.Framework.RwHtml.Parsing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Parsing
{
    /// <summary>
    /// Provides parsing of rwhtml tokens into rwhtml markup nodes.
    /// </summary>
    public class RwHtmlTokenToMarkupParser : RwHtmlMarkupParserBase<Markup.MarkupNode>
    {
        public const string NamespaceDefinitionNamespace = "xmlns";

        Stack<MarkupNode> nodeStack;
        
        protected override void Init()
        {
            // init
            nodeStack = new Stack<MarkupNode>();
        
            base.Init();
        }

        protected override void PushValue(MarkupNode value)
        {
            // push object or member begin
            switch (value.NodeType)
            {
                case MarkupNodeType.BeginObject:
                case MarkupNodeType.BeginMember:
                    nodeStack.Push(value);
                    break;
            }

            base.PushValue(value);
        }

        protected override void OnOpenTagBegin(RwOpenTagBeginToken token)
        {
            // resolve type of tag
            NameWithPrefix name = NameWithPrefix.Parse(token.TagName);
            var parentBeginObject = nodeStack.FirstOrDefault(n => n.NodeType == MarkupNodeType.BeginObject);

            // is it regular element placed inside content property?
            // example: "<rw:TextBox"
            if (!name.HasMoreNames)
            {
                // begin content property if object is placed directly into another object
                InsertContentPropertyBeginMemberIfNeeded(token.SpanPosition);

                // object start
                PushValue(new MarkupNode()
                {
                    Level = Level,
                    CurrentPosition = token.SpanPosition,
                    NodeType = MarkupNodeType.BeginObject,
                    Type = new MarkupType()
                    {
                        Name = name
                    }
                });
                return;
            }

            // make sure content property is closed
            ExitContentPropertyIfInside();

            // parent node sub-property
            // example: "<rw:ItemsControl><rw:ItemsControl.RowTemplate"
            if (parentBeginObject != null && name.BeginsWith(parentBeginObject.Type.Name))
            {
                // member start
                PushValue(new MarkupNode()
                {
                    Level = Level,
                    CurrentPosition = token.SpanPosition,
                    NodeType = MarkupNodeType.BeginMember,
                    Member = new MarkupMember()
                    {
                        IsAttachedProperty = false,
                        IsInlineDefinition = false,
                        Name = name
                    }
                });
                return;
            } 

            // attached property 
            // example: "<rw:ItemsControl><rw:Layout.Template"
            PushValue(new MarkupNode()
            {
                Level = Level,
                CurrentPosition = token.SpanPosition,
                NodeType = MarkupNodeType.BeginMember,
                Member = new MarkupMember()
                {
                    IsAttachedProperty = true,
                    IsInlineDefinition = false,
                    Name = name
                }
            });
        }

        protected override void OnOpenTagEnd()
        {
            // no operation
        }

        protected override void OnTagEnd()
        {
            // when tag ends it always ends object or member
            OnEndOfObjectOrMember();
        }

        protected override void OnNewAttributeValue(RwAttributeToken token, RwValueToken value)
        {
            // resolve type of attribute
            NameWithPrefix name = NameWithPrefix.Parse(token.Name);

            // is it namespace declaration?
            if(name.HasPrefix && string.Equals(name.Prefix, NamespaceDefinitionNamespace))
            {
                // namespace declaration
                PushValue(new MarkupNode()
                {
                    Level = Level,
                    CurrentPosition = token.SpanPosition,
                    NodeType = MarkupNodeType.NamespaceDeclaration,
                    Namespace = new RwHtmlNamespaceDeclaration()
                    {
                        Prefix = name.Names.Single(),
                        RwHtmlNamespace = value.Text
                    }
                });
                return;
            }

            // begin member - property (<rw:TextBox Prop1=") or attached property (<rw:TextBox Layout.Prop1=")
            PushValue(new MarkupNode()
            {
                Level = Level,
                CurrentPosition = token.SpanPosition,
                NodeType = MarkupNodeType.BeginMember,
                Member = new MarkupMember()
                {
                    IsAttachedProperty = name.HasMoreNames,
                    IsInlineDefinition = true,
                    Name = name
                }
            });

            // value
            PushValue(new MarkupNode()
            {
                Level = Level,
                NodeType = MarkupNodeType.Value,
                CurrentPosition = value.SpanPosition,
                Value = new MarkupValue(value.Text, false)
            });

            // end member
            OnEndOfObjectOrMember();
        }

        private void OnEndOfObjectOrMember()
        {
            // when ending an object make sure we will end also content property member if present
            ExitContentPropertyIfInside();

            // pop member or object
            var node = nodeStack.Pop();

            if (node.NodeType == MarkupNodeType.BeginObject)
            {
                PushValue(new MarkupNode()
                {
                    Level = Level,
                    NodeType = MarkupNodeType.EndObject
                });
            }
            else if (node.NodeType == MarkupNodeType.BeginMember)
            {
                PushValue(new MarkupNode()
                {
                    Level = Level,
                    NodeType = MarkupNodeType.EndMember
                });
            }
            else
            {
                throw new InvalidOperationException("Unexpected node type in stack: " + node.NodeType);
            }
        }
        
        protected override void OnLiteralToken(RwValueToken literal)
        {
            InsertContentPropertyBeginMemberIfNeeded(literal.SpanPosition);

            // value
            PushValue(new MarkupNode()
            {
                Level = Level,
                NodeType = MarkupNodeType.Value,
                CurrentPosition = literal.SpanPosition,
                Value = new MarkupValue(literal.Text, false)
            });
        }

        private void InsertContentPropertyBeginMemberIfNeeded(SpanPosition position)
        {
            // object or value directly inside object is placed inside content property
            bool shouldBeInsideContentProperty = nodeStack.Count > 0 && nodeStack.Peek().NodeType == MarkupNodeType.BeginObject;

            if (!shouldBeInsideContentProperty)
                return; // already in content property or content property is not required

            // content property begin member
            PushValue(new MarkupNode()
            {
                Level = Level,
                CurrentPosition = position,
                NodeType = MarkupNodeType.BeginMember,
                Member = new MarkupMember()
                {
                    IsContentProperty = true,
                    IsInlineDefinition = false
                }
            });
        }

        private void ExitContentPropertyIfInside()
        {
            // pop member or object
            var node = nodeStack.Peek();

            // content property "BeginMember" needs to be ended automatically
            if (node.NodeType == MarkupNodeType.BeginMember && node.Member.IsContentProperty)
            {
                // remove from stack
                nodeStack.Pop();

                // push end member of content property
                PushValue(new MarkupNode()
                {
                    Level = Level,
                    NodeType = MarkupNodeType.EndMember
                });
            }
        }

        protected override void OnEndOfDocument()
        {
            // end of document
            PushValue(new MarkupNode()
            {
                Level = Level,
                NodeType = MarkupNodeType.EndOfDocument
            });
        }

        protected int Level
        {
            get
            {
                return nodeStack.Count;
            }
        }
    }
}
