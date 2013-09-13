using UnityEngine;
using System.Collections;
/// <summary>
/// Combat window.
/// This script is attached to teh Game Manager and its purpose is to display the combat window. 
/// 
/// The Health and Damage Script Accesses this script.
/// </summary>

public class CombatWindow : MonoBehaviour {
	//Varibles Start---------------------------------------
	
	//These varibles are affected by the health and Health and Damage script.
	public string attackerName;
	
	public string destroyedName;
	
	public bool addNewEntry = false;
	
	//The string displayed in the combat log.
	private string combatLog;
	
	//The size limit for the combat log.
	private int charicterLimit = 10000;
	
	//Used in defining the combat log window.
	public Rect windowRect;
	private int windowLeft = 10;
	private int windowTop = 150;
	private int windowWidth =300;
	private int windowHeight = 150;
	private GUIStyle myStyle = new GUIStyle();
	
	//Used to scroll the combat log entrys
	private float nextScrollTime = 0;
	private float scrollRate = 12;
	
	//Varibles End ----------------------------------------
	
	// Use this for initialization
	void Start () 
	{
		myStyle.fontStyle = FontStyle.Bold;
		myStyle.fontSize = 11;
		myStyle.normal.textColor = Color.green;
		myStyle.wordWrap = true;
	}
	
	void CombatWindowFucnction(int windowID)
	{
		GUILayout.Label(combatLog, myStyle);
	}
	void OnGUI()
	{
		//Run this code for both the server and the client.
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			windowTop = Screen.height - 350;
			
			windowRect = new Rect(windowLeft, windowTop, windowWidth, windowHeight);
			
			//If a player has been destoyed then add new entry would have been set to true my HDScript. 
			if(addNewEntry == true)
			{
				//Combat Log limiter. mostly for demo I think. The latest attacker and destroyed names will be on a single line.
				//concatenated and the previous lines are pushed down a line.
				
				if(combatLog.Length <= charicterLimit)
				{
					combatLog = attackerName + " >>> " + destroyedName +"\n" +combatLog;
					
					//A time for when the contents in the combat log should shift down a line
					nextScrollTime = Time.time + scrollRate;
					
					addNewEntry = false;
				}
				if(combatLog.Length > charicterLimit)
				{
					combatLog = attackerName + " >>> " + destroyedName;
				}
			}
			windowRect =GUI.Window(4,windowRect, CombatWindowFucnction, "Combat Log");
			//This creates the scrolling effect for the combat log if noting has happened recently
			
			if(Time.deltaTime > nextScrollTime && addNewEntry == false)
			{
				combatLog = "\n" + combatLog;
				
				nextScrollTime= Time.time + scrollRate; 
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
