using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    ///<summary>Used to run graphs, either in the Unity Editor or during game runtime.</summary>
    public class GraphRunner : Singleton<GraphRunner>
    {
        private bool graphIsRunning = false;
        private GraphView currentGraphView = null;
        private List<Node> activeNodes = new List<Node>();
        private List<Node> waitingNodes = new List<Node>();

        private bool graphHasBeenStopped = false;

        private int previousSeed = 0;

        public bool HasGraphBeenStopped
        {
            get
            {
                //If there are no active or waiting nodes, the graph is not running. This is used instead of the graphIsRunning variable, because my Unit Tests do not rely on that variable.
                if (activeNodes.Count + waitingNodes.Count == 0)
                {
                    return true;
                }

                return graphHasBeenStopped;
            }

            set
            {
                graphHasBeenStopped = value;
            }
        }

        protected override void Initialize()
        {
            EventManager.GetInstance().AddListener<ToggleNodeActiveEvent>(setNodeActive);
            EventManager.GetInstance().AddListener<ToggleNodeWaitingEvent>(setNodeWaiting);
            
            base.Initialize();
        }

        ///<summary>Run the graph currently visible in the passed in <see cref="GraphView"/>.</summary>
        ///<param name="graphView">The GraphView that contains the graph that needs to be run.</param>
        ///<param name="listener">When the graph is finished, it will pass the generated <see cref="GraphOutput"/> to this method. If a listener is passed into this method, all other listeners for the <see cref="GraphFinishedEvent"/> will be removed. This is done so that users can avoid interaction with the <see cref="EventManager"/>, as that adds an extra layer of complexity. If you need multiple listeners, leave this parameter empty and use EventManager.GetInstance().AddListener() instead.</param>
        public void RunGraph(GraphView graphView, Action<GraphFinishedEvent> listener = null, bool usePreviousSeed = false)
        {
            //Is a graph already running?
            if (graphIsRunning)
            {
                Glob.GetInstance().DebugString("The graph is still running, please wait for it to finish before calling RunGraph again.\n\nIf this warning keeps showing up, try to make the Unity editor recompile by making a change in a script and saving it, or by restarting the Unity editor. ", Glob.DebugCategories.Error, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                return;
            }

            HasGraphBeenStopped = false;
            graphIsRunning = true;

            //Reset all the nodes to their default state.
            graphView.ResetNodeVariables();
            //Reset all the blackboard properties to their default state.
            graphView.ResetBlackboardProperties();

            EventManager.GetInstance().RaiseEvent(new GraphStartedEvent().Init());

            //If the user passed in a listener
            if (listener != null)
            {
                //Remove all current listeners, so that 'listener' is the only one
                EventManager.GetInstance().RemoveAllListeners<GraphFinishedEvent>();
                //Add the user's action as the only listener. After the graph is finished running, this listener is where control is given back to the user.
                EventManager.GetInstance().AddListener<GraphFinishedEvent>(listener);
            }

            if (Glob.GetInstance().PauseBetweenNodes)
            {
                currentGraphView = graphView;
            }

            if (!usePreviousSeed)
            {
                previousSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                UnityEngine.Random.InitState(previousSeed);
            }
            else
            {
                Glob.GetInstance().DebugString("Executing the graph using the previous seed: " + previousSeed, Glob.DebugCategories.Graph, Glob.DebugLevel.User, Glob.DebugTypes.Default);
                UnityEngine.Random.InitState(previousSeed);
            }

            //Run the graph
            graphView.RunGraph();

            if (!Glob.GetInstance().PauseBetweenNodes)
            {
                handleGraphFinished(graphView);
            }
        }

        ///<summary>Run a graph based on saved GraphData, without visually showing it in the editor. Use this during runtime. When the graph is finished, it will pass the generated GraphOutput to the listener.</summary>
        ///<param name="graphData">The GraphData object that contains the data of the graph that needs to be run.</param>
        ///<param name="listener">When the graph is finished, it will pass the generated <see cref="GraphOutput"/> to this method. If a listener is passed into this method, all other listeners for the <see cref="GraphFinishedEvent"/> will be removed. This is done so that users can avoid interaction with the <see cref="EventManager"/>, as that adds an extra layer of complexity. If you need multiple listeners, leave this parameter empty and use EventManager.GetInstance().AddListener() instead.</param>
        public void RunGraphDuringRuntime(GraphData graphData, Action<GraphFinishedEvent> listener)
        {
            //Check if the save file exists.
            if (graphData == null)
            {
                Debug.LogError("Target TerraTiler2D GraphData file does not exist!");
                return;
            }
            
            //Initialize an empty graph view
            GraphView graphView = new GraphView();
            
            //Load the graph into the empty graph view
            GraphSaveManager.GetInstance().LoadGraph(graphData, graphView);

            //Run the graph
            RunGraph(graphView, listener);
        }

        public int GetPreviousSeed()
        {
            return previousSeed;
        }

        public void StopGraph(GraphView graphView = null)
        {
            if (graphView == null)
            {
                if (currentGraphView != null)
                {
                    graphView = currentGraphView;
                }
                else
                {
                    Glob.GetInstance().DebugString("Tried to stop the execution of a graph, but no active GraphView was found.", Glob.DebugCategories.Graph, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                    return;
                }
            }

            if (graphIsRunning)
            {
                HasGraphBeenStopped = true;

                graphView.StopGraph();

                //TODO: See the TODO further down
                cleanUpAfterGraphExecution(graphView);
            }
        }

        private void handleGraphFinished(GraphView graphView)
        {
            //There are no more active or waiting nodes, so the graph is finished
            Glob.GetInstance().DebugString("Graph is finished", Glob.DebugCategories.Graph, Glob.DebugLevel.User, Glob.DebugTypes.Default);

            graphIsRunning = false;

#if (UNITY_EDITOR)
            if (graphView == null)
            {
                graphView = Graph.Instance.GetGraphView();
            }
#endif
            //Return the output of the graph
            GraphOutput generatedOutput = new GraphOutput(graphView.GetGraphBlackboard().GetProperties());

            //Get all the current listeners to the GraphFinishedEvent
            List<object> currentListeners = new List<object>();
            currentListeners.AddRange(EventManager.GetInstance().GetAllListeners<GraphFinishedEvent>());

            //Broadcast a GraphFinishedEvent containing the GraphOutput.
            EventManager.GetInstance().RaiseEvent(new GraphFinishedEvent().Init(generatedOutput));

            //Clean up
            cleanUpAfterGraphExecution(graphView, currentListeners);
        }

        private void cleanUpAfterGraphExecution(GraphView graphView, List<object> currentGraphFinishedEventListeners = null)
        {
            if (currentGraphFinishedEventListeners == null)
            {
                currentGraphFinishedEventListeners = EventManager.GetInstance().GetAllListeners<GraphFinishedEvent>();
            }

            //Remove all the listeners that were listening to the previously raised event.
            foreach (object listener in currentGraphFinishedEventListeners)
            {
                EventManager.GetInstance().RemoveListener((Action<GraphFinishedEvent>)listener);
            }

            if (EventManager.GetInstance().GetAllListeners<GraphFinishedEvent>().Count > 1)
            {
                Glob.GetInstance().DebugString("GraphFinishedEvent has " + EventManager.GetInstance().GetAllListeners<GraphFinishedEvent>().Count + " listeners. Although it is possible to have multiple listeners, it is not intended and may cause unexpected behaviour.", Glob.DebugCategories.Graph, Glob.DebugLevel.Low, Glob.DebugTypes.Warning);
            }

            //TODO: Make sure this does not cause a race condition when used alongside PauseBetweenNodes and a StopGraph() call
            HasGraphBeenStopped = false;
            graphIsRunning = false;

#if (UNITY_EDITOR)
            //Reset all the nodes to their default state.
            graphView.ResetNodeVariables();
            //Reset all the blackboard properties to their default state.
            graphView.ResetBlackboardProperties();
#endif
        }

        //This method keeps track of all the nodes that are currently applying their behaviour. As soon as there are 0 active nodes, the graph is finished.
        private void setNodeActive(ToggleNodeActiveEvent evt)
        {
            Glob.GetInstance().DebugString("Active node count: " + activeNodes.Count + ". Toggling: [" + evt.toggle + ": " + evt.node + "]", Glob.DebugCategories.Node, Glob.DebugLevel.Medium, Glob.DebugTypes.Default);

            //If a node is being set to inactive
            if (!evt.toggle)
            {
                //If the node is in the list
                if (activeNodes.Contains(evt.node))
                {
                    //Set the node to inactive
                    activeNodes.Remove(evt.node);

                    //If this is the last remaining active node that is not waiting
                    if (activeNodes.Count <= waitingNodes.Count)
                    {
                        //If there are no nodes waiting
                        if (waitingNodes.Count == 0)
                        {
                            handleGraphFinished(currentGraphView);
                        }
                        //If there is a node waiting
                        else
                        {
                            //Inform the most recently added node that it can stop waiting
                            EventManager.GetInstance().RaiseEvent(new ToggleNodeWaitingEvent().Init(waitingNodes[waitingNodes.Count-1], false));
                        }
                    }
                }
                else
                {
                    //TODO: Disable the connected Flow node
                    Glob.GetInstance().DebugString("Tried to set node '" + evt.node.ToString() + "' to inactive, but it was never set to active.", Glob.DebugCategories.Node, Glob.DebugLevel.Medium, Glob.DebugTypes.Warning);
                }
            }
            //If a node is being set to active
            else if (evt.toggle)
            {
                //If the node is not already tagged as active
                if (!activeNodes.Contains(evt.node))
                {
                    //Set the node to active
                    activeNodes.Add(evt.node);
                }
                else
                {
                    Glob.GetInstance().DebugString("Tried to set node '" + evt.node.ToString() + "' to active, but it was already active. Make sure to set a node to inactive after it is finished.", Glob.DebugCategories.Node, Glob.DebugLevel.Medium, Glob.DebugTypes.Error);
                }
            }
        }

        private void setNodeWaiting(ToggleNodeWaitingEvent evt)
        {
            Glob.GetInstance().DebugString("Waiting node count: " + waitingNodes.Count + ". Toggling: [" + evt.toggle + ": " + evt.node + "]", Glob.DebugCategories.Node, Glob.DebugLevel.Medium, Glob.DebugTypes.Default);
            if (evt.toggle)
            {
                //If the node is not already tagged as waiting
                if (!waitingNodes.Contains(evt.node))
                {
                    waitingNodes.Add(evt.node);
                }
                else
                {
                    Glob.GetInstance().DebugString("Tried to set node '" + evt.node.ToString() + "' to Waiting, but it was already Waiting. Make sure to set a node to Not Waiting after it has resumed.", Glob.DebugCategories.Node, Glob.DebugLevel.Low, Glob.DebugTypes.Error);
                }
            }
            else
            {
                //If the node is in the list
                if (waitingNodes.Contains(evt.node))
                {
                    waitingNodes.Remove(evt.node);
                }
            }
        }
    }
}
