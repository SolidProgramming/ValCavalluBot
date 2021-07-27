﻿using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Model;
using Shares.Enum;
using System.Threading;

namespace GRPCClient
{
    public static class GRPCClient
    {
        private const string GRPCAdress = "http://ddns.lucaweidmann.de:8081";
        public static event Action<HorseModel> OnGRPCFilterFoundHorse;

        public static async Task<BreedingCollectorResponseModel> GetBreedings(HowrseBotModel bot)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(GRPCAdress);

            BreedingHorseCollector.BreedingHorseCollectorClient client = new(channel);

            BreedingCollectorRequestModel request = new()
            {
                Server = bot.Settings.Server,
                UserId = bot.Settings.HowrseUserId
            };

            return await client.GetBreedingsAsync(request, deadline: DateTime.UtcNow.AddSeconds(20));
        }

        public static async Task<HorseCollectorResponseModel> GetHorsesFromBreedings(List<string> breedingIds, HowrseBotModel bot)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(GRPCAdress);

            BreedingHorseCollector.BreedingHorseCollectorClient client = new(channel);

            HorseCollectorRequestModel request = new()
            {
                Server = bot.Settings.Server,
                UserId = bot.Settings.HowrseUserId
            };

            request.BreedingIds.AddRange(breedingIds);

            return await client.GetHorsesFromBreedingsAsync(request, deadline: DateTime.UtcNow.AddSeconds(999));
        }

        public static async Task GetFilteredHorses(List<string> breedingIds, HowrseBotModel bot, CancellationTokenSource cts)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(GRPCAdress);

            BreedingHorseCollector.BreedingHorseCollectorClient client = new(channel);

            FilterHorsesRequestModel request = new()
            {
                Server = bot.Settings.Server,
                UserId = bot.Settings.HowrseUserId
            };

            request.BreedingIds.AddRange(breedingIds);

            using var call = client.GetFilteredHorsesFromBreedings(request);

            while (await call.ResponseStream.MoveNext(cts.Token))
            {
                HorseModel horse = ConvertToHorse(call.ResponseStream.Current);

                OnGRPCFilterFoundHorse(horse);

            }

            static HorseModel ConvertToHorse(FilterHorsesResponseModel horsedata)
            {
                HorseModel horse = new()
                {
                    Id = horsedata.Id,
                    Age = horsedata.Age,
                    AbleToBreed = horsedata.AbleToBreed,
                    InRidingCenter = horsedata.InRidingCenter,
                    IsPregnant = horsedata.IsPregnant,
                    HorseSex = (Shares.Enum.HorseSex)horsedata.Sex,
                    Name = horsedata.Name
                };

                horse.Stats.Energy = Convert.ToDecimal(horsedata.Energy);

                return horse;
            }

        }

    }
}
