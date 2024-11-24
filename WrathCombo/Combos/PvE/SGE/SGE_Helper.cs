using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE
{
    internal static partial class SGE
    {
        // Sage Gauge & Extensions
        public static SGEGauge Gauge => GetJobGauge<SGEGauge>();
        public static bool HasAddersgall(this SGEGauge gauge) => gauge.Addersgall > 0;
        public static bool HasAddersting(this SGEGauge gauge) => gauge.Addersting > 0;


        public static int GetMatchingConfigST(int i, IGameObject? optionalTarget, out uint action, out bool enabled)
        {
            var healTarget = optionalTarget ?? GetHealTarget(Config.SGE_ST_Heal_Adv && Config.SGE_ST_Heal_UIMouseOver);

            switch (i)
            {
                case 0:
                    action = Soteria;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Soteria);
                    return Config.SGE_ST_Heal_Soteria;
                case 1:
                    action = Zoe;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Zoe);
                    return Config.SGE_ST_Heal_Zoe;
                case 2:
                    action = Pepsis;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) && FindEffect(Buffs.EukrasianDiagnosis, healTarget, LocalPlayer?.GameObjectId) is not null;
                    return Config.SGE_ST_Heal_Pepsis;
                case 3:
                    action = Taurochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Taurochole) && Gauge.HasAddersgall();
                    return Config.SGE_ST_Heal_Taurochole;
                case 4:
                    action = Haima;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Haima);
                    return Config.SGE_ST_Heal_Haima;
                case 5:
                    action = Krasis;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Krasis);
                    return Config.SGE_ST_Heal_Krasis;
                case 6:
                    action = Druochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Druochole) && Gauge.HasAddersgall();
                    return Config.SGE_ST_Heal_Druochole;
            }

            enabled = false;
            action = 0;
            return 0;
        }

        public static int GetMatchingConfigAoE(int i, out uint action, out bool enabled)
        {
            switch (i)
            {
                case 0:
                    action = Kerachole;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) && (!Config.SGE_AoE_Heal_KeracholeTrait || (Config.SGE_AoE_Heal_KeracholeTrait && TraitLevelChecked(Traits.EnhancedKerachole))) && Gauge.HasAddersgall();
                    return 0;
                case 1:
                    action = Ixochole;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Ixochole) && Gauge.HasAddersgall();
                    return 0;
                case 2:
                    action = OriginalHook(Physis);
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Physis);
                    return 0;
                case 3:
                    action = Holos;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Holos);
                    return 0;
                case 4:
                    action = Panhaima;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Panhaima);
                    return 0;
                case 5:
                    action = Pepsis;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) && FindEffect(Buffs.EukrasianPrognosis) is not null;
                    return 0;
                case 6:
                    action = Philosophia;
                    enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Philosophia);
                    return 0;
            }

            enabled = false;
            action = 0;
            return 0;
        }
    }
}
