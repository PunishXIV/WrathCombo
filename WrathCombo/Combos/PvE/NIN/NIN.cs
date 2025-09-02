using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.NIN.Config;

namespace WrathCombo.Combos.PvE;

internal partial class NIN : Melee
{
    #region Simple
    internal class NIN_ST_SimpleMode : CustomCombo
    {
        protected internal MudraCasting MudraState = new();
        protected internal override Preset Preset => Preset.NIN_ST_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SpinningEdge)
                return actionID;

            NINGauge gauge = GetJobGauge<NINGauge>();

            if (OriginalHook(Ninjutsu) is Rabbit or Huton or Suiton or Doton or GokaMekkyaku or HyoshoRanryu)
                return OriginalHook(Ninjutsu);
            
            if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat() ||
                ActionWatching.LastAction == OriginalHook(Ninjutsu))
                MudraState.CurrentMudra = MudraCasting.MudraState.None;
            
            if (InMudra && MudraState.ContinueCurrentMudra(ref actionID))
                return actionID;
            
            if (HasStatusEffect(Buffs.TenChiJin))
                return STTenChiJin(actionID);

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction) && !MudraPhase)
                return contentAction;
            #endregion

            #region OGCDS
            if (InCombat() && HasBattleTarget())
            {
                if (CanKassatsu)
                    return Kassatsu;

                if (CanBunshin)
                    return Bunshin;

                if (CanTenChiJin)
                    return TenChiJin;

                if (CanTenriJindo)
                    return TenriJendo;

                if (CanAssassinate)
                    return OriginalHook(Assassinate);

                if (CanMeisui)
                    return NinkiWillOvercap ? OriginalHook(Bhavacakra) : OriginalHook(Meisui);

                if (CanBhavacakra && NinkiPooling)
                    return LevelChecked(Bhavacakra) ? OriginalHook(Bhavacakra) : OriginalHook(HellfrogMedium);

                if (CanMugST && CombatEngageDuration().TotalSeconds > 5)
                    return NinkiWillOvercap && TraitLevelChecked(Traits.MugMastery) ? OriginalHook(Bhavacakra) : OriginalHook(Mug);

                if (CanTrickST && CombatEngageDuration().TotalSeconds > 5)
                    return OriginalHook(TrickAttack);
            }
            #endregion

            #region Ninjutsu
            if (CanUseHyoshoRanryu && MudraState.CastHyoshoRanryu(ref actionID) ||
                CanUseSuiton && TrickCD <= 18 && MudraState.CastSuiton(ref actionID) ||
                CanUseRaiton && MudraState.CastRaiton(ref actionID) ||
                CanUseFumaShuriken && !LevelChecked(Raiton) && MudraState.CastFumaShuriken(ref actionID))
                return actionID;
            #endregion

            #region Selfcare
            if ((!MudraPhase || HasKassatsu && TrickCD > 5) && CanWeave())
            {
                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (ActionReady(ShadeShift) && (PlayerHealthPercentageHp() < 60 || RaidWideCasting()))
                    return ShadeShift;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }
            #endregion

            #region GCDS
            if (CanThrowingDaggers)
                return OriginalHook(ThrowingDaggers);

            if (CanRaiju)
                return FleetingRaiju;

            if (CanPhantomKamaitachi)
                return PhantomKamaitachi;

            if (ComboTimer > 1f)
            {
                switch (ComboAction)
                {
                    case SpinningEdge when LevelChecked(GustSlash):
                        return OriginalHook(GustSlash);

                    case GustSlash when GetTargetHPPercent() <= 10 && gauge.Kazematoi > 0: //Kazematoi Dump Below 10%
                        return TNAeolianEdge ? Role.TrueNorth : AeolianEdge;

                    case GustSlash when LevelChecked(ArmorCrush):
                        return gauge.Kazematoi switch
                        {
                            0 => TNArmorCrush ? Role.TrueNorth : ArmorCrush,
                            >= 4 => TNAeolianEdge ? Role.TrueNorth : AeolianEdge,
                            _ => OnTargetsFlank() || !TargetNeedsPositionals() ? ArmorCrush : AeolianEdge
                        };
                    case GustSlash when !LevelChecked(ArmorCrush) && LevelChecked(AeolianEdge):
                        return TNAeolianEdge ? Role.TrueNorth : AeolianEdge;
                }
            }
            return OriginalHook(SpinningEdge);
            #endregion
        }
    }

    internal class NIN_AoE_SimpleMode : CustomCombo
    {
        protected internal MudraCasting MudraState = new();
        protected internal override Preset Preset => Preset.NIN_AoE_SimpleMode;
        protected override uint Invoke(uint actionID)

        {
            if (actionID is not DeathBlossom)
                return actionID;

            if (OriginalHook(Ninjutsu) is Rabbit or Huton or Suiton or Doton or GokaMekkyaku or HyoshoRanryu)
                return OriginalHook(Ninjutsu);
            
            if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat() ||
                ActionWatching.LastAction == OriginalHook(Ninjutsu))
                MudraState.CurrentMudra = MudraCasting.MudraState.None;
            
            if (InMudra && MudraState.ContinueCurrentMudra(ref actionID))
                return actionID;

            if (HasStatusEffect(Buffs.TenChiJin))
                return DotonRemaining < 3
                    ? AoETenChiJinDoton(actionID)
                    : AoETenChiJinSuiton(actionID);

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction) && !MudraPhase)
                return contentAction;
            #endregion

            #region OGCDS
            if (InCombat() && HasBattleTarget())
            {
                if (CanKassatsuAoE)
                    return Kassatsu;

                if (CanBunshin)
                    return Bunshin;

                if (CanTenChiJinAoE)
                    return TenChiJin;

                if (CanTenriJindo)
                    return TenriJendo;

                if (CanAssassinateAoE)
                    return OriginalHook(Assassinate);

                if (CanMeisuiAoE)
                    return NinkiWillOvercap ? OriginalHook(HellfrogMedium) : OriginalHook(Meisui);

                if (CanHellfrogMedium && NinkiPooling)
                    return OriginalHook(HellfrogMedium);

                if (CanMugAoE && CombatEngageDuration().TotalSeconds > 5)
                    return NinkiWillOvercap && TraitLevelChecked(Traits.MugMastery) ? OriginalHook(HellfrogMedium) : OriginalHook(Mug);

                if (CanTrickAoE && CombatEngageDuration().TotalSeconds > 5)
                    return OriginalHook(TrickAttack);
            }
            #endregion

            #region Ninjutsu
            if (CanUseGokaMekkyaku && MudraState.CastGokaMekkyaku(ref actionID) ||
                CanUseHuton && TrickCD <= 18 && MudraState.CastHuton(ref actionID) ||
                CanUseDoton && GetTargetHPPercent() >= 30 && MudraState.CastDoton(ref actionID) ||
                CanUseKaton && MudraState.CastKaton(ref actionID) ||
                CanUseFumaShuriken && !LevelChecked(Katon) && MudraState.CastFumaShuriken(ref actionID))
                return actionID;
            #endregion

            #region Selfcare
            if ((!MudraPhase || HasKassatsu && TrickCD > 5) && CanWeave())
            {
                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (ActionReady(ShadeShift) && (PlayerHealthPercentageHp() < 60 || RaidWideCasting()))
                    return ShadeShift;

                if (Role.CanBloodBath(40))
                    return Role.Bloodbath;
            }
            #endregion

            #region GCDS
            if (CanThrowingDaggersAoE)
                return OriginalHook(ThrowingDaggers);

            if (CanRaiju)
                return FleetingRaiju;

            if (CanPhantomKamaitachi)
                return PhantomKamaitachi;

            if (ComboTimer > 1f)
            {
                switch (ComboAction)
                {
                    case SpinningEdge when LevelChecked(GustSlash) && !LevelChecked(DeathBlossom):
                        return OriginalHook(GustSlash);
                    case GustSlash when !LevelChecked(ArmorCrush) && LevelChecked(AeolianEdge) && !LevelChecked(DeathBlossom):
                        return TNAeolianEdge ? Role.TrueNorth : AeolianEdge;
                    case DeathBlossom when LevelChecked(HakkeMujinsatsu):
                        return HakkeMujinsatsu;
                }
            }
            return LevelChecked(DeathBlossom)
                ? DeathBlossom
                : SpinningEdge;
            #endregion
        }
    }

    #endregion

    #region Advanced
    internal class NIN_ST_AdvancedMode : CustomCombo
    {
        protected internal MudraCasting MudraState = new();
        protected internal override Preset Preset => Preset.NIN_ST_AdvancedMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not SpinningEdge)
                return actionID;

            NINGauge gauge = GetJobGauge<NINGauge>();

            if (IsEnabled(Preset.NIN_ST_AdvancedMode_BalanceOpener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (IsEnabled(Preset.NIN_ST_AdvancedMode_Ninjitsus) &&
                OriginalHook(Ninjutsu) is Rabbit or Huton or Suiton or Doton or GokaMekkyaku or HyoshoRanryu)
                return OriginalHook(Ninjutsu);
            
            if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat() ||
                ActionWatching.LastAction == OriginalHook(Ninjutsu))
                MudraState.CurrentMudra = MudraCasting.MudraState.None;
            
            if (IsEnabled(Preset.NIN_ST_AdvancedMode_Ninjitsus) && InMudra && MudraState.ContinueCurrentMudra(ref actionID))
                return actionID;
            
            if (NIN_ST_AdvancedMode_TenChiJin_Options[0] &&
                HasStatusEffect(Buffs.TenChiJin))
                return STTenChiJin(actionID);

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction) && !MudraPhase)
                return contentAction;
            #endregion

            #region OGCDS
            if (InCombat() && HasBattleTarget())
            {
                if (IsEnabled(Preset.NIN_ST_AdvancedMode_Kassatsu) && CanKassatsu)
                    return Kassatsu;

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_Bunshin) && CanBunshin)
                    return Bunshin;

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_TenChiJin) && CanTenChiJin)
                    return TenChiJin;

                if (NIN_ST_AdvancedMode_TenChiJin_Options[0] && CanTenriJindo)
                    return TenriJendo;

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_Assassinate) && CanAssassinate)
                    return OriginalHook(Assassinate);

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_Meisui) && CanMeisui)
                    return NinkiWillOvercap && IsEnabled(Preset.NIN_ST_AdvancedMode_Bhavacakra)
                        ? OriginalHook(Bhavacakra)
                        : OriginalHook(Meisui);

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_Bhavacakra) && CanBhavacakra &&
                    (NinkiPooling || !NIN_ST_AdvancedMode_Bhavacakra_Pooling))
                    return LevelChecked(Bhavacakra) ? OriginalHook(Bhavacakra) : OriginalHook(HellfrogMedium);

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_Mug) && CanMugST && CombatEngageDuration().TotalSeconds > 5 &&
                    GetTargetHPPercent() > STMugThreshold)
                    return NinkiWillOvercap &&
                           TraitLevelChecked(Traits.MugMastery) &&
                           IsEnabled(Preset.NIN_ST_AdvancedMode_Bhavacakra)
                        ? OriginalHook(Bhavacakra) : OriginalHook(Mug);

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_TrickAttack) && CanTrickST && CombatEngageDuration().TotalSeconds > 5 &&
                    GetTargetHPPercent() > STTrickThreshold)
                    return OriginalHook(TrickAttack);

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_StunInterupt) && CanWeave() && !MudraPhase &&
                    RoleActions.Melee.CanLegSweep() && !TargetIsBoss() && TargetIsCasting())
                    return Role.LegSweep;
            }
            #endregion

            #region Ninjutsu
            if (IsEnabled(Preset.NIN_ST_AdvancedMode_Ninjitsus))
            {
                if (NIN_ST_AdvancedMode_Ninjitsus_Options[2] &&
                    CanUseHyoshoRanryu && MudraState.CastHyoshoRanryu(ref actionID) ||
                    NIN_ST_AdvancedMode_Ninjitsus_Options[1] &&
                    CanUseSuiton && TrickCD <= NIN_ST_AdvancedMode_SuitonSetup && MudraState.CastSuiton(ref actionID) ||
                    NIN_ST_AdvancedMode_Ninjitsus_Options[0] &&
                    CanUseRaiton && MudraState.CastRaiton(ref actionID) ||
                    NIN_ST_AdvancedMode_Ninjitsus_Options[0] &&
                    CanUseFumaShuriken && !LevelChecked(Raiton) && MudraState.CastFumaShuriken(ref actionID))
                    return actionID;
            }
            #endregion

            #region Selfcare
            if ((!MudraPhase || HasKassatsu && TrickCD > 5) && CanWeave())
            {
                if (IsEnabled(Preset.NIN_ST_AdvancedMode_Feint) && 
                    Role.CanFeint() &&
                    RaidWideCasting())
                    return Role.Feint;

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_SecondWind) &&
                    Role.CanSecondWind(NIN_ST_AdvancedMode_SecondWindThreshold))
                    return Role.SecondWind;

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_ShadeShift) && ActionReady(ShadeShift) &&
                    (PlayerHealthPercentageHp() < NIN_ST_AdvancedMode_ShadeShiftThreshold ||
                     NIN_ST_AdvancedMode_ShadeShiftRaidwide && RaidWideCasting()))
                    return ShadeShift;

                if (IsEnabled(Preset.NIN_ST_AdvancedMode_Bloodbath) &&
                    Role.CanBloodBath(NIN_ST_AdvancedMode_BloodbathThreshold))
                    return Role.Bloodbath;
            }
            #endregion

            #region GCDS
            if (IsEnabled(Preset.NIN_ST_AdvancedMode_ThrowingDaggers) && CanThrowingDaggers && !MudraPhase)
                return OriginalHook(ThrowingDaggers);

            if (IsEnabled(Preset.NIN_ST_AdvancedMode_Raiju) && CanRaiju)
                return NIN_ST_AdvancedMode_ForkedRaiju && !InMeleeRange()
                    ? ForkedRaiju
                    : FleetingRaiju;

            if (IsEnabled(Preset.NIN_ST_AdvancedMode_PhantomKamaitachi) && CanPhantomKamaitachi)
                return PhantomKamaitachi;

            if (ComboTimer > 1f)
            {
                switch (ComboAction)
                {
                    case SpinningEdge when LevelChecked(GustSlash):
                        return OriginalHook(GustSlash);

                    case GustSlash when GetTargetHPPercent() <= NIN_ST_AdvancedMode_BurnKazematoi && gauge.Kazematoi > 0: //Kazematoi Dump Below 10%
                        return TNAeolianEdge && NIN_ST_AdvancedMode_TrueNorth ? Role.TrueNorth : AeolianEdge;

                    case GustSlash when LevelChecked(ArmorCrush):
                        return gauge.Kazematoi switch
                        {
                            0 => TNArmorCrush && NIN_ST_AdvancedMode_TrueNorth ? Role.TrueNorth : ArmorCrush,
                            >= 4 => TNAeolianEdge && NIN_ST_AdvancedMode_TrueNorth ? Role.TrueNorth : AeolianEdge,
                            _ => OnTargetsFlank() || !TargetNeedsPositionals() ? ArmorCrush : AeolianEdge
                        };
                    case GustSlash when !LevelChecked(ArmorCrush) && LevelChecked(AeolianEdge):
                        return TNAeolianEdge ? Role.TrueNorth : AeolianEdge;
                }
            }
            return actionID;
            #endregion
        }
    }

    internal class NIN_AoE_AdvancedMode : CustomCombo
    {
        protected internal MudraCasting MudraState = new();
        protected internal override Preset Preset => Preset.NIN_AoE_AdvancedMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DeathBlossom)
                return actionID;

            if (IsEnabled(Preset.NIN_AoE_AdvancedMode_Ninjitsus) &&
                OriginalHook(Ninjutsu) is Rabbit or Huton or Suiton or Doton or GokaMekkyaku or HyoshoRanryu)
                return OriginalHook(Ninjutsu);
            
            if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !InCombat() ||
                ActionWatching.LastAction == OriginalHook(Ninjutsu))
                MudraState.CurrentMudra = MudraCasting.MudraState.None;
            
            if (IsEnabled(Preset.NIN_AoE_AdvancedMode_Ninjitsus) && InMudra && MudraState.ContinueCurrentMudra(ref actionID))
                return actionID;
           
            if (NIN_AoE_AdvancedMode_TenChiJin_Options[0] &&
                HasStatusEffect(Buffs.TenChiJin))
                return NIN_AoE_AdvancedMode_Ninjitsus_Options[2] && DotonRemaining < 3
                ? AoETenChiJinDoton(actionID)
                : AoETenChiJinSuiton(actionID);

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction) && !MudraPhase)
                return contentAction;
            #endregion

            #region OGCDS
            if (InCombat() && HasBattleTarget())
            {
                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_Kassatsu) && CanKassatsuAoE)
                    return Kassatsu;

                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_Bunshin) && CanBunshin)
                    return Bunshin;

                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_TenChiJin) && CanTenChiJinAoE)
                    return TenChiJin;

                if (NIN_AoE_AdvancedMode_TenChiJin_Options[0] && CanTenriJindo)
                    return TenriJendo;

                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_Assassinate) && CanAssassinateAoE)
                    return OriginalHook(Assassinate);

                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_Meisui) && CanMeisuiAoE)
                    return NinkiWillOvercap && IsEnabled(Preset.NIN_AoE_AdvancedMode_HellfrogMedium)
                        ? OriginalHook(HellfrogMedium)
                        : OriginalHook(Meisui);

                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_HellfrogMedium) && CanHellfrogMedium &&
                    (NinkiPooling || !NIN_AoE_AdvancedMode_HellfrogMedium_Pooling))
                    return OriginalHook(HellfrogMedium);

                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_Mug) && CanMugAoE && CombatEngageDuration().TotalSeconds > 5 &&
                    GetTargetHPPercent() > AoEMugThreshold)
                    return NinkiWillOvercap &&
                           TraitLevelChecked(Traits.MugMastery) &&
                           IsEnabled(Preset.NIN_AoE_AdvancedMode_HellfrogMedium)
                        ? OriginalHook(HellfrogMedium)
                        : OriginalHook(Mug);

                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_TrickAttack) && CanTrickAoE && CombatEngageDuration().TotalSeconds > 5 &&
                    GetTargetHPPercent() > AoETrickThreshold)
                    return OriginalHook(TrickAttack);
                
                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_StunInterupt) && CanWeave() && !MudraPhase &&
                    RoleActions.Melee.CanLegSweep() && !TargetIsBoss() && TargetIsCasting())
                    return Role.LegSweep;
            }
            #endregion

            #region Ninjutsu
            if (IsEnabled(Preset.NIN_AoE_AdvancedMode_Ninjitsus))
            {
                if (NIN_AoE_AdvancedMode_Ninjitsus_Options[3] &&
                    CanUseGokaMekkyaku && MudraState.CastGokaMekkyaku(ref actionID) ||
                    NIN_AoE_AdvancedMode_Ninjitsus_Options[2] &&
                    CanUseDoton && GetTargetHPPercent() >= NIN_AoE_AdvancedMode_Doton_Threshold && MudraState.CastDoton(ref actionID) ||
                    NIN_AoE_AdvancedMode_Ninjitsus_Options[1] &&
                    CanUseHuton && TrickCD <= NIN_ST_AdvancedMode_SuitonSetup && MudraState.CastHuton(ref actionID) ||
                    NIN_AoE_AdvancedMode_Ninjitsus_Options[0] &&
                    CanUseKaton && MudraState.CastKaton(ref actionID) ||
                    NIN_AoE_AdvancedMode_Ninjitsus_Options[0] &&
                    CanUseFumaShuriken && !LevelChecked(Raiton) && MudraState.CastFumaShuriken(ref actionID))
                    return actionID;
            }
            #endregion

            #region Selfcare
            if ((!MudraPhase || HasKassatsu && TrickCD > 5) && CanWeave())
            {
                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_SecondWind) &&
                    Role.CanSecondWind(NIN_AoE_AdvancedMode_SecondWindThreshold))
                    return Role.SecondWind;

                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_ShadeShift) && ActionReady(ShadeShift) &&
                    (PlayerHealthPercentageHp() < NIN_AoE_AdvancedMode_ShadeShiftThreshold ||
                     NIN_AoE_AdvancedMode_ShadeShiftRaidwide && RaidWideCasting()))
                    return ShadeShift;

                if (IsEnabled(Preset.NIN_AoE_AdvancedMode_Bloodbath) &&
                    Role.CanBloodBath(NIN_AoE_AdvancedMode_BloodbathThreshold))
                    return Role.Bloodbath;
            }
            #endregion

            #region GCDS
            if (IsEnabled(Preset.NIN_AoE_AdvancedMode_ThrowingDaggers) && CanThrowingDaggersAoE && !MudraPhase)
                return OriginalHook(ThrowingDaggers);

            if (IsEnabled(Preset.NIN_AoE_AdvancedMode_PhantomKamaitachi) && CanPhantomKamaitachi)
                return PhantomKamaitachi;

            if (ComboTimer > 1f)
            {
                switch (ComboAction)
                {
                    case SpinningEdge when LevelChecked(GustSlash) && !LevelChecked(DeathBlossom):
                        return OriginalHook(GustSlash);
                    case GustSlash when !LevelChecked(ArmorCrush) && LevelChecked(AeolianEdge) && !LevelChecked(DeathBlossom):
                        return TNAeolianEdge ? Role.TrueNorth : AeolianEdge;
                    case DeathBlossom when LevelChecked(HakkeMujinsatsu):
                        return HakkeMujinsatsu;
                }
            }
            return LevelChecked(DeathBlossom) ? DeathBlossom : SpinningEdge;
            #endregion
        }
    }
    #endregion

    #region Standalone

    internal class NIN_MudraProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.NIN_MudraProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (RoleActions.Melee.Feint or RoleActions.Melee.Bloodbath or RoleActions.Physical.SecondWind or ShadeShift or Shukuchi or RoleActions.Melee.LegSweep))
                return actionID;

            switch (actionID)
            {
                case ShadeShift when NIN_MudraProtection_Options[0] && MudraPhase:
                        
                case Shukuchi when NIN_MudraProtection_Options[1] && MudraPhase:
                
                case RoleActions.Melee.Feint when NIN_MudraProtection_Options[2] && (MudraPhase || HasStatusEffect(RoleActions.Melee.Debuffs.Feint, CurrentTarget, true)):
                    
                case RoleActions.Melee.Bloodbath when NIN_MudraProtection_Options[3] && MudraPhase:
                        
                case RoleActions.Physical.SecondWind when NIN_MudraProtection_Options[4] && MudraPhase:
                
                case RoleActions.Melee.LegSweep when NIN_MudraProtection_Options[5] && MudraPhase:
                    return All.SavageBlade;
            }
            
            return actionID;
        }
    }    

    internal class NIN_ST_AeolianEdgeCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.NIN_ST_AeolianEdgeCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not AeolianEdge)
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is SpinningEdge && LevelChecked(GustSlash))
                    return GustSlash;

                if (ComboAction is GustSlash && LevelChecked(AeolianEdge))
                    return AeolianEdge;
            }
            return SpinningEdge;
        }
    }

    internal class NIN_ArmorCrushCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.NIN_ArmorCrushCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ArmorCrush)
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction == SpinningEdge && LevelChecked(GustSlash))
                    return GustSlash;

                if (ComboAction == GustSlash && LevelChecked(ArmorCrush))
                    return ArmorCrush;
            }
            return SpinningEdge;
        }
    }

    internal class NIN_HideMug : CustomCombo
    {
        protected internal override Preset Preset => Preset.NIN_HideMug;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Hide)
                return actionID;
            
            
            if (NIN_HideMug_Toggle && HasStatusEffect(Buffs.Hidden) &&
                (LevelChecked(Suiton) || !NIN_HideMug_ToggleLevelCheck)) //Check level to get ShadowWalker buff.
                StatusManager.ExecuteStatusOff(Buffs.Hidden);

            if (NIN_HideMug_Trick && 
                (!NIN_HideMug_Mug || !NIN_HideMug_TrickAfterMug || IsOnCooldown(OriginalHook(Mug)) || !InCombat()) && //Check mug if you want mug to have priority
                (HasStatusEffect(Buffs.Hidden) || HasStatusEffect(Buffs.ShadowWalker))) //Check for ability to use trick
                return OriginalHook(TrickAttack);

            if (InCombat() && NIN_HideMug_Mug)
                return OriginalHook(Mug);
            
            return InCombat() && NIN_HideMug_Trick ? OriginalHook(TrickAttack) : actionID;
        }
    }

    internal class NIN_KassatsuChiJin : CustomCombo
    {
        protected internal override Preset Preset => Preset.NIN_KassatsuChiJin;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Chi)
                return actionID;

            return TraitLevelChecked(250) && HasStatusEffect(Buffs.Kassatsu)
                ? Jin
                : actionID;
        }
    }

    internal class NIN_KassatsuTrick : CustomCombo
    {
        protected internal override Preset Preset => Preset.NIN_KassatsuTrick;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Kassatsu)
                return actionID;

            return HasStatusEffect(Buffs.ShadowWalker) || HasStatusEffect(Buffs.Hidden)
                ? OriginalHook(TrickAttack)
                : actionID;
        }
    }

    internal class NIN_TCJMeisui : CustomCombo
    {
        protected internal override Preset Preset => Preset.NIN_TCJMeisui;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TenChiJin)
                return actionID;

            if (HasStatusEffect(Buffs.TenChiJin) && IsEnabled(Preset.NIN_TCJ))
                return STTenChiJin(actionID);

            return HasStatusEffect(Buffs.ShadowWalker)
                ? Meisui
                : actionID;
        }
    }

    internal class NIN_Simple_Mudras : CustomCombo
    {
        protected internal override Preset Preset => Preset.NIN_Simple_Mudras;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ten or Chi or Jin) || !HasStatusEffect(Buffs.Mudra))
                return actionID;

            int mudrapath = NIN_SimpleMudra_Choice;

            if (mudrapath == 1)
            {
                if (Ten.LevelChecked() && actionID == Ten)
                {
                    if (Jin.LevelChecked() && OriginalHook(Ninjutsu) is Raiton)
                    {
                        return OriginalHook(JinCombo);
                    }

                    if (Chi.LevelChecked() && OriginalHook(Ninjutsu) is HyoshoRanryu)
                    {
                        return OriginalHook(ChiCombo);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        if (HasStatusEffect(Buffs.Kassatsu) && Traits.EnhancedKasatsu.TraitLevelChecked())
                            return JinCombo;

                        if (Chi.LevelChecked())
                            return OriginalHook(ChiCombo);

                        if (Jin.LevelChecked())
                            return OriginalHook(JinCombo);
                    }
                }

                if (Chi.LevelChecked() && actionID == Chi)
                {
                    if (OriginalHook(Ninjutsu) is Hyoton)
                    {
                        return OriginalHook(TenCombo);
                    }

                    if (Jin.LevelChecked() && OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        return OriginalHook(JinCombo);
                    }
                }

                if (Jin.LevelChecked() && actionID == Jin)
                {
                    if (OriginalHook(Ninjutsu) is GokaMekkyaku or Katon)
                    {
                        return OriginalHook(ChiCombo);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        return OriginalHook(TenCombo);
                    }
                }

                return OriginalHook(Ninjutsu);
            }

            if (mudrapath == 2)
            {
                if (Ten.LevelChecked() && actionID == Ten)
                {
                    if (Chi.LevelChecked() && OriginalHook(Ninjutsu) is Hyoton or HyoshoRanryu)
                    {
                        return OriginalHook(Chi);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        if (Jin.LevelChecked())
                            return OriginalHook(JinCombo);

                        if (Chi.LevelChecked())
                            return OriginalHook(ChiCombo);
                    }
                }

                if (Chi.LevelChecked() && actionID == Chi)
                {
                    if (Jin.LevelChecked() && OriginalHook(Ninjutsu) is Katon or GokaMekkyaku)
                    {
                        return OriginalHook(Jin);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        return OriginalHook(Ten);
                    }
                }

                if (Jin.LevelChecked() && actionID == Jin)
                {
                    if (OriginalHook(Ninjutsu) is Raiton)
                    {
                        return OriginalHook(Ten);
                    }

                    if (OriginalHook(Ninjutsu) == GokaMekkyaku)
                    {
                        return OriginalHook(Chi);
                    }

                    if (OriginalHook(Ninjutsu) == FumaShuriken)
                    {
                        if (HasStatusEffect(Buffs.Kassatsu) && Traits.EnhancedKasatsu.TraitLevelChecked())
                            return OriginalHook(Ten);
                        return OriginalHook(Chi);
                    }
                }

                return OriginalHook(Ninjutsu);
            }

            return actionID;
        }
    }
    
    internal class NIN_Simple_MudrasAlt : CustomCombo
    {
        protected internal override Preset Preset => Preset.NIN_Simple_Mudras_Alt;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ten or Chi or Jin) || !HasStatusEffect(Buffs.Mudra))
                return actionID;
            
            switch (actionID)
            {
                case Ten when LevelChecked(HyoshoRanryu) && MudraReady && HasKassatsu:
                    return UseHyoshoRanryu(actionID);
                case Ten when LevelChecked(Suiton) && MudraReady && !HasStatusEffect(Buffs.ShadowWalker) && TrickCD <= 20:
                    return UseSuiton(actionID);
                case Ten when MudraReady:
                    return LevelChecked(Raiton)
                        ? UseRaiton(actionID)
                        : UseFumaShuriken(actionID);
                case Chi when LevelChecked(GokaMekkyaku) && MudraReady && HasKassatsu:
                    return UseGokaMekkyaku(actionID);
                case Chi when LevelChecked(Huton) && MudraReady && !HasStatusEffect(Buffs.ShadowWalker) && TrickCD <= 20:
                    return UseHuton(actionID);
                case Chi when MudraReady:
                    return LevelChecked(Katon)
                        ? UseKaton(actionID)
                        : UseFumaShuriken(actionID);
                case Jin:
                    return LevelChecked(Doton)
                        ? UseDoton(actionID)
                        : UseFumaShuriken(actionID);
                default:
                    return actionID;
            }
        }
    }
    
    #endregion
}
