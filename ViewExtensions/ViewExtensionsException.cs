using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewExtensions
{
    public class ViewExtensionsException: Exception
    {
        public ViewExtensionsException(string message) : base("ViewExtensions - " + message) { }
    }
}
