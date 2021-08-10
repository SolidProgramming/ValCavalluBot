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
        public static void Parse<T>(HowrseBotModel bot, bool isFromActionResponse = false)
        {
            string html;

            string energy, health, moral;

            if (isFromActionResponse)
            {
                html = bot.HTMLActions.AfterActionHtml;

                energy = Regex.Match(html, "chevalEnergie\":(.*?),").Groups[1].Value.Replace(".", ",");
                health = Regex.Match(html, "chevalSante\":(.*?),").Groups[1].Value.Replace(".", ",");
                moral = Regex.Match(html, "chevalMoral\":(.*?),").Groups[1].Value.Replace(".", ",");
            }
            else
            {
                html = bot.HTMLActions.CurrentHtml;

                energy = Regex.Match(html, "chevalEnergie = (.*?);").Groups[1].Value.Replace(".", ",");
                health = Regex.Match(html, "chevalSante = (.*?);").Groups[1].Value.Replace(".", ",");
                moral = Regex.Match(html, "chevalMoral = (.*?);").Groups[1].Value.Replace(".", ",");
            }

            if (string.IsNullOrEmpty(energy) || string.IsNullOrEmpty(health) || string.IsNullOrEmpty(moral)) return;

            bot.Horse.Stats.Energy = Convert.ToDecimal(energy);
            bot.Horse.Stats.Health = Convert.ToDecimal(health);
            bot.Horse.Stats.Moral = Convert.ToDecimal(moral);
        }
    }
}
