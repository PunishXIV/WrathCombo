using Dalamud.Plugin.Services;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;

namespace WrathCombo.Extensions
{
    internal static class ObjectTableExtensions
    {
        // Narrower search scope
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
