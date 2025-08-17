# Guides on using specific parts of Wrath

- [IPC](/docs/IPC.md) - for other plugins to control Wrath Combo settings.
- [Action Retargeting](/docs/Retargeting.md) - for retargeting an action.

# Getting Started
Wrath is simpler to begin contributing to than other plugins as it abstracts away
a large portion of the ImGui and Dalamud-specific code, allowing you to focus on 
writing pure logic in Rotations and/or very simple copy-and-paste Preset and 
Config code.\
Additionally, the team is available to help you, you can ask in [#wrath-general](https://discord.com/channels/1001823907193552978/1271175781569003590)
in the [Puni.sh Discord server](https://discord.gg/Zzrcc8kmvy), or DM the developer that owns the job you are 
looking to contribute to.

Realistically all you need to get started working on Wrath is to open the 
solution in your development environment of choice, and find the specific Combo 
you would like to add code to.\
If you are looking to add a new Combo, you can simply copy one of the smaller 
(but probably not one-line) Combos from another job, add a Preset for it to 
reference, and add your Rotation code to the `Invoke` method of the Combo.

If you are unsure how to set up your development environment, you can start at
[The Setup Guide](/docs/Setup.md), but no further assistance will be provided by
Wrath team members for that.

# Submitting your Contribution
If you then want to contribute your additions upstream into Wrath, we have few
requirements to submit a Pull Request for what you've made:
- New or edited code must follow the styling of the surrounding code.
- Code changes outside your intended contributions should be minimized.\
  (as in, you probably shouldn't submit a PR with numerous unrelated
  variable-name changes, etc.)
- Be verbose in the Pull Request description, explaining what you have done, and
  why you have done it (and why you chose to do it the way you did).\
  (for most contributions, this ultimately amounts to just a sentence or two
  explaining the additions; changes to existing logic or entirely new features is
  where verbosity is most needed)

You can then expect a review from the owner of the job, or from another team member,
within a couple of weeks (where you should probably expect changes to be requested),
and after it is approved it will be merged in our next release cycle.\
The release cycle is typically every Sunday (or shortly thereafter), and involves
the majority of our code reviews taking place on Saturday, and then merging just 
before release time.\
Depending on the size of your PR and if there is an existing team member's PR,
yours may be merged into theirs or edited by a team member at release time to handle
conflicts.

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

Presets are all defined in [`Preset.cs`](/WrathCombo/Combos/Preset.cs).

## Terminology
- **Combos**
  - Combos are the foremost Presets, the ones that include the main Rotations.
  - Combos include both Simple Modes, both Advanced Modes, and Healing Combos.
- **Options**
  - Options are the second set of Presets, and are the choices for how to 
    configure Combos.
  - Can also be implemented as Configs if wanting to choose a default other than 
    "off".
- **Features**
  - Features are the third set of Presets, and are all the base Presets (as in, 
    that don't generally have Parents) that are not Combos.
  - Features are usually smaller than Combos, not containing a whole Rotation, 
    but rather a set of related actions; typically they are actions that combo 
    together, but sometimes they are only similar, like the Mitigation features.

<details>

<summary><h2>Preset Templates</h2></summary>

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
- Simple Mode - Single Target
    - Include Mitigation Options (Content Difficulty Filtering)
- Simple Mode - AoE
    - Include Mitigation Options
- Advanced Mode - Single Target
    - Mitigation Options (Content Difficulty Filtering)
        - All <60s mitigations (HP% slider, boss filtering)
        - All heals/mitigations that heal (HP% slider, boss filtering)
        - Invuln (enemy HP% slider, self HP% slider, boss filtering)
- Advanced Mode - AoE
    - Mitigation Options
        - All heals/mitigations that heal (HP% slider)
        - Invuln (enemy HP% slider, self HP% slider)
        - All other mitigations, including `Reprisal`, `Arm's Length`, etc without options
- One-Button Mitigation Feature (User-Prioritized)
    - Emergency Invuln Option (Content Difficulty Filtering)
    - Spammable Mitigation Options (Content Difficulty Filtering)
    - Reprisal
    - Group Mitigation (Party-check Option)
    - Bigger Mitigation Options (HP% slider)
    - Arm's Length (boss filtering, Nearby-Enemy-Count slider)
        - Mitigation should be roughly ordered by default from lowest mit to highest,
          shortest to longest cooldown.
        - Any mitigation options that have charges should have a charge slider.

</details>

## Regarding Conflicts
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
[`UserBool`](/WrathCombo/CustomCombo/Functions/Config.cs#L64), etc., 
which can then be referenced in [rotation](#rotations) code as
`Config.<your config's name>`.
