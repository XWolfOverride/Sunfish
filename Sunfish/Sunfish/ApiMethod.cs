using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinWebXplorer2
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiMethod : Attribute
    {
        public ApiMethod(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
