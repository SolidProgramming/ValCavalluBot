using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRPCClient;
using Shares.Model;

namespace ValCavalluBot.Services
{
    public class GRPCService : IGRPCService
    {
        public async Task<List<HowrseBreedingModel>> GetBreedings(HowrseBotModel bot)
        {
            List<HowrseBreedingModel> breedings = new();
            BreedingCollectorResponseModel breedingResponse = await GRPCClient.GRPCClient.GetBreedings(bot);

            for (int i = 0; i < breedingResponse.Breedings.Count; i++)
            {
                breedings.Add(new()
                {
                    Checked = false,
                    ID = breedingResponse.Breedings[i].BreedingId,
                    Name = breedingResponse.Breedings[i].BreedingName
                });
            }

            return breedings;
        }
    }
}
