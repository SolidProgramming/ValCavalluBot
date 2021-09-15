using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HowrseBotClient.Enum;
using Shares.Model;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace HowrseBotClient.Class
{
    public static class HowrseUser
    {
        public static async Task<HowrseUserModel> GetUserInfo(HowrseBotModel bot)
        {
            return await Task.Run(() =>
            {
                string endpoint = Endpoints.GetEndpoint(Endpoint.Profile, bot.Settings);
                endpoint += $"/?id={ bot.Settings.HowrseUserId }";

                bot.HTMLActions.CurrentHtml = bot.OwlientConnection.Get(endpoint);

                HtmlDocument doc = new();
                doc.LoadHtml(bot.HTMLActions.CurrentHtml);

                HtmlNodeCollection tableRowsNodes = doc.DocumentNode.SelectNodes("//tbody/tr/td/span");

                string daysLoggedIn = Regex.Match(tableRowsNodes[0].InnerText, "(\\d+)").Groups[1].Value;
                string rank = tableRowsNodes[1].InnerText.Replace(".", "");
                string registerdate = tableRowsNodes[2].InnerText;
                string horseCount = tableRowsNodes[3].InnerText;
                string lastConnection = tableRowsNodes[4].InnerText;
                string eq = tableRowsNodes[5].InnerText.Replace(".", "");
                string karma = Regex.Match(tableRowsNodes[6].InnerText, "(\\d+)").Groups[1].Value;
                string passes = Regex.Match(bot.HTMLActions.CurrentHtml, "id=\"pass\" data-amount=\"(\\d+)").Groups[1].Value;

                HowrseUserModel user = new()
                {
                    DaysLoggedIn = Convert.ToInt32(daysLoggedIn),
                    Rank = Convert.ToInt32(rank),
                    RegisterDate = Convert.ToDateTime(registerdate),
                    HorseCount = Convert.ToInt32(horseCount),
                    LastConnection = Convert.ToDateTime(lastConnection),
                    Eq = Convert.ToInt32(eq),
                    Karma = Convert.ToInt32(karma),
                    Passes = Convert.ToInt32(passes)
                };

                return user;
            });
        }
    }
}
