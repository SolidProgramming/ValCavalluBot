﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Enum;

namespace Shares.Model
{
    public struct SkillProgressModel<T>
    {
        public T Value { get; set; }
        public TrainingType TrainingType { get; set; }
    }
}
