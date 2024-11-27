using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class BLMPvP
    {
        public const uint
            Fire = 29649,
            Blizzard = 29653,
            Burst = 29657,
            Paradox = 29663,
            NightWing = 29659,
            AetherialManipulation = 29660,
            Superflare = 29661,
            Fire4 = 29650,
            Flare = 29651,
            Blizzard4 = 29654,
            Freeze = 29655,
            Lethargy = 41510,
            ElementalWeave = 41475,
            SoulResonance = 29662,
            Xenoglossy = 29658;

        public static class Buffs
        {
            public const ushort
                AstralFire1 = 3212,
                AstralFire2 = 3213,
                AstralFire3 = 3381,
                UmbralIce1 = 3214,
                UmbralIce2 = 3215,
                UmbralIce3 = 3382,
                Burst = 3221,
                SoulResonance = 3222,
                Polyglot = 3169,
                ElementalStar = 4317,
                Paradox = 3223;
        }

        public static class Debuffs
        {
            public const ushort
                AstralWarmth = 3216,
                UmbralFreeze = 3217,
                Burns = 3218,
                DeepFreeze = 3219;
        }

        public static class Config
        {
            public const string
                BLMPvP_BurstMode_WreathOfIce = "BLMPvP_BurstMode_WreathOfIce",
                BLMPvP_BurstMode_WreathOfFireExecute = "BLMPvP_BurstMode_WreathOfFireExecute";

        }

        internal class BLMPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLMPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool canWeave = CanSpellWeave(actionID);

                if (actionID is Fire or Fire4 or Flare)
                {

                    if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_WreathOfFire))
                    {
                        if (IsOffCooldown(ElementalWeave) && (TargetHasEffectAny(PvPCommon.Buffs.Guard) || GetTargetHPPercent() < GetOptionValue(Config.BLMPvP_BurstMode_WreathOfFireExecute) && IsEnabled(CustomComboPreset.BLMPvP_BurstMode_WreathOfFireExecute))
                            && canWeave && (HasEffect(Buffs.AstralFire1) || HasEffect(Buffs.AstralFire2) || HasEffect(Buffs.AstralFire3)))
                            return OriginalHook(ElementalWeave);
                    }

                    if (!PvPCommon.IsImmuneToDamage())
                    {
                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_FlareStar) && HasEffect(Buffs.ElementalStar))
                            return OriginalHook(SoulResonance);

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_Xenoglossy) && HasCharges(Xenoglossy))
                            return Xenoglossy;

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_AetherialManip) &&
                            ActionReady(AetherialManipulation) &&
                            !InMeleeRange() && IsOffCooldown(Burst) && canWeave)
                            return AetherialManipulation;

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_Burst) && InMeleeRange() &&
                            IsOffCooldown(Burst))
                            return Burst;

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_Lethargy) && IsOffCooldown(Lethargy) && canWeave)
                            return Lethargy;

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_Paradox) && HasEffect(Buffs.Paradox))
                            return Paradox;
                    }

                }

                if (actionID is Blizzard or Blizzard4 or Freeze)
                {

                    if (!PvPCommon.IsImmuneToDamage())
                    {
                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_FrostStar) && HasEffect(Buffs.ElementalStar))
                            return OriginalHook(SoulResonance);

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_Xenoglossy) && HasCharges(Xenoglossy))
                            return Xenoglossy;

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_AetherialManip) &&
                            ActionReady(AetherialManipulation) &&
                            !InMeleeRange() &&
                            IsOffCooldown(Burst) &&
                            canWeave)
                            return AetherialManipulation;

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_Burst) && InMeleeRange() &&
                            IsOffCooldown(Burst))
                            return Burst;

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_Lethargy) && IsOffCooldown(Lethargy) && canWeave)
                            return Lethargy;

                        if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_Paradox) && HasEffect(Buffs.Paradox))
                            return Paradox;
                    }

                    if (IsEnabled(CustomComboPreset.BLMPvP_BurstMode_WreathOfIce) && IsOffCooldown(ElementalWeave) && canWeave && PlayerHealthPercentageHp() <= GetOptionValue(Config.BLMPvP_BurstMode_WreathOfIce)
                        && (HasEffect(Buffs.UmbralIce1) || HasEffect(Buffs.UmbralIce2) || HasEffect(Buffs.UmbralIce3)))
                        return OriginalHook(ElementalWeave);
                }

                return actionID;
            }
        }
    }
}
