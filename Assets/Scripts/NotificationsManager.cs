using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationsManager : MonoBehaviour
{

    private Dictionary<string, List<Component>> listeners = new Dictionary<string, List<Component>>();
    //Function to add a listener for an notification to the listeners list
    public void AddListener(Component listener, string notificationName)
    {
        //Add listener to dictionary
        if (!listeners.ContainsKey(notificationName))
        {
            listeners.Add(notificationName, new List<Component>());
        }

        //Add object to listener list for this notification
        listeners[notificationName].Add(listener);
    }

    //Function to post a notification to a listener
    public void PostNotification(Component sender, string notificationName)
    {
        //If not key in dictionary exists, then exit
        if (!listeners.ContainsKey(notificationName)) return;

        //Else post notification to all matching listeners
        foreach (Component listener in listeners[notificationName])
        {
            listener.SendMessage(notificationName, sender, SendMessageOptions.DontRequireReceiver);
        }
    }
}
