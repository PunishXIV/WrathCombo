using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class BRD
{
    public static BRDOpenerMaxLevel1 Opener1 = new();
    public static WrathOpener Opener()
    {
        if (Opener1.LevelChecked) return Opener1;

        return WrathOpener.Dummy;
    }



    internal class BRDOpenerMaxLevel1 : WrathOpener
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
            PitchPerfect

        ];

        public override List<(int[], uint, Func<bool>)> ProcSteps { get; set; } =
        [
            ([6, 9, 16, 17, 20], RefulgentArrow, () => CustomComboFunctions.HasEffect(Buffs.HawksEye)),
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            5
        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(WanderersMinuet))
                return false;

            if (!CustomComboFunctions.ActionReady(BattleVoice))
                return false;

            if (!CustomComboFunctions.ActionReady(WanderersMinuet))
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

