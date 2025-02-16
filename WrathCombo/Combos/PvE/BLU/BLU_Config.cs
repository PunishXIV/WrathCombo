#region

using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace WrathCombo.Combos.PvE;

#endregion

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

        #region DPS DoT Waste Protection

        /// <summary>
        ///     HP threshold for DPS DoT Waste Protection.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 2 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        public static readonly UserInt BLU_DPS_DoT_WasteProtection_HP =
            new("BLU_DPS_DoT_WasteProtection_HP", 2);

        /// <summary>
        ///     Time threshold for DPS DoT Waste Protection.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 3 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        public static readonly UserInt BLU_DPS_DoT_WasteProtection_Time =
            new("BLU_DPS_DoT_WasteProtection_Time", 3);

        #endregion

        #region Tank DoT Waste Protection

        /// <summary>
        ///     HP threshold for Tank DoT Waste Protection.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 2 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        public static readonly UserInt BLU_Tank_DoT_WasteProtection_HP =
            new("BLU_Tank_DoT_WasteProtection_HP", 2);

        /// <summary>
        ///     Time threshold for Tank DoT Waste Protection.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 3 <br />
        ///     <b>Range</b>: 0 - 100 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Ones" />
        /// </value>
        public static readonly UserInt BLU_Tank_DoT_WasteProtection_Time =
            new("BLU_Tank_DoT_WasteProtection_Time", 3);

        #endregion

        #region Advanced Tank

        /// <summary>
        ///     Lucid Dreaming threshold for Advanced Tank.
        /// </summary>
        /// <value>
        ///     <b>Default</b>: 9500 <br />
        ///     <b>Range</b>: 0 - 10000 <br />
        ///     <b>Step</b>: <see cref="SliderIncrements.Hundreds" />
        /// </value>
        public static readonly UserInt BLU_Tank_Advanced_Lucid =
            new("BLU_Tank_Advanced_Lucid", 9500);

        #endregion
    }
}
