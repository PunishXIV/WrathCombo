using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using WrathCombo.Data;
using WrathCombo.Services;

namespace WrathCombo.CustomComboNS.Functions
{
    internal abstract partial class CustomComboFunctions
    {
        /// <summary> Gets the Resource Cost of the action. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns></returns>
        public static int GetResourceCost(uint actionID) => CustomComboCache.GetResourceCost(actionID);

        /// <summary> Gets the Resource Type of the action. </summary>
        /// <param name="actionID"> Action ID to check. </param>
        /// <returns></returns>
        public static bool IsResourceTypeNormal(uint actionID) => CustomComboCache.GetResourceCost(actionID) is >= 100 or 0;

        /// <summary> Get a job gauge. </summary>
        /// <typeparam name="T"> Type of job gauge.</typeparam>
        /// <returns> The job gauge. </returns>
        public static T GetJobGauge<T>() where T : JobGaugeBase => Service.ComboCache.GetJobGauge<T>();

        public unsafe static int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;

        public unsafe static int LimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

        public unsafe static bool IsLB1Ready => LimitBreakLevel == 1;

        public unsafe static bool IsLB2Ready => LimitBreakLevel == 2;

        public unsafe static bool IsLB3Ready => LimitBreakLevel == 3;
    }
}
