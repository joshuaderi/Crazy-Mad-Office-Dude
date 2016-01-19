using UnityEngine;

public class Weapon : MonoBehaviour
{
	//Custom enum for weapon types
	public enum WeaponType {Punch=0, Gun=1};

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
}
