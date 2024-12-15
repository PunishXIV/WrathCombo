using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class SGEPvP
    {
        internal const uint
            Dosis = 29256,
            Phlegma = 29259,
            Pneuma = 29260,
            Eukrasia = 29258,
            Icarus = 29261,
            Toxikon = 29262,
            Kardia = 29264,
            EukrasianDosis = 29257,
            Toxicon2 = 29263,
            Psyche = 41658;

        internal class Debuffs
        {
            internal const ushort
                EukrasianDosis = 3108,
                Toxicon = 3113;
        }

        internal class Buffs
        {
            internal const ushort
                Kardia = 2871,
                Kardion = 2872,
                Eukrasia = 3107,
                Addersting = 3115,
                Haima = 3110,
                Haimatinon = 3111;
        }

        internal class SGEPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGEPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == Dosis)
                {
                    if (IsEnabled(CustomComboPreset.SGEPvP_BurstMode_KardiaReminder) && !HasEffectAny(Buffs.Kardia))
                        return Kardia;

                    if (!PvPCommon.TargetImmuneToDamage())
                    {
                        // Psyche after Phlegma
                        if (IsEnabled(CustomComboPreset.SGEPvP_BurstMode_Psyche) && WasLastSpell(Phlegma))
                            return Psyche;

                        if (IsEnabled(CustomComboPreset.SGEPvP_BurstMode_Pneuma) && !GetCooldown(Pneuma).IsCooldown)
                            return Pneuma;

                        if (IsEnabled(CustomComboPreset.SGEPvP_BurstMode_Phlegma) && InMeleeRange() && !HasEffect(Buffs.Eukrasia) && GetCooldown(Phlegma).RemainingCharges > 0)
                            return Phlegma;

                        if (IsEnabled(CustomComboPreset.SGEPvP_BurstMode_Toxikon2) && HasEffect(Buffs.Addersting) && !HasEffect(Buffs.Eukrasia))
                            return Toxicon2;

                        if (IsEnabled(CustomComboPreset.SGEPvP_BurstMode_Eukrasia) && !TargetHasEffectAny(Debuffs.EukrasianDosis) && GetCooldown(Eukrasia).RemainingCharges > 0 && !HasEffect(Buffs.Eukrasia))
                            return Eukrasia;

                        if (HasEffect(Buffs.Eukrasia))
                            return OriginalHook(Dosis);

                        if (IsEnabled(CustomComboPreset.SGEPvP_BurstMode_Toxikon) && !TargetHasEffect(Debuffs.Toxicon) && GetCooldown(Toxikon).RemainingCharges > 0)
                            return OriginalHook(Toxikon);
                    }

                }
                return actionID;
            }
        }
    }
}
