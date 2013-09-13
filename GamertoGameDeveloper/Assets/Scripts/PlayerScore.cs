using UnityEngine;
using System.Collections;

/// <summary>
/// Player score.
/// this script is attached to pLayers
/// 
/// This script access the player database script
/// inthe game manager to access the player list.
/// 
/// this script access the score table script to incriment the team score
/// this script access the spawn script to see what team the player is on.
/// 
/// this script is accessed by the health and damage script.
/// </summary>

public class PlayerScore : MonoBehaviour 
{
	//Varibles Start ---------------------------------------
	
	public string destroyedEnemyName;
	public bool iDestroyedAnEnemy = false;
	
	
	public int enemiesDestroyedInOneHit;
	public int myScore;
	
	//Varibles End -----------------------------------------

	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == false)
		{
			enabled = false;
		}
		//When the player spawns they need to access their PlayerList on their game instance and retrive there score from the list.
		//This maintains there score throughout lives.
		GameObject gameManager = GameObject.Find ("GameManager");
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		
		for(int i = 0; i < dataScript.PlayerList.Count; i++)
		{
			if(dataScript.PlayerList[i].networkPlayer == int.Parse(Network.player.ToString()))
			{
				myScore = dataScript.PlayerList[i].playerScore;
				
				//When players are destroyed their RPCs are deleted so we need to tell the player database to update itself across the network 
				//so that new players will see the correct score.
				
				UpdateScoreInPlayerDatabase(myScore);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//When the player destroys an enemy increment the player's score and save their current score in the PlayerDatabase script.
		if(iDestroyedAnEnemy == true)
		{
			//enemiesDestroyed in one hit is a counter and takes into account if the player destroeyed more than one enemy in a single hit.
			//If they did then the score should be incremented for each kill. This happens with AOE
			
			for(int i = 0; i < enemiesDestroyedInOneHit; i++)
			{
				myScore++;
				
				UpdateScoreInPlayerDatabase(myScore);
				
				TellScoreTableToUpdateTeamScore();
			}
			enemiesDestroyedInOneHit = 0;
			iDestroyedAnEnemy = false;
		}
	}
	
	void UpdateScoreInPlayerDatabase (int score)
	{
		GameObject gameManager = GameObject.Find ("GameManager");
		
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		
		dataScript.scored = true;
		
		dataScript.playerScore = score;
		
	}
	
	//Inform the local score tambel script that the team this player is on should get a poitn because this
	//player has scored. the score table script wil then send out and rpc accros the network incrementing this teams score
	
	void TellScoreTableToUpdateTeamScore()
	{
		GameObject spawnManager = GameObject.Find("SpawnManager");
		
		SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
		
		GameObject gameManager = GameObject.Find("GameManager");
		
		ScoreTable tableScript = gameManager.GetComponent<ScoreTable>();
		
		if(spawnScript.amIOnTheBlueTeam == true)
		{
			tableScript.updateBlueScore = true;
			
			tableScript.enemiesDestroyedInOneHit = enemiesDestroyedInOneHit;
		}
		
		if(spawnScript.amIOnTheRedTeam == true)
		{
			tableScript.updateRedScore = true;
			
			tableScript.enemiesDestroyedInOneHit = enemiesDestroyedInOneHit;
		}
	}
}
