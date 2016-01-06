using UnityEngine;
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
        //If no key in dictionary exists, then exit
        if (!listeners.ContainsKey(notificationName)) return;

        //Else post notification to all matching listeners
        foreach (Component listener in listeners[notificationName])
        {
            listener.SendMessage(notificationName, sender, SendMessageOptions.DontRequireReceiver);
        }
    }

    //Function to remove a listener for a notification
    public void RemoveListener(Component listener, string notificationName)
    {
        //If no key in dictionary exists, then exit
        if (!listeners.ContainsKey(notificationName)) return;

        //Cycle through listeners and identify component, and then remove
        for (int i = listeners[notificationName].Count - 1; i >= 0; i--)
        {
            //check instance ID
            if (listeners[notificationName][i].GetInstanceID() == listener.GetInstanceID())
            {
                listeners[notificationName].RemoveAt(i); //Matched. Remove from list
            }
        }
    }

    //function to remove redundant listeners - deleted and removed listeners
    public void RemoveRedundancies()
    {
        //Create new dictionary
        var tmpListeners = new Dictionary<string, List<Component>>();

        //Cycle through all dictionary entries
        foreach (var eventListenersPair in listeners)
        {
            //Cycle through all listener objects in list, remove null objects
            for (int i = eventListenersPair.Value.Count; i >=0; i--)
            {
                //if null, then remove item
                if (eventListenersPair.Value[i] == null) eventListenersPair.Value.RemoveAt(i);
            }

            //if items remain in list for this notification, then add this to tmp dictionary 
            if (eventListenersPair.Value.Count > 0) tmpListeners.Add(eventListenersPair.Key, eventListenersPair.Value); 
        }

        //replace listeners object with new, optimized dictionary
        listeners = tmpListeners;
    }
}
