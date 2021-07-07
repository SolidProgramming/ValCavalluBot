using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class AddClass
    {
        [JsonProperty("module-history")]
        public string ModuleHistory { get; set; }
    }
}
