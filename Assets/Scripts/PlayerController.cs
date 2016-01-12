using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	//Amount of cash player should collect to complete level
	public float CashTotal = 1400.0f;
	
	//Amount of cash for this player
	private float cash = 0.0f;

	//Reference to transform
	private Transform thisTransform;

	//Respawn time in seconds after dying
	public float RespawnTime = 2.0f;

	//Player health
    [SerializeField]
    private int health = 100;

	//Get Mecanim animator component in children
	private Animator animComp;

	//Private damage texture
	private Texture2D damageTexture;

	//Screen coordinates
	private Rect screenRect;
	
	//Show damage texture?
	private bool showDamage = false;

	//Damage texture interval (amount of time in seconds to show texture)
	private float DamageInterval = 0.2f;
	 
	//Called when object is created
	void Start()
	{
		//Get First person capsule and make non-visible
		MeshRenderer capsule = GetComponentInChildren<MeshRenderer>();
		capsule.enabled = false;

		//Get Animator
		animComp = GetComponentInChildren<Animator>();

		//Create damage texture
		damageTexture = new Texture2D(1,1);
		damageTexture.SetPixel(0,0,new Color(255,0,0,0.5f));
		damageTexture.Apply();

		//Get cached transform
		thisTransform = transform;
	}
	 
	//Accessors to set and get cash
	public float Cash
	{
		//Return cash value
		get{return cash;}
		
		//Set cash and validate, if required
		set
		{
			//Set cash
			cash = value;
			
			//Check collection limit - post notification if limit reached
			if(cash >= CashTotal) GameManager.Notifications.PostNotification(this, "CashCollected");
		}
	}
	 
	//Accessors to set and get health
	public int Health
	{
		//Return health value
		get{return health;}
		
		//Set health and validate, if required
		set
		{
			health = value;
			
			//Playe Die functionality
			if(health <= 0) gameObject.SendMessage("Die",SendMessageOptions.DontRequireReceiver);
		}
	}
	 
	//Function to apply damage to the player
	public IEnumerator ApplyDamage(int Amount = 0)	
	{
		//Reduce health
		Health -= Amount;
		
		//Post damage notification
		GameManager.Notifications.PostNotification(this, "PlayerDamaged");
		
		//Show damage texture
		showDamage = true;

		//Wait for interval
		yield return new WaitForSeconds(DamageInterval);
		
		//Hide damage texture
		showDamage = false;
	}
	 
	//ON GUI Function to show texture
	void OnGUI()
	{
	    if (showDamage) GUI.DrawTexture(screenRect, damageTexture);
	}

	//Function called when player dies
	public IEnumerator Die()
	{
		//Disable input
		GameManager.Instance.InputAllowed = false;
		
		//Trigger death animation if available
		if(animComp) animComp.SetTrigger("ShowDeath");
		
		//Wait for respawn time
		yield return new WaitForSeconds(RespawnTime);
		
		//Restart level
		Application.LoadLevel(Application.loadedLevel);
	}

	void Update()
	{
		//Build screen rect on update (in case screen size changes)
		screenRect.x = screenRect.y = 0;
		screenRect.width = Screen.width;
		screenRect.height = Screen.height;
	}
}