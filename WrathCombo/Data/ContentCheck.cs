#region

using ECommons.GameHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;

#endregion

namespace WrathCombo.Data;

public class ContentCheck
{
    /// <summary>
    ///     Valid selections for list sets to use.
    /// </summary>
    public enum ListSet
    {
        /// <seealso cref="IsInBottomHalfContent" />
        /// <seealso cref="IsInTopHalfContent" />
        /// <seealso cref="BottomHalfContent" />
        /// <seealso cref="TopHalfContent" />
        Halved,

        /// <seealso cref="IsInCasualContent" />
        /// <seealso cref="IsInHardContent" />
        /// <seealso cref="CasualContent" />
        /// <seealso cref="HardContent" />
        CasualVSHard,

        /// <seealso cref="IsInSoftCoreContent" />
        /// <seealso cref="IsInMidCoreContent" />
        /// <seealso cref="IsInHardCoreContent" />
        /// <seealso cref="SoftCoreContent" />
        /// <seealso cref="MidCoreContent" />
        /// <seealso cref="HardCoreContent" />
        Cored,

        /// <seealso cref="IsInBossOnlyContent" />
        BossOnly,
    }

    /// <summary>
    ///     Check if the current content the user was in matches the content in the
    ///     given configuration.
    /// </summary>
    /// <param name="configuredContent">
    ///     The <see cref="UserBoolArray">User Config variable</see> of selected
    ///     valid content difficulties to check against.
    /// </param>
    /// <param name="configListSet">
    ///     The <see cref="ListSet">List Set</see> that the variable is set to.
    /// </param>
    /// <returns>
    ///     Whether the current content is matches the selected difficulties.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static bool IsInConfiguredContent
        (UserBoolArray configuredContent, ListSet configListSet)
    {
        {
            switch (configListSet)
            {
                case ListSet.Halved:
                    if (configuredContent[0] && IsInBottomHalfContent())
                        return true;
                    if (configuredContent[1] && IsInTopHalfContent())
                        return true;
                    break;

                case ListSet.CasualVSHard:
                    if (configuredContent[0] && IsInCasualContent())
                        return true;
                    if (configuredContent[1] && IsInHardContent())
                        return true;
                    break;

                case ListSet.Cored:
                    if (configuredContent[0] && IsInSoftCoreContent())
                        return true;
                    if (configuredContent[1] && IsInMidCoreContent())
                        return true;
                    if (configuredContent[2] && IsInHardCoreContent())
                        return true;
                    break;

                case ListSet.BossOnly:
                    if (configuredContent[0])
                        return true;
                    if (configuredContent[1] && IsInBossOnlyContent())
                        return true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(configListSet), configListSet, null);
            }

            return false;
        }
    }

    /// <seealso cref="IsInConfiguredContent(UserBoolArray, ListSet)"/>
    internal static bool IsInConfiguredContent
        (UserInt configuredContent, ListSet configListSet)
    {
        switch (configListSet)
        {
            case ListSet.BossOnly:
                if (configuredContent == 0)
                    return true;
                if (configuredContent == 1 && IsInBossOnlyContent())
                    return true;
                break;
            default:
                throw new ArgumentOutOfRangeException
                    (nameof(configListSet), configListSet, null);
        }

        return false;
    }
    
    public static bool IsInPOTD
    {
        get
        {
            if (!EZ.Throttle("contentCheckInPOTD", TS.FromSeconds(5)))
                return field;

            // from: https://github.com/PunishXIV/PalacePal/blob/main/Pal.Common/ETerritoryType.cs
            field = Content.TerritoryID is >= 561 and <= 565 or >= 593 and <= 607;
            return field;
        }
    }

    /// <summary>
    ///     Check if the current area is PvP content.
    /// </summary>
    public static bool IsInPVPContent
    {
        get
        {
            if (!EZ.Throttle("contentCheckInPVP", TS.FromSeconds(5)))
                return field;

            field = (Content.ContentType is ContentType.OverWorld &&
                     Content.TerritoryName == "Wolves' Den Pier") ||
                    Content.ContentType is ContentType.PVP;
            return field;
        }
    }

    /// <summary>
    ///     Check if the current instanced content is Savage or Ultimate.
    /// </summary>
    public static bool IsInSavagePlusContent
    {
        get
        {
            if (!EZ.Throttle("contentCheckInSavagePlus", TS.FromSeconds(5)))
                return field;

            field = Content.ContentDifficulty is ContentDifficulty.Savage or
                ContentDifficulty.CriterionSavage or
                ContentDifficulty.Ultimate;
            return field;
        }
    }

    /// <summary>
    ///     Check if the current instance is a Field Operation.<br />
    ///     (Bozja, Eureka, Occult Crescent, etc.)
    /// </summary>
    /// South Horn, Southern Front, etc.
    public static bool IsInFieldOperations
    {
        get
        {
            if (!EZ.Throttle("contentCheckInFieldOperations", TS.FromSeconds(5)))
                return field;

            field = Content.ContentType is ContentType.FieldOperations;
            return field;
        }
    }

    /// <summary>
    ///     Check if the current instance is a Field Raid.<br />
    ///     (Delubrum Reginae, etc.)
    /// </summary>
    public static bool IsInFieldRaids
    {
        get
        {
            if (!EZ.Throttle("contentCheckInFieldOperations", TS.FromSeconds(5)))
                return field;

            field = Content.ContentType is ContentType.FieldRaid;
            return field;
        }
    }

    #region Halved Content Lists

#pragma warning disable CS1574

    /// <summary>
    ///     Whether the current content is in the bottom half of the list of content
    ///     difficulties.
    /// </summary>
    /// <returns>
    ///     If the
    ///     <see cref="Content.DetermineContentDifficulty">
    ///         determined difficulty
    ///     </see>
    ///     is in the list of <see cref="BottomHalfContent" />.
    /// </returns>
    /// <remarks>
    ///     The rest of the list set would be checked by
    ///     <see cref="IsInTopHalfContent" />.
    /// </remarks>
    /// <seealso cref="BottomHalfContent" />
    public static bool IsInBottomHalfContent() =>
        BottomHalfContent.Contains(
            Content.ContentDifficulty ?? ContentDifficulty.Unknown
        );

    /// <summary>
    ///     Whether the current content is in the top half of the list of content
    ///     difficulties.
    /// </summary>
    /// <returns>
    ///     If the
    ///     <see cref="Content.DetermineContentDifficulty">
    ///         determined difficulty
    ///     </see>
    ///     is in the list of <see cref="TopHalfContent" />.
    /// </returns>
    /// <remarks>
    ///     The rest of the list set would be checked by
    ///     <see cref="IsInBottomHalfContent" />.
    /// </remarks>
    /// <seealso cref="TopHalfContent" />
    public static bool IsInTopHalfContent() =>
        TopHalfContent.Contains(
            Content.ContentDifficulty ?? ContentDifficulty.Unknown
        );

    /// <summary>
    ///     Roughly the bottom half of the list of content difficulties.
    /// </summary>
    /// <remarks>
    ///     The rest of the list set would be <see cref="TopHalfContent" />.
    /// </remarks>
    private static readonly List<ContentDifficulty> BottomHalfContent =
    [
        ContentDifficulty.Normal,
        ContentDifficulty.Hard,
        ContentDifficulty.Unreal,
        ContentDifficulty.FieldRaidsSavage,
    ];

    /// <summary>
    ///     A string representation of the bottom half of the list of content
    ///     difficulties.
    /// </summary>
    /// <seealso cref="BottomHalfContent" />
    public static readonly string BottomHalfContentList =
        string.Join(", ", BottomHalfContent.Select(x => x.ToString()));

    /// <summary>
    ///     Roughly the top half of the list of content difficulties.
    /// </summary>
    /// <remarks>
    ///     The rest of the list set would be <see cref="BottomHalfContent" />.
    /// </remarks>
    private static readonly List<ContentDifficulty> TopHalfContent =
    [
        ContentDifficulty.Chaotic,
        ContentDifficulty.Criterion,
        ContentDifficulty.Extreme,
        ContentDifficulty.Savage,
        ContentDifficulty.CriterionSavage,
        ContentDifficulty.Ultimate
    ];

    /// <summary>
    ///     A string representation of the top half of the list of content
    ///     difficulties.
    /// </summary>
    /// <seealso cref="TopHalfContent" />
    public static readonly string TopHalfContentList =
        string.Join(", ", TopHalfContent.Select(x => x.ToString()));

    #endregion

    #region Casual vs Hard Content Lists

    /// <summary>
    ///     Whether the current content is considered casual.
    /// </summary>
    /// <returns>
    ///     If the
    ///     <see cref="Content.DetermineContentDifficulty">
    ///         determined difficulty
    ///     </see>
    ///     is in the list of <see cref="CasualContent" />.
    /// </returns>
    /// <remarks>
    ///     The rest of the list set would be checked by
    ///     <see cref="IsInHardContent" />.
    /// </remarks>
    /// <seealso cref="CasualContent" />
    public static bool IsInCasualContent() =>
        CasualContent.Contains(
            Content.ContentDifficulty ?? ContentDifficulty.Unknown
        );

    /// <summary>
    ///     Whether the current content is considered hard.
    /// </summary>
    /// <returns>
    ///     If the
    ///     <see cref="Content.DetermineContentDifficulty">
    ///         determined difficulty
    ///     </see>
    ///     is in the list of <see cref="HardContent" />.
    /// </returns>
    /// <remarks>
    ///     The rest of the list set would be checked by
    ///     <see cref="IsInCasualContent" />.
    /// </remarks>
    /// <seealso cref="HardContent" />
    public static bool IsInHardContent() =>
        HardContent.Contains(
            Content.ContentDifficulty ?? ContentDifficulty.Unknown
        );

    /// <summary>
    ///     A list of content difficulties that are considered casual.
    /// </summary>
    /// <remarks>
    ///     The rest of the list set would be <see cref="HardContent" />.
    /// </remarks>
    private static readonly List<ContentDifficulty> CasualContent =
    [
        ContentDifficulty.Normal,
        ContentDifficulty.Hard,
    ];

    /// <summary>
    ///     A string representation of the list of casual content difficulties.
    /// </summary>
    /// <seealso cref="CasualContent" />
    public static readonly string CasualContentList =
        string.Join(", ", CasualContent.Select(x => x.ToString()));

    /// <summary>
    ///     A list of content difficulties that are considered not casual.
    /// </summary>
    /// <remarks>
    ///     The rest of the list set would be <see cref="CasualContent" />.
    /// </remarks>
    private static readonly List<ContentDifficulty> HardContent =
    [
        ContentDifficulty.Unreal,
        ContentDifficulty.FieldRaidsSavage,
        ContentDifficulty.Chaotic,
        ContentDifficulty.Criterion,
        ContentDifficulty.Extreme,
        ContentDifficulty.Savage,
        ContentDifficulty.CriterionSavage,
        ContentDifficulty.Ultimate
    ];

    /// <summary>
    ///     A string representation of the list of hard content difficulties.
    /// </summary>
    /// <seealso cref="HardContent" />
    public static readonly string HardContentList =
        string.Join(", ", HardContent.Select(x => x.ToString()));

    #endregion

    #region Cored Content Lists

    /// <summary>
    ///     Whether the current content is considered soft-core.
    /// </summary>
    /// <returns>
    ///     If the
    ///     <see cref="Content.DetermineContentDifficulty">
    ///         determined difficulty
    ///     </see>
    ///     is in the list of <see cref="SoftCoreContent" />.
    /// </returns>
    /// <remarks>
    ///     The rest of the list set would be <see cref="IsInMidCoreContent" /> and
    ///     <see cref="IsInHardCoreContent" />.
    /// </remarks>
    /// <seealso cref="SoftCoreContent" />
    public static bool IsInSoftCoreContent() =>
        SoftCoreContent.Contains(
            Content.ContentDifficulty ?? ContentDifficulty.Unknown
        );

    /// <summary>
    ///     Whether the current content is considered mid-core.
    /// </summary>
    /// <returns>
    ///     If the
    ///     <see cref="Content.DetermineContentDifficulty">
    ///         determined difficulty
    ///     </see>
    ///     is in the list of <see cref="MidCoreContent" />.
    /// </returns>
    /// <remarks>
    ///     The rest of the list set would be <see cref="IsInSoftCoreContent" /> and
    ///     <see cref="IsInHardCoreContent" />.
    /// </remarks>
    /// <seealso cref="MidCoreContent" />
    public static bool IsInMidCoreContent() =>
        MidCoreContent.Contains(
            Content.ContentDifficulty ?? ContentDifficulty.Unknown
        );

    /// <summary>
    ///     Whether the current content is considered hard-core.
    /// </summary>
    /// <returns>
    ///     If the
    ///     <see cref="Content.DetermineContentDifficulty">
    ///         determined difficulty
    ///     </see>
    ///     is in the list of <see cref="HardCoreContent" />.
    /// </returns>
    /// <remarks>
    ///     The rest of the list set would be <see cref="IsInSoftCoreContent" /> and
    ///     <see cref="IsInMidCoreContent" />.
    /// </remarks>
    /// <seealso cref="HardCoreContent" />
    public static bool IsInHardCoreContent() =>
        HardCoreContent.Contains(
            Content.ContentDifficulty ?? ContentDifficulty.Unknown
        );

#pragma warning restore CS1574

    /// <summary>
    ///     A list of content difficulties that are considered soft-core.
    /// </summary>
    /// <remarks>
    ///     The rest of the list set would be <see cref="MidCoreContent" /> and
    ///     <see cref="HardCoreContent" />.
    /// </remarks>
    private static readonly List<ContentDifficulty> SoftCoreContent =
    [
        ContentDifficulty.Normal,
        ContentDifficulty.Hard,
    ];

    /// <summary>
    ///     A string representation of the list of soft-core content difficulties.
    /// </summary>
    /// <seealso cref="SoftCoreContent" />
    public static readonly string SoftCoreContentList =
        string.Join(", ", SoftCoreContent.Select(x => x.ToString()));

    /// <summary>
    ///     A list of content difficulties that are considered mid-core.
    /// </summary>
    /// <remarks>
    ///     The rest of the list set would be <see cref="SoftCoreContent" /> and
    ///     <see cref="HardCoreContent" />.
    /// </remarks>
    private static readonly List<ContentDifficulty> MidCoreContent =
    [
        ContentDifficulty.Unreal,
        ContentDifficulty.FieldRaidsSavage,
        ContentDifficulty.Extreme,
        ContentDifficulty.Chaotic,
    ];

    /// <summary>
    ///     A string representation of the list of mid-core content difficulties.
    /// </summary>
    /// <seealso cref="MidCoreContent" />
    public static readonly string MidCoreContentList =
        string.Join(", ", MidCoreContent.Select(x => x.ToString()));

    /// <summary>
    ///     A list of content difficulties that are considered hard-core.
    /// </summary>
    /// <remarks>
    ///     The rest of the list set would be <see cref="SoftCoreContent" /> and
    ///     <see cref="MidCoreContent" />.
    /// </remarks>
    private static List<ContentDifficulty> HardCoreContent =
    [
        ContentDifficulty.Criterion,
        ContentDifficulty.Savage,
        ContentDifficulty.CriterionSavage,
        ContentDifficulty.Ultimate,
    ];

    /// <summary>
    ///     A string representation of the list of hard-core content difficulties.
    /// </summary>
    /// <seealso cref="HardCoreContent" />
    public static readonly string HardCoreContentList =
        string.Join(", ", HardCoreContent.Select(x => x.ToString()));

    #endregion

    #region Boss Only Lists

    /// <summary>
    ///     Whether the current content only contains bosses
    /// </summary>
    /// <returns>
    ///     If the
    ///     <see cref="Content.ContentFinderConditionRow">
    ///         CFC
    ///     </see>
    ///     is in a static list of boss only content; as in, trash-less bosses.
    /// </returns>
    /// <remarks>
    ///     In this case, the rest of this list would just be <c>true</c>, as the
    ///     alternative is <c>All Content</c>.
    /// </remarks>
    /// <seealso cref="UserConfig.DrawBossOnlyChoice(UserBoolArray, string)"/>
    public static bool IsInBossOnlyContent()
    {
        if (Content.ContentType is ContentType.Trial)
            return true;

        if (Content.ContentType is ContentType.Raid)
        {
            if (Content.ContentFinderConditionRow.HasValue && Content.ContentFinderConditionRow.Value.RowId is 93 or 94 or 95 or 96 or 97 or 98 or 99 or 100 or 103 or 104 or 105 or 107 or 108 or 113 or 114 or 137 or 138 or 186 or 187 or 188)
                return false;

            return true;
        }

        if (Content.ContentFinderConditionRow?.RowId is 1010) //Chaotic Alliance, might end up being refactored in the future
            return true;

        return false;
    }

    #endregion
}
