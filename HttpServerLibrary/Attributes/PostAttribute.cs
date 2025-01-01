using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerLibrary.Attributes
{
    public class PostAttribute : Attribute
    {
        public string Route { get; }

        public PostAttribute(string route)
        {
            Route = route;
        }
    }
}
