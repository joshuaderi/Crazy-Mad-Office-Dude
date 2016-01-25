//GameManager
//Singleton and persistent object to manage game state
//For high level control over game

using UnityEngine;

//Game Manager requires other manager components
[RequireComponent(typeof(NotificationsManager))] //Component for sending and receiving notifications
[RequireComponent(typeof(LoadSaveManager))]

public class GameManager : MonoBehaviour
{
    //C# property to retrieve and set input allowed status
    public bool InputAllowed
    {
        get { return bInputAllowed; }

        set
        {
            //Set Input
            bInputAllowed = value;

            //Post notification about input status changed
            Notifications.PostNotification(this, "InputChanged");
        }
    }
    //Can game accept user input?
    private bool bInputAllowed = true;

    //C# property to retrieve currently active instance of object, if any
    public static GameManager Instance
    {
        //create game manager object if required
        get { return instance ?? (instance = new GameObject("GameManager").AddComponent<GameManager>()); }
    }
    //Internal reference to single active instance of object - for singleton behaviour
    private static GameManager instance;

    //C# property to retrieve notifications manager
    public static NotificationsManager Notifications
    {
        get { return notifications ?? (notifications = instance.GetComponent<NotificationsManager>()); }
    }
    //Internal reference to notifications object
    private static NotificationsManager notifications;

    //C# property to retrieve save/load manager
    public static LoadSaveManager StateManager
    {
        get { return stateManager ?? (stateManager = instance.GetComponent<LoadSaveManager>()); }
    }
    //Internal reference to Saveload Game Manager
    private static LoadSaveManager stateManager;

    //Should load from save game state on level load, or just restart level from defaults
    private static bool shouldLoad = false;

    //Called before Start on object creation
    void Awake()
    {
        //Check if there is an existing instance of this object
        if ((instance == null) || (instance.GetInstanceID() == GetInstanceID()))
        {
            instance = this; //Make this object the only instance
            DontDestroyOnLoad(gameObject); //Set as do not destroy
        }
        else DestroyImmediate(gameObject); //Delete duplicate
    }

    // Use this for initialization
    void Start()
    {
        //Add cash collected listener to listen for win condition
        Notifications.AddListener(this, "CashCollected");

        //Add listeners for main menu
        Notifications.AddListener(this, "RestartGame");
        Notifications.AddListener(this, "ExitGame");
        Notifications.AddListener(this, "SaveGame");
        Notifications.AddListener(this, "LoadGame");

        //If we need to load level
        if (shouldLoad)
        {
            StateManager.Load(Application.persistentDataPath + "/SaveGame.xml");
            shouldLoad = false; //Reset load flag
        }
    }

    //Function called when all cash is collected in level
    public void CashCollected(Component sender)
    {
    }

    //Restart Game
    public void RestartGame()
    {
        //Load first level
        Application.LoadLevel(0);
    }

    //Exit Game
    public void ExitGame()
    {
        Application.Quit();
    }

    //Save Game
    public void SaveGame()
    {
        //Call save game functionality
        StateManager.Save(Application.persistentDataPath + "/SaveGame.xml");
    }
    
    //Load Game
    public void LoadGame()
    {
        //Set load on restart
        shouldLoad = true;

        //Restart Level
        RestartGame();
    }
}
