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
using System.Drawing;

namespace ValCavalluBot.Services
{
    public class BotManagerService : IBotManagerService
    {
        private ConcurrentBag<string> horseIds;
        private bool finished;
        public event Action<string> OnHorseSpriteChanged;
        public event Action<HowrseUserModel> OnHowrseUserInfoChanged;

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
            finished = false;

            GRPCClient.GRPCClient.OnGRPCFilterFoundHorse += OnGRPCFilterFoundHorse;
            GRPCClient.GRPCClient.OnGRPCFilterFinished += GRPCClient_OnGRPCFilterFinished;

            horseIds = new();

            bot.Status = BotClientStatus.Started;
            bot.CurrentAction = BotClientCurrentAction.PferdeSuchen;

            await GRPCClient.GRPCClient.GetFilteredHorses(bot.Settings.ChosenBreedings.Select(_ => _.ID).ToList(), bot, cts.Token);

            await BotManager.StartBreeding(bot, horseIds, finished, cts.Token);
        }
        private void GRPCClient_OnGRPCFilterFinished()
        {
            finished = true;
        }
        private void OnGRPCFilterFoundHorse(string horseId)
        {
            horseIds.Add(horseId);
        }
        public void Init()
        {
            BotManager.OnHorseSpriteChanged += BotManager_OnHorseSpriteChanged;
            BotManager.OnHowrseUserInfoChanged += BotManager_OnHowrseUserInfoChanged;            
        }

        private void BotManager_OnHowrseUserInfoChanged(HowrseUserModel howrseUser)
        {
            OnHowrseUserInfoChanged(howrseUser);
        }

        private void BotManager_OnHorseSpriteChanged(string horseSpriteBase64)
        {
            OnHorseSpriteChanged(horseSpriteBase64);
        }
    }
}
