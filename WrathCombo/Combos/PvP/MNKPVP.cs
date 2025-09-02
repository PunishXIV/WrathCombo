using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.MNKPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class MNKPvP
{
    #region IDS
    internal class Role : PvPMelee;

    public const uint
        PhantomRushCombo = 55,
        DragonKick = 29475,
        TwinSnakes = 29476,
        Demolish = 29477,
        PhantomRush = 29478,
        RisingPhoenix = 29481,
        RiddleOfEarth = 29482,
        Thunderclap = 29484,
        EarthsReply = 29483,
        Meteordrive = 29485,
        WindsReply = 41509,
        FlintsReply = 41447,
        LeapingOpo = 41444,
        RisingRaptor = 41445,
        PouncingCoeurl = 41446;

    public static class Buffs
    {
        public const ushort
            FiresRumination = 4301,
            FireResonance = 3170,
            EarthResonance = 3171;
    }
    public static class Debuffs
    {
        public const ushort
            PressurePoint = 3172;
    }
    #endregion

    #region Config
    public static class Config
    {
        public static UserInt
            MNKPvP_SmiteThreshold = new("MNKPvP_SmiteThreshold");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.MNKPvP_Smite:
                    DrawSliderInt(0, 100, MNKPvP_SmiteThreshold, "Target HP% to smite, Max damage below 25%");
                    break;
            }
        }
    }
    #endregion
       
    internal class MNKPvP_Burst : CustomCombo
    {
        protected internal override Preset Preset => Preset.MNKPvP_Burst;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (DragonKick or TwinSnakes or Demolish or LeapingOpo or RisingRaptor or PouncingCoeurl or PhantomRush)) 
                return actionID;
            
            if (IsEnabled(Preset.MNKPvP_Burst_Meteodrive) && PvPCommon.TargetImmuneToDamage() && GetTargetCurrentHP() <= 20000 && IsLB1Ready)
                return Meteordrive;

            if (!PvPCommon.TargetImmuneToDamage())
            {
                if (IsEnabled(Preset.MNKPvP_Smite) && PvPMelee.CanSmite() && GetTargetDistance() <= 10 && HasTarget() &&
                    GetTargetHPPercent() <= MNKPvP_SmiteThreshold)
                    return PvPMelee.Smite;

                if (IsEnabled(Preset.MNKPvP_Burst_RisingPhoenix))
                {
                    if (!HasStatusEffect(Buffs.FireResonance) && GetRemainingCharges(RisingPhoenix) > 1 || WasLastWeaponskill(PouncingCoeurl) && GetRemainingCharges(RisingPhoenix) > 0)
                        return OriginalHook(RisingPhoenix);
                    if (HasStatusEffect(Buffs.FireResonance) && WasLastWeaponskill(PouncingCoeurl))
                        return actionID;
                }

                if (IsEnabled(Preset.MNKPvP_Burst_RiddleOfEarth) && IsOffCooldown(RiddleOfEarth) && PlayerHealthPercentageHp() <= 95)
                    return OriginalHook(RiddleOfEarth);

                if (IsEnabled(Preset.MNKPvP_Burst_Thunderclap) && GetRemainingCharges(Thunderclap) > 0 && !InMeleeRange())
                    return OriginalHook(Thunderclap);

                if (IsEnabled(Preset.MNKPvP_Burst_WindsReply) && InActionRange(WindsReply) && IsOffCooldown(WindsReply))
                    return WindsReply;

                if (CanWeave())
                {
                    if (IsEnabled(Preset.MNKPvP_Burst_RiddleOfEarth) && HasStatusEffect(Buffs.EarthResonance) && GetStatusEffectRemainingTime(Buffs.EarthResonance) < 6)
                        return OriginalHook(EarthsReply);
                }

                if (IsEnabled(Preset.MNKPvP_Burst_FlintsReply))
                {
                    if (GetRemainingCharges(FlintsReply) > 0 && (!WasLastAction(LeapingOpo) || !WasLastAction(RisingRaptor) || !WasLastAction(PouncingCoeurl)) || HasStatusEffect(Buffs.FiresRumination) && !WasLastAction(PouncingCoeurl))
                        return OriginalHook(FlintsReply);
                }
            }
            return actionID;
        }
    }
}