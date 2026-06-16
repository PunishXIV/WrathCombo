using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using WrathCombo.Extensions;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonJobHudMNK1.ChakraGauge;

namespace WrathCombo.Data
{
    public class SimpleTargetState
    {
        public uint CurrentHP;
        public uint MaxHP;
        public ulong GameObjectID;

        public SimpleTargetState(uint currentHP, uint maxHP, ulong gameObjectID)
        {
            CurrentHP = currentHP;
            MaxHP = maxHP;
            GameObjectID = gameObjectID;
        }

        public static List<SimpleTargetState> TargetStates = [];

        public unsafe static void ManageStateList()
        {
            foreach (var o in Svc.Objects.Where(x => x is IBattleChara).Cast<IBattleChara>())
            {
                if (!o.Struct()->InCombat)
                {
                    TargetStates.RemoveAll(x => x.GameObjectID == o.GameObjectId);
                }

                if (TargetStates.Any(x => x.GameObjectID == o.GameObjectId))
                    continue;

                SimpleTargetState target = new(o.CurrentHp, o.MaxHp, o.GameObjectId);

                TargetStates.Add(target);
            }

            UpdatePendingHP();
        }

        public unsafe static void UpdatePendingHP() 
        { 
            foreach (var o in TargetStates)
            {
                BattleChara* b = (BattleChara*)o.GameObjectID.GetObject().Address;
                if (!b->InCombat)
                    continue;
                foreach (var p in ActionWatching.PendingHPChanges.Where(x => x.gameObjectId == o.GameObjectID))
                {
                    o.CurrentHP = (uint)Math.Clamp(o.CurrentHP + (p.positiveChange ? p.value : -p.value), 0, o.MaxHP);
                }
                ActionWatching.PendingHPChanges.RemoveAll(x => x.gameObjectId == o.GameObjectID);
            }
        }
    }
}
