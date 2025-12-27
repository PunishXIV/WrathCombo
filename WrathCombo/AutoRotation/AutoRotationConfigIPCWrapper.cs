#region

using System;
using WrathCombo.API.Enum;

#endregion

namespace WrathCombo.AutoRotation;

public class AutoRotationConfigIPCWrapper(AutoRotationConfig? config)
{
    public bool Enabled =>
        P?.UIHelper.AutoRotationStateControlled()?.state ??
        config?.Enabled ??
        false;

    public bool InCombatOnly
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("InCombatOnly");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : config.InCombatOnly;
        }
    }

    public DPSRotationMode DPSRotationMode
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("DPSRotationMode");
            return checkControlled is not null
                ? Enum.Parse<DPSRotationMode>(checkControlled.Value.state.ToString())
                : config.DPSRotationMode;
        }
    }

    public HealerRotationMode HealerRotationMode
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("HealerRotationMode");
            return checkControlled is not null
                ? Enum.Parse<HealerRotationMode>(
                    checkControlled.Value.state.ToString())
                : config.HealerRotationMode;
        }
    }

    public DPSSettingsIPCWrapper DPSSettings => new(config.DPSSettings);

    public HealerSettingsIPCWrapper HealerSettings => new(config.HealerSettings);

    public bool OrbwalkerIntegration
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("OrbwalkerIntegration");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : config.OrbwalkerIntegration;
        }
    }

    #region Direct Pass-Throughs (no IPC check)

    public bool BypassQuest => config.BypassQuest;

    public bool BypassFATE => config.BypassFATE;

    public bool BypassBuffs => config.BypassBuffs;

    public int CombatDelay => config.CombatDelay;

    public int Throttler => config.Throttler;

    #endregion
}

public class DPSSettingsIPCWrapper(DPSSettings settings)
{
    public bool FATEPriority
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("FATEPriority");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.FATEPriority;
        }
    }

    public bool QuestPriority
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("QuestPriority");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.QuestPriority;
        }
    }

    public bool OnlyAttackInCombat
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("OnlyAttackInCombat");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.OnlyAttackInCombat;
        }
    }

    public int? DPSAoETargets
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("DPSAoETargets");
            return checkControlled?.state ?? settings.DPSAoETargets;
        }
    }

    #region Direct Pass-Throughs (no IPC check)

    public bool PreferNonCombat => settings.PreferNonCombat;

    public float MaxDistance => settings.MaxDistance;

    public bool AlwaysSelectTarget => settings.AlwaysSelectTarget;

    public bool AoEIgnoreManual => settings.AoEIgnoreManual;
    
    public bool UnTargetAndDisableForPenalty => settings.UnTargetAndDisableForPenalty;

    #endregion
}

public class HealerSettingsIPCWrapper(HealerSettings settings)
{
    public int SingleTargetHPP =>
        P.UIHelper.AutoRotationConfigControlled("SingleTargetHPP")?.state
        ?? settings.SingleTargetHPP;

    public int AoETargetHPP =>
        P.UIHelper.AutoRotationConfigControlled("AoETargetHPP")?.state
        ?? settings.AoETargetHPP;

    public int SingleTargetExcogHPP =>
        P.UIHelper.AutoRotationConfigControlled("SingleTargetExcogHPP")?.state
        ?? settings.SingleTargetExcogHPP;
    
    public int SingleTargetRegenHPP =>
        P.UIHelper.AutoRotationConfigControlled("SingleTargetRegenHPP")?.state
        ?? settings.SingleTargetRegenHPP;

    public bool ManageKardia
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("ManageKardia");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.ManageKardia;
        }
    }

    public bool AutoRez
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("AutoRez");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.AutoRez;
        }
    }

    public bool AutoRezDPSJobs
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("AutoRezDPSJobs");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.AutoRezDPSJobs;
        }
    }

    public bool AutoRezOutOfParty
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("AutoRezOutOfParty");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.AutoRezOutOfParty;
        }
    }

    public bool AutoCleanse
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("AutoCleanse");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.AutoCleanse;
        }
    }

    public bool IncludeNPCs
    {
        get
        {
            var checkControlled =
                P.UIHelper.AutoRotationConfigControlled("IncludeNPCs");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.IncludeNPCs;
        }
    }

    #region Direct Pass-Throughs (no IPC check)

    public bool AutoRezRequireSwift => settings.AutoRezRequireSwift;

    public int? AoEHealTargetCount => settings.AoEHealTargetCount;

    public int HealDelay => settings.HealDelay;

    public bool KardiaTanksOnly => settings.KardiaTanksOnly;

    public bool PreEmptiveHoT => settings.PreEmptiveHoT;
    
    public bool AutoRezDPSJobsHealersOnly => settings.AutoRezDPSJobsHealersOnly;

    #endregion
}