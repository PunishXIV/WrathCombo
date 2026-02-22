using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.SMN.Config;
namespace WrathCombo.Combos.PvE;

internal partial class SMN : Caster
{
    #region Simple
    internal class SMN_Simple_Combo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_ST_Simple_Combo;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ruin or Ruin2 or Ruin3))
                return actionID;

            if (NeedToSummon)
                return SummonCarbuncle;

            #region Variables
            const Combo comboFlags = Combo.ST | Combo.Simple;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            if (TryOGCDSpells(comboFlags, ref actionID))
                return actionID == Rekindle && IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons_Rekindle)
                    ? OriginalHook(AstralFlow).Retarget([Ruin, Ruin2, Ruin3], 
                        SimpleTarget.TargetsTarget.IfInParty() ??
                        SimpleTarget.AnyTank.IfMissingHP() ??
                        SimpleTarget.LowestHPPAlly.IfMissingHP() ??
                        SimpleTarget.Self)
                    : actionID;
            
            if (TryMitigation(comboFlags, ref actionID))
                return actionID;
            
            if (TrySummonSpells(comboFlags, ref actionID))
                return actionID;

            #region Egi Priority
            // Egi Order
            if (!ActionReady(OriginalHook(Aethercharge)) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0)
            {
                if (Gauge.IsTitanReady)
                    return OriginalHook(SummonTopaz);

                if (Gauge.IsGarudaReady)
                    return OriginalHook(SummonEmerald);

                if (Gauge.IsIfritReady)
                    return OriginalHook(SummonRuby);
            }
            #endregion

            return actionID;
        }
    }
    internal class SMN_Simple_Combo_AoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_AoE_Simple_Combo;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Outburst or Tridisaster))
                return actionID;

            if (NeedToSummon && ActionReady(SummonCarbuncle))
                return SummonCarbuncle;

            #region Variables
            const Combo comboFlags = Combo.AoE | Combo.Simple;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            if (TryOGCDSpells(comboFlags, ref actionID))
                return actionID == Rekindle && IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons_Rekindle)
                    ? OriginalHook(AstralFlow).Retarget([Outburst, Tridisaster], 
                        SimpleTarget.TargetsTarget.IfInParty() ??
                        SimpleTarget.AnyTank.IfMissingHP() ??
                        SimpleTarget.LowestHPPAlly.IfMissingHP() ??
                        SimpleTarget.Self)
                    : actionID;
            
            if (TryMitigation(comboFlags, ref actionID))
                return actionID;
            
            if (TrySummonSpells(comboFlags, ref actionID))
                return actionID;

            #region Egi Priority
            if (!ActionReady(OriginalHook(Aethercharge)) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0)
            {
                if (Gauge.IsTitanReady)
                    return OriginalHook(SummonTopaz);

                if (Gauge.IsGarudaReady)
                    return OriginalHook(SummonEmerald);

                if (Gauge.IsIfritReady)
                    return OriginalHook(SummonRuby);
            }
            #endregion

            return actionID;
        }
    }
    #endregion

    #region Advanced
    internal class SMN_ST_Advanced_Combo : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_ST_Advanced_Combo;
        protected override uint Invoke(uint actionID)
        {
            bool allRuins = SMN_ST_Advanced_Combo_AltMode == 0;
            bool actionFound = allRuins && AllRuinsList.Contains(actionID) ||
                               !allRuins && NotRuin3List.Contains(actionID);
            if (!actionFound)
                return actionID;

            if (NeedToSummon)
                return SummonCarbuncle;

            #region Variables
            const Combo comboFlags = Combo.ST | Combo.Adv;
            var replacedActions = allRuins ? AllRuinsList.ToArray() : NotRuin3List.ToArray();
            #endregion

            #region Opener
            if (IsEnabled(Preset.SMN_ST_Advanced_Combo_Balance_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion
            
            if (TryOGCDSpells(comboFlags, ref actionID))
                return actionID == Rekindle && IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons_Rekindle)
                    ? OriginalHook(AstralFlow).Retarget(replacedActions, 
                         SimpleTarget.TargetsTarget.IfInParty() ??
                         SimpleTarget.AnyTank.IfMissingHP() ??
                         SimpleTarget.LowestHPPAlly.IfMissingHP() ??
                         SimpleTarget.Self)
                    : actionID;
            
            if (TryMitigation(comboFlags, ref actionID))
                return actionID;
            
            if (TrySummonSpells(comboFlags, ref actionID))
                return actionID;

            #region Egi Priority
            foreach (var prio in SMN_ST_Egi_Priority.OrderBy(x => x))
            {
                var index = SMN_ST_Egi_Priority.IndexOf(prio);
                var config = GetMatchingConfigST(index, out var spell,
                    out var enabled);

                if (!enabled) continue;

                if (!ActionReady(OriginalHook(Aethercharge)) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0)
                    return spell;
            }
            #endregion
            
            return actionID;
        }
    }

    internal class SMN_Advanced_Combo_AoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_AoE_Advanced_Combo;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Outburst or Tridisaster))
                return actionID;

            if (NeedToSummon)
                return SummonCarbuncle;

            #region Variables
            const Combo comboFlags = Combo.AoE | Combo.Adv;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            if (TryOGCDSpells(comboFlags, ref actionID))
                return actionID == Rekindle && IsEnabled(Preset.SMN_ST_Advanced_Combo_DemiSummons_Rekindle)
                    ? OriginalHook(AstralFlow).Retarget([Outburst, Tridisaster], 
                        SimpleTarget.TargetsTarget.IfInParty() ??
                        SimpleTarget.AnyTank.IfMissingHP() ??
                        SimpleTarget.LowestHPPAlly.IfMissingHP() ??
                        SimpleTarget.Self)
                    : actionID;
            
            if (TryMitigation(comboFlags, ref actionID))
                return actionID;
            
            if (TrySummonSpells(comboFlags, ref actionID))
                return actionID;

            #region Egi Priority
            foreach (var prio in SMN_AoE_Egi_Priority.OrderBy(x => x))
            {
                var index = SMN_AoE_Egi_Priority.IndexOf(prio);
                var config = GetMatchingConfigAoE(index, out var spell,
                    out var enabled);

                if (!enabled) continue;

                if (!ActionReady(OriginalHook(Aethercharge)) && Gauge.SummonTimerRemaining == 0 && Gauge.AttunementTimerRemaining == 0)
                    return spell;
            }
            #endregion

            return actionID;
        }
    }
    #endregion
    
    #region Small Features
    internal class SMN_Raise : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_Raise;
        protected override uint Invoke(uint actionID)
        {
            if (actionID != Role.Swiftcast)
                return actionID;

            if (IsOnCooldown(Role.Swiftcast))
                return IsEnabled(Preset.SMN_Raise_Retarget)
                    ? Resurrection.Retarget(Role.Swiftcast,
                        SimpleTarget.Stack.AllyToRaise)
                    : Resurrection;
            return actionID;
        }
    }
    internal class SMN_Searing : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_Searing;
        protected override uint Invoke(uint actionID)
        {
            if (actionID != SearingLight)
                return actionID;

            if (HasStatusEffect(Buffs.RubysGlimmer))
                return SearingFlash;

            return HasStatusEffect(Buffs.SearingLight, anyOwner: true) ? 11 : actionID;
        }
    }
    internal class SMN_RuinMobility : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_RuinMobility;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Ruin4)
                return actionID;
            bool furtherRuin = HasStatusEffect(Buffs.FurtherRuin);

            return !furtherRuin ? Ruin3 : actionID;
        }
    }
    internal class SMN_EDFester : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_EDFester;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Fester or Necrotize))
                return actionID;

            SMNGauge gauge = GetJobGauge<SMNGauge>();
            if (HasStatusEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergyDrain) && !gauge.HasAetherflowStacks && IsEnabled(Preset.SMN_EDFester_Ruin4))
                return Ruin4;

            if (LevelChecked(EnergyDrain) && !gauge.HasAetherflowStacks)
                return EnergyDrain;

            return actionID;
        }
    }
    internal class SMN_ESPainflare : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_ESPainflare;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Painflare)
                return actionID;

            SMNGauge gauge = GetJobGauge<SMNGauge>();

            if (!LevelChecked(Painflare) || gauge.HasAetherflowStacks)
                return actionID;

            if (HasStatusEffect(Buffs.FurtherRuin) && IsOnCooldown(EnergySiphon) && IsEnabled(Preset.SMN_ESPainflare_Ruin4))
                return Ruin4;

            if (LevelChecked(EnergySiphon))
                return EnergySiphon;

            return LevelChecked(EnergyDrain) ? EnergyDrain : actionID;
        }
    }
    internal class SMN_CarbuncleReminder : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_CarbuncleReminder;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ruin or Ruin2 or Ruin3 or DreadwyrmTrance or
                 AstralFlow or EnkindleBahamut or SearingLight or
                 RadiantAegis or Outburst or Tridisaster or
                 PreciousBrilliance or Gemshine))
                return actionID;

            return NeedToSummon ? SummonCarbuncle : actionID;
        }
    }

    internal class SMN_Egi_AstralFlow : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_Egi_AstralFlow;

        protected override uint Invoke(uint actionID)
        {
            if ((actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 or SummonRuby or SummonIfrit or SummonIfrit2 && HasStatusEffect(Buffs.TitansFavor)) ||
                (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonEmerald or SummonGaruda or SummonGaruda2 && HasStatusEffect(Buffs.GarudasFavor)) ||
                (actionID is SummonTopaz or SummonTitan or SummonTitan2 or SummonRuby or SummonIfrit or SummonIfrit2 && (HasStatusEffect(Buffs.IfritsFavor) || (ComboAction == CrimsonCyclone && InMeleeRange()))))
                return OriginalHook(AstralFlow);

            return actionID;
        }
    }
    internal class SMN_DemiAbilities : CustomCombo
    {
        protected internal override Preset Preset => Preset.SMN_DemiAbilities;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Aethercharge or DreadwyrmTrance or SummonBahamut) &&
                actionID is not (SummonPhoenix or SummonSolarBahamut))
                return actionID;

            if (IsOffCooldown(EnkindleBahamut) && OriginalHook(Ruin) is AstralImpulse)
                return OriginalHook(EnkindleBahamut);

            if (IsOffCooldown(EnkindlePhoenix) && OriginalHook(Ruin) is FountainOfFire)
                return OriginalHook(EnkindlePhoenix);

            if (IsOffCooldown(EnkindleSolarBahamut) && OriginalHook(Ruin) is UmbralImpulse)
                return OriginalHook(EnkindleBahamut);

            if ((OriginalHook(AstralFlow) is Deathflare && IsOffCooldown(Deathflare)) || (OriginalHook(AstralFlow) is Rekindle && IsOffCooldown(Rekindle)))
                return OriginalHook(AstralFlow);

            if (OriginalHook(AstralFlow) is Sunflare && IsOffCooldown(Sunflare))
                return OriginalHook(Sunflare);

            return actionID;
        }
    }
    #endregion
}
