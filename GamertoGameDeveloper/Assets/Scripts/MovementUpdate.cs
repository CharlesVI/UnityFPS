using UnityEngine;
using System.Collections;

/// <summary>
/// Movement update.
/// This Script is attached to the player and it ensures 
/// that every players position , rotation, and scale are kept up to date across  the network
/// </summary>
public class MovementUpdate : MonoBehaviour {
	
	//Varibles Start -----------------------------------------------------------------
	
	private Vector3 lastPosition;
	
	private Quaternion lastRotation;
	
	private Transform myTransform;
	
	//Varibles End -------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == false)
		{
			enabled = false;
		}
		myTransform = transform;
		
		
		//Ensure everyone sees everyone at the correct location the moment they spawn
		
		networkView.RPC ("updateMovement", RPCMode.OthersBuffered, myTransform.position, myTransform.rotation);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If the player moves at all call an RPC to update the players accross the network
		
		if(Vector3.Distance (myTransform.position, lastPosition) >= 0.1)
		{
			//capture the players position before the rpc is fired off.
			lastPosition = myTransform.position;
			
			networkView.RPC ("updateMovement", RPCMode.OthersBuffered, myTransform.position, myTransform.rotation);
		}
		if(Quaternion.Angle (myTransform.rotation, lastRotation) >= 1)
		{
			lastRotation = myTransform.rotation;
			networkView.RPC ("updateMovement", RPCMode.OthersBuffered, myTransform.position, myTransform.rotation);
		}
	}
	
	[RPC]
	void updateMovement (Vector3 newPosition, Quaternion newRotation)
	{
		transform.position = newPosition;
		
		transform.rotation = newRotation;
	}
}
