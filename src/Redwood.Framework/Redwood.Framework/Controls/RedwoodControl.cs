using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Redwood.Framework.Generation;
using Redwood.Framework.Binding;

namespace Redwood.Framework.Controls
{
    public abstract class RedwoodControl : RedwoodBindable
    {
        public const string DefaultSchemaNamespaceName = "http://schemas.redwood/v1/rwhtml/controls";

        public object DataContext
        {
            get
            {
                return (object)GetValue(DataContextProperty);
            }
            set
            {
                SetValue(DataContextProperty, value);
            }
        }

        public static RedwoodProperty DataContextProperty = 
            RedwoodProperty.Register<object, RedwoodControl>("DataContext", new RedwoodPropertyMetadata(null, RedwoodPropertyFlags.IsInherited));

        public void OnAddedToParent(RedwoodControl parent)
        {
            SetParent(parent);
        }
    }
}
