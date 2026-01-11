using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Extensions;

namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    private const StringComparison Lower = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Text Comparison for Tank Buster VFX Paths
    /// </summary>
    /// <param name="vfx">The VFX to check the Path of</param>
    /// <returns>Bool if vfx path matches</returns>
    public static bool IsTankBusterEffectPath(VfxInfo vfx)
    {
        return TankbusterPaths.Any(x => vfx.Path.StartsWith(x, Lower));
    }

    private static readonly FrozenSet<string> TankbusterPaths = FrozenSet.ToFrozenSet<string>([
        "vfx/lockon/eff/tank",                          // Generic TB check
        "vfx/lockon/eff/x6fe_fan100_50_0t1",            // Necron Blue Shockwave - Cone Tankbuster
        "vfx/common/eff/mon_eisyo03t",                  // M10 Deep Impact AoE TB (also generic?)
        "vfx/lockon/eff/m0676trg_tw_d0t1p",             // M10 Hot Impact shared TB
        "vfx/lockon/eff/m0676trg_tw_s6_d0t1p",          // M11 Raw Steel
        "vfx/lockon/eff/z6r2b3_8sec_lockon_c0a1",       // Kam'lanaut Princely Blow
        "vfx/lockon/eff/m0742trg_b1t1",                 // M7 Abominable Blink
        "vfx/lockon/eff/x6r9_tank_lockonae"             // M9 Hardcore Large TB
    ], StringComparer.OrdinalIgnoreCase);

    // List of Multi-Hit Shared Damage Effect Paths
    private static readonly FrozenSet<string> MHSharedDmgPaths = FrozenSet.ToFrozenSet([
        "vfx/lockon/eff/com_share4a1",
        "vfx/lockon/eff/com_share5a1",
        "vfx/lockon/eff/com_share6m7s_1v",
        "vfx/lockon/eff/com_share8s_0v",
        "vfx/lockon/eff/share_laser_5s_c0w", // Line
        "vfx/lockon/eff/share_laser_8s_c0g", // Line
        "vfx/lockon/eff/m0922trg_t2w" //Some Lightning based effect, presume specific raid?
    ], StringComparer.OrdinalIgnoreCase);

    // List of Regular Shared Damage Effect Paths
    private static readonly FrozenSet<string> SharedDmgPaths = FrozenSet.ToFrozenSet([
        "vfx/lockon/eff/coshare",
        "vfx/lockon/eff/share_laser",
        "vfx/lockon/eff/com_share",
        "vfx/lockon/eff/d1084_share_24m_s6_0k2",        // San d'Oria The Second Walk
        // Won't work till Static VFX tracking is added
        "vfx/monster/gimmick2/eff/z3o7_b1_g06c0t",      // Puppet's Bunker, Superior Flight Unit.
        "vfx/monster/gimmick4/eff/z5r1_b4_g09c0c"       // Aglaia, Nald'thal
    ], StringComparer.OrdinalIgnoreCase);

    //private static readonly FrozenSet<ushort> NoObjectStackDuties = FrozenSet.ToFrozenSet<ushort>([
    //    1194 // The Skydeep Cenote
    //]);

    /// <summary>
    /// Checks for incoming shared damage effects and retrieves relevant information.
    /// </summary>
    /// <remarks>
    /// A shared damage effect is identified by its visual effect path matching known shared damage effect paths.
    /// Not all shared damage effects may be detected, depending on the duty and effect used.
    /// Party members are prioritized when multiple valid effects are found, and the closest party member is selected.
    /// PartyMember will be null if no party member is affected (alliance / NPC helper).
    /// </remarks>
    /// <param name="distance">Distance to the effect</param>
    /// <param name="isMultiHit">Returns true if the effect will do multiple hits</param>
    /// <param name="partyMember">Returns an IBattleChara if the effect is on a party member</param>
    /// <returns></returns>
    public static bool CheckForSharedDamageEffect(out float distance, out bool isMultiHit, out IBattleChara? partyMember)
    {
        partyMember = null;
        distance = float.MaxValue;
        isMultiHit = false;

        bool MH = false; //holder for isMultiHit

        var vfxEffects = VfxManager.TrackedEffects.FilterToTargeted();

        if (vfxEffects.Count == 0)
            return false;

        // First: Get all valid multi-hit effects
        List<VfxInfo> multiHitEffects = [.. vfxEffects.Where(v =>
            v.VfxID != 0 && MHSharedDmgPaths.Any(p => v.Path.StartsWith(p, Lower)))];

        // If any multi-hit found → use that list (priority), else look for regular shared damage effects
        List<VfxInfo> AoEEffects;
        if (multiHitEffects.Count != 0)
        {
            AoEEffects = multiHitEffects;
            MH = true;
        }
        else
        {
            AoEEffects = [.. vfxEffects.Where(v =>
                v.VfxID != 0 && SharedDmgPaths.Any(p => v.Path.StartsWith(p, Lower)))];
        }

        if (AoEEffects.Count == 0)
            return false;

        #if DEBUG
        if (EzThrottler.Throttle("DebugSharedDamageEffectVFX1", 5000))
        {
            Svc.Log.Debug($"Found Incoming Shared Damage Effects {AoEEffects.Count}");
        }
		#endif

        // Expected Outcome from the LINQ:
        // Multi - hit on party member, Closest in your party
        // Multi - hit on other alliance player, That player(raid - wide stack)
        // Multi - hit on NPC/ ground marker, That marker
        // Regular share on party member, Closest in your party
        // Regular share on other alliance, Ignored
        // Regular share on NPC, That marker

        IBattleChara? bestTarget = null;
        if (AoEEffects.Count == 1) // Most battles are singular, skip LINQ if so
            bestTarget = AoEEffects[0].TargetID.GetObject() as IBattleChara;
        else
            bestTarget = AoEEffects //Note this will fail on Player based ARR Recordings. Trust Recordings are fine
                .Select(vfx => vfx.TargetID.GetObject())
                .OfType<IBattleChara>()
                // Multi-hit can be on anyone (only 1 per alliance), regular only on party members or NPCs,
                .Where(chara => MH || chara.IsInParty() || chara is IBattleNpc)
                // Prioritize party members first, then by distance
                .OrderBy(chara => chara.IsInParty() ? 0 : 1)
                .ThenBy(chara => GetTargetDistance(chara))
                .FirstOrDefault();

        if (bestTarget is null)
            return false;

        #if DEBUG
        if (EzThrottler.Throttle("DebugSharedDamageEffectVFX2", 5000))
        {
            Svc.Log.Debug($"Found Shared Damage Effects. Name:{bestTarget.Name} MH:{MH} Party:{bestTarget.IsInParty()}");
        }
        #endif

        //return only party member object (Don't want to illegally dash to Alliance or NPCs)
        isMultiHit = MH;
        partyMember = bestTarget.IsInParty() ? bestTarget : null;
        distance = GetTargetDistance(bestTarget);
        return true;

        // Flan: Saving this here for later in case I figure out how to handle no-object shared damage effects properly.
        // Don't want to remember what I wrote here otherwise.
        //else //Not in the object list, all players stack on a tower, not checking at all times.
        //if (NoObjectStackDuties.Contains(Svc.ClientState.TerritoryType) &&
        //    InBossEncounter())
        //{
        //    AoEEffects = VfxManager.TrackedEffects.FilterToNoTarget();
        //    sharedVfx = AoEEffects.FirstOrDefault(IsMultiHitSharedDamageEffectPath);
        //    if (sharedVfx.VfxID != 0) isMultiHit = true;
        //    else sharedVfx = AoEEffects.FirstOrDefault(IsShareDamageEffectPath);

        //    //Quick and dirty
        //    distance = Vector3.Distance(LocalPlayer.Position, sharedVfx.Placement.Position);
        //    return true;
        //}

        //return false;
    }

    /// <summary>
    /// Attempts to retrieve the current target of a detected tank buster visual effect.
    /// </summary>
    /// <remarks>This method searches for an active tank buster visual effect and attempts to resolve its
    /// target to a battle character. If no such effect is present or the target cannot be resolved, target is set to
    /// null and the method returns false. Probably won't work in dual tank situation.</remarks>
    /// <param name="target">When this method returns, contains the battle character targeted by the tank buster effect, if found; otherwise,
    /// null. This parameter is passed uninitialized.</param>
    /// <returns>true if a tank buster target is found and assigned to target; otherwise, false.</returns>
    public static bool TryGetTankBusterTarget(out IBattleChara target)
    {
        target = null!;

        var tankBusterVfx = VfxManager.TrackedEffects
            .FilterToTargeted()
            .FilterToTargetRole(CombatRole.Tank)
            .Where(x => x.TargetID.GetObject().IsInParty())
            .FirstOrDefault(IsTankBusterEffectPath);

        if (tankBusterVfx.VfxID == 0)
            return false;

        if (tankBusterVfx.TargetID.GetObject() is not IBattleChara battleChara)
            return false;

        target = battleChara;
        return true;
    }

    /// <summary>
    /// Checks if the specified character has an active tank buster marker on them.
    /// </summary>
    /// <param name="targetObject">The character to check. Defaults to the local player.</param>
    /// <returns>true if the target has an active tank buster effect, false otherwise.</returns>
    public static bool HasIncomingTankBusterEffect(
        IGameObject? targetObject = null)
    {
        // Default to local player if none provided
        targetObject ??= Player.Object;

        if (targetObject == null)
            return false;

        ulong targetId = targetObject.GameObjectId;

        return VfxManager.TrackedEffects
            .FilterToTarget(targetId)
            .Any(IsTankBusterEffectPath);
    }
}
