using System;
using System.Collections.Generic;
using System.Linq;
using Shares;
using Shares.Model;

namespace HowrseBotClient
{
    public static class BotManager
    {
        private static List<HowrseBotModel> Bots = new List<HowrseBotModel>();

        public static void CreateBot(BotSettingsModel botSettings)
        {
            HowrseBotModel bot = new HowrseBotModel()
            {
                Id = Guid.NewGuid().ToString(),
                Settings = botSettings
            };

            Bots.Add(bot);
        }
        public static void StartBot(string botId)
        {
            HowrseBotModel bot = Bots.Single(_ => _.Id == botId);


        }
    }
}
