using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class BRD
{
    #region ID's

    public const byte ClassID = 5;
    public const byte JobID = 23;

    public const uint
        HeavyShot = 97,
        StraightShot = 98,
        VenomousBite = 100,
        RagingStrikes = 101,
        QuickNock = 106,
        Barrage = 107,
        Bloodletter = 110,
        Windbite = 113,
        MagesBallad = 114,
        ArmysPaeon = 116,
        RainOfDeath = 117,
        BattleVoice = 118,
        EmpyrealArrow = 3558,
        WanderersMinuet = 3559,
        IronJaws = 3560,
        TheWardensPaeon = 3561,
        Sidewinder = 3562,
        PitchPerfect = 7404,
        Troubadour = 7405,
        CausticBite = 7406,
        Stormbite = 7407,
        RefulgentArrow = 7409,
        BurstShot = 16495,
        ApexArrow = 16496,
        Shadowbite = 16494,
        Ladonsbite = 25783,
        BlastArrow = 25784,
        RadiantFinale = 25785,
        WideVolley = 36974,
        HeartbreakShot = 36975,
        ResonantArrow = 36976,
        RadiantEncore = 36977;

    public static class Buffs
    {
        public const ushort
            RagingStrikes = 125,
            Barrage = 128,
            MagesBallad = 135,
            ArmysPaeon = 137,
            BattleVoice = 141,
            WanderersMinuet = 865,
            Troubadour = 1934,
            BlastArrowReady = 2692,
            RadiantFinale = 2722,
            ShadowbiteReady = 3002,
            HawksEye = 3861,
            ResonantArrowReady = 3862,
            RadiantEncoreReady = 3863;
    }

    public static class Debuffs
    {
        public const ushort
            VenomousBite = 124,
            Windbite = 129,
            CausticBite = 1200,
            Stormbite = 1201;
    }

    internal static class Traits
    {
        internal const ushort
            EnhancedBloodletter = 445;
    }

    #endregion

    public static BRDStandard Opener1 = new();
    public static BRDAdjusted Opener2 = new();
    public static BRDComfy Opener3 = new();
    internal static WrathOpener Opener()
    {
        if (CustomComboFunctions.IsEnabled(CustomComboPreset.BRD_ST_AdvMode))
        {
            if (Config.BRD_Adv_Opener_Selection == 0 && Opener1.LevelChecked) return Opener1;
            if (Config.BRD_Adv_Opener_Selection == 1 && Opener2.LevelChecked) return Opener2;
            if (Config.BRD_Adv_Opener_Selection == 2 && Opener3.LevelChecked) return Opener3;
        }

        if (Opener1.LevelChecked) return Opener1;
        return WrathOpener.Dummy;
    }

    internal class BRDOpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override int MinOpenerLevel => throw new NotImplementedException();

        public override int MaxOpenerLevel => throw new NotImplementedException();

        internal override UserData? ContentCheckConfig => throw new NotImplementedException();

        public override bool HasCooldowns() => throw new NotImplementedException();
    }

    internal class BRDStandard : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Stormbite,
            WanderersMinuet,
            EmpyrealArrow,
            CausticBite,
            BattleVoice,
            BurstShot,
            RadiantFinale,
            RagingStrikes,
            BurstShot,
            RadiantEncore,
            Barrage,
            RefulgentArrow,
            Sidewinder,
            ResonantArrow,
            EmpyrealArrow,
            BurstShot,
            BurstShot,
            IronJaws,
            BurstShot,
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([6, 9, 16, 17, 19], RefulgentArrow, () => CustomComboFunctions.HasEffect(Buffs.HawksEye)),
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            5
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        internal override UserData? ContentCheckConfig => Config.BRD_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!CustomComboFunctions.IsOffCooldown(WanderersMinuet))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(BattleVoice))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(RadiantFinale))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(RagingStrikes))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(Barrage))
                return false;


            return true;
        }
    }

    internal class BRDAdjusted : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            HeartbreakShot,
            Stormbite,
            WanderersMinuet,
            EmpyrealArrow,
            CausticBite,
            BattleVoice,
            BurstShot,
            RadiantFinale,
            RagingStrikes,
            BurstShot,
            Barrage,
            RefulgentArrow,
            Sidewinder,
            RadiantEncore,
            ResonantArrow,
            EmpyrealArrow,
            BurstShot,
            BurstShot,
            IronJaws,
            BurstShot,
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([7, 10, 17, 18, 20], RefulgentArrow, () => CustomComboFunctions.HasEffect(Buffs.HawksEye)),
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            6
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        internal override UserData? ContentCheckConfig => Config.BRD_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!CustomComboFunctions.IsOffCooldown(WanderersMinuet))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(BattleVoice))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(RadiantFinale))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(RagingStrikes))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(Barrage))
                return false;


            return true;
        }
    }

    internal class BRDComfy : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Stormbite,
            HeartbreakShot,
            WanderersMinuet,
            CausticBite,
            EmpyrealArrow,
            RadiantFinale,
            BurstShot,
            BattleVoice,
            RagingStrikes,
            BurstShot,
            Barrage,
            RefulgentArrow,
            Sidewinder,
            RadiantEncore,
            ResonantArrow,
            BurstShot,
            EmpyrealArrow,
            BurstShot,
            IronJaws,
            BurstShot,
        ];

        public override List<(int[], uint, Func<bool>)> SubstitutionSteps { get; set; } =
        [
            ([7, 10, 16, 18, 20], RefulgentArrow, () => CustomComboFunctions.HasEffect(Buffs.HawksEye)),
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        internal override UserData? ContentCheckConfig => Config.BRD_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!CustomComboFunctions.IsOffCooldown(WanderersMinuet))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(BattleVoice))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(RadiantFinale))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(RagingStrikes))
                return false;

            if (!CustomComboFunctions.IsOffCooldown(Barrage))
                return false;

            return true;
        }
    }
}

