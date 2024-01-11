using System.Collections;
using UnityEngine;

namespace TerraTiler2D
{
    /// <summary>
    /// Base class for behaviour node types.
    /// </summary>
    public abstract class Flow_Node : Node
    {
        protected Port<Flow> inputPort;
        protected Port<Flow> outputPort;

        private bool isBeingRun = false;
        private bool hasPaused = false;

        //========== Initialization ==========

        public Flow_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {

        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<Flow>("", PortDirection.Input, "FlowInput", PortCapacity.Multi, true);
        }

        protected override void InitializeOutputPorts()
        {
            outputPort = GeneratePort<Flow>("", PortDirection.Output, "FlowOutput", PortCapacity.Single, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        //========== Node methods ==========

        private void handleJobFinished(NodePause_JobFinishedEvent evt)
        {
            //Stop listening to when the FlowTileLayer_JobEvent is finished
            EventManager.GetInstance().RemoveListener<NodePause_JobFinishedEvent>(handleJobFinished);

            //Resume graph execution where we left off.
            PauseThenApplyBehaviour(evt.flow, evt.trickleDown);
        }

        public void PauseThenApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            if (!hasPaused && !GraphRunner.GetInstance().HasGraphBeenStopped)
            {
                hasPaused = true;

                //Execute a job on a separate thread, and wait for it to finish
                new NodePause_JobEvent(flow, trickleDown, waitingOnResult).Execute<NodePause_JobFinishedEvent>(handleJobFinished);

                //Stop all code execution, unfreezing the main thread
                return;
            }
            else if (!GraphRunner.GetInstance().HasGraphBeenStopped)
            {
                hasPaused = false;
                ApplyBehaviour(flow, trickleDown, waitingOnResult);
            }
        }

        public virtual void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            if (isBeingRun)
            {
                Glob.GetInstance().DebugString("Infinite loop detected at flow node '" + GetTitle() + "'. To create looping functionality, use a 'For loop' or 'While loop' node.", Glob.DebugCategories.Error, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                isBeingRun = false;
                return;
            }
            else if (GraphRunner.GetInstance().HasGraphBeenStopped && flow.direction == Flow.Direction.Forwards)
            {
                Glob.GetInstance().DebugString("Graph execution has been stopped, so node '" + this.GetTitle() + "' will not apply its behaviour.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                isBeingRun = false;
                return;
            }

            isBeingRun = true;

            if (trickleDown)
            {
#if (UNITY_EDITOR)
                //If the world output port is connected to an edge.
                if (outputPort.port.connected)
                {
                    //For every edge connected to the world output port (should not be more than 1).
                    var edges = outputPort.port.connections.GetEnumerator();
                    while (edges.MoveNext())
                    {
                        //If the edge is connected to the input port of another node.
                        if (edges.Current.input != null)
                        {
                            //Toggle the next node to active
                            if (Glob.GetInstance().PauseBetweenNodes)
                            {
                                EventManager.GetInstance().RaiseEvent(new ToggleNodeActiveEvent().Init(edges.Current.input.node as Node, true));
                                //Make the next node apply its behaviour on the world.
                                (edges.Current.input.node as Flow_Node).PauseThenApplyBehaviour(flow, trickleDown);
                            }
                            else
                            {
                                //Make the next node apply its behaviour on the world.
                                (edges.Current.input.node as Flow_Node).ApplyBehaviour(flow, trickleDown);
                            }
                        }
                    }
                }
#else
                foreach (Port_Abstract port in outputPort.connections)
                {
                    if (Glob.GetInstance().PauseBetweenNodes)
                    {
                        EventManager.GetInstance().RaiseEvent(new ToggleNodeActiveEvent().Init(port.node as Node, true));
                        //Make the next node apply its behaviour on the world.
                        (port.node as Flow_Node).PauseThenApplyBehaviour(flow, trickleDown);
                    }
                    else 
                    {
                        (port.node as Flow_Node).ApplyBehaviour(flow, trickleDown);
                    }
                    break;
                }
#endif
            }

            if (Glob.GetInstance().PauseBetweenNodes)
            {
                if (!waitingOnResult)
                {
                    //Toggle this node to inactive
                    EventManager.GetInstance().RaiseEvent(new ToggleNodeActiveEvent().Init(this, false));
                }
                else
                {
                    EventManager.GetInstance().RaiseEvent(new ToggleNodeWaitingEvent().Init(this, true));
                }
            }
            
            isBeingRun = false;
        }

        //========== Port data passing ==========

        public object GetOutput()
        {
            //Get the output of the previous flow node
            object inputObjects = inputPort.GetPortVariable();
            //If this node has multiple flow input connections
            if (((object[])inputObjects).Length > 1)
            {
                Glob.GetInstance().DebugString("Flow node '" + GetTitle() + "' has multiple Flow input connections, and can therefore not be previewed because multiple results are possible.", Glob.DebugCategories.Edge, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                return null;
            }

            //Convert the output of the previous flow node to a Flow object
            Flow returnFlow = (Flow)((object[])inputObjects)[0];
            //Apply behaviour
            ApplyBehaviour(returnFlow, false);
            //Return the results
            return returnFlow;
        }

        public override void StopNodeExecution()
        {
            base.StopNodeExecution();

            isBeingRun = false;
            hasPaused = false;

            //Set this node to inactive
            EventManager.GetInstance().RaiseEvent(new ToggleNodeActiveEvent().Init(this, false));
            //EventManager.GetInstance().RaiseEvent(new ToggleNodeWaitingEvent().Init(this, false));

        }
    }
}
