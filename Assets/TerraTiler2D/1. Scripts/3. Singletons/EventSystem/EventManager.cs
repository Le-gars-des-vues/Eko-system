using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace TerraTiler2D
{
    ///<summary>Is used to raise events, and alert listeners about them.</summary>
    public class EventManager : Singleton<EventManager>
    {
        protected Dictionary<Type, List<object>> eventHandlers = new Dictionary<Type, List<object>>();

        protected List<Event> EventsToRaiseAfterSceneLoad = new List<Event>();

        public Event[] allEventTypes = GetEventTypes();

        protected override void Initialize()
        {
            SceneManager.sceneLoaded += RaiseEventAfterSceneLoad;

            base.Initialize();
        }

        public void RaiseEvent<TEvent>(TEvent eventToRaise, bool waitForSceneLoad = false)
            where TEvent : Event
        {
            if (waitForSceneLoad)
            {
                EventsToRaiseAfterSceneLoad.Add(eventToRaise);
                return;
            }

            //If this event type is in the list.
            if (eventHandlers.ContainsKey(eventToRaise.GetType()))
            {
                List<object> callbacks = new List<object>();
                callbacks.AddRange(eventHandlers[eventToRaise.GetType()]);

                //Call all the actions linked to the event type.
                for (int i = 0; i < callbacks.Count; i++)
                {
                    //Convert the callback to the correct type
                    Action<TEvent> callback = (Action<TEvent>)callbacks[i];
                    
                    //Check if the target object has not been destroyed
                    if (callback.Target == null || callback.Target.ToString() == "null")
                    {
                        Glob.GetInstance().DebugString("An object listening to the " + eventToRaise.GetType() + " event is NULL. Removing the callback from the list.", Glob.DebugCategories.Misc, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                        RemoveListener(callback);
                        
                        continue;
                    }

                    //Invoke the callback
                    callback(eventToRaise);
                }
            }
        }

        public void RaiseEvent(Event eventToRaise, bool waitForSceneLoad = false)
        {
            if (waitForSceneLoad)
            {
                EventsToRaiseAfterSceneLoad.Add(eventToRaise);
                return;
            }

            if (eventToRaise is PropertyChangedEvent)
            {
                RaiseEvent((PropertyChangedEvent)eventToRaise);
            }
            else if (eventToRaise is PropertyDeletedEvent)
            {
                RaiseEvent((PropertyDeletedEvent)eventToRaise);
            }

            else
            {
                //NOTE: If calling RaiseEvent with an unspecified event type, you will have to cast it to their correct type. Events usually will not have an unspecified type, meaning this is usually not necessary. If you need unspecified event types, you will need to add the required event types to this if/else list.
                //PropertyChangedEvent and PropertyDeletedEvent are not required in this list, and only act as an example.
                Debug.LogError("Tried to raise an event of type 'Event', and was unable to cast it to their correct type: " + eventToRaise);
            }
        }

        private void RaiseEventAfterSceneLoad(Scene scene, LoadSceneMode mode)
        {
            foreach (Event eventToRaise in EventsToRaiseAfterSceneLoad)
            {
                RaiseEvent(eventToRaise);
            }

            EventsToRaiseAfterSceneLoad.Clear();
        }

        public void AddListener<TEvent>(Action<TEvent> listener)
            where TEvent : Event
        {
            //If this event type is already in the list.
            if (eventHandlers.ContainsKey(typeof(TEvent)))
            {
                //Add the action to the event type.
                eventHandlers[typeof(TEvent)].Add(listener);
            }
            else
            {
                //Add the new event type to the list.
                eventHandlers[typeof(TEvent)] = new List<object>();
                //Add the action to the event type.
                eventHandlers[typeof(TEvent)].Add(listener);
            }
        }

        public void RemoveListener<TEvent>(Action<TEvent> listener)
            where TEvent : Event
        {
            //If this event type is in the list.
            if (eventHandlers.ContainsKey(typeof(TEvent)))
            {
                List<object> callbacks = eventHandlers[typeof(TEvent)];

                //Check all actions that are linked to this event type
                for (int i = 0; i < callbacks.Count; i++)
                {
                    Action<TEvent> tmpCallback = (Action<TEvent>)callbacks[i];

                    if (tmpCallback == listener)
                    {
                        callbacks.RemoveAt(i);

                        break;
                    }
                }
            }
        }

        public void RemoveAllListeners<TEvent>()
            where TEvent : Event
        {
            //If this event type is in the list.
            if (eventHandlers.ContainsKey(typeof(TEvent)))
            {
                List<object> callbacks = eventHandlers[typeof(TEvent)];

                callbacks.Clear();

                eventHandlers.Remove(typeof(TEvent));
            }
        }

        public List<object> GetAllListeners<TEvent>()
            where TEvent : Event
        {
            if (eventHandlers.ContainsKey(typeof(TEvent)))
            {
                return eventHandlers[typeof(TEvent)];
            }
            else
            {
                Glob.GetInstance().DebugString("There are no listeners for the " + typeof(TEvent) + " event, so we are returning an empty list.", Glob.DebugCategories.Misc, Glob.DebugLevel.Low, Glob.DebugTypes.Warning);
                return new List<object>();
            }
        }

        private static Event[] GetEventTypes(params object[] constructorArgs)
        {
            List<Event> objects = new List<Event>();

            foreach (System.Type type in Assembly.GetAssembly(typeof(Event)).GetTypes())
            {
                if (type.IsSubclassOf(typeof(Event)) && type.IsClass && !type.IsAbstract)
                {
                    objects.Add((Event)Activator.CreateInstance(type, constructorArgs));
                }
            }
            return objects.ToArray();
        }

        public Event GetEventOfType(string type)
        {
            for (int i = 0; i < allEventTypes.Length; i++)
            {
                if (allEventTypes[i].ToString() == type)
                {
                    return allEventTypes[i];
                }
            }

            return null;
        }
    }
}