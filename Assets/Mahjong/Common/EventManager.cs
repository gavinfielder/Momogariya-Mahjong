using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

//Adapted from unity3d.com/learn/tutorials/topics/scripting/events-creating-simple-messaging-system

namespace Mahjong
{

    public class EventManager : MonoBehaviour
    {

        private Dictionary<string, UnityEvent> events;

        private static EventManager eventManager;

        private static EventManager Instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                    if (!eventManager)
                    {
                        Debug.LogError("Event Manager instance not found in scene.");
                    }
                    else
                    {
                        eventManager.Init();
                    }
                }
                return eventManager;
            }
        }

        private void Init()
        {
            if (events == null)
            {
                events = new Dictionary<string, UnityEvent>();
            }
        }

        public static void Subscribe(string eventName, UnityAction action)
        {
            UnityEvent thisEvent = null;
            if (Instance.events.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(action);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(action);
                Instance.events.Add(eventName, thisEvent);
            }
        }

        public static void Unsubscribe(string eventName, UnityAction action)
        {
            if (eventManager == null) return;
            UnityEvent thisEvent = null;
            if (Instance.events.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(action);
            }
        }

        public static void FlagEvent(string eventName)
        {
            UnityEvent thisEvent = null;
            if (Instance.events.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke();
                //Debug.Log("Event flagged: " + eventName);
            }
            else
            {
                Debug.Log("The event '" + eventName + "' was raised, but had no listeners.");
            }
        }
    }

}