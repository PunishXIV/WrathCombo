#region Dependencies

using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Linq;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class GNB
{
    public static int MaxCartridges() => TraitLevelChecked(427) ? 3 : TraitLevelChecked(257) ? 2 : 0; //Level Check helper for Maximum Ammo

    #region Simple Mode - Single Target
    internal class GNB_ST_Simple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not KeenEdge)
                return actionID; //Our button

            #region Variables
            //Gauge
            byte Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
            byte GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
                                                                  //Cooldown-related
            float gfCD = GetCooldownRemainingTime(GnashingFang); //GnashingFang's cooldown; 30s total
            float nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
            float ddCD = GetCooldownRemainingTime(DoubleDown); //Double Down's cooldown; 60s total
            float bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
            float nmLeft = GetBuffRemainingTime(Buffs.NoMercy); //Remaining time for No Mercy buff (20s)
            bool hasNM = nmCD is >= 40 and <= 60; //Checks if No Mercy is active
            bool hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
            bool hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff
                                                           //Misc
            bool inOdd = bfCD is < 90 and > 20; //Odd Minute
            bool canLateWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
            float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x
            bool justMitted = JustUsed(OriginalHook(HeartOfStone), 4f) ||
                                 JustUsed(OriginalHook(Nebula), 5f) ||
                                 JustUsed(Camouflage, 5f) ||
                                 JustUsed(All.Rampart, 5f) ||
                                 JustUsed(Aurora, 5f) ||
                                 JustUsed(Superbolide, 9f);

            #region Minimal Requirements
            //Ammo-relative
            bool canBS = LevelChecked(BurstStrike) && //Burst Strike is unlocked
                        Ammo > 0; //Has Ammo
            bool canGF = LevelChecked(GnashingFang) && //GnashingFang is unlocked
                        gfCD < 0.6f && //Gnashing Fang is off cooldown
                        !HasEffect(Buffs.ReadyToBlast) && //to ensure Hypervelocity is spent in case Burst Strike is used before Gnashing Fang
                        GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                        Ammo > 0; //Has Ammo
            bool canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                        ddCD < 0.6f && //Double Down is off cooldown
                        Ammo > 0; //Has Ammo
            bool canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                        bfCD < 0.6f; //Bloodfest is off cooldown
            //Cooldown-relative
            bool canZone = LevelChecked(DangerZone) && //Zone is unlocked
                          GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
            bool canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                           hasBreak; //No Mercy or Ready To Break is active
            bool canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                         GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
            bool canContinue = LevelChecked(Continuation); //Continuation is unlocked
            bool canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                           GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                           hasReign; //Ready To Reign is active
            #endregion
            #endregion

            if (Opener().FullOpener(ref actionID))
                return actionID;

            #region Mitigations
            if (Config.GNB_ST_MitsOptions != 1)
            {
                if (InCombat() && //Player is in combat
                    !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                {
                    //Superbolide
                    if (ActionReady(Superbolide) && //Superbolide is ready
                        PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                        return Superbolide;

                    if (IsPlayerTargeted())
                    {
                        //Nebula
                        if (ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Nebula);

                        //Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        //Reprisal
                        if (ActionReady(All.Reprisal) && //Reprisal is ready
                            InActionRange(All.Reprisal) && //Player is in range of Reprisal
                            PlayerHealthPercentageHp() < 90) //Player's health is below 90%
                            return All.Reprisal;
                    }

                    //Camouflage
                    if (ActionReady(Camouflage) && //Camouflage is ready
                        PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                        return Camouflage;

                    //Corundum
                    if (ActionReady(OriginalHook(HeartOfStone)) && //Corundum
                        PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                        return OriginalHook(HeartOfStone);

                    //Aurora
                    if (LevelChecked(Aurora) && //Aurora is unlocked
                        !(HasEffect(Buffs.Aurora) || TargetHasEffectAny(Buffs.Aurora)) && //Aurora is not active on self or target
                        PlayerHealthPercentageHp() < 85) //
                        return Aurora;
                }
            }
            #endregion

            #region Variant
            //Variant Cure
            if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) &&
                IsEnabled(Variant.VariantCure)
                && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                return Variant.VariantCure;

            //Variant SpiritDart
            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
            if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                IsEnabled(Variant.VariantSpiritDart) &&
                CanWeave() &&
                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                return Variant.VariantSpiritDart;

            //Variant Ultimatum
            if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) &&
                IsEnabled(Variant.VariantUltimatum) &&
                CanWeave() &&
                ActionReady(Variant.VariantUltimatum))
                return Variant.VariantUltimatum;
            #endregion

            #region Bozja
            if (Bozja.IsInBozja) //Checks if we're inside Bozja instances
            {
                //oGCDs
                if (CanWeave())
                {
                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFocus) && //Lost Focus is enabled
                        HasActionEquipped(Bozja.LostFocus) &&
                        GetBuffStacks(Bozja.Buffs.Boost) < 16) //Boost stacks are below 16
                        return Bozja.LostFocus;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFontOfPower) && //Lost Font of Power is enabled
                        HasActionEquipped(Bozja.LostFontOfPower) &&
                        IsOffCooldown(Bozja.LostFontOfPower)) //Lost Focus was not just used within 30 seconds
                        return Bozja.LostFontOfPower;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSlash) &&
                        HasActionEquipped(Bozja.LostSlash) &&
                        IsOffCooldown(Bozja.LostSlash))
                        return Bozja.LostSlash;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfNobleEnds))
                    {
                        if (!IsEnabled(CustomComboPreset.GNB_Bozja_PowerEnds) &&
                            HasActionEquipped(Bozja.BannerOfNobleEnds) &&
                            IsOffCooldown(Bozja.BannerOfNobleEnds))
                            return Bozja.BannerOfNobleEnds;

                        if (IsEnabled(CustomComboPreset.GNB_Bozja_PowerEnds) &&
                            HasActionEquipped(Bozja.BannerOfNobleEnds) &&
                            IsOffCooldown(Bozja.BannerOfNobleEnds) &&
                            JustUsed(Bozja.LostFontOfPower, 5f))
                            return Bozja.BannerOfNobleEnds;
                    }

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfHonoredSacrifice))
                    {
                        if (!IsEnabled(CustomComboPreset.GNB_Bozja_PowerSacrifice) &&
                            HasActionEquipped(Bozja.BannerOfHonoredSacrifice) &&
                            IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                            return Bozja.BannerOfHonoredSacrifice;
                        if (IsEnabled(CustomComboPreset.GNB_Bozja_PowerSacrifice) &&
                            HasActionEquipped(Bozja.BannerOfHonoredSacrifice) &&
                            IsOffCooldown(Bozja.BannerOfHonoredSacrifice) &&
                            JustUsed(Bozja.LostFontOfPower, 5f))
                            return Bozja.BannerOfHonoredSacrifice;
                    }

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfHonedAcuity) &&
                        HasActionEquipped(Bozja.BannerOfHonedAcuity) &&
                        IsOffCooldown(Bozja.BannerOfHonedAcuity) &&
                        !HasEffect(Bozja.Buffs.BannerOfTranscendentFinesse))
                        return Bozja.BannerOfHonedAcuity;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFairTrade) &&
                        HasActionEquipped(Bozja.LostFairTrade) &&
                        IsOffCooldown(Bozja.LostFairTrade))
                        return Bozja.LostFairTrade;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostAssassination) &&
                        HasActionEquipped(Bozja.LostAssassination) &&
                        IsOffCooldown(Bozja.LostAssassination))
                        return Bozja.LostAssassination;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostManawall) &&
                        HasActionEquipped(Bozja.LostManawall) &&
                        IsOffCooldown(Bozja.LostManawall))
                        return Bozja.LostManawall;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfTirelessConviction) &&
                        HasActionEquipped(Bozja.BannerOfTirelessConviction) &&
                        IsOffCooldown(Bozja.BannerOfTirelessConviction) &&
                        !HasEffect(Bozja.Buffs.BannerOfUnyieldingDefense))
                        return Bozja.BannerOfTirelessConviction;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBloodRage) &&
                        HasActionEquipped(Bozja.LostBloodRage) &&
                        IsOffCooldown(Bozja.LostBloodRage))
                        return Bozja.LostBloodRage;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfSolemnClarity) &&
                        HasActionEquipped(Bozja.BannerOfSolemnClarity) &&
                        IsOffCooldown(Bozja.BannerOfSolemnClarity) &&
                        !HasEffect(Bozja.Buffs.BannerOfLimitlessGrace))
                        return Bozja.BannerOfSolemnClarity;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure2) &&
                        HasActionEquipped(Bozja.LostCure2) &&
                        IsOffCooldown(Bozja.LostCure2) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure2_Health)
                        return Bozja.LostCure2;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure4) &&
                        HasActionEquipped(Bozja.LostCure4) &&
                        IsOffCooldown(Bozja.LostCure4) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure4_Health)
                        return Bozja.LostCure4;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostReflect) &&
                        HasActionEquipped(Bozja.LostReflect) &&
                        IsOffCooldown(Bozja.LostReflect) &&
                        !HasEffect(Bozja.Buffs.LostReflect))
                        return Bozja.LostReflect;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostAethershield) &&
                        HasActionEquipped(Bozja.LostAethershield) &&
                        IsOffCooldown(Bozja.LostAethershield) &&
                        !HasEffect(Bozja.Buffs.LostAethershield) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostAethershield_Health)
                        return Bozja.LostAethershield;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSwift) &&
                        HasActionEquipped(Bozja.LostSwift) &&
                        IsOffCooldown(Bozja.LostSwift) &&
                        !HasEffect(Bozja.Buffs.LostSwift))
                        return Bozja.LostSwift;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFontOfSkill) &&
                        HasActionEquipped(Bozja.LostFontOfSkill) &&
                        IsOffCooldown(Bozja.LostFontOfSkill))
                        return Bozja.LostFontOfSkill;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostRampage) &&
                        HasActionEquipped(Bozja.LostRampage) &&
                        IsOffCooldown(Bozja.LostRampage))
                        return Bozja.LostRampage;
                }

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostStealth) &&
                    HasActionEquipped(Bozja.LostStealth) &&
                    !InCombat() &&
                    IsOffCooldown(Bozja.LostStealth))
                    return Bozja.LostStealth;

                //GCDs
                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostDeath) &&
                    HasActionEquipped(Bozja.LostDeath) &&
                    IsOffCooldown(Bozja.LostDeath))
                    return Bozja.LostDeath;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure) &&
                    HasActionEquipped(Bozja.LostCure) &&
                    IsOffCooldown(Bozja.LostCure) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure_Health)
                    return Bozja.LostCure;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure3) &&
                    HasActionEquipped(Bozja.LostCure3) &&
                    IsOffCooldown(Bozja.LostCure3) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure3_Health)
                    return Bozja.LostCure3;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostArise) &&
                    HasActionEquipped(Bozja.LostArise) &&
                    IsOffCooldown(Bozja.LostArise) &&
                    GetTargetHPPercent() == 0 &&
                    !HasEffect(All.Debuffs.Raise))
                    return Bozja.LostArise;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSacrifice) &&
                    HasActionEquipped(Bozja.LostSacrifice) &&
                    IsOffCooldown(Bozja.LostSacrifice) &&
                    GetTargetHPPercent() == 0)
                    return Bozja.LostSacrifice;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostReraise) &&
                    HasActionEquipped(Bozja.LostReraise) &&
                    IsOffCooldown(Bozja.LostReraise) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostReraise_Health)
                    return Bozja.LostReraise;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSpellforge) &&
                    HasActionEquipped(Bozja.LostSpellforge) &&
                    IsOffCooldown(Bozja.LostSpellforge) &&
                    (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
                    return Bozja.LostSpellforge;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSteelsting) &&
                    HasActionEquipped(Bozja.LostSteelsting) &&
                    IsOffCooldown(Bozja.LostSteelsting) &&
                    (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
                    return Bozja.LostSteelsting;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostProtect) &&
                    HasActionEquipped(Bozja.LostProtect) &&
                    IsOffCooldown(Bozja.LostProtect) &&
                    !HasEffect(Bozja.Buffs.LostProtect))
                    return Bozja.LostProtect;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostShell) &&
                    HasActionEquipped(Bozja.LostShell) &&
                    IsOffCooldown(Bozja.LostShell) &&
                    !HasEffect(Bozja.Buffs.LostShell))
                    return Bozja.LostShell;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBravery) &&
                    HasActionEquipped(Bozja.LostBravery) &&
                    IsOffCooldown(Bozja.LostBravery) &&
                    !HasEffect(Bozja.Buffs.LostBravery))
                    return Bozja.LostBravery;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostProtect2) &&
                    HasActionEquipped(Bozja.LostProtect2) &&
                    IsOffCooldown(Bozja.LostProtect2) &&
                    !HasEffect(Bozja.Buffs.LostProtect2))
                    return Bozja.LostProtect2;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostShell2) &&
                    HasActionEquipped(Bozja.LostShell2) &&
                    IsOffCooldown(Bozja.LostShell2) &&
                    !HasEffect(Bozja.Buffs.LostShell2))
                    return Bozja.LostShell2;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBubble) &&
                    HasActionEquipped(Bozja.LostBubble) &&
                    IsOffCooldown(Bozja.LostBubble) &&
                    !HasEffect(Bozja.Buffs.LostBubble))
                    return Bozja.LostBubble;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostParalyze3) &&
                    HasActionEquipped(Bozja.LostParalyze3) &&
                    IsOffCooldown(Bozja.LostParalyze3) &&
                    !JustUsed(Bozja.LostParalyze3, 60f))
                    return Bozja.LostParalyze3;
            }
            #endregion

            #region Rotation
            //Ranged Uptime
            if (LevelChecked(LightningShot) && //Lightning Shot is unlocked
                !InMeleeRange() && //Out of melee range
                HasBattleTarget()) //Has target
                return LightningShot; //Execute Lightning Shot if conditions are met

            //No Mercy
            if (ActionReady(NoMercy) && //No Mercy is ready
                InCombat() && //In combat
                HasTarget() && //Has target
                CanWeave()) //Able to weave
            {
                if (LevelChecked(DoubleDown)) //Lv90+
                {
                    if ((inOdd && //Odd Minute window
                         (Ammo >= 2 || (ComboAction is BrutalShell && Ammo == 1))) || //2 or 3 Ammo or 1 Ammo with Solid Barrel next in combo
                        (!inOdd && //Even Minute window
                         Ammo != 3)) //Ammo is not full (3)
                        return NoMercy; //Execute No Mercy if conditions are met
                }
                if (!LevelChecked(DoubleDown)) //Lv1-89
                {
                    if (canLateWeave && //Late-weaveable
                        Ammo == MaxCartridges()) //Ammo is full
                        return NoMercy; //Execute No Mercy if conditions are met
                }
            }

            //Hypervelocity - Forced to prevent loss
            if (JustUsed(BurstStrike, 5f) && //Burst Strike was just used within 5 seconds
                LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                HasEffect(Buffs.ReadyToBlast) && //Ready To Blast buff is active
                nmCD is > 1 or <= 0.1f) //Priority hack to prevent Hypervelocity from being used before No Mercy
                return Hypervelocity; //Execute Hypervelocity if conditions are met

            //Continuation protection - Forced to prevent loss
            if (canContinue && //able to use Continuation
                (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                 HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                 HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                 HasEffect(Buffs.ReadyToBlast) || //after Burst Strike
                 HasEffect(Buffs.ReadyToRaze))) //after Fated Circle
                return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

            //oGCDs
            if (CanWeave())
            {
                //Bloodfest
                if (InCombat() && //In combat
                    HasTarget() && //Has target
                    canBF && //able to use Bloodfest
                    Ammo == 0) //Only when ammo is empty
                    return Bloodfest; //Execute Bloodfest if conditions are met

                //Zone
                if (canZone && //able to use Zone
                    (nmCD is < 57.5f and > 17f))//Optimal use; twice per minute, 1 in NM, 1 out of NM
                    return OriginalHook(DangerZone); //Execute Zone if conditions are met

                //Bow Shock
                if (canBow && //able to use Bow Shock
                    nmCD is < 57.5f and >= 40) //No Mercy is up & was not just used within 1 GCD
                    return BowShock;
            }

            //Lv90+ - every 3rd NM window
            if (LevelChecked(DoubleDown) &&
                HasEffect(Buffs.NoMercy) &&
                GunStep == 0 &&
                ComboAction is BrutalShell &&
                Ammo == 1)
                return SolidBarrel;

            //GnashingFang
            if (canGF && //able to use Gnashing Fang
                ((nmCD is > 17 and < 35) || //30s Optimal use
                 JustUsed(NoMercy, 6f))) //No Mercy was just used within 4 seconds
                return GnashingFang;

            //Double Down
            if (canDD && //able to use Double Down
                IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                hasNM) //No Mercy is active
                return DoubleDown;

            //Sonic Break
            if (canBreak && //able to use Sonic Break
                ((IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown)) || //Gnashing Fang and Double Down are both on cooldown
                 nmLeft <= GCD)) //No Mercy buff is about to expire
                return SonicBreak; //Execute Sonic Break if conditions are met

            //Reign of Beasts
            if (canReign && //able to use Reign of Beasts
                IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                IsOnCooldown(DoubleDown) && //Double Down is on cooldown
                !HasEffect(Buffs.ReadyToBreak) && //Ready To Break is not active
                GunStep == 0) //Gnashing Fang or Reign combo is not active or finished
                return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts if conditions are met

            //Burst Strike
            if (canBS && //able to use Burst Strike
                HasEffect(Buffs.NoMercy) && //No Mercy is active
                IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                IsOnCooldown(DoubleDown) && //Double Down is on cooldown
                !HasEffect(Buffs.ReadyToBreak) && //Ready To Break is not active
                !HasEffect(Buffs.ReadyToReign) && //Ready To Reign is not active
                GunStep == 0) //Gnashing Fang or Reign combo is not active or finished
                return BurstStrike; //Execute Burst Strike if conditions are met

            //Lv90+ 2cart forced Opener
            if (LevelChecked(DoubleDown) && //Lv90+
                nmCD < 1 && //No Mercy is ready or about to be
                Ammo is 3 && //Ammo is full
                bfCD > 110 && //Bloodfest was just used, but not recently
                ComboAction is KeenEdge) //Just used Keen Edge
                return BurstStrike;
            //Lv100 2cart forced 2min starter
            if (LevelChecked(ReignOfBeasts) && //Lv100
                nmCD < 1 && //No Mercy is ready or about to be
                 Ammo is 3 && //Ammo is full
                 bfCD < GCD * 12) //Bloodfest is ready or about to be
                return BurstStrike;

            //Gauge Combo Steps
            if (GunStep is 1 or 2) //Gnashing Fang combo is only for 1 and 2
                return OriginalHook(GnashingFang); //Execute Gnashing Fang combo if conditions are met
            if (GunStep is 3 or 4) //Reign of Beasts combo is only for 3 and 4
                return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts combo if conditions are met

            //123 (overcap included)
            if (ComboTimer > 0) //we're in combo
            {
                if (LevelChecked(BrutalShell) && //Brutal Shell is unlocked
                    ComboAction == KeenEdge) //just used first action in combo
                    return BrutalShell; //Execute Brutal Shell if conditions are met

                if (LevelChecked(SolidBarrel) && //Solid Barrel is unlocked
                    ComboAction == BrutalShell) //just used second action in combo
                {
                    //holds Hypervelocity if NM comes up in time
                    if (LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                        HasEffect(Buffs.ReadyToBlast) && //Ready To Blast buff is active
                        (nmCD is > 1 or <= 0.1f)) //Priority hack to prevent Hypervelocity from being used before No Mercy
                        return Hypervelocity; //Execute Hypervelocity if conditions are met

                    //Overcap protection
                    if (LevelChecked(BurstStrike) && //Burst Strike is unlocked
                        Ammo == MaxCartridges()) //Ammo is full relaive to level
                        return BurstStrike; //Execute Burst Strike if conditions are met

                    return SolidBarrel; //Execute Solid Barrel if conditions are met
                }
            }
            #endregion

            return KeenEdge; //Always default back to Keen Edge

        }
    }
    #endregion

    #region Advanced Mode - Single Target
    internal class GNB_ST_Advanced : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not KeenEdge)
                return actionID;

            #region Variables
            //Gauge
            byte Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
            byte GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
            //Cooldown-related
            float gfCD = GetCooldownRemainingTime(GnashingFang); //GnashingFang's cooldown; 30s total
            float nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
            float ddCD = GetCooldownRemainingTime(DoubleDown); //Double Down's cooldown; 60s total
            float bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
            float nmLeft = GetBuffRemainingTime(Buffs.NoMercy); //Remaining time for No Mercy buff (20s)
            bool hasNM = nmCD is >= 40 and <= 60; //Checks if No Mercy is active
            bool hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
            bool hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff
            //Misc
            bool inOdd = bfCD is < 90 and > 20; //Odd Minute
            bool canLateWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
            float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x
            int nmStop = PluginConfiguration.GetCustomIntValue(Config.GNB_ST_NoMercyStop);
            bool justMitted = JustUsed(OriginalHook(HeartOfStone), 4f) ||
                             JustUsed(OriginalHook(Nebula), 5f) ||
                             JustUsed(Camouflage, 5f) ||
                             JustUsed(All.Rampart, 5f) ||
                             JustUsed(Aurora, 5f) ||
                             JustUsed(Superbolide, 9f);
            #region Minimal Requirements
            //Ammo-relative
            bool canBS = LevelChecked(BurstStrike) && //Burst Strike is unlocked
                        Ammo > 0; //Has Ammo
            bool canGF = LevelChecked(GnashingFang) && //GnashingFang is unlocked
                        gfCD < 0.6f && //Gnashing Fang is off cooldown
                        !HasEffect(Buffs.ReadyToBlast) && //to ensure Hypervelocity is spent in case Burst Strike is used before Gnashing Fang
                        GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                        Ammo > 0; //Has Ammo
            bool canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                        ddCD < 0.6f && //Double Down is off cooldown
                        Ammo > 0; //Has Ammo
            bool canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                        bfCD < 0.6f; //Bloodfest is off cooldown
            //Cooldown-relative
            bool canZone = LevelChecked(DangerZone) && //Zone is unlocked
                          GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
            bool canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                           hasBreak; //No Mercy or Ready To Break is active
            bool canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                         GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
            bool canContinue = LevelChecked(Continuation); //Continuation is unlocked
            bool canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                           GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                           hasReign; //Ready To Reign is active
            #endregion
            #endregion

            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            #region Mitigations
            if (IsEnabled(CustomComboPreset.GNB_ST_Mitigation) && //Mitigation option is enabled
                InCombat() && //Player is in combat
                !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
            {
                //Superbolide
                if (IsEnabled(CustomComboPreset.GNB_ST_Superbolide) && //Superbolide option is enabled
                    ActionReady(Superbolide) && //Superbolide is ready
                    PlayerHealthPercentageHp() < Config.GNB_ST_Superbolide_Health && //Player's health is below selected threshold
                    (Config.GNB_ST_Superbolide_SubOption == 0 || //Superbolide is enabled for all targets
                    (TargetIsBoss() && Config.GNB_ST_Superbolide_SubOption == 1))) //Superbolide is enabled for bosses only
                    return Superbolide;

                if (IsPlayerTargeted()) //Player is being targeted by current target
                {
                    //Nebula
                    if (IsEnabled(CustomComboPreset.GNB_ST_Nebula) && //Nebula option is enabled
                        ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                        PlayerHealthPercentageHp() < Config.GNB_ST_Nebula_Health && //Player's health is below selected threshold
                        (Config.GNB_ST_Nebula_SubOption == 0 || //Nebula is enabled for all targets
                         (TargetIsBoss() && Config.GNB_ST_Nebula_SubOption == 1))) //Nebula is enabled for bosses only
                        return OriginalHook(Nebula);

                    //Rampart
                    if (IsEnabled(CustomComboPreset.GNB_ST_Rampart) && //Rampart option is enabled
                        ActionReady(All.Rampart) && //Rampart is ready
                        PlayerHealthPercentageHp() < Config.GNB_ST_Rampart_Health && //Player's health is below selected threshold
                        (Config.GNB_ST_Rampart_SubOption == 0 || //Rampart is enabled for all targets
                         (TargetIsBoss() && Config.GNB_ST_Rampart_SubOption == 1))) //Rampart is enabled for bosses only
                        return All.Rampart;

                    //Reprisal
                    if (IsEnabled(CustomComboPreset.GNB_ST_Reprisal) && //Reprisal option is enabled
                        ActionReady(All.Reprisal) && //Reprisal is ready
                        InActionRange(All.Reprisal) && //Player is in range of Reprisal
                        PlayerHealthPercentageHp() < Config.GNB_ST_Reprisal_Health && //Player's health is below selected threshold
                        (Config.GNB_ST_Reprisal_SubOption == 0 || //Reprisal is enabled for all targets
                         (TargetIsBoss() && Config.GNB_ST_Reprisal_SubOption == 1))) //Reprisal is enabled for bosses only
                        return All.Reprisal;

                    //Arms Length
                    if (IsEnabled(CustomComboPreset.GNB_ST_ArmsLength) && //Arms Length option is enabled
                        ActionReady(All.ArmsLength) && //Arms Length is ready
                        PlayerHealthPercentageHp() < Config.GNB_ST_ArmsLength_Health && //Player's health is below selected threshold
                        !InBossEncounter()) //Arms Length is enabled for bosses only
                        return All.ArmsLength;
                }

                //Camouflage
                if (IsEnabled(CustomComboPreset.GNB_ST_Camouflage) && //Camouflage option is enabled
                    ActionReady(Camouflage) && //Camouflage is ready
                    PlayerHealthPercentageHp() < Config.GNB_ST_Camouflage_Health && //Player's health is below selected threshold
                    (Config.GNB_ST_Camouflage_SubOption == 0 || //Camouflage is enabled for all targets
                     (TargetIsBoss() && Config.GNB_ST_Camouflage_SubOption == 1))) //Camouflage is enabled for bosses only
                    return Camouflage;

                //Corundum
                if (IsEnabled(CustomComboPreset.GNB_ST_Corundum) && //Corundum option is enabled
                    ActionReady(OriginalHook(HeartOfStone)) && //Corundum is ready
                    PlayerHealthPercentageHp() < Config.GNB_ST_Corundum_Health && //Player's health is below selected threshold
                    (Config.GNB_ST_Corundum_SubOption == 0 || //Corundum is enabled for all targets
                     (TargetIsBoss() && Config.GNB_ST_Corundum_SubOption == 1))) //Corundum is enabled for bosses only
                    return OriginalHook(HeartOfStone);

                //Aurora
                if (IsEnabled(CustomComboPreset.GNB_ST_Aurora) && //Aurora option is enabled
                    LevelChecked(Aurora) && //Aurora is unlocked
                    !(HasEffect(Buffs.Aurora) || TargetHasEffectAny(Buffs.Aurora)) && //Aurora is not already active on player or target
                    GetRemainingCharges(Aurora) > Config.GNB_ST_Aurora_Charges && //Aurora has more charges than set threshold
                    PlayerHealthPercentageHp() < Config.GNB_ST_Aurora_Health && //Player's health is below selected threshold
                    (Config.GNB_ST_Aurora_SubOption == 0 || //Aurora is enabled for all targets
                    (TargetIsBoss() && Config.GNB_ST_Aurora_SubOption == 1))) //Aurora is enabled for bosses only
                    return Aurora;
            }
            #endregion

            #region Variant
            //Variant Cure
            if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure)
                && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                return Variant.VariantCure;

            //Variant SpiritDart
            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
            if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                IsEnabled(Variant.VariantSpiritDart) &&
                CanWeave() &&
                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                return Variant.VariantSpiritDart;

            //Variant Ultimatum
            if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) &&
                IsEnabled(Variant.VariantUltimatum) &&
                CanWeave() &&
                ActionReady(Variant.VariantUltimatum))
                return Variant.VariantUltimatum;
            #endregion

            #region Bozja
            if (Bozja.IsInBozja) //Checks if we're inside Bozja instances
            {
                //oGCDs
                if (CanWeave())
                {
                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFocus) && //Lost Focus is enabled
                        GetBuffStacks(Bozja.Buffs.Boost) < 16) //Boost stacks are below 16
                        return Bozja.LostFocus;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFontOfPower) && //Lost Font of Power is enabled
                        IsOffCooldown(Bozja.LostFontOfPower)) //Lost Focus was not just used within 30 seconds
                        return Bozja.LostFontOfPower;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSlash) &&
                        IsOffCooldown(Bozja.LostSlash))
                        return Bozja.LostSlash;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfNobleEnds))
                    {
                        if (!IsEnabled(CustomComboPreset.GNB_Bozja_PowerEnds) &&
                            IsOffCooldown(Bozja.BannerOfNobleEnds))
                            return Bozja.BannerOfNobleEnds;
                        if (IsEnabled(CustomComboPreset.GNB_Bozja_PowerEnds) &&
                            IsOffCooldown(Bozja.BannerOfNobleEnds) &&
                            JustUsed(Bozja.LostFontOfPower, 5f))
                            return Bozja.BannerOfNobleEnds;
                    }

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfHonoredSacrifice))
                    {
                        if (!IsEnabled(CustomComboPreset.GNB_Bozja_PowerSacrifice) &&
                            IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                            return Bozja.BannerOfHonoredSacrifice;
                        if (IsEnabled(CustomComboPreset.GNB_Bozja_PowerSacrifice) &&
                            IsOffCooldown(Bozja.BannerOfHonoredSacrifice) &&
                            JustUsed(Bozja.LostFontOfPower, 5f))
                            return Bozja.BannerOfHonoredSacrifice;
                    }

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfHonedAcuity) &&
                        IsOffCooldown(Bozja.BannerOfHonedAcuity) &&
                        !HasEffect(Bozja.Buffs.BannerOfTranscendentFinesse))
                        return Bozja.BannerOfHonedAcuity;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFairTrade) &&
                        IsOffCooldown(Bozja.LostFairTrade))
                        return Bozja.LostFairTrade;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostAssassination) &&
                        IsOffCooldown(Bozja.LostAssassination))
                        return Bozja.LostAssassination;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostManawall) &&
                        IsOffCooldown(Bozja.LostManawall))
                        return Bozja.LostManawall;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfTirelessConviction) &&
                        IsOffCooldown(Bozja.BannerOfTirelessConviction) &&
                        !HasEffect(Bozja.Buffs.BannerOfUnyieldingDefense))
                        return Bozja.BannerOfTirelessConviction;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBloodRage) &&
                        IsOffCooldown(Bozja.LostBloodRage))
                        return Bozja.LostBloodRage;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfSolemnClarity) &&
                        IsOffCooldown(Bozja.BannerOfSolemnClarity) &&
                        !HasEffect(Bozja.Buffs.BannerOfLimitlessGrace))
                        return Bozja.BannerOfSolemnClarity;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure2) &&
                        IsOffCooldown(Bozja.LostCure2) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure2_Health)
                        return Bozja.LostCure2;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure4) &&
                        IsOffCooldown(Bozja.LostCure4) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure4_Health)
                        return Bozja.LostCure4;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostReflect) &&
                        IsOffCooldown(Bozja.LostReflect) &&
                        !HasEffect(Bozja.Buffs.LostReflect))
                        return Bozja.LostReflect;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostAethershield) &&
                        IsOffCooldown(Bozja.LostAethershield) &&
                        !HasEffect(Bozja.Buffs.LostAethershield) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostAethershield_Health)
                        return Bozja.LostAethershield;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSwift) &&
                        IsOffCooldown(Bozja.LostSwift) &&
                        !HasEffect(Bozja.Buffs.LostSwift))
                        return Bozja.LostSwift;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFontOfSkill) &&
                        IsOffCooldown(Bozja.LostFontOfSkill))
                        return Bozja.LostFontOfSkill;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostRampage) &&
                        IsOffCooldown(Bozja.LostRampage))
                        return Bozja.LostRampage;
                }

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostStealth) &&
                    !InCombat() &&
                    IsOffCooldown(Bozja.LostStealth))
                    return Bozja.LostStealth;

                //GCDs
                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostDeath) &&
                    IsOffCooldown(Bozja.LostDeath))
                    return Bozja.LostDeath;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure) &&
                    IsOffCooldown(Bozja.LostCure) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure_Health)
                    return Bozja.LostCure;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure3) &&
                    IsOffCooldown(Bozja.LostCure3) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure3_Health)
                    return Bozja.LostCure3;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostArise) &&
                    IsOffCooldown(Bozja.LostArise) &&
                    GetTargetHPPercent() == 0 &&
                    !HasEffect(All.Debuffs.Raise))
                    return Bozja.LostArise;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSacrifice) &&
                    IsOffCooldown(Bozja.LostSacrifice) &&
                    GetTargetHPPercent() == 0)
                    return Bozja.LostSacrifice;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostReraise) &&
                    IsOffCooldown(Bozja.LostReraise) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostReraise_Health)
                    return Bozja.LostReraise;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSpellforge) &&
                    IsOffCooldown(Bozja.LostSpellforge) &&
                    (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
                    return Bozja.LostSpellforge;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSteelsting) &&
                    IsOffCooldown(Bozja.LostSteelsting) &&
                    (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
                    return Bozja.LostSteelsting;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostProtect) &&
                    IsOffCooldown(Bozja.LostProtect) &&
                    !HasEffect(Bozja.Buffs.LostProtect))
                    return Bozja.LostProtect;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostShell) &&
                    IsOffCooldown(Bozja.LostShell) &&
                    !HasEffect(Bozja.Buffs.LostShell))
                    return Bozja.LostShell;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBravery) &&
                    IsOffCooldown(Bozja.LostBravery) &&
                    !HasEffect(Bozja.Buffs.LostBravery))
                    return Bozja.LostBravery;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostProtect2) &&
                    IsOffCooldown(Bozja.LostProtect2) &&
                    !HasEffect(Bozja.Buffs.LostProtect2))
                    return Bozja.LostProtect2;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostShell2) &&
                    IsOffCooldown(Bozja.LostShell2) &&
                    !HasEffect(Bozja.Buffs.LostShell2))
                    return Bozja.LostShell2;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBubble) &&
                    IsOffCooldown(Bozja.LostBubble) &&
                    !HasEffect(Bozja.Buffs.LostBubble))
                    return Bozja.LostBubble;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostParalyze3) &&
                    IsOffCooldown(Bozja.LostParalyze3) &&
                    !JustUsed(Bozja.LostParalyze3, 60f))
                    return Bozja.LostParalyze3;
            }
            #endregion

            #region Rotation
            //Ranged Uptime
            if (IsEnabled(CustomComboPreset.GNB_ST_RangedUptime) && //Ranged Uptime option is enabled
                LevelChecked(LightningShot) && //Lightning Shot is unlocked
                !InMeleeRange() && //Out of melee range
                HasBattleTarget()) //Has target
                return LightningShot; //Execute Lightning Shot if conditions are met

            //No Mercy
            if (IsEnabled(CustomComboPreset.GNB_ST_NoMercy) && //No Mercy option is enabled
                ActionReady(NoMercy) && //No Mercy is ready
                InCombat() && //In combat
                HasTarget() && //Has target
                CanWeave() && //Able to weave
                GetTargetHPPercent() >= nmStop) //target HP is above threshold
            {
                if (LevelChecked(DoubleDown)) //Lv90+
                {
                    if ((inOdd && //Odd Minute window
                        (Ammo >= 2 || (ComboAction is BrutalShell && Ammo == 1))) || //2 or 3 Ammo or 1 Ammo with Solid Barrel next in combo
                        (!inOdd && //Even Minute window
                        Ammo != 3)) //Ammo is not full (3)
                        return NoMercy; //Execute No Mercy if conditions are met
                }
                if (!LevelChecked(DoubleDown)) //Lv1-89
                {
                    if (canLateWeave && //Late-weaveable
                        Ammo == MaxCartridges()) //Ammo is full
                        return NoMercy; //Execute No Mercy if conditions are met
                }
            }

            //Hypervelocity - Forced to prevent loss
            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns) && //Cooldowns option is enabled
                IsEnabled(CustomComboPreset.GNB_ST_Continuation) && //Continuation option is enabled
                JustUsed(BurstStrike, 5f) && //Burst Strike was just used within 5 seconds
                LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                HasEffect(Buffs.ReadyToBlast) && //Ready To Blast buff is active
                IsEnabled(CustomComboPreset.GNB_ST_NoMercy) && //No Mercy option is enabled
                 nmCD is > 1 or <= 0.1f) //Priority hack to prevent Hypervelocity from being used before No Mercy
                return Hypervelocity; //Execute Hypervelocity if conditions are met

            //Continuation protection - Forced to prevent loss
            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns) && //Cooldowns option is enabled
                IsEnabled(CustomComboPreset.GNB_ST_Continuation) && //Continuation option is enabled
                canContinue && //able to use Continuation
                (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                 HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                 HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                 HasEffect(Buffs.ReadyToBlast) || //after Burst Strike
                 HasEffect(Buffs.ReadyToRaze))) //after Fated Circle
                return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

            //oGCDs
            if (CanWeave())
            {
                //Cooldowns
                if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns)) //Cooldowns option is enabled
                {
                    //Bloodfest
                    if (IsEnabled(CustomComboPreset.GNB_ST_Bloodfest) && //Bloodfest option is enabled
                        InCombat() && //In combat
                        HasTarget() && //Has target
                        canBF && //able to use Bloodfest
                        Ammo == 0) //Only when ammo is empty
                        return Bloodfest; //Execute Bloodfest if conditions are met

                    //Zone
                    if (IsEnabled(CustomComboPreset.GNB_ST_Zone) && //Zone option is enabled
                        canZone && //able to use Zone
                        (nmCD is < 57.5f and > 17f))//Optimal use; twice per minute, 1 in NM, 1 out of NM
                        return OriginalHook(DangerZone); //Execute Zone if conditions are met

                    //Bow Shock
                    if (IsEnabled(CustomComboPreset.GNB_ST_BowShock) && //Bow Shock option is enabled
                        canBow && //able to use Bow Shock
                        nmCD is < 57.5f and >= 40) //No Mercy is up & was not just used within 1 GCD
                        return BowShock;
                }
            }

            //Lv90+ - every 3rd NM window
            if (LevelChecked(DoubleDown) &&
                HasEffect(Buffs.NoMercy) &&
                GunStep == 0 &&
                ComboAction is BrutalShell &&
                Ammo == 1)
                return SolidBarrel;

            //GCDs
            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns)) //Cooldowns option is enabled
            {
                //GnashingFang
                if (IsEnabled(CustomComboPreset.GNB_ST_GnashingFang) && //Gnashing Fang option is enabled
                    canGF && //able to use Gnashing Fang
                    ((nmCD is > 17 and < 35) || //30s Optimal use
                     JustUsed(NoMercy, 6f))) //No Mercy was just used within 4 seconds
                    return GnashingFang;

                //Double Down
                if (IsEnabled(CustomComboPreset.GNB_ST_DoubleDown) && //Double Down option is enabled
                    canDD && //able to use Double Down
                    IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                    hasNM) //No Mercy is active
                    return DoubleDown;

                //Sonic Break
                if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) && //Sonic Break option is enabled
                    canBreak && //able to use Sonic Break
                    ((IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown)) || //Gnashing Fang and Double Down are both on cooldown
                     nmLeft <= GCD)) //No Mercy buff is about to expire
                    return SonicBreak; //Execute Sonic Break if conditions are met

                //Reign of Beasts
                if (IsEnabled(CustomComboPreset.GNB_ST_Reign) && //Reign of Beasts option is enabled
                    canReign && //able to use Reign of Beasts
                    IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                    IsOnCooldown(DoubleDown) && //Double Down is on cooldown
                    !HasEffect(Buffs.ReadyToBreak) && //Ready To Break is not active
                    GunStep == 0) //Gnashing Fang or Reign combo is not active or finished
                    return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts if conditions are met

                //Burst Strike
                if (IsEnabled(CustomComboPreset.GNB_ST_BurstStrike) && //Burst Strike option is enabled
                    canBS && //able to use Burst Strike
                    HasEffect(Buffs.NoMercy) && //No Mercy is active
                    IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                    IsOnCooldown(DoubleDown) && //Double Down is on cooldown
                    !HasEffect(Buffs.ReadyToBreak) && //Ready To Break is not active
                    !HasEffect(Buffs.ReadyToReign) && //Ready To Reign is not active
                    GunStep == 0) //Gnashing Fang or Reign combo is not active or finished
                    return BurstStrike; //Execute Burst Strike if conditions are met
            }

            //Lv90+ 2cart forced Opener
            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns) && //Cooldowns option is enabled
                IsEnabled(CustomComboPreset.GNB_ST_NoMercy) && //No Mercy option is enabled
                IsEnabled(CustomComboPreset.GNB_ST_BurstStrike) && //Burst Strike option is enabled
                GetTargetHPPercent() > nmStop && //target HP is above threshold
                LevelChecked(DoubleDown) && //Lv90+
                nmCD < 1 && //No Mercy is ready or about to be
                 Ammo is 3 && //Ammo is full
                 bfCD > 110 && //Bloodfest was just used, but not recently
                 ComboAction is KeenEdge) //Just used Keen Edge
                return BurstStrike;
            //Lv100 2cart forced 2min starter
            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns) && //Cooldowns option is enabled
                IsEnabled(CustomComboPreset.GNB_ST_NoMercy) && //No Mercy option is enabled
                IsEnabled(CustomComboPreset.GNB_ST_BurstStrike) && //Burst Strike option is enabled
                GetTargetHPPercent() > nmStop && //target HP is above threshold
                LevelChecked(ReignOfBeasts) && //Lv100
                nmCD < 1 && //No Mercy is ready or about to be
                 Ammo is 3 && //Ammo is full
                 bfCD < GCD * 12) //Bloodfest is ready or about to be
                return BurstStrike;

            //Gauge Combo Steps
            if (IsEnabled(CustomComboPreset.GNB_ST_GnashingFang) && //Gnashing Fang option is enabled
                GunStep is 1 or 2) //Gnashing Fang combo is only for 1 and 2
                return OriginalHook(GnashingFang); //Execute Gnashing Fang combo if conditions are met
            if (IsEnabled(CustomComboPreset.GNB_ST_Reign) && //Reign of Beasts option is enabled
                GunStep is 3 or 4) //Reign of Beasts combo is only for 3 and 4
                return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts combo if conditions are met

            //123 (overcap included)
            if (ComboTimer > 0) //we're in combo
            {
                if (LevelChecked(BrutalShell) && //Brutal Shell is unlocked
                    ComboAction == KeenEdge) //just used first action in combo
                    return BrutalShell; //Execute Brutal Shell if conditions are met

                if (LevelChecked(SolidBarrel) && //Solid Barrel is unlocked
                    ComboAction == BrutalShell) //just used second action in combo
                {
                    //holds Hypervelocity if NM comes up in time
                    if (IsEnabled(CustomComboPreset.GNB_ST_Continuation) && //Continuation option is enabled
                        IsEnabled(CustomComboPreset.GNB_ST_NoMercy) && //No Mercy option is enabled
                        LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                        HasEffect(Buffs.ReadyToBlast) && //Ready To Blast buff is active
                        (nmCD is > 1 or <= 0.1f || //Priority hack to prevent Hypervelocity from being used before No Mercy
                         GetTargetHPPercent() < nmStop)) //target HP is below threshold
                        return Hypervelocity; //Execute Hypervelocity if conditions are met

                    //Overcap protection
                    if (IsEnabled(CustomComboPreset.GNB_ST_Overcap) && //Overcap option is enabled
                        LevelChecked(BurstStrike) && //Burst Strike is unlocked
                        Ammo == MaxCartridges()) //Ammo is full relaive to level
                        return BurstStrike; //Execute Burst Strike if conditions are met

                    return SolidBarrel; //Execute Solid Barrel if conditions are met
                }
            }
            #endregion

            return KeenEdge; //Always default back to Keen Edge

        }
    }
    #endregion

    #region Simple Mode - AoE
    internal class GNB_AoE_Simple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DemonSlice)
                return actionID;

            #region Variables
            //Gauge
            byte Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
            byte GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
            //Cooldown-related
            float nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
            float bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
            bool hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
            bool hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff
            bool justMitted = JustUsed(OriginalHook(HeartOfStone), 4f) ||
                              JustUsed(OriginalHook(Nebula), 5f) ||
                              JustUsed(Camouflage, 5f) ||
                              JustUsed(All.Rampart, 5f) ||
                              JustUsed(Aurora, 5f) ||
                              JustUsed(Superbolide, 9f);
            #region Minimal Requirements
            //Ammo-relative
            bool canFC = LevelChecked(FatedCircle) && //Fated Circle is unlocked
                        Ammo > 0; //Has Ammo
            bool canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                        GetCooldownRemainingTime(DoubleDown) < 0.6f && //Double Down is off cooldown
                        Ammo > 0; //Has Ammo
            bool canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                        GetCooldownRemainingTime(Bloodfest) < 0.6f; //Bloodfest is off cooldown
            //Cooldown-relative
            bool canZone = LevelChecked(DangerZone) && //Zone is unlocked
                          GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
            bool canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                           hasBreak; //No Mercy or Ready To Break is active
            bool canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                         GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
            bool canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                           GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                           hasReign; //Ready To Reign is active
            #endregion
            #endregion

            #region Mitigations
            if (Config.GNB_AoE_MitsOptions != 1)
            {
                if (InCombat() && //Player is in combat
                    !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                {
                    //Superbolide
                    if (ActionReady(Superbolide) && //Superbolide is ready
                        PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                        return Superbolide;

                    if (IsPlayerTargeted())
                    {
                        //Nebula
                        if (ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Nebula);

                        //Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        //Reprisal
                        if (ActionReady(All.Reprisal) && //Reprisal is ready
                            InActionRange(All.Reprisal) && //Player is in range of Reprisal
                            PlayerHealthPercentageHp() < 90) //Player's health is below 90%
                            return All.Reprisal;
                    }

                    //Camouflage
                    if (ActionReady(Camouflage) && //Camouflage is ready
                        PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                        return Camouflage;

                    //Corundum
                    if (ActionReady(OriginalHook(HeartOfStone)) && //Corundum
                        PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                        return OriginalHook(HeartOfStone);

                    //Aurora
                    if (LevelChecked(Aurora) && //Aurora is unlocked
                        !(HasEffect(Buffs.Aurora) || TargetHasEffectAny(Buffs.Aurora)) && //Aurora is not active on self or target
                        PlayerHealthPercentageHp() < 85) //Player's health is below 85%
                        return Aurora;
                }
            }
            #endregion

            #region Variant
            //Variant Cure
            if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) &&
                IsEnabled(Variant.VariantCure)
                && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                return Variant.VariantCure;

            //Variant SpiritDart
            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
            if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                IsEnabled(Variant.VariantSpiritDart) &&
                CanWeave() &&
                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                return Variant.VariantSpiritDart;

            //Variant Ultimatum
            if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) &&
                IsEnabled(Variant.VariantUltimatum) &&
                CanWeave() &&
                ActionReady(Variant.VariantUltimatum))
                return Variant.VariantUltimatum;
            #endregion

            #region Bozja
            if (Bozja.IsInBozja) //Checks if we're inside Bozja instances
            {
                //oGCDs
                if (CanWeave())
                {
                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFocus) && //Lost Focus is enabled
                        GetBuffStacks(Bozja.Buffs.Boost) < 16) //Boost stacks are below 16
                        return Bozja.LostFocus;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFontOfPower) && //Lost Font of Power is enabled
                        IsOffCooldown(Bozja.LostFontOfPower)) //Lost Focus was not just used within 30 seconds
                        return Bozja.LostFontOfPower;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSlash) &&
                        IsOffCooldown(Bozja.LostSlash))
                        return Bozja.LostSlash;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfNobleEnds))
                    {
                        if (!IsEnabled(CustomComboPreset.GNB_Bozja_PowerEnds) &&
                            IsOffCooldown(Bozja.BannerOfNobleEnds))
                            return Bozja.BannerOfNobleEnds;
                        if (IsEnabled(CustomComboPreset.GNB_Bozja_PowerEnds) &&
                            IsOffCooldown(Bozja.BannerOfNobleEnds) &&
                            JustUsed(Bozja.LostFontOfPower, 5f))
                            return Bozja.BannerOfNobleEnds;
                    }

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfHonoredSacrifice))
                    {
                        if (!IsEnabled(CustomComboPreset.GNB_Bozja_PowerSacrifice) &&
                            IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                            return Bozja.BannerOfHonoredSacrifice;
                        if (IsEnabled(CustomComboPreset.GNB_Bozja_PowerSacrifice) &&
                            IsOffCooldown(Bozja.BannerOfHonoredSacrifice) &&
                            JustUsed(Bozja.LostFontOfPower, 5f))
                            return Bozja.BannerOfHonoredSacrifice;
                    }

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfHonedAcuity) &&
                        IsOffCooldown(Bozja.BannerOfHonedAcuity) &&
                        !HasEffect(Bozja.Buffs.BannerOfTranscendentFinesse))
                        return Bozja.BannerOfHonedAcuity;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFairTrade) &&
                        IsOffCooldown(Bozja.LostFairTrade))
                        return Bozja.LostFairTrade;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostAssassination) &&
                        IsOffCooldown(Bozja.LostAssassination))
                        return Bozja.LostAssassination;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostManawall) &&
                        IsOffCooldown(Bozja.LostManawall))
                        return Bozja.LostManawall;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfTirelessConviction) &&
                        IsOffCooldown(Bozja.BannerOfTirelessConviction) &&
                        !HasEffect(Bozja.Buffs.BannerOfUnyieldingDefense))
                        return Bozja.BannerOfTirelessConviction;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBloodRage) &&
                        IsOffCooldown(Bozja.LostBloodRage))
                        return Bozja.LostBloodRage;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfSolemnClarity) &&
                        IsOffCooldown(Bozja.BannerOfSolemnClarity) &&
                        !HasEffect(Bozja.Buffs.BannerOfLimitlessGrace))
                        return Bozja.BannerOfSolemnClarity;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure2) &&
                        IsOffCooldown(Bozja.LostCure2) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure2_Health)
                        return Bozja.LostCure2;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure4) &&
                        IsOffCooldown(Bozja.LostCure4) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure4_Health)
                        return Bozja.LostCure4;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostReflect) &&
                        IsOffCooldown(Bozja.LostReflect) &&
                        !HasEffect(Bozja.Buffs.LostReflect))
                        return Bozja.LostReflect;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostAethershield) &&
                        IsOffCooldown(Bozja.LostAethershield) &&
                        !HasEffect(Bozja.Buffs.LostAethershield) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostAethershield_Health)
                        return Bozja.LostAethershield;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSwift) &&
                        IsOffCooldown(Bozja.LostSwift) &&
                        !HasEffect(Bozja.Buffs.LostSwift))
                        return Bozja.LostSwift;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFontOfSkill) &&
                        IsOffCooldown(Bozja.LostFontOfSkill))
                        return Bozja.LostFontOfSkill;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostRampage) &&
                        IsOffCooldown(Bozja.LostRampage))
                        return Bozja.LostRampage;
                }

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostStealth) &&
                    !InCombat() &&
                    IsOffCooldown(Bozja.LostStealth))
                    return Bozja.LostStealth;

                //GCDs
                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostDeath) &&
                    IsOffCooldown(Bozja.LostDeath))
                    return Bozja.LostDeath;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure) &&
                    IsOffCooldown(Bozja.LostCure) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure_Health)
                    return Bozja.LostCure;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure3) &&
                    IsOffCooldown(Bozja.LostCure3) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure3_Health)
                    return Bozja.LostCure3;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostArise) &&
                    IsOffCooldown(Bozja.LostArise) &&
                    GetTargetHPPercent() == 0 &&
                    !HasEffect(All.Debuffs.Raise))
                    return Bozja.LostArise;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSacrifice) &&
                    IsOffCooldown(Bozja.LostSacrifice) &&
                    GetTargetHPPercent() == 0)
                    return Bozja.LostSacrifice;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostReraise) &&
                    IsOffCooldown(Bozja.LostReraise) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostReraise_Health)
                    return Bozja.LostReraise;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSpellforge) &&
                    IsOffCooldown(Bozja.LostSpellforge) &&
                    (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
                    return Bozja.LostSpellforge;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSteelsting) &&
                    IsOffCooldown(Bozja.LostSteelsting) &&
                    (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
                    return Bozja.LostSteelsting;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostProtect) &&
                    IsOffCooldown(Bozja.LostProtect) &&
                    !HasEffect(Bozja.Buffs.LostProtect))
                    return Bozja.LostProtect;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostShell) &&
                    IsOffCooldown(Bozja.LostShell) &&
                    !HasEffect(Bozja.Buffs.LostShell))
                    return Bozja.LostShell;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBravery) &&
                    IsOffCooldown(Bozja.LostBravery) &&
                    !HasEffect(Bozja.Buffs.LostBravery))
                    return Bozja.LostBravery;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostProtect2) &&
                    IsOffCooldown(Bozja.LostProtect2) &&
                    !HasEffect(Bozja.Buffs.LostProtect2))
                    return Bozja.LostProtect2;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostShell2) &&
                    IsOffCooldown(Bozja.LostShell2) &&
                    !HasEffect(Bozja.Buffs.LostShell2))
                    return Bozja.LostShell2;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBubble) &&
                    IsOffCooldown(Bozja.LostBubble) &&
                    !HasEffect(Bozja.Buffs.LostBubble))
                    return Bozja.LostBubble;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostParalyze3) &&
                    IsOffCooldown(Bozja.LostParalyze3) &&
                    !JustUsed(Bozja.LostParalyze3, 60f))
                    return Bozja.LostParalyze3;
            }
            #endregion

            #region Rotation
            if (InCombat()) //if already in combat
            {
                if (CanWeave()) //if we can weave
                {
                    //NoMercy
                    if (ActionReady(NoMercy) && //if No Mercy is ready
                        GetTargetHPPercent() > 5) //if target HP is above threshold
                        return NoMercy; //execute No Mercy
                    //BowShock
                    if (canBow && //if Bow Shock is ready
                        HasEffect(Buffs.NoMercy)) //if No Mercy is active
                        return BowShock; //execute Bow Shock
                    //Zone
                    if (canZone &&
                        nmCD is < 57.5f and > 17) //use on CD after first usage in NM
                        return OriginalHook(DangerZone); //execute Zone
                    //Bloodfest
                    if (canBF) //if Bloodfest is ready & gauge is empty
                        return Bloodfest; //execute Bloodfest
                    //Continuation
                    if (LevelChecked(FatedBrand) && //if Fated Brand is unlocked
                        HasEffect(Buffs.ReadyToRaze)) //if Ready To Raze is active
                        return FatedBrand; //execute Fated Brand
                }

                //SonicBreak
                if (canBreak && //if Ready To Break is active & unlocked
                    !HasEffect(Buffs.ReadyToRaze) && //if Ready To Raze is not active
                    HasEffect(Buffs.NoMercy)) //if No Mercy is active
                    return SonicBreak;
                //DoubleDown
                if (canDD && //if Double Down is ready && gauge is not empty
                    HasEffect(Buffs.NoMercy)) //if No Mercy is active
                    return DoubleDown; //execute Double Down
                //Reign - because leaving this out anywhere is a waste
                if (LevelChecked(ReignOfBeasts)) //if Reign of Beasts is unlocked
                {
                    if (canReign || //can execute Reign of Beasts
                        (GunStep is 3 or 4)) //can execute Noble Blood or Lion Heart
                        return OriginalHook(ReignOfBeasts);
                }
                //FatedCircle - if not LevelChecked, use BurstStrike
                if (canFC && //if Fated Circle is unlocked && gauge is not empty
                             //Normal
                    ((HasEffect(Buffs.NoMercy) && //if No Mercy is active
                      !ActionReady(DoubleDown) && //if Double Down is not ready
                      GunStep == 0) || //if Gnashing Fang or Reign combo is not active
                                       //Bloodfest prep
                     (IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && //if Bloodfest option is enabled
                      bfCD < 6))) //if Bloodfest is about to be ready
                    return FatedCircle;
                if (Ammo > 0 && //if gauge is not empty
                    !LevelChecked(FatedCircle) && //if Fated Circle is not unlocked
                    LevelChecked(BurstStrike) && //if Burst Strike is unlocked
                    HasEffect(Buffs.NoMercy) && //if No Mercy is active
                     GunStep == 0) //if Gnashing Fang or Reign combo is not active
                    return BurstStrike;
            }

            //1-2
            if (ComboTimer > 0) //if we're in combo
            {
                if (ComboAction == DemonSlice && //if last action was Demon Slice
                    LevelChecked(DemonSlaughter)) //if Demon Slaughter is unlocked
                {
                    if (Ammo == MaxCartridges())
                    {
                        if (LevelChecked(FatedCircle)) //if Fated Circle is unlocked
                            return FatedCircle; //execute Fated Circle
                        if (!LevelChecked(FatedCircle)) //if Fated Circle is not unlocked
                            return BurstStrike; //execute Burst Strike
                    }
                    if (Ammo != MaxCartridges()) //if gauge is full && if Fated Circle is not unlocked
                        return DemonSlaughter; //execute Demon Slaughter
                }
            }
            #endregion

            return DemonSlice; //Always default back to Demon Slice

        }
    }
    #endregion

    #region Advanced Mode - AoE
    internal class GNB_AoE_Advanced : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DemonSlice)
                return actionID;

            #region Variables
            //Gauge
            byte Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
            byte GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
            //Cooldown-related
            float nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
            float bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
            bool hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
            bool hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff
            bool justMitted = JustUsed(OriginalHook(HeartOfStone), 4f) ||
                              JustUsed(OriginalHook(Nebula), 5f) ||
                              JustUsed(Camouflage, 5f) ||
                              JustUsed(All.Rampart, 5f) ||
                              JustUsed(Aurora, 5f) ||
                              JustUsed(Superbolide, 9f);
            //Misc
            int nmStop = PluginConfiguration.GetCustomIntValue(Config.GNB_AoE_NoMercyStop);
            #region Minimal Requirements
            //Ammo-relative
            bool canFC = LevelChecked(FatedCircle) && //Fated Circle is unlocked
                        Ammo > 0; //Has Ammo
            bool canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                        GetCooldownRemainingTime(DoubleDown) < 0.6f && //Double Down is off cooldown
                        Ammo > 0; //Has Ammo
            bool canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                        GetCooldownRemainingTime(Bloodfest) < 0.6f; //Bloodfest is off cooldown
            //Cooldown-relative
            bool canZone = LevelChecked(DangerZone) && //Zone is unlocked
                          GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
            bool canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                           hasBreak; //No Mercy or Ready To Break is active
            bool canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                         GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
            bool canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                           GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                           hasReign; //Ready To Reign is active
            #endregion
            #endregion

            #region Mitigations
            if (IsEnabled(CustomComboPreset.GNB_AoE_Mitigation) && //Mitigation option is enabled
                InCombat() && //Player is in combat
                !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
            {
                //Superbolide
                if (IsEnabled(CustomComboPreset.GNB_AoE_Superbolide) && //Superbolide option is enabled
                    ActionReady(Superbolide) && //Superbolide is ready
                    PlayerHealthPercentageHp() < Config.GNB_AoE_Superbolide_Health && //Player's health is below selected threshold
                    (Config.GNB_AoE_Superbolide_SubOption == 0 || //Superbolide is enabled for all targets
                     (TargetIsBoss() && Config.GNB_AoE_Superbolide_SubOption == 1))) //Superbolide is enabled for bosses only
                    return Superbolide;

                if (IsPlayerTargeted()) //Player is being targeted by current target
                {
                    //Nebula
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Nebula) && //Nebula option is enabled
                        ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                        PlayerHealthPercentageHp() < Config.GNB_AoE_Nebula_Health && //Player's health is below selected threshold
                        (Config.GNB_AoE_Nebula_SubOption == 0 || //Nebula is enabled for all targets
                         (TargetIsBoss() && Config.GNB_AoE_Nebula_SubOption == 1))) //Nebula is enabled for bosses only
                        return OriginalHook(Nebula);

                    //Rampart
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Rampart) && //Rampart option is enabled
                        ActionReady(All.Rampart) && //Rampart is ready
                        PlayerHealthPercentageHp() < Config.GNB_AoE_Rampart_Health && //Player's health is below selected threshold
                        (Config.GNB_AoE_Rampart_SubOption == 0 || //Rampart is enabled for all targets
                         (TargetIsBoss() && Config.GNB_AoE_Rampart_SubOption == 1))) //Rampart is enabled for bosses only
                        return All.Rampart;

                    //Reprisal
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Reprisal) && //Reprisal option is enabled
                        ActionReady(All.Reprisal) && //Reprisal is ready
                        InActionRange(All.Reprisal) && //Player is in range of Reprisal
                        PlayerHealthPercentageHp() < Config.GNB_AoE_Reprisal_Health && //Player's health is below selected threshold
                        (Config.GNB_AoE_Reprisal_SubOption == 0 || //Reprisal is enabled for all targets
                         (TargetIsBoss() && Config.GNB_AoE_Reprisal_SubOption == 1))) //Reprisal is enabled for bosses only
                        return All.Reprisal;

                    //Arm's Length
                    if (IsEnabled(CustomComboPreset.GNB_AoE_ArmsLength) && //Arm's Length option is enabled
                        ActionReady(All.ArmsLength) && //Arm's Length is ready
                        PlayerHealthPercentageHp() < Config.GNB_AoE_ArmsLength_Health && //Player's health is below selected threshold
                        !InBossEncounter()) //Arms Length is enabled for bosses only
                        return All.ArmsLength;
                }

                //Camouflage
                if (IsEnabled(CustomComboPreset.GNB_AoE_Camouflage) && //Camouflage option is enabled
                    ActionReady(Camouflage) && //Camouflage is ready
                    PlayerHealthPercentageHp() < Config.GNB_AoE_Camouflage_Health && //Player's health is below selected threshold
                    (Config.GNB_AoE_Camouflage_SubOption == 0 || //Camouflage is enabled for all targets
                     (TargetIsBoss() && Config.GNB_AoE_Camouflage_SubOption == 1))) //Camouflage is enabled for bosses only
                    return Camouflage;

                //Corundum
                if (IsEnabled(CustomComboPreset.GNB_AoE_Corundum) && //Corundum option is enabled
                    ActionReady(OriginalHook(HeartOfStone)) && //Corundum is ready
                    PlayerHealthPercentageHp() < Config.GNB_AoE_Corundum_Health && //Player's health is below selected threshold
                    (Config.GNB_AoE_Corundum_SubOption == 0 || //Corundum is enabled for all targets
                     (TargetIsBoss() && Config.GNB_AoE_Corundum_SubOption == 1))) //Corundum is enabled for bosses only
                    return OriginalHook(HeartOfStone);

                //Aurora
                if (IsEnabled(CustomComboPreset.GNB_AoE_Aurora) && //Aurora option is enabled
                    LevelChecked(Aurora) && //Aurora is unlocked
                    GetRemainingCharges(Aurora) > Config.GNB_AoE_Aurora_Charges && //Aurora has more charges than set threshold
                    !(HasEffect(Buffs.Aurora) || TargetHasEffectAny(Buffs.Aurora)) && //Aurora is not already active on player or target
                    PlayerHealthPercentageHp() < Config.GNB_AoE_Aurora_Health && //Player's health is below selected threshold
                    (Config.GNB_AoE_Aurora_SubOption == 0 || //Aurora is enabled for all targets
                    (TargetIsBoss() && Config.GNB_AoE_Aurora_SubOption == 1))) //Aurora is enabled for bosses only
                    return Aurora;
            }
            #endregion

            #region Variant
            //Variant Cure
            if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) &&
                IsEnabled(Variant.VariantCure)
                && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                return Variant.VariantCure;

            //Variant SpiritDart
            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
            if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                IsEnabled(Variant.VariantSpiritDart) &&
                CanWeave() &&
                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                return Variant.VariantSpiritDart;

            //Variant Ultimatum
            if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) &&
                IsEnabled(Variant.VariantUltimatum) &&
                CanWeave() &&
                ActionReady(Variant.VariantUltimatum))
                return Variant.VariantUltimatum;
            #endregion

            #region Bozja
            if (Bozja.IsInBozja) //Checks if we're inside Bozja instances
            {
                //oGCDs
                if (CanWeave())
                {
                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFocus) && //Lost Focus is enabled
                        GetBuffStacks(Bozja.Buffs.Boost) < 16) //Boost stacks are below 16
                        return Bozja.LostFocus;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFontOfPower) && //Lost Font of Power is enabled
                        IsOffCooldown(Bozja.LostFontOfPower)) //Lost Focus was not just used within 30 seconds
                        return Bozja.LostFontOfPower;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSlash) &&
                        IsOffCooldown(Bozja.LostSlash))
                        return Bozja.LostSlash;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfNobleEnds))
                    {
                        if (!IsEnabled(CustomComboPreset.GNB_Bozja_PowerEnds) &&
                            IsOffCooldown(Bozja.BannerOfNobleEnds))
                            return Bozja.BannerOfNobleEnds;
                        if (IsEnabled(CustomComboPreset.GNB_Bozja_PowerEnds) &&
                            IsOffCooldown(Bozja.BannerOfNobleEnds) &&
                            JustUsed(Bozja.LostFontOfPower, 5f))
                            return Bozja.BannerOfNobleEnds;
                    }

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfHonoredSacrifice))
                    {
                        if (!IsEnabled(CustomComboPreset.GNB_Bozja_PowerSacrifice) &&
                            IsOffCooldown(Bozja.BannerOfHonoredSacrifice))
                            return Bozja.BannerOfHonoredSacrifice;
                        if (IsEnabled(CustomComboPreset.GNB_Bozja_PowerSacrifice) &&
                            IsOffCooldown(Bozja.BannerOfHonoredSacrifice) &&
                            JustUsed(Bozja.LostFontOfPower, 5f))
                            return Bozja.BannerOfHonoredSacrifice;
                    }

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfHonedAcuity) &&
                        IsOffCooldown(Bozja.BannerOfHonedAcuity) &&
                        !HasEffect(Bozja.Buffs.BannerOfTranscendentFinesse))
                        return Bozja.BannerOfHonedAcuity;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFairTrade) &&
                        IsOffCooldown(Bozja.LostFairTrade))
                        return Bozja.LostFairTrade;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostAssassination) &&
                        IsOffCooldown(Bozja.LostAssassination))
                        return Bozja.LostAssassination;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostManawall) &&
                        IsOffCooldown(Bozja.LostManawall))
                        return Bozja.LostManawall;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfTirelessConviction) &&
                        IsOffCooldown(Bozja.BannerOfTirelessConviction) &&
                        !HasEffect(Bozja.Buffs.BannerOfUnyieldingDefense))
                        return Bozja.BannerOfTirelessConviction;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBloodRage) &&
                        IsOffCooldown(Bozja.LostBloodRage))
                        return Bozja.LostBloodRage;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_BannerOfSolemnClarity) &&
                        IsOffCooldown(Bozja.BannerOfSolemnClarity) &&
                        !HasEffect(Bozja.Buffs.BannerOfLimitlessGrace))
                        return Bozja.BannerOfSolemnClarity;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure2) &&
                        IsOffCooldown(Bozja.LostCure2) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure2_Health)
                        return Bozja.LostCure2;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure4) &&
                        IsOffCooldown(Bozja.LostCure4) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure4_Health)
                        return Bozja.LostCure4;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostReflect) &&
                        IsOffCooldown(Bozja.LostReflect) &&
                        !HasEffect(Bozja.Buffs.LostReflect))
                        return Bozja.LostReflect;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostAethershield) &&
                        IsOffCooldown(Bozja.LostAethershield) &&
                        !HasEffect(Bozja.Buffs.LostAethershield) &&
                        PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostAethershield_Health)
                        return Bozja.LostAethershield;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSwift) &&
                        IsOffCooldown(Bozja.LostSwift) &&
                        !HasEffect(Bozja.Buffs.LostSwift))
                        return Bozja.LostSwift;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostFontOfSkill) &&
                        IsOffCooldown(Bozja.LostFontOfSkill))
                        return Bozja.LostFontOfSkill;

                    if (IsEnabled(CustomComboPreset.GNB_Bozja_LostRampage) &&
                        IsOffCooldown(Bozja.LostRampage))
                        return Bozja.LostRampage;
                }

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostStealth) &&
                    !InCombat() &&
                    IsOffCooldown(Bozja.LostStealth))
                    return Bozja.LostStealth;

                //GCDs
                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostDeath) &&
                    IsOffCooldown(Bozja.LostDeath))
                    return Bozja.LostDeath;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure) &&
                    IsOffCooldown(Bozja.LostCure) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure_Health)
                    return Bozja.LostCure;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostCure3) &&
                    IsOffCooldown(Bozja.LostCure3) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostCure3_Health)
                    return Bozja.LostCure3;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostArise) &&
                    IsOffCooldown(Bozja.LostArise) &&
                    GetTargetHPPercent() == 0 &&
                    !HasEffect(All.Debuffs.Raise))
                    return Bozja.LostArise;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSacrifice) &&
                    IsOffCooldown(Bozja.LostSacrifice) &&
                    GetTargetHPPercent() == 0)
                    return Bozja.LostSacrifice;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostReraise) &&
                    IsOffCooldown(Bozja.LostReraise) &&
                    PlayerHealthPercentageHp() <= Config.GNB_Bozja_LostReraise_Health)
                    return Bozja.LostReraise;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSpellforge) &&
                    IsOffCooldown(Bozja.LostSpellforge) &&
                    (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
                    return Bozja.LostSpellforge;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostSteelsting) &&
                    IsOffCooldown(Bozja.LostSteelsting) &&
                    (!HasEffect(Bozja.Buffs.LostSpellforge) || !HasEffect(Bozja.Buffs.LostSteelsting)))
                    return Bozja.LostSteelsting;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostProtect) &&
                    IsOffCooldown(Bozja.LostProtect) &&
                    !HasEffect(Bozja.Buffs.LostProtect))
                    return Bozja.LostProtect;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostShell) &&
                    IsOffCooldown(Bozja.LostShell) &&
                    !HasEffect(Bozja.Buffs.LostShell))
                    return Bozja.LostShell;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBravery) &&
                    IsOffCooldown(Bozja.LostBravery) &&
                    !HasEffect(Bozja.Buffs.LostBravery))
                    return Bozja.LostBravery;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostProtect2) &&
                    IsOffCooldown(Bozja.LostProtect2) &&
                    !HasEffect(Bozja.Buffs.LostProtect2))
                    return Bozja.LostProtect2;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostShell2) &&
                    IsOffCooldown(Bozja.LostShell2) &&
                    !HasEffect(Bozja.Buffs.LostShell2))
                    return Bozja.LostShell2;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostBubble) &&
                    IsOffCooldown(Bozja.LostBubble) &&
                    !HasEffect(Bozja.Buffs.LostBubble))
                    return Bozja.LostBubble;

                if (IsEnabled(CustomComboPreset.GNB_Bozja_LostParalyze3) &&
                    IsOffCooldown(Bozja.LostParalyze3) &&
                    !JustUsed(Bozja.LostParalyze3, 60f))
                    return Bozja.LostParalyze3;
            }
            #endregion

            #region Rotation
            if (InCombat()) //if already in combat
            {
                if (CanWeave()) //if we can weave
                {
                    //NoMercy
                    if (IsEnabled(CustomComboPreset.GNB_AoE_NoMercy) && //if No Mercy option is enabled
                        ActionReady(NoMercy) && //if No Mercy is ready
                        GetTargetHPPercent() > nmStop) //if target HP is above threshold
                        return NoMercy; //execute No Mercy
                    //BowShock
                    if (IsEnabled(CustomComboPreset.GNB_AoE_BowShock) && //if Bow Shock option is enabled
                        canBow && //if Bow Shock is ready
                        HasEffect(Buffs.NoMercy)) //if No Mercy is active
                        return BowShock; //execute Bow Shock
                    //Zone
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Zone) &&
                        canZone &&
                        nmCD is < 57.5f and > 17) //use on CD after first usage in NM
                        return OriginalHook(DangerZone); //execute Zone
                    //Bloodfest
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && //if Bloodfest option is enabled
                        canBF) //if Bloodfest is ready & gauge is empty
                        return Bloodfest; //execute Bloodfest
                    //Continuation
                    if (LevelChecked(FatedBrand) && //if Fated Brand is unlocked
                        HasEffect(Buffs.ReadyToRaze)) //if Ready To Raze is active
                        return FatedBrand; //execute Fated Brand
                }

                //SonicBreak
                if (IsEnabled(CustomComboPreset.GNB_AoE_SonicBreak) && //if Sonic Break option is enabled
                    canBreak && //if Ready To Break is active & unlocked
                    !HasEffect(Buffs.ReadyToRaze) && //if Ready To Raze is not active
                    HasEffect(Buffs.NoMercy)) //if No Mercy is active
                    return SonicBreak;
                //DoubleDown
                if (IsEnabled(CustomComboPreset.GNB_AoE_DoubleDown) && //if Double Down option is enabled
                    canDD && //if Double Down is ready && gauge is not empty
                    HasEffect(Buffs.NoMercy)) //if No Mercy is active
                    return DoubleDown; //execute Double Down
                //Reign - because leaving this out anywhere is a waste
                if (IsEnabled(CustomComboPreset.GNB_AoE_Reign) && //if Reign of Beasts option is enabled
                    LevelChecked(ReignOfBeasts)) //if Reign of Beasts is unlocked
                {
                    if (canReign || //can execute Reign of Beasts
                        (GunStep is 3 or 4)) //can execute Noble Blood or Lion Heart
                        return OriginalHook(ReignOfBeasts);
                }
                //FatedCircle - if not LevelChecked, use BurstStrike
                if (IsEnabled(CustomComboPreset.GNB_AoE_FatedCircle) && //if Fated Circle option is enabled
                    canFC && //if Fated Circle is unlocked && gauge is not empty
                             //Normal
                    ((HasEffect(Buffs.NoMercy) && //if No Mercy is active
                      !ActionReady(DoubleDown) && //if Double Down is not ready
                      GunStep == 0) || //if Gnashing Fang or Reign combo is not active
                                       //Bloodfest prep
                     (IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && //if Bloodfest option is enabled
                      bfCD < 6))) //if Bloodfest is about to be ready
                    return FatedCircle;
                if (IsEnabled(CustomComboPreset.GNB_AoE_noFatedCircle) && //if Fated Circle Burst Strike option is disabled
                    Ammo > 0 && //if gauge is not empty
                    !LevelChecked(FatedCircle) && //if Fated Circle is not unlocked
                    LevelChecked(BurstStrike) && //if Burst Strike is unlocked
                    HasEffect(Buffs.NoMercy) && //if No Mercy is active
                     GunStep == 0) //if Gnashing Fang or Reign combo is not active
                    return BurstStrike;
            }

            //1-2
            if (ComboTimer > 0) //if we're in combo
            {
                if (ComboAction == DemonSlice && //if last action was Demon Slice
                    LevelChecked(DemonSlaughter)) //if Demon Slaughter is unlocked
                {
                    if (Ammo == MaxCartridges())
                    {
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Overcap) && //if Overcap option is enabled
                            LevelChecked(FatedCircle)) //if Fated Circle is unlocked
                            return FatedCircle; //execute Fated Circle
                        if (IsEnabled(CustomComboPreset.GNB_AoE_BSOvercap) && //if Burst Strike Overcap option is enabled
                            !LevelChecked(FatedCircle)) //if Fated Circle is not unlocked
                            return BurstStrike; //execute Burst Strike
                    }
                    if (Ammo != MaxCartridges() || //if gauge is not full
                        (Ammo == MaxCartridges() && //if gauge is full
                         !LevelChecked(FatedCircle) && //if Fated Circle is not unlocked
                         !IsEnabled(CustomComboPreset.GNB_AoE_BSOvercap))) //if Burst Strike Overcap option is disabled
                    {
                        return DemonSlaughter; //execute Demon Slaughter
                    }
                }
            }
            #endregion

            return DemonSlice; //execute Demon Slice

        }
    }
    #endregion

    #region Gnashing Fang Features
    internal class GNB_GF_Features : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_GF_Features;

        protected override uint Invoke(uint actionID)
        {
            bool GFchoice = Config.GNB_GF_Features_Choice == 0; //Gnashing Fang as button
            bool NMchoice = Config.GNB_GF_Features_Choice == 1; //No Mercy as button

            if ((GFchoice && actionID is not GnashingFang) ||
                (NMchoice && actionID is not NoMercy))
                return actionID;

            #region Variables
            //Gauge
            byte Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
            byte GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
            //Cooldown-related
            float gfCD = GetCooldownRemainingTime(GnashingFang); //GnashingFang's cooldown; 30s total
            float nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
            float ddCD = GetCooldownRemainingTime(DoubleDown); //Double Down's cooldown; 60s total
            float bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
            float nmLeft = GetBuffRemainingTime(Buffs.NoMercy); //Remaining time for No Mercy buff (20s)
            bool hasNM = nmCD is >= 40 and <= 60; //Checks if No Mercy is active
            bool hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
            bool hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff

            //Misc
            bool inOdd = bfCD is < 90 and > 20; //Odd Minute
            bool canLateWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
            float GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x
            #region Minimal Requirements
            //Ammo-relative
            bool canBS = LevelChecked(BurstStrike) && //Burst Strike is unlocked
                        Ammo > 0; //Has Ammo
            bool canGF = LevelChecked(GnashingFang) && //GnashingFang is unlocked
                        gfCD < 0.6f && //Gnashing Fang is off cooldown
                        !HasEffect(Buffs.ReadyToBlast) && //to ensure Hypervelocity is spent in case Burst Strike is used before Gnashing Fang
                        GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                        Ammo > 0; //Has Ammo
            bool canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                        ddCD < 0.6f && //Double Down is off cooldown
                        Ammo > 0; //Has Ammo
            bool canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                        bfCD < 0.6f; //Bloodfest is off cooldown
            //Cooldown-relative
            bool canZone = LevelChecked(DangerZone) && //Zone is unlocked
                          GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
            bool canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                           hasBreak; //No Mercy or Ready To Break is active
            bool canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                         GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
            bool canContinue = LevelChecked(Continuation); //Continuation is unlocked
            bool canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                           GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                           hasReign; //Ready To Reign is active
            #endregion
            #endregion

            //oGCDs
            if (CanWeave())
            {
                //No Mercy
                if (IsEnabled(CustomComboPreset.GNB_GF_NoMercy) && //No Mercy option is enabled
                    ActionReady(NoMercy) && //No Mercy is ready
                    InCombat() && //In combat
                    HasTarget() && //Has target
                    CanWeave()) //Able to weave
                {
                    if (LevelChecked(DoubleDown)) //Lv90+
                    {
                        if ((inOdd && //Odd Minute window
                             (Ammo >= 2 || (ComboAction is BrutalShell && Ammo == 1))) || //2 or 3 Ammo or 1 Ammo with Solid Barrel next in combo
                            (!inOdd && //Even Minute window
                             Ammo != 3)) //Ammo is not full (3)
                            return NoMercy; //Execute No Mercy if conditions are met
                    }
                    if (!LevelChecked(DoubleDown)) //Lv1-89
                    {
                        if (canLateWeave && //Late-weaveable
                            Ammo == MaxCartridges()) //Ammo is full
                            return NoMercy; //Execute No Mercy if conditions are met
                    }
                }

                //Cooldowns
                if (IsEnabled(CustomComboPreset.GNB_GF_Features)) //Features are enabled
                {
                    //Hypervelocity
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && //Continuation option is enabled
                        LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                        (JustUsed(BurstStrike, 1) || //Burst Strike was just used within 1 second
                         HasEffect(Buffs.ReadyToBlast))) //Ready To Blast buff is active
                        return Hypervelocity; //Execute Hypervelocity if conditions are met

                    //Continuation
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && //Continuation option is enabled
                        canContinue && //able to use Continuation
                        (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                         HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                         HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                         HasEffect(Buffs.ReadyToBlast))) //after Burst Strike
                        return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

                    //Bloodfest
                    if (IsEnabled(CustomComboPreset.GNB_GF_Bloodfest) && //Bloodfest option is enabled
                        InCombat() && //In combat
                        HasTarget() && //Has target
                        canBF && //able to use Bloodfest
                        Ammo == 0) //Only when ammo is empty
                        return Bloodfest; //Execute Bloodfest if conditions are met

                    //Zone
                    if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && //Zone option is enabled
                        canZone && //able to use Zone
                        (nmCD is < 57.5f and > 17f))//Optimal use; twice per minute, 1 in NM, 1 out of NM
                        return OriginalHook(DangerZone); //Execute Zone if conditions are met

                    //Bow Shock
                    if (IsEnabled(CustomComboPreset.GNB_GF_BowShock) && //Bow Shock option is enabled
                        canBow && //able to use Bow Shock
                        nmCD is < 57.5f and >= 40) //No Mercy is up & was not just used within 1 GCD
                        return BowShock;
                }
            }

            //GCDs
            if (IsEnabled(CustomComboPreset.GNB_GF_Features)) //Features are enabled
            {
                //GnashingFang
                if (canGF && //able to use Gnashing Fang
                    ((nmCD is > 17 and < 35) || //30s Optimal use
                     JustUsed(NoMercy, 6f))) //No Mercy was just used within 4 seconds
                    return GnashingFang;

                //Double Down
                if (IsEnabled(CustomComboPreset.GNB_GF_DoubleDown) && //Double Down option is enabled
                    canDD && //able to use Double Down
                    IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                    hasNM) //No Mercy is active
                    return DoubleDown;

                //Sonic Break
                if (IsEnabled(CustomComboPreset.GNB_GF_SonicBreak) && //Sonic Break option is enabled
                    canBreak && //able to use Sonic Break
                    ((IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown)) || //Gnashing Fang and Double Down are both on cooldown
                     nmLeft <= GCD)) //No Mercy buff is about to expire
                    return SonicBreak; //Execute Sonic Break if conditions are met

                //Reign of Beasts
                if (IsEnabled(CustomComboPreset.GNB_GF_Reign) && //Reign of Beasts option is enabled
                    canReign && //able to use Reign of Beasts
                    IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                    IsOnCooldown(DoubleDown) && //Double Down is on cooldown
                    !HasEffect(Buffs.ReadyToBreak) && //Ready To Break is not active
                    GunStep == 0) //Gnashing Fang or Reign combo is not active or finished
                    return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts if conditions are met

                //Burst Strike
                if (IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && //Burst Strike option is enabled
                    canBS && //able to use Burst Strike
                    HasEffect(Buffs.NoMercy) && //No Mercy is active
                    IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                    IsOnCooldown(DoubleDown) && //Double Down is on cooldown
                    !HasEffect(Buffs.ReadyToBreak) && //Ready To Break is not active
                    !HasEffect(Buffs.ReadyToReign) && //Ready To Reign is not active
                    GunStep == 0) //Gnashing Fang or Reign combo is not active or finished
                    return BurstStrike; //Execute Burst Strike if conditions are met
            }

            //Lv90+ 2cart forced Reopener
            if (IsEnabled(CustomComboPreset.GNB_GF_Features) && //Cooldowns option is enabled
                IsEnabled(CustomComboPreset.GNB_GF_NoMercy) && //No Mercy option is enabled
                IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && //Burst Strike option is enabled
                LevelChecked(DoubleDown) && //Lv90+
                nmCD < 1 && //No Mercy is ready or about to be
                Ammo is 3 && //Ammo is full
                bfCD > 110 && //Bloodfest was recently used, but not just used
                ComboAction is KeenEdge) //Just used Keen Edge
                return BurstStrike;
            //Lv100 2cart forced 2min starter
            if (IsEnabled(CustomComboPreset.GNB_GF_Features) && //Cooldowns option is enabled
                IsEnabled(CustomComboPreset.GNB_GF_NoMercy) && //No Mercy option is enabled
                IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && //Burst Strike option is enabled
                LevelChecked(ReignOfBeasts) && //Lv100
                nmCD < 1 && //No Mercy is ready or about to be
                 Ammo is 3 && //Ammo is full
                 bfCD < GCD * 12) //Bloodfest is ready or about to be
                return BurstStrike;

            //Gauge Combo Steps
            if (GunStep is 1 or 2) //Gnashing Fang combo is only for 1 and 2
                return OriginalHook(GnashingFang); //Execute Gnashing Fang combo if conditions are met
            if (GunStep is 3 or 4) //Reign of Beasts combo is only for 3 and 4
                return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts combo if conditions are met

            return actionID;
        }
    }
    #endregion

    #region Burst Strike Features
    internal class GNB_BS_Features : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_BS_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not BurstStrike)
                return actionID;

            #region Variables
            //Gauge
            byte Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
            byte GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
            //Cooldown-related
            float gfCD = GetCooldownRemainingTime(GnashingFang); //GnashingFang's cooldown; 30s total
            float ddCD = GetCooldownRemainingTime(DoubleDown); //Double Down's cooldown; 60s total
            float bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
            bool hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff

            #region Minimal Requirements
            //Ammo-relative
            bool canGF = LevelChecked(GnashingFang) && //GnashingFang is unlocked
                        gfCD < 0.6f && //Gnashing Fang is off cooldown
                        !HasEffect(Buffs.ReadyToBlast) && //to ensure Hypervelocity is spent in case Burst Strike is used before Gnashing Fang
                        GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                        Ammo > 0; //Has Ammo
            bool canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                        ddCD < 0.6f && //Double Down is off cooldown
                        Ammo > 0; //Has Ammo
            bool canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                        bfCD < 0.6f; //Bloodfest is off cooldown
            //Cooldown-relative
            bool canContinue = LevelChecked(Continuation); //Continuation is unlocked
            bool canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                           GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                           hasReign; //Ready To Reign is active
            #endregion
            #endregion

            //Hypervelocity
            if (IsEnabled(CustomComboPreset.GNB_BS_Continuation) && //Continuation option is enabled
                IsEnabled(CustomComboPreset.GNB_BS_Hypervelocity) && //Continuation option is enabled
                LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                (JustUsed(BurstStrike, 1) || //Burst Strike was just used within 1 second
                 HasEffect(Buffs.ReadyToBlast))) //Ready To Blast buff is active
                return Hypervelocity; //Execute Hypervelocity if conditions are met

            //Continuation
            if (IsEnabled(CustomComboPreset.GNB_BS_Continuation) && //Continuation option is enabled
                !IsEnabled(CustomComboPreset.GNB_BS_Hypervelocity) && //Hypervelocity Only option is disabled
                canContinue && //able to use Continuation
                (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                 HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                 HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                 HasEffect(Buffs.ReadyToBlast))) //after Burst Strike
                return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

            //Bloodfest
            if (IsEnabled(CustomComboPreset.GNB_BS_Bloodfest) && //Bloodfest option is enabled
                HasTarget() && //Has target
                canBF && //able to use Bloodfest
                Ammo == 0) //Only when ammo is empty
                return Bloodfest; //Execute Bloodfest if conditions are met

            //Double Down higher prio if only 1 cartridge
            if (IsEnabled(CustomComboPreset.GNB_BS_DoubleDown) && //Double Down option is enabled
                LevelChecked(DoubleDown) && //Double Down is unlocked
                ddCD < 0.6f && //Double Down is off cooldown
                Ammo == 1) //Has Ammo
                return DoubleDown; //Execute Double Down if conditions are met

            //Gnashing Fang
            if ((IsEnabled(CustomComboPreset.GNB_BS_GnashingFang) && //Gnashing Fang option is enabled
                canGF) || GunStep is 1 or 2) //able to use Gnashing Fang combo
                return OriginalHook(GnashingFang); //Execute Gnashing Fang if conditions are met

            //Double Down
            if (IsEnabled(CustomComboPreset.GNB_BS_DoubleDown) && //Double Down option is enabled
                canDD) //able to use Double Down
                return DoubleDown; //Execute Double Down if conditions are met

            //Reign
            if (IsEnabled(CustomComboPreset.GNB_BS_Reign) && //Reign of Beasts option is enabled
                (canReign || GunStep is 3 or 4)) //able to use Reign of Beasts
                return OriginalHook(ReignOfBeasts); //Execute Reign combo if conditions are met

            return actionID;
        }
    }
    #endregion

    #region Fated Circle Features
    internal class GNB_FC_Features : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_FC_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FatedCircle)
                return actionID;

            #region Variables
            //Gauge
            byte Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
            byte GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
            //Cooldown-related
            float nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
            float ddCD = GetCooldownRemainingTime(DoubleDown); //Double Down's cooldown; 60s total
            float bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
            bool hasNM = nmCD is >= 40 and <= 60; //Checks if No Mercy is active
            bool hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff
            #region Minimal Requirements
            //Ammo-relative
            bool canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                        ddCD < 0.6f && //Double Down is off cooldown
                        Ammo > 0; //Has Ammo
            bool canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                        bfCD < 0.6f; //Bloodfest is off cooldown
            //Cooldown-relative
            bool canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                         GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
            bool canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                           GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                           hasReign; //Ready To Reign is active
            #endregion
            #endregion

            //Fated Brand
            if (IsEnabled(CustomComboPreset.GNB_FC_Continuation) && //Continuation option is enabled
                HasEffect(Buffs.ReadyToRaze) &&
                LevelChecked(FatedBrand))
                return FatedBrand;

            //Double Down under NM only
            if (IsEnabled(CustomComboPreset.GNB_FC_DoubleDown) && //Double Down option is enabled
                IsEnabled(CustomComboPreset.GNB_FC_DoubleDown_NM) && //Double Down No Mercy option is enabled
                canDD && //able to use Double Down
                hasNM) //No Mercy is active
                return DoubleDown; //Execute Double Down if conditions are met

            //Bloodfest
            if (IsEnabled(CustomComboPreset.GNB_FC_Bloodfest) && //Bloodfest option is enabled
                HasTarget() && //Has target
                canBF && //able to use Bloodfest
                Ammo == 0) //Only when ammo is empty
                return Bloodfest; //Execute Bloodfest if conditions are met

            //Bow Shock
            if (IsEnabled(CustomComboPreset.GNB_FC_BowShock) && //Bow Shock option is enabled
                canBow) //able to use Bow Shock
                return BowShock; //Execute Bow Shock if conditions are met

            //Double Down
            if (IsEnabled(CustomComboPreset.GNB_FC_DoubleDown) && //Double Down option is enabled
                !IsEnabled(CustomComboPreset.GNB_FC_DoubleDown_NM) && //Double Down No Mercy option is disabled
                canDD) //able to use Double Down
                return DoubleDown; //Execute Double Down if conditions are met

            //Reign
            if (IsEnabled(CustomComboPreset.GNB_FC_Reign) && //Reign of Beasts option is enabled
                (canReign || GunStep is 3 or 4)) //able to use Reign of Beasts
                return OriginalHook(ReignOfBeasts); //Execute Reign combo if conditions are met

            return actionID;
        }
    }
    #endregion

    #region No Mercy Features
    internal class GNB_NM_Features : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_NM_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not NoMercy)
                return actionID;

            #region Variables
            //Gauge
            byte Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
            //Cooldown-related
            float nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
            float bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total

            #region Minimal Requirements
            //Ammo-relative
            bool canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                        bfCD < 0.6f; //Bloodfest is off cooldown
            //Cooldown-relative
            bool canZone = LevelChecked(DangerZone) && //Zone is unlocked
                          GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
            bool canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                         GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
            bool canContinue = LevelChecked(Continuation); //Continuation is unlocked
            #endregion
            #endregion

            //oGCDs
            if (Config.GNB_NM_Features_Weave == 0) //Weave option is enabled
            {
                if (CanWeave())
                {
                    //Continuation
                    if (IsEnabled(CustomComboPreset.GNB_NM_Continuation) && //Continuation option is enabled
                        canContinue && //able to use Continuation
                        (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                         HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                         HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                         HasEffect(Buffs.ReadyToBlast) || //after Burst Strike
                         HasEffect(Buffs.ReadyToRaze))) //after Fated Circle
                        return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

                    //Bloodfest
                    if (IsEnabled(CustomComboPreset.GNB_NM_Bloodfest) && //Bloodfest option is enabled
                        InCombat() && //In combat
                        HasTarget() && //Has target
                        canBF && //able to use Bloodfest
                        Ammo == 0) //Only when ammo is empty
                        return Bloodfest; //Execute Bloodfest if conditions are met

                    //Bow Shock
                    if (IsEnabled(CustomComboPreset.GNB_NM_BowShock) && //Bow Shock option is enabled
                        canBow && //able to use Bow Shock
                        nmCD is < 57.5f and >= 40) //No Mercy is up & was not just used within 1 GCD
                        return BowShock;

                    //Zone
                    if (IsEnabled(CustomComboPreset.GNB_NM_Zone) && //Zone option is enabled
                        canZone && //able to use Zone
                        (nmCD is < 57.5f and > 17f))//Optimal use; twice per minute, 1 in NM, 1 out of NM
                        return OriginalHook(DangerZone); //Execute Zone if conditions are met
                }
            }

            if (Config.GNB_NM_Features_Weave == 1) //Force option is enabled
            {
                //Continuation
                if (IsEnabled(CustomComboPreset.GNB_NM_Continuation) && //Continuation option is enabled
                    canContinue && //able to use Continuation
                    (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                     HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                     HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                     HasEffect(Buffs.ReadyToBlast) || //after Burst Strike
                     HasEffect(Buffs.ReadyToRaze))) //after Fated Circle
                    return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

                //Bloodfest
                if (IsEnabled(CustomComboPreset.GNB_NM_Bloodfest) && //Bloodfest option is enabled
                    InCombat() && //In combat
                    HasTarget() && //Has target
                    canBF && //able to use Bloodfest
                    Ammo == 0) //Only when ammo is empty
                    return Bloodfest; //Execute Bloodfest if conditions are met

                //Bow Shock
                if (IsEnabled(CustomComboPreset.GNB_NM_BowShock) && //Bow Shock option is enabled
                    canBow && //able to use Bow Shock
                    nmCD is < 57.5f and >= 40) //No Mercy is up & was not just used within 1 GCD
                    return BowShock;

                //Zone
                if (IsEnabled(CustomComboPreset.GNB_NM_Zone) && //Zone option is enabled
                    canZone && //able to use Zone
                    (nmCD is < 57.5f and > 17f))//Optimal use; twice per minute, 1 in NM, 1 out of NM
                    return OriginalHook(DangerZone); //Execute Zone if conditions are met
            }

            return actionID;
        }
    }
    #endregion

    #region Aurora Protection
    internal class GNB_AuroraProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AuroraProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Aurora)
                return actionID;
            if ((HasFriendlyTarget() && TargetHasEffectAny(Buffs.Aurora)) ||
                (!HasFriendlyTarget() && HasEffectAny(Buffs.Aurora)))
                return OriginalHook(11);
            return actionID;
        }
    }
    #endregion

    #region One-Button Mitigation
    internal class GNB_Mit_OneButton : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_Mit_OneButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Camouflage)
                return actionID;

            if (IsEnabled(CustomComboPreset.GNB_Mit_Superbolide_Max) &&
                ActionReady(Superbolide) &&
                PlayerHealthPercentageHp() <= Config.GNB_Mit_Superbolide_Health &&
                ContentCheck.IsInConfiguredContent(
                    Config.GNB_Mit_Superbolide_Difficulty,
                    Config.GNB_Mit_Superbolide_DifficultyListSet
                ))
                return Superbolide;

            foreach (int priority in Config.GNB_Mit_Priorities.Items.OrderBy(x => x))
            {
                int index = Config.GNB_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out uint action))
                    return action;
            }

            return actionID;
        }
    }
    #endregion

}
