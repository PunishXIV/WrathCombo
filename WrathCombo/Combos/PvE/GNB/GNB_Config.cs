using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class GNB
{
    internal static class Config
    {
        public const string
            GNB_VariantCure = "GNB_VariantCure";

        public static UserInt
            GNB_ST_NoMercyStop = new("GNB_ST_NoMercyStop"),
            GNB_AoE_NoMercyStop = new("GNB_AoE_NoMercyStop");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.GNB_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, GNB_VariantCure, "HP% to be at or under", 200);

                    break;

                case CustomComboPreset.GNB_ST_NoMercy:
                    UserConfig.DrawSliderInt(0, 25, GNB_ST_NoMercyStop,
                        "Stop Usage if Target HP% is below set value.\nTo Disable this option, set to 0.");

                    break;

                case CustomComboPreset.GNB_AoE_NoMercy:
                    UserConfig.DrawSliderInt(0, 25, GNB_AoE_NoMercyStop,
                        "Stop Usage if Target HP% is below set value.\nTo Disable this option, set to 0.");

                    break;

                case CustomComboPreset.GNB_ST_HOC:
                    UserConfig.DrawDifficultyMultiChoice(
                        GNB_ST_HOCDifficulty,
                        GNB_ST_HOCDifficultyListSet,
                        "Select what difficulty this should be used in:"
                    );
                    UserConfig.DrawSliderInt(5, 40, GNB_ST_HOCThreshold,
                        startUsingAtDescription,
                        itemWidth: medium, sliderIncrement: SliderIncrements.Fives);

                    UserConfig.DrawHorizontalRadioButton(
                        GNB_ST_HOCBossRestriction, "All Enemies",
                        "Will use Heart of Corundum regardless of the type of enemy.",
                        outputValue: (int)BossAvoidance.Off, itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        GNB_ST_HOCBossRestriction, "Avoid Bosses",
                        "Will try not to use Heart of Corundum when your target is a boss.\n" +
                        "(NOTE: don't rely on this 100%, square sometimes marks enemies inconsistently)",
                        outputValue: (int)BossAvoidance.On, itemWidth: 125f);

                    break;

                case CustomComboPreset.GNB_ST_GreatNebula:
                    UserConfig.DrawDifficultyMultiChoice(
                        GNB_ST_GreatNebulaDifficulty,
                        GNB_ST_GreatNebulaDifficultyListSet,
                        "Select what difficulty this should be used in:"
                    );
                    UserConfig.DrawSliderInt(5, 55, GNB_ST_GreatNebulaThreshold,
                    startUsingAtDescription,
                        itemWidth: bigger, sliderIncrement: SliderIncrements.Fives);

                    break;

                case CustomComboPreset.GNB_ST_Superbolide:
                    UserConfig.DrawDifficultyMultiChoice(
                        GNB_ST_SuperbolideDifficulty,
                        GNB_ST_SuperbolideDifficultyListSet,
                        "Select what difficulty this should be used in:"
                    );
                    UserConfig.DrawSliderInt(5, 40, GNB_ST_SuperbolideSelfThreshold,
                        startUsingAtDescription,
                        itemWidth: medium, sliderIncrement: SliderIncrements.Fives);
                    UserConfig.DrawSliderInt(0, 10, GNB_ST_SuperbolideTargetThreshold,
                    stopUsingAtDescription,
                        itemWidth: little, sliderIncrement: SliderIncrements.Ones);

                    UserConfig.DrawHorizontalRadioButton(
                        GNB_ST_SuperbolideBossRestriction, "All Enemies",
                        "Will use Superbolide regardless of the type of enemy.",
                        outputValue: (int)BossAvoidance.Off, itemWidth: 125f);
                    UserConfig.DrawHorizontalRadioButton(
                        GNB_ST_SuperbolideBossRestriction, "Avoid Bosses",
                        "Will try not to use Superbolide when your target is a boss.\n" +
                        "(NOTE: don't rely on this 100%, SE sometimes marks enemies inconsistently)",
                        outputValue: (int)BossAvoidance.On, itemWidth: 125f);

                    break;

                case CustomComboPreset.GNB_AoE_HOC:
                    UserConfig.DrawSliderInt(5, 50, GNB_AoE_HOCThreshold,
                        startUsingAtDescription,
                        itemWidth: bigger, sliderIncrement: SliderIncrements.Fives);
                    break;

                case CustomComboPreset.GNB_AoE_GreatNebula:
                    UserConfig.DrawSliderInt(5, 55, GNB_AoE_GreatNebulaThreshold,
                        startUsingAtDescription,
                        itemWidth: bigger, sliderIncrement: SliderIncrements.Fives);

                    break;

                case CustomComboPreset.GNB_AoE_Superbolide:
                    UserConfig.DrawSliderInt(5, 30, GNB_AoE_SuperbolideSelfThreshold,
                        startUsingAtDescription,
                        itemWidth: medium, sliderIncrement: SliderIncrements.Fives);
                    UserConfig.DrawSliderInt(0, 40,
                        GNB_AoE_SuperbolideTargetThreshold,
                        stopUsingAtDescription,
                        itemWidth: little, sliderIncrement: SliderIncrements.Tens);

                    break;
            }


        }

        /// Smallest bar width
        private const float little = 100f;

        /// 2nd smallest bar width
        private const float medium = 150f;

        /// 2nd biggest bar width
        private const float bigger = 175f;

        /// Biggest bar width
        private const float biggest = 200f;

        /// Bar Description for HP% to stop using
        private const string stopUsingAtDescription =
            "Target HP% to stop using (0 = Use Always)";

        /// Bar Description for HP% to start using
        private const string startUsingAtDescription =
            "HP% to use at or below";

        /// <summary>
        ///     Whether abilities should be restricted to Bosses or not.
        /// </summary>
        /// <seealso cref="Config.GNB_ST_HOCBossRestriction" />
        /// <seealso cref="Config.GNB_ST_SuperbolideBossRestriction" />
        internal enum BossAvoidance
        {
            Off = 1,
            On = 2
        }

        #region Advanced Single Target

        /// <summary>
        ///     Self HP% to use HOC below for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 25 <br />
        ///     <b>Range</b>: 5 - 40 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_ST_HOC" />
        public static readonly UserInt GNB_ST_HOCThreshold =
            new("GNB_ST_HOCThreshold", 25);

        /// <summary>
        ///     Difficulty of HOC Threshold for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.BottomHalfContent" /> <br />
        ///     <b>Options</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and/or <see cref="ContentCheck.TopHalfContent" />
        /// </value>
        /// <seealso cref="GNB_ST_HOCThreshold" />
        public static readonly UserBoolArray GNB_ST_HOCDifficulty =
            new("GNB_ST_HOCDifficulty", [true, false]);

        /// <summary>
        ///     What Difficulty List Set
        ///     <see cref="GNB_ST_HOCDifficulty" /> is set to.
        /// </summary>
        /// <seealso cref="GNB_ST_HOCDifficulty" />
        public static readonly ContentCheck.ListSet
            GNB_ST_HOCDifficultyListSet =
                ContentCheck.ListSet.Halved;

        /// <summary>
        ///     HOC Boss Restriction for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="BossAvoidance.Off" /> <br />
        ///     <b>Options</b>: <see cref="BossAvoidance">BossAvoidance Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_ST_HOC" />
        public static readonly UserInt GNB_ST_HOCBossRestriction =
            new("GNB_ST_HOCBossRestriction", (int)BossAvoidance.Off);

        /// <summary>
        ///     Self HP% to use Great Nebula below for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 40 <br />
        ///     <b>Range</b>: 5 - 55 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_ST_GreatNebula" />
        public static readonly UserInt GNB_ST_GreatNebulaThreshold =
            new("GNB_ST_GreatNebulaThreshold", 40);

        /// <summary>
        ///     Difficulty of Great Nebula Threshold for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.BottomHalfContent" /> <br />
        ///     <b>Options</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and/or <see cref="ContentCheck.TopHalfContent" />
        /// </value>
        /// <seealso cref="GNB_ST_GreatNebulaThreshold" />
        public static readonly UserBoolArray
            GNB_ST_GreatNebulaDifficulty =
                new("GNB_ST_GreatNebulaDifficulty", [true, false]);

        /// <summary>
        ///     What Difficulty List Set
        ///     <see cref="GNB_ST_GreatNebulaDifficulty" /> is set to.
        /// </summary>
        /// <seealso cref="GNB_ST_GreatNebulaDifficulty" />
        public static readonly ContentCheck.ListSet
            GNB_ST_GreatNebulaDifficultyListSet =
                ContentCheck.ListSet.Halved;

        /// <summary>
        ///     Self HP% to use Superbolide below for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 14 <br />
        ///     <b>Range</b>: 5 - 40 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_ST_Superbolide" />
        public static readonly UserInt GNB_ST_SuperbolideSelfThreshold =
            new("GNB_ST_SuperbolideSelfThreshold", 15);

        /// <summary>
        ///     Target HP% to use Superbolide above for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 1 <br />
        ///     <b>Range</b>: 0 - 10 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_ST_Superbolide" />
        public static readonly UserInt GNB_ST_SuperbolideTargetThreshold =
            new("GNB_ST_SuperbolideTargetThreshold", 1);

        /// <summary>
        ///     Difficulty of Superbolide Threshold for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="ContentCheck.BottomHalfContent" /> <br />
        ///     <b>Options</b>: <see cref="ContentCheck.BottomHalfContent" />
        ///     and/or <see cref="ContentCheck.TopHalfContent" />
        /// </value>
        /// <seealso cref="GNB_ST_SuperbolideSelfThreshold" />
        public static readonly UserBoolArray GNB_ST_SuperbolideDifficulty =
            new("GNB_ST_SuperbolideDifficulty", [true, false]);

        /// <summary>
        ///     What Difficulty List Set
        ///     <see cref="GNB_ST_SuperbolideDifficulty" /> is set to.
        /// </summary>
        /// <seealso cref="GNB_ST_SuperbolideDifficulty" />
        public static readonly ContentCheck.ListSet
            GNB_ST_SuperbolideDifficultyListSet =
                ContentCheck.ListSet.Halved;

        /// <summary>
        ///     Superbolide Boss Restriction for Single Target.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: <see cref="BossAvoidance.On" /> <br />
        ///     <b>Options</b>: <see cref="BossAvoidance">BossAvoidance Enum</see>
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_ST_Superbolide" />
        public static readonly UserInt GNB_ST_SuperbolideBossRestriction =
            new("GNB_ST_SuperbolideBossRestriction", (int)BossAvoidance.On);
        #endregion

        #region Advanced Mode - AoE

        /// <summary>
        ///     Self HP% to use HOC below for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 55 <br />
        ///     <b>Range</b>: 5 - 55 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_AoE_HOC" />
        public static readonly UserInt GNB_AoE_HOCThreshold =
            new("GNB_AoE_HOCThreshold", 55);

        /// <summary>
        ///     Self HP% to use Great Nebula below for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 50 <br />
        ///     <b>Range</b>: 5 - 55 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_AoE_GreatNebula" />
        public static readonly UserInt GNB_AoE_GreatNebulaThreshold =
            new("GNB_AoE_GreatNebulaThreshold", 50);

        /// <summary>
        ///     Self HP% to use Superbolide below for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 20 <br />
        ///     <b>Range</b>: 5 - 30 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Fives" />
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_AoE_Superbolide" />
        public static readonly UserInt GNB_AoE_SuperbolideSelfThreshold =
            new("GNB_AoE_SuperbolideSelfThreshold", 20);

        /// <summary>
        ///     Target HP% to use Superbolide above for AoE.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 15 <br />
        ///     <b>Range</b>: 0 - 40 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Tens" />
        /// </value>
        /// <seealso cref="CustomComboPreset.GNB_AoE_Superbolide" />
        public static readonly UserInt GNB_AoE_SuperbolideTargetThreshold =
            new("GNB_AoE_SuperbolideTargetThreshold", 15);

        #endregion
    }
}
