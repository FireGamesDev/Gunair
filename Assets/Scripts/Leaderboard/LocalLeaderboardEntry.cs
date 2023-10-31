using System.Collections.Generic;

[System.Serializable]
public class LocalLeaderboardEntry
{
    public string playerName;
    public int score;
}

[System.Serializable]
public class LocalLeaderboardWrapper
{
    public List<LocalLeaderboardEntry> entries;

    public LocalLeaderboardWrapper(List<LocalLeaderboardEntry> entries)
    {
        this.entries = entries;
    }
}