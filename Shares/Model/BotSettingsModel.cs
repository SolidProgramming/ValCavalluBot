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
        public List<HowrseBreedingModel> ChosenBreedingsApler = new();
        public List<HowrseBreedingModel> ChosenBreedingsBreed = new();
        public List<HowrseBreedingModel> TempChosenBreedingsApler = new();
        public List<HowrseBreedingModel> TempChosenBreedingsBreed = new();
        public RidingCenterSettingsModel RidingCenterSettings = new();
        public string HowrseUserId { get; set; }
        public string Server { get; set; }
        public HowrseAccountType HowrseAccountType { get; set; }
         
    }
}
