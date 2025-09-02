using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.SGEPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class SGEPvP
{
    #region IDS
    internal class Role : PvPHealer;
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
            Toxicon = 3113,
            Lype = 3120;
    }

    internal class Buffs
    {
        internal const ushort
            Kardia = 2871,
            Kardion = 2872,
            Eukrasia = 3107,
            Addersting = 3115,
            Haima = 3110,
            Haimatinon = 3111,
            Mesotes = 3119;
    }
    #endregion

    #region Config
    public static class Config
    {
        public static UserBool
            SGEPvP_BurstMode_KardiaReminder_Retarget = new("SGEPvP_BurstMode_KardiaReminder_Retarget");
        public static UserInt
            SGEPvP_KardiaThreshold = new("SGEPvP_KardiaThreshold"),
            SGEPvP_DiabrosisThreshold = new("SGEPvP_DiabrosisThreshold");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.SGEPvP_Diabrosis:
                    DrawSliderInt(0, 100, SGEPvP_DiabrosisThreshold, "Target HP% to use Diabrosis");
                    break;
                
                case Preset.SGEPvP_BurstMode_KardiaReminder:
                    DrawAdditionalBoolChoice(SGEPvP_BurstMode_KardiaReminder_Retarget, "Mobile Kardia Option", "Retarget Kardia according to your heal stack. \nThis will move Kardia around via weaves. Will use Heal stack.(In Wrath Settings)");
                    if (SGEPvP_BurstMode_KardiaReminder_Retarget)
                    {
                        DrawSliderInt(0, 100, SGEPvP_KardiaThreshold, "Minimum HP% to move Kardia. Set to 100% to disable this check. ");
                    }
                    break;
            }
        }
    }
    #endregion       

    internal class SGEPvP_BurstMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGEPvP_BurstMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Dosis) return actionID;
            if (IsEnabled(Preset.SGEPvP_BurstMode_KardiaReminder) && !HasStatusEffect(Buffs.Kardia, anyOwner: true))
                return Kardia;

            if (!PvPCommon.TargetImmuneToDamage())
            {
                if (IsEnabled(Preset.SGEPvP_Diabrosis) && PvPHealer.CanDiabrosis() && HasTarget() &&
                    GetTargetHPPercent() <= SGEPvP_DiabrosisThreshold)
                    return PvPHealer.Diabrosis;

                // Psyche after Phlegma
                if (IsEnabled(Preset.SGEPvP_BurstMode_Psyche) && WasLastSpell(Phlegma))
                    return Psyche;

                if (IsEnabled(Preset.SGEPvP_BurstMode_Pneuma) && !GetCooldown(Pneuma).IsCooldown)
                    return Pneuma;

                if (IsEnabled(Preset.SGEPvP_BurstMode_Phlegma) && InMeleeRange() && !HasStatusEffect(Buffs.Eukrasia) && GetCooldown(Phlegma).RemainingCharges > 0)
                    return Phlegma;

                if (IsEnabled(Preset.SGEPvP_BurstMode_Toxikon2) && HasStatusEffect(Buffs.Addersting) && !HasStatusEffect(Buffs.Eukrasia))
                    return Toxicon2;

                if (IsEnabled(Preset.SGEPvP_BurstMode_Eukrasia) && !HasStatusEffect(Debuffs.EukrasianDosis, CurrentTarget, true) && GetCooldown(Eukrasia).RemainingCharges > 0 && !HasStatusEffect(Buffs.Eukrasia))
                    return Eukrasia;

                if (HasStatusEffect(Buffs.Eukrasia))
                    return OriginalHook(Dosis);

                if (IsEnabled(Preset.SGEPvP_BurstMode_Toxikon) && !HasStatusEffect(Debuffs.Toxicon, CurrentTarget) && GetCooldown(Toxikon).RemainingCharges > 0)
                    return OriginalHook(Toxikon);
            }
            if (IsEnabled(Preset.SGEPvP_BurstMode_KardiaReminder))
            {
                IGameObject? healTarget = SimpleTarget.Stack.AllyToHealPVP;
                
                if (SGEPvP_BurstMode_KardiaReminder_Retarget && CanWeave() && GetTargetHPPercent(healTarget) <= SGEPvP_KardiaThreshold)
                    return Kardia.Retarget(Dosis, healTarget, true);
            }
            return actionID;
        }
    }
    internal class SGEPvP_RetargetKardia : CustomCombo
    {
        protected internal override Preset Preset => Preset.SGEPvP_RetargetKardia;

        protected override uint Invoke(uint actionID)
        {
            return actionID is not Kardia 
                ? actionID 
                : Kardia.Retarget(SimpleTarget.Stack.AllyToHealPVP);
        }
    }
}