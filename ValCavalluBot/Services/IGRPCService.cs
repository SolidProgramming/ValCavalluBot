using GRPCClient;
using Shares.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ValCavalluBot.Services
{
    interface IGRPCService
    {
        Task<List<HowrseBreedingModel>> GetBreedings(HowrseBotModel bot);
        Task<List<string>> GetHorsesFromBreedings(List<string> breedingIds, HowrseBotModel bot);
        Task GetFilteredHorses(List<string> breedingIds, HowrseBotModel bot, CancellationTokenSource cts);
    }
}
