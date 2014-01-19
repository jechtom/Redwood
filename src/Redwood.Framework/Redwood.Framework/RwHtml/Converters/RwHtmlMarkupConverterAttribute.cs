using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Converters
{
    /// <summary>
    /// An attribute that specifies which converter will be used to parse this value literal in the RwHtml markup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class RwHtmlMarkupConverterAttribute : Attribute
    {
        private RwHtmlMarkupConverter singletonInstance;
        private object singletonInstanceLocker = new object();


        /// <summary>
        /// Gets the type of the converter. The converter must derive from <see cref="T:RwHtmlMarkupConverter"/>.
        /// </summary>
        public Type ConverterType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the converter instance can be reused.
        /// </summary>
        public bool IsSingleton { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="RwHtmlMarkupConverterAttribute"/> class.
        /// </summary>
        public RwHtmlMarkupConverterAttribute(Type converterType, bool isSingleton = true)
        {
            if (converterType == null)
                throw new ArgumentNullException("converterType");
            if (!typeof(RwHtmlMarkupConverter).IsAssignableFrom(converterType))
                throw new ArgumentException("A type specified in the converterType parameter must be based on the RwHtmlMarkupConverter type.");

            ConverterType = converterType;
            IsSingleton = isSingleton;
        }

        /// <summary>
        /// Gets the converter instance.
        /// </summary>
        public RwHtmlMarkupConverter GetConverter()
        {
            if (!IsSingleton)
            {
                return CreateInstance();
            }
            else
            {
                if (singletonInstance == null)
                {
                    lock (singletonInstanceLocker)
                    {
                        if (singletonInstance == null)
                        {
                            singletonInstance = CreateInstance();
                        }
                    }
                }
                return singletonInstance;
            }
        }

        private RwHtmlMarkupConverter CreateInstance()
        {
            return (RwHtmlMarkupConverter)Activator.CreateInstance(ConverterType);
        }
    }
}