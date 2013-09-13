using UnityEngine;
using System.Collections;
/// <summary>
/// Stat display.
/// This script is attached to the player and allows the player to see a box with their current health.
/// to the lower right of the corsshair.
/// 
/// This script access the health and damagage script.
/// </summary>

public class StatDisplay : MonoBehaviour 
{
	//varibles Start -------------------------------------------------
	
	//the healthbar texture is attached to this in the inspector.
	public Texture healthTex;
	
	//These are used in calculating and displaying the health.
	private float health;
	
	private int healthForDisplay;
	
	//These are used in defined stat display box.
	private int boxWidth = 160; 
	private int boxHeight = 85;
	
	private int labelHeight =20;
	private int labelWidth = 35;
	
	private int padding = 10;
	private int gap = 120;
	
	private float healthBarLength;
	private int healthBarHeight = 15;
	private GUIStyle healthStyle = new GUIStyle();
	
	private float commonLeft;
	
	private float commonTop;
	
	//A quick refrence to the Health and Damage Script.
	private HealthAndDamage HDScript;
	
	//varibles End ---------------------------------------------------
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == false)
		{
			enabled = false;
		}
		
		//access the health and damage script.
		Transform triggerTransform = transform.FindChild("Trigger");
		
		HDScript = triggerTransform.GetComponent<HealthAndDamage>();
		
		//Set the GUI Style
		healthStyle.normal.textColor = Color.green;
		
		healthStyle.fontStyle = FontStyle.Bold;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Access the health and damage script. continuously and retrive the player's current health.
		health = HDScript.myHealth;
		
		//I also want to display health as whole numbers.
		healthForDisplay = Mathf.CeilToInt(health);
		
		//Calculate how long the health bar should be. The max length is 100.
		
		healthBarLength = (health / HDScript.maxHealth) * 100;
	}
	
	void OnGUI()
	{
			//Note draw order is applied here. First in First drawn.
		commonLeft = Screen.width /2 + 180;
		commonTop = Screen.height /2 + 50;
		
		//Draw a plain box behind the health bar. 
		GUI.Box (new Rect(commonLeft, commonTop, boxWidth, boxHeight), "");
		
		//Draw a grey box behind the healthbar
		GUI.Box(new Rect(commonLeft + padding, commonTop + padding, 100, healthBarHeight), "");
		
		
		//Draw the health bar texture and make its lenghth depend on the players current health.
		GUI.DrawTexture(new Rect(commonLeft + padding, commonTop + padding, healthBarLength, healthBarHeight), healthTex);
		
		GUI.Label(new Rect(commonLeft + gap, commonTop + padding, labelWidth, labelHeight), healthForDisplay.ToString (), healthStyle);
	}
}
