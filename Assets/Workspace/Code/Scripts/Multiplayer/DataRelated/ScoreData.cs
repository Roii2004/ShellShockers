[System.Serializable]
public class ScoreData
{
    //Will be "used" by Json in ScoreSender
    public string playerName;
    public int score;

    public ScoreData(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}
