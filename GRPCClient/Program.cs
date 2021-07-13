using Grpc.Net.Client;
using System;
using GRPCClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GRPCClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //var channel = GrpcChannel.ForAddress("https://localhost:8869");


            //var client = new BreedingHorseCollector.BreedingHorseCollectorClient(channel);

            //BreedingCollectorRequestModel request = new()
            //{
            //    Server = "www.howrse.de",
            //    UserId = "10643354"
            //};

            //BreedingCollectorResponseModel response;

            //response = await client.GetBreedingsAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));

            ////using var call = client.GetBreedings(request);

            ////while (await call.ResponseStream.MoveNext(new()))
            ////{ 
            ////    var responseItem = call.ResponseStream.Current;

            ////    Console.WriteLine(responseItem.ID);
            ////}

            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
        }
    }
}
