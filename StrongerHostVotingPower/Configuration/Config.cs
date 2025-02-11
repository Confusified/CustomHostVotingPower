using BepInEx.Configuration;

namespace CustomHostVotingPower.Configuration;

public static class Config
{
    public static ConfigEntry<bool> useCustomVotingPower { get; private set; } = null!;
    public static ConfigEntry<int> customVotingPower { get; private set; } = null!;
    public static ConfigEntry<bool> usePercentageOfTotalVotesRequired { get; private set; } = null!;
    public static ConfigEntry<int> customVotingPowerPercentage { get; private set; } = null!;


    public static void Init()
    {
        CustomHostVotingPower.Logger.LogDebug("Initialising config");

        var cfg = CustomHostVotingPower.cfg;
        ConfigDescription votingPowerDescription = new("Determines how many votes will be added when you vote (as a host)", new AcceptableValueRange<int>(1, 100));

        useCustomVotingPower = cfg.Bind("Global Settings", "Use Custom Voting Power", true, "Enables custom voting power for the host. (You must be the host)");
        customVotingPower = cfg.Bind("Voting Settings", "Custom Voting Power", 2, votingPowerDescription);
        usePercentageOfTotalVotesRequired = cfg.Bind("Percentage Voting Settings", "Use Percentage Of Total Required Votes", false, "When enabled, your voting power will be equal to the percentage of required total votes");
        customVotingPowerPercentage = cfg.Bind("Percentage Voting Settings", "Custom Voting Power Percentage", 50, votingPowerDescription);

        CustomHostVotingPower.Logger.LogDebug("Config initialised");
    }
}