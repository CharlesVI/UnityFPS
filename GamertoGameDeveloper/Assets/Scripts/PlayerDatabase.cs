using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player database.
/// this script manages the player list.
/// This script is attached to the GameManager
/// Script is accessed by the player name script
/// This script is accessed by the HealthAndDamage script
/// this cript is accessed by the player score script
/// This Script is accessed by the spawn script.
/// 
/// this script is accessed by the score table script
/// </summary>

public class PlayerDatabase : MonoBehaviour {
	
	
	//Varibles Start ----------------------------------------------
	
	//Type and then Name
	public List <PlayerDataClass> PlayerList = new List<PlayerDataClass>();
	
	public NetworkPlayer networkPlayer;
	
	//these is used to update the player list
	public bool nameSet = false;
	
	public string playerName;
	
	//These is used to update the player list with the score of the player.
	public bool scored = false;
	public int playerScore;
	
	//These are used to update the player list with the players team.
	public bool joinedTeam = false;
	public string playerTeam;
	
	//these are used when the match restarts
	public List<NetworkPlayer> nPlayerList = new List<NetworkPlayer>();
	
	public bool matchRestarted = false;
	public bool addPlayerAgain = false;
	
	//Varibles End ------------------------------------------------
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(nameSet == true)
		{
			//Edit player list to add the name
			networkView.RPC ("EditPlayerListWithName", RPCMode.AllBuffered, Network.player, playerName);
			
			nameSet = false;
		}
		
		if(scored == true)
		{
			//Edit the players score in their record in the list
			networkView.RPC ("EditPlayerListWithScore", RPCMode.AllBuffered, Network.player, playerScore);
			
			scored = false;
		}
		
		if(joinedTeam == true)
		{
			//Edit the players record to include the team they are on.
			networkView.RPC ("EditPlayerListWithTeam", RPCMode.AllBuffered, Network.player, playerTeam);
			
			joinedTeam = false;
		}
		
		//When a match is restarted the player list is deleted. The reload level script is accessing this script afterthe level has reloaded.
		if(Network.isServer == true && addPlayerAgain == true)
		{
			foreach(NetworkPlayer netPlayer in nPlayerList)
			{
				networkView.RPC ("AddPlayerToList", RPCMode.AllBuffered, netPlayer);
			}
			nPlayerList.Clear();
			
			addPlayerAgain = false;
		}
		
		if(Network.isClient == true && matchRestarted == true)
		{
			networkView.RPC ("AddPlayerBack", RPCMode.Server, Network.player);
			
			matchRestarted = false;
		}
	}
	
	void OnPlayerConnected (NetworkPlayer netPlayer)
	{
		//Add player to the list. This is excecuted on the server.
		
		networkView.RPC ("AddPlayerToList",RPCMode.AllBuffered, netPlayer);
	}
	
	void OnPlayerDisconnected (NetworkPlayer netPlayer)
	{
		//Remove the player from the list on disconnect
		networkView.RPC ("RemovePlayerFromList", RPCMode.AllBuffered, netPlayer);
	}
	
	[RPC]
	void AddPlayerToList (NetworkPlayer nPlayer)
	{
		//Create a new entry in the PlayerList and supply the players Network ID as the first entry
		PlayerDataClass capture = new PlayerDataClass();
		
		capture.networkPlayer = int.Parse(nPlayer.ToString());
		
		PlayerList.Add (capture);
	}
	
	[RPC]
	void RemovePlayerFromList (NetworkPlayer nPlayer)
	{
		//Find the player in the player list based on network ID and then remove
		for(int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].networkPlayer == int.Parse (nPlayer.ToString()))
			{
				PlayerList.RemoveAt (i);
			}
		}
	}
	
	[RPC]
	void EditPlayerListWithName (NetworkPlayer nPlayer, string pName)
	{
		//Find the player in the player list based on their networkPlayer IP and add their name to the list
		for(int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].networkPlayer == int.Parse(nPlayer.ToString()))
			{
				PlayerList[i].playerName = pName;
			}
		}
	}
	
	[RPC]
	void EditPlayerListWithScore (NetworkPlayer nPlayer, int pScore)
	{
		//Find the player in the player list based on their networkPlayer IP and add edit their score
		for(int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].networkPlayer == int.Parse(nPlayer.ToString()))
			{
				PlayerList[i].playerScore = pScore;
			}
		}
	}
	
	[RPC]
	void EditPlayerListWithTeam (NetworkPlayer nPlayer, string pTeam)
	{
		//Find the player in the player list based on their networkPlayer IP and add their team to the list
		for(int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].networkPlayer == int.Parse(nPlayer.ToString()))
			{
				PlayerList[i].playerTeam = pTeam;
			}
		}
	}
	
	//This RPC is only sent to the server and is used when the match is restarted. Client requiest to be re added to the player list.
	[RPC]
	void AddPlayerBack(NetworkPlayer nPlayer)
	{
		//the players to be added are recorded in a list so that no player is missed
		nPlayerList.Add(nPlayer);
		
		addPlayerAgain = true;
	}
}
