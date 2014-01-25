using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Redwood.Framework.Hosting
{
    public class RedwoodRequestContext
    {

        public IOwinContext OwinContext { get; set; }
        
        public RedwoodPresenter Presenter { get; set; }

        public string ApplicationPhysicalPath { get; set; }


    }
}
