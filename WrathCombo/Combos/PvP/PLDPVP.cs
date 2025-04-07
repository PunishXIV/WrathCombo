using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class PLDPvP
    {
        #region IDS

        public const byte JobID = 19;

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
                PLDPvP_RampartThreshold = new("PLDPvP_RampartThreshold");

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.PLDPvP_Rampart:
                        UserConfig.DrawSliderInt(1, 100, PLDPvP_RampartThreshold,
                            "Use Rampart below set threshold for self");
                        break;

                }
            }
        }
        #endregion

        internal class PLDPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLDPvP_Burst;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is FastBlade or RiotBlade or RoyalAuthority)
                {
                    if (IsEnabled(CustomComboPreset.PLDPvP_Rampart) && PvPTank.CanRampart(Config.PLDPvP_RampartThreshold))
                        return PvPTank.Rampart;

                    if (IsEnabled(CustomComboPreset.PLDPvP_Intervene) && !InMeleeRange() && IsOffCooldown(Intervene) || IsEnabled(CustomComboPreset.PLDPvP_Intervene_Melee) && InMeleeRange() && IsOffCooldown(Intervene))
                        return Intervene;

                    // Check conditions for Holy Sheltron
                    if (IsEnabled(CustomComboPreset.PLDPvP_Sheltron) && IsOffCooldown(HolySheltron) && InCombat() && InMeleeRange())
                        return HolySheltron;

                    // Check conditions for ShieldSmite
                    if (IsEnabled(CustomComboPreset.PLDPvP_ShieldSmite) && IsOffCooldown(ShieldSmite) && InCombat() && InMeleeRange())
                        return ShieldSmite;

                    // Prioritize Imperator
                    if (IsEnabled(CustomComboPreset.PLDPvP_Imperator) && IsOffCooldown(Imperator) && InMeleeRange() && CanWeave())
                        return Imperator;

                    if (IsEnabled(CustomComboPreset.PLDPvP_PhalanxCombo))
                    {
                        if (HasStatusEffect(Buffs.BladeOfFaithReady) || WasLastSpell(BladeOfTruth) || WasLastSpell(BladeOfFaith))
                            return OriginalHook(Phalanx);
                    }

                    // Check if the custom combo preset is enabled and ConfiteorReady is active
                    if (IsEnabled(CustomComboPreset.PLDPvP_Confiteor) && HasStatusEffect(Buffs.ConfiteorReady))
                        return OriginalHook(Imperator);


                    if (IsEnabled(CustomComboPreset.PLDPvP_HolySpirit))
                    {
                        if (IsOffCooldown(HolySpirit) && !InMeleeRange() || IsOffCooldown(HolySpirit) && (!HasStatusEffect(Buffs.AttonementReady) && !HasStatusEffect(Buffs.SupplicationReady) && !HasStatusEffect(Buffs.SepulchreReady)))
                            return HolySpirit;
                    }

                }

                return actionID;
            }
        }
    }
}
