using UnityEngine;
using System.Collections;

/// <summary>
/// Spawn script.
/// This Script is attached to the spawn manager 
/// It allows the player to spawn into the multiplayer game 
/// 
/// fireblaster script accesses this to see what team the player is on. 
/// This script is Acessed by health and damage to set IamDestroyed to true.
/// Accesses the PlayerDatabase script to supply the team that the player belongs to.
/// 
/// this script is accessed by the assign health script to see if first spawn is true.
/// </summary>


public class SpawnScript : MonoBehaviour 
{
	
	//Varibles Start --------------------------------------------------------

	//used to determine if the player needs to spawn
	private bool justConnectedToServer = false;

	//Used to determine what team teh player is on.
	public bool amIOnTheRedTeam = false;
	
	public bool amIOnTheBlueTeam = false;
	
	//Define the join team window
	
	private Rect joinTeamRect;
	
	private string joinTeamWindowTitle = "Team Selection";
	
	private int joinTeamWindowWidth = 330;
	
	private int joinTeamWindowHeight = 100;
	
	private int joinTeamLeftIndent;
	
	private int joinTeamTopIndent;
	
	private int buttonHeight = 40;
	
	//Player prefabs are connected to these in the inspector. 
	public Transform redTeamPlayer;
	
	public Transform blueTeamPlayer;
	
	private int redTeamGroup = 0;
	
	private int blueTeamGroup = 1;
	
	//Used to Capture spawn points.
	private GameObject[] redSpawnPoints;
	
	private GameObject[] blueSpawnPoints;
	
	
	//Used in determining whether the player is destroyed.
	public bool iAmDestroyed = false;
	
	//Used to see if first spawn
	public bool firstSpawn = false;
	
	//Used in allowing the player to select a team again after the match has restarted.
	public bool matchRestart = false;

	//Varibles End ----------------------------------------------------------
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnConnectedToServer()
	{
		//UnityStandaredFUnction
		justConnectedToServer = true;
	}
	
	void JoinTeamWindow(int windowID)
	{
		//Only show these two buttons on first connect or on a new match.
		if(justConnectedToServer == true || matchRestart == true)
		{
			//if the player clicks on the join red team button then assign them to the red team and spawn them.
			if(GUILayout.Button("Join Red Team", GUILayout.Height (buttonHeight)))
			{
				amIOnTheRedTeam = true;
				
				justConnectedToServer = false;
				
				matchRestart = false;
				
				SpawnRedTeamPlayer();
				
				firstSpawn = true;
			}
			
			
			//if the player clicks on the join blue team button then assign them to the blue team and spawn them.
			if(GUILayout.Button("Join Blue Team", GUILayout.Height (buttonHeight)))
			{
				amIOnTheBlueTeam = true;
				
				justConnectedToServer = false;
				
				matchRestart = false;
				
				SpawnBlueTeamPlayer();
				
				firstSpawn = true;
			}
		}
		if(iAmDestroyed == true) // Respawn timer could go here.
		{
			if(GUILayout.Button("Respawn", GUILayout.Height(buttonHeight*2)))
			{
				if(amIOnTheRedTeam == true)
				{
					SpawnRedTeamPlayer();
				}
				if(amIOnTheBlueTeam == true)
				{
					SpawnBlueTeamPlayer();
				}
				iAmDestroyed = false;
			}
		}
	}
	
	void OnGUI()
	{
		//If the player has just connected to the server then draw the join team window.
		if(justConnectedToServer == true || iAmDestroyed == true || matchRestart == true && Network.isClient)
		{
			Screen.lockCursor = false;
			
			joinTeamLeftIndent = Screen.width /2 - joinTeamWindowWidth /2;
			
			joinTeamTopIndent = Screen.height/ 2 - joinTeamWindowHeight/2;
			
			joinTeamRect = new Rect(joinTeamLeftIndent, joinTeamTopIndent, joinTeamWindowWidth, joinTeamWindowHeight);
			
			joinTeamRect = GUILayout.Window(0, joinTeamRect, JoinTeamWindow, joinTeamWindowTitle);
		}
	}
	
	void SpawnRedTeamPlayer()
	{
		//Find all Red spawn points and refrence them
		redSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnRedTeam");
		
		//Randomly Select one spawn point.
		GameObject randomRedSpawn = redSpawnPoints[Random.Range (0,redSpawnPoints.Length)];
		
		//Instatntiate the player at the randomly selected spawn point.
		Network.Instantiate (redTeamPlayer, randomRedSpawn.transform.position, randomRedSpawn.transform.rotation, redTeamGroup);
		
		GameObject gameManager = GameObject.Find ("GameManager");
		
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		
		dataScript.joinedTeam = true;
		
		dataScript.playerTeam = "red";
	}
	
	void SpawnBlueTeamPlayer()
	{
		//Find all blue spawn points and refrence them
		blueSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnBlueTeam");
		
		//Randomly Select one spawn point.
		GameObject randomBlueSpawn = blueSpawnPoints[Random.Range (0,blueSpawnPoints.Length)];
		
		//Instatntiate the player at the randomly selected spawn point.
		Network.Instantiate (blueTeamPlayer, randomBlueSpawn.transform.position, randomBlueSpawn.transform.rotation, blueTeamGroup);
		
			//Tells the player dtabase what team the player belongs to.	
		GameObject gameManager = GameObject.Find ("GameManager");
		
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		
		dataScript.joinedTeam = true;
		
		dataScript.playerTeam = "blue";
	}
}
