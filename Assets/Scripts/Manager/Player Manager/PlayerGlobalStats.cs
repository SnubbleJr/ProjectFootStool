public class PlayerGlobalStats
{
    public int level;
    public int xp;
    public int kills;
    public int deaths;
    public int SDs;
    public int jumps;
    public int flumps;
    public int gamesPlayed;
    public int gamesWon;

    public PlayerGlobalStats()
    {
    }

    public PlayerGlobalStats(int[] stats)
    {
        level = stats[0];
        xp = stats[1];
        kills = stats[2];
        deaths = stats[3];
        SDs = stats[4];
        jumps = stats[5];
        flumps = stats[6];
        gamesPlayed = stats[7];
        gamesWon = stats[8];
    }

    public int[] ToArray()
    {
        return new int[] { level, xp, kills, deaths, SDs, jumps, flumps, gamesPlayed, gamesWon };
    }
}