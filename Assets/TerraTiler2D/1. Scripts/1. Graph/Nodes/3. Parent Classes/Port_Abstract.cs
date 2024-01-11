using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Runtime.InteropServices;
#if (UNITY_EDITOR)
using UnityEditor.Experimental.GraphView;
#endif

namespace TerraTiler2D
{
    public delegate object OutputPortMethod();

    public enum PortDirection
    {
        Input = 0,
        Output = 1
    }
    public enum PortCapacity
    {
        Single,
        Multi
    }

    public abstract class Port_Abstract : VisualElement
    {
#if (UNITY_EDITOR)
        public UnityEditor.Experimental.GraphView.Port port;
#endif
        public PortDirection direction;
        public PortCapacity capacity;

        public Node node;
        public List<Port_Abstract> connections = new List<Port_Abstract>();

        public bool isMandatory;
        public string Guid;

        private Image portImage;

        private bool isBeingRun = false;

        protected Port_Abstract(Node node, string portName, PortDirection portDirection, System.Type portType, PortCapacity capacity = PortCapacity.Single, bool isMandatory = true, string tooltip = "")
        {
            this.node = node;

            this.isMandatory = isMandatory;

            this.tooltip = tooltip;

            this.direction = portDirection;
            this.capacity = capacity;

            this.name = portName;

#if (UNITY_EDITOR)
            //Create a port and add it to the node
            port = GeneratePort(node, portName, portDirection, portType, capacity);

            if (portType == typeof(Flow))
            {
                changePortIcon(Glob.GetInstance().GraphFlowPortIcon);
            }
            else if (Glob.GetInstance().IsList(portType))
            {
                changePortIcon(Glob.GetInstance().ArrayPortIcon, true);
            }
            else if (portType == typeof(TileLayerMask) || portType == typeof(TileMask))
            {
                changePortIcon(Glob.GetInstance().MaskPortIcon, true);
            }
            else if(capacity == PortCapacity.Multi && isMandatory)
            {
                changePortIcon(Glob.GetInstance().IsRequiredMultiCapacityPortIcon);
            }
            else if (capacity == PortCapacity.Multi)
            {
                changePortIcon(Glob.GetInstance().MultiCapacityPortIcon);
            }
            else if (isMandatory)
            {
                changePortIcon(Glob.GetInstance().IsRequiredPortIcon);
            }
#endif
        }

        public void SetOutputPortMethod(OutputPortMethod newMethod)
        {
            GetOutputPortVariable = newMethod;
        }

#if (UNITY_EDITOR)
        //Generate a new input or output port. The port does not get added yet.
        protected Port GeneratePort(Node node, string portName, PortDirection portDirection, System.Type portType, PortCapacity capacity = PortCapacity.Single)
        {
            Port newPort = node.InstantiatePort(Orientation.Horizontal, (Direction)portDirection, (Port.Capacity)capacity, portType);

            //Set the name of the port (not the portName), so that the stylesheet knows what style to apply.
            newPort.name = portType.Name + "-port";
            Glob.GetInstance().DebugString("Set port name to: " + newPort.name + ". If this port type has no custom color, add the port name to Node.uss.", Glob.DebugCategories.Edge, Glob.DebugLevel.High, Glob.DebugTypes.Default);

            //Give the port the correct color, matching its type.
            if (Glob.GetInstance().TypeColors.ContainsKey(portType))
            {
                newPort.portColor = Glob.GetInstance().TypeColors[portType];
            }

            //Set the visual port name
            newPort.portName = portName;

            //Call OnDropOutsidePort and OnDrop for this port.
            newPort.AddManipulator(new EdgeConnector<UnityEditor.Experimental.GraphView.Edge>(node));

            //Add the port to the node
            this.Add(newPort);

            return newPort;
        }
#endif

        public virtual object GetPortVariable()
        {
            if (isBeingRun)
            {
                Glob.GetInstance().DebugString("Infinite loop detected at port '" + name + "' of node '" + node.GetTitle() + "'. Returning NULL, or the value in the port field if there is one.", Glob.DebugCategories.Error, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                isBeingRun = false;
                return null;
            }

            isBeingRun = true;

            object returnValue = null;

            if (direction == PortDirection.Input)
            {
                returnValue = GetInputPortVariable();
            }
            else
            {
                if (GetOutputPortVariable != null)
                {
                    returnValue = GetOutputPortVariable();
                }
            }

            //If the port is about to return NULL, but an actual value is expected
            if (returnValue == null && isMandatory)
            {
                string debug = "Aborting graph execution.\n";
#if (UNITY_EDITOR)
                //Variable 'port' inherits from VisualElement, so it is only available in the Unity editor
                debug += direction + " port '" + port.portName + "' of node '" + node.GetTitle() + "' ";
#else
                debug += "An " + direction + " port on Node '" + node.GetTitle() + "' ";
#endif
                debug += "returned a value of NULL. A connection to this port is required.";
                Glob.GetInstance().DebugString(debug, Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);

                //Get the nearest Flow_Nodes
                List<Flow_Node> nearestFlowNodes = this.node.GetNearestFlowNodes();
                foreach (Flow_Node flowNode in nearestFlowNodes)
                {
                    //Set all the nearest Flow_Nodes to inactive
                    EventManager.GetInstance().RaiseEvent(new ToggleNodeActiveEvent().Init(flowNode, false));
                }
                //Stop the graph
#if (UNITY_EDITOR)
                GraphRunner.GetInstance().StopGraph(node.GetGraphView());
#else
                GraphRunner.GetInstance().StopGraph();
#endif
            }

            isBeingRun = false;
            return returnValue;
        }
        protected virtual object GetInputPortVariable()
        {
            //If this is an input port
            if (direction == PortDirection.Input)
            {
                //If the port is connected to something
#if (UNITY_EDITOR)
                //Variable 'port' inherits from VisualElement, so it is only available in the Unity editor
                if (port.connected)
                {
#else
                if (connections.Count > 0)
                {
#endif
                    //Get all connected ports
                    List<Port_Abstract> connectedPorts = GetConnectedPorts();

                    //If this port can only have 1 connection
                    if (capacity == PortCapacity.Single)
                    {
                        //Get the first connected port
                        Port_Abstract firstConnectedPort = connectedPorts[0];
                        //Request the variable of the first connected port and return it
                        return firstConnectedPort.GetPortVariable();
                    }
                    else
                    {
                        List<object> connectedPortVariables = new List<object>();
                        for (int i = 0; i < connectedPorts.Count; i++)
                        {
                            //Request the variable of a connected port and add it to the array.
                            connectedPortVariables.Insert(i, connectedPorts[i].GetPortVariable());
                        }
                        //Return the array of variables.
                        return connectedPortVariables;
                    }
                }
            }
            //Nodes should handle their output ports themselves.
            //Nodes should also handle default values themselves in the case of a port with no connection.

            return null;
        }
        private OutputPortMethod GetOutputPortVariable;

        protected List<Port_Abstract> GetConnectedPorts()
        {
            bool isInput = (direction == PortDirection.Input);

            List<Port_Abstract> connectedPorts = new List<Port_Abstract>();

#if (UNITY_EDITOR)
            IEnumerator<UnityEditor.Experimental.GraphView.Edge> connectedEdges = port.connections.GetEnumerator();

            while (connectedEdges.MoveNext())
            {
                if (isInput)
                {
                    connectedPorts.Add(connectedEdges.Current.output.parent as Port_Abstract);
                }
                else
                {
                    connectedPorts.Add(connectedEdges.Current.input.parent as Port_Abstract);
                }
            }
#else
            connectedPorts = connections;
#endif
            return connectedPorts;
        }

        public virtual PortData GetPortData()
        {
            return new PortData()
            {

            };
        }
        public virtual void LoadPortData(PortData data)
        {
            Guid = data.PortGUID;
        }
        public virtual void AddConnection(Port_Abstract targetPort)
        {
            connections.Add(targetPort);

#if (UNITY_EDITOR)
            HandleConnected(targetPort);
#endif
        }

#if (UNITY_EDITOR)
        public virtual void HandleConnected(Port_Abstract otherPort)
        {
            //If this port has more than 1 connection, but should only have 1
            if (capacity == PortCapacity.Single && connections.Count > 1)
            {
                //Check all the connections
                foreach (Port_Abstract connectedPort in connections)
                {
                    //Find the connection that should be disconnected
                    if (connectedPort != otherPort)
                    {
                        UnityEditor.Experimental.GraphView.Edge edge = new UnityEditor.Experimental.GraphView.Edge();
                        if (direction == PortDirection.Input)
                        {
                            edge.input = this.port;
                            edge.output = connectedPort.port;
                        }
                        else
                        {
                            edge.input = connectedPort.port;
                            edge.output = this.port;
                        }

                        //Disconnect the old connection
                        HandleDisconnected(edge);
                        //otherPort.HandleDisconnected(edge);
                        break;
                    }
                }
            }

            //Let the node handle the new connection
            node.HandlePortConnected(this, otherPort);

            //Let the input port of the connection handle the connection
            if (this.direction == PortDirection.Input)
            {
                Port_Abstract inputPort = this;
                Port_Abstract outputPort = otherPort;

                NodeLinkData linkData = new NodeLinkData
                {
                    BaseNodeGuid = outputPort.node.Guid,
                    BasePortGuid = outputPort.Guid,
                    TargetNodeGuid = inputPort.node.Guid,
                    TargetPortGuid = inputPort.Guid,
                    PortType = port.portType
                };

                //Send an event announcing that an edge was connected
                EventManager.GetInstance().RaiseEvent(new EdgeConnectedEvent().Init(linkData));
            }
        }
        public virtual void HandleDisconnected(UnityEditor.Experimental.GraphView.Edge edge)
        {
            var outputNode = edge.output.node as Node;
            var inputNode = edge.input.node as Node;

            NodeLinkData linkData = new NodeLinkData
            {
                BaseNodeGuid = outputNode.Guid,
                BasePortGuid = (edge.output.parent as Port_Abstract).Guid,
                TargetNodeGuid = inputNode.Guid,
                TargetPortGuid = (edge.input.parent as Port_Abstract).Guid,

                PortType = edge.output.portType
            };

            //Send an event announcing that an edge was disconnected
            EventManager.GetInstance().RaiseEvent(new EdgeDisconnectedEvent().Init(linkData));

            //Actually disconnect the edge
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);

            //Let the node handle the disconnect
            node.HandlePortDisconnected(this);

            if (direction == PortDirection.Input)
            {
                connections.Remove(edge.output.parent as Port_Abstract);
                (edge.output.parent as Port_Abstract).connections.Remove(edge.input.parent as Port_Abstract);
            }
            else
            {
                connections.Remove(edge.input.parent as Port_Abstract);
                (edge.input.parent as Port_Abstract).connections.Remove(edge.output.parent as Port_Abstract);
            }
        }
        public void DisconnectAllEdges()
        {
            List<UnityEditor.Experimental.GraphView.Edge> allEdges = new List<UnityEditor.Experimental.GraphView.Edge>();

            var allEdgesEnumerator = port.connections.GetEnumerator();
            while (allEdgesEnumerator.MoveNext())
            {
                allEdges.Add(allEdgesEnumerator.Current);
            }

            foreach (UnityEditor.Experimental.GraphView.Edge edge in allEdges)
            {
                //Disconnect the edge
                HandleDisconnected(edge);

                //Delete the edge
                edge.RemoveFromHierarchy();
            }
        }
#endif

        public abstract void CopyPort(Port_Abstract copyFrom);
        public virtual void ResetPort()
        {
            isBeingRun = false;
        }

#if (UNITY_EDITOR)
        private void changePortIcon(Texture2D newImage, bool recolor = false)
        {
            if (newImage == null)
            {
                return;
            }
            portImage = new Image();
            portImage.image = newImage;
            portImage.name = "PortImage";

            //port.ElementAt(0).style.backgroundImage = newImage;

            if (recolor)
            {
                if (Glob.GetInstance().TypeColors.ContainsKey(port.portType))
                {
                    portImage.tintColor = Glob.GetInstance().TypeColors[port.portType];
                }
            }

            //Place the new port icon on top of the existing port icon
            port.ElementAt(0).Insert(1, portImage);
            portImage.PlaceInFront(port.ElementAt(0).ElementAt(0));
            portImage.style.position = Position.Absolute;

            //Enable overflow so the icon is visible
            port.ElementAt(0).style.overflow = Overflow.Visible;
            port.ElementAt(0).style.unityOverflowClipBox = OverflowClipBox.PaddingBox;

            //NOTE: Every icon should be 10x10 pixels
            portImage.style.width = 12;
            portImage.style.height = 12;
        }
        public void SetPortColor(Color newColor)
        {
            port.portColor = newColor;
            if (portImage != null)
            {
                portImage.tintColor = newColor;
            }
        }

        public string GetPortDescription()
        {
            string tooltip = "";

            //Add the port type to the tooltip
            if (this.GetType().IsConstructedGenericType)
            {
                tooltip += this.GetType().GetGenericArguments()[0].Name;
            }
            else
            {
                tooltip += this.GetType().Name;
            }

            if (!string.IsNullOrEmpty(port.portName))
            {
                if (tooltip.Length <= 6) //TODO: Improve this method of aligning
                {
                    tooltip += "\t";
                }
                tooltip += "\t(" + port.portName + ")";
            }

            return tooltip;
        }
#endif
    }
}
