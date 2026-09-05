using ECommons.GameHelpers;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.CustomComboNS.Functions.Jobs;
using ContentHelper = ECommons.GameHelpers;
using IntendedUse = ECommons.ExcelServices.TerritoryIntendedUseEnum;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE;

internal static partial class Bozja
{
    /// In Bozja (in the field or a field raid).
    public static bool IsInBozja => ContentHelper.Content.TerritoryIntendedUse == IntendedUse.Bozja && (ContentCheck.IsInFieldOperations || ContentCheck.IsInFieldRaids);

    public static bool TryGetBozjaAction(ref uint actionID)
    {
        if (!IsInBozja) return false;

        bool CanUse(uint action) => ActionReady(action);
        bool IsEnabledAndUsable(Preset preset, uint action) => IsEnabled(preset) && CanUse(action);
        switch (GetRoleFromJob(Player.Job))
        {
            case JobRole.Tank:
                if (IsEnabled(Preset.Bozja_Tank))
                {
                    if (!InCombat() && IsEnabledAndUsable(Preset.Bozja_Tank_LostStealth, LostStealth))
                    {
                        actionID = LostStealth;
                        return true;
                    }

                    if (CanWeave())
                    {
                        foreach (var (preset, action) in new[]
                        {
                            (Preset.Bozja_Tank_LostFocus, LostFocus),
                            (Preset.Bozja_Tank_LostFontOfPower, LostFontOfPower),
                            (Preset.Bozja_Tank_LostSlash, LostSlash),
                            (Preset.Bozja_Tank_LostFairTrade, LostFairTrade),
                            (Preset.Bozja_Tank_LostAssassination, LostAssassination),
                        })
                            if (IsEnabledAndUsable(preset, action))
                            {
                                actionID = action;
                                return true;
                            }

                        foreach (var (preset, action, powerPreset) in new[]
                        {
                            (Preset.Bozja_Tank_BannerOfNobleEnds, BannerOfNobleEnds, Preset.Bozja_Tank_PowerEnds),
                            (Preset.Bozja_Tank_BannerOfHonoredSacrifice, BannerOfHonoredSacrifice, Preset.Bozja_Tank_PowerSacrifice)
                        })
                            if (IsEnabledAndUsable(preset, action) && (!IsEnabled(powerPreset) || JustUsed(LostFontOfPower, 5f)))
                            {
                                actionID = action;
                                return true;
                            }

                        if (IsEnabledAndUsable(Preset.Bozja_Tank_BannerOfHonedAcuity, BannerOfHonedAcuity) && !LocalPlayer.HasStatus(Buffs.BannerOfTranscendentFinesse))
                        {
                            actionID = BannerOfHonedAcuity;
                            return true;
                        }
                    }

                    foreach (var (preset, action, condition) in new[]
                    {
                        (Preset.Bozja_Tank_LostDeath, LostDeath, true),
                        (Preset.Bozja_Tank_LostCure, LostCure, PlayerHealthPercentageHp() <= Config.Bozja_Tank_LostCure_Health),
                        (Preset.Bozja_Tank_LostArise, LostArise, GetTargetHPPercent() == 0 && !LocalPlayer.HasStatus(RoleActions.Magic.Buffs.Raise)),
                        (Preset.Bozja_Tank_LostReraise, LostReraise, PlayerHealthPercentageHp() <= Config.Bozja_Tank_LostReraise_Health),
                        (Preset.Bozja_Tank_LostProtect, LostProtect, !LocalPlayer.HasStatus(Buffs.LostProtect)),
                        (Preset.Bozja_Tank_LostShell, LostShell, !LocalPlayer.HasStatus(Buffs.LostShell)),
                        (Preset.Bozja_Tank_LostBravery, LostBravery, !LocalPlayer.HasStatus(Buffs.LostBravery)),
                        (Preset.Bozja_Tank_LostBubble, LostBubble, !LocalPlayer.HasStatus(Buffs.LostBubble)),
                        (Preset.Bozja_Tank_LostParalyze3, LostParalyze3, !JustUsed(LostParalyze3, 60f))
                    })
                        if (IsEnabledAndUsable(preset, action) && condition)
                        {
                            actionID = action;
                            return true;
                        }

                    if (IsEnabled(Preset.Bozja_Tank_LostSpellforge) && CanUse(LostSpellforge) && (!LocalPlayer.HasStatus(Buffs.LostSpellforge) || !LocalPlayer.HasStatus(Buffs.LostSteelsting)))
                    {
                        actionID = LostSpellforge;
                        return true;
                    }

                    if (IsEnabled(Preset.Bozja_Tank_LostSteelsting) && CanUse(LostSteelsting) && (!LocalPlayer.HasStatus(Buffs.LostSpellforge) || !LocalPlayer.HasStatus(Buffs.LostSteelsting)))
                    {
                        actionID = LostSteelsting;
                        return true;
                    }
                }
                break;

            //TODO: implement rest when somebody cares
            case JobRole.Healer:
                break;

            case JobRole.RangedDPS:
                break;

            case JobRole.MeleeDPS:
                break;

            case JobRole.MagicalDPS:
                break;
        }

        return false;
    }
}