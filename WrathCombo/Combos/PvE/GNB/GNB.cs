#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
#endregion

namespace WrathCombo.Combos.PvE
{
    internal partial class GNB //Job definitions
    {
        public const byte JobID = 37; //JobID value

        public const uint //Job abilities
        #region Offensive
            KeenEdge = 16137, //Lv1, instant, GCD, range 3, single-target, targets=hostile
            NoMercy = 16138, //Lv2, instant, 60.0s CD (group 10), range 0, single-target, targets=self
            BrutalShell = 16139, //Lv4, instant, GCD, range 3, single-target, targets=hostile
            DemonSlice = 16141, //Lv10, instant, GCD, range 0, AOE 5 circle, targets=self
            LightningShot = 16143, //Lv15, instant, GCD, range 20, single-target, targets=hostile
            DangerZone = 16144, //Lv18, instant, 30.0s CD (group 4), range 3, single-target, targets=hostile
            SolidBarrel = 16145, //Lv26, instant, GCD, range 3, single-target, targets=hostile
            BurstStrike = 16162, //Lv30, instant, GCD, range 3, single-target, targets=hostile
            DemonSlaughter = 16149, //Lv40, instant, GCD, range 0, AOE 5 circle, targets=self
            SonicBreak = 16153, //Lv54, instant, 60.0s CD (group 13/57), range 3, single-target, targets=hostile
            GnashingFang = 16146, //Lv60, instant, 30.0s CD (group 5/57), range 3, single-target, targets=hostile, animLock=0.700
            SavageClaw = 16147, //Lv60, instant, GCD, range 3, single-target, targets=hostile, animLock=0.500
            WickedTalon = 16150, //Lv60, instant, GCD, range 3, single-target, targets=hostile, animLock=0.770
            BowShock = 16159, //Lv62, instant, 60.0s CD (group 11), range 0, AOE 5 circle, targets=self
            AbdomenTear = 16157, //Lv70, instant, 1.0s CD (group 0), range 5, single-target, targets=hostile
            JugularRip = 16156, //Lv70, instant, 1.0s CD (group 0), range 5, single-target, targets=hostile
            EyeGouge = 16158, //Lv70, instant, 1.0s CD (group 0), range 5, single-target, targets=hostile
            Continuation = 16155, //Lv70, instant, 1.0s CD (group 0), range 0, single-target, targets=self, animLock=???
            FatedCircle = 16163, //Lv72, instant, GCD, range 0, AOE 5 circle, targets=self
            Bloodfest = 16164, //Lv76, instant, 120.0s CD (group 14), range 25, single-target, targets=hostile
            BlastingZone = 16165, //Lv80, instant, 30.0s CD (group 4), range 3, single-target, targets=hostile
            Hypervelocity = 25759, //Lv86, instant, 1.0s CD (group 0), range 5, single-target, targets=hostile
            DoubleDown = 25760, //Lv90, instant, 60.0s CD (group 12/57), range 0, AOE 5 circle, targets=self
            FatedBrand = 36936, //Lv96, instant, 1.0s CD, (group 0), range 5, AOE, targets=hostile
            ReignOfBeasts = 36937, //Lv100, instant, GCD, range 3, single-target, targets=hostile
            NobleBlood = 36938, //Lv100, instant, GCD, range 3, single-target, targets=hostile
            LionHeart = 36939, //Lv100, instant, GCD, range 3, single-target, targets=hostile

        #endregion

        #region Utility
            Camouflage = 16140, //Lv6, instant, 90.0s CD (group 15), range 0, single-target, targets=self
            RoyalGuard = 16142, //Lv10, instant, 2.0s CD (group 1), range 0, single-target, targets=self
            ReleaseRoyalGuard = 32068, //Lv10, instant, 1.0s CD (group 1), range 0, single-target, targets=self
            Nebula = 16148, //Lv38, instant, 120.0s CD (group 21), range 0, single-target, targets=self
            Aurora = 16151, //Lv45, instant, 60.0s CD (group 19/71), range 30, single-target, targets=self/party/alliance/friendly
            Superbolide = 16152, //Lv50, instant, 360.0s CD (group 24), range 0, single-target, targets=self
            HeartOfLight = 16160, //Lv64, instant, 90.0s CD (group 16), range 0, AOE 30 circle, targets=self
            HeartOfStone = 16161, //Lv68, instant, 25.0s CD (group 3), range 30, single-target, targets=self/party
            Trajectory = 36934, //Lv56, instant, 30.0s CD (group 9/70) (2? charges), range 20, single-target, targets=hostile
            HeartOfCorundum = 25758, //Lv82, instant, 25.0s CD (group 3), range 30, single-target, targets=self/party
            GreatNebula = 36935, //Lv92, instant, 120.0s CD, range 0, single-target, targeets=self
        #endregion

        #region Limit Break
            GunmetalSoul = 17105; //LB3, instant, range 0, AOE 50 circle, targets=self, animLock=3.860
        #endregion

        public static class Buffs
        {
            public const ushort
                    BrutalShell = 1898, //applied by Brutal Shell to self
                    NoMercy = 1831, //applied by No Mercy to self
                    ReadyToRip = 1842, //applied by Gnashing Fang to self
                    SonicBreak = 1837, //applied by Sonic Break to target
                    BowShock = 1838, //applied by Bow Shock to target
                    ReadyToTear = 1843, //applied by Savage Claw to self
                    ReadyToGouge = 1844, //applied by Wicked Talon to self
                    ReadyToBlast = 2686, //applied by Burst Strike to self
                    Nebula = 1834, //applied by Nebula to self
                    Rampart = 1191, //applied by Rampart to self
                    Camouflage = 1832, //applied by Camouflage to self
                    ArmsLength = 1209, //applied by Arm's Length to self
                    HeartOfLight = 1839, //applied by Heart of Light to self
                    Aurora = 1835, //applied by Aurora to self
                    Superbolide = 1836, //applied by Superbolide to self
                    HeartOfStone = 1840, // //applied by Heart of Stone to self
                    HeartOfCorundum = 2683, //applied by Heart of Corundum to self
                    ClarityOfCorundum = 2684, //applied by Heart of Corundum to self
                    CatharsisOfCorundum = 2685, //applied by Heart of Corundum to self
                    RoyalGuard = 1833, //applied by Royal Guard to self
                    GreatNebula = 3838, //applied by Nebula to self
                    ReadyToRaze = 3839, //applied by Fated Circle to self
                    ReadyToBreak = 3886, //applied by No mercy to self
                    ReadyToReign = 3840; //applied by Bloodfest to target
        }
        public static class Debuffs
        {
            public const ushort
                BowShock = 1838, //applied by Bow Shock to target
                SonicBreak = 1837; //applied by Sonic Break to target
        }
        public static int MaxCartridges(byte level) => level >= 88 ? 3 : 2; //Level Check helper for Maximum Ammo

        #region Simple Mode - Single Target
        internal class GNB_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_Simple;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge)
                {
                    #region Variables
                    //Gauge
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
                    //Cooldown-related
                    var gfCD = GetCooldownRemainingTime(GnashingFang); //GnashingFang's cooldown; 30s total
                    var nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
                    var ddCD = GetCooldownRemainingTime(DoubleDown); //Double Down's cooldown; 60s total
                    var bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
                    var nmLeft = GetBuffRemainingTime(Buffs.NoMercy); //Remaining time for No Mercy buff (20s)
                    var hasNM = nmCD is >= 40 and <= 60; //Checks if No Mercy is active
                    var hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
                    var hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff
                    //Mitigations
                    //Misc
                    var inOdd = bfCD is < 90 and > 20; //Odd Minute
                    var canLateWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
                    var GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x
                    bool justMitted = JustUsed(OriginalHook(HeartOfStone), 4f) ||
                                      JustUsed(OriginalHook(Nebula), 5f) ||
                                      JustUsed(Camouflage, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Aurora, 5f) ||
                                      JustUsed(Superbolide, 9f);

                    #region Minimal Requirements
                    //Ammo-relative
                    var canBS = LevelChecked(BurstStrike) && //Burst Strike is unlocked
                                Ammo > 0; //Has Ammo
                    var canGF = LevelChecked(GnashingFang) && //GnashingFang is unlocked
                                gfCD < 0.6f && //Gnashing Fang is off cooldown
                                !HasEffect(Buffs.ReadyToBlast) && //to ensure Hypervelocity is spent in case Burst Strike is used before Gnashing Fang
                                GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                                Ammo > 0; //Has Ammo
                    var canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                                ddCD < 0.6f && //Double Down is off cooldown
                                Ammo > 0; //Has Ammo
                    var canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                                bfCD < 0.6f; //Bloodfest is off cooldown
                    //Cooldown-relative
                    var canZone = LevelChecked(DangerZone) && //Zone is unlocked
                                GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
                    var canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                                hasBreak; //No Mercy or Ready To Break is active
                    var canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                                GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
                    var canContinue = LevelChecked(Continuation); //Continuation is unlocked
                    var canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                                GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                                hasReign; //Ready To Reign is active
                    #endregion
                    #endregion

                    // Mitigation
                    if (InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        // Superbolide
                        if (ActionReady(Superbolide) && //Superbolide is ready
                            PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                            return Superbolide;

                        // Nebula / Damnation
                        if (ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Nebula);

                        // Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        // Camouflage
                        if (ActionReady(Camouflage) && //Camouflage is ready
                            PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                            return Camouflage;

                        // Corundum
                        if (ActionReady(OriginalHook(HeartOfStone)) && //Corundum
                            PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                            return OriginalHook(HeartOfStone);
                    }

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure)
                        && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    //Ranged Uptime
                    if (LevelChecked(LightningShot) && //Lightning Shot is unlocked
                        !InMeleeRange() && //Out of melee range
                        HasBattleTarget()) //Has target
                        return LightningShot; //Execute Lightning Shot if conditions are met

                    //oGCDs
                    if (CanWeave(actionID))
                    {
                        //Variant SpiritDart
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        //Variant Ultimatum
                        if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        //Bloodfest
                        if (InCombat() && //In combat
                            HasTarget() && //Has target
                            canBF && //able to use Bloodfest
                            Ammo == 0) //Only when ammo is empty
                            return Bloodfest; //Execute Bloodfest if conditions are met

                        //No Mercy
                        if (ActionReady(NoMercy) && //No Mercy is ready
                            InCombat() && //In combat
                            HasTarget()) //Has target
                        {
                            if (LevelChecked(DoubleDown)) //Lv90+
                            {
                                if (IsOnCooldown(Bloodfest) &&
                                    Ammo != 3 &&
                                    lastComboMove is KeenEdge) //3 Ammo with Keen Edge for Opener
                                    return NoMercy; //Execute No Mercy if conditions are met
                                if ((inOdd && //Odd Minute window
                                    (Ammo == 2 || (lastComboMove is BrutalShell && Ammo == 1))) || //2 Ammo or 1 Ammo with Solid Barrel next in combo
                                    (!inOdd && //Even Minute window
                                    Ammo != 3)) //Ammo is not full (3)
                                    return NoMercy; //Execute No Mercy if conditions are met
                            }
                            if (!LevelChecked(DoubleDown)) //Lv1-89
                            {
                                if (canLateWeave && //Late-weaveable
                                    Ammo == MaxCartridges(level)) //Ammo is full
                                    return NoMercy; //Execute No Mercy if conditions are met
                            }
                        }

                        //Zone
                        if (canZone && //able to use Zone
                            (nmCD is < 57.5f and > 17f) && //Optimal use; twice per minute, 1 in NM, 1 out of NM
                            !JustUsed(NoMercy)) //No Mercy was not just used within 3 seconds
                            return OriginalHook(DangerZone); //Execute Zone if conditions are met

                        //Bow Shock
                        if (canBow && //able to use Bow Shock
                            (nmCD is < 57.5f and > 17f) && //Optimal use; twice per minute, 1 in NM, 1 out of NM
                            !JustUsed(NoMercy)) //No Mercy was not just used within 3 seconds)
                            return BowShock;
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

                    //TODO: code below is rather ass; refactor
                    //Lv90+ - every 3rd NM window
                    if (LevelChecked(DoubleDown) &&
                        HasEffect(Buffs.NoMercy) &&
                        GunStep == 0 &&
                        lastComboMove is BrutalShell &&
                        Ammo == 1)
                        return SolidBarrel;

                    //GnashingFang
                    if (canGF && //able to use Gnashing Fang
                        ((nmCD is > 17 and < 35) || //30s Optimal use
                        (JustUsed(NoMercy, 6f)))) //No Mercy was just used within 4 seconds
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

                    //Lv100 2cart forced 2min starter
                    if (LevelChecked(ReignOfBeasts) && //Lv100
                        (nmCD < 1 && //No Mercy is ready or about to be
                        Ammo is 3 && //Ammo is full
                        bfCD < GCD * 12)) //Bloodfest is ready or about to be
                        return BurstStrike;

                    //Gauge Combo Steps
                    if (GunStep is 1 or 2) //Gnashing Fang combo is only for 1 and 2
                        return OriginalHook(GnashingFang); //Execute Gnashing Fang combo if conditions are met
                    if (GunStep is 3 or 4) //Reign of Beasts combo is only for 3 and 4
                        return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts combo if conditions are met

                    //123 (overcap included)
                    if (comboTime > 0) //we're in combo
                    {
                        if (LevelChecked(BrutalShell) && //Brutal Shell is unlocked
                            lastComboMove == KeenEdge) //just used first action in combo
                            return BrutalShell; //Execute Brutal Shell if conditions are met

                        if (LevelChecked(SolidBarrel) && //Solid Barrel is unlocked
                            lastComboMove == BrutalShell) //just used second action in combo
                        {
                            //holds Hypervelocity if NM comes up in time
                            if (LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                                HasEffect(Buffs.ReadyToBlast) && //Ready To Blast buff is active
                                (nmCD is > 1 or <= 0.1f)) //Priority hack to prevent Hypervelocity from being used before No Mercy
                                return Hypervelocity; //Execute Hypervelocity if conditions are met

                            //Overcap protection
                            if (LevelChecked(BurstStrike) && //Burst Strike is unlocked
                                Ammo == MaxCartridges(level)) //Ammo is full relaive to level
                                return BurstStrike; //Execute Burst Strike if conditions are met

                            return SolidBarrel; //Execute Solid Barrel if conditions are met
                        }
                    }

                    return KeenEdge;
                }

                return actionID;
            }
        }
        #endregion

        #region Advanced Mode - Single Target
        internal class GNB_STMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge)
                {
                    #region Variables
                    //Gauge
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
                    //Cooldown-related
                    var gfCD = GetCooldownRemainingTime(GnashingFang); //GnashingFang's cooldown; 30s total
                    var nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
                    var ddCD = GetCooldownRemainingTime(DoubleDown); //Double Down's cooldown; 60s total
                    var bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
                    var nmLeft = GetBuffRemainingTime(Buffs.NoMercy); //Remaining time for No Mercy buff (20s)
                    var hasNM = nmCD is >= 40 and <= 60; //Checks if No Mercy is active
                    var hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
                    var hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff
                    //Misc
                    var inOdd = bfCD is < 90 and > 20; //Odd Minute
                    var canLateWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
                    var GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x
                    var nmStop = PluginConfiguration.GetCustomIntValue(Config.GNB_ST_NoMercyStop);
                    bool justMitted = JustUsed(OriginalHook(HeartOfStone), 4f) ||
                                      JustUsed(OriginalHook(Nebula), 5f) ||
                                      JustUsed(Camouflage, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Aurora, 5f) ||
                                      JustUsed(Superbolide, 9f);
                    #region Minimal Requirements
                    //Ammo-relative
                    var canBS = LevelChecked(BurstStrike) && //Burst Strike is unlocked
                                Ammo > 0; //Has Ammo
                    var canGF = LevelChecked(GnashingFang) && //GnashingFang is unlocked
                                gfCD < 0.6f && //Gnashing Fang is off cooldown
                                !HasEffect(Buffs.ReadyToBlast) && //to ensure Hypervelocity is spent in case Burst Strike is used before Gnashing Fang
                                GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                                Ammo > 0; //Has Ammo
                    var canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                                ddCD < 0.6f && //Double Down is off cooldown
                                Ammo > 0; //Has Ammo
                    var canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                                bfCD < 0.6f; //Bloodfest is off cooldown
                    //Cooldown-relative
                    var canZone = LevelChecked(DangerZone) && //Zone is unlocked
                                GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
                    var canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                                hasBreak; //No Mercy or Ready To Break is active
                    var canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                                GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
                    var canContinue = LevelChecked(Continuation); //Continuation is unlocked
                    var canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                                GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                                hasReign; //Ready To Reign is active
                    #endregion
                    #endregion

                    // Mitigation
                    if (IsEnabled(CustomComboPreset.GNB_ST_Mitigation) && //Mitigation option is enabled
                        InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        // Superbolide
                        if (IsEnabled(CustomComboPreset.GNB_ST_Superbolide) && //Superbolide option is enabled
                            ActionReady(Superbolide) && //Superbolide is ready
                            PlayerHealthPercentageHp() < Config.GNB_ST_Superbolide_Health && //Player's health is below selected threshold
                            (Config.GNB_ST_Superbolide_SubOption == 1 || //Superbolide is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_ST_Superbolide_SubOption == 2))) //Superbolide is enabled for bosses only
                            return Superbolide;

                        // Nebula / Damnation
                        if (IsEnabled(CustomComboPreset.GNB_ST_Nebula) && //Nebula option is enabled
                            ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                            PlayerHealthPercentageHp() < Config.GNB_ST_Nebula_Health && //Player's health is below selected threshold
                            (Config.GNB_ST_Nebula_SubOption == 1 || //Nebula is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_ST_Nebula_SubOption == 2))) //Nebula is enabled for bosses only
                            return OriginalHook(Nebula);

                        // Rampart
                        if (IsEnabled(CustomComboPreset.GNB_ST_Rampart) && //Rampart option is enabled
                            ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < Config.GNB_ST_Rampart_Health && //Player's health is below selected threshold
                            (Config.GNB_ST_Rampart_SubOption == 1 || //Rampart is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_ST_Rampart_SubOption == 2))) //Rampart is enabled for bosses only
                            return All.Rampart;

                        // Camouflage
                        if (IsEnabled(CustomComboPreset.GNB_ST_Camouflage) && //Camouflage option is enabled
                            ActionReady(Camouflage) && //Camouflage is ready
                            PlayerHealthPercentageHp() < Config.GNB_ST_Camouflage_Health && //Player's health is below selected threshold
                            (Config.GNB_ST_Camouflage_SubOption == 1 || //Camouflage is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_ST_Camouflage_SubOption == 2))) //Camouflage is enabled for bosses only
                            return Camouflage;

                        // Corundum
                        if (IsEnabled(CustomComboPreset.GNB_ST_Corundum) && //Corundum option is enabled
                            ActionReady(OriginalHook(HeartOfStone)) && //Corundum is ready
                            PlayerHealthPercentageHp() < Config.GNB_AoE_Corundum_Health && //Player's health is below selected threshold
                            (Config.GNB_AoE_Corundum_SubOption == 1 || //Corundum is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_AoE_Corundum_SubOption == 2))) //Corundum is enabled for bosses only
                            return OriginalHook(HeartOfStone);

                        // Aurora
                        if (IsEnabled(CustomComboPreset.GNB_ST_Aurora) && //Corundum option is enabled
                            ActionReady(Aurora) && //Corundum is ready
                            PlayerHealthPercentageHp() < Config.GNB_AoE_Aurora_Health && //Player's health is below selected threshold
                            (Config.GNB_AoE_Aurora_SubOption == 1 || //Corundum is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_AoE_Aurora_SubOption == 2))) //Corundum is enabled for bosses only
                            return Aurora;
                    }

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) && IsEnabled(Variant.VariantCure)
                        && PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    //Ranged Uptime
                    if (IsEnabled(CustomComboPreset.GNB_ST_RangedUptime) && //Ranged Uptime option is enabled
                        LevelChecked(LightningShot) && //Lightning Shot is unlocked
                        !InMeleeRange() && //Out of melee range
                        HasBattleTarget()) //Has target
                        return LightningShot; //Execute Lightning Shot if conditions are met

                    //oGCDs
                    if (CanWeave(actionID))
                    {
                        //Variant SpiritDart
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        //Variant Ultimatum
                        if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        //Cooldowns
                        if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup)) //Cooldowns option is enabled
                        {
                            //Bloodfest
                            if (IsEnabled(CustomComboPreset.GNB_ST_Bloodfest) && //Bloodfest option is enabled
                                InCombat() && //In combat
                                HasTarget() && //Has target
                                canBF && //able to use Bloodfest
                                Ammo == 0) //Only when ammo is empty
                                return Bloodfest; //Execute Bloodfest if conditions are met

                            //No Mercy
                            if (IsEnabled(CustomComboPreset.GNB_ST_NoMercy) && //No Mercy option is enabled
                                ActionReady(NoMercy) && //No Mercy is ready
                                InCombat() && //In combat
                                HasTarget() && //Has target
                                GetTargetHPPercent() >= nmStop) //target HP is above threshold
                            {
                                if (LevelChecked(DoubleDown)) //Lv90+
                                {
                                    if (IsOnCooldown(Bloodfest) &&
                                        Ammo != 3 &&
                                        lastComboMove is KeenEdge) //3 Ammo with Keen Edge for Opener
                                        return NoMercy; //Execute No Mercy if conditions are met
                                    if ((inOdd && //Odd Minute window
                                        (Ammo == 2 || (lastComboMove is BrutalShell && Ammo == 1))) || //2 Ammo or 1 Ammo with Solid Barrel next in combo
                                        (!inOdd && //Even Minute window
                                        Ammo != 3)) //Ammo is not full (3)
                                        return NoMercy; //Execute No Mercy if conditions are met
                                }
                                if (!LevelChecked(DoubleDown)) //Lv1-89
                                {
                                    if (canLateWeave && //Late-weaveable
                                        Ammo == MaxCartridges(level)) //Ammo is full
                                        return NoMercy; //Execute No Mercy if conditions are met
                                }
                            }

                            //Zone
                            if (IsEnabled(CustomComboPreset.GNB_ST_BlastingZone) && //Zone option is enabled
                                canZone && //able to use Zone
                                (nmCD is < 57.5f and > 17f) && //Optimal use; twice per minute, 1 in NM, 1 out of NM
                                !JustUsed(NoMercy)) //No Mercy was not just used within 3 seconds
                                return OriginalHook(DangerZone); //Execute Zone if conditions are met

                            //Bow Shock
                            if (IsEnabled(CustomComboPreset.GNB_ST_BowShock) && //Bow Shock option is enabled
                                canBow && //able to use Bow Shock
                                (nmCD is < 57.5f and > 17f) && //Optimal use; twice per minute, 1 in NM, 1 out of NM
                                !JustUsed(NoMercy)) //No Mercy was not just used within 3 seconds)
                                return BowShock;
                        }
                    }

                    //Hypervelocity - Forced to prevent loss
                    if (IsEnabled(CustomComboPreset.GNB_ST_Continuation) && //Continuation option is enabled
                        JustUsed(BurstStrike, 5f) && //Burst Strike was just used within 5 seconds
                        LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                        HasEffect(Buffs.ReadyToBlast) && //Ready To Blast buff is active
                        nmCD is > 1 or <= 0.1f) //Priority hack to prevent Hypervelocity from being used before No Mercy
                        return Hypervelocity; //Execute Hypervelocity if conditions are met

                    //Continuation protection - Forced to prevent loss
                    if (IsEnabled(CustomComboPreset.GNB_ST_Continuation) && //Continuation option is enabled
                        canContinue && //able to use Continuation
                        (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                        HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                        HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                        HasEffect(Buffs.ReadyToBlast) || //after Burst Strike
                        HasEffect(Buffs.ReadyToRaze))) //after Fated Circle
                        return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

                    //TODO: code below is rather ass; refactor
                    //Lv90+ - every 3rd NM window
                    if (LevelChecked(DoubleDown) &&
                        HasEffect(Buffs.NoMercy) &&
                        GunStep == 0 &&
                        lastComboMove is BrutalShell &&
                        Ammo == 1)
                        return SolidBarrel;

                    //GCDs
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup)) //Cooldowns option is enabled
                    {
                        //GnashingFang
                        if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && //Gnashing Fang option is enabled
                            canGF && //able to use Gnashing Fang
                            ((nmCD is > 17 and < 35) || //30s Optimal use
                            (JustUsed(NoMercy, 6f)))) //No Mercy was just used within 4 seconds
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

                    //Lv100 2cart forced 2min starter
                    if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_CooldownsGroup) && //Cooldowns option is enabled
                        IsEnabled(CustomComboPreset.GNB_ST_BurstStrike) && //Burst Strike option is enabled
                        GetTargetHPPercent() > nmStop && //target HP is above threshold
                        LevelChecked(ReignOfBeasts) && //Lv100
                        (nmCD < 1 && //No Mercy is ready or about to be
                        Ammo is 3 && //Ammo is full
                        bfCD < GCD * 12)) //Bloodfest is ready or about to be
                        return BurstStrike;

                    //Gauge Combo Steps
                    if (IsEnabled(CustomComboPreset.GNB_ST_Gnashing) && //Gnashing Fang option is enabled
                        GunStep is 1 or 2) //Gnashing Fang combo is only for 1 and 2
                        return OriginalHook(GnashingFang); //Execute Gnashing Fang combo if conditions are met
                    if (IsEnabled(CustomComboPreset.GNB_ST_Reign) && //Reign of Beasts option is enabled
                        GunStep is 3 or 4) //Reign of Beasts combo is only for 3 and 4
                        return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts combo if conditions are met

                    //123 (overcap included)
                    if (comboTime > 0) //we're in combo
                    {
                        if (LevelChecked(BrutalShell) && //Brutal Shell is unlocked
                            lastComboMove == KeenEdge) //just used first action in combo
                            return BrutalShell; //Execute Brutal Shell if conditions are met

                        if (LevelChecked(SolidBarrel) && //Solid Barrel is unlocked
                            lastComboMove == BrutalShell) //just used second action in combo
                        {
                            //holds Hypervelocity if NM comes up in time
                            if (IsEnabled(CustomComboPreset.GNB_ST_Continuation) && //Continuation option is enabled
                                LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                                HasEffect(Buffs.ReadyToBlast) && //Ready To Blast buff is active
                                (nmCD is > 1 or <= 0.1f || //Priority hack to prevent Hypervelocity from being used before No Mercy
                                GetTargetHPPercent() < nmStop)) //target HP is below threshold
                                return Hypervelocity; //Execute Hypervelocity if conditions are met

                            //Overcap protection
                            if (IsEnabled(CustomComboPreset.GNB_ST_Overcap) && //Overcap option is enabled
                                LevelChecked(BurstStrike) && //Burst Strike is unlocked
                                Ammo == MaxCartridges(level)) //Ammo is full relaive to level
                                return BurstStrike; //Execute Burst Strike if conditions are met

                            return SolidBarrel; //Execute Solid Barrel if conditions are met
                        }
                    }

                    return KeenEdge;
                }

                return actionID;
            }
        }
        #endregion

        #region Simple Mode - AoE
        internal class GNB_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_Simple;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == DemonSlice)
                {

                    #region Variables
                    //Gauge
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
                    //Cooldown-related
                    var nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
                    var bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
                    var hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
                    var hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff
                    bool justMitted = JustUsed(OriginalHook(HeartOfStone), 4f) ||
                                      JustUsed(OriginalHook(Nebula), 5f) ||
                                      JustUsed(Camouflage, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Aurora, 5f) ||
                                      JustUsed(Superbolide, 9f);
                    #region Minimal Requirements
                    //Ammo-relative
                    var canFC = LevelChecked(FatedCircle) && //Fated Circle is unlocked
                                Ammo > 0; //Has Ammo
                    var canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                                GetCooldownRemainingTime(DoubleDown) < 0.6f && //Double Down is off cooldown
                                Ammo > 0; //Has Ammo
                    var canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                                GetCooldownRemainingTime(Bloodfest) < 0.6f; //Bloodfest is off cooldown
                    //Cooldown-relative
                    var canZone = LevelChecked(DangerZone) && //Zone is unlocked
                                GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
                    var canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                                hasBreak; //No Mercy or Ready To Break is active
                    var canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                                GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
                    var canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                                GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                                hasReign; //Ready To Reign is active
                    #endregion
                    #endregion

                    // Mitigation
                    if (InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        // Superbolide
                        if (ActionReady(Superbolide) && //Superbolide is ready
                            PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                            return Superbolide;

                        // Nebula / Damnation
                        if (ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Nebula);

                        // Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        // Camouflage
                        if (ActionReady(Camouflage) && //Camouflage is ready
                            PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                            return Camouflage;

                        // Corundum
                        if (ActionReady(OriginalHook(HeartOfStone)) && //Corundum
                            PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                            return OriginalHook(HeartOfStone);
                    }

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    if (InCombat()) //if already in combat
                    {
                        if (CanWeave(actionID)) //if we can weave
                        {
                            //Variant SpiritDart
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            //Variant Ultimatum
                            if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) &&
                                IsEnabled(Variant.VariantUltimatum) &&
                                ActionReady(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            //Mitigations - Max Priority
                            //HOC
                            if (IsOffCooldown(OriginalHook(HeartOfStone))
                                && LevelChecked(HeartOfStone)
                                && PlayerHealthPercentageHp() <= 50)
                                return OriginalHook(HeartOfStone);

                            //GreatNebula
                            if (IsOffCooldown(OriginalHook(Nebula))
                                && LevelChecked(Nebula)
                                && PlayerHealthPercentageHp() <= 60)
                                return OriginalHook(Nebula);

                            //Superbolide
                            if (IsOffCooldown(Superbolide)
                                && LevelChecked(Superbolide)
                                && PlayerHealthPercentageHp() <= 25
                                && GetTargetHPPercent() >= 33)
                                return Superbolide;

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
                            (HasEffect(Buffs.NoMercy) && //if No Mercy is active
                            GunStep == 0)) //if Gnashing Fang or Reign combo is not active
                            return BurstStrike;
                    }

                    //1-2
                    if (comboTime > 0) //if we're in combo
                    {
                        if (lastComboMove == DemonSlice && //if last action was Demon Slice
                            LevelChecked(DemonSlaughter)) //if Demon Slaughter is unlocked
                        {
                            if (Ammo == MaxCartridges(level))
                            {
                                if (LevelChecked(FatedCircle)) //if Fated Circle is unlocked
                                    return FatedCircle; //execute Fated Circle
                                if (!LevelChecked(FatedCircle)) //if Fated Circle is not unlocked
                                    return BurstStrike; //execute Burst Strike
                            }
                            if (Ammo != MaxCartridges(level)) //if gauge is full && if Fated Circle is not unlocked
                                return DemonSlaughter; //execute Demon Slaughter
                        }
                    }

                    return DemonSlice; //execute Demon Slice
                }

                return actionID;
            }
        }
        #endregion

        #region Advanced Mode - AoE
        internal class GNB_AoEMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_Advanced;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == DemonSlice)
                {

                    #region Variables
                    //Gauge
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
                    //Cooldown-related
                    var nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
                    var bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
                    var hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
                    var hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff
                    bool justMitted = JustUsed(OriginalHook(HeartOfStone), 4f) ||
                                      JustUsed(OriginalHook(Nebula), 5f) ||
                                      JustUsed(Camouflage, 5f) ||
                                      JustUsed(All.Rampart, 5f) ||
                                      JustUsed(Aurora, 5f) ||
                                      JustUsed(Superbolide, 9f);
                    //Misc
                    var nmStop = PluginConfiguration.GetCustomIntValue(Config.GNB_AoE_NoMercyStop);
                    #region Minimal Requirements
                    //Ammo-relative
                    var canFC = LevelChecked(FatedCircle) && //Fated Circle is unlocked
                                Ammo > 0; //Has Ammo
                    var canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                                GetCooldownRemainingTime(DoubleDown) < 0.6f && //Double Down is off cooldown
                                Ammo > 0; //Has Ammo
                    var canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                                GetCooldownRemainingTime(Bloodfest) < 0.6f; //Bloodfest is off cooldown
                    //Cooldown-relative
                    var canZone = LevelChecked(DangerZone) && //Zone is unlocked
                                GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
                    var canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                                hasBreak; //No Mercy or Ready To Break is active
                    var canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                                GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
                    var canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                                GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                                hasReign; //Ready To Reign is active
                    #endregion
                    #endregion

                    // Mitigation
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Mitigation) && //Mitigation option is enabled
                        InCombat() && //Player is in combat
                        IsPlayerTargeted() && //Player is being targeted by current target
                        !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                    {
                        // Superbolide
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Superbolide) && //Superbolide option is enabled
                            ActionReady(Superbolide) && //Superbolide is ready
                            PlayerHealthPercentageHp() < Config.GNB_AoE_Superbolide_Health && //Player's health is below selected threshold
                            (Config.GNB_AoE_Superbolide_SubOption == 1 || //Superbolide is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_AoE_Superbolide_SubOption == 2))) //Superbolide is enabled for bosses only
                            return Superbolide;

                        // Nebula / Damnation
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Nebula) && //Nebula option is enabled
                            ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                            PlayerHealthPercentageHp() < Config.GNB_AoE_Nebula_Health && //Player's health is below selected threshold
                            (Config.GNB_AoE_Nebula_SubOption == 1 || //Nebula is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_AoE_Nebula_SubOption == 2))) //Nebula is enabled for bosses only
                            return OriginalHook(Nebula);

                        // Rampart
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Rampart) && //Rampart option is enabled
                            ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < Config.GNB_AoE_Rampart_Health && //Player's health is below selected threshold
                            (Config.GNB_AoE_Rampart_SubOption == 1 || //Rampart is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_AoE_Rampart_SubOption == 2))) //Rampart is enabled for bosses only
                            return All.Rampart;

                        // Camouflage
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Camouflage) && //Camouflage option is enabled
                            ActionReady(Camouflage) && //Camouflage is ready
                            PlayerHealthPercentageHp() < Config.GNB_AoE_Camouflage_Health && //Player's health is below selected threshold
                            (Config.GNB_AoE_Camouflage_SubOption == 1 || //Camouflage is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_AoE_Camouflage_SubOption == 2))) //Camouflage is enabled for bosses only
                            return Camouflage;

                        // Corundum
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Corundum) && //Corundum option is enabled
                            ActionReady(OriginalHook(HeartOfStone)) && //Corundum is ready
                            PlayerHealthPercentageHp() < Config.GNB_AoE_Corundum_Health && //Player's health is below selected threshold
                            (Config.GNB_AoE_Corundum_SubOption == 1 || //Corundum is enabled for all targets
                            (IsBoss(CurrentTarget!) && Config.GNB_AoE_Corundum_SubOption == 2))) //Corundum is enabled for bosses only
                            return OriginalHook(HeartOfStone);
                    }

                    //Variant Cure
                    if (IsEnabled(CustomComboPreset.GNB_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.GNB_VariantCure))
                        return Variant.VariantCure;

                    if (InCombat()) //if already in combat
                    {
                        if (CanWeave(actionID)) //if we can weave
                        {
                            //Variant SpiritDart
                            Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                            if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            //Variant Ultimatum
                            if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) &&
                                IsEnabled(Variant.VariantUltimatum) &&
                                ActionReady(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

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
                            if (IsEnabled(CustomComboPreset.GNB_AoE_DangerZone) &&
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
                            (HasEffect(Buffs.NoMercy) && //if No Mercy is active
                            GunStep == 0)) //if Gnashing Fang or Reign combo is not active
                            return BurstStrike;
                    }

                    //1-2
                    if (comboTime > 0) //if we're in combo
                    {
                        if (lastComboMove == DemonSlice && //if last action was Demon Slice
                            LevelChecked(DemonSlaughter)) //if Demon Slaughter is unlocked
                        {
                            if (Ammo == MaxCartridges(level))
                            {
                                if (IsEnabled(CustomComboPreset.GNB_AoE_Overcap) && //if Overcap option is enabled
                                    LevelChecked(FatedCircle)) //if Fated Circle is unlocked
                                    return FatedCircle; //execute Fated Circle
                                if (IsEnabled(CustomComboPreset.GNB_AoE_BSOvercap) && //if Burst Strike Overcap option is enabled
                                    !LevelChecked(FatedCircle)) //if Fated Circle is not unlocked
                                    return BurstStrike; //execute Burst Strike
                            }
                            if (Ammo != MaxCartridges(level) || //if gauge is not full
                                (Ammo == MaxCartridges(level) && //if gauge is full
                                !LevelChecked(FatedCircle) && //if Fated Circle is not unlocked
                                !IsEnabled(CustomComboPreset.GNB_AoE_BSOvercap))) //if Burst Strike Overcap option is disabled
                            {
                                return DemonSlaughter; //execute Demon Slaughter
                            }
                        }
                    }

                    return DemonSlice; //execute Demon Slice
                }

                return actionID;
            }
        }
        #endregion

        #region Gnashing Fang Features
        internal class GNB_GF_Features : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_GF_Features;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is GnashingFang)
                {
                    #region Variables
                    //Gauge
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our cartridge count
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For Gnashing Fang & Reign combo purposes
                    //Cooldown-related
                    var gfCD = GetCooldownRemainingTime(GnashingFang); //GnashingFang's cooldown; 30s total
                    var nmCD = GetCooldownRemainingTime(NoMercy); //NoMercy's cooldown; 60s total
                    var ddCD = GetCooldownRemainingTime(DoubleDown); //Double Down's cooldown; 60s total
                    var bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
                    var nmLeft = GetBuffRemainingTime(Buffs.NoMercy); //Remaining time for No Mercy buff (20s)
                    var hasNM = nmCD is >= 40 and <= 60; //Checks if No Mercy is active
                    var hasBreak = HasEffect(Buffs.ReadyToBreak); //Checks for Ready To Break buff
                    var hasReign = HasEffect(Buffs.ReadyToReign); //Checks for Ready To Reign buff

                    //Misc
                    var inOdd = bfCD is < 90 and > 20; //Odd Minute
                    var canLateWeave = GetCooldownRemainingTime(actionID) < 1 && GetCooldownRemainingTime(actionID) > 0.6; //SkS purposes
                    var GCD = GetCooldown(KeenEdge).CooldownTotal; //2.5 is base SkS, but can work with 2.4x
                    #region Minimal Requirements
                    //Ammo-relative
                    var canBS = LevelChecked(BurstStrike) && //Burst Strike is unlocked
                                Ammo > 0; //Has Ammo
                    var canGF = LevelChecked(GnashingFang) && //GnashingFang is unlocked
                                gfCD < 0.6f && //Gnashing Fang is off cooldown
                                !HasEffect(Buffs.ReadyToBlast) && //to ensure Hypervelocity is spent in case Burst Strike is used before Gnashing Fang
                                GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                                Ammo > 0; //Has Ammo
                    var canDD = LevelChecked(DoubleDown) && //Double Down is unlocked
                                ddCD < 0.6f && //Double Down is off cooldown
                                Ammo > 0; //Has Ammo
                    var canBF = LevelChecked(Bloodfest) && //Bloodfest is unlocked
                                bfCD < 0.6f; //Bloodfest is off cooldown
                    //Cooldown-relative
                    var canZone = LevelChecked(DangerZone) && //Zone is unlocked
                                GetCooldownRemainingTime(OriginalHook(DangerZone)) < 0.6f; //DangerZone is off cooldown
                    var canBreak = LevelChecked(SonicBreak) && //Sonic Break is unlocked
                                hasBreak; //No Mercy or Ready To Break is active
                    var canBow = LevelChecked(BowShock) && //Bow Shock is unlocked
                                GetCooldownRemainingTime(BowShock) < 0.6f; //BowShock is off cooldown
                    var canContinue = LevelChecked(Continuation); //Continuation is unlocked
                    var canReign = LevelChecked(ReignOfBeasts) && //Reign of Beasts is unlocked
                                GunStep == 0 && //Gnashing Fang or Reign combo is not already active
                                hasReign; //Ready To Reign is active
                    #endregion
                    #endregion

                    //oGCDs
                    if (CanWeave(ActionWatching.LastWeaponskill))
                    {
                        //Variant SpiritDart
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.GNB_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        //Variant Ultimatum
                        if (IsEnabled(CustomComboPreset.GNB_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) && ActionReady(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        //Cooldowns
                        if (IsEnabled(CustomComboPreset.GNB_GF_Features)) //Features are enabled
                        {
                            //Bloodfest
                            if (IsEnabled(CustomComboPreset.GNB_GF_Bloodfest) && //Bloodfest option is enabled
                                InCombat() && //In combat
                                HasTarget() && //Has target
                                canBF && //able to use Bloodfest
                                Ammo == 0) //Only when ammo is empty
                                return Bloodfest; //Execute Bloodfest if conditions are met

                            //No Mercy
                            if (IsEnabled(CustomComboPreset.GNB_GF_NoMercy) && //No Mercy option  is enabled
                                ActionReady(NoMercy) && //No Mercy is ready
                                InCombat() && //In combat
                                HasTarget()) //Has target
                            {
                                if (LevelChecked(DoubleDown)) //Lv90+
                                {
                                    if (IsOnCooldown(Bloodfest) &&
                                        Ammo != 3 &&
                                        lastComboMove is KeenEdge) //3 Ammo with Keen Edge for Opener
                                        return NoMercy; //Execute No Mercy if conditions are met
                                    if ((inOdd && //Odd Minute window
                                        (Ammo == 2 || (lastComboMove is BrutalShell && Ammo == 1))) || //2 Ammo or 1 Ammo with Solid Barrel next in combo
                                        (!inOdd && //Even Minute window
                                        Ammo != 3)) //Ammo is not full (3)
                                        return NoMercy; //Execute No Mercy if conditions are met
                                }
                                if (!LevelChecked(DoubleDown)) //Lv1-89
                                {
                                    if (canLateWeave && //Late-weaveable
                                        Ammo == MaxCartridges(level)) //Ammo is full
                                        return NoMercy; //Execute No Mercy if conditions are met
                                }
                            }

                            //Zone
                            if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && //Zone option is enabled
                                canZone && //able to use Zone
                                (nmCD is < 57.5f and > 17f) && //Optimal use; twice per minute, 1 in NM, 1 out of NM
                                !JustUsed(NoMercy)) //No Mercy was not just used within 3 seconds
                                return OriginalHook(DangerZone); //Execute Zone if conditions are met

                            //Bow Shock
                            if (IsEnabled(CustomComboPreset.GNB_GF_BowShock) && //Bow Shock option is enabled
                                canBow && //able to use Bow Shock
                                (nmCD is < 57.5f and > 17f) && //Optimal use; twice per minute, 1 in NM, 1 out of NM
                                !JustUsed(NoMercy)) //No Mercy was not just used within 3 seconds)
                                return BowShock;
                        }
                    }

                    //Hypervelocity - Forced to prevent loss
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && //Continuation option is enabled
                        JustUsed(BurstStrike, 5f) && //Burst Strike was just used within 5 seconds
                        LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                        HasEffect(Buffs.ReadyToBlast) && //Ready To Blast buff is active
                        nmCD is > 1 or <= 0.1f) //Priority hack to prevent Hypervelocity from being used before No Mercy
                        return Hypervelocity; //Execute Hypervelocity if conditions are met

                    //Continuation protection - Forced to prevent loss
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && //Continuation option is enabled
                        canContinue && //able to use Continuation
                        (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                        HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                        HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                        HasEffect(Buffs.ReadyToBlast) || //after Burst Strike
                        HasEffect(Buffs.ReadyToRaze))) //after Fated Circle
                        return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

                    //GCDs
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features)) //Features are enabled
                    {
                        //GnashingFang
                        if (canGF && //able to use Gnashing Fang
                            ((nmCD is > 17 and < 35) || //30s Optimal use
                            (JustUsed(NoMercy, 6f)))) //No Mercy was just used within 4 seconds
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

                    //Lv100 2cart forced 2min starter
                    if (IsEnabled(CustomComboPreset.GNB_GF_Features) && //Cooldowns option is enabled
                        IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && //Burst Strike option is enabled
                        LevelChecked(ReignOfBeasts) && //Lv100
                        (nmCD < 1 && //No Mercy is ready or about to be
                        Ammo is 3 && //Ammo is full
                        bfCD < GCD * 12)) //Bloodfest is ready or about to be
                        return BurstStrike;

                    //Gauge Combo Steps
                    if (GunStep is 1 or 2) //Gnashing Fang combo is only for 1 and 2
                        return OriginalHook(GnashingFang); //Execute Gnashing Fang combo if conditions are met
                    if (GunStep is 3 or 4) //Reign of Beasts combo is only for 3 and 4
                        return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts combo if conditions are met
                }

                return actionID;
            }
        }
        #endregion

        #region Burst Strike Features
        internal class GNB_BS_Features : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_BS_Features;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is BurstStrike)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //Our carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //For GnashingFang & (possibly) ReignCombo purposes
                    var bfCD = GetCooldownRemainingTime(Bloodfest); //Bloodfest's cooldown; 120s total
                    var ddCD = GetCooldownRemainingTime(DoubleDown); //Double Down's cooldown; 60s total

                    if (IsEnabled(CustomComboPreset.GNB_BS_Continuation) && HasEffect(Buffs.ReadyToBlast) && LevelChecked(Hypervelocity))
                        return Hypervelocity;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Bloodfest) && Ammo is 0 && LevelChecked(Bloodfest) && !HasEffect(Buffs.ReadyToBlast) && bfCD < 0.3f)
                        return Bloodfest;
                    if (IsEnabled(CustomComboPreset.GNB_BS_DoubleDown) && HasEffect(Buffs.NoMercy) && ddCD < 0.6f && Ammo >= 1 && LevelChecked(DoubleDown))
                        return DoubleDown;
                    if (IsEnabled(CustomComboPreset.GNB_BS_Reign) && (LevelChecked(ReignOfBeasts)))
                    {
                        if ((HasEffect(Buffs.ReadyToReign) && GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && !ActionReady(DoubleDown) && GunStep == 0) ||
                            (GunStep is 3 or 4))
                            return OriginalHook(ReignOfBeasts);
                    }
                }

                return actionID;
            }
        }
        #endregion

        #region Fated Circle Features
        internal class GNB_FC_Features : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_FC_Features;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is FatedCircle)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts

                    if (IsEnabled(CustomComboPreset.GNB_FC_Continuation) && HasEffect(Buffs.ReadyToRaze) && LevelChecked(FatedBrand) && CanWeave(actionID))
                        return FatedBrand;
                    if (IsEnabled(CustomComboPreset.GNB_FC_Bloodfest) && Ammo is 0 && LevelChecked(Bloodfest) && !HasEffect(Buffs.ReadyToRaze))
                        return Bloodfest;
                    if (IsEnabled(CustomComboPreset.GNB_FC_DoubleDown) && GetCooldownRemainingTime(DoubleDown) < 0.6f && Ammo >= 1 && LevelChecked(DoubleDown))
                        return DoubleDown;
                }

                return actionID;
            }
        }
        #endregion

        #region No Mercy Features
        internal class GNB_NM_Features : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_NM_Features;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == NoMercy)
                {
                    var Ammo = GetJobGauge<GNBGauge>().Ammo; //carts
                    var GunStep = GetJobGauge<GNBGauge>().AmmoComboStep; //GF/Reign combo
                    var gfCD = GetCooldownRemainingTime(GnashingFang); //GnashingFang's cooldown; 30s total

                    if (JustUsed(NoMercy, 20f) && InCombat())
                    {
                        //oGCDs
                        if (CanWeave(ActionWatching.LastWeaponskill))
                        {
                            if (IsEnabled(CustomComboPreset.GNB_NM_Bloodfest) && ActionReady(Bloodfest) && Ammo == 0)
                                return Bloodfest;
                            if (IsEnabled(CustomComboPreset.GNB_NM_Zone) && ActionReady(OriginalHook(DangerZone)) && (HasEffect(Buffs.NoMercy) || gfCD > 17))
                                return OriginalHook(DangerZone);
                            if (IsEnabled(CustomComboPreset.GNB_NM_BS) && ActionReady(BowShock) && HasEffect(Buffs.NoMercy))
                                return BowShock;
                        }

                        //GCDs
                        if (IsEnabled(CustomComboPreset.GNB_NM_SB) && HasEffect(Buffs.ReadyToBreak))
                            return SonicBreak;
                        if (IsEnabled(CustomComboPreset.GNB_NM_DD) && LevelChecked(DoubleDown) && ActionReady(DoubleDown) && Ammo >= 1 && LevelChecked(DoubleDown))
                            return DoubleDown;
                        if (IsEnabled(CustomComboPreset.GNB_NM_Reign) && (LevelChecked(ReignOfBeasts)))
                        {
                            if ((HasEffect(Buffs.ReadyToReign) && GetBuffRemainingTime(Buffs.ReadyToReign) > 0 && !ActionReady(DoubleDown) && GunStep == 0) ||
                                (GunStep is 3 or 4))
                                return OriginalHook(ReignOfBeasts);
                        }
                    }
                }

                return actionID;
            }
        }
        #endregion

        #region Aurora Protection
        internal class GNB_AuroraProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AuroraProtection;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Aurora)
                {
                    if ((HasFriendlyTarget() && TargetHasEffectAny(Buffs.Aurora)) || (!HasFriendlyTarget() && HasEffectAny(Buffs.Aurora)))
                        return OriginalHook(11);
                }
                return actionID;
            }
        }
        #endregion
    }
}