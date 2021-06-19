using Shares.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class HowrseActionParamPlayingModel
    {
        public HowrseActionParamPlayingModel()
        {
            ActionMode = HowrseActionMode.Auto;
        }

        public bool PerformPlayAction { get; set; }
        public HowrseActionMode ActionMode { get; set; }
    }
}
