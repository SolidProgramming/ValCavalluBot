using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shares.Model;
using Shares;

namespace ValCavalluBot.Services
{
    interface ISettingsService
    {       
        void SaveBotSettings(List<BotSettingsModel> botSettings);
        void SaveGeneralSettings(GeneralSettingsModel generalSettings);
        List<HowrseBotModel> LoadHowrseBotsSettings();
        GeneralSettingsModel LoadGeneralSettings();
        BotSettingsModel LoadBotSettings();

    }
}
