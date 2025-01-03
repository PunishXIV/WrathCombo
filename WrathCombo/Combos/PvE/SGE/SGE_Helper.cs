using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal static partial class SGE
{
    #region ID's

    internal const byte JobID = 40;

    // Actions
    internal const uint

        // Heals and Shields
        Diagnosis = 24284,
        Prognosis = 24286,
        Physis = 24288,
        Druochole = 24296,
        Kerachole = 24298,
        Ixochole = 24299,
        Pepsis = 24301,
        Physis2 = 24302,
        Taurochole = 24303,
        Haima = 24305,
        Panhaima = 24311,
        Holos = 24310,
        EukrasianDiagnosis = 24291,
        EukrasianPrognosis = 24292,
        Egeiro = 24287,

        // DPS
        Dosis = 24283,
        Dosis2 = 24306,
        Dosis3 = 24312,
        EukrasianDosis = 24293,
        EukrasianDosis2 = 24308,
        EukrasianDosis3 = 24314,
        Phlegma = 24289,
        Phlegma2 = 24307,
        Phlegma3 = 24313,
        Dyskrasia = 24297,
        Dyskrasia2 = 24315,
        Toxikon = 24304,
        Toxikon2 = 24316,
        Pneuma = 24318,
        EukrasianDyskrasia = 37032,
        Psyche = 37033,

        // Buffs
        Soteria = 24294,
        Zoe = 24300,
        Krasis = 24317,
        Philosophia = 37035,

        // Other
        Kardia = 24285,
        Eukrasia = 24290,
        Rhizomata = 24309;

    // Action Groups
    internal static readonly List<uint>
        AddersgallList = [Taurochole, Druochole, Ixochole, Kerachole],
        DyskrasiaList = [Dyskrasia, Dyskrasia2];

    // Debuff Pairs of Actions and Debuff
    internal static readonly Dictionary<uint, ushort>
        DosisList = new()
        {
            { Dosis, Debuffs.EukrasianDosis },
            { Dosis2, Debuffs.EukrasianDosis2 },
            { Dosis3, Debuffs.EukrasianDosis3 }
        };

    // Action Buffs
    internal static class Buffs
    {
        internal const ushort
            Kardia = 2604,
            Kardion = 2605,
            Eukrasia = 2606,
            EukrasianDiagnosis = 2607,
            EukrasianPrognosis = 2609,
            Panhaima = 2613,
            Kerachole = 2618,
            Eudaimonia = 3899;
    }

    internal static class Debuffs
    {
        internal const ushort
            EukrasianDosis = 2614,
            EukrasianDosis2 = 2615,
            EukrasianDosis3 = 2616,
            EukrasianDyskrasia = 3897;
    }

    internal static class Traits
    {
        internal const ushort
            EnhancedKerachole = 375,
            OffensiveMagicMasteryII = 376;
    }

    #endregion

    internal static SGEOpenerMaxLevel1 Opener1 = new();

    internal static SGEGauge Gauge = GetJobGauge<SGEGauge>();

    internal static bool HasAddersgall(this SGEGauge gauge) => gauge.Addersgall > 0;

    internal static bool HasAddersting(this SGEGauge gauge) => gauge.Addersting > 0;

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    public static int GetMatchingConfigST(int i, IGameObject? optionalTarget, out uint action, out bool enabled)
    {
        IGameObject? healTarget =
            optionalTarget ?? GetHealTarget(Config.SGE_ST_Heal_Adv && Config.SGE_ST_Heal_UIMouseOver);

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

                enabled = IsEnabled(CustomComboPreset.SGE_ST_Heal_Pepsis) &&
                          FindEffect(Buffs.EukrasianDiagnosis, healTarget, LocalPlayer?.GameObjectId) is not null;

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

                enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Kerachole) &&
                          (!Config.SGE_AoE_Heal_KeracholeTrait ||
                           (Config.SGE_AoE_Heal_KeracholeTrait && TraitLevelChecked(Traits.EnhancedKerachole))) &&
                          Gauge.HasAddersgall();

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

                enabled = IsEnabled(CustomComboPreset.SGE_AoE_Heal_Pepsis) &&
                          FindEffect(Buffs.EukrasianPrognosis) is not null;

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

    internal class SGEOpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 92;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Eukrasia,
            Toxikon2,
            EukrasianDosis3,
            Dosis3,
            Dosis3,
            Dosis3,
            Phlegma3,
            Psyche,
            Phlegma3,
            Dosis3,
            Dosis3,
            Dosis3,
            Dosis3,
            Eukrasia,
            EukrasianDosis3,
            Dosis3,
            Dosis3,
            Dosis3
        ];
        internal override UserData? ContentCheckConfig => Config.SGE_Balance_Content;

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(Phlegma3) < 2)
                return false;

            if (!ActionReady(Psyche))
                return false;

            if (!HasAddersting(Gauge))
                return false;

            return true;
        }
    }
}