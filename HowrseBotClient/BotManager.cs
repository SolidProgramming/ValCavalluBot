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
using HowrseBotClient.Model;
using HowrseBotClient.Enum;
using HowrseBotClient.Class;

namespace HowrseBotClient
{
    public static class BotManager
    {
        private static List<HowrseBotModel> Bots = new List<HowrseBotModel>();
        

        public static HowrseBotModel CreateBot(BotSettingsModel botSettings)
        {            
            HowrseBotModel bot = new HowrseBotModel()
            {
                Id = Guid.NewGuid().ToString(),
                Settings = botSettings
            };

            Bots.Add(bot);

            return bot;
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

            bool success = await Login(bot);

            if (!success) return;

            await PerformActions(bot);

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
        private static async Task<bool> Login(HowrseBotModel bot)
        {
            return await Task.Run(() =>
            {
                string csrf = string.Empty;
                string auth_token = string.Empty;
                string html = string.Empty;
                string sid = string.Empty;

                html = bot.OwlientConnection.Get("https://" + bot.Settings.Server + "/");

                if (string.IsNullOrEmpty(html)) return false;
               
                auth_token = Regex.Match(html, "id=\"authentification(.{5})\" type").Groups[1].Value.ToLower();
                csrf = Regex.Match(html, "value=\"(.{32})\" name=").Groups[1].Value.ToLower();

                string serverResponse = bot.OwlientConnection.Post("https://" + bot.Settings.Server + "/site/doLogIn", auth_token + "=" + csrf + "&login=" + bot.Settings.Credentials.HowrseUsername + "&password=" + bot.Settings.Credentials.HowrsePassword + "&redirection=&isBoxStyle=");

                HowrseServerLoginResponseModel howrseServerLoginResponse = JsonConvert.DeserializeObject<HowrseServerLoginResponseModel>(serverResponse);

                if (howrseServerLoginResponse.Errors.Count > 0)
                {
                    bot.Status = BotClientStatus.Error;
                    bot.CurrentAction = BotClientCurrentAction.Keine;
                    return false;
                }

                bot.OwlientConnection.Get("https://" + bot.Settings.Server + "/jeu/?identification=1&redirectionMobile=yes");
                html = bot.OwlientConnection.Get("https://" + bot.Settings.Server + "/jeu/");

                if (html.Contains("Equus"))
                {
                    bot.Settings.HowrseUserId = Regex.Match(html, "href=\"/joueur/fiche/\\?id=(\\d+)\"><span class").Groups[1].Value;
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

                    return true;
                }
                else
                {
                    bot.Status = BotClientStatus.Error;
                    bot.CurrentAction = BotClientCurrentAction.Keine;
                    return false;
                }
            });
        }
        public static async Task<bool> LoginTest(HowrseBotModel bot)
        {
            return await Task.Run(() =>
            {
                string csrf = string.Empty;
                string auth_token = string.Empty;
                string html = string.Empty;
                string sid = string.Empty;

                Connection owlientConnection = new();

                html = owlientConnection.Get("https://" + bot.Settings.Server + "/");

                if (string.IsNullOrEmpty(html)) return false;

                auth_token = Regex.Match(html, "id=\"authentification(.{5})\" type").Groups[1].Value.ToLower();
                csrf = Regex.Match(html, "value=\"(.{32})\" name=").Groups[1].Value.ToLower();

                string serverResponse = owlientConnection.Post("https://" + bot.Settings.Server + "/site/doLogIn", auth_token + "=" + csrf + "&login=" + bot.Settings.Credentials.HowrseUsername + "&password=" + bot.Settings.Credentials.HowrsePassword + "&redirection=&isBoxStyle=");

                HowrseServerLoginResponseModel howrseServerLoginResponse = JsonConvert.DeserializeObject<HowrseServerLoginResponseModel>(serverResponse);

                if (howrseServerLoginResponse.Errors.Count > 0)
                {                   
                    return false;
                }

                owlientConnection.Get("https://" + bot.Settings.Server + "/jeu/?identification=1&redirectionMobile=yes");
                html = owlientConnection.Get("https://" + bot.Settings.Server + "/jeu/");

                if (html.Contains("Equus"))
                {
                    bot.Settings.HowrseUserId = Regex.Match(html, "href=\"/joueur/fiche/\\?id=(\\d+)\"><span class").Groups[1].Value;
                    bot.SID = Regex.Match(html, "sid=(.*?)'}\\)\\);").Groups[1].Value;
                    return true;
                }
                else
                {                   
                    return false;
                }
            });
        }
        private static async Task Logout(HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                bot.CurrentAction = BotClientCurrentAction.Logout;
                bot.OwlientConnection.Post("https://www.howrse.de/site/doLogOut", $"sid={bot.SID}");
            });
        }
        private static async Task PerformActions(HowrseBotModel bot)
        {
            await Task.Run(async () =>
            {
                //foreach breeding
                //foreach horse in breeding

                bot.Status = BotClientStatus.Started;

                await ChangeHorse("", bot);

                if (bot.Settings.Actions.Drink.PerformDrinkAction)
                {
                    await PerformDrinking(bot);
                }

                if (bot.Settings.Actions.Stroke.PerformStrokeAction)
                {

                }

                if (bot.Settings.Actions.Food.PerformFoodAction)
                {

                }

                if (bot.Settings.Actions.Groom.PerformGroomAction)
                {

                }

                if (bot.Settings.Actions.Carrot.PerformCarrotAction)
                {

                }

                if (bot.Settings.Actions.Mash.PerformMashAction)
                {

                }
            });

        }
        private static async Task PerformDrinking(HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                bot.CurrentAction = BotClientCurrentAction.Tränken;
                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsTakingCareButton();

                string endPoint = Endpoints.GetEndpoint(Endpoint.DrinkingAction, bot.Settings);
                               
                //AfterActionHtmlResponse = Connection.Post(endPoint, (AuthToken.Stroke + "=" + GetCsrfToken() + "&" + TaskToken.Stroke[0] + "=" + horseId + "&" + TaskToken.Stroke[1] + "=" + xClickCoord + "&" + TaskToken.Stroke[2] + "=" + yClickCoord).ToLower());
            });
        }
        private static async Task ChangeHorse(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                bot.CurrentAction = BotClientCurrentAction.PferdWechseln;
                string endPoint = Endpoints.GetEndpoint(Endpoint.Horse, bot.Settings);
                endPoint += $"cheval?id={horseId}";

                bot.OwlientConnection.Get(endPoint);
            });
        }
    }
}
