
/// <summary>
/// Player data class.
/// this script is not attached to any GameObject but it is used by the player database script in building the player list.
/// </summary>

public class PlayerDataClass
{
	//Varibles Start ----------------------------------------------------
	
	public int networkPlayer;
	
	public string playerName;
	
	public int playerScore;
	
	public string playerTeam;
	
	//Varibles End -----------------------------------------------------
	
	
	public PlayerDataClass Constructor ()
	{
		PlayerDataClass capture = new PlayerDataClass();
	
		capture.networkPlayer = networkPlayer;

		capture.playerName = playerName;

		capture.playerScore = playerScore;

		capture.playerTeam = playerTeam;

		return capture;
	}
}
