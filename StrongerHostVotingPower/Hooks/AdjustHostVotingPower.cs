using System;
using ES3Types;
using GameNetcodeStuff;

namespace StrongerHostVotingPower.Hooks;

static class AdjustHostVotingPower
{
    // TO DO: Make these a config
    public static bool useCustomVotingPower = true;
    public static float customVotePercentile = 500;
    public static int customVoteInt = 0;

    private static bool hostCalledVote = false;
    private static int totalVotesNeeded = 0;


    public static void CheckForHostVote(Action<TimeOfDay> orig, TimeOfDay self)
    {
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
        StrongerHostVotingPower.Logger.LogDebug($"host called vote: {hostCalledVote}");
        if (useCustomVotingPower && hostCalledVote)
        {
            StartOfRound startOfRound = StartOfRound.Instance;
            totalVotesNeeded = startOfRound.connectedPlayersAmount + 1 - startOfRound.livingPlayers; // total votes needed
            float voteSmallPercentile = customVotePercentile / 100;
            customVoteInt = (int)MathF.Round(totalVotesNeeded * voteSmallPercentile);
            // Set the vote count to 1 because having 0 or less votes is not intended
            if (customVoteInt <= 0) 
            {
                customVoteInt = 0;
                customVoteInt++;
            }
            
            self.votesForShipToLeaveEarly += customVoteInt;
            StrongerHostVotingPower.Logger.LogDebug($"Increased votes by {customVoteInt} through mod");
            // Possible: Never go over the limit of votes instead match it perfectly if the voting power is bigger than the needed votes

            if (self.votesForShipToLeaveEarly >= totalVotesNeeded + 99)
            {
                self.SetShipLeaveEarlyClientRpc(self.normalizedTimeOfDay + 0.1f, self.votesForShipToLeaveEarly);
            }
            else
            {
                for (int i = 0; i < customVoteInt; i++)
                {
                    self.AddVoteForShipToLeaveEarlyClientRpc();

                }
            }
            // self.votedShipToLeaveEarlyThisRound = true;
        }
        else
        {
            orig(self);
        }
    }
}