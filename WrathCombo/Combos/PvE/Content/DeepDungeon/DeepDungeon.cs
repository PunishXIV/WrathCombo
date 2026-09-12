using WrathCombo.Combos.PvE.ALL;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE.Content.DeepDungeons;

internal static partial class DeepDungeons
{
    public static bool TryGetDDAction(ref uint actionID)
    {
        if (UseSustainingPotion(out var potionId))
        {
            actionID = Items.UseItem(potionId);
            return true;
        }

        if (UsePomander(out var pomanderId) && pomanderId != 0)
        {
            actionID = UsePomander(pomanderId);
            return true;
        }

        return false;
    }

    public static bool UseSustainingPotion(out uint potionId)
    {
        potionId = 0;

        if (IsEnabled(Preset.PoTD_SustainingPotion) && Items.ItemReady(PoTDSustainingPotion) && !LocalPlayer.HasStatus(Buffs.Rehabilitation) && PlayerHealthPercentageHp() <= Config.PoTD_SustainingPotion_HP)
        {
            potionId = PoTDSustainingPotion;
            return true;
        }

        if (IsEnabled(Preset.HoH_EmpyreanPotion) && Items.ItemReady(HoHEmpyreanPotion) && !LocalPlayer.HasStatus(Buffs.Rehabilitation) && PlayerHealthPercentageHp() <= Config.HoH_SustainingPotion_HP)
        {
            potionId = HoHEmpyreanPotion;
            return true;
        }

        if (IsEnabled(Preset.EO_OrthosPotion) && Items.ItemReady(OrthosPotion) && !LocalPlayer.HasStatus(Buffs.Rehabilitation) && PlayerHealthPercentageHp() <= Config.EO_SustainingPotion_HP)
        {
            potionId = OrthosPotion;
            return true;
        }

        if (IsEnabled(Preset.PT_PilgrimsPotion) && Items.ItemReady(PilgrimsPotion) && !LocalPlayer.HasStatus(Buffs.Rehabilitation) && PlayerHealthPercentageHp() <= Config.PT_SustainingPotion_HP)
        {
            potionId = PilgrimsPotion;
            return true;
        }

        return false;
    }

    public static bool UsePomander(out Pomanders pomanderId)
    {
        pomanderId = 0;
        // Fill this in with pomander features
        // Example

        //if (PomanderReady(Pomanders.PomanderOfStrength) && !LocalPlayer.HasStatus(Buffs.DamageUp))
        //{
        //    pomanderId = Pomanders.PomanderOfStrength;
        //    return true;
        //}

        return false;
    }

}

