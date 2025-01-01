using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateEngine
{
    public interface IHtmlTemplateEngine
    {
        public string Render(string template, string data);

        public string Render(string template, object obj);
    }
}
