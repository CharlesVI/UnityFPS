using UnityEngine;
using System.Collections;

/// <summary>
/// Reload level script.
/// This script is attached to the ReloadLevel GameObject
/// 
/// This script accesses:
 /// the playerDatabase, the spawn script
 /// 
 /// This script is accessed by the:
 /// Score table script
/// 
/// 
/// </summary>

public class ReloadLevelScript : MonoBehaviour 
{

	//Varibles Start
	
	public bool reloadLevel = false;
	public bool iAmRestartingMatch = false;
	
	public float waitTime = 0.1f;
	
	private static bool created = false;
	
	//Varibles End
	
	
	void Awake()//Awake only runs on the first instantiation not the second ... even if you make a new version of it?
	{
		//The reload level game object is the only GameObject to remain through reload. Everything else will be recreated
		//So we will ensure that this is not recreated. 
		if(created == false)
		{
			DontDestroyOnLoad(gameObject);
			
			created = true;
		}
		else
		{
			Destroy(gameObject);	
		}
		//I honestly do not understand the need for this. 
		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//The scoreTable script will set reload Level to true when a team wins. This happens on the server only.
		//Server will tell all the players to reload the level. 
		if(reloadLevel == true && Network.isServer)
		{
			//Send an RPC to everyone telling them to restart the match.
			networkView.RPC ("RestartMatch", RPCMode.All);
			
			reloadLevel = false;
			
		}
		if(iAmRestartingMatch == true)
		{
			//Access the playerDatabase and tell it that the match is restarting
			//The playerDatabase will then have each of the players asdded back into the player list
			GameObject gameManager = GameObject.Find ("GameManager");
			
			PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
			
			dataScript.matchRestarted = true;
			
			//access the spawn script and tell it that the match is restarting. The spawn script will then allow the player to choose a team again.
			GameObject spawnManager = GameObject.Find("SpawnManager");
			
			SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
			
			spawnScript.matchRestart = true;
			
			iAmRestartingMatch = false;
		}
	}
	
	[RPC]
	void RestartMatch()
	{
		//Delete all RPCs and stop communications network.
		Network.RemoveRPCs(Network.player);
		
		Network.SetSendingEnabled(0, false);
		
		Network.SetSendingEnabled(1, false);
		
		Network.isMessageQueueRunning = false;
		
		Application.LoadLevel("Series 1 Prototype");
		
		//Use coroutine to give the level a few moments to load(really need load screens) before allowing network communications to resume
		
		StartCoroutine(Delay());
	}
	
	IEnumerator Delay()
	{
		yield return new WaitForSeconds(waitTime);
		
		Network.isMessageQueueRunning = true;
		
		
		Network.SetSendingEnabled(0, true);
		
		Network.SetSendingEnabled(1, true);
		
		iAmRestartingMatch = true;
		
		
	}
}
