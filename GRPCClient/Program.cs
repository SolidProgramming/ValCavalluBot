using Grpc.Net.Client;
using System;
using GRPCClient;
using GRPCClient.Protos;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GRPCClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress("https://localhost:8869");


            var client = new BreedingHorseCollector.BreedingHorseCollectorClient(channel);

            CollectorRequestModel request = new()
            {               
                Server = "www.howrse.de",
                UserId = "12345"
            };

            request.BreedingIds.Add("12345");

            using var call = client.GetHorsesFromBreedings(request);

            while (await call.ResponseStream.MoveNext(new()))
            {
                var responseItem = call.ResponseStream.Current;

                Console.WriteLine(responseItem.ID);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
