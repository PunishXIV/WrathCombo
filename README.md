<section id="top">
    <p style="text-align:center;" align="center">
        <img align="center" src="/res/plugin/wrathcombo.png" width="250" />
    </p>
    <h1 style="text-align:center;" align="center">Wrath Combo</h1>
    <p style="text-align:center;" align="center">
        Condenses combos and mutually exclusive abilities onto a single button - and then some.
    </p>
</section>

<!-- Badges -->
<p align="center"> 
<!-- Build & commit activity -->
  <!--no workflow on wrathcombo yet <a href="https://github.com/PunishXIV/WrathCombo/actions/workflows/build.yml" alt="Build">
    <img src="https://img.shields.io/github/actions/workflow/status/PunishXIV/WrathCombo/build.yml?branch=main&style=for-the-badge" /></a>-->
  <a href="https://github.com/PunishXIV/WrathCombo/commits/main" alt="Commits">
    <img src="https://img.shields.io/github/last-commit/PunishXIV/WrathCombo/main?color=00D162&style=for-the-badge" /></a>
   <a href="https://github.com/PunishXIV/WrathCombo/commits/main" alt="Commit Activity">
    <img src="https://img.shields.io/github/commit-activity/m/PunishXIV/WrathCombo?color=00D162&style=for-the-badge" /></a>
  <br> 
<!-- Other -->
  <a href="https://github.com/PunishXIV/WrathCombo/issues" alt="Open Issues">
    <img src="https://img.shields.io/github/issues-raw/PunishXIV/WrathCombo?color=EA9C0A&style=for-the-badge" /></a>
  <a href="https://github.com/PunishXIV/WrathCombo/graphs/contributors" alt="Contributors">
    <img src="https://img.shields.io/github/contributors/PunishXIV/WrathCombo?color=009009&style=for-the-badge" /></a>
<br>
<!-- Version -->
  <a href="https://github.com/PunishXIV/WrathCombo/tags" alt="Release">
    <img src="https://img.shields.io/github/v/tag/PunishXIV/WrathCombo?label=Release&logo=git&logoColor=ffffff&style=for-the-badge" /></a>
<br>
  <a href="https://discord.gg/Zzrcc8kmvy" alt="Discord">
    <img src="https://discordapp.com/api/guilds/1001823907193552978/embed.png?style=banner2" /></a>
</p>

<br><br>

<section id="about">

# About Wrath Combo

<p> Wrath Combo is a plugin for <a href="https://goatcorp.github.io/" alt="XIVLauncher">XIVLauncher</a>.<br><br>
    It's a heavily enhanced version of the XIVCombo plugin, offering highly 
    customisable features and options to allow users to have their 
    rotations be as complex or simple as possible, even to the point of a single
    button; for PvE, PvP, and more.
    <br><br>
    Wrath Combo is regularly updated to include new features and to keep
    up-to-date with the latest job changes in Final Fantasy XIV.
    <br><br>
    <img src="/res/readme_images/demo.gif" width="450" />
    <br>
    In that clip, the plugin is configured to condense the entire rotation of a 
    job onto a single button, and that button is being pressed repeatedly -
    all actions executed are being shown on a timeline for demonstration.
</p>
</section>

<!-- Installation -->
<section>

# Installation

<img src="/res/readme_images/adding_repo.jpg" width="450" />

Open the Dalamud Settings menu in game and follow the steps below.
This can be done through the button at the bottom of the plugin installer or by
typing `/xlsettings` in the chat.

1. Under Custom Plugin Repositories, enter `https://love.puni.sh/ment.json` into the
   empty box at the bottom.
2. Click the "+" button.
3. Click the "Save and Close" button.

Open the Dalamud Plugin Installer menu in game and follow the steps below.
This can be done through `/xlplugins` in the chat.

1. Click the "All Plugins" tab on the left.
2. Search for "Wrath Combo".
3. Click the "Install" button.

<p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>
</section> <br>

<!-- Features -->
<section>

# Features

Below you can find a small example of some of the features and options we offer in
Wrath Combo. <br>
Please note, this is just an excerpt and is not representative of the full
feature-set.


  <details><summary>PvE Features</summary> <br>

 - "Simple" (one-button) Mode for many jobs
 - "Advanced" Mode for many jobs, get as simple as you want
 - Auto-Rotation, to execute your rotation automatically, based on your settings
 - Variant Dungeon specific features
<br><br>
 - Tank Double Reprisal Protection
 - Tank Interrupt Feature
 - Healer Raise Feature
 - Magical Ranged DPS Double Addle Protection
 - Magical Ranged DPS Raise Feature
 - Melee DPS Double Feint Protection
 - Melee DPS True North Protection
 - Physical Ranged DPS Double Mitigation Protection
 - Physical Ranged DPS Interrupt Feature
    
 And much more!

  </details>

  <details><summary>PvP Features</summary> <br>

 - "Burst Mode" offense features for all jobs
 - Emergency Heals
 - Emergency Guard
 - Quick Purify
 - Guard Cancellation Prevention
    
 And much more!

  </details>

  <details><summary>Miscellaneous Features</summary> <br>

- Island Sanctuary Sprint Feature
- [BTN/MIN] Eureka Feature
- [BTN/MIN] Locate & Truth Feature
- [FSH] Cast to Hook Feature
- [FSH] Diving Feature

 And much more!

  </details>

To experience the full set of features on
offer, <a href="#installation" alt="install">install</a> the plugin or visit
the [Discord](https://discord.gg/Zzrcc8kmvy) server for more info.

<p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>

## Use with Other Plugins

### [Orbwalker](https://puni.sh/plugin/Orbwalker)

Wrath Combo can use Orbwalker to stop player movement in Auto-Rotation mode 
instead of requiring the player to stop before choosing to cast.

1. Open Wrath Combo's Auto-Rotation Settings: `/wrath autosettings`.
2. Check "Enable Orbwalker Integration".
3. Open Orbwalker and confirm your settings: `/orbwalker`.

### [AutoDuty](https://github.com/ffxivcode/AutoDuty)

Wrath Combo can be used as the Rotation Engine for AutoDuty, such that Wrath Combo's
Auto-Rotation will be used during duties.
To enable this:
1. Open AutoDuty's Config window: `/autoduty cfg`.
2. Expand the "Duty Config Settings" section.
3. Enable "Auto Manage Rotation Plugin State".
4. (Also check "> Wrath Config Options <" -> "Auto setup jobs for autorotation")\
   (if you already have your jobs setup, you can skip this step)

### [Questionable](https://git.carvel.li/liza/Questionable)

Wrath Combo can be used as the Combat Module for Questionable, such that Wrath 
Combo's Auto-Rotation will be employed during questing.
To enable this:
1. Open Questionable's Settings window: `/qst config`.
2. Go to the "General" tab.
3. Select "Wrath Combo" as the "Preferred Combat Module".

> By default, the two plugins above will ensure that combos in Wrath are set up, and
will lock all settings under those combos to `On` if combos were not set up, to
ensure that the rotation will run.

  <p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>
</section> 

<!-- Commands -->
<section>

# Commands

| **Chat command**                       | **Function**                                                                                                                                                                   |
|:---------------------------------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `/wrath`                               | Toggles the main plugin window, where you can enable/disable features, access settings and more.                                                                               |
| `/wrath pve`                           | Opens the main plugin window, to the PvE tab.                                                                                                                                  |
| `/wrath pvp`                           | Opens the main plugin window, to the PvP tab.                                                                                                                                  |
| `/wrath settings`                      | Opens the main plugin window, to the Settings tab.                                                                                                                             |
| `/wrath autosettings`                  | Opens the main plugin window, to the Auto-Rotation tab.                                                                                                                        |
| `/wrath <X>`                           | Opens the main plugin window, to a specific job's PvE features.<br>Replace `<X>` with the jobs abbreviation.                                                                   |
| `/wrath auto`                          | Toggles Auto-Rotation **on** or **off**.                                                                                                                                       |
| `/wrath auto <X>`                      | Sets Auto-Rotation to a specific state.<br>Replace `<X>` with `on`, `off`, or `toggle`.                                                                                        |
| `/wrath auto target <X> <Y>`           | Sets Auto-Rotation targeting mode.<br>Replace `<X>` with `damage` or `healer`.<br>For damage, `<Y>` can be: `manual`, `highest_max`, `lowest_max`, `highest_current`, `lowest_current`, `tank_target`, `nearest`, `furthest`.<br>For healer, `<Y>` can be: `manual`, `highest_current`, `lowest_current`.                |
| `/wrath combo`                         | Toggles action replacing **on** or **off**.<br>When off, actions will not be replaced with combos from the plugin. Auto-Rotation will still work.                              |
| `/wrath combo <X>`                     | Sets action replacing to a specific state.<br>Replace `<X>` with `on`, `off`, or `toggle`.                                                                                     |
| `/wrath ignore`                        | Adds a targeted NPC, and all instances of it, to an ignore list for Auto-Rotation's auto-targeting.<br>Manage this list in the Auto-Rotation tab.                              |
| `/wrath toggle <X>`                    | Toggles a specific feature or option **on** or **off**. Does not work while in combat.<br>Replace `<X>` with its internal name (or ID).                                        |
| `/wrath set <X>`                       | Turns a specific feature/option **on**. Does not work when in combat.<br>Replace `<X>` with its internal name (or ID).                                                         |
| `/wrath unset <X>`                     | Turn a specific feature/option **off**. Does not work when in combat.<br>Replace `<X>` with its internal name (or ID).                                                         |
| `/wrath unsetall`                      | Turns all features and options **off** at once.                                                                                                                                |
| `/wrath list ...`                      | Prints lists of feature's internal names to the game chat based on filter arguments.<br>Requires an appended filter. See Below.                                                |
| `/wrath list set`<br/>`/wrath enabled` | Prints a list of all currently enabled features & options in the game chat.                                                                                                    |
| `/wrath list unset`                    | Prints a list of all currently disabled features & options in the game chat.                                                                                                   |
| `/wrath list all`                      | Prints a list of every feature & option in the game chat, regardless of state.                                                                                                 |
| `/wrath list ... <X>`                  | All list commands can also optionally accept a job parameter, to filter the list down to a specific job.<br>Replace `<X>` with the jobs abbreviation.                          |
| `/wrath debug`                         | Outputs a debug file to your desktop containing only relevant features/options for your current job.<br>To be sent to developers, to help in bug-fixing. Completely anonymous. |
| `/wrath debug <X>`                     | Outputs a debug file containing only job-relevant features/options.<br>Replace `<X>` with the jobs abbreviation.                                                               |
| `/wrath debug all`                     | Outputs a debug file containing all features/options.                                                                                                                          |

<p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>
</section>

<!-- Contributing -->
<section>

# Contributing

Contributions to the project are always welcome - please feel free to submit
a [pull request](https://github.com/PunishXIV/WrathCombo/pulls) here on GitHub,
but ideally get in contact with us over on
the [Discord](https://discord.gg/Zzrcc8kmvy) server so we can communicate with one
another to make any necessary changes and review your submission!

You may also find [contributing info](CONTRIBUTING.md) and
[available guides](CONTRIBUTING.md#guides-on-using-specific-parts-of-wrath) helpful
in getting started.

   <p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>
</section>

<br><br>

<!-- Attribution -->
<div align="center">
  <a href="https://puni.sh/" alt="Puni.sh">
    <img src="https://github.com/PunishXIV/AutoHook/assets/13919114/a8a977d6-457b-4e43-8256-ca298abd9009" /></a>
<br>
  <a href="https://discord.gg/Zzrcc8kmvy" alt="Discord">
    <img src="https://discordapp.com/api/guilds/1001823907193552978/embed.png?style=banner2" /></a>
</div>
