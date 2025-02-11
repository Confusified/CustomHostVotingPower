using LethalConfig;
using LethalConfig.ConfigItems;

namespace CustomHostVotingPower.Compatibility;

public static class RegisterLethalConfig
{
    public static void Init()
    {
        CustomHostVotingPower.Logger.LogDebug($"Creating config entries");

        LethalConfigManager.SkipAutoGen();

        BoolCheckBoxConfigItem booluseCustom = new(Configuration.Config.useCustomVotingPower, requiresRestart: false);
        BoolCheckBoxConfigItem boolusePercentage = new(Configuration.Config.usePercentageOfTotalVotesRequired, requiresRestart: false);
        BoolCheckBoxConfigItem boolLimitCustomVotes = new(Configuration.Config.limitCustomVotes, requiresRestart: false);
        IntInputFieldConfigItem intItem = new(Configuration.Config.customVotingPower, requiresRestart: false);
        IntSliderConfigItem intSliderItem = new(Configuration.Config.customVotingPowerPercentage, requiresRestart: false);

        LethalConfigManager.AddConfigItem(booluseCustom);
        LethalConfigManager.AddConfigItem(boolusePercentage);
        LethalConfigManager.AddConfigItem(boolLimitCustomVotes);
        LethalConfigManager.AddConfigItem(intItem);
        LethalConfigManager.AddConfigItem(intSliderItem);

        CustomHostVotingPower.Logger.LogDebug($"Config entries created");
    }
}