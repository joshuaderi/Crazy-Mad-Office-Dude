using UnityEngine;
using System.Collections;

public class Weapon_Gun : Weapon
{
	//Default Sprite to show for weapon when active and not attacking

    //Sound to play on attack
	public AudioClip WeaponAudio = null;
	
	//Audio Source for sound playback
	private AudioSource sfx;

	//Reference to all child sprite renderers for this weapon
	private SpriteRenderer[] weaponSprites;

	// Use this for initialization
	void Start ()
	{
		//Find sound object in scene
		GameObject soundsObject = GameObject.FindGameObjectWithTag("sounds");
		
		//If no sound object, then exit
		if(soundsObject == null) return;
		
		//Get audio source component for sfx
		sfx = soundsObject.GetComponent<AudioSource>();

		//Get all child sprite renderers for weapon
		weaponSprites = gameObject.GetComponentsInChildren<SpriteRenderer>();

		//Register weapon for weapon change events
		GameManager.Notifications.AddListener(this, "WeaponChange");
	}
	
    // Update is called once per frame
	void Update ()
	{
		//If not equipped then exit
		if(!IsEquipped) return;

		//If cannot accept input, then exit
		if(!GameManager.Instance.InputAllowed) return;

		//Check for fire button input
		if(Input.GetButton("Fire1") && CanFire)
			StartCoroutine(Fire());
	}
	
    //Coroutine to fire weapon
	public IEnumerator Fire()
	{
		//If can fire
		if(!CanFire || !IsEquipped || Ammo <= 0) yield break;

		//Set refire to false
		CanFire = false;

		//Play Fire Animation
		gameObject.SendMessage("PlaySpriteAnimation", 0, SendMessageOptions.DontRequireReceiver);

		//Play collection sound, if audio source is available
		if(sfx){sfx.PlayOneShot(WeaponAudio, 1.0f);}

		//Calculate hit

		//Get ray from screen center target
		Ray R = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2,0));
		
		//Test for ray collision
		RaycastHit hit;
		
		if(Physics.Raycast(R.origin, R.direction, out hit, Range))
		{
			//Target hit - check if target is enemy
			if(hit.collider.gameObject.CompareTag("enemy"))
			{
				//Send damage message (deal damage to enemy)
				hit.collider.gameObject.SendMessage("Damage",Damage, SendMessageOptions.DontRequireReceiver);
			}
		}

		//Reduce ammo
		--Ammo;

		//Check remaining ammo - post empty notification
		if(Ammo <= 0) GameManager.Notifications.PostNotification(this, "AmmoExpired");

		//Wait for recovery before re-enabling CanFire
		yield return new WaitForSeconds(RecoveryDelay);

		//Re-enable CanFire
		CanFire = true;
	}
	
    //Called when animation has completed playback
	public void SpriteAnimationStopped()
	{
		//If not equipped then exit
		if(!IsEquipped) return;
		
		//Show default sprite
		DefaultSprite.enabled = true;
	}
	
	//Weapon change event - called when player changes weapon
	public void WeaponChange(Component Sender)
	{
		//Has player changed to this weapon?
		if(Sender.GetInstanceID() == GetInstanceID()) return;

		//Has changed to other weapon. Hide this weapon
		StopAllCoroutines();
		gameObject.SendMessage("StopSpriteAnimation", 0, SendMessageOptions.DontRequireReceiver);

		//Deactivate equipped
		IsEquipped = false;

		foreach(SpriteRenderer SR in weaponSprites)
			SR.enabled = false;
	}
}
