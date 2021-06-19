using Shares.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class HowrseActionParamTrainingModel
    {
        public HowrseActionParamTrainingModel()
        {
            ActionMode = HowrseActionMode.Auto;
        }

        public HowrseActionMode ActionMode { get; set; }
        public bool PerformTrainingAction { get; set; }
        public List<TrainingType> TrainingTypes { get; set; }
    }
}
