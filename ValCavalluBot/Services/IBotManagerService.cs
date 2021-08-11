using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shares.Model;

namespace ValCavalluBot.Services
{
    interface IBotManagerService
    {
        event Action<string> OnHorseSpriteChanged;
        void Init();
        Task StartBot(string botId);
        Task StopBot(string botId);
        HowrseBotModel CreateBot(BotSettingsModel botSettings);
        List<HowrseBotModel> GetBots();
        HowrseBotModel GetBot(string botId);
        void DeleteBot(string botId);
        Task<bool> TestLogin(HowrseBotModel bot);
        Task StartBreeding(HowrseBotModel bot, CancellationTokenSource cts);
    }
}
