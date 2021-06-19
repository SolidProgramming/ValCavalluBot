using Shares.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class HowrseActionParamFoodModel
    {
        public HowrseActionParamFoodModel()
        {
            AmountofHay = 0;
            AmountofOat = 0;
            ActionMode = HowrseActionMode.Auto;
        }

        public bool PerformFoodAction { get; set; }
        // public boolean doHay { get; set; }
        //public boolean doOat { get; set; }

        public HowrseActionMode ActionMode { get; set; }

        public int AmountofHay { get; set; }
        public int AmountofOat { get; set; }
    }
}
