using Redwood.Framework.Binding;
using Redwood.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    public class RwHtmlMarkupBuilder
    {
        public RwHtmlMarkupBuilder()
        {
            elementStack = new Stack<MarkupElement>();
            namespaceScope = new RwHtmlNamespaceScope();
            typeMapper = ControlTypeMapper.Default;

            // hack - default namespace
            namespaceScope.AddNamespace("rw", Redwood.Framework.Controls.RedwoodControl.DefaultSchemaNamespaceName);
        }

        RwHtmlNamespaceScope namespaceScope;
        ControlTypeMapper typeMapper;
        Stack<MarkupElement> elementStack;

        public void PushElement(MarkupElement element)
        {
            // push scope for defining namespace prefixes
            namespaceScope.PushScope();

            bool hasAnyBinding = element.Attributes.Any(e => e.Value.IsExpression);
            bool isControl = element.Name.HasPrefix;

            // register control namespaces
            RegisterNamespacePrefixRegistration(element);

            // convert to control if binding is applied to regular non-control html element
            if (hasAnyBinding && !isControl)
            {
                ConvertToHtmlElementControl(ref element);
                isControl = true;
            }

            if (isControl)
            {
                RedwoodControl control = BuildControl(element);
            }
            else
            {
                
            }
        }

        private void ConvertToHtmlElementControl(ref MarkupElement element)
        {
            throw new NotImplementedException("Using binding on regular html elements is not supported. Yet.");
        }

        private RedwoodControl BuildControl(MarkupElement element)
        {
            var name = element.Name;

            RedwoodControl control = ActivateControl(name);

            foreach (var atr in element.Attributes)
            {
                if (atr.Key.HasPrefix)
                    continue; // maybe attached property?

                throw new NotImplementedException();
                RedwoodProperty property = null; // control.GetPropertyByName(atr.Key.Name);
                object value;
                if (atr.Value.IsExpression)
                {
                    // binding
                    var binding = new BindingBase()
                    {
                        Path = new PropertyPath(atr.Value.Value)
                    };
                    value = binding;
                }
                else
                {
                    // regular value
                    value = DefaultModelBinder.ConvertValue(atr.Value.Value, property.PropertyType);
                }
                control.SetValue(property, value);
            }

            return control;
        }

        private RedwoodControl ActivateControl(NameWithPrefix name)
        {
            if (!name.HasPrefix)
                throw new InvalidOperationException("Can't build control without namespace prefix."); ;

            var rwhtmlNamespace = namespaceScope.GetNamespaceByPrefix(name.Prefix);
            if (rwhtmlNamespace == null)
                throw new InvalidOperationException("Invalid namespace prefix: " + name.Prefix);

            var type = typeMapper.GetType(rwhtmlNamespace, name.SingleName());

            if (type == null)
                throw new InvalidOperationException(string.Format("Control \"{0}\" not found in namespace \"{1}\".", name.SingleName(), rwhtmlNamespace));

            var instance = Activator.CreateInstance(type);
            var controlInstance = instance as RedwoodControl;
            if (controlInstance == null)
                throw new InvalidOperationException(string.Format("Type \"{0}\" is not RedwoodControl.", instance.GetType()));

            return controlInstance;
        }

        public void WriteValue(MarkupValue value)
        {
            throw new NotImplementedException();
        }

        public void PopElement()
        {
            namespaceScope.PopScope();
        }

        private void RegisterNamespacePrefixRegistration(MarkupElement element)
        {
            // register namespace prefix registrations
            foreach (var attribute in element.Attributes)
            {
                if (attribute.Key.HasPrefix && attribute.Key.Prefix == namespaceScope.NamespaceDefinitionNamespace)
                {
                    if (attribute.Value.IsExpression)
                        throw new InvalidOperationException("Expression is not supported for namespace reference.");

                    namespaceScope.AddNamespace(attribute.Key.Names.Single(), attribute.Value.Value);
                }
            }
        }
    }
}
