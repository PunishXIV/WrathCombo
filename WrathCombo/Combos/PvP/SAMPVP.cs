using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.AutoRotation.AutoRotationController;

namespace WrathCombo.Combos.PvP
{
    internal static class SAMPvP
    {
        public static bool 已通知 = false;

        private static IGameObject LastTarget = null;

        public const byte JobID = 34;

        public const uint
            KashaCombo = 58,
            Yukikaze = 29523,
            Gekko = 29524,
            Kasha = 29525,
            Hyosetsu = 29526,
            Mangetsu = 29527,
            Oka = 29528,
            OgiNamikiri = 29530,
            Soten = 29532,
            Chiten = 29533,
            Mineuchi = 29535,
            MeikyoShisui = 29536,
            Midare = 29529,
            Kaeshi = 29531,
            斩铁剑 = 29537,
            TendoSetsugekka = 41454,
            TendoKaeshiSetsugekka = 41455,
            Zanshin = 41577;

        public static class Buffs
        {
            public const ushort
                Chiten = 1240,
                ZanshinReady = 1318,
                MeikyoShisui = 1320,
                Kaiten = 3201,
                TendoSetsugekkaReady = 3203;
        }

        public static class Debuffs
        {
            public const ushort
                崩破 = 3202;
        }

        public static class Config
        {
            public static UserInt
                SAMPvP_Soten_Range = new("SAMPvP_Soten_Range", 3),
                SAMPvP_Soten_Charges = new("SAMPvP_Soten_Charges", 1),
                SAMPvP_Chiten_PlayerHP = new("SAMPvP_Chiten_PlayerHP", 70),
                SAMPvP_Mineuchi_TargetHP = new("SAMPvP_Mineuchi_TargetHP", 40);

            public static UserBool
                SAMPvP_Soten_SubOption = new("SAMPvP_Soten_SubOption"),
                SAMPvP_Mineuchi_SubOption = new("SAMPvP_Mineuchi_SubOption");
        }

        private static unsafe IGameObject? 寻找斩铁剑目标()
        {
            var query = Svc.Objects.Where(x => !x.IsDead && x.IsHostile() && x.IsTargetable);
            if (!query.Any())
                return null;

            IGameObject? target = null;

            foreach (var t in query) {
                //不是冰
                if (CustomComboFunctions.GetTargetMaxHp(t) < 300000) {
                    //有自己的崩破
                    if(CustomComboFunctions.FindEffect(Debuffs.崩破, t, CustomComboFunctions.LocalPlayer?.GameObjectId) is not null) {
                        //范围够
                        if (CustomComboFunctions.InActionRange(CustomComboFunctions.OriginalHook(斩铁剑), t)) {
                            //不免疫
                            if (!PvPCommon.TargetImmuneToDamage2(false, t)) {
                                //血量低
                                if (CustomComboFunctions.GetTargetHPPercent(t, true) <= 100f) {
                                    target = t;
                                    break;
                                }
                            }
                        }
                    }
                   
                }
            }
            return target;
        }

        internal class SAMPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAMPvP_Burst;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is Yukikaze or Gekko or Kasha)
                {
                    #region Variables
                    float targetDistance = GetTargetDistance();
                    float targetCurrentPercentHp = GetTargetHPPercent();
                    float playerCurrentPercentHp = PlayerHealthPercentageHp();
                    uint chargesSoten = HasCharges(Soten) ? GetCooldown(Soten).RemainingCharges : 0;
                    bool isMoving = IsMoving();
                    bool inCombat = InCombat();
                    bool hasTarget = HasTarget();
                    bool inMeleeRange = targetDistance <= 5;
                    bool hasKaiten = HasEffect(Buffs.Kaiten);
                    bool hasZanshin = OriginalHook(Chiten) is Zanshin;
                    bool hasBind = HasEffectAny(PvPCommon.Debuffs.Bind);
                    bool targetHasImmunity = PvPCommon.TargetImmuneToDamage();
                    bool isTargetPrimed = hasTarget && !targetHasImmunity;
                    bool targetHasKuzushi = TargetHasEffect(Debuffs.崩破);
                    bool hasKaeshiNamikiri = OriginalHook(OgiNamikiri) is Kaeshi;
                    bool hasTendo = OriginalHook(MeikyoShisui) is TendoSetsugekka;
                    bool isYukikazePrimed = ComboTimer == 0 || ComboAction is Kasha;
                    bool hasTendoKaeshi = OriginalHook(MeikyoShisui) is TendoKaeshiSetsugekka;
                    bool hasPrioWeaponskill = hasTendo || hasTendoKaeshi || hasKaeshiNamikiri;
                    bool isMeikyoPrimed = IsOnCooldown(OgiNamikiri) && !hasKaeshiNamikiri && !hasKaiten && !isMoving;
                    bool isZantetsukenPrimed = IsLB1Ready && !hasBind && hasTarget && targetHasKuzushi && targetDistance <= 20;
                    bool isSotenPrimed = chargesSoten > Config.SAMPvP_Soten_Charges && !hasKaiten && !hasBind && !hasPrioWeaponskill;
                    bool isTargetInvincible = TargetHasEffectAny(PLDPvP.Buffs.HallowedGround) || TargetHasEffectAny(DRKPvP.Buffs.UndeadRedemption) || TargetHasEffectAny(PLDPvP.Buffs.被保护);
                    bool 盾不厚 = GetTargetHPPercent(CurrentTarget, true) <= 100f;
                    #endregion

                    //处理斩铁剑
                    if (IsEnabled(CustomComboPreset.SAMPvP_Zantetsuken)) {
                        if (IsLB1Ready) {
                            var target = 寻找斩铁剑目标();
                            if (target != null) {
                                CustomComboFunctions.TargetObject(target);
                                //再次确认
                                if (HasTarget() && targetHasKuzushi && !isTargetInvincible && 盾不厚)
                                    return OriginalHook(斩铁剑);
                            }
                        }
                    }


                    //// Zantetsuken
                    //if (IsEnabled(CustomComboPreset.SAMPvP_Zantetsuken) && isZantetsukenPrimed && !isTargetInvincible)
                    //    return OriginalHook(斩铁剑);

                    // Chiten
                    if (IsEnabled(CustomComboPreset.SAMPvP_Chiten) && IsOffCooldown(Chiten) && inCombat && playerCurrentPercentHp < Config.SAMPvP_Chiten_PlayerHP)
                        return OriginalHook(Chiten);

                    if (isTargetPrimed)
                    {
                        // Zanshin
                        if (hasZanshin && targetDistance <= 8)
                            return OriginalHook(Chiten);

                        // Soten
                        if (IsEnabled(CustomComboPreset.SAMPvP_Soten) && isSotenPrimed && targetDistance <= Config.SAMPvP_Soten_Range &&
                            (!Config.SAMPvP_Soten_SubOption || (Config.SAMPvP_Soten_SubOption && isYukikazePrimed)))
                            return OriginalHook(Soten);

                        if (inMeleeRange)
                        {
                            // Meikyo Shisui
                            if (IsEnabled(CustomComboPreset.SAMPvP_Meikyo) && IsOffCooldown(MeikyoShisui) && isMeikyoPrimed)
                                return OriginalHook(MeikyoShisui);

                            // Mineuchi
                            if (IsEnabled(CustomComboPreset.SAMPvP_Mineuchi) && IsOffCooldown(Mineuchi) && !HasBattleTarget() &&
                                (targetCurrentPercentHp < Config.SAMPvP_Mineuchi_TargetHP || (Config.SAMPvP_Mineuchi_SubOption && hasTendo && !hasKaiten)))
                                return OriginalHook(Mineuchi);
                        }
                    }

                    // Tendo Kaeshi Setsugekka
                    if (hasTendoKaeshi)
                        return OriginalHook(MeikyoShisui);

                    // Kaeshi Namikiri
                    if (hasKaeshiNamikiri)
                        return OriginalHook(OgiNamikiri);

                    // Kaiten
                    if (hasKaiten)
                        return OriginalHook(actionID);

                    if (!isMoving && isTargetPrimed)
                    {
                        // Tendo Setsugekka
                        if (hasTendo)
                            return OriginalHook(MeikyoShisui);

                        // Ogi Namikiri
                        if (IsOffCooldown(OgiNamikiri))
                            return OriginalHook(OgiNamikiri);
                    }
                }

                return actionID;
            }
        }
    }
}