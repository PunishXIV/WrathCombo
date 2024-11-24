using Dalamud.Game.ClientState.Objects.SubKinds;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;

namespace WrathCombo.Combos.PvE
{
    internal partial class GNB
    {
        ///<summary>
        ///    Shorter reference to the local player.
        ///</summary>
        private static readonly IPlayerCharacter? LocalPlayer = CustomComboFunctions.LocalPlayer;

        ///<summary>
        ///    Whether the player has a shield from HOC from themselves.
        ///</summary>
        private static bool HasOwnHOC => LocalPlayer != null && CustomComboFunctions.FindEffect(Buffs.HeartOfCorundum | Buffs.HeartOfStone, LocalPlayer, LocalPlayer.GameObjectId) != null;

        ///<summary>
        ///    Whether the player has buff from HOC from anyone.
        ///</summary>
        private static bool HasAnyHOC => LocalPlayer != null && CustomComboFunctions.FindEffect(Buffs.HeartOfCorundum | Buffs.HeartOfStone) != null;

        ///<summary>
        ///    Decides if the player should use HOC on themselves, based on general rules and the player's configuration.
        ///</summary>
        ///<param name="aoe">Whether AoE or ST CustomComboPreset should be checked.</param>
        ///<returns>Whether HOC should be used on self.</returns>
        private static bool ShouldHOCSelf(bool aoe = false)
        {
            // Return false if HOC is disabled or already present
            if ((!aoe && (!CustomComboFunctions.IsEnabled(CustomComboPreset.GNB_ST_Mitigation) || !CustomComboFunctions.IsEnabled(CustomComboPreset.GNB_ST_HOC))) ||
                (aoe && (!CustomComboFunctions.IsEnabled(CustomComboPreset.GNB_AoE_Mitigation) || !CustomComboFunctions.IsEnabled(CustomComboPreset.GNB_AoE_HOC))) ||
                HasOwnHOC || LocalPlayer == null || LocalPlayer.TargetObject == null)
            {
                return false;
            }

            // Check if in configured content
            var inHOCContent = aoe || ContentCheck.IsInConfiguredContent(Config.GNB_ST_HOCDifficulty, Config.GNB_ST_HOCDifficultyListSet);
            if (!inHOCContent) return false;

            var hpRemaining = CustomComboFunctions.PlayerHealthPercentageHp();
            var hpThreshold = aoe ? 90f : (float)Config.GNB_ST_HOCThreshold;

            // Return false if above health threshold
            if (hpRemaining > hpThreshold) return false;

            var targetIsBoss = CustomComboFunctions.IsBoss(LocalPlayer.TargetObject);
            var bossRestriction = aoe ? (int)Config.BossAvoidance.Off : (int)Config.GNB_ST_HOCBossRestriction;

            // Return false if avoiding bosses and the target is one
            if (bossRestriction == (int)Config.BossAvoidance.On && targetIsBoss) return false;

            return true;
        }
    }
}
