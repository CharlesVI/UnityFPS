using UnityEngine;
using System.Collections;
/// <summary>
/// Player label.
/// This script is attached to the player and it draws the healthbar of the player above them
/// Also it draws the player name.
/// 
/// This script accesses health and damage for info about the HP
/// </summary>

public class PlayerLabel : MonoBehaviour {
	
	//varibles start ------------------------------------------------
	
	//The health bar texture is attached to this in the inspector
	public Texture healthTex;
	
	//refrences
	private Camera myCamera;
	private Transform myTransform;
	
	private Transform triggerTransform;
	private HealthAndDamage HDScript;
	
	
	//These are used in determining whether the healthbar should be drawn and where.
	private Vector3 worldPosition = new Vector3();
	private Vector3 screenPosition = new Vector3();
	private Vector3 cameraRelativePosition = new Vector3();
	private float minimumZ = 1.5f;
	
	//These varibles are used in defining the health bar
	private int labelTop = 18;
	private int labelWidth = 110;
	private int labelHeight = 15;
	private int barTop = 1;
	private int healthBarHeight = 5;
	private int healthBarLeft = 110;
	private float healthBarLength; 
	private float adjustment = 1; 
	
	
	//Used in displaying the players name.
	public string playerName;
	
	private GUIStyle myStyle = new GUIStyle();
	
	//varibles end --------------------------------------------------
	
	// Use this for initialization
	void Start () 
	{
		//This only runs for other player charicters
		if(networkView.isMine == true)
		{
			enabled = false;
		}
		
		myTransform = transform;
		
		myCamera = Camera.main;
		
		//the font color of the GUIstyle depends on which theam the player is on
		if(myTransform.tag == "BlueTeam")
		{
			myStyle.normal.textColor = Color.blue;
		}
		if(myTransform.tag == "RedTeam")
		{
			myStyle.normal.textColor = Color.red;
		}
		
		myStyle.fontSize = 12;
		
		myStyle.fontStyle = FontStyle.Bold;
		
		//allow the text to extend beyond
		myStyle.clipping = TextClipping.Overflow;
		
		Transform triggerTransform = transform.FindChild("Trigger");
		
		HDScript = triggerTransform.GetComponent<HealthAndDamage>();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Capture wheter the player is infront or behind the camera.
		cameraRelativePosition = myCamera.transform.InverseTransformPoint(myTransform.position);
		
		//access the players health and damage script and figure out how long the bar is.
		//access the health and damage script.
	

		//forsome reason put Health to 1 if players health drops below 1 I guess it fucks up the bar
		
		if(HDScript.myHealth < 1)
		{
			healthBarLength = 1;
		}
		if(HDScript.myHealth >= 1)
		{
			healthBarLength = (HDScript.myHealth / HDScript.maxHealth) *100;
		}
	}
	
	void OnGUI ()
	{
		//Only display the players name if they are infront of the camera and also the plaeyr should be in front of the camera by at least minimumZ
		if(cameraRelativePosition.z > minimumZ)
		{
			//set the world position to be just a bit above the player.
			worldPosition = new Vector3(myTransform.position.x, myTransform.position.y + adjustment, myTransform.position.z);
			
			//Convert world position to a screen position
			screenPosition = myCamera.WorldToScreenPoint(worldPosition);
			
			//Drawthe health bar and the grey bar behind it.
			
			GUI.Box (new Rect(screenPosition.x - healthBarLeft / 2, Screen.height - screenPosition.y - barTop,100,healthBarHeight),"");
			
			GUI.DrawTexture(new Rect(screenPosition.x - healthBarLeft /2, Screen.height - screenPosition.y - barTop, healthBarLength, healthBarHeight),healthTex);
			
			//Draw the players name above them.
			
			GUI.Label(new Rect(screenPosition.x - labelWidth / 2, Screen.height - screenPosition.y - labelTop, labelWidth, labelHeight), playerName, myStyle);
		}
	}
}
