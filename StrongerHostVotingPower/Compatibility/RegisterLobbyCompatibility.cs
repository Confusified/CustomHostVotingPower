using LobbyCompatibility.Features;
using LobbyCompatibility.Enums;
using System;

namespace CustomHostVotingPower.Compatibility;

public static class RegisterLobbyCompatibility
{
    public static void Init()
    {
        CustomHostVotingPower.Logger.LogDebug($"Registering mod to LobbyCompatibility");

        PluginHelper.RegisterPlugin(MyPluginInfo.PLUGIN_GUID, Version.Parse(MyPluginInfo.PLUGIN_VERSION), CompatibilityLevel.ServerOnly, VersionStrictness.None);

        CustomHostVotingPower.Logger.LogDebug($"Mod registered");
    }
}