using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TerraTiler2D
{
    ///<summary>Is used to implement an Update method in non-monobehaviour based classes.</summary>
    public class UpdateManager : Singleton<UpdateManager>
    {
        private List<Action> listeners = new List<Action>();
        private List<Tuple<Action, bool>> listenerChange = new List<Tuple<Action, bool>>();

        private TerraTiler2DUpdateCaller updateCaller;

        protected override void Initialize()
        {
            base.Initialize();

#if (UNITY_EDITOR)
            //Derive the update behaviour from the Unity Editor
            EditorApplication.update += Update;
#else
            //Instantiate a monobehaviour and derive the update from there
            initializeUpdateCaller();
#endif
        }

        private void initializeUpdateCaller()
        {
            //If there is no updateCaller yet
            if (updateCaller == null)
            {
                //Check if one has been instantiated earlier
                updateCaller = GameObject.FindObjectOfType<TerraTiler2DUpdateCaller>();
            
                //If there was no updateCaller instantiated before
                if (updateCaller == null)
                {
                    //Instantiate a new updateCaller
                    updateCaller = new GameObject("TerraTiler2D Update Caller").AddComponent<TerraTiler2DUpdateCaller>();
                    //Apply DontDestoyOnLoad
                    GameObject.DontDestroyOnLoad(updateCaller);
                }

                //If this UpdateManage has not subscribed to the updateCaller yet
                if (updateCaller.OnUpdate == null || updateCaller.OnUpdate.GetInvocationList().Length <= 0)
                {
                    //Subscribe to the updateCaller
                    updateCaller.OnUpdate += Update;
                }
            }
        }

        public void AddListener(Action listener)
        {
            listenerChange.Add(new Tuple<Action, bool>(listener, true));
        }

        public void RemoveListener(Action listener)
        {
            listenerChange.Add(new Tuple<Action, bool>(listener, false));        
        }

        private void Update()
        {
            processChanges();

            //If there is at least 1 listener
            if (listeners.Count > 0)
            {
                foreach (Action listener in listeners)
                {
                    listener.Invoke();
                }
            }
        }

        private void processChanges()
        {
            foreach (Tuple<Action, bool> change in listenerChange)
            {
                if (change.Item2)
                {
                    listeners.Add(change.Item1);
                }
                else
                {
                    listeners.Remove(change.Item1);
                }
            }

            listenerChange.Clear();
        }
    }

    public class TerraTiler2DUpdateCaller : MonoBehaviour
    {
        public System.Action OnUpdate;

        private void Update()
        {
            OnUpdate();
        }
    }
}
