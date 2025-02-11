using System;
using CustomHostVotingPower.Configuration;
using TMPro;

namespace CustomHostVotingPower.Hooks;

public static class AdjustHostVotingPower
{
    private static int customVoteInt = 0;

    private static bool hostCalledVote = false;
    private static int totalVotesNeeded = 0;

    public static HUDManager hudManager = null!;

    public static void CheckForHostVote(Action<TimeOfDay> orig, TimeOfDay self)
    {
        // Only bother checking if the host has not voted before
        if (!self.votedShipToLeaveEarlyThisRound)
        {
            hostCalledVote = self.NetworkManager.IsHost;
        }
        orig(self);
        hostCalledVote = false;
    }

    public static void UpdateHostVotingPower(Action<TimeOfDay> orig, TimeOfDay self)
    {
        // This is always ran by the host, so checking to see if they're a host in this method would not work
        CustomHostVotingPower.Logger.LogDebug($"host called vote: {hostCalledVote}");
        if (Config.useCustomVotingPower.Value && hostCalledVote)
        {
            StartOfRound startOfRound = StartOfRound.Instance;
            // By fetching the total required votes like this, it'll be compatible with any mods that modify the required amount of votes
            totalVotesNeeded = int.Parse(hudManager.holdButtonToEndGameEarlyVotesText.text.Split("/")[1].Split(" ")[0]);
            // totalVotesNeeded = startOfRound.connectedPlayersAmount + 1 - startOfRound.livingPlayers; // this only works for vanilla

            if (Config.usePercentageOfTotalVotesRequired.Value)
            {
                // When doing value / 100, it does not work
                float voteSmallPercentile = Config.customVotingPowerPercentage.Value * (float)0.01;
                customVoteInt = (int)MathF.Round(totalVotesNeeded * voteSmallPercentile);
            }
            else
            {
                customVoteInt = Config.customVotingPower.Value;
            }

            // Set the vote count to 1 because having 0 or less votes is not intended
            if (customVoteInt <= 0)
            {
                customVoteInt = 0;
                customVoteInt++;
            }

            self.votesForShipToLeaveEarly += customVoteInt;

            CustomHostVotingPower.Logger.LogDebug($"Increased votes with {customVoteInt}");

            if (self.votesForShipToLeaveEarly >= totalVotesNeeded)
            {
                if (Config.limitCustomVotes.Value)
                {
                    self.votesForShipToLeaveEarly = totalVotesNeeded;
                    CustomHostVotingPower.Logger.LogDebug($"Amount of votes was set to the total required amount because the threshold has been reached");
                }

                self.SetShipLeaveEarlyClientRpc(self.normalizedTimeOfDay + 0.1f, self.votesForShipToLeaveEarly);
            }
            else
            {
                // This updates the UI of other clients to reflect the amount of votes correctly, otherwise there will be a desync in the UI elements
                for (int i = 0; i < customVoteInt; i++)
                {
                    self.AddVoteForShipToLeaveEarlyClientRpc();

                }
            }
        }
        else
        {
            orig(self);
        }
    }

    public static void GetHUDManager(Action<HUDManager> orig, HUDManager self)
    {
        orig(self);
        hudManager = self;
    }
}