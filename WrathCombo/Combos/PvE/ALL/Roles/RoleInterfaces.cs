using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal static partial class RoleActions
{
        #region Action-Specific Interfaces
    // Action-specific interfaces
    internal interface ILucidDreaming
    {
        uint LucidDreaming { get; }
        bool CanLucidDream(int MPThreshold, bool spellweave = true);
    }

    internal interface ISwiftcast
    {
        uint Swiftcast { get; }
        bool CanSwiftcast(bool spellweave = true);
    }

    internal interface ISurecast
    {
        uint Surecast { get; }
        bool CanSurecast();
    }

    internal interface IAddle
    {
        uint Addle { get; }
        bool CanAddle();
    }

    internal interface ISleep
    {
        uint Sleep { get; }
        bool CanSleep();
    }

    internal interface IRepose
    {
        uint Repose { get; }
        bool CanRepose();
    }

    internal interface IEsuna
    {
        uint Esuna { get; }
        bool CanEsuna();
    }

    internal interface IRescue
    {
        uint Rescue { get; }
        bool CanRescue();
    }

    internal interface ISecondWind
    {
        uint SecondWind { get; }
        bool CanSecondWind(int healthpercent);
    }

    internal interface IArmsLength
    {
        uint ArmsLength { get; }
        bool CanArmsLength();
        bool CanArmsLength(int enemyCount, UserInt? avoidanceSetting = null);
        bool CanArmsLength(int enemyCount, All.Enums.BossAvoidance avoidanceSetting);
    }

    internal interface ILegGraze
    {
        uint LegGraze { get; }
        bool CanLegGraze();
    }

    internal interface IFootGraze
    {
        uint FootGraze { get; }
        bool CanFootGraze();
    }

    internal interface IPeloton
    {
        uint Peloton { get; }
        bool CanPeloton();
    }

    internal interface IHeadGraze
    {
        uint HeadGraze { get; }
        internal bool CanHeadGraze(Preset preset, WeaveTypes weave = WeaveTypes.None);

        internal bool CanHeadGraze(bool simpleMode, WeaveTypes weave = WeaveTypes.None);
    }

    internal interface ILegSweep
    {
        uint LegSweep { get; }
        bool CanLegSweep();
    }

    internal interface IBloodbath
    {
        uint Bloodbath { get; }
        bool CanBloodBath(int healthpercent);
    }

    internal interface IFeint
    {
        uint Feint { get; }
        bool CanFeint();
    }

    internal interface ITrueNorth
    {
        uint TrueNorth { get; }
        bool CanTrueNorth();
    }

    internal interface IRampart
    {
        uint Rampart { get; }
        bool CanRampart(int healthPercent = 100);
    }

    internal interface ILowBlow
    {
        uint LowBlow { get; }
        bool CanLowBlow();
    }

    internal interface IProvoke
    {
        uint Provoke { get; }
        bool CanProvoke();
    }

    internal interface IInterject
    {
        uint Interject { get; }
        bool CanInterject();
    }

    internal interface IReprisal
    {
        uint Reprisal { get; }
        bool CanReprisal(int healthPercent = 101, int? enemyCount = null, bool checkTargetForDebuff = true, IGameObject? target = null);
    }

    internal interface IShirk
    {
        uint Shirk { get; }
        bool CanShirk();
    }
        #endregion

        #region Status Effect Interfaces
    // Buff and Debuff interfaces
    internal interface IMagicBuffs
    {
        ushort Raise { get; }
        ushort Swiftcast { get; }
        ushort Surecast { get; }
    }

    internal interface ICasterBuffs : IMagicBuffs
    {
    }

    internal interface IHealerBuffs : IMagicBuffs
    {
    }

    internal interface IPhysicalRoleBuffs
    {
        ushort ArmsLength { get; }
    }

    internal interface IPhysRangedBuffs : IPhysicalRoleBuffs
    {
        ushort Peloton { get; }
    }

    internal interface IMeleeBuffs : IPhysicalRoleBuffs
    {
        ushort BloodBath { get; }
        ushort TrueNorth { get; }
    }

    internal interface ITankBuffs : IPhysicalRoleBuffs
    {
    }

    internal interface ICasterDebuffs
    {
        ushort Addle { get; }
    }

    internal interface IMeleeDebuffs
    {
        ushort Feint { get; }
    }

    internal interface ITankDebuffs
    {
        ushort Reprisal { get; }
    }
        #endregion

        #region Role Interfaces
    // Base interface for shared functionality / lists/ idfk
    public interface IRoleAction
    {
    }

    // Shared interfaces for Inheritance
    internal interface IMagicShared : IRoleAction, ILucidDreaming, ISwiftcast, ISurecast
    {
    }

    internal interface IPhysicalRoleShared : IRoleAction, ISecondWind, IArmsLength
    {
    }

    // Role-specific interfaces using inheritance
    internal interface IMagic : IMagicShared
    {
        IMagicBuffs Buffs { get; }
    }

    internal interface ICaster : IMagicShared, IAddle, ISleep
    {
        ICasterBuffs Buffs { get; }
        ICasterDebuffs Debuffs { get; }
    }

    internal interface IHealer : IMagicShared, IRepose, IEsuna, IRescue
    {
        IHealerBuffs Buffs { get; }
    }

    internal interface IPhysicalRole : IPhysicalRoleShared
    {
        IPhysicalRoleBuffs Buffs { get; }
    }

    internal interface IPhysicalRanged : IPhysicalRoleShared, ILegGraze, IFootGraze, IPeloton, IHeadGraze
    {
        IPhysRangedBuffs Buffs { get; }
    }

    internal interface IMelee : IPhysicalRoleShared, ILegSweep, IBloodbath, IFeint, ITrueNorth
    {
        IMeleeBuffs Buffs { get; }
        IMeleeDebuffs Debuffs { get; }
    }

    internal interface ITank : IPhysicalRoleShared, IRampart, ILowBlow, IProvoke, IInterject, IReprisal, IShirk
    {
        ITankBuffs Buffs { get; }
        ITankDebuffs Debuffs { get; }
    }
        #endregion
}