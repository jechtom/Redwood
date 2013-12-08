using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.ViewModel
{
    public class ClientImplementationAttribute : Attribute
    {

        public string Expression { get; set; }

        public ClientImplementationAttribute(string expression)
        {
            Expression = expression;
        }

    }
}
