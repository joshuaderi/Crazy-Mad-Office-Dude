public class Enemy_Drone : Enemy
{
    public override EnemyType Type
    {
        get { return EnemyType.Drone; }

    }

    //Event called when damaged by an attack
    public void Damage(int Damage = 0)
    {
        //Reduce health
        Health -= Damage;

        ////Play damage animation
        PingPongSpriteColor.PlayColorAnimation();

        //Check if dead
        if (Health <= 0)
        {
            //Send enemy destroyed notification
            GameManager.Notifications.PostNotification(this, "EnemyDestroyed");

            //Play collection sound, if audio source is available
            if (SFX)
            {
                SFX.PlayOneShot(DestroyAudio, 1.0f);
            }

            //Remove object from scene
            DestroyImmediate(gameObject);

            //Clean up old listeners
            GameManager.Notifications.RemoveRedundancies();
        }
    }
}