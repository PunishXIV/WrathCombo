#region

using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

// ReSharper disable AccessToStaticMemberViaDerivedType
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable ReturnTypeCanBeNotNullable
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class WHM
{
    internal static bool NeedsDoT()
    {
        var dotAction = OriginalHook(Aero);
        var hpThreshold = Config.WHM_ST_DPS_AeroOptionSubOption ==
                          (int)Config.BossAvoidance.Off ||
                          !InBossEncounter()
            ? Config.WHM_ST_DPS_AeroOption
            : 0;
        AeroList.TryGetValue(dotAction, out var dotDebuffID);
        var dotRemaining = GetStatusEffectRemainingTime(dotDebuffID, CurrentTarget);

        return ActionReady(dotAction) &&
               HasBattleTarget() &&
               GetTargetHPPercent() > hpThreshold &&
               dotRemaining <= Config.WHM_ST_MainCombo_DoT_Threshold;
    }

    #region Heal Priority

    public static int GetMatchingConfigST(
        int i,
        IGameObject? optionalTarget,
        out uint action,
        out bool enabled)
    {
        //var healTarget = optionalTarget ?? GetHealTarget(Config.WHM_STHeals_UIMouseOver);
        //leaving in case Regen gets a slider and is added

        var canWeave = CanWeave(0.3);

        switch (i)
        {
            case 0:
                action = Benediction;

                enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Benediction) &&
                          (!Config.WHM_STHeals_BenedictionWeave ||
                           Config.WHM_STHeals_BenedictionWeave && canWeave);

                return Config.WHM_STHeals_BenedictionHP;

            case 1:
                action = Tetragrammaton;

                enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Tetragrammaton) &&
                          (!Config.WHM_STHeals_TetraWeave ||
                           Config.WHM_STHeals_TetraWeave && canWeave);

                return Config.WHM_STHeals_TetraHP;

            case 2:
                action = DivineBenison;

                enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Benison) &&
                          (!Config.WHM_STHeals_BenisonWeave ||
                           Config.WHM_STHeals_BenisonWeave && canWeave);

                return Config.WHM_STHeals_BenisonHP;

            case 3:
                action = Aquaveil;

                enabled = IsEnabled(CustomComboPreset.WHM_STHeals_Aquaveil) &&
                          (!Config.WHM_STHeals_AquaveilWeave ||
                           Config.WHM_STHeals_AquaveilWeave && canWeave);

                return Config.WHM_STHeals_AquaveilHP;
        }

        enabled = false;
        action = 0;

        return 0;
    }

    #endregion

    #region Variables

    //Lists
    internal static readonly List<uint>
        StoneGlareList = [Stone1, Stone2, Stone3, Stone4, Glare1, Glare3];

    internal static readonly Dictionary<uint, ushort>
        AeroList = new()
        {
            { Aero, Debuffs.Aero },
            { Aero2, Debuffs.Aero2 },
            { Dia, Debuffs.Dia },
        };

    // Gauge Stuff
    internal static WHMGauge gauge = GetJobGauge<WHMGauge>();
    internal static bool CanLily => gauge.Lily > 0;
    internal static bool FullLily => gauge.Lily == 3;
    internal static bool AlmostFullLily => gauge is { Lily: 2, LilyTimer: >= 17000 };
    internal static bool BloodLilyReady => gauge.BloodLily == 3;

    #endregion

    #region Opener

    internal static WHMOpenerMaxLevel1 Opener1 = new();

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal class WHMOpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 92;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Glare3,
            Dia,
            Glare3,
            Glare3,
            PresenceOfMind,
            Glare4,
            Assize,
            Glare4,
            Glare4,
            Glare3,
            Glare3,
            Glare3,
            Glare3,
            Glare3,
            Glare3,
            Dia,
        ];

        internal override UserData ContentCheckConfig => Config.WHM_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(PresenceOfMind))
                return false;

            if (!IsOffCooldown(Assize))
                return false;

            return true;
        }
    }

    #endregion

    #region ID's

    public const byte ClassID = 6;
    public const byte JobID = 24;

    public const uint
        // DPS
        Glare1 = 16533,
        Glare3 = 25859,
        Glare4 = 37009,
        Stone1 = 119,
        Stone2 = 127,
        Stone3 = 3568,
        Stone4 = 7431,
        Assize = 3571,
        Holy = 139,
        Holy3 = 25860,

        // DoT
        Aero = 121,
        Aero2 = 132,
        Dia = 16532,

        // Heals
        Cure = 120,
        Cure2 = 135,
        Cure3 = 131,
        Regen = 137,
        AfflatusSolace = 16531,
        AfflatusRapture = 16534,
        Raise = 125,
        Benediction = 140,
        AfflatusMisery = 16535,
        Medica1 = 124,
        Medica2 = 133,
        Medica3 = 37010,
        Tetragrammaton = 3570,
        DivineBenison = 7432,
        Aquaveil = 25861,
        DivineCaress = 37011,

        // Buffs
        ThinAir = 7430,
        PresenceOfMind = 136,
        PlenaryIndulgence = 7433;


    public static class Buffs
    {
        public const ushort
            Regen = 158,
            Medica2 = 150,
            Medica3 = 3880,
            PresenceOfMind = 157,
            ThinAir = 1217,
            DivineBenison = 1218,
            Aquaveil = 2708,
            SacredSight = 3879,
            DivineGrace = 3881;
    }

    public static class Debuffs
    {
        public const ushort
            Aero = 143,
            Aero2 = 144,
            Dia = 1871;
    }

    #endregion
}
