using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shares;
using Shares.Enum;
using Shares.Model;
using ValCavalluBot.Data.Model;

namespace ValCavalluBot.Data
{
    public static class HowrseActionHelper
    {
        public static List<HowrseActionCheckboxModel> Actions { get; set; } = new()
        {
            new()
            {
                TaskType = HowrseTaskType.Feeding,
                Checked = true,
                ImageUrl = "/ICON/Farm/SVG/5996 - Wheat.svg"
            }
            ,
            new()
            {
                TaskType = HowrseTaskType.Drinking,
                Checked = true,
                ImageUrl = "/ICON/Grooming/SVG/Hydrotherapy.svg"
            },
            new()
            {
                TaskType = HowrseTaskType.Stroking,
                Checked = true,
                ImageUrl = "/ICON/HorseRacing/SVG/12.svg"
            },
            new()
            {
                TaskType = HowrseTaskType.Grooming,
                Checked = true,
                ImageUrl = "/ICON/Grooming/SVG/Pet Brush.svg"
            },
            new()
            {
                TaskType = HowrseTaskType.GiveCarrot,
                Checked = true,
                ImageUrl = "/ICON/Farm/SVG/5992 - Vegetables.svg"
            },
            new()
            {
                TaskType = HowrseTaskType.GiveMash,
                Checked = true,
                ImageUrl = "/ICON/Energy/SVG/flash.svg"
            },
            new()
            {
                TaskType = HowrseTaskType.Sleeping,
                Checked = true,
                ImageUrl = "/ICON/Sleep/SVG/CLOUD AND MOON.svg"
            },
            new()
            {
                TaskType = HowrseTaskType.Aging,
                Checked = true,
                ImageUrl = "/ICON/Aging/SVG/Lineal Color/Color_20.svg"
            }

        };
    }
}
