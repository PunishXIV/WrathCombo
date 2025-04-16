using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvP
{
    internal class DRKPvP
    {
        #region IDS

        public const byte JobID = 32;

        internal class Role : PvPTank;

        public const uint
            HardSlash = 29085,
            SyphonStrike = 29086,
            Souleater = 29087,
            Shadowbringer = 29091,
            Plunge = 29092,
            BlackestNight = 29093,
            SaltedEarth = 29094,
            Bloodspiller = 29088,
            SaltAndDarkness = 29095,
            Impalement = 41438,
            Eventide = 29097;

        public class Buffs
        {
            public const ushort
                Blackblood = 3033,
                BlackestNight = 1308,
                SaltedEarthDMG = 3036,
                SaltedEarthDEF = 3037,
                DarkArts = 3034,
                UndeadRedemption = 3039,
                Scorn = 4290;
        }
        #endregion

        #region Config
        public static class Config
        {
            public static UserInt
                ShadowbringerThreshold = new("ShadowbringerThreshold"),
                DRKPvP_RampartThreshold = new("DRKPvP_RampartThreshold");


            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.DRKPvP_Shadowbringer:
                        UserConfig.DrawSliderInt(20, 100,
                            ShadowbringerThreshold,
                            "HP% to be at or Above to use ",
                            itemWidth: 150f, sliderIncrement: SliderIncrements.Fives);

                        break;

                    case CustomComboPreset.DRKPvP_Rampart:
                        UserConfig.DrawSliderInt(1, 100, DRKPvP_RampartThreshold,
                            "Use Rampart below set threshold for self");
                        break;

                }
            }
        }
        #endregion
      

        internal class DRKPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRKPvP_Burst;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is HardSlash or SyphonStrike or Souleater)
                {
                    bool canWeave = CanWeave();
                    int shadowBringerThreshold = Config.ShadowbringerThreshold;

                    if (IsEnabled(CustomComboPreset.DRKPvP_Rampart) && PvPTank.CanRampart(Config.DRKPvP_RampartThreshold))
                        return PvPTank.Rampart;

                    if (!PvPCommon.TargetImmuneToDamage())
                    {
                        if (IsEnabled(CustomComboPreset.DRKPvP_Plunge) && ActionReady(Plunge))
                        {
                            if (HasTarget() && (!InMeleeRange()) || (InMeleeRange() && IsEnabled(CustomComboPreset.DRKPvP_PlungeMelee)))
                                return OriginalHook(Plunge);
                        }

                        if (IsEnabled(CustomComboPreset.DRKPvP_Scorn) && HasStatusEffect(Buffs.Scorn))
                            return OriginalHook(Eventide);

                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DRKPvP_BlackestNight) && ActionReady(BlackestNight) && !HasStatusEffect(Buffs.BlackestNight) && !WasLastAbility(BlackestNight))
                                return OriginalHook(BlackestNight);

                            if (IsEnabled(CustomComboPreset.DRKPvP_SaltedEarth) && ActionReady(SaltedEarth) && IsEnabled(CustomComboPreset.DRKPvP_SaltedEarth))
                                return OriginalHook(SaltedEarth);

                            if (IsEnabled(CustomComboPreset.DRKPvP_SaltAndDarkness) && HasStatusEffect(Buffs.SaltedEarthDMG) && ActionReady(SaltAndDarkness))
                                return OriginalHook(SaltAndDarkness);

                            if (IsEnabled(CustomComboPreset.DRKPvP_Shadowbringer) && !HasStatusEffect(Buffs.Blackblood) && (HasStatusEffect(Buffs.DarkArts) || PlayerHealthPercentageHp() >= shadowBringerThreshold))
                                return OriginalHook(Shadowbringer);
                        }

                        if (InMeleeRange())
                        {
                            if (IsEnabled(CustomComboPreset.DRKPvP_Impalement) && ActionReady(Impalement))
                                return OriginalHook(Impalement);

                            if (ComboTimer > 1f)
                            {
                                if (ComboAction == HardSlash)
                                    return OriginalHook(SyphonStrike);

                                if (ComboAction == SyphonStrike)
                                    return OriginalHook(Souleater);
                            }

                            return OriginalHook(HardSlash);
                        }
                    }
                }

                return actionID;
            }
        }

    }
}
