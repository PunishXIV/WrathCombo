using Dalamud.Game.ClientState.Statuses;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    #region ID's

    public const byte ClassID = 29;
    public const byte JobID = 30;

    public const uint
        SpinningEdge = 2240,
        ShadeShift = 2241,
        GustSlash = 2242,
        Hide = 2245,
        Assassinate = 2246,
        ThrowingDaggers = 2247,
        Mug = 2248,
        DeathBlossom = 2254,
        AeolianEdge = 2255,
        TrickAttack = 2258,
        Kassatsu = 2264,
        ArmorCrush = 3563,
        DreamWithinADream = 3566,
        TenChiJin = 7403,
        Bhavacakra = 7402,
        HakkeMujinsatsu = 16488,
        Meisui = 16489,
        Bunshin = 16493,
        PhantomKamaitachi = 25774,
        ForkedRaiju = 25777,
        FleetingRaiju = 25778,
        Hellfrog = 7401,
        HollowNozuchi = 25776,
        TenriJendo = 36961,
        KunaisBane = 36958,
        ZeshoMeppo = 36960,
        Dokumori = 36957,

        //Mudras
        Ninjutsu = 2260,
        Rabbit = 2272,

        //-- initial state mudras (the ones with charges)
        Ten = 2259,
        Chi = 2261,
        Jin = 2263,

        //-- mudras used for combos (the ones used while you have the mudra buff)
        TenCombo = 18805,
        ChiCombo = 18806,
        JinCombo = 18807,

        //Ninjutsu
        FumaShuriken = 2265,
        Hyoton = 2268,
        Doton = 2270,
        Katon = 2266,
        Suiton = 2271,
        Raiton = 2267,
        Huton = 2269,
        GokaMekkyaku = 16491,
        HyoshoRanryu = 16492,

        //TCJ Jutsus (why they have another ID I will never know)
        TCJFumaShurikenTen = 18873,
        TCJFumaShurikenChi = 18874,
        TCJFumaShurikenJin = 18875,
        TCJKaton = 18876,
        TCJRaiton = 18877,
        TCJHyoton = 18878,
        TCJHuton = 18879,
        TCJDoton = 18880,
        TCJSuiton = 18881;

    public static class Buffs
    {
        public const ushort
            Mudra = 496,
            Kassatsu = 497,
            //Suiton = 507,
            Higi = 3850,
            TenriJendo = 3851,
            ShadowWalker = 3848,
            Hidden = 614,
            TenChiJin = 1186,
            AssassinateReady = 1955,
            RaijuReady = 2690,
            PhantomReady = 2723,
            Meisui = 2689,
            Doton = 501,
            Bunshin = 1954;
    }

    public static class Debuffs
    {
        public const ushort
            Dokumori = 3849,
            TrickAttack = 3254,
            KunaisBane = 3906,
            Mug = 638;
    }

    public static class Traits
    {
        public const uint
            EnhancedKasatsu = 250;
    }

    #endregion

    internal static bool InMudra = false;
    internal static NINOpenerMaxLevel4thGCDKunai Opener1 = new();
    internal static NINOpenerMaxLevel3rdGCDDokumori Opener2 = new();
    internal static NINOpenerMaxLevel3rdGCDKunai Opener3 = new();

    internal static WrathOpener Opener()
    {
        if (IsEnabled(CustomComboPreset.NIN_ST_AdvancedMode))
        {
            if (Config.NIN_Adv_Opener_Selection == 0 && Opener1.LevelChecked)
                return Opener1;

            if (Config.NIN_Adv_Opener_Selection == 1 && Opener2.LevelChecked)
                return Opener2;

            if (Config.NIN_Adv_Opener_Selection == 2 && Opener3.LevelChecked)
                return Opener3;
        }

        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal static bool OriginalJutsu => IsOriginal(Ninjutsu);

    internal static bool TrickDebuff => TargetHasTrickDebuff();

    internal static bool MugDebuff => TargetHasMugDebuff();

    private static bool TargetHasTrickDebuff()
    {
        return TargetHasEffect(Debuffs.TrickAttack) ||
               TargetHasEffect(Debuffs.KunaisBane);
    }

    private static bool TargetHasMugDebuff()
    {
        return TargetHasEffect(Debuffs.Mug) ||
               TargetHasEffect(Debuffs.Dokumori);
    }

    public static Status? MudraBuff => FindEffect(Buffs.Mudra);

    public static uint CurrentNinjutsu => OriginalHook(Ninjutsu);

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
            if (InMudra)
                return true;

            float gcd = GetCooldown(GustSlash).CooldownTotal;

            if (gcd == 0.5)
                return true;

            if (GetRemainingCharges(Ten) == 0 &&
                !HasEffect(Buffs.Mudra) &&
                !HasEffect(Buffs.Kassatsu))
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
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
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
                    actionID = OriginalHook(Chi);

                    return true;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
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
                    actionID = OriginalHook(Ten);

                    return true;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Chi);
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
                if (!CanCast() || HasEffect(Buffs.Kassatsu) || ActionWatching.LastAction == Hyoton)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
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
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Ten);

                    return true;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Chi);
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
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Chi);

                    return true;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
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
                    actionID = OriginalHook(Chi);

                    return true;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Ten);
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
                if (!CanCast() || !HasEffect(Buffs.Kassatsu) || ActionWatching.LastAction == GokaMekkyaku)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Ten);

                    return true;
                }

                if (ActionWatching.LastAction == TenCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Chi);
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
                if (!CanCast() || !HasEffect(Buffs.Kassatsu) || ActionWatching.LastAction == HyoshoRanryu)
                {
                    CurrentMudra = MudraState.None;

                    return false;
                }

                if (ActionWatching.LastAction == ChiCombo)
                {
                    actionID = OriginalHook(Jin);

                    return true;
                }

                if (ActionWatching.LastAction == JinCombo)
                {
                    actionID = OriginalHook(Ninjutsu);

                    return true;
                }

                actionID = OriginalHook(Chi);
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

            if (ActionWatching.LastAction == FumaShuriken ||
                 ActionWatching.LastAction == Katon ||
                 ActionWatching.LastAction == Raiton ||
                 ActionWatching.LastAction == Hyoton ||
                 ActionWatching.LastAction == Huton ||
                 ActionWatching.LastAction == Doton ||
                 ActionWatching.LastAction == Suiton ||
                 ActionWatching.LastAction == GokaMekkyaku ||
                 ActionWatching.LastAction == HyoshoRanryu)
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

        internal override UserData? ContentCheckConfig => Config.NIN_Balance_Content;

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Ten) < 1) return false;
            if (IsOnCooldown(Mug)) return false;
            if (IsOnCooldown(TenChiJin)) return false;
            if (IsOnCooldown(PhantomKamaitachi)) return false;
            if (IsOnCooldown(Bunshin)) return false;
            if (IsOnCooldown(DreamWithinADream)) return false;
            if (IsOnCooldown(Kassatsu)) return false;
            if (IsOnCooldown(TrickAttack)) return false;

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

        internal override UserData? ContentCheckConfig => Config.NIN_Balance_Content;

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Ten) < 1) return false;
            if (IsOnCooldown(Mug)) return false;
            if (IsOnCooldown(TenChiJin)) return false;
            if (IsOnCooldown(PhantomKamaitachi)) return false;
            if (IsOnCooldown(Bunshin)) return false;
            if (IsOnCooldown(DreamWithinADream)) return false;
            if (IsOnCooldown(Kassatsu)) return false;
            if (IsOnCooldown(TrickAttack)) return false;

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

        internal override UserData? ContentCheckConfig => Config.NIN_Balance_Content;
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Ten) < 1) return false;
            if (IsOnCooldown(Mug)) return false;
            if (IsOnCooldown(TenChiJin)) return false;
            if (IsOnCooldown(PhantomKamaitachi)) return false;
            if (IsOnCooldown(Bunshin)) return false;
            if (IsOnCooldown(DreamWithinADream)) return false;
            if (IsOnCooldown(Kassatsu)) return false;
            if (IsOnCooldown(TrickAttack)) return false;

            return true;
        }
    }
}

