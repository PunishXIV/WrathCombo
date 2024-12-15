using WrathCombo.Core;
using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class MCHPvP
    {
        public const byte JobID = 31;

        public const uint
            BlastCharge = 29402,
            BlazingShot = 41468,
            Scattergun = 29404,
            Drill = 29405,
            BioBlaster = 29406,
            AirAnchor = 29407,
            ChainSaw = 29408,
            Wildfire = 29409,
            BishopTurret = 29412,
            AetherMortar = 29413,
            Analysis = 29414,
            MarksmanSpite = 29415,
            FullMetalField = 41469;

        public static class Buffs
        {
            public const ushort
                Heat = 3148,
                Overheated = 3149,
                DrillPrimed = 3150,
                BioblasterPrimed = 3151,
                AirAnchorPrimed = 3152,
                ChainSawPrimed = 3153,
                Analysis = 3158;
        }

        public static class Debuffs
        {
            public const ushort
                Wildfire = 1323;
        }

        public static class Config
        {
            public const string
                MCHPVP_MarksmanSpite = "MCHPVP_MarksmanSpite",
                MCHPVP_FMFOption = "MCHPVP_FMFOption",
                MCHPVP_Heat = "MCHPVP_Heat";

        }

        internal class MCHPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCHPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == BlastCharge)
                {
                    var canWeave = CanWeave(actionID);
                    var analysisStacks = GetRemainingCharges(Analysis);
                    var bigDamageStacks = GetRemainingCharges(OriginalHook(Drill));
                    var overheated = HasEffect(Buffs.Overheated);
                    var FMFOption = PluginConfiguration.GetCustomIntValue(Config.MCHPVP_FMFOption);

                    if (!PvPCommon.TargetImmuneToDamage())
                    {
                        // MarksmanSpite execute condition - todo add config
                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_MarksmanSpite) && EnemyHealthCurrentHp() < GetOptionValue(Config.MCHPVP_MarksmanSpite) && IsLB1Ready)
                            return MarksmanSpite;

                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_Wildfire) && canWeave && overheated && IsOffCooldown(Wildfire))
                            return OriginalHook(Wildfire);

                        // FullMetalField condition when not overheated or if overheated and FullMetalField is off cooldown
                        if(IsEnabled(CustomComboPreset.MCHPvP_BurstMode_FullMetalField) && IsOffCooldown(FullMetalField))
                        {
                            if (FMFOption == 1)
                            {
                                if (!overheated && IsOffCooldown(Wildfire))
                                    return FullMetalField;
                            }
                            if (FMFOption == 2)
                            {
                                if (overheated)
                                    return FullMetalField;
                            }
                        }

                        // If overheated, BlazingShot is the next action
                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_BlazingShot) && overheated)
                            return OriginalHook(BlazingShot);

                        // Check if primed buffs and analysis conditions are met
                        bool hasPrimedBuffs = HasEffect(Buffs.DrillPrimed) ||
                                              (HasEffect(Buffs.ChainSawPrimed) && !IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AltAnalysis)) ||
                                              (HasEffect(Buffs.AirAnchorPrimed) && IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AltAnalysis));

                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_Analysis))
                        {
                            if (hasPrimedBuffs && !HasEffect(Buffs.Analysis) && analysisStacks > 0 &&
                                (!IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AltDrill) || IsOnCooldown(Wildfire)) &&
                                !canWeave && !overheated && bigDamageStacks > 0)
                            {
                                return OriginalHook(Analysis);
                            }
                        }

                        // BigDamageStacks logic with checks for primed buffs
                        if (bigDamageStacks > 0)
                        {
                            if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_Drill) && HasEffect(Buffs.DrillPrimed))
                                return OriginalHook(Drill);

                            if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_BioBlaster) && HasEffect(Buffs.BioblasterPrimed) && GetTargetDistance() <= 12)
                                return OriginalHook(BioBlaster);

                            if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_AirAnchor) && HasEffect(Buffs.AirAnchorPrimed))
                                return OriginalHook(AirAnchor);

                            if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_ChainSaw) && HasEffect(Buffs.ChainSawPrimed))
                                return OriginalHook(ChainSaw);
                        }
                    }

                }

                return actionID;
            }
        }
    }
}
