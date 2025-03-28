using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class DNCPvP
    {
        public const byte JobID = 38;

        internal class Role : PvPPhysRanged;

        internal const uint
            FountainCombo = 54,
            Cascade = 29416,
            Fountain = 29417,
            ReverseCascade = 29418,
            Fountainfall = 29419,
            SaberDance = 29420,
            StarfallDance = 29421,
            HoningDance = 29422,
            HoningOvation = 29470,
            FanDance = 29428,
            CuringWaltz = 29429,
            EnAvant = 29430,
            ClosedPosition = 29431,
            Contradance = 29432;

        internal class Buffs
        {
            internal const ushort
                EnAvant = 2048,
                FanDance = 2052,
                Bladecatcher = 3159,
                FlourishingSaberDance = 3160,
                StarfallDance = 3161,
                HoningDance = 3162,
                Acclaim = 3163,
                HoningOvation = 3164,
                ClosedPosition = 2026;
        }
        public static class Config
        {
            public const string
                DNCPvP_WaltzThreshold = "DNCWaltzThreshold",
                DNCPvP_EnAvantCharges = "DNCPvP_EnAvantCharges";
        }

        internal class DNCPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNCPvP_BurstMode;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is Cascade or Fountain or ReverseCascade or Fountainfall)
                {
                    bool starfallDanceReady = !GetCooldown(StarfallDance).IsCooldown;
                    bool starfallDance = HasEffect(Buffs.StarfallDance);
                    bool curingWaltzReady = !GetCooldown(CuringWaltz).IsCooldown;
                    bool honingDanceReady = !GetCooldown(HoningDance).IsCooldown;
                    var acclaimStacks = GetBuffStacks(Buffs.Acclaim);
                    bool canWeave = CanWeave();
                    var distance = GetTargetDistance();
                    var HPThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCPvP_WaltzThreshold);
                    var HP = PlayerHealthPercentageHp();
                    bool enemyGuarded = TargetHasEffectAny(PvPCommon.Buffs.Guard);

                    // Honing Dance Option

                    if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_Partner) && ActionReady(ClosedPosition) && !HasEffect(Buffs.ClosedPosition) & GetPartyMembers().Count > 1)
                        return ClosedPosition;

                    if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_HoningDance) && honingDanceReady && HasTarget() && distance <= 5 && !enemyGuarded)
                    {
                        if (HasEffect(Buffs.Acclaim) && acclaimStacks < 4)
                            return WHM.Assize;

                        return HoningDance;
                    }

                    if (canWeave)
                    {
                        // Curing Waltz Option
                        if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_CuringWaltz) && curingWaltzReady && HP <= HPThreshold)
                            return OriginalHook(CuringWaltz);

                        // Fan Dance weave
                        if (IsOffCooldown(FanDance) && distance < 13 && !enemyGuarded) // 2y below max to avoid waste
                            return OriginalHook(FanDance);

                        if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_Dash) && !HasEffect(Buffs.EnAvant) && GetRemainingCharges(EnAvant) > GetOptionValue(Config.DNCPvP_EnAvantCharges))
                            return EnAvant;
                    }

                    // Starfall Dance
                    if (!starfallDance && starfallDanceReady && distance < 20 && !enemyGuarded) // 5y below max to avoid waste
                        return OriginalHook(StarfallDance);
                }

                return actionID;
            }
        }
    }
}
