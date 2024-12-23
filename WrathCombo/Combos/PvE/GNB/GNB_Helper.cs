using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class GNB
{
    public static GNBOpenerMaxLevel1 Opener1 = new();
    public static GNBOpenerMaxLevel2 Opener2 = new();

    public static WrathOpener Opener()
    {
        var gcd = ActionManager.GetAdjustedRecastTime(ActionType.Action, KeenEdge) / 1000f;

        if (gcd <= 2.47f && Opener1.LevelChecked)
            return Opener1;

        if (Opener2.LevelChecked)
            return Opener2;

        return WrathOpener.Dummy;
    }


    internal class GNBOpenerMaxLevel1 : WrathOpener
    {
        //2.47 GCD or lower
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest,
            KeenEdge,
            BrutalShell,
            NoMercy,
            GnashingFang,
            JugularRip,
            BowShock,
            DoubleDown,
            BlastingZone,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart,
            BurstStrike,
            Hypervelocity,
            SonicBreak

        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            2,
            5,
        ];
        internal override UserData? ContentCheckConfig => Config.GNB_ST_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(Bloodfest))
                return false;

            if (!CustomComboFunctions.ActionReady(NoMercy))
                return false;

            if (!CustomComboFunctions.ActionReady(Hypervelocity))
                return false;

            if (!CustomComboFunctions.ActionReady(SonicBreak))
                return false;

            if (!CustomComboFunctions.ActionReady(DoubleDown))
                return false;

            if (!CustomComboFunctions.ActionReady(BowShock))
                return false;

            return true;
        }
    }

    internal class GNBOpenerMaxLevel2 : WrathOpener
    {
        //Above 2.47 GCD
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest,
            KeenEdge,
            BurstStrike,
            NoMercy,
            Hypervelocity,
            GnashingFang,
            JugularRip,
            BowShock,
            DoubleDown,
            BlastingZone,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart

        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            2,
        ];

        internal override UserData? ContentCheckConfig => Config.GNB_ST_Balance_Content;
        public override bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(Bloodfest))
                return false;

            if (!CustomComboFunctions.ActionReady(NoMercy))
                return false;

            if (!CustomComboFunctions.ActionReady(Hypervelocity))
                return false;

            if (!CustomComboFunctions.ActionReady(SonicBreak))
                return false;

            if (!CustomComboFunctions.ActionReady(DoubleDown))
                return false;

            if (!CustomComboFunctions.ActionReady(BowShock))
                return false;

            return true;
        }
    }
}

