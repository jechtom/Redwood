using Redwood.Framework.Binding;
using Redwood.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Parsing;
using Redwood.Framework.RwHtml.Markup;

namespace Redwood.Framework.RwHtml
{
    public class RwHtmlSerializer
    {
        Markup.NamespaceDeclaration[] defaultNamespaces;
        ControlTypeMapper typeMapper;

        public RwHtmlSerializer()
        {
            typeMapper = ControlTypeMapper.Default;

            // hack - default namespace
            defaultNamespaces = new [] { 
                new Markup.NamespaceDeclaration() { Prefix = "rw", RwHtmlNamespace = RedwoodControl.DefaultSchemaNamespaceName }
            };
        }

        public object LoadFromString(string rwhtml)
        {
            var input = new StringTextReader(rwhtml);
            return LoadFrom(input);
        }

        public object LoadFrom(StringTextReader input)
        {
            var tokenizer = new Redwood.Framework.RwHtml.Parsing.RwHtmlTokenizer();
            var tokenParser = new Redwood.Framework.RwHtml.Parsing.RwHtmlTokenToMarkupParser();
            var namespaceVisitor = new Redwood.Framework.RwHtml.Markup.MarkupStreamNamespaceVisitor(defaultNamespaces);
            var htmlElementVisitor = new Redwood.Framework.RwHtml.Markup.MarkupStreamHtmlElementVisitor();
            var mapperVisitor = new Redwood.Framework.RwHtml.Markup.MarkupStreamMapperVisitor();
            var activatorVisitor = new Redwood.Framework.RwHtml.Markup.MarkupStreamActivatorVisitor();
            
            // tokenize input
            var tokenizerOutput = tokenizer.Parse(input);

            // read markup from tokens
            var tokenParserOutput = tokenParser.Read(tokenizerOutput);

            // resolve namespaces
            var namespaceVisitorOutput = namespaceVisitor.Process(tokenParserOutput);

            // resolve "raw" HTML elements
            var htmlElementVisitorOutput = htmlElementVisitor.Process(namespaceVisitorOutput);

            // map to CLR types and properties
            var mapperVisitorOutput = mapperVisitor.Process(htmlElementVisitorOutput);

            // activate object
            var activatorVisitorOutput = activatorVisitor.Process(mapperVisitorOutput);

            foreach (var item in activatorVisitorOutput)
            {
                //Debug.WriteLine(item.ToDebugString());
            }

            return activatorVisitor.Result;
        }

        //private RedwoodControl BuildControl(MarkupElement element)
        //{
        //    var name = element.Name;

        //    RedwoodControl control = ActivateControl(name);

        //    foreach (var atr in element.Attributes)
        //    {
        //        if (atr.Key.HasPrefix)
        //            continue; // maybe attached property?

        //        throw new NotImplementedException();
        //        RedwoodProperty property = null; // control.GetPropertyByName(atr.Key.Name);
        //        object value;
        //        if (atr.Value.IsExpression)
        //        {
        //            // binding
        //            var binding = new BindingBase()
        //            {
        //                Path = new PropertyPath(atr.Value.Value)
        //            };
        //            value = binding;
        //        }
        //        else
        //        {
        //            // regular value
        //            value = DefaultModelBinder.ConvertValue(atr.Value.Value, property.PropertyType);
        //        }
        //        control.SetValue(property, value);
        //    }

        //    return control;
        //}

        //private RedwoodControl ActivateControl(NameWithPrefix name)
        //{
        //    if (!name.HasPrefix)
        //        throw new InvalidOperationException("Can't build control without namespace prefix."); ;

        //    var rwhtmlNamespace = namespaceScope.GetNamespaceByPrefix(name.Prefix);
        //    if (rwhtmlNamespace == null)
        //        throw new InvalidOperationException("Invalid namespace prefix: " + name.Prefix);

        //    var type = typeMapper.GetType(rwhtmlNamespace, name.SingleName());

        //    if (type == null)
        //        throw new InvalidOperationException(string.Format("Control \"{0}\" not found in namespace \"{1}\".", name.SingleName(), rwhtmlNamespace));

        //    var instance = Activator.CreateInstance(type);
        //    var controlInstance = instance as RedwoodControl;
        //    if (controlInstance == null)
        //        throw new InvalidOperationException(string.Format("Type \"{0}\" is not RedwoodControl.", instance.GetType()));

        //    return controlInstance;
        //}


    }
}
