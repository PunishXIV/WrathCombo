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
  <a href="https://github.com/PunishXIV/WrathCombo/actions/workflows/build.yml" alt="Build">
    <img src="https://img.shields.io/github/actions/workflow/status/Nik-Potokar/XIVSlothCombo/build.yml?branch=main&style=for-the-badge" /></a>
  <a href="https://github.com/Nik-Potokar/XIVSlothCombo/commits/main" alt="Commits">
    <img src="https://img.shields.io/github/last-commit/Nik-Potokar/XIVSlothCombo/main?color=00D162&style=for-the-badge" /></a>
   <a href="https://github.com/Nik-Potokar/XIVSlothCombo/commits/main" alt="Commit Activity">
    <img src="https://img.shields.io/github/commit-activity/m/Nik-Potokar/XIVSlothCombo?color=00D162&style=for-the-badge" /></a>
  <br> 
<!-- Version -->
  <a href="https://github.com/Nik-Potokar/XIVSlothCombo/tags" alt="Release">
    <img src="https://img.shields.io/github/v/tag/Nik-Potokar/XIVSlothCombo?label=Release&logo=git&logoColor=ffffff&style=for-the-badge" /></a>
<!-- Other -->
  <a href="https://github.com/Nik-Potokar/XIVSlothCombo/issues" alt="Open Issues">
    <img src="https://img.shields.io/github/issues-raw/Nik-Potokar/XIVSlothCombo?color=EA9C0A&style=for-the-badge" /></a>
  <a href="https://github.com/Nik-Potokar/XIVSlothCombo/graphs/contributors" alt="Contributors">
    <img src="https://img.shields.io/github/contributors/Nik-Potokar/XIVSlothCombo?color=009009&style=for-the-badge" /></a>
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
</p>
</section>

<!-- Installation -->
<section>

# Installation

<img src="/res/readme_images/adding_repo.jpg" width="450" />

Open the Dalamud Settings menu in game and follow the steps below.
This can be  done through the button at the bottom of the plugin installer or by 
typing `/xlsettings` in the chat.

1. Under Custom Plugin Repositories, enter `https://love.puni.sh/ment.json` into the 
empty box at the bottom.
2. Click the "+" button.
3. Click the "Save and Close" button.

<p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>
</section> <br>

<!-- Commands -->
<section>

# Commands

| **Chat command &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;** | **Function**                                                                                                                                 |
|:-------------------------------------------------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------|
| `/wrath`                                                                 | Opens the main plugin window, where you can enable/disable features, access settings and more.                                               |
| `/wrath auto`                                                            | Toggles Auto-Rotation **on** or **off**.                                                                                                     |
| `/wrath set`                                                             | Turns a specific feature/option **on** by referring to its internal name.<br>Does not work when in combat.                                   |
| `/wrath unset`                                                           | Turn a specific feature/option **off** by referring to its internal name.<br>Does not work while in combat.                                  |
| `/wrath unsetall`                                                        | Turns all features and options **off** at once.                                                                                              |
| `/wrath toggle`                                                          | Toggles a specific feature/option **on or off** by referring to its internal name.<br>Does not work while in combat.                         |
| `/wrath list`                                                            | Prints lists to the game chat based on filter arguments. <br>Requires an appended filter.                                                    |
| `/wrath list set`<br>`/wrath enabled`                                    | Prints a list of all currently enabled features & options in the game chat.                                                                  |
| `/wrath list unset`                                                      | Prints a list of all currently disabled features & options in the game chat.                                                                 |
| `/wrath list all`                                                        | Prints a list of every feature & option in the game chat, regardless of state.                                                               |
| `/wrath debug`                                                           | Outputs a full debug file to your desktop that can be sent to developers for utilisation in bug-fixing.                                      |
| `/wrath debug JOB`                                                       | Outputs a debug file to your desktop containing only job-relevant features/options. <br>Replace `JOB` with the appropriate job abbreviation. |

<p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>
</section>

<!-- Features -->
<section>

# Features

Below you can find a small example of some of the features and options we offer in
Wrath Combo. <br>
Please note, this is just an excerpt and is not representative of the full
feature-set.


  <details><summary>PvE Features</summary> <br>

    "Simple" (one-button) Mode for many jobs
    "Advanced" Mode for many jobs, get as simple as you want
    Auto-Rotation, to execute your rotation automatically, based on your settings
    Variant Dungeon specific features

    Tank Double Reprisal Protection
    Tank Interrupt Feature
    Healer Raise Feature
    Magical Ranged DPS Double Addle Protection
    Magical Ranged DPS Raise Feature
    Melee DPS Double Feint Protection
    Melee DPS True North Protection
    Physical Ranged DPS Double Mitigation Protection
    Physical Ranged DPS Interrupt Feature
    
    And much more!

  </details>

  <details><summary>PvP Features</summary> <br>

    "Burst Mode" offense features for all jobs
    Emergency Heals
    Emergency Guard
    Quick Purify
    Guard Cancellation Prevention
    
    And much more!

  </details>

  <details><summary>Miscellaneous Features</summary> <br>

    Island Sanctuary Sprint Feature
    [BTN/MIN] Eureka Feature
    [BTN/MIN] Locate & Truth Feature
    [FSH] Cast to Hook Feature
    [FSH] Diving Feature
    
    And much more!

  </details>

To experience the full set of features on
offer, <a href="#installation" alt="install">install</a> the plugin or visit
the [Discord](https://discord.gg/Zzrcc8kmvy) server for more info.

  <p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>
</section> 

<!-- Contributing -->
<section>

# Contributing

Contributions to the project are always welcome - please feel free to submit
a [pull request](https://github.com/Nik-Potokar/XIVSlothCombo/pulls) here on GitHub,
but ideally get in contact with us over on
the [Discord](https://discord.gg/Zzrcc8kmvy) server so we can communicate with one
another to make any necessary changes and review your submission!

   <p align="right"><a href="#top" alt="Back to top"><img src=/res/readme_images/arrowhead-up.png width ="25"/></a></p>
</section> 

#

<!-- Attribution -->
<div align="center">
  <a href="https://puni.sh/" alt="Puni.sh">
    <img src="https://github.com/PunishXIV/AutoHook/assets/13919114/a8a977d6-457b-4e43-8256-ca298abd9009" /></a>
<br>
  <a href="https://discord.gg/Zzrcc8kmvy" alt="Discord">
    <img src="https://discordapp.com/api/guilds/1001823907193552978/embed.png?style=banner2" /></a>
</div>
