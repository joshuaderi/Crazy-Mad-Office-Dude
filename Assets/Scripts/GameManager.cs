//GameManager
//Singleton and persistent object to manage game state
//For high level control over game

using UnityEngine;

//Game Manager requires other manager components
[RequireComponent(typeof(NotificationsManager))] //Component for sending and receiving notifications

public class GameManager : MonoBehaviour
{
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
}
