﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class HowrseUserModel
    {
        public int Eq { get; set; }
        public int Passes { get; set; }
        public int Rank { get; set; }
        public int HorseCount { get; set; }
        public int DaysLoggedIn { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime LastConnection{ get; set; }
        public int Karma { get; set; }

    }
}
