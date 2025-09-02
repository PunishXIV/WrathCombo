using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.PLDPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class PLDPvP
{
    #region IDS
    internal class Role : PvPTank;
    public const uint
        FastBlade = 29058,
        RiotBlade = 29059,
        RoyalAuthority = 29060,
        ShieldSmite = 41430,
        HolySpirit = 29062,
        Imperator = 41431,
        Intervene = 29065,
        HolySheltron = 29067,
        Guardian = 29066,
        Phalanx = 29069,
        BladeOfFaith = 29071,
        BladeOfTruth = 29072,
        BladeOfValor = 29073;

    internal class Buffs
    {
        internal const ushort
            Covered = 1301,
            ConfiteorReady = 3028,
            HallowedGround = 1302,
            AttonementReady = 2015,
            SupplicationReady = 4281,
            SepulchreReady = 4282,
            BladeOfFaithReady = 3250;
    }

    internal class Debuffs
    {
        internal const ushort
            Stun = 1343,
            ShieldSmite = 4283;
    }
    #endregion

    #region Config
    public static class Config
    {
        public static UserInt
            PLDPvP_RampartThreshold = new("PLDPvP_RampartThreshold"),
            PLDPvP_HolySpirit_Threshold = new("PLDPvP_HolySpirit_Threshold");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.PLDPvP_Rampart:
                    DrawSliderInt(1, 100, PLDPvP_RampartThreshold, "Use Rampart below set threshold for self");
                    break;
                
                case Preset.PLDPvP_HolySpirit:
                    DrawSliderInt(0, 6000, PLDPvP_HolySpirit_Threshold, "Must be missing this much health to use Holy Spirit", sliderIncrement:1000);
                    break;
            }
        }
    }
    #endregion

    internal class PLDPvP_Burst : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLDPvP_Burst;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FastBlade or RiotBlade or RoyalAuthority)) 
                return actionID;
            
            if (IsEnabled(Preset.PLDPvP_Rampart) && PvPTank.CanRampart(PLDPvP_RampartThreshold))
                return PvPTank.Rampart;

            if (IsEnabled(Preset.PLDPvP_Intervene) && !InMeleeRange() && IsOffCooldown(Intervene) || IsEnabled(Preset.PLDPvP_Intervene_Melee) && InMeleeRange() && IsOffCooldown(Intervene))
                return Intervene;

            // Check conditions for Holy Sheltron
            if (IsEnabled(Preset.PLDPvP_Sheltron) && IsOffCooldown(HolySheltron) && InCombat() && InMeleeRange())
                return HolySheltron;

            // Check conditions for ShieldSmite
            if (IsEnabled(Preset.PLDPvP_ShieldSmite) && IsOffCooldown(ShieldSmite) && InCombat() && InMeleeRange())
                return ShieldSmite;

            // Prioritize Imperator
            if (IsEnabled(Preset.PLDPvP_Imperator) && IsOffCooldown(Imperator) && InMeleeRange() && CanWeave())
                return Imperator;

            if (IsEnabled(Preset.PLDPvP_PhalanxCombo))
            {
                if (HasStatusEffect(Buffs.BladeOfFaithReady) || WasLastSpell(BladeOfTruth) || WasLastSpell(BladeOfFaith))
                    return OriginalHook(Phalanx);
            }

            // Check if the custom combo preset is enabled and ConfiteorReady is active
            if (IsEnabled(Preset.PLDPvP_Confiteor) && HasStatusEffect(Buffs.ConfiteorReady))
                return OriginalHook(Imperator);

            var missinghealth = LocalPlayer.MaxHp - LocalPlayer.CurrentHp;

            if (IsEnabled(Preset.PLDPvP_HolySpirit) && ActionReady(HolySpirit) && missinghealth >= PLDPvP_HolySpirit_Threshold)
            {
                if (!InMeleeRange() || !HasStatusEffect(Buffs.AttonementReady) && !HasStatusEffect(Buffs.SupplicationReady) && !HasStatusEffect(Buffs.SepulchreReady))
                    return HolySpirit;
            }
            return actionID;
        }
    }
}