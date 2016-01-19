using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    //Custom enum for weapon types
    public enum WeaponType { Punch = 0, Gun = 1 };

    //Weapon type
    public WeaponType Type = WeaponType.Punch;

    //Damage this weapon causes
    public float Damage = 0.0f;

    //Range of weapon (linear distance outwards from camera) measured in world units
    public float Range = 1.0f;

    //Amount of ammo remaining (-1 = infinite)
    public int Ammo = -1;

    //Recovery delay
    //Amount of time in seconds before weapon can be used again
    public float RecoveryDelay = 0.0f;

    //Has this weapon been collected?
    public bool Collected = false;

    //Is this weapon currently equipped on player
    public bool IsEquipped = false;

    //Can this weapon be fired
    public bool CanFire = true;

    //Next weapon in cycle
    public Weapon NextWeapon = null;
    public SpriteRenderer DefaultSprite = null;

    //Audio Source for sound playback
    protected AudioSource sfx;

    //Reference to all child sprite renderers for this weapon
    protected SpriteRenderer[] WeaponSprites;

    protected SpriteAnimator SpriteAnimator;

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsEquipped || !GameManager.Instance.InputAllowed) return;

        //Check for fire button input
        if (Input.GetButton("Fire1") && CanFire) StartCoroutine(Fire());
    }

    public abstract IEnumerator Fire();

    protected bool CanEquipWeapon(WeaponType weaponType)
    {
        return (weaponType == Type) && Collected && (Ammo != 0) && !IsEquipped;
    }

    //Equip weapon
    public bool Equip(WeaponType weaponType)
    {
        if (!CanEquipWeapon(weaponType)) return false;

        IsEquipped = true;

        //Show default sprite
        DefaultSprite.enabled = true;

        //Activate Can Fire
        CanFire = true;

        //Send weapon change event
        GameManager.Notifications.PostNotification(this, "WeaponChange");

        //Weapon was equipped
        return true;
    }

    protected void Init()
    {
        SpriteAnimator = GetComponent<SpriteAnimator>();

        //Find sound object in scene
        GameObject soundsObject = GameObject.FindGameObjectWithTag("sounds");

        //If no sound object, then exit
        if (soundsObject == null) return;

        //Get audio source component for sfx
        sfx = soundsObject.GetComponent<AudioSource>();
        
        //Register weapon for weapon change events
        GameManager.Notifications.AddListener(this, "WeaponChange");
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
        SpriteAnimator.StopSpriteAnimation();
        //Deactivate equipped
        IsEquipped = false;

        SpriteAnimator.HideAllSprites();
    }
}
