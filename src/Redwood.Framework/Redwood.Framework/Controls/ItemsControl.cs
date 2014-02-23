using Redwood.Framework.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Controls
{
    public abstract class ItemsControl : RenderableControl
    {
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly RedwoodProperty ItemsSourceProperty = RedwoodProperty.Register<IEnumerable, ItemsControl>("ItemsSource");
    }
}
