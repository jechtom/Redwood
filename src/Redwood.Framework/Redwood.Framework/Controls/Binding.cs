using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public class Binding
    {

        /// <summary>
        /// Gets the name of the binding path.
        /// </summary>
        public string Path { get; private set; }
        
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        public AttributeList<string> Attributes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class.
        /// </summary>
        public Binding(string path)
        {
            Path = path;
            Attributes = new AttributeList<string>();
        }


        /// <summary>
        /// Gets or sets whether the value should be HTML encoded.
        /// </summary>
        public bool HtmlEncode
        {
            get { return Convert.ToBoolean(Attributes.GetValueOrDefault(Properties.HtmlEncode, "false")); }
            set { Attributes[Properties.HtmlEncode] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets whether the binding should be rendered on server.
        /// </summary>
        public bool RenderOnServer
        {
            get { return Convert.ToBoolean(Attributes.GetValueOrDefault(Properties.RenderOnServer, "false")); }
            set { Attributes[Properties.RenderOnServer] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the string format.
        /// </summary>
        public bool StringFormat
        {
            get { return Convert.ToBoolean(Attributes.GetValueOrDefault(Properties.StringFormat)); }
            set { Attributes[Properties.StringFormat] = value.ToString(); }
        }


        /// <summary>
        /// Evaluates the specified target.
        /// </summary>
        public object Evaluate(object target)
        {
            if (string.IsNullOrEmpty(Path))
            {
                return target;
            }

            var parts = Path.Split('.');
            foreach (var part in parts)
            {
                target = target.GetType().GetProperty(part).GetValue(target);
            }
            return target;
        }


        /// <summary>
        /// Converts the value.
        /// </summary>
        public static object ConvertValue(object value, Type type)
        {
            // handle null values
            if ((value == null) && (type.IsValueType))
                return Activator.CreateInstance(type);

            // handle nullable types
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if ((value is string) && ((string)value == string.Empty))
                {
                    // value is an empty string, return null
                    return null;
                }
                else
                {
                    // value is not null
                    var nullableConverter = new NullableConverter(type);
                    type = nullableConverter.UnderlyingType;
                }
            }

            // handle exceptions
            if ((value is string) && (type == typeof(Guid)))
                return new Guid((string)value);
            if (type == typeof(object)) return value;

            // convert
            return Convert.ChangeType(value, type);
        }



        public class Properties
        {
            public const string HtmlEncode = "HtmlEncode";
            public const string StringFormat = "StringFormat";
            public const string RenderOnServer = "RenderOnServer";
        }


        /// <summary>
        /// Joins the paths.
        /// </summary>
        public static string JoinPaths(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1)) return path2;
            if (string.IsNullOrEmpty(path2)) return path1;
            return path1 + "." + path2;
        }
    }
}