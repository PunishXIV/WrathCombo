using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Memory;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Linq;
using WrathCombo.Combos.PvE;
using WrathCombo.Data;
using GameMain = FFXIVClientStructs.FFXIV.Client.Game.GameMain;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    /// <summary> Gets the player or null. </summary>
    public static IPlayerCharacter? LocalPlayer => Player.Object;

    /// <summary> Find if the player has a certain condition. </summary>
    /// <param name="flag"> Condition flag. </param>
    /// <returns> A value indicating whether the player is in the condition. </returns>
    public static bool HasCondition(ConditionFlag flag) => Svc.Condition[flag];

    /// <summary> Find if the player is in combat. </summary>
    /// <returns> A value indicating whether the player is in combat. </returns>
    public static bool InCombat() => Svc.Condition[ConditionFlag.InCombat];

    /// <summary> Find if the player is bound by duty. </summary>
    /// <returns> A value indicating whether the player is bound by duty. </returns>
    public static unsafe bool InDuty() => GameMain.Instance()->CurrentContentFinderConditionId > 0;

    /// <summary> Find if the player has a pet present. </summary>
    /// <returns> A value indicating whether the player has a pet (fairy/carbuncle) present. </returns>
    public static bool HasPetPresent() => Svc.Buddies.PetBuddy != null;

    /// <summary> Find if the player has a companion (chocobo) present. </summary>
    /// <returns> A value indicating whether the player has a companion (chocobo). </returns>
    public static bool HasCompanionPresent() => Svc.Buddies.CompanionBuddy?.GameObject != null;

    /// <summary> Checks if the player is in a PVP enabled zone. </summary>
    /// <returns> A value indicating whether the player is in a PVP enabled zone. </returns>
    public static bool InPvP() => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();

    /// <summary> Checks if the player has completed the required job quest for the ability. </summary>
    /// <returns> A value indicating a quest has been completed for a job action.</returns>
    public static unsafe bool IsActionUnlocked(uint id)
    {
        var unlockLink = ActionWatching.ActionSheet[id].UnlockLink.RowId;
        return unlockLink == 0 || UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(unlockLink);
    }

    public static unsafe bool InFATE()
    {
        var currentFate = FateManager.Instance()->CurrentFate;
        return currentFate is not null && LocalPlayer.Level <= currentFate->MaxLevel;
    }

    public static bool PlayerHasTankStance()
    {
        return Player.Job switch
        {
            Job.GLA or Job.PLD => HasStatusEffect(PLD.Buffs.IronWill),
            Job.MRD or Job.WAR => HasStatusEffect(WAR.Buffs.Defiance),
            Job.DRK => HasStatusEffect(DRK.Buffs.Grit),
            Job.GNB => HasStatusEffect(GNB.Buffs.RoyalGuard),
            Job.BLU => HasStatusEffect(BLU.Buffs.TankMimicry),
            _ => false
        };
    }

    public static unsafe bool InBossEncounter()
    {
        if (NearbyBosses.Count() == 0)
            return false;

        foreach (var boss in NearbyBosses)
        {
            var plate = boss.GetNameplateKind();
            if (boss.Struct()->InCombat &&
                plate is NameplateKind.HostileEngagedSelfDamaged or
                    NameplateKind.HostileEngagedSelfUndamaged)
                return true;

            if ((Player.Available &&
                 Player.BattleChara->TargetId == boss.GameObjectId) ||
                ((int)(Content.ContentType ?? 0) > (int)ContentType.PVP &&
                 boss.GetNameplateKind().ToString().Contains("HostileEngaged")))
                return true;
        }

        return false;
    }

    public static unsafe AllianceGroup GetAllianceGroup()
    {
        if (GroupManager.Instance()->MainGroup.IsAlliance)
        {
            var array = UIModule.Instance()->GetRaptureAtkModule()->AtkModule.AtkArrayDataHolder.StringArrays[3]->StringArray[4];
            var str = MemoryHelper.ReadSeStringNullTerminated(new System.IntPtr(array));
            var lastChar = str.TextValue.Last();

            return lastChar switch
            {
                'A' => AllianceGroup.GroupA,
                'B' => AllianceGroup.GroupB,
                'C' => AllianceGroup.GroupC,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        return AllianceGroup.NotInAlliance;
    }
}