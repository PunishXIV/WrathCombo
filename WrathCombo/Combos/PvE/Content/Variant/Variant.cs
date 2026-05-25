using ECommons.DalamudServices;
using ECommons.GameHelpers;
using WrathCombo.Attributes;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.CustomComboNS.Functions.Jobs;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

namespace WrathCombo.Combos.PvE;

// Static utility class for shared logic
internal static partial class Variant
{
    public static bool IsInVariantDungeon => ContentCheck.IsInVariantDungeon;

    private static ushort TerritoryID
    {
        get
        {
            if (!EZ.Throttle("variantTerritory", TS.FromSeconds(4)))
                return field;

            field = (ushort)Svc.ClientState.TerritoryType;
            return field;
        }
    }

    public static bool TryGetVariantAction(ref uint actionID)
    {
        if (!IsInVariantDungeon) return false;

        switch (GetRoleFromJob(Player.Job))
        {
            case JobRole.Tank:
                if (IsEnabled(Preset.Variant_Tank))
                {
                    if (CheckCure(Preset.Variant_Tank_Cure, Config.Variant_Tank_Cure))
                    {
                        actionID = Cure;
                        return true;
                    }
                    if (CheckUltimatum(Preset.Variant_Tank_Ultimatum))
                    {
                        actionID = Ultimatum;
                        return true;
                    }
                    if (CheckRaise(Preset.Variant_Tank_Raise))
                    {
                        actionID = Raise;
                        return true;
                    }
                    if (CheckSpiritDart(Preset.Variant_Tank_SpiritDart))
                    {
                        actionID = SpiritDart;
                        return true;
                    }
                    if (CheckEagleEyeShot(Preset.Variant_Tank_EagleEyeShot))
                    {
                        actionID = EagleEyeShot;
                        return true;
                    }
                }
                break;

            case JobRole.Healer:
                if (IsEnabled(Preset.Variant_Healer))
                {
                    if (CheckUltimatum(Preset.Variant_Healer_Ultimatum))
                    {
                        actionID = Ultimatum;
                        return true;
                    }
                    if (CheckSpiritDart(Preset.Variant_Healer_SpiritDart))
                    {
                        actionID = SpiritDart;
                        return true;
                    }
                    if (CheckRampart(Preset.Variant_Healer_Rampart))
                    {
                        actionID = Rampart;
                        return true;
                    }
                    if (CheckEagleEyeShot(Preset.Variant_Healer_EagleEyeShot))
                    {
                        actionID = EagleEyeShot;
                        return true;
                    }
                }
                break;

            case JobRole.RangedDPS:
                if (IsEnabled(Preset.Variant_PhysRanged))
                {
                    if (CheckCure(Preset.Variant_PhysRanged_Cure, Config.Variant_PhysRanged_Cure))
                    {
                        actionID = Cure;
                        return true;
                    }
                    if (CheckUltimatum(Preset.Variant_PhysRanged_Ultimatum))
                    {
                        actionID = Ultimatum;
                        return true;
                    }
                    if (CheckRaise(Preset.Variant_PhysRanged_Raise))
                    {
                        actionID = Raise;
                        return true;
                    }
                    if (CheckRampart(Preset.Variant_PhysRanged_Rampart))
                    {
                        actionID = Rampart;
                        return true;
                    }
                    if (CheckEagleEyeShot(Preset.Variant_PhysRanged_EagleEyeShot))
                    {
                        actionID = EagleEyeShot;
                        return true;
                    }
                }
                break;

            case JobRole.MeleeDPS:
                if (IsEnabled(Preset.Variant_Melee))
                {
                    if (CheckCure(Preset.Variant_Melee_Cure, Config.Variant_Melee_Cure))
                    {
                        actionID = Cure;
                        return true;
                    }
                    if (CheckUltimatum(Preset.Variant_Melee_Ultimatum))
                    {
                        actionID = Ultimatum;
                        return true;
                    }
                    if (CheckRaise(Preset.Variant_Melee_Raise))
                    {
                        actionID = Raise;
                        return true;
                    }
                    if (CheckRampart(Preset.Variant_Melee_Rampart))
                    {
                        actionID = Rampart;
                        return true;
                    }
                    if (CheckEagleEyeShot(Preset.Variant_Melee_EagleEyeShot))
                    {
                        actionID = EagleEyeShot;
                        return true;
                    }
                }
                break;

            case JobRole.MagicalDPS:
                if (IsEnabled(Preset.Variant_Magic))
                {
                    if (CheckCure(Preset.Variant_Magic_Cure, Config.Variant_Magic_Cure))
                    {
                        actionID = Cure;
                        return true;
                    }
                    if (CheckUltimatum(Preset.Variant_Magic_Ultimatum))
                    {
                        actionID = Ultimatum;
                        return true;
                    }
                    if (CheckRaise(Preset.Variant_Magic_Raise))
                    {
                        actionID = Raise;
                        return true;
                    }
                    if (CheckRampart(Preset.Variant_Magic_Rampart))
                    {
                        actionID = Rampart;
                        return true;
                    }
                    if (CheckEagleEyeShot(Preset.Variant_Magic_EagleEyeShot))
                    {
                        actionID = EagleEyeShot;
                        return true;
                    }
                }
                break;
        }

        return false;
    }

    // Quick separate check for AutoRotation
    public static bool CanRaise()
    {
        if (!IsInVariantDungeon) return false;
        return GetRoleFromJob(Player.Job) switch
        {
            JobRole.Tank => IsEnabled(Preset.Variant_Tank) && CheckRaise(Preset.Variant_Tank_Raise),
            JobRole.RangedDPS => IsEnabled(Preset.Variant_PhysRanged) && CheckRaise(Preset.Variant_PhysRanged_Raise),
            JobRole.MeleeDPS => IsEnabled(Preset.Variant_Melee) && CheckRaise(Preset.Variant_Melee_Raise),
            JobRole.MagicalDPS => IsEnabled(Preset.Variant_Magic) && CheckRaise(Preset.Variant_Magic_Raise),
            _ => false
        };
    }
}