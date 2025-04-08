using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class DNCPvP
    {
        #region IDS
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
        #endregion

        #region Config
        public static class Config
        {
            public static UserInt
                DNCPvP_EagleThreshold = new("DNCPvP_EagleThreshold"),
                DNCPvP_WaltzThreshold = new("DNCWaltzThreshold"),
                DNCPvP_EnAvantCharges = new("DNCPvP_EnAvantCharges");
        

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.DNCPvP_BurstMode_CuringWaltz:
                        UserConfig.DrawSliderInt(0, 90,
                            DNCPvP_WaltzThreshold,
                            "Curing Waltz HP% - caps at 90 to prevent waste.");

                        break;

                    case CustomComboPreset.DNCPvP_BurstMode_Dash:
                        UserConfig.DrawSliderInt(0, 3,
                            DNCPvP_EnAvantCharges,
                            "How many to save for manual");

                        break;

                    case CustomComboPreset.DNCPvP_Eagle:
                        UserConfig.DrawSliderInt(0, 100,
                            DNCPvP_EagleThreshold,
                            "Target HP percent threshold to use Eagle Eye Shot Below.");

                        break;

                }
            }
        }
        #endregion

        internal class DNCPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNCPvP_BurstMode;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is Cascade or Fountain or ReverseCascade or Fountainfall)
                {
                    #region Variables

                    bool starfallDanceReady = !GetCooldown(StarfallDance).IsCooldown;
                    bool starfallDance = HasEffect(Buffs.StarfallDance);
                    bool curingWaltzReady = !GetCooldown(CuringWaltz).IsCooldown;
                    bool honingDanceReady = !GetCooldown(HoningDance).IsCooldown;
                    var acclaimStacks = GetBuffStacks(Buffs.Acclaim);
                    bool canWeave = CanWeave();
                    var distance = GetTargetDistance();
                    var HP = PlayerHealthPercentageHp();
                    bool enemyGuarded = TargetHasEffectAny(PvPCommon.Buffs.Guard);

                    #endregion

                    // Honing Dance Option

                    if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_Partner) && ActionReady(ClosedPosition) && !HasEffect(Buffs.ClosedPosition) & GetPartyMembers().Count > 1)
                        return ClosedPosition;

                    if (IsEnabled(CustomComboPreset.DNCPvP_Eagle) && PvPPhysRanged.CanEagleEyeShot() && (PvPCommon.TargetImmuneToDamage() || GetTargetHPPercent() <= Config.DNCPvP_EagleThreshold))
                        return PvPPhysRanged.EagleEyeShot;

                    if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_HoningDance) && honingDanceReady && HasTarget() && distance <= 5 && !enemyGuarded)
                    {
                        if (HasEffect(Buffs.Acclaim) && acclaimStacks < 4)
                            return WHM.Assize;

                        return HoningDance;
                    }

                    if (canWeave)
                    {
                        // Curing Waltz Option
                        if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_CuringWaltz) && curingWaltzReady && HP <= Config.DNCPvP_WaltzThreshold)
                            return OriginalHook(CuringWaltz);

                        // Fan Dance weave
                        if (IsOffCooldown(FanDance) && distance < 13 && !enemyGuarded) // 2y below max to avoid waste
                            return OriginalHook(FanDance);

                        if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_Dash) && !HasEffect(Buffs.EnAvant) && GetRemainingCharges(EnAvant) > Config.DNCPvP_EnAvantCharges)
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
