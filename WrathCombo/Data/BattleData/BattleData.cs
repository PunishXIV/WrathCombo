using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using static ECommons.ExcelServices.ExcelTerritoryHelper;

namespace WrathCombo.Data.BattleData
{
    internal static partial class BattleData
    {
        #region Private Vars
        /// <summary>
        /// Current encounter-specific invincibility check.
        ///
        /// Parameters:
        /// 1. Target battle character.
        /// 2. Target BaseId.
        /// 3. Cached status IDs on the target.
        /// </summary>
        private static Func<IBattleChara, uint, HashSet<uint>, InvincibleResult> _invincibleCheck = (_, _, _) => new InvincibleResult(false, false);
        private static Func<bool> _pauseActions = () => false;
        private static uint _territoryID;
        private static FrozenSet<uint> _tankbusterAIDs = [];
        private static FrozenSet<uint> _raidwideAIDs = [];
        private static FrozenSet<uint> _ignoreRaidwideAIDs = [];
        #endregion

        // Invincible
        public readonly record struct InvincibleResult(
            bool Invincible,
            bool CheckGenerics
        );

        public static InvincibleResult IsInvincible(IBattleChara target, uint targetId, HashSet<uint> targetStatuses)
        {
            return _invincibleCheck(target, targetId, targetStatuses);
        }

        // Pause Actions
        public static bool PauseActions() => _pauseActions();

        // Tankbusters
        public static FrozenSet<uint> TankbusterAIDs => _tankbusterAIDs;

        public static bool IsTankbuster(uint actionId)
            => _tankbusterAIDs.Contains(actionId);

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
            _invincibleCheck = (_, _, _) => new InvincibleResult(false, !IsSanctuary(_territoryID)); //Don't bother in Sanctuaries
            _pauseActions = () => false;
            _tankbusterAIDs = [];
            _raidwideAIDs = [];
            _ignoreRaidwideAIDs = [];

            // Save the territory ID for later
            _territoryID = territoryID;

            TerritoryType? map = Get(_territoryID);
            if (map is not null)
            {
                // Using TerritoryType.ExVersion listed for the map to determine the splitup
                // ExVersion is Expansion
                // Please verify the expansion in the TerritoryType sheet https://exd.camora.dev/sheet/TerritoryType
                switch (map.Value.ExVersion.RowId)
                {
                    case 0: LoadARR(); break;
                    case 1: LoadHW(); break;
                    case 2: LoadSB(); break;
                    case 3: LoadShB(); break;
                    case 4: LoadEW(); break;
                    case 5: LoadDT(); break;
                    //case 6: LoadEC(); break;
                }
            }
        }

        private static bool CheckForCast(uint baseID, uint actionID, float percentCast = 0)
        {
            if (Svc.Objects.FirstOrDefault(x => x.BaseId == baseID) is IBattleChara target)
            {
                if (target.IsCasting && target.CastActionId == actionID)
                    return (target.CurrentCastTime / target.TotalCastTime) >= percentCast;
                return false;
            }
            return false;
        }
    }
}
