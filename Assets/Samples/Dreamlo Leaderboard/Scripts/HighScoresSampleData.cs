using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoresSampleData : MonoBehaviour
{
    [SerializeField] HighScores highScores;

    private void Start()
    {
        if (highScores == null)
        {
            Debug.LogError($"Need to link to Highscores component. {this.name} on {this.gameObject.name} has been disabled.");
            this.enabled = false;
            return;
        }
    }

    /// <summary>
    /// Used to add inital Random data to Dreamlo.com leaderboards
    /// <para> Some cool online tools: https://wheelofnames.com/ </para>
    ///  <para>In the end I just used common game characters</para>
    /// </summary>
    /// <param name="useReallyHighFirstPlace"></param>
    public void Random25Records(bool useReallyHighFirstPlace = false)
    {
        List<PlayerDataForLeaderboard> data = new List<PlayerDataForLeaderboard>()
        {
           //new PlayerDataForLeaderboard("Rayman", 10, 60, "Average"),
           new PlayerDataForLeaderboard("MaxPayne", 10, 60, "Average"),
           new PlayerDataForLeaderboard("SubZero", 10, 60, "Average"),
           new PlayerDataForLeaderboard("Scorpion", 10, 60, "Get over here"),
           //new PlayerDataForLeaderboard("CloudStrife", 10, 60, "Average"),
           new PlayerDataForLeaderboard("JohnMarston", 10, 60, "Average"),
           new PlayerDataForLeaderboard("Geralt", 10, 60, "Average"),
           new PlayerDataForLeaderboard("SamFisher", 10, 60, "Average"),
           new PlayerDataForLeaderboard("CommanderShepard", 10, 60, "Average"),
           new PlayerDataForLeaderboard("JillValentine", 10, 60, "Average"),
           new PlayerDataForLeaderboard("Agent47", 10, 60, "Average"),
           new PlayerDataForLeaderboard("Kratos", 10, 60, "Average"),
           new PlayerDataForLeaderboard("PacMan", 10, 60, "Yum"),
           new PlayerDataForLeaderboard("SolidSnake", 10, 60, "Average"),
           new PlayerDataForLeaderboard("CrashBandicoot", 10, 60, "Average"),
           new PlayerDataForLeaderboard("DonkeyKong", 10, 60, "Average"),
           new PlayerDataForLeaderboard("LaraCroft", 10, 60, "I am puzzled"),
           new PlayerDataForLeaderboard("MasterChief", 10, 60, "Halo"),
           new PlayerDataForLeaderboard("Link", 10, 60, "Average"),
           new PlayerDataForLeaderboard("Sonic", 10, 60, "Speed is the key"),
           new PlayerDataForLeaderboard("Pikachu", 10, 60, "Got to catch them all"),
           new PlayerDataForLeaderboard("Mario", 10, 60, "Average"),
           new PlayerDataForLeaderboard("NikoBellic", 10, 60, "GTA"),
           new PlayerDataForLeaderboard("Steve", 10, 60, "Minecraft for life"),
           new PlayerDataForLeaderboard("GordonFreeman", 10, 60, "Average"),
           new PlayerDataForLeaderboard("NathanDrake", 10, 60, "Average"),
           new PlayerDataForLeaderboard("Spyro", 10, 60, "Average"),
           new PlayerDataForLeaderboard("ZblockTetromino", 10, 60, "Squiggly"),
           new PlayerDataForLeaderboard("RicoRodriguez", 10, 60, "Just Cause")
        };

        for (int i = 0; i < 25; i++)
        {
            int idToUse = UnityEngine.Random.Range(0, data.Count);
            PlayerDataForLeaderboard itemToUse = data[idToUse];
            data.RemoveAt(idToUse);
            highScores.UploadScoreTimeComment(
                itemToUse.username,
                UnityEngine.Random.Range(42, 200) * (i + 1),
                UnityEngine.Random.Range(45, 325),
                itemToUse.comment,
                overrideSamePlayer: true
                );
        }

        if (useReallyHighFirstPlace)
        {
            highScores.UploadScoreTimeComment("Steve", 35000, 5, "Minecraft4Life", overrideSamePlayer: true);
        }
    }
}
