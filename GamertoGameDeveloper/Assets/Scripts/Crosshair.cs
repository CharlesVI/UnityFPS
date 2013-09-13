using UnityEngine;
using System.Collections;

/// <summary>
/// Crosshair.
/// This script is attached to the player and draws the crosshair in the center of the screen. 
/// </summary>

public class Crosshair : MonoBehaviour {
	
	//Varibles Start ----------------------------------------
	
	public Texture crosshairTex;
	
	private float crosshairDimension = 256;
	
	private float halfCrosshairWidth = 128;
	
	//Varibles End ------------------------------------------
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == false)
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		//display the crosshair in the center of the screen while the cursor is locked.
		
		if(Screen.lockCursor == true)
		{
			GUI.DrawTexture(new Rect(Screen.width / 2 - halfCrosshairWidth, Screen.height / 2 - halfCrosshairWidth, crosshairDimension, crosshairDimension), crosshairTex);
		}
	}
}
