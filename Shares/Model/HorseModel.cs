using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Enum;

namespace Shares.Model
{
    public class HorseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Size { get; set; }
        public int Weight { get; set; }
        public HorseRace HorseRace { get; set; }
        public HorseSex HorseSex { get; set; }
        public HorseFur HorseFur { get; set; }
        public string BreedingId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public HorseStatsModel<decimal> Stats { get; set; } = new();
        public List<SkillProgressModel<decimal>> SkillProgress = new();
        public HorseStatus Status { get; set; } = HorseStatus.Normal;
        public bool AbleToBreed { get; set; }
        public bool InRidingCenter { get; set; }
        public bool IsPregnant { get; set; }
        //TODO: Accessoriess & UserObjects in Zubbel Bot V2 mit Interface
        //public List<AccessorieModel> Accessories = new List<AccessorieModel>(); 
        //public List<UserObjectModel> Objects = new List<UserObjectModel>();
    }
}
