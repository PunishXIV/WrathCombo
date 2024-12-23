#region

using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class WHM
{
    public const byte ClassID = 6;
    public const byte JobID = 24;

    public const uint

        // Heals
        Cure = 120,
        Cure2 = 135,
        Cure3 = 131,
        Regen = 137,
        AfflatusSolace = 16531,
        AfflatusRapture = 16534,
        Raise = 125,
        Benediction = 140,
        AfflatusMisery = 16535,
        Medica1 = 124,
        Medica2 = 133,
        Medica3 = 37010,
        Tetragrammaton = 3570,
        DivineBenison = 7432,
        Aquaveil = 25861,
        DivineCaress = 37011,

        // DPS
        Glare1 = 16533,
        Glare3 = 25859,
        Glare4 = 37009,
        Stone1 = 119,
        Stone2 = 127,
        Stone3 = 3568,
        Stone4 = 7431,
        Assize = 3571,
        Holy = 139,
        Holy3 = 25860,

        // DoT
        Aero = 121,
        Aero2 = 132,
        Dia = 16532,

        // Buffs
        ThinAir = 7430,
        PresenceOfMind = 136,
        PlenaryIndulgence = 7433;

    //Action Groups
    internal static readonly List<uint>
        StoneGlareList = [Stone1, Stone2, Stone3, Stone4, Glare1, Glare3];

    //Debuff Pairs of Actions and Debuff
    internal static readonly Dictionary<uint, ushort>
        AeroList = new()
        {
            { Aero, Debuffs.Aero },
            { Aero2, Debuffs.Aero2 },
            { Dia, Debuffs.Dia }
        };

    public static class Buffs
    {
        public const ushort
            Regen = 158,
            Medica2 = 150,
            Medica3 = 3880,
            PresenceOfMind = 157,
            ThinAir = 1217,
            DivineBenison = 1218,
            Aquaveil = 2708,
            SacredSight = 3879,
            DivineGrace = 3881;
    }

    public static class Debuffs
    {
        public const ushort
            Aero = 143,
            Aero2 = 144,
            Dia = 1871;
    }

    internal class WHM_SolaceMisery : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_SolaceMisery;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is AfflatusSolace && BloodLilies == 3
                ? AfflatusMisery
                : actionID;
        }
    }

    internal class WHM_RaptureMisery : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_RaptureMisery;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is AfflatusRapture && BloodLilies == 3
                ? AfflatusMisery
                : actionID;
        }
    }

    internal class WHM_CureSync : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_CureSync;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is Cure2 && !LevelChecked(Cure2)
                ? Cure
                : actionID;
        }
    }

    internal class WHM_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_Raise;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is All.Swiftcast)
            {
                bool thinAirReady = !HasEffect(Buffs.ThinAir) && LevelChecked(ThinAir) && HasCharges(ThinAir);

                if (HasEffect(All.Buffs.Swiftcast))
                    return IsEnabled(CustomComboPreset.WHM_ThinAirRaise) && thinAirReady
                        ? ThinAir
                        : Raise;
            }

            return actionID;
        }
    }

    internal class WHM_ST_MainCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_ST_MainCombo;
        
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            bool ActionFound;

            if (Config.WHM_ST_MainCombo_Adv && Config.WHM_ST_MainCombo_Adv_Actions.Count > 0)
            {
                bool onStones = Config.WHM_ST_MainCombo_Adv_Actions[0] && StoneGlareList.Contains(actionID);
                bool onAeros = Config.WHM_ST_MainCombo_Adv_Actions[1] && AeroList.ContainsKey(actionID);
                bool onStone2 = Config.WHM_ST_MainCombo_Adv_Actions[2] && actionID is Stone2;
                ActionFound = onStones || onAeros || onStone2;
            }
            else
            {
                ActionFound = StoneGlareList.Contains(actionID); //default handling
            }

            if (ActionFound)
            {
                if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Opener))
                    if (Opener().FullOpener(ref actionID))
                        return actionID;

                bool liliesFull = gauge.Lily == 3;
                bool liliesNearlyFull = gauge.Lily == 2 && gauge.LilyTimer >= 17000;

                if (CanSpellWeave())
                {
                    bool lucidReady = ActionReady(All.LucidDreaming) && LevelChecked(All.LucidDreaming) &&
                                      LocalPlayer.CurrentMp <= Config.WHM_STDPS_Lucid;
                    bool pomReady = LevelChecked(PresenceOfMind) && IsOffCooldown(PresenceOfMind);
                    bool assizeReady = LevelChecked(Assize) && IsOffCooldown(Assize);
                    bool pomEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_PresenceOfMind);
                    bool assizeEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Assize);
                    bool lucidEnabled = IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Lucid);

                    if (IsEnabled(CustomComboPreset.WHM_DPS_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave())
                        return Variant.VariantRampart;

                    if (pomEnabled && pomReady)
                        return PresenceOfMind;

                    if (assizeEnabled && assizeReady)
                        return Assize;

                    if (lucidEnabled && lucidReady)
                        return All.LucidDreaming;
                }

                if (InCombat())
                {
                    // DoTs
                    if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_DoT) && LevelChecked(Aero) && HasBattleTarget() &&
                        AeroList.TryGetValue(OriginalHook(Aero), out ushort dotDebuffID))
                    {
                        if (IsEnabled(CustomComboPreset.WHM_DPS_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            GetDebuffRemainingTime(Variant.Debuffs.SustainedDamage) <= 3 &&
                            CanSpellWeave())
                            return Variant.VariantSpiritDart;

                        // DoT Uptime & HP% threshold
                        float refreshtimer =
                            Config.WHM_ST_MainCombo_DoT_Adv ? Config.WHM_ST_MainCombo_DoT_Threshold : 3;

                        if (GetDebuffRemainingTime(dotDebuffID) <= refreshtimer &&
                            GetTargetHPPercent() > Config.WHM_STDPS_MainCombo_DoT)
                            return OriginalHook(Aero);
                    }

                    // Glare IV
                    if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_GlareIV)
                        && HasEffect(Buffs.SacredSight)
                        && GetBuffStacks(Buffs.SacredSight) > 0)
                        return OriginalHook(Glare4);

                    if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_LilyOvercap) && LevelChecked(AfflatusRapture) &&
                        (liliesFull || liliesNearlyFull))
                        return AfflatusRapture;

                    if (IsEnabled(CustomComboPreset.WHM_ST_MainCombo_Misery_oGCD) && LevelChecked(AfflatusMisery) &&
                        gauge.BloodLily >= 3)
                        return AfflatusMisery;

                    return OriginalHook(Stone1);
                }
            }

            return actionID;
        }
    }

    internal class WHM_AoEHeals : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_AoEHeals;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is Medica1)
            {
                bool thinAirReady = LevelChecked(ThinAir) && !HasEffect(Buffs.ThinAir) &&
                                    GetRemainingCharges(ThinAir) > Config.WHM_AoEHeals_ThinAir;
                bool canWeave = CanSpellWeave(0.3);
                bool lucidReady = ActionReady(All.LucidDreaming) && LocalPlayer.CurrentMp <= Config.WHM_AoEHeals_Lucid;

                bool plenaryReady = ActionReady(PlenaryIndulgence) &&
                                    (!Config.WHM_AoEHeals_PlenaryWeave ||
                                     (Config.WHM_AoEHeals_PlenaryWeave && canWeave));
                bool divineCaressReady = ActionReady(DivineCaress) && HasEffect(Buffs.DivineGrace);

                bool assizeReady = ActionReady(Assize) &&
                                   (!Config.WHM_AoEHeals_AssizeWeave || (Config.WHM_AoEHeals_AssizeWeave && canWeave));

                IGameObject? healTarget = OptionalTarget ??
                                          (Config.WHM_AoEHeals_MedicaMO
                                              ? GetHealTarget(Config.WHM_AoEHeals_MedicaMO)
                                              : LocalPlayer);
                Status? hasMedica2 = FindEffect(Buffs.Medica2, healTarget, LocalPlayer?.GameObjectId);
                Status? hasMedica3 = FindEffect(Buffs.Medica3, healTarget, LocalPlayer?.GameObjectId);

                if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Assize) && assizeReady)
                    return Assize;

                if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Plenary) && plenaryReady)
                    return PlenaryIndulgence;

                if (IsEnabled(CustomComboPreset.WHM_AoEHeals_DivineCaress) && divineCaressReady)
                    return OriginalHook(DivineCaress);

                if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Lucid) && canWeave && lucidReady)
                    return All.LucidDreaming;

                if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Misery) && gauge.BloodLily == 3)
                    return AfflatusMisery;

                if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Rapture) && LevelChecked(AfflatusRapture) &&
                    gauge.Lily > 0)
                    return AfflatusRapture;

                if (IsEnabled(CustomComboPreset.WHM_AoEHeals_ThinAir) && thinAirReady)
                    return ThinAir;

                if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Medica2)
                    && ((hasMedica2 == null && hasMedica3 == null) // No Medica buffs
                        || (hasMedica2 != null &&
                            hasMedica2.RemainingTime <=
                            Config.WHM_AoEHeals_MedicaTime) // Medica buff, but falling off soon
                        || (hasMedica3 != null && hasMedica3.RemainingTime <= Config.WHM_AoEHeals_MedicaTime)) // ^
                    && (ActionReady(Medica2) || ActionReady(Medica3)))
                    return LevelChecked(Medica3) ? Medica3 : Medica2;

                if (IsEnabled(CustomComboPreset.WHM_AoEHeals_Cure3)
                    && ActionReady(Cure3)
                    && (LocalPlayer.CurrentMp >= Config.WHM_AoEHeals_Cure3MP
                        || HasEffect(Buffs.ThinAir)))
                    return Cure3;
            }

            return actionID;
        }
    }

    internal class WHM_ST_Heals : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_STHeals;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is Cure)
            {
                IGameObject? healTarget = OptionalTarget ?? GetHealTarget(Config.WHM_STHeals_UIMouseOver);

                bool thinAirReady = LevelChecked(ThinAir) && !HasEffect(Buffs.ThinAir) &&
                                    GetRemainingCharges(ThinAir) > Config.WHM_STHeals_ThinAir;

                bool regenReady = ActionReady(Regen) && (FindEffectOnMember(Buffs.Regen, healTarget) is null ||
                                                         FindEffectOnMember(Buffs.Regen, healTarget)?.RemainingTime <=
                                                         Config.WHM_STHeals_RegenTimer);

                if (IsEnabled(CustomComboPreset.WHM_STHeals_Esuna) && ActionReady(All.Esuna) &&
                    GetTargetHPPercent(healTarget, Config.WHM_STHeals_IncludeShields) >= Config.WHM_STHeals_Esuna &&
                    HasCleansableDebuff(healTarget))
                    return All.Esuna;

                if (IsEnabled(CustomComboPreset.WHM_STHeals_Lucid) &&
                    All.CanUseLucid(actionID, Config.WHM_STHeals_Lucid))
                    return All.LucidDreaming;

                foreach (int prio in Config.WHM_ST_Heals_Priority.Items.OrderBy(x => x))
                {
                    int index = Config.WHM_ST_Heals_Priority.IndexOf(prio);
                    int config = WHMHelper.GetMatchingConfigST(index, OptionalTarget, out uint spell, out bool enabled);

                    if (enabled)
                        if (GetTargetHPPercent(healTarget, Config.WHM_STHeals_IncludeShields) <= config &&
                            ActionReady(spell))
                            return spell;
                }

                if (IsEnabled(CustomComboPreset.WHM_STHeals_Regen) && regenReady)
                    return Regen;

                if (IsEnabled(CustomComboPreset.WHM_STHeals_Solace) && gauge.Lily > 0 && ActionReady(AfflatusSolace))
                    return AfflatusSolace;

                if (IsEnabled(CustomComboPreset.WHM_STHeals_ThinAir) && thinAirReady)
                    return ThinAir;

                if (ActionReady(Cure2))
                    return Cure2;
            }

            return actionID;
        }
    }

    internal class WHM_AoE_DPS : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WHM_AoE_DPS;

        internal static int AssizeCount => ActionWatching.CombatActions.Count(x => x == Assize);

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is Holy or Holy3)
            {
                bool liliesFullNoBlood = gauge.Lily == 3 && gauge.BloodLily < 3;
                bool liliesNearlyFull = gauge.Lily == 2 && gauge.LilyTimer >= 17000;
                bool PresenceOfMindReady = ActionReady(PresenceOfMind) && !Config.WHM_AoEDPS_PresenceOfMindWeave;

                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_SwiftHoly) &&
                    ActionReady(All.Swiftcast) &&
                    AssizeCount == 0 && !IsMoving() && InCombat())
                    return All.Swiftcast;

                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_SwiftHoly) &&
                    WasLastAction(All.Swiftcast))
                    return actionID;

                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Assize) && ActionReady(Assize))
                    return Assize;

                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_PresenceOfMind) && PresenceOfMindReady)
                    return PresenceOfMind;

                if (IsEnabled(CustomComboPreset.WHM_DPS_Variant_Rampart) &&
                    IsEnabled(Variant.VariantRampart) &&
                    IsOffCooldown(Variant.VariantRampart))
                    return Variant.VariantRampart;

                if (IsEnabled(CustomComboPreset.WHM_DPS_Variant_SpiritDart) &&
                    IsEnabled(Variant.VariantSpiritDart) &&
                    GetDebuffRemainingTime(Variant.Debuffs.SustainedDamage) <= 3 &&
                    HasBattleTarget())
                    return Variant.VariantSpiritDart;

                if (CanSpellWeave() || IsMoving())
                {
                    if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_PresenceOfMind) && ActionReady(PresenceOfMind))
                        return PresenceOfMind;

                    if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Lucid) && ActionReady(All.LucidDreaming) &&
                        LocalPlayer.CurrentMp <= Config.WHM_AoEDPS_Lucid)
                        return All.LucidDreaming;
                }

                // Glare IV
                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_GlareIV)
                    && HasEffect(Buffs.SacredSight)
                    && GetBuffStacks(Buffs.SacredSight) > 0)
                    return OriginalHook(Glare4);

                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_LilyOvercap) && LevelChecked(AfflatusRapture) &&
                    (liliesFullNoBlood || liliesNearlyFull))
                    return AfflatusRapture;

                if (IsEnabled(CustomComboPreset.WHM_AoE_DPS_Misery) && LevelChecked(AfflatusMisery) &&
                    gauge.BloodLily >= 3 && HasBattleTarget())
                    return AfflatusMisery;
            }

            return actionID;
        }
    }
}