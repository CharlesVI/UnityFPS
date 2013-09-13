using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Score table.
/// This script is attached to the GameManager and is responsible for displaying the player scoreboard
/// and keeping team scores up to date.
/// 
/// This script makes use of the player data class so that it can construct the sortinglist.
/// 
/// This script accesses the player database so that the player list can be copied into the sorting list.
/// This script accesses the multiplayer script to obtain the defualt winning criteria.
/// this script accesses reload level when a team reaches the winning score.
/// 
/// This script is accessed by the player score script to update the gamescore.
/// </summary>

public class ScoreTable : MonoBehaviour 
{
	//Varibles Start ---------------------------------------------------
	
	//These varibles are used in displaying the score board
	public bool showScoreTable = false;
	public List<PlayerDataClass> SortingList = new List<PlayerDataClass>();
	
	private GUIStyle myStyle = new GUIStyle();
	
	//These are used in managing the team score and dipslyaing it.
	private GUIStyle redHeaderStyle = new GUIStyle();
	private GUIStyle blueHeaderStyle = new GUIStyle();
	
	public bool updateRedScore = false;
	public bool updateBlueScore = false;
	
	public int enemiesDestroyedInOneHit;
	
	public bool serverRefreshScore = false;
	public int redTeamScore;
	public int blueTeamScore;
	
	//These are used for the winning score
	private GUIStyle winStyle = new GUIStyle();
	
	public bool redTeamHasWon = false;
	public bool blueTeamHasWon = false;
	
	public int winningScore;
	public int waitTime = 7;
	//Varibles End -----------------------------------------------------
	
	
	// Use this for initialization
	void Start () 
	{
		myStyle.fontStyle = FontStyle.Bold;
		
		myStyle.normal.textColor = Color.white;
		
		
		
		redHeaderStyle.fontSize = 16;
		redHeaderStyle.fontStyle = FontStyle.Bold;
		redHeaderStyle.normal.textColor = Color.red;
		
		blueHeaderStyle.fontSize = 16;
		blueHeaderStyle.fontStyle = FontStyle.Bold;
		blueHeaderStyle.normal.textColor = Color.blue;
		
		winStyle.fontSize = 40;
		winStyle.normal.textColor = Color.white;
		winStyle.fontStyle = FontStyle.Bold;
		winStyle.alignment = TextAnchor.MiddleCenter;
		
		//Get the default winning criteria from the Multiplayer script.
		
		GameObject multiManager = GameObject.Find("MultiplayerManager");
		
		MultiplayerScript multiScript = multiManager.GetComponent<MultiplayerScript>();
		
		winningScore = multiScript.winningScore;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButton("Show Player Scores"))
		{
			showScoreTable = true;
		}
		
		if(Input.GetButtonUp("Show Player Scores"))
		{
			showScoreTable = false;
		}
		
		//whenever the players score increases in their playerscore script.
		//theplayer score script will send a signal to this script to increment the overall team score.
		//An rpc is sent out accross the network so that everyone gets the latest team score
		
		if(updateBlueScore == true)
		{
			for(int i = 0; i < enemiesDestroyedInOneHit; i++)
			{
				networkView.RPC ("UpdateBlueTeamScore", RPCMode.All);
			}
			enemiesDestroyedInOneHit = 0;
			updateBlueScore = false;
		}
		
		if(updateRedScore == true)
		{
			for(int i = 0; i < enemiesDestroyedInOneHit; i++)
			{
				networkView.RPC ("UpdateRedTeamScore", RPCMode.All);
			}
			enemiesDestroyedInOneHit = 0;
			updateRedScore = false;
		}
		
		//server needs to refresh the score because when the player who scored leaves their rpcs are deleted. So new players will not get there buffered updates.
		
		if(Network.isServer && serverRefreshScore == true)
		{
			networkView.RPC ("ServerRefreshScore", RPCMode.AllBuffered, redTeamScore, blueTeamScore);
			
			serverRefreshScore = false;
		}
		
		//If either team has reached the winning score then activate the appropriate bool
		//Then Edit the cursor controll script on this player is constantly waatching to see if blue team has won. and the player will become paused.
		
		if(blueTeamScore >= winningScore)
		{
			blueTeamHasWon = true;
		}
		if(redTeamScore >= winningScore)
		{
			redTeamHasWon = true;
		}
	}
	void OnGUI()
	{
		if(showScoreTable == true)
		{
			//Clear the list used for displaying players and their scores.
			SortingList.Clear();
			
			//access the player database as we will copy the contents of the player list.
			PlayerDatabase dataScript = transform.GetComponent<PlayerDatabase>(); //Transform b/c they are both on GameManager
			
			//copy each item in the player list into the sorting list.
			for(int i = 0; i < dataScript.PlayerList.Count; i++)
			{
				SortingList.Add(dataScript.PlayerList[i]);
			}
			
			//Sort the sortinglist by default  players wil be sorted in ascending score. so this will get reversed to display better.
			
			SortingList.Sort(delegate(PlayerDataClass player1, PlayerDataClass player2)
			{
				return player1.playerScore.CompareTo(player2.playerScore);	
			});
			
			//Display the score board header.
			
			GUI.Box(new Rect(Screen.width / 2 -260, 10, 520, 30), "");
			
			GUI.Label(new Rect(Screen.width /2 -150, 15, 300, 30), "The team that reaches a score of " + winningScore.ToString () + " wins", myStyle);
			
			
			
			//Start a new GUI area on the left portion of the screen. This area will be used for displaying red team scores.
			
			GUILayout.BeginArea(new Rect(Screen.width / 2 -260,50,250,Screen.height - 10));
			
			GUILayout.BeginVertical("box");
			
			GUILayout.BeginHorizontal("");
			
			//Display a header with the team name and score.
			GUILayout.Label("Red Team" , redHeaderStyle, GUILayout.Width (200));
			
			GUILayout.Label(redTeamScore.ToString(), redHeaderStyle, GUILayout.Width(40));
			
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
			
			
			
			
	
			
			GUILayout.EndArea();
			
			
			//Start a new GUI area on the left portion of the screen. This area will be used for displaying blue team scores.
			GUILayout.BeginArea(new Rect(Screen.width / 2 +10,50,250,Screen.height - 10));
			
			//Go through the sortingList in reverse and pick out each player that belongs to the blue team and display their name and score.
			for(int i = SortingList.Count - 1; i >= 0; i--)
			{
				if(SortingList[i].playerTeam == "blue")
				{
					GUILayout.BeginHorizontal("box"); 
					
					GUILayout.Label(SortingList[i].playerName, myStyle, GUILayout.Width(200));
					
					GUILayout.Label(SortingList[i].playerScore.ToString(), myStyle, GUILayout.Width (40));
					
					GUILayout.EndHorizontal();
				}
			}
			//Blue stuff
			GUILayout.BeginVertical("box");
			
			GUILayout.BeginHorizontal("");
			
			//Display a header with the team name and score.
			GUILayout.Label("Blue Team" , blueHeaderStyle, GUILayout.Width (200));
			
			GUILayout.Label(blueTeamScore.ToString(), blueHeaderStyle, GUILayout.Width(40));
			
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
			
			//Go through the sortingList in reverse and pick out each player that belongs to the red team and display their name and score.
			
			for(int i = SortingList.Count - 1; i >= 0; i--)
			{
				if(SortingList[i].playerTeam == "red")
				{
					GUILayout.BeginHorizontal("box"); 
					
					GUILayout.Label(SortingList[i].playerName, myStyle, GUILayout.Width(200));
					
					GUILayout.Label(SortingList[i].playerScore.ToString(), myStyle, GUILayout.Width (40));
					
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndArea();	
		}
		//When a team wins display a box covering the screenand overlay the winning message on top. 
		//Only the server has the authority to restart a match once the restartMatch timer has gone to zero.
		
		if(blueTeamHasWon == true)
		{
			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
			
			GUI.Box(new Rect(0,0, Screen.width,Screen.height), "Blue Team has won the match!", winStyle);
			
			if(Network.isServer)
			{
				StartCoroutine(RestartMatch());
			}
		}
		
		if(redTeamHasWon == true)
		{
			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
			
			GUI.Box(new Rect(0,0, Screen.width,Screen.height), "Red Team has won the match!", winStyle);
			
			if(Network.isServer)
			{
				StartCoroutine(RestartMatch());
			}
			
		}
	}
	[RPC]
	void UpdateRedTeamScore()
	{
		redTeamScore++;
		
		serverRefreshScore = true;
	}
	
	[RPC]
	void  UpdateBlueTeamScore()
	{
		blueTeamScore++;
		
		serverRefreshScore = true;
	}
	
	[RPC]
	void ServerRefreshScore(int redScore, int blueScore)
	{
		redTeamScore = redScore;
		
		blueTeamScore = blueScore;
	}
	
	//Used to restart the match by the server.
	
	void RestartGame()
	{
		GameObject reload = GameObject.Find("ReloadLevel");
		
		ReloadLevelScript reloadScript = reload.GetComponent<ReloadLevelScript>();
		
		reloadScript.reloadLevel = true;
	}
	
	IEnumerator RestartMatch()
	{
		yield return new WaitForSeconds(waitTime);
		
		RestartGame();
	}
}
