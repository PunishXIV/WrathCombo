using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class BRD
{
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
            if (!CustomComboFunctions.ActionReady(WanderersMinuet))
                return false;

            if (!CustomComboFunctions.ActionReady(BattleVoice))
                return false;

            if (!CustomComboFunctions.ActionReady(RadiantFinale))
                return false;

            if (!CustomComboFunctions.ActionReady(RagingStrikes))
                return false;

            if (!CustomComboFunctions.ActionReady(Barrage))
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
            if (!CustomComboFunctions.ActionReady(WanderersMinuet))
                return false;

            if (!CustomComboFunctions.ActionReady(BattleVoice))
                return false;

            if (!CustomComboFunctions.ActionReady(RadiantFinale))
                return false;

            if (!CustomComboFunctions.ActionReady(RagingStrikes))
                return false;

            if (!CustomComboFunctions.ActionReady(Barrage))
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
            if (!CustomComboFunctions.ActionReady(WanderersMinuet))
                return false;

            if (!CustomComboFunctions.ActionReady(BattleVoice))
                return false;

            if (!CustomComboFunctions.ActionReady(RadiantFinale))
                return false;

            if (!CustomComboFunctions.ActionReady(RagingStrikes))
                return false;

            if (!CustomComboFunctions.ActionReady(Barrage))
                return false;


            return true;
        }
    }
}

