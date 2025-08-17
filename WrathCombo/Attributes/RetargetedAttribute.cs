#region

using ECommons.Logging;
using System;
using WrathCombo.Combos;
using WrathCombo.Combos.PvE;
using WrathCombo.Core;
using WrathCombo.Services;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

// ReSharper disable ClassNeverInstantiated.Global

#endregion

namespace WrathCombo.Attributes;

/// <summary>
///     Flags a Preset as one that is Retargeted with
///     <see cref="ActionRetargeting" />.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class RetargetedAttribute : RetargetedAttributeBase
{
    /// <summary>
    ///     Designates a Preset as one that is Retargeted.
    /// </summary>
    /// <param name="retargetedActions">
    ///     List of all actions that are Retargeted by this Preset.
    /// </param>
    internal RetargetedAttribute(params uint[] retargetedActions)
    {
        RetargetedActions = retargetedActions;
    }
}

/// <summary>
///     Flags a Preset as one that may be Retargeted with
///     <see cref="ActionRetargeting" />.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class PossiblyRetargetedAttribute : RetargetedAttributeBase
{
    private const string DefaultSettingInfo =
        "Settings Tab > Retarget (Single Target) Healing Actions";
    private const Condition DefaultCondition =
        Condition.RetargetHealingActionsEnabled;
    
    /// <summary>
    ///     Designates a Preset as one that may be Retargeted.
    /// </summary>
    /// <param name="settingInfo">
    ///     The setting that controls whether this Preset is Retargeted or not.
    /// </param>
    /// <param name="condition">
    ///     The <see cref="Condition" /> that needs to be satisfied to turn from
    ///     yellow to green.
    /// </param>
    /// <param name="retargetedActions">
    ///     List of all actions that are Retargeted by this Preset.
    /// </param>
    /// <remarks>
    ///     Defaults to the <paramref name="settingInfo" /> and
    ///     <paramref name="condition" /> for Single Target Healing Actions.
    /// </remarks>
    internal PossiblyRetargetedAttribute
    (string settingInfo = DefaultSettingInfo,
        Condition condition = DefaultCondition,
        params uint[] retargetedActions)
    {
        SettingInfo = settingInfo;
        PossibleCondition = condition;
        RetargetedActions = retargetedActions;
    }
    
    /// <seealso cref="PossiblyRetargetedAttribute(string, Condition, uint[])"/>
    internal PossiblyRetargetedAttribute(params uint[] retargetedActions)
        : this(DefaultSettingInfo, DefaultCondition, retargetedActions)
    {
    }

    public string SettingInfo { get; }
    public Condition PossibleCondition { get; }

    /// <summary>
    ///     These conditions should be implemented in the switch in
    ///     <see cref="IsConditionSatisfied" />.
    /// </summary>
    internal enum Condition
    {
        RetargetHealingActionsEnabled,
        ASTQuickTargetCardsFeatureEnabled,
        ASTRetargetingFeaturesEnabledForBenefic,
        ASTRetargetingFeaturesEnabledForSTMit,
        WHMRetargetingFeaturesEnabledForSTMit,
        WHMRetargetingFeaturesEnabledForAoEMit,
        WHMRetargetingFeaturesEnabledForSolace,
        WHMRetargetingFeaturesEnabledForCure,
        SGERetargetingFeaturesEnabledForKardia,
        SGERetargetingFeaturesEnabledForEDiagnosis,
        SGERetargetingFeaturesEnabledForTauroDruo,
        SGERetargetingFeaturesEnabledForSTMit,
        SCHRetargetingFeaturesEnabledForLustcog,
        SCHRetargetingFeaturesEnabledForAdlocog,
        SCHRetargetingFeaturesEnabledForAdloDeployment,
        SCHRetargetingFeaturesEnabledForSTMit,
        SCHRetargetingFeaturesEnabledForAoEMit
    }
    
    /// <summary>
    ///     Checks if a given <see cref="Condition" />
    ///     (for a <see cref="PossiblyRetargetedAttribute" />)
    ///     is satisfied.
    /// </summary>
    /// <param name="condition">
    ///     The <see cref="Condition" /> to check.
    /// </param>
    /// <returns>
    ///     Whether the condition is satisfied or not,
    ///     or <see langword="null" /> if the condition is unsupported.
    /// </returns>
    public static bool? IsConditionSatisfied(Condition condition)
    {
        switch (condition)
        {
            case Condition.RetargetHealingActionsEnabled:
                return Service.Configuration.RetargetHealingActionsToStack;
            case Condition.ASTQuickTargetCardsFeatureEnabled:
                return IsEnabled(Preset.AST_Cards_QuickTargetCards);
            case Condition.ASTRetargetingFeaturesEnabledForBenefic:
                return IsEnabled(Preset.AST_Retargets_Benefic);
            case Condition.ASTRetargetingFeaturesEnabledForSTMit:
                return IsEnabled(Preset.AST_Retargets) &&
                       IsEnabled(Preset.AST_Retargets_Exaltation) &&
                       (AST.Config.AST_Mit_ST_Options[0] != true ||
                        IsEnabled(Preset.AST_Retargets_CelestialIntersection)) &&
                       (AST.Config.AST_Mit_ST_Options[1] != true ||
                        IsEnabled(Preset.AST_Retargets_EssentialDignity));
            case Condition.WHMRetargetingFeaturesEnabledForSTMit:
                return IsEnabled(Preset.WHM_Retargets) &&
                       IsEnabled(Preset.WHM_Re_Aquaveil) &&
                       (WHM.Config.WHM_AquaveilOptions[0] != true ||
                        IsEnabled(Preset.WHM_Re_DivineBenison)) &&
                       (WHM.Config.WHM_AquaveilOptions[1] != true ||
                        IsEnabled(Preset.WHM_Re_Tetragrammaton));
            case Condition.WHMRetargetingFeaturesEnabledForAoEMit:
                return IsEnabled(Preset.WHM_Retargets) &&
                       IsEnabled(Preset.WHM_Re_Asylum);
            case Condition.WHMRetargetingFeaturesEnabledForSolace:
                return IsEnabled(Preset.WHM_Retargets) &&
                       IsEnabled(Preset.WHM_Re_Solace);
            case Condition.WHMRetargetingFeaturesEnabledForCure:
                return IsEnabled(Preset.WHM_Retargets) &&
                       IsEnabled(Preset.WHM_Re_Cure);
            case Condition.SGERetargetingFeaturesEnabledForKardia:
                return IsEnabled(Preset.SGE_Retarget) &&
                       IsEnabled(Preset.SGE_Retarget_Kardia);
            case Condition.SGERetargetingFeaturesEnabledForEDiagnosis:
                return IsEnabled(Preset.SGE_Retarget) &&
                       SGE.Config.SGE_Eukrasia_Mode == 1 &&
                       IsEnabled(Preset.SGE_Retarget_EukrasianDiagnosis);
            case Condition.SGERetargetingFeaturesEnabledForTauroDruo:
                return IsEnabled(Preset.SGE_Retarget) &&
                       IsEnabled(Preset.SGE_Retarget_Druochole) &&
                       IsEnabled(Preset.SGE_Retarget_Taurochole);
            case Condition.SGERetargetingFeaturesEnabledForSTMit:
                return IsEnabled(Preset.SGE_Retarget) &&
                       IsEnabled(Preset.SGE_Retarget_Krasis) &&
                       IsEnabled(Preset.SGE_Retarget_EukrasianDiagnosis) &&
                       (SGE.Config.SGE_Mit_ST_Options[1] != true ||
                       IsEnabled(Preset.SGE_Retarget_Taurochole)) &&
                       (SGE.Config.SGE_Mit_ST_Options[0] != true ||
                       IsEnabled(Preset.SGE_Retarget_Haima));
            case Condition.SCHRetargetingFeaturesEnabledForLustcog:
                return IsEnabled(Preset.SCH_Retarget) &&
                       IsEnabled(Preset.SCH_Retarget_Excogitation) &&
                       IsEnabled(Preset.SCH_Retarget_Lustrate);
            case Condition.SCHRetargetingFeaturesEnabledForAdlocog:
                return IsEnabled(Preset.SCH_Retarget) &&
                       (SCH.Config.SCH_Recitation_Mode == 3 &&
                       IsEnabled(Preset.SCH_Retarget_Excogitation) ||
                       SCH.Config.SCH_Recitation_Mode == 0 &&
                       IsEnabled(Preset.SCH_Retarget_Adloquium));
            case Condition.SCHRetargetingFeaturesEnabledForAdloDeployment:
                return IsEnabled(Preset.SCH_Retarget) &&
                       IsEnabled(Preset.SCH_Retarget_Adloquium) &&
                       IsEnabled(Preset.SCH_Retarget_DeploymentTactics);
            case Condition.SCHRetargetingFeaturesEnabledForSTMit:
                return IsEnabled(Preset.SCH_Retarget) &&
                       IsEnabled(Preset.SCH_Retarget_Adloquium) &&
                       IsEnabled(Preset.SCH_Retarget_Protraction) &&
                       IsEnabled(Preset.SCH_Retarget_DeploymentTactics) &&
                       IsEnabled(Preset.SCH_Retarget_Excogitation);
            case Condition.SCHRetargetingFeaturesEnabledForAoEMit:
                return IsEnabled(Preset.SCH_Retarget) &&
                       IsEnabled(Preset.SCH_Retarget_SacredSoil);
            
            default:
                PluginLog.Error($"Unknown PossiblyRetargeted Condition: {condition}");
                return null;
        }
    }
}

/// <summary>
///     Just a common base class for all Retargeted Attributes.
/// </summary>
internal abstract class RetargetedAttributeBase : Attribute
{
    /// <summary>
    ///     The actions that are Retargeted by this Preset.
    /// </summary>
    public uint[] RetargetedActions { get; internal set; } = [];
}