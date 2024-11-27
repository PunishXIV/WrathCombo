using WrathCombo.Core;
using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class SMNPvP
    {
        public const byte ClassID = 26;
        public const byte JobID = 27;

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

        public static class Config
        {
            public const string
                SMNPvP_RadiantAegisThreshold = "SMNPvP_RadiantAegisThreshold";
            public const string
                SMNPvP_FesterThreshold = "SMNPvP_FesterThreshold";
        }

        internal class SMNPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SMNPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Ruin3)
                {
                    #region Types
                    bool canWeave = CanWeave(actionID);
                    bool bahamutBurst = OriginalHook(Ruin3) is AstralImpulse;
                    bool phoenixBurst = OriginalHook(Ruin3) is FountainOfFire;
                    double playerHP = PlayerHealthPercentageHp();
                    bool canBind = !TargetHasEffectAny(PvPCommon.Debuffs.Bind);
                    int radiantThreshold = PluginConfiguration.GetCustomIntValue(Config.SMNPvP_RadiantAegisThreshold);
                    #endregion

                    if (!PvPCommon.IsImmuneToDamage())
                    {
                        if (canWeave)
                        {
                            // Radiant Aegis
                            if (IsEnabled(CustomComboPreset.SMNPvP_BurstMode_RadiantAegis) &&
                                IsOffCooldown(RadiantAegis) && playerHP <= radiantThreshold)
                                return RadiantAegis;
                        }
                        // Phoenix & Bahamut bursts
                        if (IsEnabled(CustomComboPreset.SMNPvP_BurstMode_BrandofPurgatory) && phoenixBurst && IsOffCooldown(BrandofPurgatory))
                            return BrandofPurgatory;

                        if (IsEnabled(CustomComboPreset.SMNPvP_BurstMode_DeathFlare) && bahamutBurst && IsOffCooldown(DeathFlare))
                            return DeathFlare;

                        if (IsEnabled(CustomComboPreset.SMNPvP_BurstMode_Necrotize) && GetRemainingCharges(Necrotize) > 0 && !HasEffect(Buffs.FurtherRuin))
                            return Necrotize;
                        
                        // Ifrit (check CrimsonCyclone conditions)
                        if (IsEnabled(CustomComboPreset.SMNPvP_BurstMode_CrimsonStrike) && OriginalHook(CrimsonCyclone) is CrimsonStrike)
                            return CrimsonStrike;

                        if (IsEnabled(CustomComboPreset.SMNPvP_BurstMode_CrimsonCyclone) && IsOffCooldown(CrimsonCyclone))
                            return CrimsonCyclone;

                        // Titan
                        if (IsEnabled(CustomComboPreset.SMNPvP_BurstMode_MountainBuster) && IsOffCooldown(MountainBuster) && InActionRange(MountainBuster))
                            return MountainBuster;

                        // Garuda (check Slipstream cooldown)
                        if (IsEnabled(CustomComboPreset.SMNPvP_BurstMode_Slipstream) && IsOffCooldown(Slipstream) && !IsMoving)
                            return Slipstream;
                    }
                }

                return actionID;
            }
        }
    }
}
