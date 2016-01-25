using UnityEngine;
using System.Collections;

public class WeaponPunch : Weapon
{
    //Sound to play on attack
    public AudioClip WeaponAudio = null;
    
    //Coroutine to fire weapon
    public override IEnumerator Fire()
    {
        //If can fire
        if (!CanFire || !IsEquipped) yield break;

        //Set refire to false
        CanFire = false;

        //Play Fire Animation
        StartCoroutine(SpriteAnimator.PlaySpriteAnimation());

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
}
