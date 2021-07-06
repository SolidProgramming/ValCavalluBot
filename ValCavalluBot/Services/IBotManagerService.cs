﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shares.Model;

namespace ValCavalluBot.Services
{
    interface IBotManagerService
    {
        Task StartBot(string botId);
        Task StopBot(string botId);
        HowrseBotModel CreateBot(BotSettingsModel botSettings);
        List<HowrseBotModel> GetBots();
        HowrseBotModel GetBot(string botId);
        void DeleteBot(string botId);
        Task<bool> TestLogin(HowrseBotModel bot);
    }
}
