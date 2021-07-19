using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Enum;

namespace Shares.Model
{
    public class RidingCenterSettingsModel
    {
        public RCFilter Filter { get; set; } = RCFilter.None;
        public int Price { get; set; } = 20;
        public int Duration { get; set; } = 30;
        public int Profit { get; set; } = 0;
    }
    
}
