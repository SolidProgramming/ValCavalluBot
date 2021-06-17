using Shares.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValCavalluBot.Data.Model
{
    public class HowrseActionCheckboxModel
    {
        public string ImageUrl { get; set; }
        public bool Checked { get; set; }
        public HowrseTaskType TaskType { get; set; }
    }
}
