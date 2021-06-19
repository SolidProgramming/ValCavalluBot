using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Enum;

namespace Shares.Model
{
    public class HowrseTaskModel
    {
        public string Name { get; set; }
        public HowrseTaskType TaskType { get; set; }
        public HowrseActionMode Mode { get; set; }

        public HowrseActionParamRidingModel Riding = new();
        public HowrseActionParamFoodModel Food = new();
        public HowrseActionParamDrinkModel Drink = new();
        public HowrseActionParamStrokeModel Stroke = new();
        public HowrseActionParamGroomModel Groom = new();
        public HowrseActionParamCarrotModel Carrot = new();
        public HowrseActionParamMashModel Mash = new();
        public HowrseActionParamSleepModel Sleep = new();
        public HowrseActionParamMissionModel Mission = new();
        public HowrseActionParamAgingModel Age = new();
        public HowrseActionParamTrainingModel Training = new();
        public HowrseActionParamPlayingModel Playing = new();

    }
}
