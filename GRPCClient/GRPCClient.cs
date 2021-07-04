using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRPCClient
{
    public static class GRPCClient
    {
        public static async Task<BreedingCollectorResponseModel> GetBreedings()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:8869");

            var client = new BreedingHorseCollector.BreedingHorseCollectorClient(channel);

            BreedingCollectorRequestModel request = new()
            {
                Server = "www.howrse.de",
                UserId = "3676376"
            };

            return await client.GetBreedingsAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
        }
    }
}
