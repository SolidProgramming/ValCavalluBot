using Shares.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HowrseBotClient.Class
{
    public static class HorseStatus
    {
        public static HorseStatsModel<T> Get<T>(HowrseBotModel bot, bool isFromActionResponse = false)
        {
            string html;

            if (isFromActionResponse)
            {
                html = bot.HTMLActions.AfterActionHtml;
            }
            else
            {
                html = bot.HTMLActions.CurrentHtml;
            }

            if (string.IsNullOrEmpty(html)) return null;

            string energy = Regex.Match(html, "chevalEnergie\":(.*?),").Groups[1].Value;
            string health = Regex.Match(html, "chevalSante\":(.*?),").Groups[1].Value;
            string moral = Regex.Match(html, "chevalMoral\":(.*?),").Groups[1].Value;

            bot.Horse.Stats.Energy = Convert.ToDecimal(energy);
            bot.Horse.Stats.Health = Convert.ToDecimal(health);
            bot.Horse.Stats.Energy = Convert.ToDecimal(moral);

            return null;
        }
    }
}
