# Rotations
Rotations refer to the code behind the Combo presets, specifically the logic
that appears in the `Invoke` methods in the `Combos/` [`PvE`](/WrathCombo/Combos/PvE)
and [`PvP`](/WrathCombo/Combos/PvP)  folders.

All rotations should first and foremost follow
[The Balance](https://discord.gg/thebalanceffxiv)'s guidelines.

## Openers
Openers are hard-coded starts to rotations that are defined in `_Helper.cs`.
Every job needs to have at least one opener defined, and it should be as close
to The Balance's standard opener for that class as possible.

# Presets
Presets are the first set of options that are shown to users under each Job, and
includes Combos and Options.\
It does not include [Configs](#configs), which come from `_Config.cs` files, and are 
mostly UI code.

Presets are all defined in [`CustomComboPreset.cs`](/WrathCombo/Combos/CustomComboPreset.cs).

## Preset Templates
### Standard Preset Naming Template
- Simple Mode - Single Target
- Simple Mode - AoE
- Advanced Mode - Single Target
- Advanced Mode - AoE
- `<combo name>` Feature
  - `<option name>` Option

### [Healers] Healing Feature Naming Template
- Simple Heals - Single Target
- Simple Heals - AoE

### [Tanks] Mitigations Option template:
> NOTE: the Mitigation option must be called "Mitigation Options" in order to get highlighted.

- Simple Mode - Single Target
  - Include "Mitigation Options" (Content Difficulty Filtering)
- Simple Mode - AoE
  - Include "Mitigation Options"
- Advanced Mode - Single Target
  - "Mitigation Options" (Content Difficulty Filtering)
    - All <60s mitigations (HP% slider, boss filtering)
    - All heals/mitigations that heal (HP% slider, boss filtering)
    - Invuln (enemy HP% slider, self HP% slider, boss filtering)
- Advanced Mode - AoE
  - "Mitigation Options"
    - All heals/mitigations that heal (HP% slider)
    - Invuln (enemy HP% slider, self HP% slider)
    - All other mitigations, including `Reprisal`, `Arm's Length`, etc without options

## Concerning Conflicts
- Conflicts should always go both ways. If X conflicts with Y, Y must conflict with X.
- Conflicts should only be on Combos.
  - Options should never conflict with Combos, it is just unnecessary.
  - Options should never conflict with each other. In this case, a radio UI element should instead be used.
- Openers should be configs, not presets, and should be conflicted where necessary with UI radio elements instead.

# Configs
Configs are the rest of the options (as in, those that accompany [Presets](#presets))
that are shown to users under each Job -more specifically: under presets- and are
defined in `_Config.cs` files.

This is mostly ImGUI code, primarily set up through
[`UserConfig`](/WrathCombo/Window/Functions/UserConfig.cs) methods, but all options
will need backed by a `User...`-Type option, e.g.
[`UserInt`](/WrathCombo/CustomCombo/Functions/Config.cs#L45),
[`UserBool`](/WrathCombo/CustomCombo/Functions/Config.cs#L64), etc, 
which can then be referenced in [rotation](#rotations) code as
`Config.<your config's name>`.
