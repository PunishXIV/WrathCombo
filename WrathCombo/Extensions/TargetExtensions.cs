using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameFunctions;
namespace WrathCombo.Extensions;

internal static class TargetExtensions
{
    public unsafe static uint GetNameId(this IGameObject t)
    {
        return t.Struct()->GetNameId();
    }
}