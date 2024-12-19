using Dalamud.Game.ClientState.Statuses;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    internal static bool InMudra = false;
    internal static NINOpenerMaxLevel4thGCDKunai Opener1 = new();
    internal static NINOpenerMaxLevel3rdGCDDokumori Opener2 = new();
    internal static NINOpenerMaxLevel3rdGCDKunai Opener3 = new();

    internal static WrathOpener Opener()
    {
        if (CustomComboFunctions.IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode))
        {
            if (Config.NIN_Adv_Opener_Selection == 0 && Opener1.LevelChecked) return Opener1;
            if (Config.NIN_Adv_Opener_Selection == 1 && Opener2.LevelChecked) return Opener2;
            if (Config.NIN_Adv_Opener_Selection == 2 && Opener3.LevelChecked) return Opener3;
        }

        if (Opener1.LevelChecked) return Opener1;
        return WrathOpener.Dummy;
    }

    internal static bool OriginalJutsu => CustomComboFunctions.IsOriginal(Ninjutsu);

    internal static bool TrickDebuff => TargetHasTrickDebuff();

    internal static bool MugDebuff => TargetHasMugDebuff();

    private static bool TargetHasTrickDebuff()
    {
        return CustomComboFunctions.TargetHasEffect(Debuffs.TrickAttack) ||
               CustomComboFunctions.TargetHasEffect(Debuffs.KunaisBane);
    }

    private static bool TargetHasMugDebuff()
    {
        return CustomComboFunctions.TargetHasEffect(Debuffs.Mug) ||
               CustomComboFunctions.TargetHasEffect(Debuffs.Dokumori);
    }

    public static Status? MudraBuff => CustomComboFunctions.FindEffect(Buffs.Mudra);

    public static uint CurrentNinjutsu => CustomComboFunctions.OriginalHook(Ninjutsu);


    internal class MudraCasting
    {
        public enum MudraState
        {
            None,
            CastingFumaShuriken,
            CastingKaton,
            CastingRaiton,
            CastingHyoton,
            CastingHuton,
            CastingDoton,
            CastingSuiton,
            CastingGokaMekkyaku,
            CastingHyoshoRanryu
        }

        public MudraState CurrentMudra = MudraState.None;

        ///<summary> Checks if the player is in a state to be able to cast a ninjitsu.</summary>
        private static bool CanCast()
        {
            if (InMudra) return true;

            float gcd = CustomComboFunctions.GetCooldown(GustSlash).CooldownTotal;

            if (gcd == 0.5) return true;

            if (CustomComboFunctions.GetRemainingCharges(Ten) == 0 &&
                !CustomComboFunctions.HasEffect(Buffs.Mudra) &&
                !CustomComboFunctions.HasEffect(Buffs.Kassatsu))
                return false;

            return true;
        }

        /// <summary> Simple method of casting Fuma Shuriken.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastFumaShuriken(ref uint actionID)
        {
            if (FumaShuriken.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingFumaShuriken)
            {
                if (!CanCast() || ActionWatching.LastAction == FumaShuriken)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Ten or TenCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = CustomComboFunctions.OriginalHook(Ten);
                CurrentMudra = MudraState.CastingFumaShuriken;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Raiton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastRaiton(ref uint actionID)
        {
            if (Raiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingRaiton)
            {
                if (!CanCast() || ActionWatching.LastAction == Raiton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Ten or TenCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Chi);

                    return true;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = CustomComboFunctions.OriginalHook(Ten);
                CurrentMudra = MudraState.CastingRaiton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Katon.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastKaton(ref uint actionID)
        {
            if (Katon.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingKaton)
            {
                if (!CanCast() || ActionWatching.LastAction == Katon)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Chi or ChiCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ten);

                    return true;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = CustomComboFunctions.OriginalHook(Chi);
                CurrentMudra = MudraState.CastingKaton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Hyoton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastHyoton(ref uint actionID)
        {
            if (Hyoton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHyoton)
            {
                if (!CanCast() || CustomComboFunctions.HasEffect(Buffs.Kassatsu) || ActionWatching.LastAction == Hyoton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = CustomComboFunctions.OriginalHook(Ten);
                CurrentMudra = MudraState.CastingHyoton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Huton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastHuton(ref uint actionID)
        {
            if (Huton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHuton)
            {
                if (!CanCast() || ActionWatching.LastAction == Huton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Chi or ChiCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ten);

                    return true;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = CustomComboFunctions.OriginalHook(Chi);
                CurrentMudra = MudraState.CastingHuton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Doton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastDoton(ref uint actionID)
        {
            if (Doton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingDoton)
            {
                if (!CanCast() || ActionWatching.LastAction == Doton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Ten or TenCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Chi);

                    return true;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = CustomComboFunctions.OriginalHook(Ten);
                CurrentMudra = MudraState.CastingDoton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Suiton.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastSuiton(ref uint actionID)
        {
            if (Suiton.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingSuiton)
            {
                if (!CanCast() || ActionWatching.LastAction == Suiton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction is Ten or TenCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Chi);

                    return true;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = CustomComboFunctions.OriginalHook(Ten);
                CurrentMudra = MudraState.CastingSuiton;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Goka Mekkyaku.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastGokaMekkyaku(ref uint actionID)
        {
            if (GokaMekkyaku.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingGokaMekkyaku)
            {
                if (!CanCast() || !CustomComboFunctions.HasEffect(Buffs.Kassatsu) || ActionWatching.LastAction == GokaMekkyaku)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ten);

                    return true;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = CustomComboFunctions.OriginalHook(Chi);
                CurrentMudra = MudraState.CastingGokaMekkyaku;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        /// <summary> Simple method of casting Hyosho Ranryu.</summary>
        /// <param name="actionID">The actionID from the combo.</param>
        /// <returns>True if in a state to cast or continue the ninjitsu, modifies actionID to the step of the ninjitsu.</returns>
        public bool CastHyoshoRanryu(ref uint actionID)
        {
            if (HyoshoRanryu.LevelChecked() && CurrentMudra is MudraState.None or MudraState.CastingHyoshoRanryu)
            {
                if (!CanCast() || !CustomComboFunctions.HasEffect(Buffs.Kassatsu) || ActionWatching.LastAction == HyoshoRanryu)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = CustomComboFunctions.OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = CustomComboFunctions.OriginalHook(Chi);
                CurrentMudra = MudraState.CastingHyoshoRanryu;

                return true;
            }

            CurrentMudra = MudraState.None;

            return false;
        }

        public bool ContinueCurrentMudra(ref uint actionID)
        {

            if (ActionWatching.TimeSinceLastAction.TotalSeconds > 1 && CurrentNinjutsu == Ninjutsu && CurrentMudra != MudraState.None)
            {
                InMudra = false;
                ActionWatching.LastAction = 0;
                CurrentMudra = MudraState.None;
            }

            if ((ActionWatching.LastAction == FumaShuriken ||
                 ActionWatching.LastAction == Katon ||
                 ActionWatching.LastAction == Raiton ||
                 ActionWatching.LastAction == Hyoton ||
                 ActionWatching.LastAction == Huton ||
                 ActionWatching.LastAction == Doton ||
                 ActionWatching.LastAction == Suiton ||
                 ActionWatching.LastAction == GokaMekkyaku ||
                 ActionWatching.LastAction == HyoshoRanryu))
            {
                CurrentMudra = MudraState.None;
                InMudra = false;
            }

            return CurrentMudra switch
            {
                MudraState.None => false,
                MudraState.CastingFumaShuriken => CastFumaShuriken(ref actionID),
                MudraState.CastingKaton => CastKaton(ref actionID),
                MudraState.CastingRaiton => CastRaiton(ref actionID),
                MudraState.CastingHyoton => CastHyoton(ref actionID),
                MudraState.CastingHuton => CastHuton(ref actionID),
                MudraState.CastingDoton => CastDoton(ref actionID),
                MudraState.CastingSuiton => CastSuiton(ref actionID),
                MudraState.CastingGokaMekkyaku => CastGokaMekkyaku(ref actionID),
                MudraState.CastingHyoshoRanryu => CastHyoshoRanryu(ref actionID),
                _ => false
            };
        }
    }

    internal class NINOpenerMaxLevel4thGCDKunai : WrathOpener
    {
        //4th GCD Kunai
        public override List<uint> OpenerActions { get; set; } =
        [
            Ten,
            ChiCombo,
            JinCombo,
            Suiton,
            Kassatsu,
            SpinningEdge,
            GustSlash,
            Dokumori,
            Bunshin,
            PhantomKamaitachi,
            ArmorCrush,
            KunaisBane,
            ChiCombo,
            JinCombo,
            HyoshoRanryu,
            DreamWithinADream,
            Ten,
            ChiCombo,
            Raiton,
            TenChiJin,
            TCJFumaShurikenTen,
            TCJRaiton,
            TCJSuiton,
            Meisui,
            FleetingRaiju,
            ZeshoMeppo,
            TenriJendo,
            FleetingRaiju,
            Bhavacakra,
            Ten,
            ChiCombo,
            Raiton,
            FleetingRaiju,
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            12
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Ten) < 1) return false;
            if (CustomComboFunctions.IsOnCooldown(Mug)) return false;
            if (CustomComboFunctions.IsOnCooldown(TenChiJin)) return false;
            if (CustomComboFunctions.IsOnCooldown(PhantomKamaitachi)) return false;
            if (CustomComboFunctions.IsOnCooldown(Bunshin)) return false;
            if (CustomComboFunctions.IsOnCooldown(DreamWithinADream)) return false;
            if (CustomComboFunctions.IsOnCooldown(Kassatsu)) return false;
            if (CustomComboFunctions.IsOnCooldown(TrickAttack)) return false;

            return true;
        }
    }

    internal class NINOpenerMaxLevel3rdGCDDokumori : WrathOpener
    {
        //3rd GCD Dokumori
        public override List<uint> OpenerActions { get; set; } =
        [
            Ten,
            ChiCombo,
            JinCombo,
            Suiton,
            Kassatsu,
            SpinningEdge,
            GustSlash,
            ArmorCrush,
            Dokumori,
            Bunshin,
            PhantomKamaitachi,
            KunaisBane,
            ChiCombo,
            JinCombo,
            HyoshoRanryu,
            DreamWithinADream,
            Ten,
            ChiCombo,
            Raiton,
            TenChiJin,
            TCJFumaShurikenTen,
            TCJRaiton,
            TCJSuiton,
            Meisui,
            FleetingRaiju,
            ZeshoMeppo,
            TenriJendo,
            FleetingRaiju,
            Ten,
            ChiCombo,
            Raiton,
            FleetingRaiju,
            Bhavacakra,
            SpinningEdge
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            12
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Ten) < 1) return false;
            if (CustomComboFunctions.IsOnCooldown(Mug)) return false;
            if (CustomComboFunctions.IsOnCooldown(TenChiJin)) return false;
            if (CustomComboFunctions.IsOnCooldown(PhantomKamaitachi)) return false;
            if (CustomComboFunctions.IsOnCooldown(Bunshin)) return false;
            if (CustomComboFunctions.IsOnCooldown(DreamWithinADream)) return false;
            if (CustomComboFunctions.IsOnCooldown(Kassatsu)) return false;
            if (CustomComboFunctions.IsOnCooldown(TrickAttack)) return false;

            return true;
        }
    }

    internal class NINOpenerMaxLevel3rdGCDKunai : WrathOpener
    {
        //3rd GCD Kunai
        public override List<uint> OpenerActions { get; set; } =
        [
            Ten,
            ChiCombo,
            JinCombo,
            Suiton,
            Kassatsu,
            SpinningEdge,
            GustSlash,
            Dokumori,
            Bunshin,
            PhantomKamaitachi,
            KunaisBane,
            ChiCombo,
            JinCombo,
            HyoshoRanryu,
            DreamWithinADream,
            Ten,
            ChiCombo,
            Raiton,
            TenChiJin,
            TCJFumaShurikenTen,
            TCJRaiton,
            TCJSuiton,
            Meisui,
            FleetingRaiju,
            ZeshoMeppo,
            TenriJendo,
            FleetingRaiju,
            ArmorCrush,
            Bhavacakra,
            Ten,
            ChiCombo,
            Raiton,
            FleetingRaiju,
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            11
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Ten) < 1) return false;
            if (CustomComboFunctions.IsOnCooldown(Mug)) return false;
            if (CustomComboFunctions.IsOnCooldown(TenChiJin)) return false;
            if (CustomComboFunctions.IsOnCooldown(PhantomKamaitachi)) return false;
            if (CustomComboFunctions.IsOnCooldown(Bunshin)) return false;
            if (CustomComboFunctions.IsOnCooldown(DreamWithinADream)) return false;
            if (CustomComboFunctions.IsOnCooldown(Kassatsu)) return false;
            if (CustomComboFunctions.IsOnCooldown(TrickAttack)) return false;

            return true;
        }
    }
}

