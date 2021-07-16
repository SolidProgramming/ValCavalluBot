using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Enum
{
    [Flags]
    public enum RCFilter
    {
        None = 0,
        PriceFilter = 1,
        DurationFilter = 2,
        ProfitFilter = 4,
        FreeHayFilter = 8,
        FreeOatFilter = 16,
        FreeDrinkFilter = 32,
        FreeShowerFilter = 64
    }
}
