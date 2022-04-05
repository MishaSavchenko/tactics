using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class FieldEventManager : MonoBehaviour
{
    private Dictionary <string, UnityEvent> eventDictionary;

    private static FieldEventManager field_event_manager;

    public static FieldEventManager field_event_maanger_instance
    {
        get
        {
            if (!field_event_manager)
            {
                field_event_manager = FindObjectOfType (typeof (FieldEventManager)) as FieldEventManager;

                if (!field_event_manager)
                {
                    Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    field_event_manager.Init (); 
                }
            }

            return field_event_manager;
        }
    }

    void Init ()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening (string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (field_event_maanger_instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.AddListener (listener);
        } 
        else
        {
            thisEvent = new UnityEvent ();
            thisEvent.AddListener (listener);
            field_event_maanger_instance.eventDictionary.Add (eventName, thisEvent);
        }
    }

    public static void StopListening (string eventName, UnityAction listener)
    {
        if (field_event_manager == null) return;
        UnityEvent thisEvent = null;
        if (field_event_maanger_instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.RemoveListener (listener);
        }
    }

    public static void TriggerEvent (string eventName)
    {
        UnityEvent thisEvent = null;
        if (field_event_maanger_instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.Invoke ();
        }
    }
}