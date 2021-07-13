using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Model;

namespace GRPCClient
{
    public static class GRPCClient
    {
        public static async Task<BreedingCollectorResponseModel> GetBreedings(HowrseBotModel bot)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:8869");

            var client = new BreedingHorseCollector.BreedingHorseCollectorClient(channel);

            BreedingCollectorRequestModel request = new()
            {
                Server = bot.Settings.Server,
                UserId = bot.Settings.HowrseUserId
            };

            return await client.GetBreedingsAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
        }

        public static async Task<HorseCollectorResponseModel> GetHorsesFromBreedings(List<string> breedingIds, HowrseBotModel bot)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:8869");

            var client = new BreedingHorseCollector.BreedingHorseCollectorClient(channel);

            HorseCollectorRequestModel request = new()
            {
                Server = bot.Settings.Server,
                UserId = bot.Settings.HowrseUserId
            };

            request.BreedingIds.AddRange(breedingIds);

            return await client.GetHorsesFromBreedingsAsync(request, deadline: DateTime.UtcNow.AddSeconds(999));
        }

    }
}
