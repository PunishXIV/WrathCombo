using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;
using System.Linq;
using GameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace WrathCombo.Services
{
    //To be moved ?
    public static class GameObjectUtils
    {
        /// <summary>
        /// Converts a GameObject pointer to an IGameObject from the object table.
        /// </summary>
        /// <param name="ptr">The GameObject pointer to convert.</param>
        /// <returns>An IGameObject if found in the object table; otherwise, null.</returns>
        public static unsafe IGameObject? GetIGameObject(GameObject* ptr)
        {
            if (ptr == null) return null;
            return Svc.Objects.FirstOrDefault(x => x.Address == (IntPtr)ptr);
        }
    }

    public static unsafe class PartyUITargeting
    {
        private static readonly Lazy<IntPtr> pronounModule = new(() =>
        {
            var ptr = Framework.Instance()->GetUIModule()->GetPronounModule();
            return ptr != null ? (IntPtr)ptr : IntPtr.Zero;
        });

        // Signature for PronounModule::GetGameObjectByPronounId.
        private static readonly delegate* unmanaged<IntPtr, uint, GameObject*> getGameObjectFromPronounID = InitializePronounDelegate();

        private static delegate* unmanaged<IntPtr, uint, GameObject*> InitializePronounDelegate()
        {
            try
            {
                return (delegate* unmanaged<IntPtr, uint, GameObject*>)Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B D8 48 85 C0 0F 85 ?? ?? ?? ?? 8D 4F DD");
            }
            catch (Exception ex)
            {
                ex.Log();
                return null;
            }
        }

        /// <summary>
        /// Gets the game object currently under the mouse cursor in the UI (e.g., party list, target bar).
        /// </summary>
        /// <returns>An IGameObject if a UI target exists; otherwise, null.</returns>
        public static IGameObject? UiMouseOverTarget
        {
            get
            {
                if (pronounModule.Value == IntPtr.Zero) return null;
                unsafe
                {
                    return GameObjectUtils.GetIGameObject(((PronounModule*)pronounModule.Value)->UiMouseOverTarget);
                }
            }
        }

        /// <summary>
        /// Gets a game object by pronoun ID (e.g., 44–50 for party members 2–8).
        /// </summary>
        /// <param name="id">The pronoun ID.</param>
        /// <returns>An IGameObject if found; otherwise, null.</returns>
        public static IGameObject? GetIGameObjectFromPronounID(int id)
        {
            if (pronounModule.Value == IntPtr.Zero || getGameObjectFromPronounID == null) return null;
            try
            {
                return GameObjectUtils.GetIGameObject(getGameObjectFromPronounID(pronounModule.Value, (uint)id));
            }
            catch (Exception ex)
            {
                ex.Log();
                return null;
            }
        }

        /// <summary>
        /// Gets a party member by slot number (1–8).
        /// </summary>
        /// <param name="slot">The party slot (1 for local player, 2–8 for party members).</param>
        /// <returns>An IGameObject for the party member if found; otherwise, null.</returns>
        /// IDs start at 44 and go to 51
        public static IGameObject? GetPartySlot(int slot)
        {
            if (slot < 1 || slot > 8) return null;
            return slot == 1 ? Player.Object : GetIGameObjectFromPronounID(42 + slot);
        }
    }
}
