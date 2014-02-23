using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Binding;

namespace Redwood.Framework.Controls
{
    public abstract class TemplatedItemsControl : ItemsControl
    {

        public RedwoodTemplate ItemTemplate
        {
            get { return (RedwoodTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public static readonly RedwoodProperty ItemTemplateProperty = RedwoodProperty.Register<RedwoodTemplate, ItemsControl>("ItemTemplate");

    }
}