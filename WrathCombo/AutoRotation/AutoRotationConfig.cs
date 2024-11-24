namespace WrathCombo.AutoRotation
{
    public class AutoRotationConfig
    {
        public bool Enabled;
        public bool InCombatOnly = false;
        public bool BypassQuest = false;
        public bool BypassFATE = false;
        public int CombatDelay = 1;
        public DPSRotationMode DPSRotationMode;
        public HealerRotationMode HealerRotationMode;
        public HealerSettings HealerSettings = new();
        public DPSSettings DPSSettings = new();
    }

    public class DPSSettings
    {
        public bool FATEPriority = false;
        public bool QuestPriority = false;
        public int? DPSAoETargets = 3;
    }

    public class HealerSettings
    {
        public int SingleTargetHPP = 70;
        public int AoETargetHPP = 80;
        public int SingleTargetRegenHPP = 60;
        public int? AoEHealTargetCount = 2;
        public bool ManageKardia = false;
        public bool KardiaTanksOnly = false;
        public bool AutoRez = false;
        public bool AutoRezDPSJobs = false;
        public bool AutoCleanse = false;
        public bool PreEmptiveHoT = false;

    }
}
