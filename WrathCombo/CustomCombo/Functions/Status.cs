using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Linq;
using WrathCombo.Data;
using WrathCombo.Services;
using Status = Dalamud.Game.ClientState.Statuses.Status;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract partial class CustomComboFunctions
{
    /// <summary>
    /// Retrieves a Status object that is on the Player or specified Target, null if not found
    /// </summary>
    /// <param name="statusId">Status Effect ID</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <param name="target">Optional target</param>
    /// <returns>Status object or null.</returns>
    public static Status? GetStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)
    {
        // Default to LocalPlayer if no target/bad target
        target ??= LocalPlayer;

        // Use LocalPlayer's GameObjectId if playerOwned, null otherwise
        ulong? sourceId = !anyOwner ? LocalPlayer.GameObjectId : null;

        return Service.ComboCache.GetStatus(statusId, target, sourceId);
    }

    /// <summary>
    /// Checks to see if a status is on the Player or an optional target
    /// </summary>
    /// <param name="statusId">Status Effect ID</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <returns>Boolean if the status effect exists or not</returns>
    public static bool HasStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)
    {
        // Default to LocalPlayer if no target provided
        target ??= LocalPlayer;
        return GetStatusEffect(statusId, target, anyOwner) is not null;
    }

    /// <summary>
    /// Checks to see if a status is on the Player or an optional target, and supplies the Status as well
    /// </summary>
    /// <param name="statusId">Status Effect ID</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <param name="status">Retrieved Status object</param>
    /// <returns>Boolean if the status effect exists or not</returns>
    public static bool HasStatusEffect(ushort statusId, out Status? status, IGameObject? target = null, bool anyOwner = false)
    {
        target ??= LocalPlayer;
        status = GetStatusEffect(statusId, target, anyOwner);
        return status is not null;
    }

    /// <summary>
    /// Checks to see if any statuses are on the Player or an optional target
    /// </summary>
    /// <param name="statusId">List Of StatusIDs</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the statuses, true means anyone owns</param>
    /// <seealso cref="HasStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)"/>
    public static bool HasAnyStatusEffects(ushort[] status, IGameObject? target = null, bool anyOwner = false) =>
        status.Any(statusId => HasStatusEffect(statusId, target ?? LocalPlayer, anyOwner));

    /// <summary>
    /// Checks to see if all statuses are on the Player or an optional target
    /// </summary>
    /// <param name="statusId">List Of StatusIDs</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the statuses, true means anyone owns</param>
    /// <seealso cref="HasStatusEffect(ushort statusId, IGameObject? target = null, bool anyOwner = false)"/>
    public static bool HasAllStatusEffects(ushort[] status, IGameObject? target = null, bool anyOwner = false) =>
        status.All(statusId => HasStatusEffect(statusId, target ?? LocalPlayer, anyOwner));

    /// <summary>
    /// Gets remaining time of a Status Effect
    /// </summary>
    /// <param name="effect">Dalamud Status object</param>
    /// <returns>Float representing remaining status effect time</returns>
    public unsafe static float GetStatusEffectRemainingTime(Status? effect)
    {
        if (effect is null) return 0;
        if (effect.RemainingTime < 0) return (effect.RemainingTime * -1) + ActionManager.Instance()->AnimationLock;
        return effect.RemainingTime;
    }

    /// <summary>
    /// Retrieves remaining time of a Status Effect on the Player or Optional Target
    /// </summary>
    /// <param name="effectId">Status Effect ID</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <returns>Float representing remaining status effect time</returns>
    public unsafe static float GetStatusEffectRemainingTime(ushort effectId, IGameObject? target = null, bool anyOwner = false) =>
        GetStatusEffectRemainingTime(GetStatusEffect(effectId, target, anyOwner));

    /// <summary>
    /// Retrieves remaining time of a Status Effect
    /// </summary>
    /// <param name="effect">Dalamud Status object</param>
    /// <returns>Integer representing status effect stack count</returns>
    public static ushort GetStatusEffectStacks(Status? effect) => effect?.Param ?? 0;

    /// <summary>
    /// Retrieves the status effect stack count
    /// </summary>
    /// <param name="effectId">Status Effect ID</param>
    /// <param name="target">Optional Target</param>
    /// <param name="anyOwner">Check if the Player owns/created the status, true means anyone owns</param>
    /// <returns>Integer representing status effect stack count</returns>
    public static ushort GetStatusEffectStacks(ushort effectId, IGameObject? target = null, bool anyOwner = false) =>
        GetStatusEffectStacks(GetStatusEffect(effectId, target, anyOwner));


    /// <summary> Returns the name of a status effect from its ID. </summary>
    /// <param name="id"> ID of the status. </param>
    /// <returns></returns>
    public static string GetStatusName(uint id) => StatusCache.GetStatusName(id);

    public static bool TargetHasDamageDown(IGameObject? target) => StatusCache.HasDamageDown(target);

    public static bool TargetHasDamageUp(IGameObject? target) => StatusCache.HasDamageUp(target);

    public static bool TargetHasRezWeakness(IGameObject? target, bool checkForWeakness = true)
    {
        if (checkForWeakness && HasStatusEffect(43, target, true)) //Weakness = 43
            return true;

        return HasStatusEffect(44, target, true); //Brink of Death = 44
    }

    /// <summary>
    /// Checks if the target has a debuff that can be dispelled.
    /// </summary>
    /// <param name="target">The game object to check. Defaults to the current target if null.</param>
    /// <returns>True if the target has a cleansable debuff; otherwise, false.</returns>
    public static bool HasCleansableDebuff(IGameObject? target) => StatusCache.HasCleansableDebuff(target);

    /// <summary>
    /// Checks if the target has a beneficial status.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool HasBeneficialStatus(IGameObject? target) => StatusCache.HasBeneficialStatus(target);

    public static bool HasPhantomDispelStatus(IGameObject? target) => StatusCache.HasDamageUp(target) || StatusCache.HasEvasionUp(target) || HasStatusEffect(4355, target) || TargetIsInvincible(target);

    /// <summary>
    /// Checks to see if the player has a status that should stop all actions and unselect targets
    /// Acceleration bombs and Pyretics
    /// </summary>
    public static bool PlayerHasActionPenalty()
    {
        bool hasActionPenalty =
            //Player.IsInDuty &&  <-?
            Player.Status.Any(s =>
                // Acceleration Bomb within Timeframe
                (StatusCache.AccelerationBombs.Contains(s.StatusId) &&
                    GetStatusEffectRemainingTime(s) is > 0f and < 1.5f) ||

                // Pyretic
                StatusCache.Pyretics.Contains(s.StatusId)

            ) == true;

        if (hasActionPenalty)
            Svc.Targets.Target = null;

        return hasActionPenalty;
    }

    /// <summary>
    /// Checks if the target is invincible due to status effects or encounter-specific mechanics.
    /// </summary>
    /// <param name="target">The game object to check.</param>
    /// <returns>True if the target is invincible; otherwise, false.</returns>
    public static bool TargetIsInvincible(IGameObject? target)
    {
        if (target is not IBattleChara tar)
            return false;

        // Turn Target's status to uint hashset
        var targetStatuses = tar.StatusList.Select(s => s.StatusId).ToHashSet();
        uint targetID = tar.DataId;

        // Returning False in each case because there should be no other General Invincibility Check needed
        // for specified areas
        switch (Svc.ClientState.TerritoryType)
        {
            case 174: // Labyrinth of the Ancients
                // Thanatos, Spooky Ghosts Only
                if (targetID is 2350) return !HasStatusEffect(398);
                // Allagan Bomb
                if (targetID is 2407)
                    return NumberOfObjectsInRange<SelfCircle>(30,
                        checkInvincible: false) > 1;

                return false;

            case 508: // The Void Ark
                // Sawtooth 5103
                // Irminsul 5105
                if ((targetID is 5105 or 5103) &&
                    ((Player.Job.IsPhysicalRangedDps() && HasStatusEffect(941, tar, true)) ||
                     (Player.Job.IsMagicalRangedDps() && HasStatusEffect(942, tar, true))
                    )
                   ) return true;
                // Cuchulainn 5139, Checking one of the Stoneskins
                if (targetID is 5139 && HasStatusEffect(152, tar, true)) return true;
                return false;

            case 801 or 805 or 1122: // Interdimensional Rift (Omega 12 / Alphascape 4), Regular/Savage?/Ultimate?
                // Omega-M = 9339
                // Omega-F = 9340
                if (targetID is 9339 or 9340) //numbers are for Regular
                {
                    if (HasStatusEffect(1660)) return targetID == 9339; // Packet Filter M
                    if (HasStatusEffect(1661)) return targetID == 9340; // Packet Filter F
                    if (targetID is 9340) return HasStatusEffect(671, tar, true); // F being covered by M
                }

                //Savage/Ultimate? Not sure which omega fight uses 3499 and 3500.
                //Also, SE, why use a new Omega-M status and reuse the old Omega-F? -_-'
                //Wonder if targetIDs are the same......
                if ((tar.StatusList.Any(x => x.StatusId == 3454) && HasStatusEffect(3499)) ||
                    (tar.StatusList.Any(x => x.StatusId == 1675) && HasStatusEffect(3500)))
                    return true;

                //Check for any ol invincibility
                if (StatusCache.CompareLists(StatusCache.InvincibleStatuses, targetStatuses)) return true;

                return false;

            case 821: //Dohn Mheg Final Boss Lyre
                if (targetID is 3939 && !HasStatusEffect(386))
                    return true; //Unfooled means you can attack the Lyre
                return false;

            case 917: //Puppet's Bunker, Flight Mechs
                // 724P Alpha = 11792 (A)
                // 767P Beta  = 11793 (B)
                // 772P Chi   = 11794 (C)
                if (targetID is 11792 or 11793 or 11794)
                {
                    if (HasStatusEffect(2288)) return targetID != 11792;
                    if (HasStatusEffect(2289)) return targetID != 11793;
                    if (HasStatusEffect(2290)) return targetID != 11794;
                }
                return false;

            case 952: // Tower of Zot final bosses
                      // Technically not invincible, just need to ignore
                if (targetID is (13298 or 13299) && Svc.Objects.Any(y => y.DataId is 13297 && !y.IsDead))
                    return true;
                return false;

            case 966: //The Tower at Paradigm's Breach, Hansel & Gretel
                // Hansel = 12709
                // Gretel = 12708
                // If boss has shield or too close, boss gains 680 Parry, so easier to check for that
                // 680 Directional Parry
                // 2538 Strong of Shield
                // 2539 Stronger Together

                if (targetID is 12709 or 12708)
                {
                    bool isTank = Player.Job.IsTank();
                    bool bossHasParry = HasStatusEffect(680, tar);
                    bool isFrontFacing = AngleToTarget(tar) is AttackAngle.Front;

                    // Non Tanks should just ignore parrying boss(s)
                    // Tanks should only ignore their target if it has the buff and they aren't in front.
                    if (bossHasParry && (!isTank || !isFrontFacing)) return true;
                }
                return false;

            case 1248: // Jeuno 1 Ark Angels
                // ArkAngel HM = 1804
                // ArkAngel MR = 18051 (A)
                // ArkAngel GK = 18053 (B)
                // ArkAngel TT = 18052 (C)
                if (targetID is 18049 && HasStatusEffect(4410, tar, true)) return true;

                if (targetID is 18051 or 18052 or 18053)
                {
                    if (HasStatusEffect(4192)) return targetID != 18051; // Alliance A Red Epic
                    if (HasStatusEffect(4194)) return targetID != 18053; // Alliance B Yellow Fated
                    if (HasStatusEffect(4196)) return targetID != 18052; // Alliance C Blue Vaunted
                }
                return false;

            case 1267: //Sunken Temple of Qarn Temple Guardian
                if (targetID is 18300 && HasStatusEffect(350, tar, true)) return true;
                return false;

            case 1292: //Meso Terminal
                // Bloody Headsman = 18576 a
                // Pale Headsman = 18577 b
                // Ravenous Headsman = 18578 y
                // Pestilent Headsman = 18579 d
                // Hellmaker = 18642

                // Alpha = 4542 Player / 4546 Boss
                // Beta = 4543 Player / 4547 Boss
                // Gamma = 4544 Player / 4548 Boss
                // Delta = 4545 Player / 4549 Boss

                if (targetID is 18576 or 18577 or 18578 or 18579 or 18642)
                {
                    if (HasStatusEffect(3065)) return targetID != 18642; // Hellmaker checking for fire floor debuff
                    if (HasStatusEffect(4542)) return targetID != 18576; // Alpha
                    if (HasStatusEffect(4543)) return targetID != 18577; // Beta
                    if (HasStatusEffect(4544)) return targetID != 18578; // Gamma
                    if (HasStatusEffect(4545)) return targetID != 18579; // Delta
                }
                return false;

            default:
                // General invincibility check
                // Due to large size of InvincibleStatuses, best to check process this way
                return StatusCache.CompareLists(StatusCache.InvincibleStatuses, targetStatuses);
        }
    }

    /// <summary>
    /// Checks if a target has the max number of entries in their status list.
    /// <para>30 for players, 60 for NPCs.</para>
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public unsafe static bool TargetIsStatusCapped(IGameObject? target)
    {
        target ??= LocalPlayer;
        if (target is IBattleChara bc)
            return bc.StatusList.Count(x => x.StatusId != 0) == bc.Struct()->StatusManager.NumValidStatuses;

        return false;
    }

    /// <summary>
    /// Checks if the target has any remaining entries in the status list to be able to add a new status, or if the status is already on them from the player.
    /// <para>Does not actually validate status logic i.e player buffs on enemies isn't checked.</para>
    /// </summary>
    /// <param name="target"></param>
    /// <param name="statusId"></param>
    /// <returns></returns>
    public static bool CanApplyStatus(IGameObject? target, ushort statusId)
    {
        target ??= LocalPlayer;
        if (!TargetIsStatusCapped(target) || HasStatusEffect(statusId, target))
            return true;

        return false;
    }

    /// <summary>
    ///     Overload to accept a list of status IDs.
    /// </summary>
    /// <seealso cref="CanApplyStatus(IGameObject?,ushort)"/>
    public static bool CanApplyStatus(IGameObject? target, ushort[] status) =>
        status.Any(statusId => CanApplyStatus(target, statusId));
}