using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal class DRKPvP
    {
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

        public class Config
        {
            public const string
                ShadowbringerThreshold = nameof(ShadowbringerThreshold);

        }

        internal class DRKPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRKPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is HardSlash or SyphonStrike or Souleater)
                {
                    bool canWeave = CanWeave();
                    int shadowBringerThreshold = GetOptionValue(Config.ShadowbringerThreshold);

                    if (!PvPCommon.TargetImmuneToDamage())
                    {
                        if (IsEnabled(CustomComboPreset.DRKPvP_Plunge) && ActionReady(Plunge))
                        {
                            if (HasTarget() && (!InMeleeRange()) || (InMeleeRange() && IsEnabled(CustomComboPreset.DRKPvP_PlungeMelee)))
                                return OriginalHook(Plunge);
                        }

                        if (IsEnabled(CustomComboPreset.DRKPvP_Scorn) && HasEffect(Buffs.Scorn))
                            return OriginalHook(Eventide);

                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.DRKPvP_BlackestNight) && ActionReady(BlackestNight) && !HasEffect(Buffs.BlackestNight) && !WasLastAbility(BlackestNight))
                                return OriginalHook(BlackestNight);

                            if (IsEnabled(CustomComboPreset.DRKPvP_SaltedEarth) && ActionReady(SaltedEarth) && IsEnabled(CustomComboPreset.DRKPvP_SaltedEarth))
                                return OriginalHook(SaltedEarth);

                            if (IsEnabled(CustomComboPreset.DRKPvP_SaltAndDarkness) && HasEffect(Buffs.SaltedEarthDMG) && ActionReady(SaltAndDarkness))
                                return OriginalHook(SaltAndDarkness);

                            if (IsEnabled(CustomComboPreset.DRKPvP_Shadowbringer) && !HasEffect(Buffs.Blackblood) && (HasEffect(Buffs.DarkArts) || PlayerHealthPercentageHp() >= shadowBringerThreshold))
                                return OriginalHook(Shadowbringer);
                        }

                        if (InMeleeRange())
                        {
                            if (IsEnabled(CustomComboPreset.DRKPvP_Impalement) && ActionReady(Impalement))
                                return OriginalHook(Impalement);

                            if (comboTime > 1f)
                            {
                                if (lastComboActionID == HardSlash)
                                    return OriginalHook(SyphonStrike);

                                if (lastComboActionID == SyphonStrike)
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
