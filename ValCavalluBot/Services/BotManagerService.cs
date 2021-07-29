using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shares;
using Shares.Model;
using HowrseBotClient;
using System.Threading;
using System.Collections.Concurrent;
using Shares.Enum;

namespace ValCavalluBot.Services
{
    public class BotManagerService : IBotManagerService
    {
        private ConcurrentBag<HorseModel> males;
        private ConcurrentBag<HorseModel> females;


        public HowrseBotModel CreateBot(BotSettingsModel botSettings)
        {
            return BotManager.CreateBot(botSettings);
        }
        public void DeleteBot(string botId)
        {
            BotManager.DeleteBot(botId);
        }
        public HowrseBotModel GetBot(string botId)
        {
            return BotManager.GetBot(botId);
        }
        public List<HowrseBotModel> GetBots()
        {
            return BotManager.GetBots();
        }
        public async Task StartBot(string botId)
        {
            await BotManager.StartBot(botId);
        }
        public async Task StopBot(string botId)
        {
            await BotManager.StopBot(botId);
        }
        public async Task<bool> TestLogin(HowrseBotModel bot)
        {
            return await BotManager.LoginTest(bot);
        }
        public async Task StartBreeding(HowrseBotModel bot, CancellationTokenSource cts)
        {
            males = new();
            females = new();

            GRPCClient.GRPCClient.OnGRPCFilterFoundHorse += OnGRPCFilterFoundHorse;

            await GRPCClient.GRPCClient.GetFilteredHorses(bot.Settings.ChosenBreedings.Select(_ => _.ID).ToList(), bot, cts.Token);
            
            await BotManager.StartBreeding(bot, males, females, cts.Token);
        }
        private void OnGRPCFilterFoundHorse(HorseModel horse)
        {           
            switch (horse.HorseSex)
            {
                case HorseSex.Male:
                    males.Add(horse);
                    break;
                case HorseSex.Female:
                    females.Add(horse);
                    break;
                default:
                    break;
            }
        }
    }
}
