using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HowrseBotClient.Enum;
using HowrseBotClient.Model;
using Shares.Model;

namespace HowrseBotClient.Class
{
    public static class Endpoints
    {
        private static readonly Dictionary<Endpoint, string> EndpointList = new()
        {
            {
                Endpoint.DrinkingAction,
                "elevage/chevaux/doDrink"
            }
        };

        public static string GetEndpoint(Endpoint endpoint, BotSettingsModel botSettings)
        {
            return $"https://{botSettings.Server}/{EndpointList[endpoint]}";
        }
        
    }
}
