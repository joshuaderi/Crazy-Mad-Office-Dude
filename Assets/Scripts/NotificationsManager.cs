using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationsManager : MonoBehaviour {

	private Dictionary<string, List<Component>> Listeners = new Dictionary<string, List<Component>>();
    //Function to add a listener for an notification to the listeners list
    public void AddListener(Component Listener, string NotificationName)
    {
        //Add listener to dictionary
        if (!Listeners.ContainsKey(NotificationName))
        {
            Listeners.Add(NotificationName, new List<Component>());
        }

        //Add object to listener list for this notification
        Listeners[NotificationName].Add(Listener);
    }
}
