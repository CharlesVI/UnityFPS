using UnityEngine;
using System.Collections;
/// <summary>
/// Communication window.
/// This script is attached to the game Manager;
/// 
/// This script accesses the spawnscript to see if the player has joined a team.
/// 
/// This script is accessed by the player name script. 
/// This script is accessed by the cursorControl script.
/// </summary>

public class CommunicationWindow : MonoBehaviour 
{

	//Varibles Start -----------------------------------------------
	
	//String of the plaeyr name. Supplied by the player name script
	public string playerName;
	
	//These are used in sending a message.
	private string messageToSend;
	private string communication;
	private bool showTextBox= false;
	private bool sendMessage = false;
	public bool unlockCursor = false;
	
	//These are used to define the communication window.
	private Rect windowRect;
	private int windowLeft = 10;
	private int windowTop;
	private int windowWidth = 300;
	private int windowHeight = 140;
	private int padding = 20;
	private int textFeildHeight = 30;
	private Vector2 scrollPosition;
	private GUIStyle myStyle = new GUIStyle();
	
	//quick refrences
	private GameObject spawnManager;
	private SpawnScript spawnScript;
	
	//Used to inform all the other players that a new player has connected.
	
	public bool tellEveryoneIJoined = true;
	
	//Varibles End -------------------------------------------------
	
	// Use this for initialization
	void Awake()
	{
		//allow my pressing the return key to be recognized when using the text feild
		Input.eatKeyPressOnTextFieldFocus = false;
		
		messageToSend = "";
		
		myStyle.normal.textColor = Color.white;
		myStyle.wordWrap = true;
		
		
	}
	
	void Start () 
	{
		spawnManager = GameObject.Find("SpawnManager");
		spawnScript = spawnManager.GetComponent<SpawnScript>();
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			//If the player presses the T key then set the show text box to true.
			//This will allow the player to type in messages.
			
			if(Input.GetButtonDown("Communication") && showTextBox == false)
			{
				showTextBox = true;
			}
			
			//If the player presses return and the textfeild is visible then set send message to true.
			if(Input.GetButtonDown("Send Message") && showTextBox == true)
			{
				sendMessage = true;
			}
		}
		
		//When the player joins for the first Time. tell everyone they have joined and only send rpc if it is a client. and the playername is not empty.
		if(Network.isClient && tellEveryoneIJoined == true && playerName != "")
		{
			networkView.RPC("TellEveryonePlayerJoined", RPCMode.All, playerName);
			
			tellEveryoneIJoined = false;
		}
	}
	
	void CommLogWindow (int windowID)
	{
		//begin a scroll view so that as the label increases with length the scroll bar will appear and allow the player to view past messages.
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width (windowWidth - padding),GUILayout.Height(windowHeight - padding - 5));
		
		GUILayout.Label (communication, myStyle);
		GUILayout.EndScrollView();
	}
	
	void OnGUI()
	{
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			windowTop = Screen.height - windowHeight - textFeildHeight;
			
			windowRect = new Rect(windowLeft, windowTop, windowWidth, windowHeight);
			
			//Access the spawn script so that we can check if they are in. Do not display comm log unitll they have joined or are a server.
			if(spawnScript.amIOnTheRedTeam == true || spawnScript.amIOnTheBlueTeam == true || Network.isServer == true)
			{
				windowRect = GUI.Window(5, windowRect, CommLogWindow, "Communication Log");
				
				GUILayout.BeginArea(new Rect(windowLeft, windowTop + windowHeight, windowWidth, windowHeight));
				
				//If show text box is true then allow the textfieldthat will allow the players to type messages.
				
				if(showTextBox == true)
				{
					unlockCursor = true; 
					Screen.lockCursor = false;
					
					//Give text feild a name so that it can be found. the GUI.FocusControl method does this
					GUI.SetNextControlName("MyTextField");
					
					messageToSend = GUILayout.TextField(messageToSend, GUILayout.Width (windowWidth));
					
					//Give focus to the textfeild so the user can immediatly start typing without having to click.
					GUI.FocusControl("MyTextField");
					
					if(sendMessage == true)
					{
						//Only send Message if text feild is not empty.
						//If it is empty and you press return just close the window.
						
						if(messageToSend != "")
						{
							if(Network.isClient == true)
							{
								networkView.RPC ("SendMessageToEveryone", RPCMode.All, messageToSend, playerName);
							}
							if(Network.isServer == true)
							{
								networkView.RPC ("SendMessageToEveryone", RPCMode.All, messageToSend, "Server");
							}	
						}
						//set Message to false so it doesnt keep sending
						sendMessage = false;
						
						//set showtext box to false too because its done.
						showTextBox = false;
						
						unlockCursor = false;
						
						//Reset message to send
						messageToSend = "";
					}
				}
				GUILayout.EndArea();
			}
		}
	}
	
	[RPC]
	void SendMessageToEveryone(string message, string pName)
	{
		//This string displayed by the label in the com log window
		
		communication = pName + " : " + message + "\n" + "\n" + communication;
	}
	
	[RPC]
	void TellEveryonePlayerJoined (string pName)
	{
		communication = pName + " has joined the game." + "\n" + "\n" + communication;	
	}
}
