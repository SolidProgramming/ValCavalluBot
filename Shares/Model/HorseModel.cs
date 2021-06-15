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
        public string Id;
        public string Name;
        public int Age;
        public int Size;
        public int Weight;
        public HorseRace HorseRace;
        public HorseSex HorseSex;
        public HorseFur HorseFur;
        public string BreedingId;
        public DateTime DateOfBirth;
        public HorseStatsModel<decimal> Stats;
        //TODO: Accessoriess & UserObjects in Zubbel Bot V2 mit Interface
        //public List<AccessorieModel> Accessories = new List<AccessorieModel>(); 
        //public List<UserObjectModel> Objects = new List<UserObjectModel>();
        public List<SkillProgressModel<decimal>> SkillProgress = new();
        public bool ExitConditionMet = false;
    }
}
