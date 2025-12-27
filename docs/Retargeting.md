> [!TIP]
> See [The Example C#](RetargetingExample.cs) for barebones examples of how to 
> set up Retargeting for an action.

> [!IMPORTANT]
> Be sure to open the `Action Retargeting` section of the Debug tab, if you are 
> implementing any Retargeting yourself: `/wrath dbg`

## Capabilities of Action Retargeting

Action Retargeting is meant to remove our dependence on Redirect and primarily
Reaction also aims to open up new possibilities for features
that would hinge on targeting capabilities.\
As such, it has similar capabilities to Redirect and Reaction, but with more data
(since it's plugged into rotations).

What it can do:
- Retarget X action to the specified target.
- Retarget X action and Y other actions to the specified target.
- Retarget X (and Y other actions) to the specified target from a `TargetResolver`.
- Retarget X action from two different combos (with different Y other actions) to 
  the specified target.

What it cannot do:
- Retarget X action from two different combos (with the same Y other actions, or 
  lack thereof) to the specified target, they will overwrite each other.
  - Realistically, this would be a Conflict for the Presets anyway.

Given that it's pretty simple behavior to boot, there's not much to say about 
what it can't do, mostly it's just dealing with conflicts from two features trying
to Retarget the same action, which should always be possible to handle.

## Use Cases for Retargeting

Similar to how Wrath Combo was before `1.0.1.6`, Retargeting is primarily meant to
remove a dependency of ours and add a limited set of new features.\
It isn't meant to be the solution for Wrath to have Redirect built into everything.

It should be used:
- Where Redirect/Reaction was required before.
- Where you can add another action to a job's rotation that you could not before 
  without targeting.
- Where you would be applying beneficial effects to other players, and without 
  targeting this would require the user to target who you were wanting the 
  ability to be used on.\
  (healing actions, but also think stuff like mitigating the main tank when off-
  tanking)

### When Retargeting should NOT be used

Again, we do not want Wrath Combo to be a "rotation plugin, but with Redirect 
baked in", we maintain the same stance of still expecting the user to target 
their enemy.

It should not be used:
- When trying to use a hostile action that would normally be a non-targeted
  action as part of a rotation.
    - Of course, there may be exceptions (possibly for Hidden Features for getting
      aggro, for example), but this would be a path leading to Redirect just being
      baked into every aspect of Wrath, which is not a desirable goal.
- When trying to provide a target for a beneficial effect that requires one,
  where there is no consideration for the user's targets.
    - Even if the point of the feature is to perform logic to pick the best target,
      there should always be options for the user to manually target the action.\
      (For example, AST's Card Targeting or DNC's Dance Partner Features - both 
      have options to override the automatic target searching)

## Explanation of `Retarget`'s Signature

Retargeting should always be done with the `Retarget` extension method (though
`P.ActionRetargeting.Register()` is left exposed, if it must be used), which has 
a pretty simple signature, but will be explained fully here.\
You can see the signature, and its overloads, more closely in the `UIntExtensions`
class at the bottom of [ActionRetargeting.cs](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Core/ActionRetargeting.cs#L465).

```csharp
internal static uint Retarget
        (this uint action,
        [uint OR uint[]] replaced,                    // Optional, choice via overloads
        [IGameObject? OR Func<IGameObject?>] target)  // Choice via overloads
```

Parameters:
- `action` - The action to be retargeted.
  - Already supplied by the action that `Retarget` is chained onto:
    ```csharp
    SomeAction.Retarget(SimpleTarget.HardTarget); // SomeAction is the first parameter, `action`
    ```
- `replaced` - The action(s) to be retargeted.
  - These are the action(s) that will actually be *pressed* by the user, as in, the
    action(s) that can be on the hotbar - the Combo's Replaced Actions.
- `target` - The target to be used for the action.
  - This can also be a `TargetResolver`, which is just a method that also returns 
    a `IGameObject?` to be used as the target.
  - You should not *run* your `TargetResolver` in the `Retarget` call:
    ```csharp
    SomeAction.Retarget(MyCustomResolver()); // We lose your Resolver's name which is vital for debugging, and the resolver is run slightly earlier than it needs to be, leading to more perceived "lag" on the target it picks
    ```
    Just provide the same thing but without the `()` at the end of your method's 
    name.

## Using Retargeting

Implementing Retargeting can be very, very simple, and only requires 
`using WrathCombo.Core;` (and `using WrathCombo.CustomComboNS;`, for 
`SimpleTarget`, which your code almost certainly already has) and has a pretty 
simple signature (see the section above for more details).

To use it then, in its simplest form, instead of returning your action normally with:
```csharp
return action;
```

You would simply return the action, with Retargeting:
```csharp
return action.Retarget(SimpleTarget.AnyEnemy);
```

Then you would tag the Preset in `Preset.cs` with the
`[Retargeted]` or `[PossiblyRetargeted]` attribute, depending on whether the Preset
is always Retargeted or if it depends on another setting, respectively.
```csharp
[ReplaceSkill(AST.EssentialDignity)]
[CustomComboInfo("Retarget Essential Dignity Feature", "Will Retarget Essential Dignity outside of Healing combos to your Heal Stack.", AST.JobID)]
[Retargeted]
AST_RetargetEssentialDignity = 1059,
// This preset is always Retargeted if enabled
```
```csharp
[ParentCombo(AST_ST_DPS)]
[CustomComboInfo("Card Play Weave Option", "Weaves your Balance or Spear card (best used with Quick Target Cards)",
    AST.JobID)]
[PossiblyRetargeted("AST's Quick Target Damage Cards Feature", Condition.ASTQuickTargetCardsFeatureEnabled)]
AST_DPS_AutoPlay = 1037,
// This preset is only Retargeted if the listed setting is also enabled
// PossiblyRetargeted does have default values for those parameters, but they are for ST Healing Actions
```

> If you're adding a Possibly Retargeted Feature, you will need to also add the 
> Condition to the `Condition` enum in `RetargetedAttribute.cs`, and the logic 
> for that Condition to the `switch` in `Presets.DrawRetargetedAttribute()`.

### Using Retargeting with `replaced` Actions

Ultimately, `replaced` actions are just the actions the Combo goes onto, simple 
as that.\
This parameter should be the action, or list of actions, that your combo is set 
to replace.\
(See when you should provide `replaced` actions [below](#when-to-provide-replaced-actions))

In a combo that starts with:
```csharp
if (actionID is not (ClosedPosition or Ending)) return actionID;
```

The `replaced` actions to provide would simply be `[ClosedPosition, Ending]`:
```csharp
SomeAction.Retarget([ClosedPosition, Ending], SimpleTarget.SoftTarget);
```

---

It can be more complex, like in Healer-based classes, that have options to choose 
what actions are replaced by the combo.\
See AST, for example, whose Single Target combo begins with:
```csharp
bool actionFound = !alternateMode && MaleficList.Contains(actionID) ||
                   alternateMode && CombustList.ContainsKey(actionID);
if (!actionFound) return actionID;
```
Where the combo can replace a list of `Malefic` actions or `Combust` actions.

But you just need to derive a simple list of whichever is selected by the user, 
then you can use that just as simply:
```csharp
var replacedActions = alternateMode
    ? CombustList.Keys.ToArray()
    : MaleficList.ToArray();

// ...

Play1.Retarget(replacedActions, CardResolver);
```

#### When to Provide `replaced` Actions

This is fairly simple:
- If the `action` being Retargeted is NOT the same as the Replaced Action (not 
  plural) of the combo.

If your combo replaces multiple actions, they all need to be provided.\
If your combo replaces an action, and it is not the same as the action being 
Retargeted, it needs to be provided.

### Using Retargeting with a custom `TargetResolver`

As explained in the [Signature section](#explanation-of-retargets-signature) 
above, it is as simple as providing the name of your `TargetResolver` in the 
`target` parameter:

```csharp
SomeAction.Retarget(MyCustomResolver);
```

Note the lack of `()` at the end of the method name, as this is a reference to the 
method, not the result of running it.

## Setting up a custom `TargetResolver`

A custom `TargetResolver` is just a method that returns a `IGameObject?`, and as 
such is not inherently much more complex in concept than just providing a target.\
However, it does allow for a lot of flexibility in how you can pick your target,
you can build ***much*** more logic into it.

Here is a simple example of a `TargetResolver` that just returns the first party 
member suitable to the action (from [The Example C#](RetargetingExample.cs)):
```csharp
[ActionRetargeting.TargetResolver]
private static IGameObject? PartnerResolver() =>
    GetPartyMembers()
        .Where(member => member.GameObjectId != LocalPlayer.GameObjectId)
        .Where(member => !member.BattleChara.IsDead)
        .Where(member => InActionRange(ClosedPosition, member.BattleChara))
        .Where(member => !HasAnyPartner(member) || !HasMyPartner(member))
        .Select(member => member.BattleChara)
        .FirstOrDefault();
```

Note that the method *must* return a `IGameObject?`, and that it is marked with the 
`[ActionRetargeting.TargetResolver]` attribute.\
Without this attribute, the method will throw Warnings periodically, as the 
attribute has documentation point to `ActionRetargeting` for others to see and 
the author to reference, and indicates the behavior the method should 
exhibit.

There are two notable examples of more complex `TargetResolvers`
- [AST's `CardResolver`](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Combos/PvE/AST/AST_Helper.cs#L67)
  - This is only a simple masking method, to very simply fall back to using Cards 
    on self, which was not the desired behavior on `CardTarget` itself.
- [DNC's `DesirablePartnerResolver`](https://github.com/PunishXIV/WrathCombo/blob/main/WrathCombo/Combos/PvE/DNC/DNC_Helper.cs#L195)
  - This is only a simple masking method as it calls `TryGetDancePartner` in a 
    different way from `FeatureDesirablePartnerResolver`.

 (both methods *could* be the method that they call, instead of masks of them, 
 that's just how both of these worked out and is not a recommended pattern by any 
 means)
