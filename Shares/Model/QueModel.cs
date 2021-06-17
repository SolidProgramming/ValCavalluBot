using Shares.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class QueModel
    {
        public List<HorseModel> Horses = new();
        public List<HowrseTaskModel> Tasks = new();

        //private static ConditionSettingModel ConditionSettings = new ConditionSettingModel { Type = ExitConditionType.None };

        //public delegate void OnQueTaskAddedEventHandler(HowrseTaskModel task);
        //public static event OnQueTaskAddedEventHandler OnQueTaskAdded;

        //public delegate void OnQueTaskRemovedEventHandler(int listIndex);
        //public static event OnQueTaskRemovedEventHandler OnQueTaskRemoved;

        //public delegate void OnQueChangedEventHandler(int horseCount, int breedingCount);
        //public static event OnQueChangedEventHandler OnQueChanged;

        public void AddHorse(HorseModel horse)
        {
            Horses.Add(horse);
            //OnQueChanged(Horses.Count, Horses.GroupBy(x => x.BreedingId).Count());
        }

        public void AddBreeding(string breedingId)
        {
            // var bla = Breeding.Breedings.Single(x => x.);
            //selectedBreedings.Add();
        }

        public void AddTask(HowrseTaskModel task)
        {
            Tasks.Add(task);
            //OnQueTaskAdded(task);
        }

        public void RemoveTask(int index)
        {           
            Tasks.RemoveAt(index);
            //OnQueTaskRemoved(index);
        }
    }
}
