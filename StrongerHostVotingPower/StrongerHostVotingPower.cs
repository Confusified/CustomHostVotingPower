using BepInEx;
using BepInEx.Logging;
using StrongerHostVotingPower.Hooks;
using System.Reflection;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using HarmonyLib;

namespace StrongerHostVotingPower;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
// [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
public class StrongerHostVotingPower : BaseUnityPlugin
{
    public static StrongerHostVotingPower Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static List<IDetour> Hooks { get; set; } = new List<IDetour>();

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        Hook();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Hook()
    {
        Logger.LogDebug("Hooking...");

        /*
         *  Add to the Hooks list for each method you're patching with:
         *
         *  Hooks.Add(new Hook(
         *      typeof(Class).GetMethod("Method", AccessTools.allDeclared),
         *      CustomClass.CustomMethod));
         */
        Hooks.Add(new Hook(typeof(TimeOfDay).GetMethod("SetShipLeaveEarlyServerRpc", AccessTools.allDeclared), AdjustHostVotingPower.UpdateHostVotingPower));
        Hooks.Add(new Hook(typeof(TimeOfDay).GetMethod("VoteShipToLeaveEarly", AccessTools.allDeclared), AdjustHostVotingPower.CheckForHostVote));

        Logger.LogDebug("Finished Hooking!");
    }
}
