using System;
using System.Collections.Generic;
using System.Linq;
using Shares;
using Shares.Model;
using Shares.Enum;
using System.Web;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

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
            HowrseBotModel bot = GetBot(botId);

            bot.OnBotStatusChanged += Bot_OnBotStatusChanged;

            bot.Status = BotClientStatus.Started;
            bot.CurrentAction = BotClientCurrentAction.Login;

            await Login(bot);
        }
        public static async Task StopBot(string botId)
        {
            HowrseBotModel bot = GetBot(botId);

            await Logout(bot);

            bot.Status = BotClientStatus.Stopped;
            bot.CurrentAction = BotClientCurrentAction.Keine;
        }
        private static void Bot_OnBotStatusChanged(BotClientStatus status)
        {
            
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
        public static async Task Login(HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                string csrf = string.Empty;
                string auth_token = string.Empty;
                string html = string.Empty;
                string sid = string.Empty;

                html = Connection.Get("https://" + bot.Settings.Server + "/");

                //tolower?
                auth_token = Regex.Match(html, "id=\"authentification(.{5})\" type").Groups[1].Value.ToLower();
                csrf = Regex.Match(html, "value=\"(.{32})\" name=").Groups[1].Value.ToLower();
                
                string serverResponse = Connection.Post("https://" + bot.Settings.Server + "/site/doLogIn", auth_token + "=" + csrf + "&login=" + bot.Settings.Credentials.HowrseUsername + "&password=" + bot.Settings.Credentials.HowrsePassword + "&redirection=&isBoxStyle=");

                HowrseServerLoginResponseModel howrseServerLoginResponse = JsonConvert.DeserializeObject<HowrseServerLoginResponseModel>(serverResponse);

                if (howrseServerLoginResponse.errors.Count > 0)
                {
                    bot.Status = BotClientStatus.Error;
                    bot.CurrentAction = BotClientCurrentAction.Keine;
                    return;
                }

                Connection.Get("https://" + bot.Settings.Server + "/jeu/?identification=1&redirectionMobile=yes");
                html = Connection.Get("https://" + bot.Settings.Server + "/jeu/");

                if (html.Contains("Equus"))
                {
                    bot.HowrseUserId = Regex.Match(html, "href=\"/joueur/fiche/\\?id=(\\d+)\"><span class").Groups[1].Value;
                    bot.SID = Regex.Match(html, "sid=(.*?)'}\\)\\);").Groups[1].Value;

                    if (html.Contains("/header/logo/vip/"))
                    {
                        bot.Settings.HowrseAccountType = HowrseAccountType.VIP;
                    }
                    else if (html.Contains("/header/logo/pegase"))
                    {

                        bot.Settings.HowrseAccountType = HowrseAccountType.Pegasus;
                    }
                    else
                    {
                        bot.Settings.HowrseAccountType = HowrseAccountType.Normal;
                    }
                }
                else
                {
                    bot.Status = BotClientStatus.Error;
                    bot.CurrentAction = BotClientCurrentAction.Keine;
                }
            });
        }
        public static async Task Logout(HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                bot.CurrentAction = BotClientCurrentAction.Logout;
                Connection.Post("https://www.howrse.de/site/doLogOut", $"sid={bot.SID}");               
            });
        }
        private static async Task PerformActions()
        {

        }
    }
}
