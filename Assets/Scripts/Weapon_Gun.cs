using UnityEngine;
using System.Collections;

public class Weapon_Gun : Weapon
{
    //Default Sprite to show for weapon when active and not attacking

    //Sound to play on attack
    public AudioClip WeaponAudio = null;
    
    //Coroutine to fire weapon
    public override IEnumerator Fire()
    {
        //If can fire
        if (!CanFire || !IsEquipped || Ammo <= 0) yield break;

        //Set refire to false
        CanFire = false;

        StartCoroutine(SpriteAnimator.PlaySpriteAnimation());

        //Play collection sound, if audio source is available
        if (sfx) { sfx.PlayOneShot(WeaponAudio, 1.0f); }

        //Calculate hit

        //Get ray from screen center target
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));

        //Test for ray collision
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Range))
        {
            //Target hit - check if target is enemy
            if (hit.collider.gameObject.CompareTag("enemy"))
            {
                //Send damage message (deal damage to enemy)
                hit.collider.gameObject.SendMessage("Damage", Damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        //Reduce ammo
        --Ammo;

        //Check remaining ammo - post empty notification
        if (Ammo <= 0) GameManager.Notifications.PostNotification(this, "AmmoExpired");

        //Wait for recovery before re-enabling CanFire
        yield return new WaitForSeconds(RecoveryDelay);

        //Re-enable CanFire
        CanFire = true;
    }
}
