#region

using System;

#endregion

namespace WrathCombo.AutoRotation;

public class AutoRotationConfigIPCWrapper(AutoRotationConfig? config)
{
    public bool Enabled =>
        P?.UIHelper?.AutoRotationStateControlled()?.state ??
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

    #region Direct Pass-Throughs (no IPC check)

    public bool BypassQuest => config.BypassQuest;

    public bool BypassFATE => config.BypassFATE;

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
                : settings.QuestPriority;
        }
    }

    #region Direct Pass-Throughs (no IPC check)

    public int? DPSAoETargets => settings.DPSAoETargets;

    public bool PreferNonCombat => settings.PreferNonCombat;

    public float MaxDistance => settings.MaxDistance;

    public bool AlwaysSelectTarget => settings.AlwaysSelectTarget;

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

    public int? AoEHealTargetCount => settings.AoEHealTargetCount;

    public int HealDelay => settings.HealDelay;

    public bool KardiaTanksOnly => settings.KardiaTanksOnly;

    public bool PreEmptiveHoT => settings.PreEmptiveHoT;

    #endregion
}
