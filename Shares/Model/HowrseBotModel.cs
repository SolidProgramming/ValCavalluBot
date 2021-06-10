using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Enum;

namespace Shares.Model
{
    public class HowrseBotModel
    {
        public BotSettingsModel Settings = new();
        public string Id { get; set; }
        public BotClientStatusModel Status { get; set; }
    }
}
