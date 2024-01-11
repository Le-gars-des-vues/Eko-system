using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using TerraTiler2D;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

namespace TerraTiler2D
{
    public abstract class JobEvent
    {
        protected JobHandle handle;

        public virtual void Execute<TEvent>(Action<TEvent> listener)
            where TEvent : Event
        {
            EventManager.GetInstance().AddListener<TEvent>(listener);

            //TODO: This does not work in a build
            //Subscribe to an update function.
            UpdateManager.GetInstance().AddListener(Update);
            //EditorApplication.update += Update;

            //Start all scheduled jobs
            JobHandle.ScheduleBatchedJobs();
        }

        protected void Update()
        {
            //Debug.Log(this + "" + DateTime.Now.Ticks);

            HandleJobCompletion();
        }

        //Gets called every frame when subscribed
        private void HandleJobCompletion()
        {
            //If the job is finished
            if (handle.IsCompleted)
            {
                //Unsubscribe from the update function
                UpdateManager.GetInstance().RemoveListener(Update);
                //EditorApplication.update -= Update;

                //Get the results
                handle.Complete();

                //Let the job process the results
                FinishJob();
                
            }
        }

        protected abstract void FinishJob();
    }
}