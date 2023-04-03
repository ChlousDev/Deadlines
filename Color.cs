using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deadlines
{
    public class Color
    {
        public string DisplayName { get; set; }
        public string Code { get; set; }

        public Color(string name, string code)
        {
            this.DisplayName = name;
            this.Code = code;
        }
    }
}
