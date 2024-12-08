using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class BLU
{
    internal static class Config
    {
        internal static void Draw(CustomComboPreset preset)
        {
            //switch (preset)
            //{
            //    //Presets
            //}
        }

        internal static UserInt
            BLU_DPS_DoT_WasteProtection_HP =
                new("BLU_DPS_DoT_WasteProtection_HP", 2),
            BLU_DPS_DoT_WasteProtection_Time =
                new("BLU_DPS_DoT_WasteProtection_Time", 3),
            BLU_Tank_DoT_WasteProtection_HP =
                new("BLU_Tank_DoT_WasteProtection_HP", 2),
            BLU_Tank_DoT_WasteProtection_Time =
                new("BLU_Tank_DoT_WasteProtection_Time", 3),
            BLU_Tank_Advanced_Lucid =
                new("BLU_Tank_Advanced_Lucid", 9500);
    }
}
