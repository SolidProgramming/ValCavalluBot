using Shares.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class HorseStatsModel<T>
    {
        //public event Action<T> OnHorseStatsChanged;

        private T _Energy;
        private T _Health;
        private T _Moral;

        public T Energy {
            get 
            {
                return _Energy;
            }
            set
            {
                _Energy = value;
                //OnHorseStatsChanged(value);                
            }
        }
        public T Health {
            get
            {
                return _Health;
            }
            set 
            {
                _Health = value;
                //OnHorseStatsChanged(value);
            }
        }
        public T Moral {
            get
            {
                return _Moral;
            }
            set
            {
                _Moral = value;
               // OnHorseStatsChanged(value);
            }
        }
    }
}
