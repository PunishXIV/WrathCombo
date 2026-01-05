#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.Combos.PvE.GNB.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class GNB : Tank
{
    #region Variables
    private static byte Ammo => GetJobGauge<GNBGauge>().Ammo; //cartridge count
    private static byte GunStep => GetJobGauge<GNBGauge>().AmmoComboStep; //GF & Reign combo steps
    private static float HPP => PlayerHealthPercentageHp(); //player HP percentage
    private static float NMcd => GetCooldownRemainingTime(NoMercy); //No Mercy cooldown
    private static bool HasNM => NMcd is > 39.5f and <= 60; //Has No Mercy buff, using its cooldown instead of buff timer (for snappier reaction) with a small 0.4s leeway
    private static float GCDLength => ActionManager.GetAdjustedRecastTime(ActionType.Action, KeenEdge) / 1000f; //current GCD length in seconds
    private static bool Slow => GCDLength >= 2.5f; //2.5s or higher GCD
    private static bool Fast => GCDLength < 2.5f; //2.5s or lower GCD
    private static int MaxCartridges =>
        TraitLevelChecked(Traits.CartridgeChargeII) ? //3 max base, 6 max buffed
            HasStatusEffect(Buffs.Bloodfest) ? 6 : 3 :
        TraitLevelChecked(Traits.CartridgeCharge) ? //2 max base, 4 max buffed
            HasStatusEffect(Buffs.Bloodfest) ? 4 : 2 : 0;
    private static bool MitUsed =>
        JustUsed(OriginalHook(HeartOfStone), 4f) || //just used Heart of Stone within 4s
        JustUsed(OriginalHook(Nebula), 5f) || //just used Nebula within 5s
        JustUsed(Camouflage, 5f) || //just used Camouflage within 5s
        JustUsed(Role.Rampart, 5f) || //just used Rampart within 5s
        JustUsed(Aurora, 3f) || //just used Aurora within 3s
        JustUsed(Superbolide, 9f); //just used Superbolide within 9s

    private static bool CanGF =>
        Ammo > 0 && //have at least 1 cartridge
        GunStep == 0 && //not already in GF or Reign combo
        LevelChecked(GnashingFang) && //unlocked
        !HasStatusEffect(Buffs.ReadyToBlast) && //Hypervelocity safety - if we just used Burst Strike, we want to use Hypervelocity first even if we clip it
        GetCooldownRemainingTime(GnashingFang) < 30.5f; //off cooldown
    private static bool CanDD =>
        Ammo >= 2 && //have at least 2 cartridges
        CanUse(DoubleDown); //can use
    private static bool CanSB =>
        LevelChecked(SonicBreak) && //unlocked
        HasStatusEffect(Buffs.ReadyToBreak); //has appropriate buff needed
    private static bool CanContinue =>
        LevelChecked(Continuation) && //unlocked
        (HasStatusEffect(Buffs.ReadyToRip) || //after Gnashing Fang 
        HasStatusEffect(Buffs.ReadyToTear) || //after Savage Claw
        HasStatusEffect(Buffs.ReadyToGouge)); //after Fated Circle
    private static bool CanReign =>
        GunStep == 0 && //not in GF combo
        LevelChecked(ReignOfBeasts) && //unlocked
        HasStatusEffect(Buffs.ReadyToReign); //has appropriate buff needed
    private static bool CanUse(uint action) =>
        LevelChecked(action) && //unlocked
        GetCooldownRemainingTime(action) < 0.5f; //off cooldown
    private static bool MitigationRunning =>
        HasStatusEffect(Role.Buffs.ArmsLength) ||
        HasStatusEffect(Role.Buffs.Rampart) || 
        HasStatusEffect(Buffs.Superbolide) ||
        HasStatusEffect(Buffs.Camouflage) ||
        HasStatusEffect(Buffs.Nebula) || 
        HasStatusEffect(Buffs.GreatNebula);
    
    private static bool JustMitted =>
        JustUsed(OriginalHook(Camouflage)) ||
        JustUsed(OriginalHook(Nebula)) ||
        JustUsed(OriginalHook(HeartOfStone)) ||
        JustUsed(Role.ArmsLength) ||
        JustUsed(Role.Rampart) ||
        JustUsed(Superbolide);
    #endregion
    
    #region Auto Mitigation System
    
    [Flags]
    private enum RotationMode{
        simple = 1 << 0,
        advanced = 1 << 1
    }
    
    private static bool TryUseMits(RotationMode rotationFlags, ref uint actionID) => CanUseNonBossMits(rotationFlags, ref actionID) || CanUseBossMits(rotationFlags, ref actionID);
    
    private static bool CanUseNonBossMits(RotationMode rotationFlags, ref uint actionID)
    {
        #region Initial Bailout
        if (!InCombat() || !CanWeave() || InBossEncounter() || JustMitted || !IsEnabled(Preset.GNB_Mitigation_NonBoss))  
            return false;
        #endregion
        
        #region Heart Of Stone/Corundrum Use Always
        if (IsEnabled(Preset.GNB_Mitigation_NonBoss_HeartOfStone) && 
            ActionReady(OriginalHook(HeartOfStone)) && 
            !HasStatusEffect(Buffs.Superbolide))
        {
            actionID = OriginalHook(HeartOfStone);
            return true;
        }
        #endregion

        #region Mitigation Threshold Bailout
        float mitigationThreshold = rotationFlags.HasFlag(RotationMode.simple) 
            ? 10 
            : GNB_Mitigation_NonBoss_MitigationThreshold;
        
        if (GetAvgEnemyHPPercentInRange(5f) <= mitigationThreshold) 
            return false;
        #endregion
        
        var numberOfEnemies = NumberOfEnemiesInRange(Role.Reprisal);
        
        #region Heart of Light Overlapping 5+
        if (numberOfEnemies >= 5 && IsEnabled(Preset.GNB_Mitigation_NonBoss_HeartOfLight) && 
            ActionReady(HeartOfLight) && !HasStatusEffect(Buffs.Superbolide))
        {
            actionID = HeartOfLight;
            return true;
        }
        #endregion
        
        #region Reprisal Overlapping 5+
        if (numberOfEnemies >= 5 &&  IsEnabled(Preset.GNB_Mitigation_NonBoss_Reprisal) && 
            ActionReady(Role.Reprisal) && !HasStatusEffect(Buffs.Superbolide))
        {
            actionID = Role.Reprisal;
            return true;
        }
        #endregion
        
        #region Aurora Overlapping 3+
        if (numberOfEnemies >= 3 &&  IsEnabled(Preset.GNB_Mitigation_NonBoss_Aurora) && 
            ActionReady(Aurora) && !HasStatusEffect(Buffs.Aurora) && !JustUsed(Aurora))
        {
            actionID = OriginalHook(Aurora);
            return true;
        }
        #endregion
        
        if (MitigationRunning || numberOfEnemies <= 2) return false; //Bail if already Mitted or too few enemies
        
        #region Mitigation 5+
        if (numberOfEnemies >= 5)
        {
            if (ActionReady(Superbolide) && IsEnabled(Preset.GNB_Mitigation_NonBoss_Superbolide))
            {
                actionID = Superbolide;
                return true;
            }
            if (ActionReady(OriginalHook(Nebula)) && IsEnabled(Preset.GNB_Mitigation_NonBoss_Nebula))
            {
                actionID = OriginalHook(Nebula);
                return true;
            }
            if (ActionReady(Role.ArmsLength) && IsEnabled(Preset.GNB_Mitigation_NonBoss_ArmsLength))
            {
                actionID = Role.ArmsLength;
                return true;
            }
        }
        #endregion
        
        #region Mitigation 3+
        if (Role.CanRampart() && IsEnabled(Preset.GNB_Mitigation_NonBoss_Rampart))
        {
            actionID = Role.Rampart;
            return true;
        }
        if (ActionReady(Camouflage) && IsEnabled(Preset.GNB_Mitigation_NonBoss_Camouflage))
        {
            actionID = Camouflage;
            return true;
        }
        
        #endregion
        
        return false;

        bool IsEnabled(Preset preset)
        {
            if (rotationFlags.HasFlag(RotationMode.simple))
                return true;
            
            return CustomComboFunctions.IsEnabled(preset);
        }
    }
    
    private static bool CanUseBossMits(RotationMode rotationFlags, ref uint actionID)
    {
        #region Initial Bailout
        if (!InCombat() || !CanWeave() || !InBossEncounter() || !IsEnabled(Preset.GNB_Mitigation_Boss)) return false;
        #endregion
        
        #region Nebula
        var nebulaFirst = rotationFlags.HasFlag(RotationMode.simple)
            ? false
            : GNB_Mitigation_Boss_Nebula_First;
        
        var nebulaInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                           ContentCheck.IsInConfiguredContent(GNB_Mitigation_Boss_Nebula_Difficulty, GNB_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.GNB_Mitigation_Boss_Nebula) && 
            ActionReady(OriginalHook(Nebula)) && nebulaInMitigationContent && HasIncomingTankBusterEffect() && 
            !JustUsed(Role.Rampart, 20f) && // Prevent double big mits
            (!ActionReady(Role.Rampart) || nebulaFirst)) //Nebula First or don't use unless rampart is on cd.
        {
            actionID = OriginalHook(Nebula);
            return true;
        }
        #endregion
        
        #region Rampart
        var rampartInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                         ContentCheck.IsInConfiguredContent(GNB_Mitigation_Boss_Rampart_Difficulty, GNB_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.GNB_Mitigation_Boss_Rampart) && 
            ActionReady(Role.Rampart) && rampartInMitigationContent && HasIncomingTankBusterEffect() && 
            !JustUsed(OriginalHook(Nebula), 15f)) // Prevent double big mits
        {
            actionID = Role.Rampart;
            return true;
        }
        #endregion
        
        #region Heart of Stone/Corundrum
        var HeartOfStoneOnCDInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) ||
                                                  ContentCheck.IsInConfiguredContent(GNB_Mitigation_Boss_HeartOfStone_OnCD_Difficulty, GNB_Boss_Mit_DifficultyListSet);
        
        var HeartOfStoneTankBusterInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) ||
                                                        ContentCheck.IsInConfiguredContent(GNB_Mitigation_Boss_HeartOfStone_TankBuster_Difficulty, GNB_Boss_Mit_DifficultyListSet);
        var HeartOfStoneHealthThreshold = rotationFlags.HasFlag(RotationMode.simple) 
            ? 50
            : GNB_Mitigation_Boss_HeartOfStone_Health;

        bool heartOfStoneOnCD = IsEnabled(Preset.GNB_Mitigation_Boss_HeartOfStone_OnCD) &&  
                                PlayerHealthPercentageHp() <= HeartOfStoneHealthThreshold && IsPlayerTargeted() && HeartOfStoneOnCDInMitigationContent;
        bool heartOfStoneTankBuster = IsEnabled(Preset.GNB_Mitigation_Boss_HeartOfStone_TankBuster) &&
                                      HasIncomingTankBusterEffect() && HeartOfStoneTankBusterInMitigationContent;
            
        if (ActionReady(OriginalHook(HeartOfStone)) && (heartOfStoneOnCD || heartOfStoneTankBuster))
        {
            actionID = OriginalHook(HeartOfStone);
            return true;
        }
        #endregion
        
        #region Camouflage
        float emergencyCamouflageThreshold = rotationFlags.HasFlag(RotationMode.simple)
            ? 80
            : GNB_Mitigation_Boss_Camouflage_Threshold;
        
        var alignCamouflage = rotationFlags.HasFlag(RotationMode.simple)
            ? true
            : GNB_Mitigation_Boss_Camouflage_Align;
        
        var CamouflageInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                         ContentCheck.IsInConfiguredContent(GNB_Mitigation_Boss_Camouflage_Difficulty, GNB_Boss_Mit_DifficultyListSet);

        bool emergencyCamo = PlayerHealthPercentageHp() <= emergencyCamouflageThreshold;
        bool noOtherMitsToUse = !ActionReady(OriginalHook(Nebula)) && !JustUsed(OriginalHook(Nebula), 13f) && !ActionReady(Role.Rampart) && !JustUsed(Role.Rampart, 18f);
        bool alignCamouflageWithRampart = JustUsed(Role.Rampart, 20f) && alignCamouflage;
        
        if (IsEnabled(Preset.GNB_Mitigation_Boss_Camouflage) && ActionReady(Camouflage) && HasIncomingTankBusterEffect() && CamouflageInMitigationContent &&
            ( emergencyCamo || noOtherMitsToUse || alignCamouflageWithRampart))
        {
            actionID = Camouflage;
            return true;
        }
        #endregion
        
        #region Aurora
        var auroraThreshold = rotationFlags.HasFlag(RotationMode.simple)
            ? 90
            : GNB_Mitigation_Boss_Aurora_Health;
        
        if (IsEnabled(Preset.GNB_Mitigation_Boss_Aurora) && 
            ActionReady(Aurora) && PlayerHealthPercentageHp() <= auroraThreshold &&
            !HasStatusEffect(Buffs.Aurora) && !JustUsed(Aurora))
        {
            actionID = OriginalHook(Aurora);
            return true;
        }
        #endregion
        
        #region Reprisal
        
        var ReprisalInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) ||
                                          ContentCheck.IsInConfiguredContent(GNB_Mitigation_Boss_Reprisal_Difficulty, GNB_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.GNB_Mitigation_Boss_Reprisal) && 
            ReprisalInMitigationContent && Role.CanReprisal(enemyCount:1) && GroupDamageIncoming() &&
            !JustUsed(HeartOfLight, 10f))
        {
            actionID = Role.Reprisal;
            return true;
        }
        #endregion 
        
        #region Heart Of Light
        var HeartOfLightInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                              ContentCheck.IsInConfiguredContent(GNB_Mitigation_Boss_HeartOfLight_Difficulty, GNB_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.GNB_Mitigation_Boss_HeartOfLight) && 
            HeartOfLightInMitigationContent && ActionReady(HeartOfLight) && GroupDamageIncoming() &&
            !JustUsed(Role.Reprisal, 10f))
        {
            actionID = HeartOfLight;
            return true;
        }
        #endregion
        
        return false;
        
        bool IsEnabled(Preset preset)
        {
            if (rotationFlags.HasFlag(RotationMode.simple))
                return true;
            
            return CustomComboFunctions.IsEnabled(preset);
        }
    }
    
    #endregion

    #region Openers
    public static Lv90FastNormalNM GNBLv90FastNormalNM = new();
    public static Lv100FastNormalNM GNBLv100FastNormalNM = new();
    public static Lv90SlowNormalNM GNBLv90SlowNormalNM = new();
    public static Lv100SlowNormalNM GNBLv100SlowNormalNM = new();
    public static Lv90FastEarlyNM GNBLv90FastEarlyNM = new();
    public static Lv100FastEarlyNM GNBLv100FastEarlyNM = new();
    public static Lv90SlowEarlyNM GNBLv90SlowEarlyNM = new();
    public static Lv100SlowEarlyNM GNBLv100SlowEarlyNM = new();

    public static WrathOpener Opener() => (!IsEnabled(Preset.GNB_ST_Opener) || !LevelChecked(DoubleDown)) ? WrathOpener.Dummy : GetOpener(GNB_Opener_NM == 0);
    private static WrathOpener GetOpener(bool isNormal)
    {
        if (Fast)
            return isNormal
                ? (LevelChecked(ReignOfBeasts) ? GNBLv100FastNormalNM : GNBLv90FastNormalNM)
                : (LevelChecked(ReignOfBeasts) ? GNBLv100FastEarlyNM : GNBLv90FastEarlyNM);

        if (Slow)
            return isNormal
                ? (LevelChecked(ReignOfBeasts) ? GNBLv100SlowNormalNM : GNBLv90SlowNormalNM)
                : (LevelChecked(ReignOfBeasts) ? GNBLv100SlowEarlyNM : GNBLv90SlowEarlyNM);

        return WrathOpener.Dummy;
    }

    #region Lv90
    internal abstract class GNBOpenerLv90Base : WrathOpener
    {
        public override int MinOpenerLevel => 90;
        public override int MaxOpenerLevel => 99;
        internal override UserData ContentCheckConfig => GNB_ST_Balance_Content;
        public override bool HasCooldowns() => IsOffCooldown(NoMercy) && IsOffCooldown(GnashingFang) && IsOffCooldown(BowShock) && IsOffCooldown(Bloodfest) && IsOffCooldown(DoubleDown) && Ammo == 0;

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = [([1], () => GNB_Opener_StartChoice == 1)];
    }
    internal class Lv90FastNormalNM : GNBOpenerLv90Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            KeenEdge,
            BrutalShell,
            NoMercy, //LateWeave
            GnashingFang, //-1 (2)
            JugularRip,
            DoubleDown, //-1 (0)
            BlastingZone,
            BowShock,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            SolidBarrel, //+1 (1)
            GnashingFang, //-1 (0)
            JugularRip,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge
        ];
        public override Preset Preset => Preset.GNB_ST_Opener;
        public override List<int> VeryDelayedWeaveSteps { get; set; } = [5];
    }
    internal class Lv90SlowNormalNM : GNBOpenerLv90Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            KeenEdge,
            BrutalShell,
            NoMercy,
            GnashingFang, //-1 (2)
            JugularRip,
            DoubleDown, //-1 (0)
            BlastingZone,
            BowShock,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            SolidBarrel, //+1 (1)
            GnashingFang, //-1 (0)
            JugularRip,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge
        ];
        public override Preset Preset => Preset.GNB_ST_Opener;
    }
    internal class Lv90FastEarlyNM : GNBOpenerLv90Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            KeenEdge,
            NoMercy, //LateWeave
            GnashingFang, //-1 (2)
            JugularRip,
            DoubleDown, //-1 (0)
            BlastingZone,
            BowShock,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            BrutalShell,
            SolidBarrel, //+1 (1)
            GnashingFang, //-1 (0)
            JugularRip,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge
        ];
        public override Preset Preset => Preset.GNB_ST_Opener;
        public override List<int> VeryDelayedWeaveSteps { get; set; } = [4];
    }
    internal class Lv90SlowEarlyNM : GNBOpenerLv90Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            KeenEdge,
            NoMercy,
            GnashingFang, //-1 (2)
            JugularRip,
            DoubleDown, //-1 (0)
            BlastingZone,
            BowShock,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            BrutalShell,
            SolidBarrel, //+1 (1)
            GnashingFang, //-1 (0)
            JugularRip,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge
        ];
        public override Preset Preset => Preset.GNB_ST_Opener;
    }
    #endregion

    #region Lv100
    internal abstract class GNBOpenerLv100Base : WrathOpener
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;
        internal override UserData ContentCheckConfig => GNB_ST_Balance_Content;
        public override bool HasCooldowns() => IsOffCooldown(Bloodfest) && IsOffCooldown(NoMercy) && IsOffCooldown(GnashingFang) && IsOffCooldown(DoubleDown) && IsOffCooldown(BowShock) && Ammo == 0;

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } = [([1], () => GNB_Opener_StartChoice == 1)];
    }
    internal class Lv100FastNormalNM : GNBOpenerLv100Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            KeenEdge,
            BrutalShell,
            NoMercy, //LateWeave
            GnashingFang, //-1 (2)
            JugularRip,
            DoubleDown, //-1 (0)
            BlastingZone,
            BowShock,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart,
            SolidBarrel, //+1 (1)
            GnashingFang, //-1 (0)
            JugularRip,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge
        ];
        public override Preset Preset => Preset.GNB_ST_Opener;
        public override List<int> VeryDelayedWeaveSteps { get; set; } = [5];
    }
    internal class Lv100SlowNormalNM : GNBOpenerLv100Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            KeenEdge,
            BrutalShell,
            NoMercy,
            GnashingFang, //-1 (2)
            JugularRip,
            BowShock,
            DoubleDown, //-1 (0)
            BlastingZone,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart,
            SolidBarrel, //+1 (1)
            GnashingFang, //-1 (0)
            JugularRip,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge
        ];
        public override Preset Preset => Preset.GNB_ST_Opener;
    }
    internal class Lv100FastEarlyNM : GNBOpenerLv100Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            NoMercy, //LateWeave
            GnashingFang, //-1 (2)
            JugularRip,
            DoubleDown, //-1 (0)
            BlastingZone,
            BowShock,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart,
            KeenEdge,
            BrutalShell,
            SolidBarrel, //+1 (1)
            GnashingFang, //-1 (0)
            JugularRip,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge
        ];
        public override Preset Preset => Preset.GNB_ST_Opener;
        public override List<int> VeryDelayedWeaveSteps { get; set; } = [3];
    }
    internal class Lv100SlowEarlyNM : GNBOpenerLv100Base
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest, //+3 (3)
            NoMercy,
            GnashingFang, //-1 (2)
            JugularRip,
            BowShock,
            DoubleDown, //-1 (0)
            BlastingZone,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart,
            KeenEdge,
            BrutalShell,
            SolidBarrel, //+1 (1)
            GnashingFang, //-1 (0)
            JugularRip,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge
        ];

        public override Preset Preset => Preset.GNB_ST_Opener;
    }
    #endregion

    #endregion

    #region Rotation
    private static bool ShouldUseNoMercy(Preset preset, int stop, int boss)
    {
        var condition =
            IsEnabled(preset) && //option enabled
            InCombat() && //in combat
            NMcd < 0.5f && //off cooldown
            HasBattleTarget() && //has a battle target
            GetTargetDistance() <= 5 && //not far from target
            GetTargetHPPercent() > stop && //HP% stop condition
            (boss == 0 || boss == 1 && InBossEncounter()); //boss encounter condition

        return
            (Slow && condition && CanWeave()) || //weave anywhere
            (Fast && condition && CanDelayedWeave(0.9f)); //late weave only
    }

    private static bool ShouldUseBloodfest(Preset preset) =>
        IsEnabled(preset) && //option enabled
        CanUse(Bloodfest) && //can use
        CanWeave() && //can weave
        HasBattleTarget(); //has a target

    private static bool ShouldUseZone(Preset preset) =>
        IsEnabled(preset) && //option enabled
        CanUse(OriginalHook(DangerZone)) && //can use
        CanWeave() && //can weave
        NMcd is < 57.5f and > 15f; //use in No Mercy but not directly after it's used and off cooldown in filler - if desynced, try to hold for NM window

    private static bool ShouldUseBowShock(Preset preset) =>
        IsEnabled(preset) && //option enabled
        CanUse(BowShock) && //can use
        CanWeave() && //can weave
        NMcd is < 57.5f and >= 40; //use in No Mercy window but not directly after it's used

    private static bool ShouldUseGnashingFangBurst(Preset preset) =>
        IsEnabled(preset) && //option enabled
        CanGF && //can use
        ((JustUsed(NoMercy) && GetStatusEffectRemainingTime(Buffs.NoMercy) > 10) || //has No Mercy buff or just used it within 2.5s
        (NMcd > 7 && GetCooldownRemainingTime(GnashingFang) < 0.5f)); //overcap, but wait if No Mercy is close

    private static bool ShouldUseGnashingFangFiller(Preset preset) =>
        IsEnabled(preset) && //option enabled
        CanGF && //can use
        GetRemainingCharges(GnashingFang) != 2 && //not at max charges
        NMcd > 7 && //if No Mercy is close, then wait for it
        ComboTimer > (GCDLength * 4) && //our combo can actually drop if we carelessly send both charges asap in burst LMAO - we will use 4 GCDs (3 base + 1 buffer) as our threshold
        !HasStatusEffect(Buffs.ReadyToReign); //no Reign buff

    //there is some opti for Burst Strike and Fated Circle regarding No Mercy and their relative Continuation procs
    //the idea is to use `Cart Action -> No Mercy -> Continuation Proc` to buff proc damage
    //it was substantial before, but now it has become more prevelant due to the 7.4 changes allowing us to do it every 1m
    private static bool ShouldUseBurstStrike(Preset preset, int setup) =>
        IsEnabled(preset) && //option enabled
        LevelChecked(BurstStrike) && //unlocked
        Ammo > 0 && //at least 1 cartridge
        ((Ammo > 3 && NMcd > 10) || //leftover carts - try to spend them asap, but not if No Mercy is close
        (setup == 0 && Slow && LevelChecked(DoubleDown) && NMcd < 1) || //BS>NM setup logic for 2.5 only
        (HasNM && GunStep == 0 && !HasStatusEffect(Buffs.ReadyToReign) && GetRemainingCharges(GnashingFang) == 0 && (!LevelChecked(DoubleDown) || IsOnCooldown(DoubleDown)))); //burst logic

    private static bool ShouldUseFatedCircle(Preset preset, int setup) =>
        IsEnabled(preset) && //option enabled
        LevelChecked(BurstStrike) && //unlocked
        Ammo > 0 && //at least 1 cartridge
        ((Ammo > 3 && NMcd > 10) || //leftover carts - try to spend them asap, but not if No Mercy is close
        (setup == 0 && Slow && LevelChecked(DoubleDown) && NMcd < 1) || //BS>NM setup logic for 2.5 only
        (HasNM && GunStep == 0 && !HasStatusEffect(Buffs.ReadyToReign) && (!LevelChecked(DoubleDown) || IsOnCooldown(DoubleDown)))); //burst logic

    private static bool ShouldUseDoubleDown(Preset preset) =>
        IsEnabled(preset) && //option enabled
        CanDD && //can use
        HasNM && //has No Mercy buff
        (GetRemainingCharges(GnashingFang) < 2 || Ammo == 2); //if we have both GF charges then we need to use after GF for cd purposes or if we have exactly 2 carts left (which should be unlikely now)

    private static bool ShouldUseSonicBreak(Preset preset) =>
        IsEnabled(preset) && //option enabled
        CanSB && //can use
        ((Slow && //if slow SkS
            (IsOnCooldown(DoubleDown) || !LevelChecked(DoubleDown)) && //if DD is unlocked and on cooldown, else just send
            (GetRemainingCharges(GnashingFang) < 2 || !LevelChecked(GnashingFang))) || //if GF is unlocked and has less than 2 charges, else just send
        (Fast && GetStatusEffectRemainingTime(Buffs.ReadyToBreak) <= (GCDLength + 10.000f))); //if fast SkS, use as last GCD in NM - determined by SB timer + 10s to prevent not sending at all if missed

    private static bool ShouldUseReignOfBeasts(Preset preset) =>
        IsEnabled(preset) && //option enabled
        CanReign && //can use
        HasNM && //has No Mercy buff
        GunStep == 0 && //not in GF combo
        IsOnCooldown(DoubleDown) && //DD is on cooldown
        GetRemainingCharges(GnashingFang) < 2 && //has less than 2 GF charges
        (!Slow || !HasStatusEffect(Buffs.ReadyToBreak)); //Sonic Break safety - if we're 2.5 & we have Sonic Break, we want to use it first before Reign - otherwise just send it after everything

    private static bool ShouldUseLightningShot(Preset preset, int holdforproc) =>
        IsEnabled(preset) && //option enabled
        (holdforproc == 0 || (holdforproc == 1 && !(CanContinue || HasStatusEffect(Buffs.ReadyToBlast)))) && //not holding for proc
        ((CanContinue || HasStatusEffect(Buffs.ReadyToBlast)) ? GetTargetDistance() > 5 : !InMeleeRange()) && //out of melee range
        HasBattleTarget() && //has a target
        LevelChecked(LightningShot); //unlocked 

    private static uint STCombo(int overcap)
    {
        if (ComboTimer > 0) //in combo
        {
            if (ComboAction == KeenEdge && //just used 1
                LevelChecked(BrutalShell)) //2 is unlocked
                return BrutalShell; //use 2

            if (ComboAction == BrutalShell && //just used 2
                LevelChecked(SolidBarrel))
            {
                return
                    (LevelChecked(BurstStrike) && //Burst Strike unlocked
                    Ammo == MaxCartridges && //at max cartridges
                    overcap == 0) //overcap option selected
                    ? BurstStrike //use Burst Strike
                    : SolidBarrel; //else use 3
            }
        }
        return KeenEdge; //1
    }
    private static uint AOECombo(int overcap, int bsChoice)
    {
        if (ComboTimer > 0) //in combo
        {
            if (ComboAction == DemonSlice && //just used 1
                LevelChecked(DemonSlaughter))
            {
                if (Ammo == MaxCartridges && //at max cartridges
                    overcap == 0) //overcap option selected
                {
                    if (LevelChecked(FatedCircle)) //Fated Circle is unlocked   
                        return FatedCircle;

                    if (!LevelChecked(FatedCircle) && //Fated Circle not unlocked
                        bsChoice == 0) //Burst Strike option selected
                        return BurstStrike; //use Burst Strike
                }

                return DemonSlaughter; //use 2
            }
        }
        return DemonSlice; //1
    }
    #endregion

    #region IDs

    public const uint //Actions
    #region Offensive

        KeenEdge = 16137, //Lv1, instant, GCD, range 3, single-target, targets=hostile
        NoMercy = 16138, //Lv2, instant, 60.0s CD (group 10), range 0, single-target, targets=self
        BrutalShell = 16139, //Lv4, instant, GCD, range 3, single-target, targets=hostile
        DemonSlice = 16141, //Lv10, instant, GCD, range 0, AOE 5 circle, targets=self
        LightningShot = 16143, //Lv15, instant, GCD, range 20, single-target, targets=hostile
        DangerZone = 16144, //Lv18, instant, 30s CD (group 4), range 3, single-target, targets=hostile
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
    #region Defensive

        Camouflage = 16140, //Lv6, instant, 90.0s CD (group 15), range 0, single-target, targets=self
        RoyalGuard = 16142, //Lv10, instant, 2.0s CD (group 1), range 0, single-target, targets=self
        ReleaseRoyalGuard = 32068, //Lv10, instant, 1.0s CD (group 1), range 0, single-target, targets=self
        Nebula = 16148, //Lv38, instant, 120.0s CD (group 21), range 0, single-target, targets=self
        Aurora = 16151, //Lv45, instant, 60.0s CD (group 19/71), range 30, single-target, targets=self/party/alliance/friendly
        Superbolide = 16152, //Lv50, instant, 360.0s CD (group 24), range 0, single-target, targets=self
        HeartOfLight = 16160, //Lv64, instant, 90.0s CD (group 16), range 0, AOE 30 circle, targets=self
        HeartOfStone = 16161, //Lv68, instant, 25.0s CD (group 3), range 30, single-target, targets=self/party
        Trajectory = 36934, //Lv56, instant, 30s CD (group 9/70) (2? charges), range 20, single-target, targets=hostile
        HeartOfCorundum = 25758, //Lv82, instant, 25.0s CD (group 3), range 30, single-target, targets=self/party
        GreatNebula = 36935, //Lv92, instant, 120.0s CD, range 0, single-target, targeets=self

    #endregion

    //Limit Break
    GunmetalSoul = 17105; //LB3, instant, range 0, AOE 50 circle, targets=self, animLock=3.860

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
            HeartOfLight = 1839, //applied by Heart of Light to self
            Aurora = 1835, //applied by Aurora to self
            Superbolide = 1836, //applied by Superbolide to self
            HeartOfStone = 1840, //applied by Heart of Stone to self
            HeartOfCorundum = 2683, //applied by Heart of Corundum to self
            ClarityOfCorundum = 2684, //applied by Heart of Corundum to self
            CatharsisOfCorundum = 2685, //applied by Heart of Corundum to self
            RoyalGuard = 1833, //applied by Royal Guard to self
            GreatNebula = 3838, //applied by Nebula to self
            ReadyToRaze = 3839, //applied by Fated Circle to self
            ReadyToBreak = 3886, //applied by No mercy to self
            ReadyToReign = 3840, //applied by Bloodfest to target
            Bloodfest = 5051; //applied by Bloodfest to target
    }
    public static class Debuffs
    {
        public const ushort
            BowShock = 1838, //applied by Bow Shock to target
            SonicBreak = 1837; //applied by Sonic Break to target
    }
    public static class Traits
    {
        public const ushort
            TankMastery = 320, //Lv1
            CartridgeCharge = 257, //Lv30
            EnhancedBrutalShell = 258, //Lv52
            DangerZoneMastery = 259, //Lv80
            HeartOfStoneMastery = 424, //Lv82
            EnhancedAurora = 425, //Lv84
            MeleeMastery = 507, //Lv84
            EnhancedContinuation = 426, //Lv86
            CartridgeChargeII = 427, //Lv88
            NebulaMastery = 574, //Lv92
            EnhancedContinuationII = 575,//Lv96
            EnhancedBloodfest = 576; //Lv100
    }

    #endregion

    #region Mitigation Priority

    ///<summary>
    ///   The list of Mitigations to use in the One-Button Mitigation combo.<br />
    ///   The order of the list needs to match the order in
    ///   <see cref="Preset" />.
    ///</summary>
    ///<value>
    ///   <c>Action</c> is the action to use.<br />
    ///   <c>Preset</c> is the preset to check if the action is enabled.<br />
    ///   <c>Logic</c> is the logic for whether to use the action.
    ///</value>
    ///<remarks>
    ///    Each logic check is already combined with checking if the preset is
    ///    enabled and if the action is <see cref="ActionReady(uint)">ready</see>
    ///    and <see cref="LevelChecked(uint)">level-checked</see>.<br />
    ///   Do not add any of these checks to <c>Logic</c>.
    ///</remarks>
    private static (uint Action, Preset Preset, System.Func<bool> Logic)[]
        PrioritizedMitigation =>
    [
        //Heart of Corundum
        (OriginalHook(HeartOfStone), Preset.GNB_Mit_Corundum,
            () => !HasStatusEffect(Buffs.HeartOfCorundum) &&
                  !HasStatusEffect(Buffs.HeartOfStone) &&
                  PlayerHealthPercentageHp() <= GNB_Mit_Corundum_Health),
        //Aurora
        (Aurora, Preset.GNB_Mit_Aurora,
            () => !(TargetIsFriendly() && HasStatusEffect(Buffs.Aurora, CurrentTarget, true) ||
                    !TargetIsFriendly() && HasStatusEffect(Buffs.Aurora, anyOwner: true)) &&
                  GetRemainingCharges(Aurora) > GNB_Mit_Aurora_Charges &&
                  PlayerHealthPercentageHp() <= GNB_Mit_Aurora_Health),
        //Camouflage
        (Camouflage, Preset.GNB_Mit_Camouflage, () => true),
        //Reprisal
        (Role.Reprisal, Preset.GNB_Mit_Reprisal,
            () => Role.CanReprisal(checkTargetForDebuff:false)),
        //Heart of Light
        (HeartOfLight, Preset.GNB_Mit_HeartOfLight,
            () => GNB_Mit_HeartOfLight_PartyRequirement ==
                  (int)PartyRequirement.No ||
                  IsInParty()),
        //Rampart
        (Role.Rampart, Preset.GNB_Mit_Rampart,
            () => Role.CanRampart()),
        //Arm's Length
        (Role.ArmsLength, Preset.GNB_Mit_ArmsLength,
            () => Role.CanArmsLength(GNB_Mit_ArmsLength_EnemyCount,
                GNB_Mit_ArmsLength_Boss)),
        //Nebula
        (OriginalHook(Nebula), Preset.GNB_Mit_Nebula,
            () => true)
    ];

    ///<summary>
    ///   Given the index of a mitigation in <see cref="PrioritizedMitigation" />,
    ///   checks if the mitigation is ready and meets the provided requirements.
    ///</summary>
    ///<param name="index">
    ///   The index of the mitigation in <see cref="PrioritizedMitigation" />,
    ///   which is the order of the mitigation in <see cref="Preset" />.
    ///</param>
    ///<param name="action">
    ///   The variable to set to the action to, if the mitigation is set to be
    ///   used.
    ///</param>
    ///<returns>
    ///   Whether the mitigation is ready, enabled, and passes the provided logic
    ///   check.
    ///</returns>
    private static bool CheckMitigationConfigMeetsRequirements
        (int index, out uint action)
    {
        action = PrioritizedMitigation[index].Action;
        return ActionReady(action) && LevelChecked(action) &&
               PrioritizedMitigation[index].Logic() &&
               IsEnabled(PrioritizedMitigation[index].Preset);
    }

    #endregion
}
