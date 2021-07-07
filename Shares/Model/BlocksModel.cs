using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class Blocks
    {
        [JsonProperty("care-body-content")]
        public string CareBodyContent { get; set; }

        [JsonProperty("history-body-content")]
        public string HistoryBodyContent { get; set; }

        [JsonProperty("history-foot-content")]
        public string HistoryFootContent { get; set; }

        [JsonProperty("status-body-content")]
        public object StatusBodyContent { get; set; }

        [JsonProperty("hour-body-content")]
        public object HourBodyContent { get; set; }

        [JsonProperty("competition-body-content")]
        public string CompetitionBodyContent { get; set; }

        [JsonProperty("training-body-content")]
        public string TrainingBodyContent { get; set; }

        [JsonProperty("walk-body-content")]
        public string WalkBodyContent { get; set; }
    }
}
