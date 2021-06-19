using Shares.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class HowrseActionParamRidingModel
    {
        public HowrseActionParamRidingModel()
        {
            ActionMode = HowrseActionMode.Auto;
        }
        public HowrseRidingType ridingType { get; set; }
        public HowrseActionMode ActionMode { get; set; }
        public bool PerformRidingAction { get; set; }
    }
}
