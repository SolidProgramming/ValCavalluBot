using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HowrseBotClient.Enum;
using Shares.Model;
using HowrseBotClient.Model;
using System.Text.RegularExpressions;
using Shares.Enum;

namespace HowrseBotClient.Class
{
    public static class Tokens
    {
        private static readonly Dictionary<HowrseTaskType, string> HowrseTaskNames = new()
        {
            { HowrseTaskType.Feeding, "feeding" },
            { HowrseTaskType.Drinking, "form-do-drink" },
            { HowrseTaskType.Stroking, "form-do-stroke" },
            { HowrseTaskType.Grooming, "form-do-groom" },
            { HowrseTaskType.GiveCarrot, "form-do-eat-treat-carotte" },
            { HowrseTaskType.Playing, "formCenterPlay" },
            { HowrseTaskType.GiveMash, "form-do-eat-treat-mash" },
            { HowrseTaskType.Sleeping, "form-do-night" },
            { HowrseTaskType.Training, "" },
            { HowrseTaskType.Riding, "formbalade.*?" },
            { HowrseTaskType.Competition, "" },
            { HowrseTaskType.Reproduction, "" },
            { HowrseTaskType.GiveSuckle, "form-do-suckle" },
            { HowrseTaskType.Aging, "age" }
        };

        public static HowrseTaskTokenModel GetHowrseTaskToken(HTMLActionsModel htmlActions)
        {
            return new HowrseTaskTokenModel
            {
                Feeding = GetTaskToken(HowrseTaskType.Feeding, htmlActions.CurrentHtml),
                Drink = GetTaskToken(HowrseTaskType.Drinking, htmlActions.CurrentHtml),
                Stroke = GetTaskToken(HowrseTaskType.Stroking, htmlActions.CurrentHtml),
                Groom = GetTaskToken(HowrseTaskType.Grooming, htmlActions.CurrentHtml),
                Carrot = GetTaskToken(HowrseTaskType.GiveCarrot, htmlActions.CurrentHtml),
                Mash = GetTaskToken(HowrseTaskType.GiveMash, htmlActions.CurrentHtml),
                Sleep = GetTaskToken(HowrseTaskType.Sleeping, htmlActions.CurrentHtml),
                Suckle = GetTaskToken(HowrseTaskType.GiveSuckle, htmlActions.CurrentHtml),
                Aging = GetTaskToken(HowrseTaskType.Aging, htmlActions.CurrentHtml),
                Riding = GetTaskToken(HowrseTaskType.Riding, htmlActions.CurrentHtml)
            };
        }
        public static HowrseAuthTokenModel GetHowrseAuthToken(HTMLActionsModel htmlActions)
        {
            return new HowrseAuthTokenModel
            {
                Feeding = GetAuthToken(HowrseTaskType.Feeding, htmlActions.CurrentHtml),
                Drink = GetAuthToken(HowrseTaskType.Drinking, htmlActions.CurrentHtml),
                Stroke = GetAuthToken(HowrseTaskType.Stroking, htmlActions.CurrentHtml),
                Groom = GetAuthToken(HowrseTaskType.Grooming, htmlActions.CurrentHtml),
                Carrot = GetAuthToken(HowrseTaskType.GiveCarrot, htmlActions.CurrentHtml),
                Mash = GetAuthToken(HowrseTaskType.GiveMash, htmlActions.CurrentHtml),
                Sleep = GetAuthToken(HowrseTaskType.Sleeping, htmlActions.CurrentHtml),
                Suckle = GetAuthToken(HowrseTaskType.GiveSuckle, htmlActions.CurrentHtml),
                Aging = GetAuthToken(HowrseTaskType.Aging, htmlActions.CurrentHtml),
                Riding = GetAction(HowrseTaskType.Riding)
            };
        }
        private static string GetAction(HowrseTaskType taskType)
        {
            return HowrseTaskNames.Single(a => a.Key == taskType).Value;
        }
        private static string[] GetTaskToken(HowrseTaskType taskType, string html)
        {
            return Regex.Matches(html, "name=\"" + GetAction(taskType) + "(.{10})\" id=")
                            .Cast<Match>()
                            .Select(m => m.Groups[1].Value.ToLower())
                            .ToArray();
        }
        private static string GetAuthToken(HowrseTaskType taskType, string html)
        {
            return Regex.Match(html, "id=\"" + GetAction(taskType) + "(.{5})\" type").Groups[1].Value.ToLower();
        }
    }
}
