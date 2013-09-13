using UnityEngine;
using System.Collections;

/// <summary>
/// Blaster script.
/// This script is attached to the Blaster projectile and it
/// governs the behavior of the projectile.
/// This is acessed by the fire blaster script.
/// </summary>
public class BlasterScript : MonoBehaviour {
	
	//varibles Start--------------------------------------
	
	//The explosion effect is attached to this in the inspector
	public GameObject blasterExplosion;
	
	// a quick refrence.
	private Transform myTransform;
	
	
	// the projectiles flight speed.	
	private float projectileSpeed = 10;
	
	//prevent the projectile from causing further harm once it has hit something.
	private bool expended = false;
	
	//a ray projected in front of the projectile to see if it will hit a recognisable collider
	private RaycastHit hit;
	
	//The range of that ray.
	private float range = 1.5f;
	
	//The life span of the projectile
	private float expireTime = 5;
	
	//Used in hit detection
	public string team;
	public string myOriginator;
	

	
	//varibles end ----------------------------------------
	
	
	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		
		// As soon as the projectile is created start a countdown to destroy it
		StartCoroutine (DestroyMyselfAfterSomeTime());
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		//Translate the projectile in the up direction (the pointed end).
		myTransform.Translate(Vector3.up * projectileSpeed * Time.deltaTime);	
		
		//If the ray hits something then excecute this code.
		if(Physics.Raycast(myTransform.position, myTransform.up, out hit, range)&&
			expended == false)
		{
			// if the colider has the tag of floor then..
			if(hit.transform.tag == "Floor")
			{
				expended = true;
			
				//Instatiate an explosion effect.
				
				Instantiate(blasterExplosion, hit.point, Quaternion.identity);
				
				//Make the projectile become invisible
				myTransform.renderer.enabled = false;
				
				//Turn off the light. (why not just delete the object?)
				myTransform.light.enabled = false;
			}
			
			if(hit.transform.tag == "BlueTeamTrigger" || hit.transform.tag == "RedTeamTrigger")
			{
				expended = true;
			
				//Instatiate an explosion effect.
				
				Instantiate(blasterExplosion, hit.point, Quaternion.identity);
				
				//Make the projectile become invisible
				myTransform.renderer.enabled = false;
				
				//Turn off the light. (why not just delete the object?)
				myTransform.light.enabled = false;
				
				//Access the health and damage script of the enemy player and inform them that they have been attacked and by whom.
				
				if(hit.transform.tag == "BlueTeamTrigger" && team == "Red")
				{
					HealthAndDamage HDscript = hit.transform.GetComponent<HealthAndDamage>();
					
					HDscript.iWasJustAttacked = true;
					HDscript.myAttacker = myOriginator;
					HDscript.hitByBlaster = true; 
				}
				if(hit.transform.tag == "RedTeamTrigger" && team == "Blue")
				{
					HealthAndDamage HDscript = hit.transform.GetComponent<HealthAndDamage>();
					
					HDscript.iWasJustAttacked = true;
					HDscript.myAttacker = myOriginator;
					HDscript.hitByBlaster = true; 
				}
			}
		}
	}
	
	IEnumerator DestroyMyselfAfterSomeTime()
	{
		//Wait for the timer to count up to the expireTime and then destroy the projectile.
		
		yield return new WaitForSeconds(expireTime);
		
		Destroy(myTransform.gameObject);
	}
}
