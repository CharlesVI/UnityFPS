using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the player and it
/// causes the camera to continuously follow the
/// CameraHead.
/// </summary>

public class CameraScript : MonoBehaviour {
	
	
	//Variables Start-------------------------------------------
	
	private Camera myCamera;
	
	private Transform cameraHeadTransform;
	
	//Varibles End ---------------------------------------------
	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{
		myCamera = Camera.main;
		
		cameraHeadTransform = transform.FindChild("CameraHead");
		}
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Make the camera follow the player's cameraHeadTransform
		
		myCamera.transform.position = cameraHeadTransform.position;
		
		myCamera.transform.rotation = cameraHeadTransform.rotation;
	}
}
