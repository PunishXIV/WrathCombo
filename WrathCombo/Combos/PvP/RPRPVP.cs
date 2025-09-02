using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.RPRPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class RPRPvP
{
    #region IDS
    internal class Role : PvPMelee;

    internal const uint
        Slice = 29538,
        WaxingSlice = 29539,
        InfernalSlice = 29540,
        HarvestMoon = 29545,
        PlentifulHarvest = 29546,
        GrimSwathe = 29547,
        LemuresSlice = 29548,
        DeathWarrant = 29549,
        ArcaneCrest = 29552,
        HellsIngress = 29550,
        Regress = 29551,
        Communio = 29554,
        TenebraeLemurum = 29553;

    internal class Buffs
    {
        internal const ushort
            Soulsow = 2750,
            SoulReaver = 2854,
            GallowsOiled = 2856,
            Enshrouded = 2863,
            ImmortalSacrifice = 3204,
            PlentifulHarvest = 3205,
            DeathWarrant = 4308,
            PerfectioParata = 4309;
    }

    internal class Debuffs
    {
        internal const ushort
            DeathWarrant = 3206;
    }
    #endregion

    #region Config
    public static class Config
    {
        public static UserInt
                
            RPRPvP_ImmortalStackThreshold = new("RPRPvPImmortalStackThreshold"),            
            RPRPvP_ArcaneCircleThreshold = new("RPRPvPArcaneCircleOption"),            
            RPRPvP_SmiteThreshold = new("RPRPvP_SmiteThreshold");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.RPRPvP_Burst_ImmortalPooling:
                    DrawSliderInt(0, 8, RPRPvP_ImmortalStackThreshold, "Set a value of Immortal Sacrifice Stacks to hold for burst.");
                    break;

                case Preset.RPRPvP_Burst_ArcaneCircle:
                    DrawSliderInt(5, 90, RPRPvP_ArcaneCircleThreshold, "Set a HP percentage value. Caps at 90 to prevent waste.");
                    break;

                case Preset.RPRPvP_Smite:
                    DrawSliderInt(0, 100, RPRPvP_SmiteThreshold, "Target HP% to smite, Max damage below 25%");
                    break;
            }
        }
    }
    #endregion
    
    internal class RPRPvP_Burst : CustomCombo
    {
        protected internal override Preset Preset => Preset.RPRPvP_Burst;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Slice or WaxingSlice or InfernalSlice)) 
                return actionID;

            #region Variables
            double distance = GetTargetDistance();
            bool canWeave = CanWeave();                    
            bool canBind = !HasStatusEffect(PvPCommon.Debuffs.Bind, CurrentTarget);
            bool deathWarrantReady = IsOffCooldown(DeathWarrant);
            bool plentifulReady = IsOffCooldown(PlentifulHarvest);
            bool enshrouded = HasStatusEffect(Buffs.Enshrouded);
            float enshroudStacks = GetStatusEffectStacks(Buffs.Enshrouded);
            float immortalStacks = GetStatusEffectStacks(Buffs.ImmortalSacrifice);
            int immortalThreshold = RPRPvP_ImmortalStackThreshold;
            #endregion

            // Arcane Cirle Option
            if (IsEnabled(Preset.RPRPvP_Burst_ArcaneCircle)
                && ActionReady(ArcaneCrest) && PlayerHealthPercentageHp() <= RPRPvP_ArcaneCircleThreshold)
                return ArcaneCrest;

            if (!PvPCommon.TargetImmuneToDamage()) // Guard check on target
            {
                //Smite
                if (IsEnabled(Preset.RPRPvP_Smite) && PvPMelee.CanSmite() && GetTargetDistance() <= 10 && HasTarget() &&
                    GetTargetHPPercent() <= RPRPvP_SmiteThreshold)
                    return PvPMelee.Smite;

                // Harvest Moon Ranged Option
                if (IsEnabled(Preset.RPRPvP_Burst_RangedHarvest) && distance > 5)
                    return HarvestMoon;

                // Enshroud
                if (IsEnabled(Preset.RPRPvP_Burst_Enshrouded) && enshrouded)
                {
                    if (canWeave)
                    {
                        // Enshrouded Death Warrant Option
                        if (IsEnabled(Preset.RPRPvP_Burst_Enshrouded_DeathWarrant) &&
                            deathWarrantReady && enshroudStacks >= 3 && distance <= 25 || HasStatusEffect(Buffs.DeathWarrant) && GetStatusEffectRemainingTime(Buffs.DeathWarrant) <= 3)
                            return OriginalHook(DeathWarrant);

                        // Lemure's Slice
                        if (ActionReady(LemuresSlice) && canBind && distance <= 8)
                            return LemuresSlice;
                    }

                    // Communio Option
                    if (IsEnabled(Preset.RPRPvP_Burst_Enshrouded_Communio) &&
                        enshroudStacks == 1 && distance <= 25)
                    {
                        // Holds Communio when moving & Enshrouded Time Remaining > 2s
                        // Returns a Void/Cross Reaping if under 2s to avoid charge waste
                        if (IsMoving() && GetStatusEffectRemainingTime(Buffs.Enshrouded) > 2)
                            return BLM.Xenoglossy;

                        // Returns Communio if stationary
                        if (!IsMoving())
                            return Communio;
                    }
                }

                // Outside of Enshroud
                if (!enshrouded)
                {
                    if (HasStatusEffect(Buffs.PerfectioParata))
                        return OriginalHook(TenebraeLemurum);

                    // Pooling Plentiful with Death warrant
                    if (IsEnabled(Preset.RPRPvP_Burst_ImmortalPooling))
                    {
                        if (IsEnabled(Preset.RPRPvP_Burst_DeathWarrant) && deathWarrantReady && distance <= 25 &&
                            (GetCooldownRemainingTime(PlentifulHarvest) > 20 ||     //if plentiful will be back for the next death warrant
                             (plentifulReady && immortalStacks >= immortalThreshold) || // if plentiful is ready for this death warrant and you have the charges you want
                             (plentifulReady && immortalStacks <= immortalThreshold - 2))) // if plentiful is ready, but 2 grim swathes away from having the immortal threshold. Early fight. 
                            return OriginalHook(DeathWarrant);

                        if (plentifulReady && immortalStacks >= immortalThreshold &&
                            HasStatusEffect(Debuffs.DeathWarrant, CurrentTarget) && distance <= 15)
                            return PlentifulHarvest;
                    }

                    // Weaves
                    if (canWeave)
                    {                               
                        // Death Warrant without pooling
                        if (!IsEnabled(Preset.RPRPvP_Burst_ImmortalPooling) && IsEnabled(Preset.RPRPvP_Burst_DeathWarrant) && deathWarrantReady && distance <= 25)
                            return OriginalHook(DeathWarrant);

                        // Grim Swathe Option
                        if (IsEnabled(Preset.RPRPvP_Burst_GrimSwathe) && ActionReady(GrimSwathe) && distance <= 8)
                            return GrimSwathe;
                    }
                    // Harvest Moon Execute 
                    if (IsEnabled(Preset.RPRPvP_Burst_RangedHarvest) && GetRemainingCharges(HarvestMoon) > 0 &&
                        GetTargetCurrentHP() < 12000)
                        return HarvestMoon;
                }
            }
            return actionID;
        }
    }
}