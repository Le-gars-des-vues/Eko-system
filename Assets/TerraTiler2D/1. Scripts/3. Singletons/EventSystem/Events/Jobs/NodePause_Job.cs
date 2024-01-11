using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

namespace TerraTiler2D
{
    public class NodePause_JobEvent : JobEvent
    {
        public struct NodePause_Job : IJob
        {
            public void Execute()
            {

            }
        }

        public Flow flow;
        public bool trickleDown;
        public bool waitingOnResult;

        public NodePause_JobEvent(Flow flow, bool trickleDown, bool waitingOnResult)
        {
            this.flow = flow;
            this.trickleDown = trickleDown;
            this.waitingOnResult = waitingOnResult;
        }

        public override void Execute<TEvent>(Action<TEvent> listener)
        {
            //Prepare the job
            NodePause_Job jobData = new NodePause_Job();
            //Schedule the job
            handle = jobData.Schedule();

            //Resumes the main thread, and forces the job to execute
            base.Execute(listener);
        }

        protected override void FinishJob()
        {
            //Inform the main thread about the results
            EventManager.GetInstance().RaiseEvent(new NodePause_JobFinishedEvent().Init(flow, trickleDown, waitingOnResult));
        }
    }

    public class NodePause_JobFinishedEvent : JobFinishedEvent
    {
        public Flow flow;
        public bool trickleDown;
        public bool waitingOnResult;

        public NodePause_JobFinishedEvent Init(Flow flow, bool trickleDown, bool waitingOnResult)
        {
            this.flow = flow;
            this.trickleDown = trickleDown;
            this.waitingOnResult = waitingOnResult;

            base.Init();

            return this;
        }
    }
}