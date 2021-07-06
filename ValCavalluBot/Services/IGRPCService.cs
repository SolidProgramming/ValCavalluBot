using GRPCClient;
using Shares.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValCavalluBot.Services
{
    interface IGRPCService
    {
        Task<List<HowrseBreedingModel>> GetBreedings(HowrseBotModel bot);
    }
}
