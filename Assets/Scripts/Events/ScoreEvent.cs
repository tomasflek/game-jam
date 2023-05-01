using Events;

public class  ScoreEvent : EventBase
{
	public Player Player { get; set; }
	public ScoreEvent(Player players)
	{
		Player = players;
	}
}

public class Player
{
	public string PlayerName;
	public int Score;
	public int PlayerIndex;
}
