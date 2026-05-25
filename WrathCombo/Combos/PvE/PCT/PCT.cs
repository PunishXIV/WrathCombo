using Dalamud.Game.ClientState.JobGauge.Types;
using WrathCombo.CustomComboNS;
using static WrathCombo.Combos.PvE.PCT.Config;
namespace WrathCombo.Combos.PvE;

internal partial class PCT : Caster
{
    #region Single Target Combos
    internal class PCT_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PCT_ST_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FireInRed)
                return actionID;
            

            // Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            
            const Combo comboFlags = Combo.ST | Combo.Simple;
            
            //OGCD Spells
            if (TryOGCDSpells(comboFlags, ref actionID))
                return actionID;
            
            //Mitigation
            if (TryMitigation(comboFlags, ref actionID))
                return actionID;
            
            //Movement Options
            if (TryMovementOption(comboFlags, ref actionID))
                return actionID;
            
            //GCD Spells
            if (TryGCDSpells(comboFlags, ref actionID))
                return actionID;
            
            //Motifs
            if (TryDrawMotif(comboFlags, ref actionID))
                return actionID;
            
            //SubCombo
            if (TryCombos(comboFlags, ref actionID))
                return actionID;
            else
                return actionID;
            
            
        }
    }
    internal class PCT_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PCT_ST_AdvancedMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FireInRed)
                return actionID;

            //Opener
            if (IsEnabled(Preset.PCT_ST_Advanced_Openers) && Opener().FullOpener(ref actionID))
                return actionID;

            // Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            
            const Combo comboFlags = Combo.ST | Combo.Adv;
            
            //OGCD Spells
            if (TryOGCDSpells(comboFlags, ref actionID))
                return actionID;
            
            //Mitigation
            if (TryMitigation(comboFlags, ref actionID))
                return actionID;
            
            //Movement Options
            if (TryMovementOption(comboFlags, ref actionID))
                return actionID;
            
            //GCD Spells
            if (TryGCDSpells(comboFlags, ref actionID))
                return actionID;
            
            //Motifs
            if (TryDrawMotif(comboFlags, ref actionID))
                return actionID;
            
            //SubCombo
            if (TryCombos(comboFlags, ref actionID))
                return actionID;
            else
                return actionID;
        }
    }
    #endregion

    #region AOE Combos

    internal class PCT_AoE_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PCT_AoE_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FireIIinRed)
                return actionID;

            // Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            
            const Combo comboFlags = Combo.AoE | Combo.Simple;
            
            //OGCD Spells
            if (TryOGCDSpells(comboFlags, ref actionID))
                return actionID;
            
            //Movement Options
            if (TryMovementOption(comboFlags, ref actionID))
                return actionID;
            
            //GCD Spells
            if (TryGCDSpells(comboFlags, ref actionID))
                return actionID;
            
            //Motifs
            if (TryDrawMotif(comboFlags, ref actionID))
                return actionID;
            
            //SubCombo
            if (TryCombos(comboFlags, ref actionID))
                return actionID;
            else
                return actionID;
        }
    }
    internal class PCT_AoE_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PCT_AoE_AdvancedMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FireIIinRed)
                return actionID;

            // Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            
            const Combo comboFlags = Combo.AoE | Combo.Adv;
            
            //OGCD Spells
            if (TryOGCDSpells(comboFlags, ref actionID))
                return actionID;
            
            //Movement Options
            if (TryMovementOption(comboFlags, ref actionID))
                return actionID;
            
            //GCD Spells
            if (TryGCDSpells(comboFlags, ref actionID))
                return actionID;
            
            //Motifs
            if (TryDrawMotif(comboFlags, ref actionID))
                return actionID;
            
            //SubCombo
            if (TryCombos(comboFlags, ref actionID))
                return actionID;
            else
                return actionID;
        }
    }
    #endregion

    #region Smaller Features
    internal class CombinedAetherhues : CustomCombo
    {
        protected internal override Preset Preset => Preset.CombinedAetherhues;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (BlizzardinCyan or BlizzardIIinCyan))
                return actionID;

            int choice = CombinedAetherhueChoices;

            if (actionID == BlizzardinCyan && choice is 0 or 1)
            {
                return HasStatusEffect(Buffs.SubtractivePalette)
                    ? OriginalHook(BlizzardinCyan)
                    : OriginalHook(FireInRed);
                
            }
            if (actionID == BlizzardIIinCyan && choice is 0 or 2)
            {
                return HasStatusEffect(Buffs.SubtractivePalette)
                    ? OriginalHook(BlizzardIIinCyan)
                    : OriginalHook(FireIIinRed);
            }
            return actionID;
        }
    }
    internal class CombinedMotifs : CustomCombo
    {
        protected internal override Preset Preset => Preset.CombinedMotifs;
        protected override uint Invoke(uint actionID)
        {
            PCTGauge gauge = GetJobGauge<PCTGauge>();

            if (actionID == CreatureMotif)
            {
                if ((CombinedMotifsMog && gauge.MooglePortraitReady) || (CombinedMotifsMadeen && gauge.MadeenPortraitReady && IsOffCooldown(OriginalHook(MogoftheAges))))
                    return OriginalHook(MogoftheAges);

                if (gauge.CreatureMotifDrawn)
                    return OriginalHook(LivingMuse);
            }
            if (actionID == WeaponMotif)
            {
                if (CombinedMotifsWeapon && HasStatusEffect(Buffs.HammerTime))
                    return OriginalHook(HammerStamp);

                if (gauge.WeaponMotifDrawn)
                    return OriginalHook(SteelMuse);
            }
            if (actionID == LandscapeMotif)
            {
                if (CombinedMotifsLandscape && HasStatusEffect(Buffs.Starstruck))
                    return OriginalHook(StarPrism);

                if (gauge.LandscapeMotifDrawn)
                    return OriginalHook(ScenicMuse);
            }
            return actionID;
        }
    }
    internal class CombinedPaint : CustomCombo
    {
        protected internal override Preset Preset => Preset.CombinedPaint;
        protected override uint Invoke(uint actionID)
        {
            if (actionID != HolyInWhite)
                return actionID;
            if (HasStatusEffect(Buffs.MonochromeTones))
                return CometinBlack;
            return actionID;
        }
    }
    #endregion
}
