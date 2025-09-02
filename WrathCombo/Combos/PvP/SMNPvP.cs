using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.SMNPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class SMNPvP
{
    #region IDs

    internal class Role : PvPCaster;

    internal const uint
        Ruin3 = 29664,
        AstralImpulse = 29665,
        FountainOfFire = 29666,
        CrimsonCyclone = 29667,
        CrimsonStrike = 29668,
        Slipstream = 29669,
        RadiantAegis = 29670,
        MountainBuster = 29671,
        Necrotize = 41483,
        DeathFlare = 41484,
        Megaflare = 29675,          // unused
        Wyrmwave = 29676,           // unused
        AkhMorn = 29677,            // unused
        EnkindlePhoenix = 29679,
        ScarletFlame = 29681,       // unused
        Revelation = 29682,         // unused
        Ruin4 = 41482,
        BrandofPurgatory = 41485;

    internal class Buffs
    {
        internal const ushort
            FurtherRuin = 4399;

    }
    #endregion

    #region Config
    public static class Config
    {
        public static UserInt
            SMNPvP_RadiantAegisThreshold = new("SMNPvP_RadiantAegisThreshold"),
            SMNPvP_PhantomDartThreshold = new("SMNPvP_PhantomDartThreshold", 50);

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                // Phantom Dart
                case Preset.SMNPvP_PhantomDart:
                    DrawSliderInt(1, 100, SMNPvP_PhantomDartThreshold, "Target HP% to use Phantom Dart at or below");
                    break;

                case Preset.SMNPvP_BurstMode_RadiantAegis:
                    DrawSliderInt(0, 90, SMNPvP_RadiantAegisThreshold, "Caps at 90 to prevent waste.");
                    break;
            }
        }
    }
    #endregion

    internal class SMNPvP_BurstMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMNPvP_BurstMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Ruin3) return actionID;

            #region Variables
            bool canWeave = CanWeave();
            bool bahamutBurst = OriginalHook(Ruin3) is AstralImpulse;
            bool phoenixBurst = OriginalHook(Ruin3) is FountainOfFire;
            double playerHP = PlayerHealthPercentageHp();
            int radiantThreshold = PluginConfiguration.GetCustomIntValue(SMNPvP_RadiantAegisThreshold);
            #endregion

            if (PvPCommon.TargetImmuneToDamage() && HasStatusEffect(Buffs.FurtherRuin)) // Block for ruin 4 because it is on action ID
                return All.SavageBlade;
                    
            if (!PvPCommon.TargetImmuneToDamage())
            {
                if (canWeave)
                {
                    // Radiant Aegis
                    if (IsEnabled(Preset.SMNPvP_BurstMode_RadiantAegis) &&
                        IsOffCooldown(RadiantAegis) && playerHP <= radiantThreshold)
                        return RadiantAegis;

                    if (IsEnabled(Preset.SMNPvP_PhantomDart) && PvPCaster.CanPhantomDart() && GetTargetHPPercent() <= SMNPvP_PhantomDartThreshold)
                        return PvPCaster.PhantomDart;
                }
                // Phoenix & Bahamut bursts
                if (IsEnabled(Preset.SMNPvP_BurstMode_BrandofPurgatory) && phoenixBurst && IsOffCooldown(BrandofPurgatory))
                    return BrandofPurgatory;

                if (IsEnabled(Preset.SMNPvP_BurstMode_DeathFlare) && bahamutBurst && IsOffCooldown(DeathFlare))
                    return DeathFlare;

                if (HasStatusEffect(Buffs.FurtherRuin))
                    return actionID;

                if (IsEnabled(Preset.SMNPvP_BurstMode_Necrotize) && GetRemainingCharges(Necrotize) > 0 && !HasStatusEffect(Buffs.FurtherRuin))
                    return Necrotize;
                        
                // Ifrit (check CrimsonCyclone conditions)
                if (IsEnabled(Preset.SMNPvP_BurstMode_CrimsonStrike) && OriginalHook(CrimsonCyclone) is CrimsonStrike)
                    return CrimsonStrike;

                if (IsEnabled(Preset.SMNPvP_BurstMode_CrimsonCyclone) && IsOffCooldown(CrimsonCyclone))
                    return CrimsonCyclone;

                // Titan
                if (IsEnabled(Preset.SMNPvP_BurstMode_MountainBuster) && IsOffCooldown(MountainBuster) && InActionRange(MountainBuster))
                    return MountainBuster;

                // Garuda (check Slipstream cooldown)
                if (IsEnabled(Preset.SMNPvP_BurstMode_Slipstream) && IsOffCooldown(Slipstream) && !IsMoving())
                    return Slipstream;
            }
            return actionID;
        }
    }
}