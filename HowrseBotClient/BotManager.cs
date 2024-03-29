﻿using System;
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
using HtmlAgilityPack;
using System.Threading;
using System.Collections.Concurrent;

namespace HowrseBotClient
{
    public static class BotManager
    {
        private static List<HowrseBotModel> Bots = new();
        private static GeneralSettingsModel GeneralSettings = new();
        public static event Action<string> OnHorseSpriteChanged;
        public static event Action<HowrseUserModel> OnHowrseUserInfoChanged;

        public static HowrseBotModel CreateBot(BotSettingsModel botSettings)
        {
            
            HowrseBotModel bot = new()
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

            if (!await Login(bot)) return;

            HowrseUserModel howrseUser = await HowrseUser.GetUserInfo(bot);
            OnHowrseUserInfoChanged(howrseUser);

            await PerformActions(bot);

            bot.CurrentAction = BotClientCurrentAction.Keine;
        }
        public static async Task StopBot(string botId)
        {
            HowrseBotModel bot = GetBot(botId);

            await Logout(bot);

            bot.Status = BotClientStatus.Stopped;
            bot.CurrentAction = BotClientCurrentAction.Keine;
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
                bot.CurrentAction = BotClientCurrentAction.Login;

                string csrf = string.Empty;
                string auth_token = string.Empty;
                string html = string.Empty;
                string sid = string.Empty;

                html = bot.OwlientConnection.Get("https://" + bot.Settings.Server + "/");

                if (string.IsNullOrEmpty(html)) return false;

                auth_token = Regex.Match(html, "id=\"authentification(.{5})\" type").Groups[1].Value.ToLower();
                csrf = Regex.Match(html, "value=\"(.{32})\" name=").Groups[1].Value.ToLower();

                string serverResponse = bot.OwlientConnection.Post("https://" + bot.Settings.Server + "/site/doLogIn", auth_token + "=" + csrf + "&login=" + bot.Settings.Credentials.HowrseUsername.ToUrlEncode() + "&password=" + bot.Settings.Credentials.HowrsePassword.ToUrlEncode() + "&redirection=&isBoxStyle=");

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

                string serverResponse = owlientConnection.Post("https://" + bot.Settings.Server + "/site/doLogIn", auth_token + "=" + csrf + "&login=" + bot.Settings.Credentials.HowrseUsername.ToUrlEncode() + "&password=" + bot.Settings.Credentials.HowrsePassword.ToUrlEncode() + "&redirection=&isBoxStyle=");

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

                List<string> breedingIds = bot.Settings.ChosenBreedingsApler.Select(_ => _.ID).ToList();
                HorseCollectorResponseModel response = await GRPCClient.GRPCClient.GetHorsesFromBreedings(breedingIds, bot);

                foreach (string horseId in response.HorseIds)
                {
                    await PerformActions(horseId, bot);
                }

            });

            static async Task PerformActions(string horseId, HowrseBotModel bot)
            {
                GeneralSettings = SettingsHandler.LoadSettings<GeneralSettingsModel>(FileType.GeneralSettings);

                if (GeneralSettings is null)
                {
                    GeneralSettings = new();
                }

                await ChangeHorse(horseId, bot);

                if (bot.Settings.Actions.Drink.PerformDrinkAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformDrinking(horseId, bot);
                }

                if (bot.Settings.Actions.Stroke.PerformStrokeAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformStroking(horseId, bot);
                }

                if (bot.Settings.Actions.Sleep.PerformRCRegistrationAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformRidingCenter(horseId, bot);
                }

                if (bot.Settings.Actions.Mission.PerformMissionAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformMission(horseId, bot);
                }

                if (bot.Settings.Actions.Food.PerformFoodAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformFeeding(horseId, bot);
                }

                if (bot.Settings.Actions.Groom.PerformGroomAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformGrooming(horseId, bot);
                }

                if (bot.Settings.Actions.Carrot.PerformCarrotAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformGiveCarrot(horseId, bot);
                }

                if (bot.Settings.Actions.Mash.PerformMashAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformGiveMash(horseId, bot);
                }

                if (bot.Settings.Actions.Sleep.PerformSleepAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformSleeping(horseId, bot);
                }

                if (bot.Settings.Actions.Age.PerformAgingAction)
                {
                    await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                    await PerformAging(horseId, bot);
                }
            }

            bot.CurrentAction = BotClientCurrentAction.Keine;
            bot.Status = BotClientStatus.Stopped;

        }
        private static async Task PerformMission(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                if (!bot.HTMLActions.CurrentHtml.Contains("module-item\" id=\"mission") || bot.Horse.Age < 24) return;

                string endPoint = Endpoints.GetEndpoint(Endpoint.MissionAction, bot.Settings);
                string postParam = $"id={horseId}".ToLower();

                bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post(endPoint, postParam);

                Class.HorseStatus.Parse<decimal>(bot, true);
            });
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

                Class.HorseStatus.Parse<decimal>(bot, true);
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

                Class.HorseStatus.Parse<decimal>(bot, true);
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

                Class.HorseStatus.Parse<decimal>(bot, true);
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

                Class.HorseStatus.Parse<decimal>(bot, true);
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

                Class.HorseStatus.Parse<decimal>(bot, true);
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

                Class.HorseStatus.Parse<decimal>(bot, true);
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

                switch (bot.Horse.Status)
                {
                    case Shares.Enum.HorseStatus.Fat:
                        neededFood[0] = "0";
                        break;
                    case Shares.Enum.HorseStatus.Skinny:
                        neededFood[0] = "20";
                        break;
                    default:
                        break;
                }


                if (neededFood2.Length > 1)
                {
                    if (Convert.ToInt32(neededFood[0]) < Convert.ToInt32(neededFood2[0]) || Convert.ToInt32(neededFood[1]) < Convert.ToInt32(neededFood2[1]))
                    {
                        neededFood = neededFood2;
                    }
                }

                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsFeedingButton();

                string hayToken = Regex.Match(bot.HTMLActions.CurrentHtml, "type=\"hidden\" name=\"feeding(.*?)\" value=").Groups[1].Value;
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
                    currentHayAmount = Regex.Match(bot.HTMLActions.CurrentHtml, "fourrage-quantity\"> (\\d+) / <strong class=\"section-fourrage").Groups[1].Value;

                    if (bot.Settings.Actions.Food.ActionMode == HowrseActionMode.Auto)
                    {
                        //currentHayAmount = Regex.Match(bot.HTMLActions.CurrentHtml, "fourrage-quantity\"> (\\d+) / <strong class=\"section-fourrage").Groups[1].Value;
                        neededHayAmount = Convert.ToInt16(neededFood[0]) - Convert.ToInt16(currentHayAmount);
                    }
                    else if (bot.Settings.Actions.Food.ActionMode == HowrseActionMode.Manual)
                    {
                        neededHayAmount = Convert.ToInt16(bot.Settings.Actions.Food.AmountofHay);
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

                Class.HorseStatus.Parse<decimal>(bot, true);

            });
        }
        private static async Task PerformSleeping(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {
                HowrseAuthTokenModel authtokens = Tokens.GetHowrseAuthToken(bot.HTMLActions);
                HowrseTaskTokenModel taskTokens = Tokens.GetHowrseTaskToken(bot.HTMLActions);

                if (taskTokens.Sleep.Length == 0 || authtokens.Sleep == string.Empty)
                {
                    authtokens = Tokens.GetAuthTokenFromAction(bot.HTMLActions);
                    taskTokens = Tokens.GetTaskTokenFromAction(bot.HTMLActions);
                }

                if (taskTokens.Sleep.Length == 0 || authtokens.Sleep == string.Empty) return;

                bot.CurrentAction = BotClientCurrentAction.Schlafen;
                ButtonClickCoordinationsModel coords = Helper.GetRandomClickCoordsTakingCareButton();

                string endPoint = Endpoints.GetEndpoint(Endpoint.SleepAction, bot.Settings);
                string csrfToken = Tokens.GetCsrfToken(bot.HTMLActions);

                string postParam = (authtokens.Sleep + "=" + csrfToken + "&" + taskTokens.Sleep[0] + "=" + horseId + "&" + taskTokens.Sleep[1] + "=" + coords.X + "&" + taskTokens.Sleep[2] + "=" + coords.Y).ToLower();

                bot.HTMLActions.AfterActionHtml = bot.OwlientConnection.Post(endPoint, postParam);

                Class.HorseStatus.Parse<decimal>(bot, true);
            });
        }
        private static async Task PerformRidingCenter(string horseId, HowrseBotModel bot)
        {
            await Task.Run(async () =>
            {
                if (!bot.HTMLActions.CurrentHtml.Contains("elevage/chevaux/centreInscription?id=")) return;

                bot.CurrentAction = BotClientCurrentAction.Einstallen;
                RidingCenterModel ridingCenter = RidingCenter.GetRidingCenter(horseId, bot);
                RidingCenter.DoRegistration(horseId, ridingCenter, bot);
                //TODO: RC Name in action log
                await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                await ChangeHorse(horseId, bot);
            });
        }
        private static async Task ChangeHorse(string horseId, HowrseBotModel bot)
        {
            await Task.Run(async () =>
            {
                bot.HTMLActions.CurrentHtml = string.Empty;
                bot.HTMLActions.AfterActionHtml = string.Empty;
                bot.CurrentAction = BotClientCurrentAction.PferdWechseln;
                string endPoint = Endpoints.GetEndpoint(Endpoint.Horse, bot.Settings);
                endPoint += $"cheval?id={horseId}";

                bot.HTMLActions.CurrentHtml = bot.OwlientConnection.Get(endPoint);

                await ScrapeHorseInfos(bot);
                await CheckAndCallVet(horseId, bot);
            });
        }
        private static async Task ScrapeHorseInfos(HowrseBotModel bot)
        {
            await Task.Run(async () =>
            {
                int age = await GetHorseAge(bot);
                bot.Horse.Age = age;

                string horseSpriteBase64 = HorsePersonalization.GetSpriteBase64(bot);
                OnHorseSpriteChanged(horseSpriteBase64);

                bot.Horse.Status = await GetHorseStatus(bot);

                Class.HorseStatus.Parse<decimal>(bot);

                string horseName = Regex.Match(bot.HTMLActions.CurrentHtml, "<title>(.*?) - Howrse</title>").Groups[1].Value;
                bot.Horse.Name = horseName;

                string horseId = Regex.Match(bot.HTMLActions.CurrentHtml, "var chevalId = (\\d+);").Groups[1].Value;
                bot.Horse.Id = horseId;

            });
        }
        private static async Task<int> GetHorseAge(HowrseBotModel bot)
        {
            return await Task.Run(() =>
            {
                string age = Regex.Match(bot.HTMLActions.CurrentHtml, "chevalAge = (\\d+);").Groups[1].Value;

                if (string.IsNullOrEmpty(age))
                {
                    return -1;
                }

                return Convert.ToInt32(age);
            });
        }
        private static async Task<Shares.Enum.HorseStatus> GetHorseStatus(HowrseBotModel bot)
        {
            return await Task.Run(() =>
            {
                HtmlDocument doc = new();
                doc.LoadHtml(bot.HTMLActions.CurrentHtml);

                HtmlNode div = doc.DocumentNode.SelectSingleNode("//*[@id=\"care-tab-feed\"]//*[@id=\"messageBoxInline\"]/div/div/span/span[2]");

                if (div is null) return Shares.Enum.HorseStatus.Normal;

                if (div.InnerText.Contains("20"))
                {
                    return Shares.Enum.HorseStatus.Skinny;
                }

                return Shares.Enum.HorseStatus.Fat;
            });
        }       
        public static async Task StartBreeding(HowrseBotModel bot, ConcurrentBag<string> horseIds, bool finished, CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                bot.Status = BotClientStatus.Started;
                bot.CurrentAction = BotClientCurrentAction.Login;

                if (!await Login(bot))
                {
                    bot.Status = BotClientStatus.Error;
                    bot.CurrentAction = BotClientCurrentAction.Keine;
                    return;
                }
                ConcurrentBag<HorseModel> males = new();
                ConcurrentBag<HorseModel> females = new();

                bot.CurrentAction = BotClientCurrentAction.PferdeSuchen;

                Task breedingTask = new(async() =>
                {
                    await Breed(bot, males, females, horseIds.Count, ct);
                });

                breedingTask.Start();

                while ((!finished || (finished && !horseIds.IsEmpty) && !ct.IsCancellationRequested))
                {
                    if (horseIds.TryTake(out string horseId))
                    {
                        HorseModel horse = await GetHorseData(bot, horseId);

                        if (!horse.AbleToBreed) continue;

                        switch (horse.HorseSex)
                        {
                            case Shares.Enum.HorseSex.Male:
                                males.Add(horse);
                                break;
                            case Shares.Enum.HorseSex.Female:
                                females.Add(horse);
                                break;
                            default:
                                break;
                        }

                    }
                    await Task.Delay(100);                    
                }

                bot.CurrentAction = BotClientCurrentAction.Keine;
                bot.Status = BotClientStatus.Stopped;
            }, CancellationToken.None);

        }
        private static async Task<HorseModel> GetHorseData(HowrseBotModel bot, string horseId)
        {
            return await Task.Run(async () =>
            {
                await ChangeHorse(horseId, bot);

                if (string.IsNullOrEmpty(bot.HTMLActions.CurrentHtml)) return null;

                HtmlDocument doc = new();
                doc.LoadHtml(bot.HTMLActions.CurrentHtml);

                HorseModel horse = new();

                string age = Regex.Match(bot.HTMLActions.CurrentHtml, "var chevalAge = (\\d+);").Groups[1].Value;
                string health = Regex.Match(bot.HTMLActions.CurrentHtml, "var chevalSante = (\\d+|\\d+\\.\\d+);").Groups[1].Value.Replace('.', ',');
                string energy = Regex.Match(bot.HTMLActions.CurrentHtml, "var chevalEnergie = (\\d+|\\d+\\.\\d+);").Groups[1].Value.Replace('.', ',');
                string moral = Regex.Match(bot.HTMLActions.CurrentHtml, "var chevalMoral = (\\d+|\\d+\\.\\d+);").Groups[1].Value.Replace('.', ',');
                string sex = Regex.Match(bot.HTMLActions.CurrentHtml, "var chevalSexe = '(.*?)';").Groups[1].Value;
                string name = Regex.Match(bot.HTMLActions.CurrentHtml, "var chevalNom = '<b>(.*?)</b>';").Groups[1].Value;

                if (sex == "masculin")
                {
                    horse.HorseSex = Shares.Enum.HorseSex.Male;
                }
                else
                {
                    horse.HorseSex = Shares.Enum.HorseSex.Female;
                }

                horse.Id = horseId;
                horse.Name = name;
                horse.Age = Convert.ToInt32(age);
                horse.Stats.Health = decimal.Parse(health);
                horse.Stats.Energy = decimal.Parse(energy);
                horse.Stats.Moral = decimal.Parse(moral);

                HtmlNode reproductionNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"reproduction-tab-0\"]/table/tbody/tr/td[3]/a");

                if (horse.Age > 30 && horse.Stats.Energy >= 45 && reproductionNode is not null && reproductionNode.Id != "boutonEchographie" && !reproductionNode.HasClass("action-disabled"))
                {
                    horse.AbleToBreed = true;
                }

                return horse;
            });
        }              
        private static async Task Breed(HowrseBotModel bot, ConcurrentBag<HorseModel> males, ConcurrentBag<HorseModel> females, int horseIdsCount, CancellationToken ct)
        {
            //männchen verfügbare decksprünge als property
            await Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested || (horseIdsCount == 0 && !males.IsEmpty && !females.IsEmpty))
                {
                    if (!males.TryTake(out HorseModel male))
                    {
                        continue;
                    }

                    int availableBreedings = Convert.ToInt32(male.Stats.Energy) - 20 / 25;

                    if (availableBreedings <= 0)
                    {
                        continue;
                    }

                    for (int i = 0; i < availableBreedings; i++)
                    {
                        if (!females.TryTake(out HorseModel female))
                        {
                            continue;
                        }

                        await OfferAndAcceptReproduction(bot, male, female);
                    }  
                   
                }
            }, ct);
        }
        private static async Task OfferAndAcceptReproduction(HowrseBotModel bot, HorseModel male, HorseModel female)
        {
            await Task.Run(async () =>
            {

                bot.OwlientConnection.Post($"https://{ bot.Settings.Server }/elevage/chevaux/reserverJument", "id=" + male.Id + "&action=save&type=moi&price=0&owner=&nom=&mare=" + female.Id); //" + sStutenName + "

                await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));

                bot.CurrentAction = BotClientCurrentAction.PferdWechseln;

                string html = bot.OwlientConnection.Get($"https://{ bot.Settings.Server }/elevage/chevaux/cheval?id=" + female.Id);

                string horseFemaleHorseName = Regex.Match(html, "<a href=\"/elevage/chevaux/cheval\\?id=\\d+\">(.*?)</a>").Groups[1].Value;

                await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));

                bot.CurrentAction = BotClientCurrentAction.DecksprungAnbieten;

                string offerId = Regex.Match(html, "offre=(\\d+)&amp").Groups[1].Value;

                bot.OwlientConnection.Post($"https://{ bot.Settings.Server }/elevage/chevaux/saillie?offre=" + offerId + "&amp;jument=" + female.Id, "offre=" + offerId + "&amp;jument=" + female.Id);

                await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));

                bot.CurrentAction = BotClientCurrentAction.DecksprungAnnehmen;

                bot.OwlientConnection.Post($"https://{ bot.Settings.Server }/elevage/chevaux/doReproduction", "id=" + female.Id + "&offer=" + offerId + "&action=accept&search=");

                await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));

            });
        }
        private static async Task CheckAndCallVet(string horseId, HowrseBotModel bot)
        {
            await Task.Run(async () =>
            {
                if (!GeneralSettings.VetSettings.AutoCallVet) return;

                bot.CurrentAction = BotClientCurrentAction.AbfohlenPrüfen;

                HtmlDocument doc = new();
                doc.LoadHtml(bot.HTMLActions.CurrentHtml);

                HtmlNode vetButtonNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"boutonVeterinaire\"]");

                if (vetButtonNode is null) return;

                bot.CurrentAction = BotClientCurrentAction.TierarztRufen;

                string endPoint = Endpoints.GetEndpoint(Endpoint.MettreBas, bot.Settings);
                bot.OwlientConnection.Get(endPoint + $"?jument={horseId}");

                await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));

                endPoint = Endpoints.GetEndpoint(Endpoint.ChoisirNoms, bot.Settings);
                bot.OwlientConnection.Get(endPoint + "?jument=" + horseId);

                await Task.Delay(Helper.GetRandomSleepFromSettings(GeneralSettings));
                //TODO: geschlecht vom fohlen abfragen und ob es mehrere pferde sind. (ISIS kann bis zu 5 Pferde gegeben)
                //string horsename;

                //if (string.IsNullOrEmpty(GeneralSettings.VetSettings.FemaleHorseName))
                //{
                //    horsename = "stute";
                //}

                //if (true)
                //{

                //}

                endPoint = Endpoints.GetEndpoint(Endpoint.ChoisirNoms, bot.Settings);
                bot.OwlientConnection.Post(endPoint + "?jument=" + horseId, "valider=ok&poulain-1=" + "horsename");

                //GET https://www.howrse.de/elevage/chevaux/mettreBas?jument=78430252 
                //GET https://www.howrse.de/elevage/chevaux/choisirNoms?jument=78430252 

                //POST https://www.howrse.de/elevage/chevaux/choisirNoms?jument=78430252 //valider=ok&poulain-1=m+5336.07
                //GET https://www.howrse.de/elevage/chevaux/cheval?id=84291175&message=naissance 
            });
        }
        private static async Task CheckAndCatchUFO(string horseId, HowrseBotModel bot)
        {
            await Task.Run(() =>
            {

            });

            string sUfoID = "";
            string ufoImageName = "";
            string ufoPath = "";

            //sUfoID = GetBetween(html, "{initUfo(\"0\", {id:\"", "\"}, \"");

            //if (sUfoID != "")
            //{
            //    ufoPath = Regex.Match(html, "/media/equideo/image/(.*?)\", .*?;</script></div>").Groups[1].Value;
            //    ufoImageName = ufoPath.Substring(ufoPath.LastIndexOf("/") + 1);

            //    Connection.pPost("https://" + AccountManagement._sServer + "/member/ufo/catch", "&ufo=" + sUfoID);


            //    return Ufo.getReadableType(ufoImageName, ufoPath);
            //}
            //else
            //{
            //    return null;
            //}
        }
    }
}
