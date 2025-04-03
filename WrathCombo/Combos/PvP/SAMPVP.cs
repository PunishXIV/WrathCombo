using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvP
{   
    internal static class SAMPvP
    {
        #region IDS
        public const byte JobID = 34;

        internal class Role : PvPMelee;

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
            Zantetsuken = 29537,
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
                Kuzushi = 3202;
        }
        #endregion

        #region Config
        public static class Config
        {
            public static UserInt
                SAMPvP_Soten_Range = new("SAMPvP_Soten_Range", 3),
                SAMPvP_Soten_Charges = new("SAMPvP_Soten_Charges", 1),
                SAMPvP_Chiten_PlayerHP = new("SAMPvP_Chiten_PlayerHP", 70),
                SAMPvP_Mineuchi_TargetHP = new("SAMPvP_Mineuchi_TargetHP", 40),
                SAMPvP_SmiteThreshold = new("SAMPvP_SmiteThreshold", 25);

            public static UserBool
                SAMPvP_Soten_SubOption = new("SAMPvP_Soten_SubOption"),
                SAMPvP_Mineuchi_SubOption = new("SAMPvP_Mineuchi_SubOption");

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    // Chiten
                    case CustomComboPreset.SAMPvP_Chiten:
                        UserConfig.DrawSliderInt(10, 100, SAMPvP.Config.SAMPvP_Chiten_PlayerHP, "Player HP%", 210);

                        break;

                    // Mineuchi
                    case CustomComboPreset.SAMPvP_Mineuchi:
                        UserConfig.DrawSliderInt(10, 100, SAMPvP.Config.SAMPvP_Mineuchi_TargetHP, "Target HP%", 210);

                        UserConfig.DrawAdditionalBoolChoice(SAMPvP.Config.SAMPvP_Mineuchi_SubOption, "Burst Preparation",
                            "Also uses Mineuchi before Tendo Setsugekka.");

                        break;

                    // Soten
                    case CustomComboPreset.SAMPvP_Soten:
                        UserConfig.DrawSliderInt(0, 2, SAMPvP.Config.SAMPvP_Soten_Charges, "Charges to Keep", 178);
                        UserConfig.DrawSliderInt(1, 10, SAMPvP.Config.SAMPvP_Soten_Range, "Maximum Range", 173);

                        UserConfig.DrawAdditionalBoolChoice(SAMPvP.Config.SAMPvP_Soten_SubOption, "Yukikaze Only",
                            "Also requires next weaponskill to be Yukikaze.");

                        break;

                    // Smite
                    case CustomComboPreset.SAMPvP_Smite:
                        UserConfig.DrawSliderInt(0, 100, SAMPvP.Config.SAMPvP_SmiteThreshold,
                            "Target HP% to smite, Max damage below 25%");

                        break;

                }
            }
        }
        #endregion
       
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
                    bool targetHasKuzushi = TargetHasEffect(Debuffs.Kuzushi);
                    bool hasKaeshiNamikiri = OriginalHook(OgiNamikiri) is Kaeshi;
                    bool hasTendo = OriginalHook(MeikyoShisui) is TendoSetsugekka;
                    bool isYukikazePrimed = ComboTimer == 0 || ComboAction is Kasha;
                    bool hasTendoKaeshi = OriginalHook(MeikyoShisui) is TendoKaeshiSetsugekka;
                    bool hasPrioWeaponskill = hasTendo || hasTendoKaeshi || hasKaeshiNamikiri;
                    bool isMeikyoPrimed = IsOnCooldown(OgiNamikiri) && !hasKaeshiNamikiri && !hasKaiten && !isMoving;
                    bool isZantetsukenPrimed = IsLB1Ready && !hasBind && hasTarget && targetHasKuzushi && targetDistance <= 20;
                    bool isSotenPrimed = chargesSoten > Config.SAMPvP_Soten_Charges && !hasKaiten && !hasBind && !hasPrioWeaponskill;
                    bool isTargetInvincible = TargetHasEffectAny(PLDPvP.Buffs.HallowedGround) || TargetHasEffectAny(DRKPvP.Buffs.UndeadRedemption);
                    #endregion

                    // Zantetsuken
                    if (IsEnabled(CustomComboPreset.SAMPvP_Zantetsuken) && isZantetsukenPrimed && !isTargetInvincible)
                        return OriginalHook(Zantetsuken);

                    //Smite
                    if (IsEnabled(CustomComboPreset.SAMPvP_Smite) && PvPMelee.CanSmite() && !PvPCommon.TargetImmuneToDamage() && GetTargetDistance() <= 10 && HasTarget() &&
                        GetTargetHPPercent() <= GetOptionValue(Config.SAMPvP_SmiteThreshold))
                        return PvPMelee.Smite;

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