using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.GameFunctions;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using WrathCombo.CustomComboNS.Functions;
namespace WrathCombo.Extensions;

internal static class BattleCharaExtensions
{
    public unsafe static CombatRole GetRole(this WrathPartyMember chara)
    {
        if (chara.RealJob?.Role == 1) return CombatRole.Tank;
        if (chara.RealJob?.Role == 2) return CombatRole.DPS;
        if (chara.RealJob?.Role == 3) return CombatRole.DPS;
        if (chara.RealJob?.Role == 4) return CombatRole.Healer;
        return CombatRole.NonCombat;
    }

    extension(IBattleChara chara)
    {
        public unsafe uint RawShieldValue()
        {
            FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* baseVal = (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)chara.Address;
            var value = baseVal->Character.CharacterData.ShieldValue;
            var rawValue = chara.MaxHp / 100 * value;

            return rawValue;
        }

        public unsafe byte ShieldPercentage()
        {
            FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* baseVal = (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)chara.Address;
            var value = baseVal->Character.CharacterData.ShieldValue;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasShield() => chara.RawShieldValue() > 0;

        public string GetInitials()
        {
            var ret = string.Concat(chara.Name.TextValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Length >= 1 && char.IsLetter(x[0]))
                .Select(x => char.ToUpper(x[0])));

            return ret;
        }
    }

    extension (IBattleChara? chara)
    {
        /// <summary>
        ///     Can be chained onto a <see cref="IGameObject" /> to make it return
        ///     <see langword="null" /> if the target is not below 99% HP.
        /// </summary>
        public IBattleChara? IfMissingHP(float missingHpp = 99) =>
            chara is not null && chara.Health <= missingHpp
                ? chara
                : null;
    }
}