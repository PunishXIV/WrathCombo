using ECommons.GameHelpers;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.NINPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class NINPvP
{
    #region IDS
    internal class Role : PvPMelee;
    internal const uint
        SpinningEdge = 29500,
        GustSlash = 29501,
        AeolianEdge = 29502,
        FumaShuriken = 29505,
        Dokumori = 41451,
        ThreeMudra = 29507,
        Bunshin = 29511,
        Shukuchi = 29513,
        SeitonTenchu = 29515,
        ForkedRaiju = 29510,
        FleetingRaiju = 29707,
        HyoshoRanryu = 29506,
        GokaMekkyaku = 29504,
        Meisui = 29508,
        Huton = 29512,
        Doton = 29514,
        Assassinate = 29503,
        ZeshoMeppo = 41452;

    internal class Buffs
    {
        internal const ushort
            ThreeMudra = 1317,
            Hidden = 1316,
            Bunshin = 2010,
            ShadeShift = 2011,
            SeitonUnsealed = 3192,
            FleetingRaijuReady = 3211,
            ZeshoMeppoReady = 4305;
    }
    internal class Debuffs
    {
        internal const ushort
            SealedHyoshoRanryu = 3194,
            SealedGokaMekkyaku = 3193,
            SealedHuton = 3196,
            SealedDoton = 3197,
            SeakedForkedRaiju = 3195,
            SealedMeisui = 3198,                
            Dokumori = 4303;
    }
    #endregion

    #region Config
    public static class Config
    {
        public static UserInt
            NINPvP_Meisui_ST = new("NINPvP_Meisui_ST"),
            NINPvP_Meisui_AoE = new("NINPvP_Meisui_AoE"),
            NINPVP_SeitonTenchu = new("NINPVP_SeitonTenchu"),
            NINPVP_SeitonTenchuAoE = new("NINPVP_SeitonTenchuAoE"),
            NINPvP_SmiteThreshold = new("NINPvP_SmiteThreshold");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.NINPvP_ST_SeitonTenchu:
                    DrawSliderInt(1, 50, NINPVP_SeitonTenchu, "Target's HP% to be at or under", 200);
                    break;
                case Preset.NINPvP_AoE_SeitonTenchu:
                    DrawSliderInt(1, 50, NINPVP_SeitonTenchuAoE, "Target's HP% to be at or under", 200);
                    break;
                case Preset.NINPvP_Smite:
                    DrawSliderInt(0, 100, NINPvP_SmiteThreshold,
                        "Target HP% to smite, Max damage below 25%");
                    break;

                case Preset.NINPvP_ST_Meisui:
                    string descriptionST = "Set the HP percentage to be at or under for the feature to kick in.\n100% is considered to start at 8,000 less than your max HP to prevent wastage.";

                    if (Player.Object != null)
                    {
                        uint maxHP = Player.Object.MaxHp <= 8000 ? 0 : Player.Object.MaxHp - 8000;
                        if (maxHP > 0)
                        {
                            float hpThreshold = (float)maxHP / 100 * NINPvP_Meisui_ST;

                            descriptionST += $"\nHP Value to be at or under: {hpThreshold}";
                        }
                    }
                    DrawSliderInt(1, 100, NINPvP_Meisui_ST, descriptionST);
                    break;

                case Preset.NINPvP_AoE_Meisui:
                    string descriptionAoE = "Set the HP percentage to be at or under for the feature to kick in.\n100% is considered to start at 8,000 less than your max HP to prevent wastage.";

                    if (Player.Object != null)
                    {
                        uint maxHP = Player.Object.MaxHp <= 8000 ? 0 : Player.Object.MaxHp - 8000;
                        if (maxHP > 0)
                        {
                            float hpThreshold = (float)maxHP / 100 * NINPvP_Meisui_AoE;

                            descriptionAoE += $"\nHP Value to be at or under: {hpThreshold}";
                        }
                    }
                    DrawSliderInt(1, 100, NINPvP_Meisui_AoE, descriptionAoE);
                    break;
            }
        }
    }
    #endregion
       
    internal class NINPvP_ST_BurstMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.NINPvP_ST_BurstMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SpinningEdge or GustSlash or AeolianEdge)) 
                return actionID;
            
            // Cached variables for repeated conditions
            var threeMudrasCD = GetCooldown(ThreeMudra);
            var fumaCD = GetCooldown(FumaShuriken);
            var bunshinStacks = HasStatusEffect(Buffs.Bunshin) ? GetStatusEffectStacks(Buffs.Bunshin) : 0;
            bool canWeave = CanWeave();
            bool mudraMode = HasStatusEffect(Buffs.ThreeMudra);
            bool inMeleeRange = InMeleeRange();
            bool isHidden = HasStatusEffect(Buffs.Hidden);
            var jobMaxHp = LocalPlayer.MaxHp;
            var maxHPThreshold = jobMaxHp - 8000;
            float remainingPercentage = (float)LocalPlayer.CurrentHp / maxHPThreshold;
            bool inMeisuiRange = (NINPvP_Meisui_ST) >= (remainingPercentage * 100);

            // Hidden state actions
            if (isHidden)
                return OriginalHook(Assassinate);

            if (!PvPCommon.TargetImmuneToDamage())
            {
                // Seiton Tenchu priority for targets below 50% HP
                if (IsEnabled(Preset.NINPvP_ST_SeitonTenchu) && GetTargetHPPercent() < (NINPVP_SeitonTenchu) &&
                    (IsLB1Ready || HasStatusEffect(Buffs.SeitonUnsealed)))  // Limit Break or Unsealed buff
                    return OriginalHook(SeitonTenchu);

                //Smite
                if (IsEnabled(Preset.NINPvP_Smite) && PvPMelee.CanSmite() && GetTargetDistance() <= 10 && HasTarget() &&
                    GetTargetHPPercent() <= (NINPvP_SmiteThreshold))
                    return PvPMelee.Smite;

                // Zesho Meppo
                if (HasStatusEffect(Buffs.ZeshoMeppoReady) && InMeleeRange())
                    return ZeshoMeppo;

                if (canWeave)
                {
                    // Melee range actions
                    if (IsEnabled(Preset.NINPvP_ST_Dokumori) && inMeleeRange && !GetCooldown(Dokumori).IsCooldown)
                        return OriginalHook(Dokumori);

                    // Bunshin
                    if (IsEnabled(Preset.NINPvP_ST_Bunshin) && !GetCooldown(Bunshin).IsCooldown)
                        return OriginalHook(Bunshin);

                    // Three Mudra
                    if (IsEnabled(Preset.NINPvP_ST_ThreeMudra) && threeMudrasCD.RemainingCharges > 0 && !mudraMode)
                    {
                        if (!IsEnabled(Preset.NINPvP_ST_ThreeMudraPool) || HasStatusEffect(Buffs.Bunshin))
                            return OriginalHook(ThreeMudra);
                    }  
                }
                // Mudra mode actions
                if (mudraMode)
                {
                    if (IsEnabled(Preset.NINPvP_ST_Meisui) && inMeisuiRange && !HasStatusEffect(Debuffs.SealedMeisui))
                        return OriginalHook(Meisui);

                    if (IsEnabled(Preset.NINPvP_ST_MudraMode))
                    {
                        if (!HasStatusEffect(Debuffs.SealedHyoshoRanryu))
                            return OriginalHook(HyoshoRanryu);

                        if (!HasStatusEffect(Debuffs.SeakedForkedRaiju) && bunshinStacks > 0)
                            return OriginalHook(ForkedRaiju);

                        if (!HasStatusEffect(Debuffs.SealedHuton))
                            return OriginalHook(Huton);
                    }
                    else return actionID;
                }
                // Fuma Shuriken
                if (IsEnabled(Preset.NINPvP_ST_FumaShuriken) && fumaCD.RemainingCharges > 0 && !HasStatusEffect(Buffs.FleetingRaijuReady))
                    return OriginalHook(FumaShuriken);
            }
            return actionID;
        }
    }

    internal class NINPvP_AoE_BurstMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.NINPvP_AoE_BurstMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FumaShuriken) 
                return actionID;
            
            var threeMudrasCD = GetCooldown(ThreeMudra);
            var fumaCD = GetCooldown(FumaShuriken);
            bool meisuiLocked = HasStatusEffect(Debuffs.SealedMeisui);
            bool dotonLocked = HasStatusEffect(Debuffs.SealedDoton);
            bool gokaLocked = HasStatusEffect(Debuffs.SealedGokaMekkyaku);
            bool mudraMode = HasStatusEffect(Buffs.ThreeMudra);
            bool canWeave = CanWeave();
            var jobMaxHp = LocalPlayer.MaxHp;
            var threshold = NINPvP_Meisui_AoE;
            var maxHPThreshold = jobMaxHp - 8000;
            var remainingPercentage = (float)LocalPlayer.CurrentHp / (float)maxHPThreshold;
            bool inMeisuiRange = threshold >= (remainingPercentage * 100);

            if (HasStatusEffect(Buffs.Hidden))
                return OriginalHook(Assassinate);

            if (!PvPCommon.TargetImmuneToDamage())
            {
                // Seiton Tenchu priority for targets below 50% HP
                if (IsEnabled(Preset.NINPvP_AoE_SeitonTenchu) && GetTargetHPPercent() < (NINPVP_SeitonTenchu) && IsLB1Ready)
                    return OriginalHook(SeitonTenchu);

                if (canWeave)
                {
                    if (IsEnabled(Preset.NINPvP_AoE_Dokumori) && InMeleeRange() && !GetCooldown(Dokumori).IsCooldown)
                        return OriginalHook(Dokumori);

                    if (IsEnabled(Preset.NINPvP_AoE_Bunshin) && !GetCooldown(Bunshin).IsCooldown)
                        return OriginalHook(Bunshin);

                    // Three Mudra
                    if (IsEnabled(Preset.NINPvP_AoE_ThreeMudra) && threeMudrasCD.RemainingCharges > 0 && !mudraMode)
                    {
                        if (!IsEnabled(Preset.NINPvP_AoE_ThreeMudraPool) || HasStatusEffect(Buffs.Bunshin))
                            return OriginalHook(ThreeMudra);
                    }
                }

                if (mudraMode)
                {
                    if (IsEnabled(Preset.NINPvP_AoE_MudraMode))
                    {
                        if (IsEnabled(Preset.NINPvP_AoE_Meisui) && inMeisuiRange && !meisuiLocked)
                            return OriginalHook(Meisui);

                        if (!dotonLocked)
                            return OriginalHook(Doton);

                        if (!gokaLocked)
                            return OriginalHook(GokaMekkyaku);
                    }
                    else return actionID;  // if automatic is not enabled and in mudra mode, ensures fuma shuriken is the option so mudras can be properly chosen
                }

                if (IsEnabled(Preset.NINPvP_AoE_FumaShuriken) && fumaCD.RemainingCharges > 0)
                    return OriginalHook(FumaShuriken);

                if (InMeleeRange()) // Melee Combo
                {
                    switch (ComboAction)
                    {
                        case GustSlash:
                            return OriginalHook(AeolianEdge);
                        case SpinningEdge:
                            return OriginalHook(GustSlash);
                        default:
                            return OriginalHook(SpinningEdge);
                    }
                }
            }
            return actionID;
        }
    }
}