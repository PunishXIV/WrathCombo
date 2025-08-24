using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE
{
    internal static partial class Variant
    {
        internal partial class Config
        {
            public static readonly UserInt
                Variant_Tank_Cure = new("Variant_Tank_Cure", 50),
                Variant_PhysRanged_Cure = new("Variant_PhysRanged_Cure", 50),
                Variant_Melee_Cure = new("Variant_Melee_Cure", 50),
                Variant_Magic_Cure = new("Variant_Magic_Cure", 50);

            internal static void Draw(Preset preset)
            {
                switch (preset)
                {
                    case Preset.Variant_Tank_Cure:
                        DrawSliderInt(1, 80, Variant_Tank_Cure,
                            "HP% to be at or under",
                            itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);
                        break;
                    case Preset.Variant_PhysRanged_Cure:
                        DrawSliderInt(1, 80, Variant_PhysRanged_Cure,
                            "HP% to be at or under",
                            itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);
                        break;
                    case Preset.Variant_Melee_Cure:
                        DrawSliderInt(1, 80, Variant_Melee_Cure,
                            "HP% to be at or under",
                            itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);
                        break;
                    case Preset.Variant_Magic_Cure:
                        DrawSliderInt(1, 80, Variant_Magic_Cure,
                            "HP% to be at or under",
                            itemWidth: 200f, sliderIncrement: SliderIncrements.Fives);
                        break;
                }
            }
        }
    }
}
