using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shares;
using Shares.Model;
using HowrseBotClient;

namespace ValCavalluBot.Services
{
    public class BotManagerService : IBotManagerService
    {
        public void CreateBot(BotSettingsModel botSettings)
        {
            BotManager.CreateBot(botSettings);
        }

        public void DeleteBot(string botId)
        {
            BotManager.DeleteBot(botId);
        }

        public HowrseBotModel GetBot(string botId)
        {
            return BotManager.GetBot(botId);
        }

        public List<HowrseBotModel> GetBots()
        {
            return BotManager.GetBots();
        }

        public void StartBot(string botId)
        {
            BotManager.StartBot(botId);
        }
    }
}
