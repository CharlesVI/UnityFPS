using UnityEngine;
using System.Collections;

/// <summary>
/// Fire blaster.
/// This script is attached to the player and allows 
/// them to fire the blaster projectile.
/// 
/// This script accesses the spawn script.
/// </summary>

public class FireBlaster : MonoBehaviour {

	//Varibles Start -----------------------------------------
	
	//the blaster projectile is attached to this in the inspector
	public GameObject blaster;
	
	//Quick refrences.
	private Transform myTransform;
	private Transform cameraHeadTransform;
	
	//The position at which the projectile should be instantiated.
	
	private Vector3 launchPosition = new Vector3();
	
	//Used to control the rate of fire.
	private float fireRate = 0.2f;
	private float nextFire = 0;
	
	//used in determining which team the player is on
	private bool IAmOnTheBlueTeam = false;
	private bool IAmOnTheRedTeam = false;
	
	//varibles end -------------------------------------------
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{
			//my transform is whatever script's tranform is attached too.
			myTransform = transform;
			
			cameraHeadTransform = myTransform.FindChild ("CameraHead");
				
			//find the spawnmanager and access the spawn script to see what team the player is on.
			GameObject spawnManager = GameObject.Find("SpawnManager");
			
			SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
			
			if(spawnScript.amIOnTheRedTeam == true)
			{
				IAmOnTheRedTeam = true;
			}
			if(spawnScript.amIOnTheBlueTeam == true)
			{
				IAmOnTheBlueTeam = true;
			}
		}
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButton("Fire Weapon") && Time.time > nextFire && Screen.lockCursor == true)
		{
			//Gametime + CoolDown = next possible shot time.
			nextFire = Time.time + fireRate;
			
			//The launch position of the projectile will be just infront of the CameraHead
			launchPosition = cameraHeadTransform.TransformPoint(0,0,0.2f);
			
			//create the blaster projectile accross the network at the launch position and tilt it so that it is horizontal 
			// using the angle eulerangles .x + 90
			//make it team specific too.
			if(IAmOnTheRedTeam == true)
			{
				networkView.RPC ("SpawnProjectile", RPCMode.All, launchPosition,Quaternion.Euler (cameraHeadTransform.eulerAngles.x +90,myTransform.eulerAngles.y,0),
					myTransform.name, "Red");
				//Instantiate(blaster, launchPosition,Quaternion.Euler (cameraHeadTransform.eulerAngles.x +90,myTransform.eulerAngles.y,0));
			}
			if(IAmOnTheBlueTeam == true)
			{
				networkView.RPC ("SpawnProjectile", RPCMode.All, launchPosition,Quaternion.Euler (cameraHeadTransform.eulerAngles.x +90,myTransform.eulerAngles.y,0),
					myTransform.name, "Blue");
				//Instantiate(blaster, launchPosition,Quaternion.Euler (cameraHeadTransform.eulerAngles.x +90,myTransform.eulerAngles.y,0));
			}
		}
	}
	
	[RPC]
	void SpawnProjectile (Vector3 position, Quaternion rotation, string originatorName, string team)
	{
		//Access the blasterscript off the new projectile and supply the players name and team.
		GameObject go = Instantiate (blaster, position, rotation) as GameObject;
		
		BlasterScript bScript = go.GetComponent<BlasterScript>();
		
		bScript.myOriginator = originatorName;
		bScript.team = team;
	}
}
