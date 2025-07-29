--[=====[
[[SND Metadata]]
author: zbee, Nonunon
version: 1.0.0
description: >-
  This is an example setup for using Wrath within SND's Lua.

  Please see the full documentation, or `/snd`>Help>Lua>IPC>Wrath for documentation:

  https://github.com/PunishXIV/WrathCombo/blob/docs/docs/IPC.md


  Update from:

  https://github.com/PunishXIV/WrathCombo/blob/docs/docs/IPCExample.lua
plugin_dependencies:
- WrathCombo

[[End Metadata]]
--]=====]

local scriptName = "Wrath SND Example"
local leaseID

-- This does the bare minimum to enable Wrath Combo for your current job.
function EnableWrathAuto()
    -- Enable Wrath Combo Auto Rotation
    IPC.WrathCombo.SetAutoRotationState(GetLeaseID(), true)

    -- Report Wrath Combo's Auto Rotation state
    yield("/wait 0.75")
    log("Wrath Combo Auto Rotation is now " ..
            (IPC.WrathCombo.GetAutoRotationState() and "enabled" or "disabled"))

    -- Make sure the current job is ready for Auto Rotation
    IPC.WrathCombo.SetCurrentJobAutoRotationReady(GetLeaseID())

    -- Report Wrath Combo's current job readiness state
    yield("/wait 1.0")
    log("Wrath Combo's current job is " ..
            (IPC.WrathCombo.IsCurrentJobAutoRotationReady() and "ready" or "not ready"))

    yield("/wait 10.0")
end

-- This goes a bit above the previous function, and configures Wrath Combo to 
-- rotate according to your needs.
function EnableWrathAutoAndConfigure()
    -- Enable Wrath Combo Auto Rotation
    IPC.WrathCombo.SetAutoRotationState(GetLeaseID(), true)

    -- Report Wrath Combo's Auto Rotation state
    yield("/wait 0.75")
    log("Wrath Combo Auto Rotation is now " ..
            (IPC.WrathCombo.GetAutoRotationState() and "enabled" or "disabled"))

    -- Make sure the current job is ready for Auto Rotation
    local readyReturn = IPC.WrathCombo.SetCurrentJobAutoRotationReady(GetLeaseID())
    -- Check the actual return as well, since it can be an error code.
    if readyReturn == SetResult.Okay or readyReturn == SetResult.OkayWorking then
        log("Current job is being readied for Auto Rotation.")
    else
        log("Failed to ready current job for Auto Rotation: " ..
                tostring(readyReturn))
        return
    end

    -- Report Wrath Combo's current job readiness state
    yield("/wait 1.0")
    local jobReady = IPC.WrathCombo.IsCurrentJobAutoRotationReady()
    log("Wrath Combo's current job is " ..
            (jobReady and "ready" or "not ready"))

    -- Set various Auto Rotation settings
    IPC.WrathCombo.SetAutoRotationConfigState(GetLeaseID(),
            AutoRotationConfigOption.InCombatOnly, false)
    IPC.WrathCombo.SetAutoRotationConfigState(GetLeaseID(),
            AutoRotationConfigOption.AutoRez, true)
    IPC.WrathCombo.SetAutoRotationConfigState(GetLeaseID(),
            AutoRotationConfigOption.SingleTargetHPP, 60)

    yield("/wait 10.0")
end

-- This is primarily for show, for how to handle the return types of various methods.
function DemonstrateHandling()
    -- Checking that Wrath is present and ready
    ---------------------------------------------------------------------------------
    if not IPC.WrathCombo.IPCReady() then
        log("IPC is not ready; cannot demonstrate handling.")
        return
    end

    -- Attempting to register for a lease
    ---------------------------------------------------------------------------------
    local registrationReturn = IPC.WrathCombo.Register("test name")
    leaseID = registrationReturn -- Here to make sure all leases are cleared on stop
    if registrationReturn == nil then
        log("Failed to register for Wrath Combo control; " ..
                "see logs from Wrath Combo for why.")
        return
    else
        log("Successfully registered for Wrath Combo control with leaseId: " ..
                tostring(registrationReturn))
    end

    -- Looking at all combo and option names for the current job
    ---------------------------------------------------------------------------------
    local jobID = Player.Job.Id
    -- Combos
    local comboNamesList = {}
    log("Current job ID: " .. tostring(jobID) ..
            " (" .. Player.Job.Name .. ")")
    local comboNamesRaw = IPC.WrathCombo.GetComboNamesForJob(jobID)
    if comboNamesRaw == nil or comboNamesRaw.Count == nil then
        log("Failed to get combo names for job ID: " .. tostring(jobID))
        return
    else
        -- For whatever reason, combo names are returned as `userdata`
        -- Convert it to a table for easier handling:
        for i = 0, comboNamesRaw.Count - 1 do
            table.insert(comboNamesList, tostring(comboNamesRaw[i]))
        end
        log("Combo names for job ID " .. tostring(jobID) .. ": " ..
                table.concat(comboNamesList, ", "))
    end
    -- Options
    local optionNamesList = {}
    local optionNamesRaw = IPC.WrathCombo.GetComboOptionNamesForJob(jobID)
    for i = 0, comboNamesRaw.Count - 1 do
        local combo = tostring(comboNamesRaw[i])
        -- Try to get the list for this combo key
        local optionList = optionNamesRaw:get_Item(combo)
        if optionList ~= nil and type(optionList) ~= "string" then
            -- Process each option in the list
            for j = 0, optionList.Count - 1 do
                local option = tostring(optionList[j])
                table.insert(optionNamesList, option)
            end
        end
    end
    if optionNamesRaw == nil or optionNamesList.Count == 0 then
        log("Failed to get option names for job ID: " .. tostring(jobID))
        return
    else
        log("Option names for job ID " .. tostring(jobID) .. ": " ..
                table.concat(optionNamesList, ", "))
    end

    -- Check Combo and Option states
    ---------------------------------------------------------------------------------
    local presetReturn
    -- Combo
    presetReturn = IPC.WrathCombo.GetComboState(comboNamesList[1])
    log("Combo state for " .. tostring(comboNamesList[1]) .. " -> " ..
            "enabled: " .. tostring(presetReturn[ComboStateKeys.Enabled]) ..
            " (type: " .. type(presetReturn[ComboStateKeys.Enabled]) .. "), " ..
            "enabled in auto: " .. tostring(presetReturn[ComboStateKeys.AutoMode]) ..
            " (type: " .. type(presetReturn[ComboStateKeys.AutoMode]) .. ")")
    -- Option
    presetReturn = IPC.WrathCombo.GetComboOptionState(optionNamesList[1])
    log("Option state for " .. tostring(optionNamesList[1]) .. " -> " ..
            "enabled: " .. tostring(presetReturn) ..
            " (type: " .. type(presetReturn) .. "), ")

    -- Set Combo and Option states
    ---------------------------------------------------------------------------------
    local setResult
    -- Combo
    setResult = IPC.WrathCombo.SetComboState(GetLeaseID(), comboNamesList[1],
            true, true)
    if setResult == SetResult.Okay or setResult == SetResult.OkayWorking then
        log("Successfully set combo state for " .. tostring(comboNamesList[1]) ..
                " to enabled and auto mode on.")
    else
        log("Failed to set combo state for " .. tostring(comboNamesList[1]) ..
                ": " .. tostring(setResult))
    end
    -- Option
    setResult = IPC.WrathCombo.SetComboOptionState(GetLeaseID(),
            optionNamesList[1], true)
    if setResult == SetResult.Okay or setResult == SetResult.OkayWorking then
        log("Successfully set option state for " .. tostring(optionNamesList[1]) ..
                " to enabled.")
    else
        log("Failed to set option state for " .. tostring(optionNamesList[1]) ..
                ": " .. tostring(setResult))
    end

    -- Checking Auto Rotation settings
    ---------------------------------------------------------------------------------
    local autoRotConfigReturn
    -- A boolean
    autoRotConfigReturn = IPC.WrathCombo.GetAutoRotationConfigState(
            AutoRotationConfigOption.InCombatOnly)
    log("Auto Rotation Config Option " ..
            tostring(AutoRotationConfigOption.InCombatOnly) .. " -> " ..
            tostring(autoRotConfigReturn) ..
            " (type: " .. type(autoRotConfigReturn) .. ")")
    -- A DPSRotationMode Enum value
    autoRotConfigReturn = IPC.WrathCombo.GetAutoRotationConfigState(
            AutoRotationConfigOption.DPSRotationMode)
    log("Auto Rotation Config Option " ..
            tostring(AutoRotationConfigOption.DPSRotationMode) .. " -> " ..
            tostring(autoRotConfigReturn) ..
            " (type: " .. type(autoRotConfigReturn) .. ")")
    -- A HealerRotationMode Enum value
    autoRotConfigReturn = IPC.WrathCombo.GetAutoRotationConfigState(
            AutoRotationConfigOption.HealerRotationMode)
    log("Auto Rotation Config Option " ..
            tostring(AutoRotationConfigOption.HealerRotationMode) .. " -> " ..
            tostring(autoRotConfigReturn) ..
            " (type: " .. type(autoRotConfigReturn) .. ")")
    -- An int
    autoRotConfigReturn = IPC.WrathCombo.GetAutoRotationConfigState(
            AutoRotationConfigOption.SingleTargetHPP)
    log("Auto Rotation Config Option " ..
            tostring(AutoRotationConfigOption.SingleTargetHPP) .. " -> " ..
            tostring(autoRotConfigReturn) ..
            " (type: " .. type(autoRotConfigReturn) .. ")")

    yield("/wait 10.0")
end

-- Returns the current Lease ID if there is one, or registers for a new one.
function GetLeaseID()
    if leaseID ~= nil then
        return leaseID
    else
        registered = false
    end

    if IPC.WrathCombo.IPCReady() == false then
        log("IPC is not ready; cannot register for Wrath Combo control. " ..
                "Likely just waiting for caches to be built after a job change.")
        return
    end

    log("Registering for Wrath Combo control...")

    leaseID = IPC.WrathCombo.Register(scriptName)

    if leaseID ~= nil then
        log("Successfully registered with leaseId: " .. tostring(leaseID))
    else
        log("Failed to register for lease. " ..
                "See logs from Wrath Combo for why.")
    end

    return leaseID
end

-- Releases the lease if one exists when the script stops.
function OnStop()
    if leaseID ~= nil then
        log("Releasing leaseId: " .. tostring(leaseID))
        IPC.WrathCombo.ReleaseControl(leaseID)
        leaseID = nil
    else
        log("leaseId is nil; nothing to release.")
    end
end

-- Just simplifies logging to always have a customized prefix.
function log(string)
    Dalamud.Log("[" .. scriptName .. "] " .. string)
end

log("---------------------------------------------")
log("Running " .. scriptName .. " script")
log("---------------------------------------------")
log("It is suggested you filter logs by: (notice the space at the end) " ..
        "(exclude the backticks)")
log("`\\[Wrath `")
log("---------------------------------------------")

log("---------------------------------------------")
log("Running EnableWrathAuto()")
log("---------------------------------------------")
EnableWrathAuto()

log("---------------------------------------------")
log("Releasing stuff between example executions")
log("---------------------------------------------")
OnStop()

log("---------------------------------------------")
log("Running EnableWrathAutoAndConfigure()")
log("---------------------------------------------")
EnableWrathAutoAndConfigure()

log("---------------------------------------------")
log("Running DemoHandling()")
log("---------------------------------------------")
DemonstrateHandling()