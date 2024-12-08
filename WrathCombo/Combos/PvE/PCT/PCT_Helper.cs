using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using WrathCombo.Combos.JobHelpers.Enums;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using CanvasFlags = Dalamud.Game.ClientState.JobGauge.Enums.CanvasFlags;

namespace WrathCombo.Combos.PvE;

internal partial class PCT
{
    #region Lvl 100 Opener

    internal class PCTOpenerLogicLvl100 : PCT
    {
        private OpenerState currentState = OpenerState.PrePull;

        public uint OpenerStep;

        public uint PrePullStep;

        private static uint OpenerLevel => 100;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && HasMotifs() && LevelChecked;

        public OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull) Svc.Log.Debug("Entered PrePull Opener");
                    if (value == OpenerState.InOpener) OpenerStep = 1;

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private static bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(StarryMuse))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 3)
                return false;

            if (CustomComboFunctions.GetRemainingCharges(SteelMuse) < 2)
                return false;

            return true;
        }

        private static bool HasMotifs()
        {
            PCTGauge gauge = CustomComboFunctions.GetJobGauge<PCTGauge>();

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Pom))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Weapon))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Landscape))
                return false;

            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
                return false;

            return true;
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0) PrePullStep = 1;

            if (!HasCooldowns() && !HasMotifs()) PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull)
            {
                if (CustomComboFunctions.LocalPlayer.CastActionId == RainbowDrip && PrePullStep == 1)
                    CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = RainbowDrip;

                if (CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                if (!HasMotifs())
                    return false;

                return true;
            }
            PrePullStep = 0;

            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_1) || CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_SimpleMode))
                {
                    if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = RainbowDrip;

                    if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = StrikingMuse;

                    if (CustomComboFunctions.WasLastAction(HolyInWhite) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = HolyInWhite;

                    if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = PomMuse;

                    if (CustomComboFunctions.LocalPlayer.CastActionId ==
                        CustomComboFunctions.OriginalHook(CreatureMotif) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = CustomComboFunctions.OriginalHook(CreatureMotif);

                    if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = StarryMuse;

                    if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = HammerStamp;

                    if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == 8)
                        OpenerStep++;
                    else if (OpenerStep == 8) actionID = SubtractivePalette;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = BlizzardinCyan;

                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = StoneinYellow;

                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == 11)
                        OpenerStep++;
                    else if (OpenerStep == 11) actionID = ThunderinMagenta;

                    if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = CometinBlack;

                    if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = WingedMuse;

                    if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = MogoftheAges;

                    if (CustomComboFunctions.WasLastAction(StarPrism) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = StarPrism;

                    if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = HammerBrush;

                    if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = PolishingHammer;

                    if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = RainbowDrip;
                }

                if (CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_2))
                {
                    if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = RainbowDrip;

                    if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = PomMuse;

                    if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = StrikingMuse;

                    if (CustomComboFunctions.LocalPlayer.CastActionId ==
                        CustomComboFunctions.OriginalHook(CreatureMotif) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = CustomComboFunctions.OriginalHook(CreatureMotif);

                    if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = StarryMuse;

                    if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = HammerStamp;

                    if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == 7)
                        OpenerStep++;
                    else if (OpenerStep == 7) actionID = SubtractivePalette;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = BlizzardinCyan;

                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = StoneinYellow;

                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == 10)
                        OpenerStep++;
                    else if (OpenerStep == 10) actionID = ThunderinMagenta;

                    if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = CometinBlack;

                    if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = WingedMuse;

                    if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = MogoftheAges;

                    if (CustomComboFunctions.WasLastAction(StarPrism) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = StarPrism;

                    if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = HammerBrush;

                    if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = PolishingHammer;

                    if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = RainbowDrip;
                }

                if (CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_3))
                {

                    if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = RainbowDrip;

                    if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = PomMuse;

                    if (CustomComboFunctions.WasLastAction(All.Swiftcast) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = All.Swiftcast;

                    if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = WingMotif;

                    if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = StrikingMuse;

                    if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6 && CustomComboFunctions.CanDelayedWeave(WingMotif)) actionID = StarryMuse;

                    if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = HammerStamp;

                    if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == 8)
                        OpenerStep++;
                    else if (OpenerStep == 8) actionID = SubtractivePalette;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = BlizzardinCyan;

                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = StoneinYellow;

                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == 11)
                        OpenerStep++;
                    else if (OpenerStep == 11) actionID = ThunderinMagenta;

                    if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = CometinBlack;

                    if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = WingedMuse;

                    if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = MogoftheAges;

                    if (CustomComboFunctions.WasLastAction(StarPrism) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = StarPrism;

                    if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = HammerBrush;

                    if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = PolishingHammer;

                    if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = RainbowDrip;
                }

                //Svc.Log.Debug($"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");

                    return false;
                }

                if (OpenerStep >= 18)
                {
                    CurrentState = OpenerState.OpenerFinished;
                    Svc.Log.Information("Opener completed successfully.");

                    return false;
                }

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
                if (DoOpener(ref actionID))
                    return true;

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }

    #endregion

    #region Lvl 92 Opener

    internal class PCTOpenerLogicLvl92 : PCT
    {
        private OpenerState currentState = OpenerState.PrePull;

        public uint OpenerStep;

        public uint PrePullStep;

        private static uint OpenerLevel => 92;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && HasMotifs() && LevelChecked;

        public OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull) Svc.Log.Debug("Entered PrePull Opener");
                    if (value == OpenerState.InOpener) OpenerStep = 1;

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 2)
                return false;

            return true;
        }

        private static bool HasMotifs()
        {
            PCTGauge gauge = CustomComboFunctions.GetJobGauge<PCTGauge>();

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Pom))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Weapon))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Landscape))
                return false;

            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
                return false;

            return true;
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0) PrePullStep = 1;

            if (!HasCooldowns() && !HasMotifs()) PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.LocalPlayer.CastActionId == RainbowDrip && PrePullStep == 1)
                    CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = RainbowDrip;

                if (CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                if (!HasMotifs())
                    return false;

                return true;
            }
            PrePullStep = 0;

            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                bool isEarlyOpenerEnabled =
                    CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_EarlyOpener);

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(HolyInWhite) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = HolyInWhite;
                }

                int adjustedStep = isEarlyOpenerEnabled ? 2 : 3;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PomMuse;

                adjustedStep++;

                if (CustomComboFunctions.LocalPlayer.CastActionId ==
                    CustomComboFunctions.OriginalHook(CreatureMotif) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = CustomComboFunctions.OriginalHook(CreatureMotif);

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarryMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == adjustedStep)
                    OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = SubtractivePalette;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StoneinYellow;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep)
                    OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = CometinBlack;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingedMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = MogoftheAges;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(FireInRed) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = FireInRed;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerBrush;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PolishingHammer;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(RainbowDrip) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = RainbowDrip;

                Svc.Log.Debug(
                    $"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");

                    return false;
                }

                if (OpenerStep > adjustedStep)
                {
                    CurrentState = OpenerState.OpenerFinished;
                    Svc.Log.Information("Opener completed successfully.");

                    return false;
                }

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
                if (DoOpener(ref actionID))
                    return true;

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }

    #endregion

    #region Lvl 90 Opener

    internal class PCTOpenerLogicLvl90 : PCT
    {
        private OpenerState currentState = OpenerState.PrePull;

        public uint OpenerStep;

        public uint PrePullStep;

        private static uint OpenerLevel => 90;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && HasMotifs() && LevelChecked;

        public OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull) Svc.Log.Debug("Entered PrePull Opener");
                    if (value == OpenerState.InOpener) OpenerStep = 1;

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SteelMuse) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 2)
                return false;

            return true;
        }

        private static bool HasMotifs()
        {
            PCTGauge gauge = CustomComboFunctions.GetJobGauge<PCTGauge>();

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Pom))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Weapon))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Landscape))
                return false;

            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
                return false;

            return true;
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0) PrePullStep = 1;

            if (!HasCooldowns() && !HasMotifs()) PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.WasLastAction(FireInRed) && PrePullStep == 1)
                    CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = FireInRed;

                if (!HasMotifs())
                    return false;

                return true;
            }
            PrePullStep = 0;

            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                if (!CustomComboFunctions.InCombat())
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to not being in combat.");

                    return false;
                }

                bool isEarlyOpenerEnabled =
                    CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_EarlyOpener);

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(AeroInGreen) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = AeroInGreen;
                }

                int adjustedStep = isEarlyOpenerEnabled ? 2 : 3;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PomMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingMotif;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarryMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingedMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerBrush) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerBrush;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = MogoftheAges;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(PolishingHammer) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PolishingHammer;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == adjustedStep)
                    OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = SubtractivePalette;

                adjustedStep++;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = CometinBlack;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep)
                        CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;
                }
                else
                {
                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(CometinBlack) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = CometinBlack;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep)
                        CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;
                }

                Svc.Log.Debug(
                    $"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");

                    return false;
                }

                if (OpenerStep > adjustedStep)
                {
                    CurrentState = OpenerState.OpenerFinished;
                    Svc.Log.Information("Opener completed successfully.");

                    return false;
                }

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
                if (DoOpener(ref actionID))
                    return true;

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }

    #endregion

    #region Lvl 80 Opener

    internal class PCTOpenerLogicLvl80 : PCT
    {
        private OpenerState currentState = OpenerState.PrePull;

        public uint OpenerStep;

        public uint PrePullStep;

        private static uint OpenerLevel => 80;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && HasMotifs() && LevelChecked;

        public OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull) Svc.Log.Debug("Entered PrePull Opener");
                    if (value == OpenerState.InOpener) OpenerStep = 1;

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private static bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(SteelMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(LivingMuse))
                return false;

            return true;
        }

        private static bool HasMotifs()
        {
            PCTGauge gauge = CustomComboFunctions.GetJobGauge<PCTGauge>();

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Pom))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Weapon))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Landscape))
                return false;

            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
                return false;

            return true;
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0) PrePullStep = 1;

            if (!HasCooldowns() && !HasMotifs()) PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.WasLastAction(FireInRed) && PrePullStep == 1)
                    CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = FireInRed;

                if (!HasMotifs())
                    return false;

                return true;
            }
            PrePullStep = 0;

            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener && CustomComboFunctions.InCombat())
            {
                bool isEarlyOpenerEnabled =
                    CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_EarlyOpener);

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(AeroInGreen) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = AeroInGreen;
                }

                int adjustedStep = isEarlyOpenerEnabled ? 2 : 3;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PomMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingMotif;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarryMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingedMuse;

                adjustedStep++;

                if ((CustomComboFunctions.WasLastAction(HammerStamp) ||
                     CustomComboFunctions.WasLastAction(HammerBrush)) && OpenerStep == adjustedStep)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == adjustedStep)
                {
                    if (HammerBrush.LevelChecked())
                        actionID = HammerBrush;
                    else
                        actionID = HammerStamp;
                }

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = MogoftheAges;

                adjustedStep++;

                if ((CustomComboFunctions.WasLastAction(HammerStamp) ||
                     CustomComboFunctions.WasLastAction(PolishingHammer)) && OpenerStep == adjustedStep)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == adjustedStep)
                {
                    if (PolishingHammer.LevelChecked())
                        actionID = PolishingHammer;
                    else
                        actionID = HammerStamp;
                }

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == adjustedStep)
                    OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = SubtractivePalette;

                adjustedStep++;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep)
                        CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;
                }
                else
                {
                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep)
                        CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;
                }

                Svc.Log.Debug(
                    $"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");

                    return false;
                }

                if (OpenerStep > (isEarlyOpenerEnabled ? 14 : 15))
                {
                    CurrentState = OpenerState.OpenerFinished;
                    Svc.Log.Information("Opener completed successfully.");

                    return false;
                }

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
                if (DoOpener(ref actionID))
                    return true;

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }

    #endregion

    #region Lvl 70 Opener

    internal class PCTOpenerLogicLvl70 : PCT
    {
        private OpenerState currentState = OpenerState.PrePull;

        public uint OpenerStep;

        public uint PrePullStep;

        private static uint OpenerLevel => 70;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && HasMotifs() && LevelChecked;

        public OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull) Svc.Log.Debug("Entered PrePull Opener");
                    if (value == OpenerState.InOpener) OpenerStep = 1;

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private static bool HasCooldowns()
        {
            if (!CustomComboFunctions.ActionReady(SteelMuse))
                return false;

            if (!CustomComboFunctions.ActionReady(ScenicMuse))
                return false;

            if (CustomComboFunctions.GetRemainingCharges(LivingMuse) < 2)
                return false;

            return true;
        }

        private static bool HasMotifs()
        {
            PCTGauge gauge = CustomComboFunctions.GetJobGauge<PCTGauge>();

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Pom))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Weapon))
                return false;

            if (!gauge.CanvasFlags.HasFlag(CanvasFlags.Landscape))
                return false;

            if (CustomComboFunctions.HasEffect(Buffs.SubtractivePalette))
                return false;

            return true;
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0) PrePullStep = 1;

            if (!HasCooldowns() && !HasMotifs()) PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.WasLastAction(FireInRed) && PrePullStep == 1)
                    CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = FireInRed;

                if (!HasMotifs())
                    return false;

                return true;
            }
            PrePullStep = 0;

            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                bool isEarlyOpenerEnabled =
                    CustomComboFunctions.IsEnabled(CustomComboPreset.PCT_ST_Advanced_Openers_EarlyOpener);

                if (CustomComboFunctions.WasLastAction(StrikingMuse) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = StrikingMuse;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(AeroInGreen) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = AeroInGreen;
                }

                int adjustedStep = isEarlyOpenerEnabled ? 2 : 3;

                if (CustomComboFunctions.WasLastAction(PomMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = PomMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingMotif) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingMotif;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(StarryMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = StarryMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(WingedMuse) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = WingedMuse;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(MogoftheAges) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = MogoftheAges;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(HammerStamp) && OpenerStep == adjustedStep) OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = HammerStamp;

                adjustedStep++;

                if (CustomComboFunctions.WasLastAction(SubtractivePalette) && OpenerStep == adjustedStep)
                    OpenerStep++;
                else if (OpenerStep == adjustedStep) actionID = SubtractivePalette;

                adjustedStep++;

                if (!isEarlyOpenerEnabled)
                {
                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep)
                        CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;
                }
                else
                {
                    if (CustomComboFunctions.WasLastAction(StoneinYellow) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = StoneinYellow;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(ThunderinMagenta) && OpenerStep == adjustedStep)
                        OpenerStep++;
                    else if (OpenerStep == adjustedStep) actionID = ThunderinMagenta;

                    adjustedStep++;

                    if (CustomComboFunctions.WasLastAction(BlizzardinCyan) && OpenerStep == adjustedStep)
                        CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == adjustedStep) actionID = BlizzardinCyan;
                }

                Svc.Log.Debug(
                    $"TimeSinceLastAction: {ActionWatching.TimeSinceLastAction.TotalSeconds}, OpenerStep: {OpenerStep}");

                if (ActionWatching.TimeSinceLastAction.TotalSeconds > 4)
                {
                    CurrentState = OpenerState.FailedOpener;
                    Svc.Log.Warning("Opener Failed due to timeout.");

                    return false;
                }

                if (OpenerStep > 14)
                {
                    CurrentState = OpenerState.OpenerFinished;
                    Svc.Log.Information("Opener completed successfully.");

                    return false;
                }

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
                if (DoOpener(ref actionID))
                    return true;

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }

    #endregion
}