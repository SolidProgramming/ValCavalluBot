using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Enum;

namespace Shares.Model
{
    public class BotSettingsModel
    {
        public HowrseCredentialsModel Credentials = new();
        public HowrseTaskModel Actions = new();
        public List<HowrseBreedingModel> Breedings = new();
        public List<HowrseBreedingModel> ChosenBreedings = new();
        public string HowrseUserId { get; set; }
        public string Server { get; set; }
        public HowrseAccountType HowrseAccountType { get; set; }
         
    }
}
