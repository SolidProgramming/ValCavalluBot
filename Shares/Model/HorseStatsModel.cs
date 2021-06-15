using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class HorseStatsModel<T>
    {
        public T Energy { get; set; }
        public T Health { get; set; }
        public T Moral { get; set; }
    }
}
