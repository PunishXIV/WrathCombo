# Prerequisites

Given that all plugins are C# projects, you will need to have the .NET SDK installed.

You can download the required SDK from the [.NET downloads page](https://dotnet.microsoft.com/en-us/download).\
<sup>At the time of writing, this is .Net 9. You may find you need to restart 
after installing it.</sup>

Click "All .NET downloads" under whichever .NET version you are looking for, and 
only look on the left side of the page (the SDKs), and the top-most table on the 
left side (the most recent release), and choose the link for your OS and system;
the most likely SDK you'll want is **Windows**' `x64`.

# Cloning Wrath
It is recommended you clone Wrath via [GitHub Desktop](https://github.com/apps/desktop) if you are unfamiliar
with using Git from the command line, this will clone (read: download) our code, 
and our dependencies at the same time.\
Just install that program, then on Wrath's GitHub page, click the green "Code"
button, and select "Open with GitHub Desktop", the rest should be simple, but it 
is recommended that you select "To Contribute" when asked what your intentions 
are for the clone.

If you are using the command line, you can clone Wrath with:
```bash
git clone https://github.com/PunishXIV/WrathCombo.git --recurse-submodules
```

If you are trying to build a previous release, you can instead download the 
source code `.zip` file from the [releases page](https://github.com/PunishXIV/WrathCombo/releases), or by clicking the Commit hash
identifier near the top of the release, and then cloning at that point.

# Opening the Solution
Once you have cloned Wrath, you can open the `.sln` file in your development
environment of choice.\
You can find the solution file in the root of the repository, you can open that 
folder by right-clicking the repository name in GitHub Desktop and selecting
"Open in Explorer".

If you do not already have one, there are numerous options:
- [Visual Studio](https://visualstudio.microsoft.com/) (the standard)
- [JetBrains Rider](https://www.jetbrains.com/rider/) (much better tool if you expect
  to contribute heavily)
- [VS Code](https://code.visualstudio.com/) (simplest startup)

# Building the Solution
In Rider or Visual Studio you can now just build the solution, which is very 
prominent in both IDEs.\
In VS Code, ensure that the Solution Explorer window is open on the left and the 
solution is loaded, then you can right-click on the Wrath project in there and 
select build.

Expect to see a variety of warnings, especially the first time you build it (from 
our dependencies), but you should have no errors.\
If you do have errors first check that you have the latest version of the .NET SDK
installed, and that you have cloned the repository's submodules, which are required

> If you did not initially clone the repository with the submodules, you can open 
> your terminal and path into the WrathCombo folder, and run
> ```bash
> git submodule update --init --recursive
> ```
> to download them now.

If you do have errors, you can ask for help in the [#plugin-development](https://discord.com/channels/1001823907193552978/1164253241350037624) in the
[Puni.sh Discord server](https://discord.gg/Zzrcc8kmvy).

# Loading the Built Plugin
Once you have built the solution, you can load the plugin into Dalamud by copying 
the `.dll` path from the end of the build output, for example with:
```bash
WrathCombo -> C:/Users/User/AppData/Local/devPlugins/WrathCombo.dll
```
you would copy the path `C:/Users/User/AppData/Local/devPlugins/WrathCombo.dll`.\
You can then paste that into `/xlsettings` (in-game), on the Experimental tab,
under Dev Plugins, click the Plus button, and Save.

At which point, if you disable your current Wrath Combo plugin, you can enable 
your developer version in `/xlplugins`, in the Dev Plugins tab on the left side.

If it fails to load, check `/xllog` for the error message, but it is likely to 
only do so if you have not disabled another instance of the Wrath Combo plugin, 
or you added a duplicate Config name.

If you do have struggles with getting the plugin to load without error, it is 
recommended you use the [UnloadErrorFuckOff](https://github.com/NightmareXIV/UnloadErrorFuckOff) plugin from Nightmare to remove the 
error without having to restart the game by running `/fuckoff` in-game:
```bash
https://github.com/NightmareXIV/MyDalamudPlugins/raw/main/pluginmaster.json
```

Once loaded, there are a variety of toggle buttons next to the Disable switch, 
one of which is Auto Reload, which will reload the plugin every time you re-build it,
and you can start testing your changes!

# Submitting your Contributions
Once you have made and tested your changes, you may wish to submit them.\
If you cloned the repository with GitHub Desktop, you can simply write a brief 
message for the commit, commit your changes, and then click the Publish button.

At which point, you can go to the [Wrath Combo Pull Requests page](http://github.com/PunishXIV/WrathCombo/pulls) and it should
have a ribbon at the top suggesting you open a Pull Request.

If you did not use GitHub Desktop, this is now a point where it is suggested that 
you do, and either restart and copy over your changes, or choose to "Add an 
Existing Repository" and selecting the one you cloned.\
If you don't wish to, then GitHub has a guide that can probably help you in this 
situation [here](https://docs.github.com/en/desktop/overview/creating-your-first-repository-using-github-desktop).