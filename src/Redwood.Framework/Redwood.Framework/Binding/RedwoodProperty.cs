using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class RedwoodProperty
    {
        private int id;

        private string name;

        private Type propertyType;

        private Type ownerType;

        private bool isInherited;

        private object defaultValue;

        public int Id
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Type PropertyType
        {
            get
            {
                return propertyType;
            }
        }

        public Type OwnerType
        {
            get
            {
                return ownerType;
            }
        }

        public bool IsInherited
        {
            get
            {
                return isInherited;
            }
        }

        public object DefaultValue
        {
            get
            {
                return defaultValue;
            }
        }

        public static RedwoodProperty Register<TProp, TOwner>(string name, TProp defaultValue = default(TProp), bool isInherited = false)
        {
            var prop = new RedwoodProperty()
            {
                id = RedwoodPropertyMap.Default.AssignNewId(),
                name = name,
                propertyType = typeof(TProp),
                ownerType = typeof(TOwner),
                defaultValue = defaultValue,
                isInherited = isInherited
            };

            RedwoodPropertyMap.Default.RegisterProperty(prop);

            return prop;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}) on {2}", name, propertyType.Name, ownerType.Name);
        }
    }
}
