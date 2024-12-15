using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrathCombo.Extensions
{
    internal static class TargetExtensions
    {
        public unsafe static uint GetNameId(this IGameObject t)
        {
            return t.Struct()->GetNameId();
        }
    }
}
