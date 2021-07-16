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
using GRPCClient;

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
                bot.Status = BotClientStatus.Started;

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
                bot.CurrentAction = BotClientCurrentAction.PferdeSuchen;
                //foreach breeding
                //foreach horse in breeding

                List<string> breedingIds = bot.Settings.ChosenBreedings.Select(_ => _.ID).ToList();
                HorseCollectorResponseModel response = await GRPCClient.GRPCClient.GetHorsesFromBreedings(breedingIds, bot);

                foreach (string horseId in response.HorseIds)
                {
                    await PerformActions(horseId, bot);
                }


            });

            async Task PerformActions(string horseId, HowrseBotModel bot)
            {
                await ChangeHorse(horseId, bot);

                if (bot.Settings.Actions.Drink.PerformDrinkAction)
                {
                    await PerformDrinking(horseId, bot);
                }

                if (bot.Settings.Actions.Stroke.PerformStrokeAction)
                {
                    await PerformStroking(horseId, bot);
                }

                if (bot.Settings.Actions.Food.PerformFoodAction)
                {
                    await PerformFeeding(horseId, bot);
                }

                if (bot.Settings.Actions.Groom.PerformGroomAction)
                {
                    await PerformGrooming(horseId, bot);
                }

                if (bot.Settings.Actions.Carrot.PerformCarrotAction)
                {
                    await PerformGiveCarrot(horseId, bot);
                }

                if (bot.Settings.Actions.Mash.PerformMashAction)
                {
                    await PerformGiveMash(horseId, bot);
                }

                if (bot.Settings.Actions.Sleep.PerformRCRegistrationAction)
                {
                    await PerformRidingCenter(horseId, bot);
                }

                if (bot.Settings.Actions.Sleep.PerformSleepAction)
                {
                    await PerformSleeping(horseId, bot);
                }

                if (bot.Settings.Actions.Age.PerformAgingAction)
                {
                    await PerformAging(horseId, bot);
                }


            }

        }
        private static async Task PerformDrinking(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                HowrseAuthTokenModel authtokens = Tokens.GetHowrseAuthToken(bot.HTMLActions);
                HowrseTaskTokenModel taskTokens = Tokens.GetHowrseTaskToken(bot.HTMLActions);

                if (taskTokens.Drink.Length == 0) return;

                bot.CurrentAction = BotClientCurrentAction.Tränken;
                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsTakingCareButton();

                string endPoint = Endpoints.GetEndpoint(Endpoint.DrinkingAction, bot.Settings);
                string csrfToken = Tokens.GetCsrfToken(bot.HTMLActions);

                string postParam = (authtokens.Drink + "=" + csrfToken + "&" + taskTokens.Drink[0] + "=" + horseId + "&" + taskTokens.Drink[1] + "=" + coords.X + "&" + taskTokens.Drink[2] + "=" + coords.Y).ToLower();

                bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post(endPoint, postParam);
            });
        }
        private static async Task PerformStroking(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                HowrseAuthTokenModel authtokens = Tokens.GetHowrseAuthToken(bot.HTMLActions);
                HowrseTaskTokenModel taskTokens = Tokens.GetHowrseTaskToken(bot.HTMLActions);

                if (taskTokens.Stroke.Length == 0) return;

                bot.CurrentAction = BotClientCurrentAction.Streicheln;
                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsTakingCareButton();

                string endPoint = Endpoints.GetEndpoint(Endpoint.StrokingAction, bot.Settings);
                string csrfToken = Tokens.GetCsrfToken(bot.HTMLActions);

                string postParam = (authtokens.Stroke + "=" + csrfToken + "&" + taskTokens.Stroke[0] + "=" + horseId + "&" + taskTokens.Stroke[1] + "=" + coords.X + "&" + taskTokens.Stroke[2] + "=" + coords.Y).ToLower();

                bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post(endPoint, postParam);
            });
        }
        private static async Task PerformGrooming(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                HowrseAuthTokenModel authtokens = Tokens.GetHowrseAuthToken(bot.HTMLActions);
                HowrseTaskTokenModel taskTokens = Tokens.GetHowrseTaskToken(bot.HTMLActions);

                if (taskTokens.Groom.Length == 0) return;

                bot.CurrentAction = BotClientCurrentAction.Striegeln;
                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsTakingCareButton();

                string endPoint = Endpoints.GetEndpoint(Endpoint.GroomingAction, bot.Settings);
                string csrfToken = Tokens.GetCsrfToken(bot.HTMLActions);

                string postParam = (authtokens.Groom + "=" + csrfToken + "&" + taskTokens.Groom[0] + "=" + horseId + "&" + taskTokens.Groom[1] + "=" + coords.X + "&" + taskTokens.Groom[2] + "=" + coords.Y).ToLower();

                bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post(endPoint, postParam);
            });
        }
        private static async Task PerformGiveCarrot(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                HowrseAuthTokenModel authtokens = Tokens.GetHowrseAuthToken(bot.HTMLActions);
                HowrseTaskTokenModel taskTokens = Tokens.GetHowrseTaskToken(bot.HTMLActions);

                if (string.IsNullOrEmpty(authtokens.Carrot)) return;

                bot.CurrentAction = BotClientCurrentAction.Karotte;
                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsTakingCareButton();

                string endPoint = Endpoints.GetEndpoint(Endpoint.CarrotAction, bot.Settings);
                string csrfToken = Tokens.GetCsrfToken(bot.HTMLActions);

                string postParam = (authtokens.Carrot + "=" + csrfToken + "&id=" + horseId + "&friandise=carotte").ToLower();

                bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post(endPoint, postParam);
            });
        }
        private static async Task PerformGiveMash(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                HowrseAuthTokenModel authtokens = Tokens.GetHowrseAuthToken(bot.HTMLActions);
                HowrseTaskTokenModel taskTokens = Tokens.GetHowrseTaskToken(bot.HTMLActions);

                if (string.IsNullOrEmpty(authtokens.Mash)) return;

                bot.CurrentAction = BotClientCurrentAction.Mash;
                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsTakingCareButton();

                string endPoint = Endpoints.GetEndpoint(Endpoint.MashAction, bot.Settings);
                string csrfToken = Tokens.GetCsrfToken(bot.HTMLActions);

                string postParam = (authtokens.Mash + "=" + csrfToken + "&id=" + horseId + "&friandise=mash").ToLower();

                bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post(endPoint, postParam);
            });
        }
        private static async Task PerformAging(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                HowrseAuthTokenModel authtokens = Tokens.GetHowrseAuthToken(bot.HTMLActions);
                HowrseTaskTokenModel taskTokens = Tokens.GetHowrseTaskToken(bot.HTMLActions);

                if (taskTokens.Aging.Length == 0 || authtokens.Aging == string.Empty)
                {
                    authtokens = Tokens.GetAuthTokenFromAction(bot.HTMLActions);
                    taskTokens = Tokens.GetTaskTokenFromAction(bot.HTMLActions);
                }

                if (taskTokens.Aging.Length == 0 || authtokens.Aging == string.Empty) return;

                bot.CurrentAction = BotClientCurrentAction.Altern;
                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsTakingCareButton();

                int horseAge = Convert.ToInt16(Regex.Match(bot.HTMLActions.CurrentHtml, "var chevalAge = (\\d+);").Groups[1].Value);

                string endPoint = Endpoints.GetEndpoint(Endpoint.AgingAction, bot.Settings);
                string csrfToken = Tokens.GetCsrfToken(bot.HTMLActions);

                string postParam = (authtokens.Aging + "=" + csrfToken + "&" + taskTokens.Aging[0] + "=" + horseId + "&" + taskTokens.Aging[1] + "=" + horseAge + "&" + taskTokens.Aging[2] + "=" + coords.X + "&" + taskTokens.Aging[3] + "=" + coords.Y).ToLower();

                bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post(endPoint, postParam);
            });
        }
        private static async Task PerformFeeding(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                string currentHayAmount = "0";
                string currentOatsAmount = "0";
                int neededHayAmount = 0;
                int neededOatsAmount = 0;

                string[] neededFood = new string[2];
                string[] neededFood2 = new string[2];

                neededFood = Regex.Matches(bot.HTMLActions.CurrentHtml, "-target\">(\\d+)").Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
                neededFood2 = Regex.Matches(bot.HTMLActions.AfterActionHtml, "-target\\\\\">(\\d+)").Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

                if (neededFood2.Length > 1)
                {
                    if (Convert.ToInt32(neededFood[0]) < Convert.ToInt32(neededFood2[0]) || Convert.ToInt32(neededFood[1]) < Convert.ToInt32(neededFood2[1]))
                    {
                        neededFood = neededFood2;
                    }
                }

                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsFeedingButton();

                string hayToken = Regex.Match(bot.HTMLActions.CurrentHtml, "type=\"hidden\" name=\"feeding(.*?)\" value=").Groups[1].Value;//Helper.GetBetween(html, "type=\"hidden\" name=\"feeding", "\" value=\"");
                string oatToken = Regex.Match(bot.HTMLActions.CurrentHtml, "oatsSlider-sliderHidden\" type=\"hidden\" name=\"feeding(.*?)\" value=").Groups[1].Value;//Helper.GetBetween(html, "oatsSlider-sliderHidden\" type=\"hidden\" name=\"feeding", "\" value=\"");

                string csrfToken = Tokens.GetCsrfToken(bot.HTMLActions);

                HowrseAuthTokenModel authtokens = Tokens.GetHowrseAuthToken(bot.HTMLActions);
                HowrseTaskTokenModel taskTokens = Tokens.GetHowrseTaskToken(bot.HTMLActions);

                int horseAge = Convert.ToInt16(Regex.Match(bot.HTMLActions.CurrentHtml, "var chevalAge = (\\d+);").Groups[1].Value);

                if (neededFood.Length == 2)
                {

                    if (bot.Settings.Actions.Food.ActionMode == HowrseActionMode.Auto)
                    {
                        currentHayAmount = Regex.Match(bot.HTMLActions.CurrentHtml, "fourrage-quantity\"> (\\d+) / <strong class=\"section-fourrage").Groups[1].Value;
                        neededHayAmount = Convert.ToInt32(neededFood[0]) - Convert.ToInt32(currentHayAmount);

                        currentOatsAmount = Regex.Match(bot.HTMLActions.CurrentHtml, "avoine-quantity\"> (\\d+) / <strong class=\"section-avoine section-avoine").Groups[1].Value;
                        neededOatsAmount = Convert.ToInt32(neededFood[1]) - Convert.ToInt32(currentOatsAmount);
                    }
                    else if (bot.Settings.Actions.Food.ActionMode == HowrseActionMode.Manual)
                    {
                        neededHayAmount = Convert.ToInt16(bot.Settings.Actions.Food.AmountofHay);
                        currentHayAmount = Regex.Match(bot.HTMLActions.CurrentHtml, "fourrage-quantity\"> (\\d+) / <strong class=\"section-fourrage").Groups[1].Value;
                        neededHayAmount -= Convert.ToInt16(currentHayAmount);

                        neededOatsAmount = Convert.ToInt16(bot.Settings.Actions.Food.AmountofOat);
                        currentOatsAmount = Regex.Match(bot.HTMLActions.CurrentHtml, "avoine-quantity\"> (\\d+) / <strong class=\"section-avoine section-avoine").Groups[1].Value;
                        neededOatsAmount -= Convert.ToInt16(currentOatsAmount);
                    }
                    else
                    {
                        // System.Windows.Forms.MessageBox.Show("Fehler: Futter Param Action Mode = NULL!(HAY&OAT)");
                    }


                    if (neededHayAmount > 0 && neededOatsAmount > 0)
                    {
                        bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post("https://" + bot.Settings.Server + "/elevage/chevaux/doEat", (authtokens.Feeding + "=" + csrfToken + "&" + taskTokens.Feeding[0] + "=" + horseId + "&" + taskTokens.Feeding[1] + "=" + coords.X + "&" + taskTokens.Feeding[2] + "=" + coords.Y + "&" + hayToken + "=" + neededHayAmount + "&" + oatToken + "=" + neededOatsAmount).ToLower());

                    }
                    else if (neededHayAmount <= 0 && neededOatsAmount > 0)
                    {
                        bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post("https://" + bot.Settings.Server + "/elevage/chevaux/doEat", (authtokens.Feeding + "=" + csrfToken + "&" + taskTokens.Feeding[0] + "=" + horseId + "&" + taskTokens.Feeding[1] + "=" + coords.X + "&" + taskTokens.Feeding[2] + "=" + coords.Y + "&" + hayToken + "=0&" + oatToken + "=" + neededOatsAmount).ToLower());

                    }
                    else if (neededHayAmount > 0 && neededOatsAmount <= 0)
                    {
                        bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post("https://" + bot.Settings.Server + "/elevage/chevaux/doEat", (authtokens.Feeding + "=" + csrfToken + "&" + taskTokens.Feeding[0] + "=" + horseId + "&" + taskTokens.Feeding[1] + "=" + coords.X + "&" + taskTokens.Feeding[2] + "=" + coords.Y + "&" + hayToken + "=" + neededHayAmount + "&" + oatToken + "=0").ToLower());

                    }

                }
                else if (neededFood.Length == 1)
                {
                    currentHayAmount = Regex.Match(bot.HTMLActions.CurrentHtml, "fourrage-quantity\"> (\\d+)/ <strong class=\"section-fourrage").Groups[1].Value;

                    if (bot.Settings.Actions.Food.ActionMode == HowrseActionMode.Auto)
                    {
                        //currentHayAmount = Helper.GetBetween(currentHtml, "fourrage-quantity\"> ", "/ <strong class=\"section-fourrage");
                        neededHayAmount = Convert.ToInt16(neededFood[0]) - Convert.ToInt16(currentHayAmount);
                    }
                    else if (bot.Settings.Actions.Food.ActionMode == HowrseActionMode.Manual)
                    {
                        neededHayAmount = Convert.ToInt16(bot.Settings.Actions.Food.AmountofHay);
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show("Fehler: Futter Param Action = NULL!(HAY)");
                    }

                    if (neededHayAmount > 0)
                    {
                        bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post("https://" + bot.Settings.Server + "/elevage/chevaux/doEat", (authtokens.Feeding + "=" + csrfToken + "&" + taskTokens.Feeding[0] + "=" + horseId + "&" + taskTokens.Feeding[1] + "=" + coords.X + "&" + taskTokens.Feeding[2] + "=" + coords.Y + "&" + hayToken + "=" + neededHayAmount).ToLower());

                    }

                }
                else if (neededFood.Length == 0 && Convert.ToInt16(horseAge) < 6 && bot.Settings.Actions.Food.PerformFoodAction)
                {
                    if (authtokens.Suckle == string.Empty || taskTokens.Suckle.Length == 0)
                        return;

                    bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post("https://" + bot.Settings.Server + "/elevage/chevaux/doSuckle", (authtokens.Suckle + "=" + csrfToken + "&" + taskTokens.Suckle[0] + "=" + horseId).ToLower());

                }


            });
        }
        private static async Task PerformSleeping(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                HowrseAuthTokenModel authtokens = Tokens.GetHowrseAuthToken(bot.HTMLActions);
                HowrseTaskTokenModel taskTokens = Tokens.GetHowrseTaskToken(bot.HTMLActions);

                if (taskTokens.Aging.Length == 0 || authtokens.Aging == string.Empty)
                {
                    authtokens = Tokens.GetAuthTokenFromAction(bot.HTMLActions);
                    taskTokens = Tokens.GetTaskTokenFromAction(bot.HTMLActions);
                }

                if (taskTokens.Aging.Length == 0 || authtokens.Aging == string.Empty) return;


            });
        }
        private static async Task PerformRidingCenter(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                if (!bot.HTMLActions.CurrentHtml.Contains("elevage/chevaux/centreInscription?id=")) return;

                bot.CurrentAction = BotClientCurrentAction.Einstallen;
                RidingCenterModel ridingCenter = RidingCenter.GetRidingCenter(horseId, bot);
                RidingCenter.DoRegistration(horseId, ridingCenter, bot);
                //TODO: RC Name in action log
            });
        }
        private static async Task ChangeHorse(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                bot.HTMLActions.CurrentHtml = string.Empty;
                bot.HTMLActions.AfterActionHtml = string.Empty;
                bot.CurrentAction = BotClientCurrentAction.PferdWechseln;
                string endPoint = Endpoints.GetEndpoint(Endpoint.Horse, bot.Settings);
                endPoint += $"cheval?id={horseId}";

                string html = bot.OwlientConnection.Get(endPoint);

                bot.HTMLActions.CurrentHtml = html;
            });
        }
    }
}
