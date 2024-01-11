using UnityEngine;
using System.Collections.Generic;
#if (UNITY_EDITOR)
using UnityEditor.Experimental.GraphView;
#endif

namespace TerraTiler2D
{
    /// <summary>
    /// Starting point of the graph. Gets created automatically.  	
    /// </summary>
    public class Entry_Node : Node
    {
        private Port<Flow> outputPort;

        //========== Initialization ==========

        public Entry_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Entry;

            SetTooltip("Starting point of the graph. Creates a new empty world.");

#if (UNITY_EDITOR)
            //Make the node undeletable.
            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Copiable;
#endif
        }

        protected override void InitializeAdditionalElements()
        {

        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            //Create an output port.
            outputPort = GeneratePort<Flow>("", PortDirection.Output, "Output", PortCapacity.Single, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        //========== Node methods ==========

        public GraphOutput RunGraph(List<Blackboard_Property_Abstract> properties = null)
        {
            if (Glob.GetInstance().PauseBetweenNodes)
            {
                //Set this node to active
                EventManager.GetInstance().RaiseEvent(new ToggleNodeActiveEvent().Init(this, true));
            }

            if (properties == null)
            {
                properties = new List<Blackboard_Property_Abstract>();
            }

            //Create a new flow.
            Flow flow = (Flow)outputPort.GetPortVariable();

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
                        if (Glob.GetInstance().PauseBetweenNodes)
                        {
                            EventManager.GetInstance().RaiseEvent(new ToggleNodeActiveEvent().Init(edges.Current.input.node as Node, true));
                        }

                        //Make the next node apply its behaviour.
                        (edges.Current.input.node as Flow_Node).ApplyBehaviour(flow, true);

                        break;
                    }
                }
            }
#else
            foreach (Port_Abstract port in outputPort.connections)
            {
                if (Glob.GetInstance().PauseBetweenNodes)
                {
                    EventManager.GetInstance().RaiseEvent(new ToggleNodeActiveEvent().Init(port.node as Node, true));
                }

                (port.node as Flow_Node).ApplyBehaviour(flow, true);

                break;
            }
#endif
            if (Glob.GetInstance().PauseBetweenNodes)
            {
                EventManager.GetInstance().RaiseEvent(new ToggleNodeActiveEvent().Init(this, false));

            }

            GraphOutput output = new GraphOutput(properties);

            //Return the output
            return output;
        }

        //========== Port data passing ==========

        public object GetOutput()
        {
            return new Flow(Flow.Direction.Forwards);
        }

        //========== NodeData saving and loading ==========

    }
}
