using UnityEngine;
using System.Collections;

public class Weapon_Punch : Weapon
{
    //Sound to play on attack
    public AudioClip WeaponAudio = null;

    //Audio Source for sound playback
    private AudioSource sfx;

    //Reference to all child sprite renderers for this weapon
    private SpriteRenderer[] weaponSprites;

    void Start()
    {
        //Find sound object in scene
        GameObject soundsObject = GameObject.FindGameObjectWithTag("sounds");

        //If no sound object, then exit
        if (soundsObject == null) return;

        //Get audio source component for sfx
        sfx = soundsObject.GetComponent<AudioSource>();

        //Get all child sprite renderers for weapon
        weaponSprites = gameObject.GetComponentsInChildren<SpriteRenderer>();

        //Register weapon for weapon change events
        GameManager.Notifications.AddListener(this, "WeaponChange");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsEquipped || !GameManager.Instance.InputAllowed) return;

        //Check for fire button input
        if (Input.GetButton("Fire1") && CanFire) StartCoroutine(Fire());
    }

    //Coroutine to fire weapon
    public IEnumerator Fire()
    {
        //If can fire
        if (!CanFire || !IsEquipped) yield break;

        //Set refire to false
        CanFire = false;

        //Play Fire Animation
        gameObject.SendMessage("PlaySpriteAnimation", 0, SendMessageOptions.DontRequireReceiver);

        Vector3 position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0);

        //Get ray from screen center target
        Ray ray = Camera.main.ScreenPointToRay(position);

        //Test for ray collision
        RaycastHit hit;

        //Calculate hit
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Range))
        {
            GameObject colliderGO = hit.collider.gameObject;

            //Target hit - check if target is enemy
            if (colliderGO.CompareTag("enemy"))
            {
                //Play collection sound, if audio source is available
                if (sfx) sfx.PlayOneShot(WeaponAudio, 1.0f);

                //Send damage message (deal damage to enemy)
                colliderGO.SendMessage("Damage", Damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        //Wait for recovery before re-enabling CanFire
        yield return new WaitForSeconds(RecoveryDelay);

        //Re-enable CanFire
        CanFire = true;
    }
    
    //Called when animation has completed playback
    public void SpriteAnimationStopped()
    {
        //If not equipped then exit
        if (!IsEquipped) return;

        //Show default sprite
        DefaultSprite.enabled = true;
    }
    
    //Weapon change event - called when player changes weapon
    public void WeaponChange(Component sender)
    {
        //Has player changed to this weapon?
        if (sender.GetInstanceID() == GetInstanceID()) return;

        //Has changed to other weapon. Hide this weapon
        StopAllCoroutines();
        gameObject.SendMessage("StopSpriteAnimation", 0, SendMessageOptions.DontRequireReceiver);

        //Deactivate equipped
        IsEquipped = false;

        foreach (SpriteRenderer SR in weaponSprites) SR.enabled = false;
    }
}
