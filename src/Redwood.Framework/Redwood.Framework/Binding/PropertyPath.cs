using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class PropertyPath
    {
        private string path;

        public PropertyPath(string path)
        {
            this.path = path;
        }

        public string Path
        {
            get { return path; }
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
