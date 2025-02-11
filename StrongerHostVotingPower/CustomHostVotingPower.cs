using BepInEx;
using BepInEx.Logging;
using CustomHostVotingPower.Hooks;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using HarmonyLib;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using Discord;

namespace CustomHostVotingPower;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(DependencyStrings.LethalConfig, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(DependencyStrings.LobbyCompatibility, BepInDependency.DependencyFlags.SoftDependency)]
public class CustomHostVotingPower : BaseUnityPlugin
{
    public static CustomHostVotingPower Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static List<IDetour> Hooks { get; set; } = new List<IDetour>();
    private static readonly string configLocation = Utility.CombinePaths(Paths.ConfigPath + "\\" + MyPluginInfo.PLUGIN_GUID[22..34].Replace(".", "\\")) + MyPluginInfo.PLUGIN_GUID[..21];
    internal static ConfigFile cfg = new(configLocation + ".cfg", false);

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        Configuration.Config.Init();
        InitCompatibility();
        Hook();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Hook()
    {
        Logger.LogDebug("Hooking methods");

        Hooks.Add(new Hook(typeof(TimeOfDay).GetMethod("SetShipLeaveEarlyServerRpc", AccessTools.allDeclared), AdjustHostVotingPower.UpdateHostVotingPower));
        Hooks.Add(new Hook(typeof(TimeOfDay).GetMethod("VoteShipToLeaveEarly", AccessTools.allDeclared), AdjustHostVotingPower.CheckForHostVote));
        Hooks.Add(new Hook(typeof(HUDManager).GetMethod("Awake", AccessTools.allDeclared), AdjustHostVotingPower.GetHUDManager));

        Logger.LogDebug("Finished hooking");
    }

    internal static void InitCompatibility()
    {
        foreach (string dependencyString in DependencyStrings.dependencyList)
        {
            if (Chainloader.PluginInfos.ContainsKey(dependencyString))
            {
                switch (dependencyString)
                {
                    case DependencyStrings.LethalConfig:
                        Compatibility.RegisterLethalConfig.Init();
                        break;
                    case DependencyStrings.LobbyCompatibility:
                        Compatibility.RegisterLobbyCompatibility.Init();
                        break;
                    default:
                        Logger.LogDebug("Dependency does not have any code made for it");
                        break;
                }
            }
        }
    }
}


internal static class DependencyStrings
{

    internal const string LethalConfig = "ainavt.lc.lethalconfig";
    internal const string LobbyCompatibility = "BMX.LobbyCompatibility";
    internal static List<string> dependencyList = [LethalConfig, LobbyCompatibility];
}
