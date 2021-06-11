using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shares;
using Shares.Enum;
using Shares.Model;

namespace ValCavalluBot.Services
{
    public class SettingsService : ISettingsService
    {
        public BotSettingsModel LoadBotSettings()
        {
            return SettingsHandler.LoadSettings<BotSettingsModel>(FileType.BotSettings);
        }

        public GeneralSettingsModel LoadGeneralSettings()
        {
            return SettingsHandler.LoadSettings<GeneralSettingsModel>(FileType.GeneralSettings);
        }

        public List<HowrseBotModel> LoadHowrseBotsSettings()
        {
            return SettingsHandler.LoadSettings<List<HowrseBotModel>>(FileType.BotSettings);
        }

        public void SaveBotSettings(List<BotSettingsModel> botSettings)
        {
            SettingsHandler.SaveSettings(botSettings, FileType.BotSettings);
        }

        public void SaveGeneralSettings(GeneralSettingsModel generalSettings)
        {
            SettingsHandler.SaveSettings(generalSettings, FileType.GeneralSettings);
        }
    }
}
