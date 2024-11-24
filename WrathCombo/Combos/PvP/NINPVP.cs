using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class NINPvP
    {
        public const byte ClassID = 29;
        public const byte JobID = 30;

        internal const uint
            SpinningEdge = 29500,
            GustSlash = 29501,
            AeolianEdge = 29502,
            FumaShuriken = 29505,
            Dokumori = 41451,
            ThreeMudra = 29507,
            Bunshin = 29511,
            Shukuchi = 29513,
            SeitonTenchu = 29515,
            ForkedRaiju = 29510,
            FleetingRaiju = 29707,
            HyoshoRanryu = 29506,
            GokaMekkyaku = 29504,
            Meisui = 29508,
            Huton = 29512,
            Doton = 29514,
            Assassinate = 29503,
            ZeshoMeppo = 41452;

        internal class Buffs
        {
            internal const ushort
                ThreeMudra = 1317,
                Hidden = 1316,
                Bunshin = 2010,
                ShadeShift = 2011,
                ZeshoMeppoReady = 4305;
        }

        internal class Debuffs
        {
            internal const ushort
                SealedHyoshoRanryu = 3194,
                SealedGokaMekkyaku = 3193,
                SealedHuton = 3196,
                SealedDoton = 3197,
                SeakedForkedRaiju = 3195,
                SealedMeisui = 3198,
                Dokumori = 4303;
        }

        internal class Config
        {
            internal const string
                NINPvP_Meisui_ST = "NINPvP_Meisui_ST",
                NINPvP_Meisui_AoE = "NINPvP_Meisui_AoE",
                NINPVP_SeitonTenchu = "NINPVP_SeitonTenchu";
        }

        internal class NINPvP_ST_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NINPvP_ST_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is SpinningEdge or GustSlash or AeolianEdge)
                {
                    // Cached variables for repeated conditions
                    var threeMudrasCD = GetCooldown(ThreeMudra);
                    var fumaCD = GetCooldown(FumaShuriken);
                    var bunshinStacks = HasEffect(Buffs.Bunshin) ? GetBuffStacks(Buffs.Bunshin) : 0;
                    bool canWeave = CanWeave(SpinningEdge);
                    bool mudraMode = HasEffect(Buffs.ThreeMudra);
                    bool inMeleeRange = InMeleeRange();
                    bool isHidden = HasEffect(Buffs.Hidden);
                    var jobMaxHp = LocalPlayer.MaxHp;
                    var maxHPThreshold = jobMaxHp - 8000;
                    float remainingPercentage = (float)LocalPlayer.CurrentHp / maxHPThreshold;
                    bool inMeisuiRange = GetOptionValue(Config.NINPvP_Meisui_ST) >= (remainingPercentage * 100);

                    // Hidden state actions
                    if (isHidden)
                        return OriginalHook(Assassinate);

                    if (!PvPCommon.IsImmuneToDamage())
                    {

                        // Seiton Tenchu priority for targets below 50% HP
                        if (IsEnabled(CustomComboPreset.NINPvP_ST_SeitonTenchu) && GetTargetHPPercent() < GetOptionValue(Config.NINPVP_SeitonTenchu) && IsLB1Ready)
                            return OriginalHook(SeitonTenchu);

                        // Zesho Meppo
                        if (HasEffect(Buffs.ZeshoMeppoReady) && InMeleeRange())
                            return ZeshoMeppo;

                        if (canWeave)
                        {
                            // Melee range actions
                            if (IsEnabled(CustomComboPreset.NINPvP_ST_Dokumori) && inMeleeRange && !GetCooldown(Dokumori).IsCooldown)
                                return OriginalHook(Dokumori);

                            // Bunshin
                            if (IsEnabled(CustomComboPreset.NINPvP_ST_Bunshin) && !GetCooldown(Bunshin).IsCooldown)
                                return OriginalHook(Bunshin);

                            // Three Mudra
                            if (IsEnabled(CustomComboPreset.NINPvP_ST_ThreeMudra) && threeMudrasCD.RemainingCharges > 0 && !mudraMode)
                                return OriginalHook(ThreeMudra);
                        }

                        // Mudra mode actions
                        if (mudraMode)
                        {
                            if (IsEnabled(CustomComboPreset.NINPvP_ST_Meisui) && inMeisuiRange && !HasEffect(Debuffs.SealedMeisui))
                                return OriginalHook(Meisui);

                            if (!HasEffect(Debuffs.SealedHyoshoRanryu))
                                return OriginalHook(HyoshoRanryu);

                            if (!HasEffect(Debuffs.SeakedForkedRaiju) && bunshinStacks > 0)
                                return OriginalHook(ForkedRaiju);

                            if (!HasEffect(Debuffs.SealedHuton))
                                return OriginalHook(Huton);
                        }

                        // Fuma Shuriken
                        if (IsEnabled(CustomComboPreset.NINPvP_ST_FumaShuriken) && fumaCD.RemainingCharges > 0)
                            return OriginalHook(FumaShuriken);
                    }

                }

                return actionID;

            }
        }

        internal class NINPvP_AoE_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NINPvP_AoE_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == FumaShuriken)
                {
                    var threeMudrasCD = GetCooldown(ThreeMudra);
                    var fumaCD = GetCooldown(FumaShuriken);
                    bool meisuiLocked = HasEffect(Debuffs.SealedMeisui);
                    bool dotonLocked = HasEffect(Debuffs.SealedDoton);
                    bool gokaLocked = HasEffect(Debuffs.SealedGokaMekkyaku);
                    bool mudraMode = HasEffect(Buffs.ThreeMudra);
                    bool canWeave = CanWeave(SpinningEdge);
                    var jobMaxHp = LocalPlayer.MaxHp;
                    var threshold = GetOptionValue(Config.NINPvP_Meisui_AoE);
                    var maxHPThreshold = jobMaxHp - 8000;
                    var remainingPercentage = (float)LocalPlayer.CurrentHp / (float)maxHPThreshold;
                    bool inMeisuiRange = threshold >= (remainingPercentage * 100);

                    if (HasEffect(Buffs.Hidden))
                        return OriginalHook(Assassinate);

                    if (!PvPCommon.IsImmuneToDamage())
                    {
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.NINPvP_AoE_Dokumori) && InMeleeRange() && !GetCooldown(Dokumori).IsCooldown)
                                return OriginalHook(Dokumori);

                            if (IsEnabled(CustomComboPreset.NINPvP_AoE_Bunshin) && !GetCooldown(Bunshin).IsCooldown)
                                return OriginalHook(Bunshin);

                            if (IsEnabled(CustomComboPreset.NINPvP_AoE_ThreeMudra) && threeMudrasCD.RemainingCharges > 0 && !mudraMode)
                                return OriginalHook(ThreeMudra);
                        }

                        if (mudraMode)
                        {
                            if (IsEnabled(CustomComboPreset.NINPvP_AoE_Meisui) && inMeisuiRange && !meisuiLocked)
                                return OriginalHook(Meisui);

                            if (!dotonLocked)
                                return OriginalHook(Doton);

                            if (!gokaLocked)
                                return OriginalHook(GokaMekkyaku);
                        }

                        if (InMeleeRange())
                        {
                            if (lastComboActionID == GustSlash)
                                return OriginalHook(AeolianEdge);

                            if (lastComboActionID == SpinningEdge)
                                return OriginalHook(GustSlash);

                            return OriginalHook(SpinningEdge);
                        }

                        if (IsEnabled(CustomComboPreset.NINPvP_AoE_FumaShuriken) && fumaCD.RemainingCharges > 0)
                            return OriginalHook(FumaShuriken);
                    }
                }

                return actionID;
            }
        }
    }
}
