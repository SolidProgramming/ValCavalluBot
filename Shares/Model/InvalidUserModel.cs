using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public struct InvalidUser
    {
        public string text { get; set; }
        public List<string> position { get; set; }
        public string highlight { get; set; }
        public List<object> values { get; set; }
    }
}
