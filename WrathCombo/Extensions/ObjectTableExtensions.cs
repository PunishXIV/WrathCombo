using Dalamud.Plugin.Services;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;

namespace WrathCombo.Extensions
{
    internal static class ObjectTableExtensions
    {
        /// <summary>
        /// Searches the IObjectTable for IBattleCharas in the specific locations in which they are stored
        /// </summary>
        /// <param name="objects">The Dalamud Game ObjectTable (ie Svc.Objects)</param>
        /// <param name="searchNonNetwork">IBattleCharas that "aren't networked" can exist, but 99.99% of the time looking in the non networked area is not required.</param>
        /// <returns>IEnumbable of non null IBattleCharas</returns>
        // https://github.com/aers/FFXIVClientStructs/blob/main/FFXIVClientStructs/FFXIV/Client/Game/Object/GameObjectManager.cs
        public static IEnumerable<IBattleChara> GetBattleCharas(this IObjectTable objects, bool searchNonNetwork = false)
        {
            // Networked battle characters (0-199, every other index)
            // Their minions/mounts/etc are the number skipped over
            for (var index = 0; index < 200; index += 2)
            {
                if (objects[index] is IBattleChara battleChara)
                {
                    yield return battleChara;
                }
            }

            if (searchNonNetwork)
            {
                // Non-networked objects (200-448)
                for (var index = 200; index < 449; index++)
                {
                    if (objects[index] is IBattleChara battleChara)
                    {
                        yield return battleChara;
                    }
                }
            }
        }
    }
}
