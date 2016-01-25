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
    private float damageInterval = 0.2f;

    //Default player weapon (Punch)
    public Weapon DefaultWeapon;

    //Currently active weapon
    public Weapon ActiveWeapon;
    public Weapon CollectWeapon;

    //Called when object is created

    void Start()
    {
        //Activate default weapon
        DefaultWeapon.gameObject.SendMessage("Equip", DefaultWeapon.Type);

        //Set active weapon
        ActiveWeapon = DefaultWeapon;

        //Register controller for weapon expiration events
        GameManager.Notifications.AddListener(this, "AmmoExpired");

        //Register controller for input change events
        GameManager.Notifications.AddListener(this, "InputChanged");

        //Add listeners for saving games
        GameManager.Notifications.AddListener(this, "SaveGamePrepare");
        GameManager.Notifications.AddListener(this, "LoadGameComplete");

        //Get First person capsule and make non-visible
        MeshRenderer capsule = GetComponentInChildren<MeshRenderer>();
        capsule.enabled = false;

        //Get Animator
        animComp = GetComponentInChildren<Animator>();

        //Create damage texture
        damageTexture = new Texture2D(1, 1);
        damageTexture.SetPixel(0, 0, new Color(255, 0, 0, 0.5f));
        damageTexture.Apply();

        //Get cached transform
        thisTransform = transform;
    }

    //Accessors to set and get cash

    public float Cash
    {
        //Return cash value
        get { return cash; }

        //Set cash and validate, if required
        set
        {
            //Set cash
            cash = value;

            //Check collection limit - post notification if limit reached
            if (cash >= CashTotal) GameManager.Notifications.PostNotification(this, "CashCollected");
        }
    }

    //Accessors to set and get health

    public int Health
    {
        //Return health value
        get { return health; }

        //Set health and validate, if required
        set
        {
            health = value;

            //Playe Die functionality
            if (health <= 0) gameObject.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
        }
    }

    //Function called when saving game
    public void SaveGamePrepare(Component Sender)
    {
        //Get Player Data Object
        LoadSaveManager.GameStateData.DataPlayer PlayerData = GameManager.StateManager.GameState.Player;
        //Fill in player data for save game
        PlayerData.CollectedCash = Cash;
        PlayerData.CollectedGun = CollectWeapon.Collected;
        PlayerData.Health = Health;
        PlayerData.PosRotScale.X = thisTransform.position.x;
        PlayerData.PosRotScale.Y = thisTransform.position.y;
        PlayerData.PosRotScale.Z = thisTransform.position.z;
        PlayerData.PosRotScale.RotX = thisTransform.localEulerAngles.x;
        PlayerData.PosRotScale.RotY = thisTransform.localEulerAngles.y;
        PlayerData.PosRotScale.RotZ = thisTransform.localEulerAngles.z;
        PlayerData.PosRotScale.ScaleX = thisTransform.localScale.x;
        PlayerData.PosRotScale.ScaleY = thisTransform.localScale.y;
        PlayerData.PosRotScale.ScaleZ = thisTransform.localScale.z;
    }

    //Function called when loading is complete
    public void LoadGameComplete(Component Sender)
    {
        //Get Player Data Object
        LoadSaveManager.GameStateData.DataPlayer PlayerData = GameManager.StateManager.GameState.Player;

        //Load data back to Player
        Cash = PlayerData.CollectedCash;

        //Give player weapon, activate and destroy weapon power-up
        if (PlayerData.CollectedGun)
        {
            //Find weapon powerup in level
            GameObject WeaponPowerUp = GameObject.Find("spr_upgrade_weapon");

            //Send OnTriggerEnter message
            WeaponPowerUp.SendMessage("OnTriggerEnter", GetComponent<Collider>(), SendMessageOptions.DontRequireReceiver);
        }

        Health = PlayerData.Health;

        //Set position
        thisTransform.position = new Vector3(PlayerData.PosRotScale.X, PlayerData.PosRotScale.Y, PlayerData.PosRotScale.Z);

        //Set rotation
        thisTransform.localRotation = Quaternion.Euler(PlayerData.PosRotScale.RotX, PlayerData.PosRotScale.RotY, PlayerData.PosRotScale.RotZ);

        //Set scale
        thisTransform.localScale = new Vector3(PlayerData.PosRotScale.ScaleX, PlayerData.PosRotScale.ScaleY, PlayerData.PosRotScale.ScaleZ);
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
        yield return new WaitForSeconds(damageInterval);

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
        //TODO: player can move after he dies

        //Disable input
        GameManager.Instance.InputAllowed = false;

        //Trigger death animation if available
        if (animComp) animComp.SetTrigger("ShowDeath");

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

        if (Input.GetKeyDown(KeyCode.Period)) EquipNextWeapon();
    }

    //Equip next available weapon
    public void EquipNextWeapon()
    {
        //No weapon found yet
        bool bFoundWeapon = false;

        //Loop until weapon found
        while (!bFoundWeapon)
        {
            //Get next weapon
            ActiveWeapon = ActiveWeapon.NextWeapon;

            //Activate weapon, if possible
            ActiveWeapon.gameObject.SendMessage("Equip", ActiveWeapon.Type);

            //Is successfully equipped?
            bFoundWeapon = ActiveWeapon.IsEquipped;
        }
    }

    //Event called when ammo expires
    public void AmmoExpired(Component sender)
    {
        //Ammo expired for this weapon. Equip next
        EquipNextWeapon();
    }
}