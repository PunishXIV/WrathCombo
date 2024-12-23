#region

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class BLM
{
    // BLM Gauge & Extensions
    internal static BLMGauge Gauge = GetJobGauge<BLMGauge>();
    internal static BLMOpenerMaxLevel1 Opener1 = new();

    internal static uint CurMp => LocalPlayer.CurrentMp;

    internal static int MaxPolyglot =>
        TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 :
        TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;

    internal static float ElementTimer => Gauge.ElementTimeRemaining / 1000f;

    internal static double GCDsInTimer =>
        Math.Floor(ElementTimer / GetActionCastTime(Gauge.InAstralFire ? Fire : Blizzard));

    internal static int RemainingPolyglotCD =>
        Math.Max(0,
            (MaxPolyglot - Gauge.PolyglotStacks) * 30000 + (Gauge.EnochianTimer - 30000));

    internal static Status? ThunderDebuffST =>
        FindEffect(ThunderList[OriginalHook(Thunder)], CurrentTarget, LocalPlayer?.GameObjectId);

    internal static Status? ThunderDebuffAoE =>
        FindEffect(ThunderList[OriginalHook(Thunder2)], CurrentTarget, LocalPlayer?.GameObjectId);

    internal static bool CanSwiftF =>
        TraitLevelChecked(Traits.AspectMasteryIII) &&
        IsOffCooldown(All.Swiftcast);

    internal static bool HasPolyglotStacks(BLMGauge gauge) => gauge.PolyglotStacks > 0;

    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;

        return WrathOpener.Dummy;
    }

    internal static float MPAfterCast()
    {
        uint castedSpell = LocalPlayer.CastActionId;

        int nextMpGain = Gauge.UmbralIceStacks switch
        {
            0 => 0,
            1 => 2500,
            2 => 5000,
            3 => 10000,
            var _ => 0
        };

        return castedSpell is Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2
            ? Math.Max(LocalPlayer.MaxMp, LocalPlayer.CurrentMp + nextMpGain)
            : Math.Max(0, LocalPlayer.CurrentMp - GetResourceCost(castedSpell));
    }

    internal static bool DoubleBlizz()
    {
        List<uint> spells = ActionWatching.CombatActions.Where(x =>
            ActionWatching.GetAttackType(x) == ActionWatching.ActionAttackType.Spell &&
            x != OriginalHook(Thunder) && x != OriginalHook(Thunder2)).ToList();

        if (spells.Count < 1)
            return false;

        uint firstSpell = spells[^1];

        switch (firstSpell)
        {
            case Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2:
            {
                uint castedSpell = LocalPlayer.CastActionId;

                if (castedSpell is Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2)
                    return true;

                if (spells.Count >= 2)
                {
                    uint secondSpell = spells[^2];

                    switch (secondSpell)
                    {
                        case Blizzard or Blizzard2 or Blizzard3 or Blizzard4 or Freeze or HighBlizzard2:
                            return true;
                    }
                }

                break;
            }
        }

        return false;
    }
    internal class BLMOpenerMaxLevel1 : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            Fire3,
            HighThunder,
            All.Swiftcast,
            Amplifier,
            Fire4,
            Fire4,
            Xenoglossy,
            Triplecast,
            LeyLines,
            Fire4,
            Fire4,
            Despair,
            Manafont,
            Fire4,
            Triplecast,
            Fire4,
            FlareStar,
            Fire4,
            HighThunder,
            Paradox,
            Fire4,
            Fire4,
            Fire4,
            Despair
        ];
        internal override UserData? ContentCheckConfig => Config.BLM_ST_Balance_Content;

        public override bool HasCooldowns()
        {
            if (GetCooldown(Fire).BaseCooldownTotal > 2.45)
                return false;

            if (!ActionReady(Manafont))
                return false;

            if (GetRemainingCharges(Triplecast) < 2)
                return false;

            if (!ActionReady(All.Swiftcast))
                return false;

            if (!ActionReady(Amplifier))
                return false;

            if (Gauge.InUmbralIce)
                return false;

            return true;
        }
    }
}