using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;

namespace WrathCombo.Combos.PvE
{
    internal static partial class AST
    {
        public static ASTGauge Gauge => CustomComboFunctions.GetJobGauge<ASTGauge>();
        public static CardType DrawnCard { get; set; }
        public static ASTOpenerMaxLevel1 Opener1 = new();

        public static WrathOpener Opener()
        {
            if (Opener1.LevelChecked) return Opener1;

            return WrathOpener.Dummy;
        }

        internal static void InitCheckCards()
        {
            Svc.Framework.Update += CheckCards;
        }

        private static void CheckCards(IFramework framework)
        {
            if (Svc.ClientState.LocalPlayer is null || Svc.ClientState.LocalPlayer.ClassJob.RowId != 33)
                return;

            if (Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas] || Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.Unconscious])
            {
                QuickTargetCards.SelectedRandomMember = null;
                return;
            }

            if (DrawnCard != Gauge.DrawnCards[0])
            {
                DrawnCard = Gauge.DrawnCards[0];
            }

            if (CustomComboFunctions.IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards) &&
                (QuickTargetCards.SelectedRandomMember is null || BetterTargetAvailable()))
            {
                if (CustomComboFunctions.ActionReady(Play1))
                    QuickTargetCards.Invoke();
            }

            if (DrawnCard == CardType.NONE)
                QuickTargetCards.SelectedRandomMember = null;

        }

        private static bool BetterTargetAvailable()
        {
            if (QuickTargetCards.SelectedRandomMember is null ||
                QuickTargetCards.SelectedRandomMember.IsDead ||
                CustomComboFunctions.OutOfRange(Balance, QuickTargetCards.SelectedRandomMember))
                return true;

            var m = QuickTargetCards.SelectedRandomMember as IBattleChara;
            if ((DrawnCard is CardType.BALANCE && CustomComboFunctions.JobIDs.Melee.Any(x => x == m.ClassJob.RowId)) ||
                (DrawnCard is CardType.SPEAR && CustomComboFunctions.JobIDs.Ranged.Any(x => x == m.ClassJob.RowId)))
                return false;

            var targets = new List<IBattleChara>();
            for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
            {
                if (CustomComboFunctions.GetPartySlot(i) is not IBattleChara member) continue;
                if (member.GameObjectId == QuickTargetCards.SelectedRandomMember.GameObjectId) continue;
                if (member is null) continue; //Skip nulls/disconnected people
                if (member.IsDead) continue;
                if (CustomComboFunctions.OutOfRange(Balance, member)) continue;

                if (CustomComboFunctions.FindEffectOnMember(Buffs.BalanceBuff, member) is not null) continue;
                if (CustomComboFunctions.FindEffectOnMember(Buffs.SpearBuff, member) is not null) continue;

                if (Config.AST_QuickTarget_SkipDamageDown && CustomComboFunctions.TargetHasDamageDown(member)) continue;
                if (Config.AST_QuickTarget_SkipRezWeakness && CustomComboFunctions.TargetHasRezWeakness(member)) continue;

                if (member.GetRole() is CombatRole.Healer or CombatRole.Tank) continue;

                targets.Add(member);
            }

            if (targets.Count == 0) return false;
            if ((DrawnCard is CardType.BALANCE && targets.Any(x => CustomComboFunctions.JobIDs.Melee.Any(y => y == x.ClassJob.RowId))) ||
                (DrawnCard is CardType.SPEAR && targets.Any(x => CustomComboFunctions.JobIDs.Ranged.Any(y => y == x.ClassJob.RowId))))
            {
                QuickTargetCards.SelectedRandomMember = null;
                return true;
            }

            return false;

        }

        internal class QuickTargetCards : CustomComboFunctions
        {

            internal static List<IGameObject> PartyTargets = [];

            internal static IGameObject? SelectedRandomMember;

            public static void Invoke()
            {
                if (DrawnCard is not CardType.NONE)
                {
                    if (GetPartySlot(2) is not null)
                    {
                        SetTarget();
                        Svc.Log.Debug($"Set card to {SelectedRandomMember?.Name}");
                    }
                    else
                    {
                        Svc.Log.Debug($"Setting card to {LocalPlayer?.Name}");
                        SelectedRandomMember = LocalPlayer;
                    }
                }
                else
                {
                    SelectedRandomMember = null;
                }
            }

            private static bool SetTarget()
            {
                if (Gauge.DrawnCards[0].Equals(CardType.NONE)) return false;
                CardType cardDrawn = Gauge.DrawnCards[0];
                PartyTargets.Clear();
                for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
                {
                    if (GetPartySlot(i) is not IBattleChara member) continue;
                    if (member is null) continue; //Skip nulls/disconnected people
                    if (member.IsDead) continue;
                    if (OutOfRange(Balance, member)) continue;

                    if (FindEffectOnMember(Buffs.BalanceBuff, member) is not null) continue;
                    if (FindEffectOnMember(Buffs.SpearBuff, member) is not null) continue;

                    if (Config.AST_QuickTarget_SkipDamageDown && TargetHasDamageDown(member)) continue;
                    if (Config.AST_QuickTarget_SkipRezWeakness && TargetHasRezWeakness(member)) continue;

                    PartyTargets.Add(member);
                }
                //The inevitable "0 targets found" because of debuffs
                if (PartyTargets.Count == 0)
                {
                    for (int i = 1; i <= 8; i++) //Checking all 8 available slots and skipping nulls & DCs
                    {
                        if (GetPartySlot(i) is not IBattleChara member) continue;
                        if (member is null) continue; //Skip nulls/disconnected people
                        if (member.IsDead) continue;
                        if (OutOfRange(Balance, member)) continue;

                        if (FindEffectOnMember(Buffs.BalanceBuff, member) is not null) continue;
                        if (FindEffectOnMember(Buffs.SpearBuff, member) is not null) continue;

                        PartyTargets.Add(member);
                    }
                }

                if (SelectedRandomMember is not null)
                {
                    if (PartyTargets.Any(x => x.GameObjectId == SelectedRandomMember.GameObjectId))
                    {
                        //TargetObject(SelectedRandomMember);
                        return true;
                    }
                }


                if (PartyTargets.Count > 0)
                {
                    PartyTargets.Shuffle();
                    //Give card to DPS first
                    for (int i = 0; i <= PartyTargets.Count - 1; i++)
                    {
                        byte job = PartyTargets[i] is IBattleChara ? (byte)(PartyTargets[i] as IBattleChara).ClassJob.RowId : (byte)0;
                        if (((cardDrawn is CardType.BALANCE) && JobIDs.Melee.Contains(job)) ||
                            ((cardDrawn is CardType.SPEAR) && JobIDs.Ranged.Contains(job)))
                        {
                            //TargetObject(PartyTargets[i]);
                            SelectedRandomMember = PartyTargets[i];
                            return true;
                        }
                    }
                    //Give card to unsuitable DPS next
                    for (int i = 0; i <= PartyTargets.Count - 1; i++)
                    {
                        byte job = PartyTargets[i] is IBattleChara ? (byte)(PartyTargets[i] as IBattleChara).ClassJob.RowId : (byte)0;
                        if (((cardDrawn is CardType.BALANCE) && JobIDs.Ranged.Contains(job)) ||
                            ((cardDrawn is CardType.SPEAR) && JobIDs.Melee.Contains(job)))
                        {
                            //TargetObject(PartyTargets[i]);
                            SelectedRandomMember = PartyTargets[i];
                            return true;
                        }
                    }

                    //Give cards to healers/tanks if backup is turned on
                    if (IsEnabled(CustomComboPreset.AST_Cards_QuickTargetCards_TargetExtra))
                    {
                        for (int i = 0; i <= PartyTargets.Count - 1; i++)
                        {
                            byte job = PartyTargets[i] is IBattleChara ? (byte)(PartyTargets[i] as IBattleChara).ClassJob.RowId : (byte)0;
                            if ((cardDrawn is CardType.BALANCE && JobIDs.Tank.Contains(job)) ||
                                (cardDrawn is CardType.SPEAR && JobIDs.Healer.Contains(job)))
                            {
                                //TargetObject(PartyTargets[i]);
                                SelectedRandomMember = PartyTargets[i];
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        internal static void DisposeCheckCards()
        {
            Svc.Framework.Update -= CheckCards;
        }

        internal class ASTOpenerMaxLevel1 : WrathOpener
        {
            public override List<uint> OpenerActions { get; set; } =
            [
                EarthlyStar,
                FallMalefic,
                Combust3,
                Lightspeed,
                FallMalefic,
                FallMalefic,
                Divination,
                Balance,
                FallMalefic,
                LordOfCrowns,
                UmbralDraw,
                FallMalefic,
                Spear,
                Oracle,
                FallMalefic,
                FallMalefic,
                FallMalefic,
                FallMalefic,
                FallMalefic,
                Combust3,
                FallMalefic

            ];
            public override int MinOpenerLevel => 92;
            public override int MaxOpenerLevel => 109;

            public override bool HasCooldowns()
            {
                if (CustomComboFunctions.GetCooldown(EarthlyStar).CooldownElapsed >= 4f)
                    return false;

                if (!CustomComboFunctions.ActionReady(Lightspeed))
                    return false;

                if (!CustomComboFunctions.ActionReady(Divination))
                    return false;

                if (!CustomComboFunctions.ActionReady(Balance))
                    return true;

                if (!CustomComboFunctions.ActionReady(LordOfCrowns))
                    return false;

                if (!CustomComboFunctions.ActionReady(UmbralDraw))
                    return false;


                return true;
            }
        }
    }
}
