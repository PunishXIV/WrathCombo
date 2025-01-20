using ECommons.DalamudServices;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvE;

internal class All
{
    public const byte JobID = 0;

    /// Used to block user input.
    public const uint SavageBlade = 11;

    public const uint
        //Tank
        ShieldWall = 197, //LB1, instant, range 0, AOE 50 circle, targets=self, animLock=1.930
        Stronghold = 198, //LB2, instant, range 0, AOE 50 circle, targets=self, animLock=3.860
        Rampart = 7531, //Lv8, instant, 90.0s CD (group 46), range 0, single-target, targets=self
        LowBlow = 7540, //Lv12, instant, 25.0s CD (group 41), range 3, single-target, targets=hostile
        Provoke = 7533, //Lv15, instant, 30.0s CD (group 42), range 25, single-target, targets=hostile
        Interject = 7538, //Lv18, instant, 30.0s CD (group 43), range 3, single-target, targets=hostile
        Reprisal = 7535, //Lv22, instant, 60.0s CD (group 44), range 0, AOE 5 circle, targets=self
        Shirk = 7537, //Lv48, instant, 120.0s CD (group 49), range 25, single-target, targets=party

        //Healer
        HealingWind = 206, //LB1, 2.0s cast, range 0, AOE 50 circle, targets=self, castAnimLock=2.100
        BreathOfTheEarth = 207, //LB2, 2.0s cast, range 0, AOE 50 circle, targets=self, castAnimLock=5.130
        Repose = 16560, //Lv8, 2.5s cast, GCD, range 30, single-target, targets=hostile
        Esuna = 7568, //Lv10, 1.0s cast, GCD, range 30, single-target, targets=self/party/alliance/friendly
        Rescue = 7571, //Lv48, instant, 120.0s CD (group 49), range 30, single-target, targets=party

        //Melee
        Braver = 200, //LB1, 2.0s cast, range 8, single-target, targets=hostile, castAnimLock=3.860
        Bladedance = 201, //LB2, 3.0s cast, range 8, single-target, targets=hostile, castAnimLock=3.860
        LegSweep = 7863, //Lv10, instant, 40.0s CD (group 41), range 3, single-target, targets=hostile
        Bloodbath = 7542, //Lv12, instant, 90.0s CD (group 46), range 0, single-target, targets=self
        Feint = 7549, //Lv22, instant, 90.0s CD (group 47), range 10, single-target, targets=hostile
        TrueNorth = 7546, //Lv50, instant, 45.0s CD (group 45/50) (2 charges), range 0, single-target, targets=self

        //PhysRanged
        BigShot = 4238, //LB1, 2.0s cast, range 30, AOE 30+R width 4 rect, targets=hostile, castAnimLock=3.100
        Desperado = 4239, //LB2, 3.0s cast, range 30, AOE 30+R width 5 rect, targets=hostile, castAnimLock=3.100
        LegGraze = 7554, //Lv6, instant, 30.0s CD (group 42), range 25, single-target, targets=hostile
        FootGraze = 7553, //Lv10, instant, 30.0s CD (group 41), range 25, single-target, targets=hostile
        Peloton = 7557, //Lv20, instant, 5.0s CD (group 40), range 0, AOE 30 circle, targets=self
        HeadGraze = 7551, //Lv24, instant, 30.0s CD (group 43), range 25, single-target, targets=hostile

        //Caster
        Skyshard = 203, //LB1, 2.0s cast, range 25, AOE 8 circle, targets=area, castAnimLock=3.100
        Starstorm = 204, //LB2, 3.0s cast, range 25, AOE 10 circle, targets=area, castAnimLock=5.100
        Addle = 7560, //Lv8 BLM/SMN/RDM/BLU, instant, 90.0s CD (group 46), range 25, single-target, targets=hostile
        Sleep = 25880, //Lv10 BLM/SMN/RDM/BLU, 2.5s cast, GCD, range 30, AOE 5 circle, targets=hostile

        //Multi-role actions
        SecondWind =
            7541, //Lv8 MNK/DRG/BRD/NIN/MCH/SAM/DNC/RPR, instant, 120.0s CD (group 49), range 0, single-target, targets=self
        LucidDreaming =
            7562, //Lv14 WHM/BLM/SMN/SCH/AST/RDM/BLU/SGE, instant, 60.0s CD (group 45), range 0, single-target, targets=self
        Swiftcast =
            7561, //Lv18 WHM/BLM/SMN/SCH/AST/RDM/BLU/SGE, instant, 60.0s CD (group 44), range 0, single-target, targets=self
        ArmsLength =
            7548, //Lv32 PLD/MNK/WAR/DRG/BRD/NIN/MCH/DRK/SAM/GNB/DNC/RPR, instant, 120.0s CD (group 48), range 0, single-target, targets=self
        Surecast =
            7559, //Lv44 WHM/BLM/SMN/SCH/AST/RDM/BLU/SGE, instant, 120.0s CD (group 48), range 0, single-target, targets=self

        //Misc
        Resurrection = 173, //Lv12 SMN/SCH, 8.0s cast, GCD, range 30, single-target, targets=party/alliance/friendly
        Sprint = 3,
        Raise = 125, //Lv12 WHM/AST/SGE, 8.0s cast, GCD, range 30, single-target, targets=party/alliance/friendly
        SolidReason = 232,
        AgelessWords = 215,
        WiseToTheWorldMIN = 26521,
        WiseToTheWorldBTN = 26522,

        //Duty actions
        SmokeScreen = 7816,
        AethericSiphon = 9102,
        Shatterstone = 9823,
        Deflect = 10006,
        DeflectVeryEasy = 18863;

    private const uint
        IsleSprint = 31314;

    /// <summary>
    ///     Quick Level, Offcooldown, spellweave, and MP check of Lucid Dreaming
    /// </summary>
    /// <param name="actionID">action id to check weave</param>
    /// <param name="MPThreshold">Player MP less than Threshold check</param>
    /// <param name="weave">Spell Weave check by default</param>
    /// <returns></returns>
    public static bool CanUseLucid(uint actionID, int MPThreshold, bool weave = true) => 
        CustomComboFunctions.ActionReady(LucidDreaming)
        && CustomComboFunctions.LocalPlayer.CurrentMp <= MPThreshold
        && (!weave || CustomComboFunctions.CanSpellWeave());

    public static class Buffs
    {
        public const ushort
            Weakness = 43,
            WellFed = 48,
            Medicated = 49,
            Bloodbath = 84,
            Surecast = 160,
            Swiftcast = 167,
            Rampart = 1191,
            Peloton = 1199,
            LucidDreaming = 1204,
            ArmsLength = 1209,
            TrueNorth = 1250,
            Sprint = 50;
    }

    public static class Debuffs
    {
        public const ushort

            //Tank
            Reprisal = 1193, //applied by Reprisal to target

            //Melee
            Feint = 1195, //applied by Feint to self
            TrueNorth = 1250, //applied by True North to self

            //PhysRanged
            Peloton = 1199, //applied by Peloton to self/party

            //Caster/Healer
            Addle = 1203, //applied by Addle to target
            Swiftcast = 167, //applied by Swiftcast to self
            Raise = 148; //applied by Raise to target
    }

    internal class ALL_IslandSanctuary_Sprint : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_IslandSanctuary_Sprint;

        protected override uint Invoke(uint actionID) =>
            actionID is Sprint && Svc.ClientState.TerritoryType is 1055
                ? IsleSprint
                : actionID;
    }

    //Tank Features
    internal class ALL_Tank_Interrupt : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Tank_Interrupt;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case LowBlow or PLD.ShieldBash when CanInterruptEnemy() && ActionReady(Interject):
                    return Interject;

                case LowBlow or PLD.ShieldBash when ActionReady(LowBlow):
                    return LowBlow;

                case LowBlow or PLD.ShieldBash when actionID == PLD.ShieldBash && IsOnCooldown(LowBlow):
                default:
                    return actionID;
            }
        }
    }

    internal class ALL_Tank_Reprisal : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Tank_Reprisal;

        protected override uint Invoke(uint actionID) =>
            actionID is Reprisal && TargetHasEffectAny(Debuffs.Reprisal) && IsOffCooldown(Reprisal)
                ? OriginalHook(11)
                : actionID;
    }

    //Healer Features
    internal class ALL_Healer_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Healer_Raise;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case WHM.Raise or AST.Ascend or SGE.Egeiro:
                case SCH.Resurrection when LocalPlayer.ClassJob.Value.RowId is SCH.JobID:
                {
                    if (ActionReady(Swiftcast))
                        return Swiftcast;

                    if (actionID == WHM.Raise && IsEnabled(CustomComboPreset.WHM_ThinAirRaise) &&
                        ActionReady(WHM.ThinAir) && !HasEffect(WHM.Buffs.ThinAir))
                        return WHM.ThinAir;

                    return actionID;
                }

                default:
                    return actionID;
            }
        }
    }

    //Caster Features
    internal class ALL_Caster_Addle : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Caster_Addle;

        protected override uint Invoke(uint actionID) =>
            actionID is Addle && TargetHasEffectAny(Debuffs.Addle) && IsOffCooldown(Addle)
                ? OriginalHook(11)
                : actionID;
    }

    internal class ALL_Caster_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Caster_Raise;

        protected override uint Invoke(uint actionID)
        {
            switch (actionID)
            {
                case BLU.AngelWhisper or RDM.Verraise:
                case SMN.Resurrection when LocalPlayer.ClassJob.RowId is SMN.JobID:
                {
                    if (HasEffect(Buffs.Swiftcast) || HasEffect(RDM.Buffs.Dualcast))
                        return actionID;

                    if (IsOffCooldown(Swiftcast))
                        return Swiftcast;

                    if (LocalPlayer.ClassJob.RowId is RDM.JobID &&
                        ActionReady(RDM.Vercure))
                        return RDM.Vercure;

                    break;
                }
            }

            return actionID;
        }
    }

    //Melee DPS Features
    internal class ALL_Melee_Feint : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Melee_Feint;

        protected override uint Invoke(uint actionID) =>
            actionID is Feint && TargetHasEffectAny(Debuffs.Feint) && IsOffCooldown(Feint)
                ? OriginalHook(11)
                : actionID;
    }

    internal class ALL_Melee_TrueNorth : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Melee_TrueNorth;

        protected override uint Invoke(uint actionID) =>
            actionID is TrueNorth && HasEffect(Buffs.TrueNorth)
                ? OriginalHook(11)
                : actionID;
    }

    //Ranged Physical Features
    internal class ALL_Ranged_Mitigation : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Ranged_Mitigation;

        protected override uint Invoke(uint actionID) =>
            actionID is BRD.Troubadour or MCH.Tactician or DNC.ShieldSamba &&
            (HasEffectAny(BRD.Buffs.Troubadour) || HasEffectAny(MCH.Buffs.Tactician) ||
             HasEffectAny(DNC.Buffs.ShieldSamba)) &&
            IsOffCooldown(actionID)
                ? OriginalHook(11)
                : actionID;
    }

    internal class ALL_Ranged_Interrupt : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ALL_Ranged_Interrupt;

        protected override uint Invoke(uint actionID) =>
            actionID is FootGraze && CanInterruptEnemy() && ActionReady(HeadGraze)
                ? HeadGraze
                : actionID;
    }
}
