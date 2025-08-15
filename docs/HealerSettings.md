# Healing Settings

> [!Note]
> If you choose to use Autorotation, see that guide first to set up that part.

> [!Note]
> If you are not using Autorotation, see below on how to set up your Heal stack (retargeting).

Healing in wrath works by replacing your primary healing buttons for single target and AoE with additional spells
in order determined by your settings. It is a bit more involved than dps settings and takes a bit of play to get
it dialed in to how you will want it to perform.

> [!Tip]
> This is best done in a duty support. Adjust settings between or even during pulls, and scions never complain.


## Understanding Priorities.
- Every Heal has a priority selector so you can set where it falls in the list.
    - The lower the priority number, the sooner it comes in the list.
- Every Heal also has a health threshold you can set to determine if that spell will be used.
    - If using Autorotation, the healing combo will still not activate until the autorotation threshold is met.
- Other limiting options exist such as "only weave" and "not on bosses" to further limit when a spell is used.
> [!Tip]
> Not on bosses is used on many AoE spells in the Single target combo to allow them to be used on dungeon trash.

How it all works
- A target falls below the autorotation healing threshold, so wrath switches to the ST or AoE healing combo based on how many targets need healing.
- Wrath looks at your number 1 spell first. If it matches the threshold and options you set, it will use it. If not, it will skip to next.
- Wrath proceeds down the list based on priority to determine which spell to use.
- If no spell in the list matches, then it will finally do the default spell the button is replacing such as Medica or Cure 2.

>[!Tip]
> Knowing how this works, you can see how it would make sense to set something like Benediction
> to a low health threshold but priority number 1 so that if it is ever needed, it fires immediately.

# Heal Stack (Retargeting)

With the introduction of retargeting, you no longer need an additional mod such as ReactionEx or Redirect to
set up custom targeting such as Mouseover. when you mouseover your target, Wrath will take that targets health for the healing thresholds.

## The Settings

You will locate the settings for retargeting in the "settings" button in the left column. The scrolling down to targeting options.
Here it will have the options for enabling retargeting as well as the Heal stack, and Raise stack.
> [!Tip]
> Stack simply refers to the priority in which you target yous spells. If 
> you have a mouseover then use that if not then hard target, then self.

### Customizing the Stacks
Your current heal stack will be displayed in the settings. You can expand the heal stack customization dropdown and add options to
the heal stack such as mouseover, and it will update the display to show you the target priority.

>[!Tip]
> If you are still using another retargeting mod with its own stacks, use 
> these settings or the custom heal stack to match your other mods' setups. This is 
> for when wrath checks the health of a target, it checks the correct one you want.

You can also Customize the raise stack so if you choose any of the retarget raise features throughout wrath,
it will smartly determine the target of that raise for you. This works for Summoners and Red Mages as well. 