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

### [AutoDuty](https://github.com/ffxivcode/AutoDuty)

Wrath Combo can be used as the Rotation Engine for AutoDuty, such that Wrath Combo's
Auto-Rotation will be used during duties.
To enable this:
1. Open AutoDuty's main window.
2. Go to the "Config" tab.
3. Expand the "Duty Config" section.
4. Enable "Auto Manage Rotation Plugin State".

You will still need to configure Wrath Combo's Auto-Rotation settings and what 
combos you have set to be enabled in it as normal, AutoDuty will currently on enable
or disable the Auto-Rotation feature.

1. Open Wrath Combo and navigate to the "Auto-Rotation" tab.
    - Configure your Auto-Rotation settings as desired.
2. Navigate to the "PvE Features" tab and select your job.
    - Enable at least 1 combo, but preferably a Single and Multi-target Combo.
        - You can use "Simple Mode" combos, or "Advanced Mode" combos and 
          configure them as desired.
    - Tick the "Auto-Mode" checkbox to the right of your chosen combos.
3. AutoDuty will enable Wrath Combo when it commences a duty, and disable it when 
   it ends.

  <p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>
</section> 

<!-- Commands -->
<section>

# Commands

| **Chat command**                       | **Function**                                                                                                                                                |
|:---------------------------------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `/wrath`                               | Opens the main plugin window, where you can enable/disable features, access settings and more.                                                              |
| `/wrath auto`                          | Toggles Auto-Rotation **on** or **off**.                                                                                                                    |
| `/wrath auto STATE`                    | Sets Auto-Rotation to a specific state. Replace `STATE` with `on` or `off`.                                                                                 |
| `/wrath combo STATE`                   | When toggled off, actions will not be replaced with combos from the plugin.<br>Auto-Rotation will still work. Replace `STATE` with `on`, `off` or `toggle`. |
| `/wrath toggle INT`                    | Toggles a specific feature/option **on or off** by replacing `INT` with its internal name.<br>Does not work while in combat.                                |
| `/wrath set INT`                       | Turns a specific feature/option **on** by replacing `INT` with its internal name.<br>Does not work when in combat.                                          |
| `/wrath unset INT`                     | Turn a specific feature/option **off** by replacing `INT` with its internal name.<br>Does not work while in combat.                                         |
| `/wrath unsetall`                      | Turns all features and options **off** at once.                                                                                                             |
| `/wrath list ...`                      | Prints lists of feature's internal names to the game chat based on filter arguments.<br>Requires an appended filter. See Below.                             |
| `/wrath list set`<br/>`/wrath enabled` | Prints a list of all currently enabled features & options in the game chat.                                                                                 |
| `/wrath list unset`                    | Prints a list of all currently disabled features & options in the game chat.                                                                                |
| `/wrath list all`                      | Prints a list of every feature & option in the game chat, regardless of state.                                                                              |
| `/wrath debug`                         | Outputs a full debug file to your desktop that can be sent to developers for utilisation in bug-fixing.                                                     |
| `/wrath debug JOB`                     | Outputs a debug file to your desktop containing only job-relevant features/options. <br>Replace `JOB` with the appropriate job abbreviation.                |

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

You may also find the [contributing guidelines](CONTRIBUTING.md) helpful in getting
started.

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
