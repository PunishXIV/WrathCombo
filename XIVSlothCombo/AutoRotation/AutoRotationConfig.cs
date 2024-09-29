namespace XIVSlothCombo.AutoRotation
{
    public class AutoRotationConfig
    {
        public bool Enabled;
        public bool InCombatOnly = false;
        public bool InPartyCombat = false;
        public DPSRotationMode DPSRotationMode;
        public HealerRotationMode HealerRotationMode;
        public HealerSettings HealerSettings = new();
        public DPSSettings DPSSettings = new();
    }

    public class DPSSettings
    {
        public bool FATEPriority = false;
        public bool QuestPriority = false;
        public int? DPSAoETargets = 2;
    }

    public class HealerSettings
    {
        public int SingleTargetHPP = 70;
        public int AoETargetHPP = 60;
        public int SingleTargetRegenHPP = 80;
        public int? AoEHealTargetCount = 2;
    }
}
