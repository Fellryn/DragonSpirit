
/// <summary>
/// TODO: Move to own file
/// </summary>
[System.Serializable]
public struct PlayerDataForLeaderboard //Creates place to store the variables for the name and score of each player
{
    public string fullUsername;
    public string username;
    public int score;
    public int time;
    public string comment;

    public PlayerDataForLeaderboard(string _username, int _score, int _time = 0, string _comment = "", string _fullUsername = "")
    {
        fullUsername = _fullUsername;
        username = _username;
        score = _score;
        time = _time;
        comment = _comment;
    }
}