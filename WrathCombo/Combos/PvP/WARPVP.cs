using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.WARPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class WARPvP
{
    #region IDS
    internal class Role : PvPTank;
    internal const uint
        HeavySwing = 29074,
        Maim = 29075,
        StormsPath = 29076,
        PrimalRend = 29084,
        Onslaught = 29079,
        Orogeny = 29080,
        Blota = 29081,
        Bloodwhetting = 29082,
        PrimalScream = 29083,
        PrimalWrath = 41433;


    internal class Buffs
    {
        internal const ushort
            InnerRelease = 1303,
            NascentChaos = 1992,
            InnerChaosReady = 4284,
            PrimalRuinationReady = 4285,
            Wrathfull = 4286;
    }
    #endregion

    #region Config
    public static class Config
    {
        public static UserInt
            WARPVP_BlotaTiming = new("WARPVP_BlotaTiming"),
            WARPvP_RampartThreshold = new("WARPvP_RampartThreshold");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.WARPvP_Rampart:
                    DrawSliderInt(1, 100, WARPvP_RampartThreshold,
                        "Use Rampart below set threshold for self");
                    break;

                case Preset.WARPvP_BurstMode_Blota:
                    DrawHorizontalRadioButton(WARPVP_BlotaTiming, $"Before {PrimalRend.ActionName()}", "", 0);
                    DrawHorizontalRadioButton(WARPVP_BlotaTiming, $"After {PrimalRend.ActionName()}", "", 1);
                    break;
            }
        }
    }
    #endregion
       
    internal class WARPvP_BurstMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.WARPvP_BurstMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavySwing or Maim or StormsPath)) 
                return actionID;
            
            if (IsEnabled(Preset.WARPvP_Rampart) && PvPTank.CanRampart(WARPvP_RampartThreshold))
                return PvPTank.Rampart;

            if (!PvPCommon.TargetImmuneToDamage())
            {
                var canWeave = CanWeave();

                // Bloodwhetting condition (both WARPvP BurstMode and CanWeave)
                if (IsEnabled(Preset.WARPvP_BurstMode_Bloodwhetting))
                {
                    if (!GetCooldown(Bloodwhetting).IsCooldown || canWeave && IsOffCooldown(Bloodwhetting))
                        return OriginalHook(Bloodwhetting);
                }

                // Primal Wrath if in melee range and Wrathfull effect is active
                if (IsEnabled(Preset.WARPvP_BurstMode_PrimalScream) && InMeleeRange() && canWeave && HasStatusEffect(Buffs.Wrathfull))
                    return OriginalHook(PrimalScream);

                // Blota and PrimalRend conditions based on range and cooldowns
                if (!InMeleeRange())
                {
                    // Blota with specific conditions and burst mode enabled
                    if (IsOffCooldown(Blota) && !HasStatusEffect(PvPCommon.Debuffs.Stun, CurrentTarget, true) && IsEnabled(Preset.WARPvP_BurstMode_Blota))
                    {
                        if (WARPVP_BlotaTiming == 0 && IsOffCooldown(PrimalRend))
                            return OriginalHook(Blota);
                        if (WARPVP_BlotaTiming == 1 && IsOnCooldown(PrimalRend))
                            return OriginalHook(Blota);
                    }

                    // PrimalRend if ready or BurstMode enabled
                    if (IsEnabled(Preset.WARPvP_BurstMode_PrimalRend))
                    {
                        if ((IsOffCooldown(PrimalRend) || HasStatusEffect(Buffs.PrimalRuinationReady)))
                            return OriginalHook(PrimalRend);
                    }

                }

                // In melee range logic
                if (InMeleeRange())
                {
                    // Inner Chaos effect logic
                    if (IsEnabled(Preset.WARPvP_BurstMode_InnerChaos) && HasStatusEffect(Buffs.InnerChaosReady))
                        return OriginalHook(Blota);

                    // Onslaught and Orogeny conditions for melee
                    if (IsEnabled(Preset.WARPvP_BurstMode_Onslaught) && !GetCooldown(Onslaught).IsCooldown && canWeave)
                        return OriginalHook(Onslaught);

                    // Nascent Chaos and Orogeny conditions
                    if (IsEnabled(Preset.WARPvP_BurstMode_Bloodwhetting) && HasStatusEffect(Buffs.NascentChaos))
                        return OriginalHook(Bloodwhetting);

                    if (IsEnabled(Preset.WARPvP_BurstMode_Orogeny) && !GetCooldown(Orogeny).IsCooldown && canWeave)
                        return OriginalHook(Orogeny);

                    // PrimalRend if ready or BurstMode enabled
                    if (IsEnabled(Preset.WARPvP_BurstMode_PrimalRend))
                    {
                        if (IsOffCooldown(PrimalRend) || HasStatusEffect(Buffs.PrimalRuinationReady))
                            return OriginalHook(PrimalRend);
                    }

                    // Blota with specific conditions and burst mode enabled in meleerange
                    if (IsOffCooldown(Blota) && !HasStatusEffect(PvPCommon.Debuffs.Stun, CurrentTarget, true) && IsEnabled(Preset.WARPvP_BurstMode_Blota))
                    {
                        if (WARPVP_BlotaTiming == 0 && IsOffCooldown(PrimalRend) || WARPVP_BlotaTiming == 1 && IsOnCooldown(PrimalRend))
                            return OriginalHook(Blota);
                    }
                }
            }
            return actionID;
        }
    }
}