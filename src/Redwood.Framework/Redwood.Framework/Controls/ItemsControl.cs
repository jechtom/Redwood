using Redwood.Framework.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public abstract class ItemsControl : RenderableControl
    {
        public RedwoodTemplate ItemTemplate
        {
            get
            {
                return (RedwoodTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        public static readonly RedwoodProperty ItemTemplateProperty = RedwoodProperty.Register<RedwoodTemplate, ItemsControl>("ItemTemplate");

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly RedwoodProperty ItemsSourceProperty = RedwoodProperty.Register<IEnumerable, ItemsControl>("ItemsSource");
    }
}
