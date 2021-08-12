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
            },
            {
                Endpoint.StrokingAction,
                "elevage/chevaux/doStroke"
            },
            {
                Endpoint.GroomingAction,
                "elevage/chevaux/doGroom"
            },
            {
                Endpoint.CarrotAction,
                "elevage/chevaux/doEatTreat"
            },
            {
                Endpoint.MashAction,
                "elevage/chevaux/doEatTreat"
            },
            {
                Endpoint.AgingAction,
                "elevage/chevaux/doAge"
            },
            {
                Endpoint.SleepAction,
                "elevage/chevaux/doNight"
            },
            {
                Endpoint.MissionAction,
                "elevage/chevaux/doCentreMission"
            },
            {
                Endpoint.Horse,
                "elevage/chevaux/"
            },
            {
                Endpoint.MettreBas,
                "elevage/chevaux/mettreBas"
            },
            {
                Endpoint.ChoisirNoms,
                "elevage/chevaux/choisirNoms"
            }
        };

        public static string GetEndpoint(Endpoint endpoint, BotSettingsModel botSettings)
        {
            return $"https://{botSettings.Server}/{EndpointList[endpoint]}";
        }
        
    }
}
