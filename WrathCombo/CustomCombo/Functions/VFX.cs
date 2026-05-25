using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Gui.Toast;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.Misc;
using WrathCombo.Services;

namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    public static SpeechSynthesizer? TTS;
    static CustomComboFunctions()
    {
        try
        {
            TTS = new();
        }
        catch(Exception e)
        {
            e.LogInfo();
        }
    }
    private const StringComparison Lower = StringComparison.OrdinalIgnoreCase;

    private static readonly StringComparer Lowerer =
        StringComparer.FromComparison(Lower);

    private static uint? CurrentCFC => Content.ContentFinderConditionRowId;

    private static List<TTSData> TTSTankbusters = [];
    private static List<TTSData> TTSGroupwides = [];
    private static bool CurrentRaidwideHandled = false;

    /// <summary>
    /// Determines whether a VFX path matches any list of VFX paths.<br/>
    /// Also checks that (if the path is restricted to certain duties) the current
    /// duty matches one of those specified.
    /// </summary>
    private static bool CheckPath(
        FrozenDictionary<string, uint[]> paths, string vfxPath)
    {
        foreach (var entry in paths)
        {
            if (!vfxPath.StartsWith(entry.Key, Lower))
                continue;

            if (entry.Value.Length == 0)
                return true;

            if (CurrentCFC is { } cfc && entry.Value.Contains(cfc))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Text Comparison for Tank Buster VFX Paths
    /// </summary>
    /// <param name="vfx">The VFX to check the Path of</param>
    /// <returns>Bool if vfx path matches</returns>
    public static bool IsTankBusterEffectPath(VfxInfo vfx) =>
        CheckPath(TankbusterPaths, vfx.Path);

    private static readonly FrozenDictionary<string, uint[]> TankbusterPaths =
        new Dictionary<string, uint[]>
        {
            { "vfx/lockon/eff/tank", [] },                    // Generic TB check
            { "vfx/lockon/eff/x6fe_fan100_50_0t1", [] },      // Necron Blue Shock
            { "vfx/common/eff/mon_eisyo03t", [] },            // M10 Deep Impact
            { "vfx/lockon/eff/m0676trg_tw_d0t1p", [] },       // M10 Hot Impact
            { "vfx/lockon/eff/m0676trg_tw_s6_d0t1p", [] },    // M11 Raw Stee
            { "vfx/lockon/eff/z6r2b3_8sec_lockon_c0a1", [] }, // Kam'lanaut Princely
            { "vfx/lockon/eff/m0742trg_b1t1", [] },           // M7 Abominable Blink
            { "vfx/lockon/eff/x6r9_tank_lockonae", [] },      // M9 Hardcore Big
            { "vfx/lockon/eff/target_ae_s5f", [779] },        // YorHa 3 (also matches some spread markers)
            { "vfx/lockon/eff/sharelaser2tank", [] },         // Unknown source
        }.ToFrozenDictionary(Lowerer);

    // List of Multi-Hit Shared Damage Effect Paths
    private static readonly FrozenDictionary<string, uint[]> MHSharedDmgPaths =
        new Dictionary<string, uint[]>
        {
            { "vfx/lockon/eff/com_share4a1", [] },
            { "vfx/lockon/eff/com_share5a1", [] },
            { "vfx/lockon/eff/com_share6m7s_1v", [] },
            { "vfx/lockon/eff/com_share8s_0v", [] },
            { "vfx/lockon/eff/share_laser_5s_c0w", [] }, // Line
            { "vfx/lockon/eff/share_laser_8s_c0g", [] }, // Line
            { "vfx/lockon/eff/m0922trg_t2w", [] },       // Some Lightning thing
        }.ToFrozenDictionary(Lowerer);

    // List of Regular Shared Damage Effect Paths
    private static readonly FrozenDictionary<string, uint[]> SharedDmgPaths =
        new Dictionary<string, uint[]>
        {
            { "vfx/lockon/eff/coshare", [] },
            { "vfx/lockon/eff/share_laser", [] },
            { "vfx/lockon/eff/share_1", [] },
            { "vfx/lockon/eff/com_share", [] },
            { "vfx/lockon/eff/d1084_share_24m_s6_0k2", [] },   // San d'Oria 2nd Walk
            { "vfx/monster/gimmick2/eff/z3o7_b1_g06c0t", [] }, // YorHa 2, Flight
            { "vfx/monster/gimmick3/eff/n4r1_b2_g06x", [] },   // Vanguard, Protector
            { "vfx/monster/gimmick4/eff/z5r1_b4_g09c0c", [] }, // Aglaia, Nald'thal
        }.ToFrozenDictionary(Lowerer);

    /* Associated logic removed. See commit: 844dcef4
    private static readonly FrozenSet<ushort> NoObjectStackDuties = FrozenSet
        .ToFrozenSet<ushort>([
        1194 // The Skydeep Cenote
    ]);
    */

    /// <summary>
    /// Checks for incoming shared damage effects and retrieves relevant information.
    /// </summary>
    /// <remarks>
    /// A shared damage effect is identified by its visual effect path matching known shared damage effect paths.
    /// Not all shared damage effects may be detected, depending on the duty and effect used.
    /// Party members are prioritized when multiple valid effects are found, and the closest party member is selected.
    /// PartyMember will be null if no party member is affected (alliance / NPC helper).
    /// </remarks>
    /// <param name="isMultiHit">Returns true if the effect will do multiple hits</param>
    /// <param name="partyMember">Returns a Game Object if the effect is on a party member.</param>
    /// <param name="maxDistance">Maximum distance to the effect</param>
    public static bool CheckForSharedDamageEffect(out bool isMultiHit,
        out IGameObject? partyMember, float maxDistance = 30f)
    {
        partyMember = null;
        isMultiHit = false;

        bool MH = false; //holder for isMultiHit
        bool PlaybackClosest = false;

        var vfxEffects = VfxManager.TrackedEffects.FilterToTargeted();

        if (vfxEffects.Count == 0)
            return false;

        // First: Get all valid multi-hit effects
        List<VfxInfo> multiHitEffects = vfxEffects
            .Where(v => v.VfxID != 0 && CheckPath(MHSharedDmgPaths, v.Path))
            .ToList();

        // If any multi-hit found → use that list (priority), else look for regular shared damage effects
        List<VfxInfo> AoEEffects;
        if (multiHitEffects.Count != 0)
        {
            AoEEffects = multiHitEffects;
            MH = true;
        }
        else
            AoEEffects = vfxEffects
                .Where(v => v.VfxID != 0 && CheckPath(SharedDmgPaths, v.Path))
                .ToList();

        if (AoEEffects.Count == 0)
            return false;

#if DEBUG
        if (EzThrottler.Throttle("DebugSharedDamageEffectVFX1", 5000))
            Svc.Log.Debug($"Found Incoming Shared Damage Effects {AoEEffects.Count}");
#endif

        // Expected Outcome from the LINQ:
        // Multi - hit on party member, Closest in your party
        // Multi - hit on other alliance player, That player(raid - wide stack)
        // Multi - hit on NPC/ ground marker, That marker
        // Regular share on party member, Closest in your party
        // Regular share on other alliance, Ignored
        // Regular share on NPC, That marker

        IGameObject? bestTarget;

        if (AoEEffects.Count == 1) // Most battles are singular, skip LINQ if so
            bestTarget = AoEEffects[0].TargetID.GetObject();
        else
        {
#if DEBUG
            if (Svc.Condition[ConditionFlag.DutyRecorderPlayback]) PlaybackClosest = true; //Trick to allow alliance targets during ARR recording Playback.
#endif
            bestTarget = AoEEffects //Note this will fail on Player based ARR Recordings. Trust Recordings are fine
                .Select(vfx => vfx.TargetID.GetObject())
                .OfType<IBattleChara>()
                // Multi-hit can be on anyone (only 1 per alliance), regular only on party members or NPCs,
                .Where(chara => chara.IsWithinRange(maxDistance) &&
                                (MH || chara.IsInParty() || chara is IBattleNpc || PlaybackClosest))
                // Prioritize party members first, then by distance
                .OrderBy(chara => chara.IsInParty() ? 0 : 1)
                .ThenBy(chara => GetTargetDistance(chara))
                .FirstOrDefault();
        }
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
        return true;
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

    public static void PlayTankbusterAlert()
    {
        if (!EzThrottler.Throttle("TankbusterTTS", 100))
            return;

        QuestToastOptions opts = new() { Position = QuestToastPosition.Centre, DisplayCheckmark = false };
        foreach (var vfx in VfxManager.TrackedEffects.FilterToTargeted().Where(x => IsTankBusterEffectPath(x) && x.TargetID.GetObject().IsInParty()))
        {
            if (!TTSTankbusters.Any(x => x.VFX == vfx))
                TTSTankbusters.Add(new TTSData() { VFX = vfx });
        }

        if (TTSTankbusters.Any(x => !x.TTSHandled))
        {
            var targets = TTSTankbusters.Where(x => !x.TTSHandled).Select(x => x.VFX.TargetID == Player.Object.GameObjectId ? "You" : x.VFX.TargetID.GetObject()?.Name.ToString()).ToList();
            if (Service.Configuration.TankbusterTTS)
                TTS?.SpeakAsync(string.Format(MiscStrings.TankbusterTTS, JoinNaturally(targets)));
            if (Service.Configuration.TankbusterToast)
                Svc.Toasts.ShowQuest(string.Format(MiscStrings.TankbusterTTS, JoinNaturally(targets)), opts);

            TTSTankbusters.ForEach(x => x.TTSHandled = true);
        }

        TTSTankbusters.RemoveAll(x => x.VFX.AgeSeconds >= 10);
    }

    public static void PlayGroupwideAlert()
    {
        if (!EzThrottler.Throttle("RaidwideTTS", 100))
            return;

        QuestToastOptions opts = new() { Position = QuestToastPosition.Centre, DisplayCheckmark = false };
        foreach (var vfx in VfxManager.TrackedEffects.Where(v => v.VfxID != 0 && (CheckPath(MHSharedDmgPaths, v.Path)) || CheckPath(SharedDmgPaths, v.Path)))
        {
            if (!TTSGroupwides.Any(x => x.VFX == vfx))
                TTSGroupwides.Add(new TTSData() { VFX = vfx });
        }

        if (TTSGroupwides.Any(x => !x.TTSHandled))
        {
            var multiHit = TTSGroupwides.Any(x => CheckPath(MHSharedDmgPaths, x.VFX.Path));
            var targets = TTSGroupwides.Where(x => !x.TTSHandled).Select(x => x.VFX.TargetID == Player.Object.GameObjectId ? "You" : x.VFX.TargetID.GetObject()?.Name.ToString()).ToList();
            if (Service.Configuration.AoEDamageTTS)
                TTS?.SpeakAsync(string.Format(MiscStrings.StackTTS, (multiHit ? MiscStrings.MultiHit : ""), JoinNaturally(targets)));
            if (Service.Configuration.AoEDamageToast)
                Svc.Toasts.ShowQuest(string.Format(MiscStrings.StackTTS, (multiHit ? MiscStrings.MultiHit : ""), JoinNaturally(targets)), opts);

            TTSGroupwides.ForEach(x => x.TTSHandled = true);
        }

        TTSGroupwides.RemoveAll(x => x.VFX.AgeSeconds >= 10);

        if (RaidwideCasting())
        {
            if (!CurrentRaidwideHandled)
            {
                if (Service.Configuration.AoEDamageTTS)
                    TTS?.SpeakAsync(MiscStrings.RaidwideTTS);
                if (Service.Configuration.AoEDamageToast)
                    Svc.Toasts.ShowQuest(MiscStrings.RaidwideTTS, opts);

                CurrentRaidwideHandled = true;
            }
        }
        else
            CurrentRaidwideHandled = false;
    }   

    public static string JoinNaturally(IList<string?> items)
    {
        if (items == null || items.Count == 0)
            return string.Empty;

        if (items.Count == 1)
            return items[0]!;

        if (items.Count == 2)
            return $"{items[0]} and {items[1]}";

        return string.Join(", ", items.Take(items.Count - 1))
               + " and " + items.Last();
    }

    private class TTSData
    {
        public VfxInfo VFX;
        public bool TTSHandled;
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
    
    /// <summary>
    /// Checks if the specified character has an active tank buster marker on them.
    /// </summary>
    /// <param name="ageSeconds">The number of seconds the oldest tank buster marker has been present.</param>
    /// <param name="targetObject">The character to check. Defaults to the local player.</param>
    /// <seealso cref="HasIncomingTankBusterEffect(IGameObject?)"/>
    /// <returns>true if the target has an active tank buster effect, false otherwise.</returns>
    public static bool HasIncomingTankBusterEffect(
        out float ageSeconds, IGameObject? targetObject = null)
    {
        // Default age to NaN
        ageSeconds = float.NaN;
        // Default to local player if none provided
        targetObject ??= Player.Object;

        if (targetObject == null)
            return false;

        ulong targetId = targetObject.GameObjectId;

        var targetEffects = VfxManager.TrackedEffects
            .FilterToTarget(targetId)
            .Where(IsTankBusterEffectPath)
            .ToArray();

        if (targetEffects.Length == 0)
            return false;

        ageSeconds = targetEffects.Max(e => e.AgeSeconds);

        return true;
    }
}
