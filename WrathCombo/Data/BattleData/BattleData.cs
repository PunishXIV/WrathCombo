using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Logging;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.Services;
using static ECommons.ExcelServices.ExcelTerritoryHelper;

namespace WrathCombo.Data.BattleData
{
    internal static partial class BattleData
    {
        #region Private Vars
        /// <summary>
        /// Current encounter-specific invincibility check. <br/>
        ///
        /// Parameters:<br/>
        /// 1. Target battle character.<br/>
        /// 2. Target BaseId.<br/>
        /// 3. Cached status IDs on the target.<br/>
        /// </summary>
        /// <returns>Enum flag for Invincibility (true/false/check statuses)</returns>
        private static Func<IBattleChara, uint, HashSet<uint>, Invincible> _invincibleCheck = (_, _, _) => Invincible.CheckStatuses;

        /// <summary>
        /// Current encounter-specific pause actions check.
        /// </summary>
        private static Func<bool> _pauseActions = () => false;

        /// <summary>
        /// Cached copy of territory ID for quick lookup <br/>
        /// Set on territory change.
        /// </summary>
        private static uint _territoryID;


        /// <summary>
        /// List of action IDs that are tankbusters
        /// </summary>
        private static FrozenSet<uint> _tankbusterAIDs = [];

        /// <summary>
        /// List of action IDs that are raidwide damage
        /// </summary>
        private static FrozenSet<uint> _raidwideAIDs = [];

        /// <summary>
        /// List of action IDs that are raidwides, but should be ignored (gazes for instance)
        /// </summary>
        private static FrozenSet<uint> _ignoreRaidwideAIDs = [];

        /// <summary>
        /// Set if any given territory has loaded battle data
        /// </summary>
        public static bool BattleDataLoaded;
        #endregion

        #region Invincible Checks
        /// <summary>
        /// Invincible result enum<br/>
        /// True: Is Invincible<br/>
        /// False: Is not Invincible<br/>
        /// CheckStatus: Is not Invincible per BattleData, but should be checked against master Invincibility status list
        /// </summary>
        public enum Invincible
        {
            False,
            True,
            CheckStatuses,
        }

        /// <summary>
        /// Quickly takes a bool and selects Invincible.True or Invincible.False<br/>
        /// Will not fall back to check basic Invincibility Statuses
        /// </summary>
        /// <param name="invincible">bool parameter / statement if invincible or not</param>
        /// <returns></returns>
        private static Invincible Result(bool invincible)
            => invincible ? Invincible.True : Invincible.False;

        public static Invincible IsInvincible(IBattleChara target, uint targetId, HashSet<uint> targetStatuses)
        {
            return _invincibleCheck(target, targetId, targetStatuses);
        }
        #endregion

        // Pause Actions
        public static bool PauseActions() => _pauseActions();

        // Tankbusters
        public static FrozenSet<uint> TankbusterAIDs => _tankbusterAIDs;

        public static bool IsTankbuster(uint actionId)
            => _tankbusterAIDs.Contains(actionId);

        public static bool IsCastingTankbuster(IGameObject? targetObject = null)
        {
            var target = targetObject as IBattleChara
                         ?? SimpleTarget.HardTarget as IBattleChara;

            return target is not null
                && target.IsCasting
                && IsTankbuster(target.CastActionId)
                && target.TargetObject == Player.Object;
        }

        // Raidwides
        public static FrozenSet<uint> RaidwideAIDs => _raidwideAIDs;
        public static bool IsRaidwide(uint actionId)
            => _raidwideAIDs.Contains(actionId);

        // Ignore Raidwides
        public static FrozenSet<uint> IgnoreRaidwideAIDs => _ignoreRaidwideAIDs;
        public static bool IgnoreRaidwide(uint actionId)
            => _ignoreRaidwideAIDs.Contains(actionId);

        // Execute on Territory Change
        public static void LoadCombatData(uint territoryID)
        {
            // Reset Combat Functions and FrozenSets
            _invincibleCheck = (_, _, _) =>
                IsSanctuary(_territoryID)
                    ? Invincible.False
                    : Invincible.CheckStatuses;
            _pauseActions = () => false;
            _tankbusterAIDs = [];
            _raidwideAIDs = [];
            _ignoreRaidwideAIDs = [];

            // Save the territory ID for later
            _territoryID = territoryID;

            if (Get(_territoryID) is TerritoryType map)
            {
                // Using TerritoryType.ExVersion listed for the map to determine the splitup, not actual regions
                // ExVersion is Expansion
                // Please verify the expansion in the TerritoryType sheet https://exd.camora.dev/sheet/TerritoryType
                // Example: Epic of Alexander is Shadowbringers (3) Content, even though the region says Dravania (1/Heavensward),
                BattleDataLoaded = map.ExVersion.RowId switch
                {
                    0 => LoadARR(),
                    1 => LoadHW(),
                    2 => LoadSB(),
                    3 => LoadShB(),
                    4 => LoadEW(),
                    5 => LoadDT(),
                    _ => false,
                    //6 => LoadEC(),
                };

#if DEBUG
                if (BattleDataLoaded)
                    DuoLog.Debug($"{map.PlaceName.Value.Name} Battle Data Loaded");
#endif
            }
        }

        /// <summary>
        /// Used primarily to look for any gaze actions since it'll use a config to determine when to pause
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="timeRemaining"></param>
        /// <returns></returns>
        private static bool CheckForGazeCasts(uint actionID)
        {
            var battleChars = Svc.Objects.Where(x => x is IBattleChara).Cast<IBattleChara>();
            return battleChars.Any(x => x.IsCasting && x.CastActionId == actionID && (x.TotalCastTime - x.CurrentCastTime) <= Service.Configuration.PenaltyPause);
        }

        /// <summary>
        /// Used primarily to look for any gaze like actions since it'll use a config to determine when to pause
        /// </summary>
        /// <param name="baseId">BaseID of the NPC to look for</param>
        /// <param name="actionID">action ID of the cast</param>
        /// <returns></returns>
        public static bool CheckForGazeCasts(uint baseId, uint actionID)
        {
            return Svc.Objects
                .OfType<IBattleChara>()
                .Any(x =>
                    x.BaseId == baseId &&
                    x.IsCasting &&
                    x.CastActionId == actionID &&
                    (x.TotalCastTime - x.CurrentCastTime) <= Service.Configuration.PenaltyPause);
        }
    }
}
