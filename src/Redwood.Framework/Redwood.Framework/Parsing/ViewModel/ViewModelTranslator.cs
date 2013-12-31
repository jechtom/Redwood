using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.ViewModel
{
    /// <summary>
    /// Translates C# ViewModels to TypeScript ones.
    /// </summary>
    public class ViewModelTranslator
    {
        private readonly IViewModelMetadataExtractor extractor;
        private readonly IViewModelTypeMapper typeMapper;
        private readonly IViewModelWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelTranslator"/> class.
        /// </summary>
        public ViewModelTranslator(IViewModelMetadataExtractor extractor, IViewModelTypeMapper typeMapper, IViewModelWriter writer)
        {
            this.extractor = extractor;
            this.typeMapper = typeMapper;
            this.writer = writer;
        }


        /// <summary>
        /// Translates the view models.
        /// </summary>
        public string TranslateViewModels(params Type[] viewModelTypes)
        {
            var dependent = viewModelTypes.SelectMany(extractor.GetDependentTypes).Distinct();
            
            writer.WriteBeginFile();
            
            // write classes
            foreach (var type in dependent)
            {
                writer.WriteBeginClass(typeMapper.MapType(type));

                // write properties
                foreach (var prop in extractor.GetProperties(type))
                {
                    if (prop.IsReadOnly)
                    {
                        if (string.IsNullOrEmpty(prop.ClientImplementation))
                        {
                            throw new InvalidOperationException(string.Format("The property '{0}' of type '{1}' must have the ClientImplementationAttribute!", prop.PropertyName, type));
                        }
                        writer.WriteReadOnlyProperty(prop.PropertyName, typeMapper.MapType(prop.PropertyType), prop.ClientImplementation);
                    }
                    else
                    {
                        writer.WriteProperty(prop.PropertyName, typeMapper.MapType(prop.PropertyType));
                    }
                }

                // write commands
                foreach (var command in extractor.GetCommands(type))
                {
                    string body;
                    if (string.IsNullOrEmpty(command.ClientFunctionName))
                    {
                        body = string.Format("Redwood.PostBack(element, '{0}', arguments);", command.CommandName);
                    }
                    else
                    {
                        body = command.ClientFunctionName + "();";
                    }

                    writer.WriteFunction(command.CommandName, new string[] { }, "void", body);
                }

                writer.WriteEndClass();
            }
            writer.WriteEndFile();

            // return the output
            return writer.GetOutput();
        }
    }
}
