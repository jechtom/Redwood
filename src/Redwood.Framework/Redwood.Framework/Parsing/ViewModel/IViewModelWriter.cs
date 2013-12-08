using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.ViewModel
{
    public interface IViewModelWriter
    {
        void WriteBeginFile();

        void WriteBeginClass(string name);

        void WriteProperty(string name, string type);

        void WriteReadOnlyProperty(string name, string type, string body);

        void WriteEndClass();

        void WriteEndFile();

        string GetOutput();

        void WriteFunction(string name, string[] paramDefinitions, string returnType, string body);
    }
}