using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HowrseBotClient.Model;
using Shares;
using Shares.Model;
using Shares.Enum;

namespace HowrseBotClient.Class
{
    public static class RidingCenter
    {
        //public RidingCenterModel? Registration(string horseId, HTMLActionsModel htmlActions)
        //{
        //    if (htmlActions.CurrentHtml.Contains("elevage/chevaux/centreInscription?id=")) return null;



        //}

        public static RidingCenterModel GetRidingCenter(string horseId, HowrseBotModel bot)
        {
            static bool TryGetHash(string _html, string _horseId, string _rcId, int _duration, out string _hash)
            {
                if (_html.Contains("id=" + _horseId + "&centre=" + _rcId + "&duree=" + _duration + "&elevage="))
                {
                    _hash = Regex.Match(_html, "id=" + _horseId + "&centre=" + _rcId + "&duree=" + _duration + "&elevage=&hash=(.*?)'}\\)\\)}\\)}").Groups[1].Value;
                    return true;
                }
                _hash = null;
                return false;
            }

            //TODO: duration so machen? 1 = 1 tag default
            int duration = (bot.Settings.RidingCenterSettings.Filter.HasFlag(RCFilter.DurationFilter) ? bot.Settings.RidingCenterSettings.Duration : 1);
            int price = bot.Settings.RidingCenterSettings.Filter.HasFlag(RCFilter.PriceFilter) ? bot.Settings.RidingCenterSettings.Price : 20;
            string profit = bot.Settings.RidingCenterSettings.Filter.HasFlag(RCFilter.ProfitFilter) ? bot.Settings.RidingCenterSettings.Profit.ToString() : "";
            string hay = bot.Settings.RidingCenterSettings.Filter.HasFlag(RCFilter.FreeHayFilter) ? "1" : "2";
            string oat = bot.Settings.RidingCenterSettings.Filter.HasFlag(RCFilter.FreeOatFilter) ? "1" : "2";
            string drink = bot.Settings.RidingCenterSettings.Filter.HasFlag(RCFilter.FreeDrinkFilter) ? "1" : "2";
            string shower = bot.Settings.RidingCenterSettings.Filter.HasFlag(RCFilter.FreeShowerFilter) ? "1" : "2";

            //TODO: RZ Standort
            string html = bot.OwlientConnection.Post("https://" + bot.Settings.Server + "/elevage/chevaux/centreSelection", "cheval=" + horseId + "&elevage=&cheval=" + horseId + "&competence=0&tri=tarif5&sens=ASC&tarif=" + price + "&leconsPrix=" + profit + "&foret=" + 2 + "&montagne=" + 2 + "&plage=" + 2 + "&classique=2&western=2&fourrage=" + hay + "&avoine=" + oat + "&carotte=2&mash=2&hasSelles=2&hasBrides=2&hasTapis=2&hasBonnets=2&hasBandes=2&abreuvoir=" + drink + "&douche=" + shower + "&centre=&centreBox=0&directeur=&prestige=&bonus=&boxType=&boxLitiere=&prePrestige=&prodSelles=&prodBrides=&prodTapis=&prodBonnets=&prodBandes=");

            string[] rcIds = Regex.Matches(html, "fiche\\?id=(\\d+)")
                    .Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .ToArray();

            var ridingCenter = new RidingCenterModel();
            string hash = string.Empty;

            for (int i = 0; i < rcIds.Length; i++)
            {
                if (TryGetHash(html, horseId, rcIds[i], duration, out hash))
                {
                    ridingCenter.Id = rcIds[i];
                    ridingCenter.Duration = duration;
                    ridingCenter.Hash = hash;
                    ridingCenter.Price = price;

                    break;
                }
            }

            return ridingCenter;
        }
        public static string DoRegistration(string horseId, RidingCenterModel ridingCenter, HowrseBotModel bot)
        {
            bot.OwlientConnection.Post("https://" + bot.Settings.Server + "/elevage/chevaux/doCentreInscription", "id=" + horseId + "&centre=" + ridingCenter.Id + "&duree=" + ridingCenter.Duration + "&elevage=&hash=" + ridingCenter.Hash);
            bot.HTMLActions.CurrentHtml = bot.OwlientConnection.Get("https://" + bot.Settings.Server + "/elevage/chevaux/cheval?id=" + horseId);

            string rcName = Regex.Match(bot.HTMLActions.CurrentHtml, "href=\"/centre/fiche\\?id=" + ridingCenter.Id + "\">(.*?)</a>").Groups[1].Value.ToUrlDecode();

            return rcName;
        }
    }
}
