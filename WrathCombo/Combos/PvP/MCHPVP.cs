using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using ECommons.GameFunctions;
using WrathCombo.CustomComboNS.Functions;

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
            Ä§µ¯ÉäÊÖ = 29415,
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

        private static unsafe IGameObject? Ñ°ÕÒÄ§µ¯ÉäÊÖÄ¿±ê()
        {
            var query = Svc.Objects.Where(x => !x.IsDead && x.IsHostile() && x.IsTargetable);
            if (!query.Any())
                return null;

            IGameObject? target = null;

            foreach (var t in query) {
                //²»ÊÇ±ù
                if (CustomComboFunctions.GetTargetMaxHp(t) < 300000) {
                    //·¶Î§¹»
                    if (CustomComboFunctions.InActionRange(CustomComboFunctions.OriginalHook(Ä§µ¯ÉäÊÖ), t)) {
                        //²»ÃâÒß
                        if (!PvPCommon.TargetImmuneToDamage2(true, t)) {
                            //ÑªÁ¿µÍ
                            float x = CustomComboFunctions.EnemyHealthCurrentHp(t);
                            if (x> 0f && x < 16000f) {
                                target = t;
                                break;
                            }
                        }
                    }                  
                }
            }
            return target;
        }

        internal class MCHPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MCHPvP_BurstMode;

            protected override uint Invoke(uint actionID)
            {
                if (actionID == BlastCharge)
                {
                    var canWeave = CanWeave();
                    var analysisStacks = GetRemainingCharges(Analysis);
                    var bigDamageStacks = GetRemainingCharges(OriginalHook(Drill));
                    var overheated = HasEffect(Buffs.Overheated);
                    var FMFOption = PluginConfiguration.GetCustomIntValue(Config.MCHPVP_FMFOption);

                    if (!PvPCommon.TargetImmuneToDamage() && HasBattleTarget())
                    {
                        //´¦ÀíÕ¶Ìú½£
                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_MarksmanSpite)) {
                            if (IsLB1Ready) {
                                var target = Ñ°ÕÒÄ§µ¯ÉäÊÖÄ¿±ê();
                                if (target != null) {
                                    CustomComboFunctions.TargetObject(target);
                                    float x = CustomComboFunctions.EnemyHealthCurrentHp(target);
                                    //ÔÙ´ÎÈ·ÈÏ
                                    if (HasTarget() && x < 16000f)
                                        return OriginalHook(Ä§µ¯ÉäÊÖ);
                                }
                            }
                        }



                        // MarksmanSpite execute condition - todo add config
                        //if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_MarksmanSpite) && HasBattleTarget() && EnemyHealthCurrentHp() < GetOptionValue(Config.MCHPVP_MarksmanSpite) && IsLB1Ready)
                        //    return Ä§µ¯ÉäÊÖ;

                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_Wildfire) && canWeave && overheated && IsOffCooldown(Wildfire))
                            return OriginalHook(Wildfire);

                        // FullMetalField condition when not overheated or if overheated and FullMetalField is off cooldown
                        if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_FullMetalField) && IsOffCooldown(FullMetalField))
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

                            if (IsEnabled(CustomComboPreset.MCHPvP_BurstMode_BioBlaster) && HasEffect(Buffs.BioblasterPrimed) && HasBattleTarget() && GetTargetDistance() <= 12)
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
