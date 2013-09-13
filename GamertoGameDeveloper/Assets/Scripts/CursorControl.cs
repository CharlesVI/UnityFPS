using UnityEngine;
using System.Collections;

/// <summary>
/// Cursor control.
/// This script is attached to the player
/// It controlls weather the cursor is locked or unlocked
/// 
/// This script accessses the multiplayer script
/// This script accesses the CommunicationWindowScript.
/// </summary>
public class CursorControl : MonoBehaviour {
	
	//Varibles Start -----------------------------------------------------------
	
	private GameObject multiplayerManager;
	
	private MultiplayerScript multiScript;
	
	private GameObject gameManager;
	private CommunicationWindow commScript;
	
	private ScoreTable scoreScript;
	
	//Varibles End--------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == false)
		{
			enabled = false;
		}
		multiplayerManager = GameObject.Find ("MultiplayerManager");
		
		multiScript = multiplayerManager.GetComponent<MultiplayerScript>();
		
		gameManager = GameObject.Find("GameManager");
		
		commScript = gameManager.GetComponent<CommunicationWindow>();
		
		scoreScript = gameManager.GetComponent<ScoreTable>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(multiScript.showDisconnectWindow == false && commScript.unlockCursor == false && scoreScript.blueTeamHasWon == false && scoreScript.redTeamHasWon == false)
		{
			Screen.lockCursor = true;
		}
		if(multiScript.showDisconnectWindow == true || commScript.unlockCursor == true || scoreScript.blueTeamHasWon == true || scoreScript.redTeamHasWon == true)
		{
			Screen.lockCursor = false;
		}
	}
}
