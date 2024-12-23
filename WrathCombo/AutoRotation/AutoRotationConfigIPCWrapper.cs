#region

using System;

#endregion

namespace WrathCombo.AutoRotation;

public class AutoRotationConfigIPCWrapper(AutoRotationConfig config)
{
    public bool Enabled =>
        P.IPC.UIHelper.AutoRotationStateControlled()?.state ??
        config.Enabled;

    public bool InCombatOnly
    {
        get
        {
            var checkControlled =
                P.IPC.UIHelper.AutoRotationConfigControlled("InCombatOnly");
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
                P.IPC.UIHelper.AutoRotationConfigControlled("DPSRotationMode");
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
                P.IPC.UIHelper.AutoRotationConfigControlled("HealerRotationMode");
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

    #endregion
}

public class DPSSettingsIPCWrapper(DPSSettings settings)
{
    public bool FATEPriority
    {
        get
        {
            var checkControlled =
                P.IPC.UIHelper.AutoRotationConfigControlled("FATEPriority");
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
                P.IPC.UIHelper.AutoRotationConfigControlled("QuestPriority");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.QuestPriority;
        }
    }

    #region Direct Pass-Throughs (no IPC check)

    public int? DPSAoETargets => settings.DPSAoETargets;

    public bool PreferNonCombat => settings.PreferNonCombat;

    public bool OnlyAttackInCombat => settings.OnlyAttackInCombat;

    public float MaxDistance => settings.MaxDistance;

    #endregion
}

public class HealerSettingsIPCWrapper(HealerSettings settings)
{
    public int SingleTargetHPP =>
        P.IPC.UIHelper.AutoRotationConfigControlled("SingleTargetHPP")?.state
        ?? settings.SingleTargetHPP;

    public int AoETargetHPP =>
        P.IPC.UIHelper.AutoRotationConfigControlled("AoETargetHPP")?.state
        ?? settings.AoETargetHPP;

    public int SingleTargetRegenHPP =>
        P.IPC.UIHelper.AutoRotationConfigControlled("SingleTargetRegenHPP")?.state
        ?? settings.SingleTargetRegenHPP;

    public bool ManageKardia
    {
        get
        {
            var checkControlled =
                P.IPC.UIHelper.AutoRotationConfigControlled("ManageKardia");
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
                P.IPC.UIHelper.AutoRotationConfigControlled("AutoRez");
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
                P.IPC.UIHelper.AutoRotationConfigControlled("AutoRezDPSJobs");
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
                P.IPC.UIHelper.AutoRotationConfigControlled("AutoCleanse");
            return checkControlled is not null
                ? checkControlled.Value.state == 1
                : settings.AutoCleanse;
        }
    }

    #region Direct Pass-Throughs (no IPC check)

    public int? AoEHealTargetCount => settings.AoEHealTargetCount;

    public int HealDelay => settings.HealDelay;

    public bool KardiaTanksOnly => settings.KardiaTanksOnly;

    public bool PreEmptiveHoT => settings.PreEmptiveHoT;

    #endregion
}
