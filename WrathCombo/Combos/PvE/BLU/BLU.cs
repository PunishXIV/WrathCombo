#region

using Dalamud.Game.ClientState.Conditions;
using WrathCombo.CustomComboNS;
using Options = WrathCombo.Combos.CustomComboPreset;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

#endregion

namespace WrathCombo.Combos.PvE
{
    internal partial class BLU
    {
        public const byte JobID = 36;

        public const byte BypassAction = 2;

        #region Abilities

        public const uint
            WaterCannon_Spell1 = 11385,
            FlameThrower_Spell2 = 11402,
            AquaBreath_Spell3 = 11390,
            FlyingFrenzy_Spell4 = 11389,
            DrillCannons_Spell5 = 11398,
            HighVoltage_Spell6 = 11387,
            Loom_Spell7 = 11401,
            FinalSting_Spell8 = 11407,
            SongofTorment_Spell9 = 11386,
            Glower_Spell10 = 11404,
            Plaincracker_Spell11 = 11391,
            Bristle_Spell12 = 11393,
            WhiteWind_Spell13 = 11406,
            LevelfivePetrify_Spell14 = 11414,
            SharpenedKnife_Spell15 = 11400,
            IceSpikes_Spell16 = 11418,
            BloodDrain_Spell17 = 11395,
            AcornBomb_Spell18 = 11392,
            BombToss_Spell19 = 11396,
            Offguard_Spell20 = 11411,
            Selfdestruct_Spell21 = 11408,
            Transfusion_Spell22 = 11409,
            Faze_Spell23 = 11403,
            FlyingSardine_Spell24 = 11423,
            Snort_Spell25 = 11383,
            FourTonzeWeight_Spell26 = 11384,
            TheLook_Spell27 = 11399,
            BadBreath_Spell28 = 11388,
            Diamondback_Spell29 = 11424,
            MightyGuard_Spell30 = 11417,
            StickyTongue_Spell31 = 11412,
            ToadOil_Spell32 = 11410,
            RamsVoice_Spell33 = 11419,
            TheDragonsVoice_Spell34 = 11420,
            Missile_Spell35 = 11405,
            ThousandNeedles_Spell36 = 11397,
            InkJet_Spell37 = 11422,
            FireAngon_Spell38 = 11425,
            MoonFlute_Spell39 = 11415,
            TailScrew_Spell40 = 11413,
            MindBlast_Spell41 = 11394,
            Doom_Spell42 = 11416,
            PeculiarLight_Spell43 = 11421,
            FeatherRain_Spell44 = 11426,
            Eruption_Spell45 = 11427,
            MountainBuster_Spell46 = 11428,
            ShockStrike_Spell47 = 11429,
            GlassDance_Spell48 = 11430,
            VeiloftheWhorl_Spell49 = 11431,
            AlpineDraft_Spell50 = 18295,
            ProteanWave_Spell51 = 18296,
            Northerlies_Spell52 = 18297,
            Electrogenesis_Spell53 = 18298,
            Kaltstrahl_Spell54 = 18299,
            AbyssalTransfixion_Spell55 = 18300,
            Chirp_Spell56 = 18301,
            EerieSoundwave_Spell57 = 18302,
            PomCure_Spell58 = 18303,
            Gobskin_Spell59 = 18304,
            MagicHammer_Spell60 = 18305,
            Avail_Spell61 = 18306,
            FrogLegs_Spell62 = 18307,
            SonicBoom_Spell63 = 18308,
            Whistle_Spell64 = 18309,
            WhiteKnightsTour_Spell65 = 18310,
            BlackKnightsTour_Spell66 = 18311,
            LevelfiveDeath_Spell67 = 18312,
            Launcher_Spell68 = 18313,
            PerpetualRay_Spell69 = 18314,
            Cactguard_Spell70 = 18315,
            RevengeBlast_Spell71 = 18316,
            AngelWhisper_Spell72 = 18317,
            Exuviation_Spell73 = 18318,
            Reflux_Spell74 = 18319,
            Devour_Spell75 = 18320,
            CondensedLibra_Spell76 = 18321,
            AethericMimicry_Spell77 = 18322,
            Surpanakha_Spell78 = 18323,
            Quasar_Spell79 = 18324,
            JKick_Spell80 = 18325,
            TripleTrident_Spell81 = 23264,
            Tingle_Spell82 = 23265,
            Tatamigaeshi_Spell83 = 23266,
            ColdFog_Spell84 = 23267,
            Stotram_Spell85 = 23269,
            SaintlyBeam_Spell86 = 23270,
            FeculentFlood_Spell87 = 23271,
            AngelsSnack_Spell88 = 23272,
            ChelonianGate_Spell89 = 23273,
            RoseofDestruction_Spell90 = 23275,
            BasicInstinct_Spell91 = 23276,
            Ultravibration_Spell92 = 23277,
            Blaze_Spell93 = 23278,
            MustardBomb_Spell94 = 23279,
            DragonForce_Spell95 = 23280,
            AetherialSpark_Spell96 = 23281,
            HydroPull_Spell97 = 23282,
            MaledictionofWater_Spell98 = 23283,
            ChocoMeteor_Spell99 = 23284,
            MatraMagic_Spell100 = 23285,
            PeripheralSynthesis_Spell101 = 23286,
            BothEnds_Spell102 = 23287,
            PhantomFlurry_Spell103 = 23288,
            Nightbloom_Spell104 = 23290,
            GoblinPunch_Spell105 = 34563,
            RightRound_Spell106 = 34564,
            Schiltron_Spell107 = 34565,
            Rehydration_Spell108 = 34566,
            BreathofMagic_Spell109 = 34567,
            WildRage_Spell110 = 34568,
            PeatPelt_Spell111 = 34569,
            DeepClean_Spell112 = 34570,
            RubyDynamics_Spell113 = 34571,
            DivinationRune_Spell114 = 34572,
            DimensionalShift_Spell115 = 34573,
            ConvictionMarcato_Spell116 = 34574,
            ForceField_Spell117 = 34575,
            WingedReprobation_Spell118 = 34576,
            LaserEye_Spell119 = 34577,
            CandyCane_Spell120 = 34578,
            MortalFlame_Spell121 = 34579,
            SeaShanty_Spell122 = 34580,
            Apokalypsis_Spell123 = 34581,
            BeingMortal_Spell124 = 34582;

        #endregion

        public static class Buffs
        {
            public const ushort
                MoonFlute = 1718,
                Bristle = 1716,
                WaningNocturne = 1727,
                PhantomFlurry = 2502,
                Tingle = 2492,
                Whistle = 2118,
                TankMimicry = 2124,
                DPSMimicry = 2125,
                BasicInstinct = 2498,
                WingedReprobation = 3640;
        }

        public static class Debuffs
        {
            public const ushort
                Slow = 9,
                Bind = 13,
                BadBreath = 18,
                Stun = 142,
                SongOfTorment = 1714,
                NightBloom = 1714,
                FeatherRain = 1723,
                DeepFreeze = 1731,
                Offguard = 1717,
                Malodorous = 1715,
                Conked = 2115,
                Lightheaded = 2501,
                MortalFlame = 3643,
                BreathOfMagic = 3712,
                Begrimed = 3636;
        }

        internal enum DoT
        {
            [DoTInfo(
                Debuffs.SongOfTorment,
                SongofTorment_Spell9,
                Options.BLU_DPS_DoT)]
            DPS_SongOfTorment,

            [DoTInfo(
                Debuffs.BreathOfMagic,
                BreathofMagic_Spell109,
                Options.BLU_DPS_DoT_Breath)]
            DPS_BreathOfMagic,

            [DoTInfo(
                Debuffs.MortalFlame,
                MortalFlame_Spell121,
                Options.BLU_DPS_DoT_Flame)]
            DPS_MortalFlame,

            [DoTInfo(
                Debuffs.FeatherRain,
                FeatherRain_Spell44,
                Options.BLU_Tank_DoT)]
            Tank_FeatherRain,

            [DoTInfo(
                Debuffs.SongOfTorment,
                SongofTorment_Spell9,
                Options.BLU_Tank_DoT_Torment)]
            Tank_SongOfTorment,

            [DoTInfo(
                Debuffs.BadBreath,
                BadBreath_Spell28,
                Options.BLU_Tank_DoT_Bad)]
            Tank_BadBreath,

            [DoTInfo(
                Debuffs.BreathOfMagic,
                BreathofMagic_Spell109,
                Options.BLU_Tank_DoT_Breath)]
            Tank_BreathOfMagic,

            [DoTInfo(
                Debuffs.MortalFlame,
                MortalFlame_Spell121,
                Options.BLU_Tank_DoT_Flame)]
            Tank_MortalFlame
        }

        #region Openers

        internal class BLU_NewMoonFluteOpener : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_NewMoonFluteOpener;

            protected override uint Invoke(uint actionID, uint lastComboActionID,
                float comboTime, byte level)
            {
                if (actionID is MoonFlute_Spell39)
                {
                    if (!HasEffect(Buffs.MoonFlute))
                    {
                        if (IsSpellActive(Whistle_Spell64) &&
                            !HasEffect(Buffs.Whistle) &&
                            !WasLastAction(Whistle_Spell64))
                            return Whistle_Spell64;

                        if (IsSpellActive(Tingle_Spell82) &&
                            !HasEffect(Buffs.Tingle))
                            return Tingle_Spell82;

                        if (IsSpellActive(RoseofDestruction_Spell90) &&
                            GetCooldown(RoseofDestruction_Spell90)
                                .CooldownRemaining < 1f)
                            return RoseofDestruction_Spell90;

                        if (IsSpellActive(MoonFlute_Spell39))
                            return MoonFlute_Spell39;
                    }

                    if (IsSpellActive(JKick_Spell80) && IsOffCooldown(JKick_Spell80))
                        return JKick_Spell80;

                    if (IsSpellActive(TripleTrident_Spell81) &&
                        IsOffCooldown(TripleTrident_Spell81))
                        return TripleTrident_Spell81;

                    if (IsSpellActive(Nightbloom_Spell104) &&
                        IsOffCooldown(Nightbloom_Spell104))
                        return Nightbloom_Spell104;

                    if (IsEnabled(CustomComboPreset
                            .BLU_NewMoonFluteOpener_DoTOpener))
                    {
                        if ((!TargetHasEffectAny(Debuffs.BreathOfMagic) &&
                             IsSpellActive(BreathofMagic_Spell109)) ||
                            (!TargetHasEffectAny(Debuffs.MortalFlame) &&
                             IsSpellActive(MortalFlame_Spell121)))
                        {
                            if (IsSpellActive(Bristle_Spell12) &&
                                !HasEffect(Buffs.Bristle))
                                return Bristle_Spell12;

                            if (IsSpellActive(FeatherRain_Spell44) &&
                                IsOffCooldown(FeatherRain_Spell44))
                                return FeatherRain_Spell44;

                            if (IsSpellActive(SeaShanty_Spell122) &&
                                IsOffCooldown(SeaShanty_Spell122))
                                return SeaShanty_Spell122;

                            if (IsSpellActive(BreathofMagic_Spell109) &&
                                !TargetHasEffectAny(Debuffs.BreathOfMagic))
                                return BreathofMagic_Spell109;
                            if (IsSpellActive(MortalFlame_Spell121) &&
                                !TargetHasEffectAny(Debuffs.MortalFlame))
                                return MortalFlame_Spell121;
                        }
                    }
                    else
                    {
                        if (IsSpellActive(WingedReprobation_Spell118) &&
                            IsOffCooldown(WingedReprobation_Spell118) &&
                            !WasLastSpell(WingedReprobation_Spell118) &&
                            !WasLastAbility(FeatherRain_Spell44) &&
                            (!HasEffect(Buffs.WingedReprobation) ||
                             FindEffect(Buffs.WingedReprobation)?.StackCount < 2))
                            return WingedReprobation_Spell118;

                        if (IsSpellActive(FeatherRain_Spell44) &&
                            IsOffCooldown(FeatherRain_Spell44))
                            return FeatherRain_Spell44;

                        if (IsSpellActive(SeaShanty_Spell122) &&
                            IsOffCooldown(SeaShanty_Spell122))
                            return SeaShanty_Spell122;
                    }

                    if (IsSpellActive(WingedReprobation_Spell118) &&
                        IsOffCooldown(WingedReprobation_Spell118) &&
                        !WasLastAbility(ShockStrike_Spell47) &&
                        FindEffect(Buffs.WingedReprobation)?.StackCount < 2)
                        return WingedReprobation_Spell118;

                    if (IsSpellActive(ShockStrike_Spell47) &&
                        IsOffCooldown(ShockStrike_Spell47))
                        return ShockStrike_Spell47;

                    if (IsSpellActive(BeingMortal_Spell124) &&
                        IsOffCooldown(BeingMortal_Spell124) &&
                        IsNotEnabled(CustomComboPreset
                            .BLU_NewMoonFluteOpener_DoTOpener))
                        return BeingMortal_Spell124;

                    if (IsSpellActive(Bristle_Spell12) &&
                        !HasEffect(Buffs.Bristle) &&
                        IsOffCooldown(MatraMagic_Spell100) &&
                        IsSpellActive(MatraMagic_Spell100))
                        return Bristle_Spell12;

                    if (IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (IsSpellActive(Surpanakha_Spell78) &&
                        GetRemainingCharges(Surpanakha_Spell78) > 0)
                        return Surpanakha_Spell78;

                    if (IsSpellActive(MatraMagic_Spell100) &&
                        HasEffect(All.Buffs.Swiftcast))
                        return MatraMagic_Spell100;

                    if (IsSpellActive(BeingMortal_Spell124) &&
                        IsOffCooldown(BeingMortal_Spell124) &&
                        IsEnabled(CustomComboPreset
                            .BLU_NewMoonFluteOpener_DoTOpener))
                        return BeingMortal_Spell124;

                    if (IsSpellActive(PhantomFlurry_Spell103) &&
                        IsOffCooldown(PhantomFlurry_Spell103))
                        return PhantomFlurry_Spell103;

                    if (HasEffect(Buffs.PhantomFlurry) &&
                        FindEffect(Buffs.PhantomFlurry)?.RemainingTime < 2)
                        return OriginalHook(PhantomFlurry_Spell103);

                    if (HasEffect(Buffs.MoonFlute))
                        return OriginalHook(11);
                }

                return actionID;
            }
        }

        #endregion

        #region Primal Combo

        internal class BLU_PrimalCombo : CustomCombo
        {
            internal static bool surpanakhaReady;

            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_PrimalCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is FeatherRain_Spell44 or Eruption_Spell45)
                {
                    if (HasEffect(Buffs.PhantomFlurry))
                        return OriginalHook(PhantomFlurry_Spell103);

                    if (!HasEffect(Buffs.PhantomFlurry))
                    {
                        if (IsEnabled(CustomComboPreset
                                .BLU_PrimalCombo_WingedReprobation) &&
                            FindEffect(Buffs.WingedReprobation)?.StackCount > 1 &&
                            IsOffCooldown(WingedReprobation_Spell118))
                            return OriginalHook(WingedReprobation_Spell118);

                        if (IsOffCooldown(FeatherRain_Spell44) &&
                            IsSpellActive(FeatherRain_Spell44) &&
                            (IsNotEnabled(Options.BLU_PrimalCombo_Pool) ||
                             (IsEnabled(Options.BLU_PrimalCombo_Pool) &&
                              (GetCooldownRemainingTime(Nightbloom_Spell104) > 30 ||
                               IsOffCooldown(Nightbloom_Spell104)))))
                            return FeatherRain_Spell44;
                        if (IsOffCooldown(Eruption_Spell45) &&
                            IsSpellActive(Eruption_Spell45) &&
                            (IsNotEnabled(Options.BLU_PrimalCombo_Pool) ||
                             (IsEnabled(Options.BLU_PrimalCombo_Pool) &&
                              (GetCooldownRemainingTime(Nightbloom_Spell104) > 30 ||
                               IsOffCooldown(Nightbloom_Spell104)))))
                            return Eruption_Spell45;
                        if (IsOffCooldown(ShockStrike_Spell47) &&
                            IsSpellActive(ShockStrike_Spell47) &&
                            (IsNotEnabled(Options.BLU_PrimalCombo_Pool) ||
                             (IsEnabled(Options.BLU_PrimalCombo_Pool) &&
                              (GetCooldownRemainingTime(Nightbloom_Spell104) > 60 ||
                               IsOffCooldown(Nightbloom_Spell104)))))
                            return ShockStrike_Spell47;
                        if (IsOffCooldown(RoseofDestruction_Spell90) &&
                            IsSpellActive(RoseofDestruction_Spell90) &&
                            (IsNotEnabled(Options.BLU_PrimalCombo_Pool) ||
                             (IsEnabled(Options.BLU_PrimalCombo_Pool) &&
                              (GetCooldownRemainingTime(Nightbloom_Spell104) > 30 ||
                               IsOffCooldown(Nightbloom_Spell104)))))
                            return RoseofDestruction_Spell90;
                        if (IsOffCooldown(GlassDance_Spell48) &&
                            IsSpellActive(GlassDance_Spell48) &&
                            (IsNotEnabled(Options.BLU_PrimalCombo_Pool) ||
                             (IsEnabled(Options.BLU_PrimalCombo_Pool) &&
                              (GetCooldownRemainingTime(Nightbloom_Spell104) > 90 ||
                               IsOffCooldown(Nightbloom_Spell104)))))
                            return GlassDance_Spell48;
                        if (IsEnabled(Options.BLU_PrimalCombo_JKick) &&
                            IsOffCooldown(JKick_Spell80) &&
                            IsSpellActive(JKick_Spell80) &&
                            (IsNotEnabled(Options.BLU_PrimalCombo_Pool) ||
                             (IsEnabled(Options.BLU_PrimalCombo_Pool) &&
                              (GetCooldownRemainingTime(Nightbloom_Spell104) > 60 ||
                               IsOffCooldown(Nightbloom_Spell104)))))
                            return JKick_Spell80;
                        if (IsEnabled(CustomComboPreset
                                .BLU_PrimalCombo_Nightbloom) &&
                            IsOffCooldown(Nightbloom_Spell104) &&
                            IsSpellActive(Nightbloom_Spell104))
                            return Nightbloom_Spell104;
                        if (IsEnabled(Options.BLU_PrimalCombo_Matra) &&
                            IsOffCooldown(MatraMagic_Spell100) &&
                            IsSpellActive(MatraMagic_Spell100))
                            return MatraMagic_Spell100;
                        if (IsEnabled(CustomComboPreset
                                .BLU_PrimalCombo_Suparnakha) &&
                            IsSpellActive(Surpanakha_Spell78))
                        {
                            if (GetRemainingCharges(Surpanakha_Spell78) == 4)
                                surpanakhaReady = true;
                            if (surpanakhaReady &&
                                GetRemainingCharges(Surpanakha_Spell78) > 0)
                                return Surpanakha_Spell78;
                            if (GetRemainingCharges(Surpanakha_Spell78) == 0)
                                surpanakhaReady = false;
                        }

                        if (IsEnabled(CustomComboPreset
                                .BLU_PrimalCombo_WingedReprobation) &&
                            IsSpellActive(WingedReprobation_Spell118) &&
                            IsOffCooldown(WingedReprobation_Spell118))
                            return OriginalHook(WingedReprobation_Spell118);

                        if (IsEnabled(Options.BLU_PrimalCombo_SeaShanty) &&
                            IsSpellActive(SeaShanty_Spell122) &&
                            IsOffCooldown(SeaShanty_Spell122))
                            return SeaShanty_Spell122;

                        if (IsEnabled(
                                Options.BLU_PrimalCombo_PhantomFlurry) &&
                            IsOffCooldown(PhantomFlurry_Spell103) &&
                            IsSpellActive(PhantomFlurry_Spell103))
                            return PhantomFlurry_Spell103;
                    }
                }

                return actionID;
            }
        }

        #endregion

        #region General Combos

        internal class BLU_FinalSting : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_FinalSting;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is FinalSting_Spell8)
                {
                    if (IsEnabled(Options.BLU_SoloMode) &&
                        HasCondition(ConditionFlag.BoundByDuty) &&
                        !HasEffect(Buffs.BasicInstinct) &&
                        GetPartyMembers().Count == 0 &&
                        IsSpellActive(BasicInstinct_Spell91))
                        return BasicInstinct_Spell91;
                    if (!HasEffect(Buffs.Whistle) &&
                        IsSpellActive(Whistle_Spell64) &&
                        !WasLastAction(Whistle_Spell64))
                        return Whistle_Spell64;
                    if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle_Spell82) &&
                        !WasLastSpell(Tingle_Spell82))
                        return Tingle_Spell82;
                    if (!HasEffect(Buffs.MoonFlute) &&
                        !WasLastSpell(MoonFlute_Spell39) &&
                        IsSpellActive(MoonFlute_Spell39))
                        return MoonFlute_Spell39;
                    if (IsEnabled(Options.BLU_Primals))
                    {
                        if (IsOffCooldown(RoseofDestruction_Spell90) &&
                            IsSpellActive(RoseofDestruction_Spell90))
                            return RoseofDestruction_Spell90;
                        if (IsOffCooldown(FeatherRain_Spell44) &&
                            IsSpellActive(FeatherRain_Spell44))
                            return FeatherRain_Spell44;
                        if (IsOffCooldown(Eruption_Spell45) &&
                            IsSpellActive(Eruption_Spell45))
                            return Eruption_Spell45;
                        if (IsOffCooldown(MatraMagic_Spell100) &&
                            IsSpellActive(MatraMagic_Spell100))
                            return MatraMagic_Spell100;
                        if (IsOffCooldown(GlassDance_Spell48) &&
                            IsSpellActive(GlassDance_Spell48))
                            return GlassDance_Spell48;
                        if (IsOffCooldown(ShockStrike_Spell47) &&
                            IsSpellActive(ShockStrike_Spell47))
                            return ShockStrike_Spell47;
                    }

                    if (IsOffCooldown(All.Swiftcast) && LevelChecked(All.Swiftcast))
                        return All.Swiftcast;
                    if (IsSpellActive(FinalSting_Spell8))
                        return FinalSting_Spell8;
                }

                return actionID;
            }
        }

        internal class BLU_Ultravibrate : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_Ultravibrate;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is not Ultravibration_Spell92) return actionID;

                // Setup for Ultravibration
                if (IsEnabled(Options.BLU_HydroPull) &&
                    !InMeleeRange() && IsSpellActive(HydroPull_Spell97))
                    return HydroPull_Spell97;
                if (!TargetHasEffectAny(Debuffs.DeepFreeze) &&
                    IsOffCooldown(Ultravibration_Spell92) &&
                    IsSpellActive(RamsVoice_Spell33))
                    return RamsVoice_Spell33;

                if (!TargetHasEffectAny(Debuffs.DeepFreeze)) return actionID;

                // Ultravibration
                if (IsOffCooldown(All.Swiftcast))
                    return All.Swiftcast;
                if (IsSpellActive(Ultravibration_Spell92) &&
                    IsOffCooldown(Ultravibration_Spell92))
                    return Ultravibration_Spell92;

                return actionID;
            }
        }

        #endregion

        #region DPS Combos

        internal class BLU_TridentCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_TridentCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is not TripleTrident_Spell81) return actionID;

                // Show cooldown
                if (IsOnCooldown(TripleTrident_Spell81))
                    return TripleTrident_Spell81;

                // Buff
                if (!HasEffect(Buffs.Whistle) && IsSpellActive(Whistle_Spell64))
                    return Whistle_Spell64;
                if (!HasEffect(Buffs.Tingle) && IsSpellActive(Tingle_Spell82) &&
                    HasEffect(Buffs.Whistle))
                    return Tingle_Spell82;

                // Triple Trident
                if (IsSpellActive(TripleTrident_Spell81) &&
                    HasEffect(Buffs.Tingle) && HasEffect(Buffs.Whistle))
                    return TripleTrident_Spell81;

                return actionID;
            }
        }

        internal class BLU_DPS_DoT : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_DPS_DoT;

            public uint getDoT() => Invoke(BypassAction, 0, 0, 0);

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is not (SongofTorment_Spell9 or BypassAction))
                    return actionID;

                var dotHelper = new DoTs(
                    Config.BLU_DPS_DoT_WasteProtection_HP,
                    Config.BLU_DPS_DoT_WasteProtection_Time,
                    [
                        DoT.DPS_SongOfTorment,
                        DoT.DPS_BreathOfMagic,
                        DoT.DPS_MortalFlame
                    ]);

                // Waste protection
                if (IsEnabled(Options.BLU_DPS_DoT_WasteProtection) &&
                    HasTarget() && !dotHelper.AnyDotsWanted())
                    return PLD.ShieldBash;

                // Buffed Song of Torment
                if (dotHelper.CheckDotWanted(DoT.DPS_SongOfTorment))
                {
                    if (!HasEffect(Buffs.Bristle) && IsSpellActive(Bristle_Spell12))
                        return Bristle_Spell12;
                    if (IsSpellActive(SongofTorment_Spell9))
                        return SongofTorment_Spell9;
                }

                // Breath of Magic
                if (IsEnabled(Options.BLU_DPS_DoT_Breath) &&
                    IsSpellActive(BreathofMagic_Spell109) &&
                    dotHelper.CheckDotWanted(DoT.DPS_BreathOfMagic))
                    return BreathofMagic_Spell109;

                // Mortal Flame
                if (IsEnabled(Options.BLU_DPS_DoT_Flame) &&
                    IsSpellActive(MortalFlame_Spell121) &&
                    dotHelper.CheckDotWanted(DoT.DPS_MortalFlame) &&
                    InCombat())
                    return MortalFlame_Spell121;

                return actionID;
            }
        }

        #endregion

        #region Tank Combos

        internal class BLU_Tank_Advanced : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_Tank_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is not GoblinPunch_Spell105) return actionID;

                // Pre Pull
                if (!InCombat() && HasTarget())
                {
                    if (IsEnabled(Options.BLU_Tank_Advanced_DoTs) &&
                        IsEnabled(Options.BLU_Tank_DoT_Torment))
                        return SongofTorment_Spell9;

                    if (IsEnabled(Options.BLU_Tank_Advanced_Uptime))
                        return SonicBoom_Spell63;
                }

                // Surpanakha dump
                if (IsEnabled(Options.BLU_Tank_Advanced_Surpanakha) &&
                    WasLastAction(Surpanakha_Spell78) &&
                    HasCharges(Surpanakha_Spell78))
                    return Surpanakha_Spell78;

                #region oGCDs

                if (CanWeave(actionID))
                {
                    // J Kick
                    if (IsEnabled(Options.BLU_Tank_Advanced_JKick) &&
                        IsOffCooldown(JKick_Spell80) &&
                        InMeleeRange() && IsSpellActive(JKick_Spell80))
                        return JKick_Spell80;

                    // Surpanakha
                    if (IsEnabled(Options.BLU_Tank_Advanced_Surpanakha) &&
                        CanWeave(actionID) &&
                        GetRemainingCharges(Surpanakha_Spell78) > 3)
                        return Surpanakha_Spell78;

                    // Lucid Dreaming
                    if (IsEnabled(Options.BLU_Tank_Advanced_Lucid) &&
                        IsOffCooldown(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= Config.BLU_Tank_Advanced_Lucid &&
                        LevelChecked(All.LucidDreaming))
                        return All.LucidDreaming;
                }

                #endregion

                // Uptime Sonic Boom
                if (IsEnabled(Options.BLU_Tank_Advanced_Uptime) &&
                    IsSpellActive(SonicBoom_Spell63) &&
                    IsOffCooldown(SonicBoom_Spell63) &&
                    !InMeleeRange())
                    return SonicBoom_Spell63;

                // Include DoTs
                BLU_Tank_DoT DoTCheck = new();
                if (IsEnabled(Options.BLU_Tank_Advanced_DoTs))
                {
                    var DoTCheckOutput = DoTCheck.getDoT();
                    if (DoTCheckOutput != BypassAction)
                        return DoTCheckOutput;
                }

                return actionID;
            }
        }

        internal class BLU_Tank_DoT : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_Tank_DoT;

            public uint getDoT() => Invoke(BypassAction, 0, 0, 0);

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is not (FeatherRain_Spell44 or BypassAction))
                    return actionID;

                var dotHelper = new DoTs(
                    Config.BLU_Tank_DoT_WasteProtection_HP,
                    Config.BLU_Tank_DoT_WasteProtection_Time,
                    [
                        DoT.Tank_FeatherRain,
                        DoT.Tank_SongOfTorment,
                        DoT.Tank_BadBreath,
                        DoT.Tank_BreathOfMagic,
                        DoT.Tank_MortalFlame
                    ]);

                // Waste protection
                if (IsEnabled(Options.BLU_Tank_DoT_WasteProtection) &&
                    HasTarget() && !dotHelper.AnyDotsWanted() && actionID != 2)
                    return PLD.ShieldBash;

                // Feather Rain
                if (dotHelper.CheckDotWanted(DoT.Tank_FeatherRain))
                    return FeatherRain_Spell44;

                // Buffed Song of Torment
                if (dotHelper.CheckDotWanted(DoT.Tank_SongOfTorment) &&
                    IsSpellActive(SongofTorment_Spell9))
                    return SongofTorment_Spell9;

                // Bad Breath
                if (IsEnabled(Options.BLU_Tank_DoT_Bad) &&
                    IsSpellActive(BadBreath_Spell28) &&
                    dotHelper.CheckDotWanted(DoT.Tank_BadBreath))
                    return BadBreath_Spell28;

                // Breath of Magic
                if (IsEnabled(Options.BLU_Tank_DoT_Breath) &&
                    IsSpellActive(BreathofMagic_Spell109) &&
                    dotHelper.CheckDotWanted(DoT.Tank_BreathOfMagic))
                    return BreathofMagic_Spell109;

                // Mortal Flame
                if (IsEnabled(Options.BLU_Tank_DoT_Flame) &&
                    IsSpellActive(MortalFlame_Spell121) &&
                    dotHelper.CheckDotWanted(DoT.Tank_MortalFlame) &&
                    InCombat())
                    return MortalFlame_Spell121;

                return actionID;
            }
        }

        internal class BLU_DebuffCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_DebuffCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is not Devour_Spell75)
                    return actionID;

                // Offguard
                if (!TargetHasEffectAny(Debuffs.Offguard) &&
                    IsOffCooldown(Offguard_Spell20) &&
                    IsSpellActive(Offguard_Spell20))
                    return Offguard_Spell20;

                // Bad Breath
                if (!TargetHasEffectAny(Debuffs.Malodorous) &&
                    HasEffect(Buffs.TankMimicry) && IsSpellActive(BadBreath_Spell28))
                    return BadBreath_Spell28;

                // Devour
                if (IsOffCooldown(Devour_Spell75) && HasEffect(Buffs.TankMimicry) &&
                    IsSpellActive(Devour_Spell75))
                    return Devour_Spell75;

                // Lucid Dreaming
                if (IsOffCooldown(All.LucidDreaming) &&
                    LocalPlayer.CurrentMp <= 9000 &&
                    LevelChecked(All.LucidDreaming))
                    return All.LucidDreaming;

                return actionID;
            }
        }

        internal class BLU_Addle : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_Addle;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                return (actionID is MagicHammer_Spell60 &&
                        IsOnCooldown(MagicHammer_Spell60) &&
                        IsOffCooldown(All.Addle) &&
                        !TargetHasEffect(All.Debuffs.Addle) &&
                        !TargetHasEffect(Debuffs.Conked))
                    ? All.Addle
                    : actionID;
            }
        }

        #endregion

        #region Healer Combos

        //

        #endregion

        #region Unsorted Combos

        internal class BLU_KnightCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_KnightCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is WhiteKnightsTour_Spell65 or BlackKnightsTour_Spell66)
                {
                    if (TargetHasEffect(Debuffs.Slow) &&
                        IsSpellActive(BlackKnightsTour_Spell66))
                        return BlackKnightsTour_Spell66;
                    if (TargetHasEffect(Debuffs.Bind) &&
                        IsSpellActive(WhiteKnightsTour_Spell65))
                        return WhiteKnightsTour_Spell65;
                }

                return actionID;
            }
        }

        internal class BLU_LightHeadedCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_LightHeadedCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                if (actionID is PeripheralSynthesis_Spell101)
                {
                    if (!TargetHasEffect(Debuffs.Lightheaded) &&
                        IsSpellActive(PeripheralSynthesis_Spell101))
                        return PeripheralSynthesis_Spell101;
                    if (TargetHasEffect(Debuffs.Lightheaded) &&
                        IsSpellActive(MustardBomb_Spell94))
                        return MustardBomb_Spell94;
                }

                return actionID;
            }
        }

        internal class BLU_PerpetualRayStunCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_PerpetualRayStunCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                return (actionID is PerpetualRay_Spell69 &&
                        (TargetHasEffectAny(Debuffs.Stun) ||
                         WasLastAction(PerpetualRay_Spell69)) &&
                        IsSpellActive(SharpenedKnife_Spell15) && InMeleeRange())
                    ? SharpenedKnife_Spell15
                    : actionID;
            }
        }

        internal class BLU_MeleeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_MeleeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove,
                float comboTime, byte level)
            {
                return (actionID is SonicBoom_Spell63 && GetTargetDistance() <= 3 &&
                        IsSpellActive(SharpenedKnife_Spell15))
                    ? SharpenedKnife_Spell15
                    : actionID;
            }
        }

        internal class BLU_PeatClean : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } =
                Options.BLU_PeatClean;

            protected override uint Invoke(uint actionID, uint lastComboActionID,
                float comboTime, byte level)
            {
                if (actionID is not DeepClean_Spell112) return actionID;

                if (IsSpellActive(PeatPelt_Spell111) &&
                    !TargetHasEffect(Debuffs.Begrimed))
                    return PeatPelt_Spell111;

                return actionID;
            }
        }

        #endregion
    }
}
