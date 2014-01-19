using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Converters
{
    /// <summary>
    /// Converts the value from the string in RwHtml markup to the desired type.
    /// </summary>
    public abstract class RwHtmlMarkupConverter
    {

        public bool TryConvertFromString(string value, out object result)
        {
            return TryConvertFromStringCore(value, out result);
        }

        protected abstract bool TryConvertFromStringCore(string value, out object result);

    }

    /// <summary>
    /// Converts the value from the string in RwHtml markup to the desired type.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    public abstract class RwHtmlMarkupConverter<T> : RwHtmlMarkupConverter
    {
        public bool TryConvertFromString(string value, out T result)
        {
            object innerResult;
            var canConvert = base.TryConvertFromString(value, out innerResult);
            result = (T)innerResult;
            return canConvert;
        }
    }
}