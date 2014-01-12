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

        private RedwoodPropertyMetadata metadata;

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

        public RedwoodPropertyMetadata Metadata
        {
            get
            {
                return metadata;
            }
        }

        public static RedwoodProperty Register<TProp, TOwner>(string name, object defaultValue)
        {
            var meta = new RedwoodPropertyMetadata(defaultValue, RedwoodPropertyFlags.None);
            return Register<TProp, TOwner>(name, meta);
        }

        public static RedwoodProperty Register<TProp, TOwner>(string name, RedwoodPropertyMetadata metadata = null)
        {
            var prop = new RedwoodProperty()
            {
                id = RedwoodPropertyMap.Default.AssignNewId(),
                name = name,
                propertyType = typeof(TProp),
                ownerType = typeof(TOwner),
                metadata = metadata ?? new RedwoodPropertyMetadata(default(TProp))
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
