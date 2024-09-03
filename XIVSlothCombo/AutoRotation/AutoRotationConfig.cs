﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVSlothCombo.AutoRotation
{
    public class AutoRotationConfig
    {
        public bool Enabled;
        public bool InCombatOnly = false;
        public DPSRotationMode DPSRotationMode;
        public HealerRotationMode HealerRotationMode;
        public TankRotationMode TankRotationMode;
        public int DPSAoETargets = 2;
        public int HealerAoETargets = 2;
        public int TankAoETargets = 2;
        public HealerSettings HealerSettings = new();
    }

    public class HealerSettings
    {
        public int SingleTargetHPP = 70;
        public int AoETargetHPP = 60;
    }
}