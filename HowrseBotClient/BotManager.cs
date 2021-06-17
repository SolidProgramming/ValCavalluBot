using System;
using System.Collections.Generic;
using System.Linq;
using Shares;
using Shares.Model;
using Shares.Enum;
using System.Web;
using System.Threading.Tasks;

namespace HowrseBotClient
{
    public static class BotManager
    {
        private static List<HowrseBotModel> Bots = new List<HowrseBotModel>();

        public static void CreateBot(BotSettingsModel botSettings)
        {
            botSettings.Credentials.HowrsePassword = HttpUtility.UrlEncode(botSettings.Credentials.HowrsePassword);

            HowrseBotModel bot = new HowrseBotModel()
            {
                Id = Guid.NewGuid().ToString(),
                Settings = botSettings
            };

            Bots.Add(bot);
        }
        public static void DeleteBot(string botId)
        {
            Bots.RemoveAll(_ => _.Id == botId);
            SettingsHandler.SaveSettings(Bots, FileType.BotSettings);
        }
        public static async Task StartBot(string botId)
        {
            HowrseBotModel bot = Bots.Single(_ => _.Id == botId);

            bot.OnLoginFailed += Bot_OnLoginFailed;
            bot.OnLoginSuccessful += Bot_OnLoginSuccessful;
            bot.OnBotStatusChanged += Bot_OnBotStatusChanged;

            await bot.Login();
        }
        private static void Bot_OnBotStatusChanged(BotClientStatus obj)
        {
            //TODO: tbc
        }
        private static void Bot_OnLoginSuccessful()
        {
            Console.WriteLine("Logged in!");
        }
        private static void Bot_OnLoginFailed(HowrseServerLoginResponseModel obj)
        {
            Console.WriteLine("Login failed: " + obj.errorsText);
        }
        public static List<HowrseBotModel> GetBots()
        {
            if (Bots.Count == 0)
            {
                List<BotSettingsModel> botSettings = SettingsHandler.LoadSettings<List<BotSettingsModel>>(FileType.BotSettings);

                if (botSettings is null) return null;

                foreach (BotSettingsModel botSetting in botSettings)
                {
                    CreateBot(botSetting);
                }
            }
            return Bots;
        }
        public static HowrseBotModel GetBot(string botId)
        {
            return Bots.Single(_ => _.Id == botId);
        }
        public static void AddTaskToQue(string botId, HowrseTaskModel howrseTask)
        {
            HowrseBotModel bot = Bots.Single(_ => _.Id == botId);

            bot.AddToQue(howrseTask);
        }
    }
}
