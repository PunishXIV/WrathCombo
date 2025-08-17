// Will make all instances of Closed Position return Closed Position,
// but targeted to the user's Hard Target.
// Will not work in a more complex combo,
// where Closed Position is not the button on the hotbar.
class Example1_SimpleFeature
{
    internal class DNC_DesirablePartner : CustomCombo
    {
        protected internal override Preset Preset => Preset.DNC_DesirablePartner;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ClosedPosition) return actionID;

            return ClosedPosition.Retarget(SimpleTarget.HardTarget);
        }
    }
}

// Will make the larger combo return Closed Position,
// targeted to the user's Hard Target,
// when the player does not have the Closed Position buff.
// Will work in a more complex combo, where the Replaced Actions passed to
// Retarget are the buttons that are being Pressed, not the action itself.
class Example2_StandardCombo
{
    internal class DNC_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DNC_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cascade) return actionID;

            // ... other actions

            if (CanWeave() && !HasStatusEffect(Buffs.ClosedPosition))
            // The Cascade parameter here can also be a list of actions,
            // like [Cascade, Fountain]
                return ClosedPosition.Retarget(Cascade, SimpleTarget.HardTarget);

            // ... other actions

            return actionID;
        }
    }
}

// Will make the larger combo return Closed Position,
// targeted to the target returned by the TargetResolver, where extra logic can be applied,
// when the player does not have the Closed Position buff.
class Example3_StandardComboWithComplexTargetResolving
{
    internal class DNC_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.DNC_ST_AdvancedMode;

        // If this attribute is lacking on a TargetResolver provided to Retarget,
        // you will get warnings
        [ActionRetargeting.TargetResolver]
        private static IGameObject? DancePartnerResolver() =>
            GetPartyMembers()
                .Where(member => member.GameObjectId != LocalPlayer.GameObjectId)
                .Where(member => !member.BattleChara.IsDead)
                .Where(member => InActionRange(ClosedPosition, member.BattleChara))
                .Where(member => !HasAnyPartner(member) || !HasMyPartner(member))
                .Select(member => member.BattleChara)
                .FirstOrDefault();
        // Or even more complex logic, like the actual implementation of this in `DNC_Helper.cs`

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cascade) return actionID;

            // ... other actions

            if (CanWeave() && !HasStatusEffect(Buffs.ClosedPosition))
            // The Cascade parameter here can also be a list of actions,
            // like [Cascade, Fountain]
                return ClosedPosition.Retarget(Cascade, DancePartnerResolver);

            // ... other actions

            return actionID;
        }
    }
}
