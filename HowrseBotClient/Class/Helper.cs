using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HowrseBotClient.Model;
using Shares.Model;

namespace HowrseBotClient.Class
{
    public static class Helper
    {
        public static ButtonClickCoordinationsModel GetRandomClickCoordsTakingCareButton()
        {
            Random rnd = new();
            int xClickCoord = rnd.Next(10, 80);
            int yClickCoord = rnd.Next(10, 72);

            return new ButtonClickCoordinationsModel { X = xClickCoord, Y = yClickCoord };
        }
        public static ButtonClickCoordinationsModel GetRandomClickCoordsFeedingButton()
        {
            Random rnd = new();
            int xClickCoord = rnd.Next(5, 20);
            int yClickCoord = rnd.Next(5, 110);

            return new ButtonClickCoordinationsModel { X = xClickCoord, Y = yClickCoord };
        }
        public static string ToUrlEncode(this string text)
        {
            return Uri.EscapeDataString(text);
        }
        public static string ToUrlDecode(this string text)
        {
            return System.Net.WebUtility.UrlDecode(text);
        }
        public static int GetRandomSleepFromSettings(GeneralSettingsModel generalSettings)
        {
            Random rnd = new();
            return rnd.Next(generalSettings.WaitTimeFrom, generalSettings.WaitTimeTo);
        }
       
    }
}
