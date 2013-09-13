using UnityEngine;
using System.Collections;
/// <summary>
/// Assign health.
/// This script is attached to the GameManager
/// 
/// This script accesses the spawn script to see if first spawn is true.
/// </summary>
/// 
public class AssignHealth : MonoBehaviour {
	
	//Varibles Start
	
	private GameObject[] redTeamPlayers;
	private GameObject[] blueTeamPlayers;
	
	private float waitTime = 5;
	
	//Varibles End 
	
	// Use this for initializatio
	
	
	void OnConnectedToServer()
	{
		StartCoroutine(AssignHealthOnJoiningGame());
	}
	
	IEnumerator AssignHealthOnJoiningGame()
	{
	
		//Dont excecute the code untill wait time has elapsed.
		yield return new WaitForSeconds(waitTime);
	
		//Find the trigger game object of all players in both teams and place a refrence to them in both arrays.
		//ME this just fills them UP?
		redTeamPlayers = GameObject.FindGameObjectsWithTag("RedTeamTrigger");
		
		blueTeamPlayers = GameObject.FindGameObjectsWithTag("BlueTeamTrigger");
		
		//Assign the buffered previous health value to the plaeyrs current health.
		//So you dont spawn seeing them all with max HP. (Why not just have them report HP on join?)
		
		foreach(GameObject red in  redTeamPlayers)
		{
			HealthAndDamage HDScript = red.GetComponent<HealthAndDamage>();

			HDScript.myHealth = HDScript.previousHealth;
		}
		
		foreach(GameObject blue in blueTeamPlayers)
		{
			HealthAndDamage HDScript  = blue.GetComponent<HealthAndDamage>();	
			
			HDScript.myHealth = HDScript.previousHealth;
		}
	
		
		//Disable this script just incase.
		enabled = false;
		
		
	}
}
