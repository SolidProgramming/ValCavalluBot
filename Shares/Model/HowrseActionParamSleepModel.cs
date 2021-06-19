using Shares.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class HowrseActionParamSleepModel
    {
        public HowrseActionParamSleepModel()
        {
            ActionMode = HowrseActionMode.Auto;
        }

        public HowrseActionMode ActionMode { get; set; }
        public bool PerformRCRegistrationAction { get; set; }
        public bool PerformSleepAction { get; set; }
        public bool StoreInMyBoxes { get; set; }
        public short Duration { get; set; }
        public short Price { get; set; }
    }
}
