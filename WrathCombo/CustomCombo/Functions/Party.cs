using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WrathCombo.AutoRotation;
using WrathCombo.Combos.PvE;
using WrathCombo.Extensions;
using WrathCombo.Services;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    /// <summary> Checks if the player is in a party. Optionally, refine by minimum party size. </summary>
    /// <param name="partySize"> The minimum amount of party members required. </param>
    public static bool IsInParty(int partySize = 2) => GetPartyMembers().Count >= partySize;

    /// <summary> Gets the party list </summary>
    /// <returns> Current party list. </returns>
    public static unsafe List<WrathPartyMember> GetPartyMembers(bool allowCache = true)
    {
        if (!Player.Available) return [];
        _partyList.RemoveAll(x => x.BattleChara is null);
        if (allowCache && !EzThrottler.Throttle("PartyUpdateThrottle", 2000))
            return _partyList;

        var existingIds = _partyList.Select(x => x.GameObjectId).ToHashSet();

        for (int i = 1; i <= 8; i++)
        {
            var member = SimpleTarget.GetPartyMemberInSlotSlot(i);
            if (member is IBattleChara chara)
            {
                var existingMember = _partyList.FirstOrDefault(x => x.GameObjectId == chara.GameObjectId);
                if (existingMember != null)
                {
                    // Update existing member's properties as needed
                    existingMember.CurrentHP = chara.CurrentHp;
                    if (member is IBattleNpc)
                    {
                        foreach (var p in InfoProxyPartyMember.Instance()->CharDataSpan)
                        {
                            if (p.Sort == i - 1)
                                existingMember.NPCClassJob = p.Job;
                        }
                    }
                }
                else
                {
                    WrathPartyMember wmember = new()
                    {
                        GameObjectId = chara.GameObjectId,
                        CurrentHP = chara.CurrentHp
                    };
                    if (member is IBattleNpc)
                    {
                        foreach (var p in InfoProxyPartyMember.Instance()->CharDataSpan)
                        {
                            if (p.Sort == i - 1)
                                wmember.NPCClassJob = p.Job;
                        }
                    }
                    _partyList.Add(wmember);
                    existingIds.Add(chara.GameObjectId);
                }
            }
        }

        if ((Service.Configuration.AddOutOfPartyNPCsToRetargeting) || (AutoRotationController.cfg?.Enabled == true && AutoRotationController.cfg.HealerSettings.IncludeNPCs && Player.Job.IsHealer()))
        {
            foreach (var npc in Svc.Objects.OfType<IBattleNpc>().Where(x => !existingIds.Contains(x.GameObjectId)))
            {
                if (npc.BattleNpcKind is BattleNpcSubKind.Pet) continue; // Skips carbuncles, fairies etc.
                if (npc.BattleNpcKind is BattleNpcSubKind.Chocobo && npc.OwnerId != Player.GameObject->GetGameObjectId()) continue; // Skips other players' chocobos

                if (ActionManager.CanUseActionOnTarget(RoleActions.Healer.Esuna, npc.GameObject()))
                {
                    WrathPartyMember wmember = new()
                    {
                        GameObjectId = npc.GameObjectId,
                        CurrentHP = npc.CurrentHp,
                        IsOutOfPartyNPC = true
                    };
                    _partyList.Add(wmember);
                    existingIds.Add(npc.GameObjectId);
                }
            }
        }
        else
        {
            _partyList.RemoveAll(x => x.IsOutOfPartyNPC);
        }

        _partyList.RemoveAll(x => x.BattleChara is null);
        return _partyList;
    }

    private static List<WrathPartyMember> _partyList = new();

    [field: MaybeNull]
    public static List<WrathPartyMember> DeadPeople
    {
        get
        {
            field ??= new();
            foreach (var pc in Svc.Objects.Where(x => x is IBattleChara chara && chara.IsFriendly()).Cast<IBattleChara>().ToList())
            {
                if (pc.IsDead && !pc.StatusList.Any(x => x.StatusId == All.Buffs.Raised))
                {
                    if (!field.Any(x => x.GameObjectId == pc.GameObjectId))
                        field.Add(new WrathPartyMember
                        {
                            GameObjectId = pc.GameObjectId,
                            CurrentHP = pc.CurrentHp,
                            NPCClassJob = pc.ClassJob.RowId
                        });
                }
            }
            field.RemoveAll(x => x.BattleChara is null || !x.BattleChara.IsDead);
            return field;
        }
    }

    public static float GetPartyAvgHPPercent()
    {
        float totalHP = 0;
        int count = 0;

        for (int i = 1; i <= 8; i++)
        {
            if (SimpleTarget.GetPartyMemberInSlotSlot(i) is IBattleChara member && !member.IsDead)
            {
                totalHP += GetTargetHPPercent(member);
                count++;
            }
        }

        return count == 0 ? 0 : totalHP / count;
    }

    public static float GetPartyBuffPercent(ushort buff)
    {
        int buffCount = 0;
        int partyCount = 0;

        for (int i = 1; i <= 8; i++)
        {
            if (SimpleTarget.GetPartyMemberInSlotSlot(i) is IBattleChara member && !member.IsDead)
            {
                if (HasStatusEffect(buff, member, true)) buffCount++;
                partyCount++;
            }
        }

        return partyCount == 0 ? 0 : (float)buffCount / partyCount * 100f;
    }

    public static bool PartyInCombat() => PartyEngageDuration().Ticks > 0;
}

public enum AllianceGroup
{
    GroupA,
    GroupB,
    GroupC,
    NotInAlliance
}

public class WrathPartyMember
{
    public bool HPUpdatePending = false;
    public bool MPUpdatePending = false;
    public ulong GameObjectId;
    public uint NPCClassJob;
    public bool IsOutOfPartyNPC = false;

    public ClassJob? RealJob => NPCClassJob > 0 && ExcelJobHelper.TryGetJobById(NPCClassJob, out var realJob)
        ? realJob
        : BattleChara?.ClassJob.Value ?? ExcelJobHelper.GetJobById(0);

    public IBattleChara? BattleChara => Svc.Objects.FirstOrDefault(x => x.GameObjectId == GameObjectId) as IBattleChara;
    public IGameObject? GameObject => Svc.Objects.FirstOrDefault(x => x.GameObjectId == GameObjectId);
    public Dictionary<ushort, long> BuffsGainedAt = new();

    private uint _currentHP;
    public uint CurrentHP
    {
        get
        {
            if (BattleChara != null)
            {
                if ((_currentHP > BattleChara.CurrentHp && !HPUpdatePending) || _currentHP < BattleChara.CurrentHp)
                    _currentHP = BattleChara.CurrentHp;
            }
            return _currentHP;
        }
        set => _currentHP = value;
    }

    private uint _currentMP;
    public uint CurrentMP
    {
        get
        {
            if (BattleChara != null)
            {
                if ((_currentMP > BattleChara.CurrentMp && !MPUpdatePending) || _currentMP < BattleChara.CurrentMp)
                    _currentMP = BattleChara.CurrentMp;
            }
            return _currentMP;
        }
        set => _currentMP = value;
    }

    public float TimeSinceBuffApplied(ushort buff)
    {
        return BuffsGainedAt.TryGetValue(buff, out var timestamp) ? (Environment.TickCount64 - timestamp) / 1000f : 0;
    }
}