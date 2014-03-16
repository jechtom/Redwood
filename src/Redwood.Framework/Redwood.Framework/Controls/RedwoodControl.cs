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
            get { return GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }
        public static RedwoodProperty DataContextProperty = 
            RedwoodProperty.Register<object, RedwoodControl>("DataContext", new RedwoodPropertyMetadata(null, RedwoodPropertyFlags.IsInherited));

        

        public string ID
        {
            get { return (string)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }
        public static RedwoodProperty IDProperty = RedwoodProperty.Register<string, RedwoodControl>("ID", new RedwoodPropertyMetadata(""));



        public void OnAddedToParent(RedwoodControl parent)
        {
            SetParent(parent);
        }


        /// <summary>
        /// Ensures that the control has some ID. If not, it generates a random one.
        /// </summary>
        protected void EnsureHasID()
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = Guid.NewGuid().ToString().Replace("-", "");
            }
        }
    }
}
