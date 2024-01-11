using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
#if (UNITY_EDITOR)
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace TerraTiler2D
{
    /// <summary>
    /// Base class for node types.
    /// </summary>
    public abstract class Node
#if (UNITY_EDITOR) 
        : UnityEditor.Experimental.GraphView.Node, IEdgeConnectorListener 
#endif
    {
        public enum NodeContainers
        {
            TitleContainer,
            InputContainer,
            OutputContainer,
            ExtensionContainer
        }

        public Glob.NodeTypes nodeType = Glob.NodeTypes.Default;

        //Unique node id
        public string Guid;

        protected string[] searchMenuEntry;

        protected List<Port_Abstract> inputPorts = new List<Port_Abstract>();
        protected List<Port_Abstract> outputPorts = new List<Port_Abstract>();

        //========== Initialization ==========

        public Node(string nodeName, Vector2 position, string guid = null)
        {
            //Set the name of this node.
            SetTitle(nodeName);

            //If no GUID was passed into this constructor, generate a new unique GUID for this node.
            if (guid == null)
            {
#if (UNITY_EDITOR)
                this.Guid = GUID.Generate().ToString();
#endif
            }
            else
            {
                //Otherwise, use the GUID that was passed into this constructor.
                this.Guid = guid;
            }

            InitializeInputPorts();
            InitializeOutputPorts();
            InitializeAdditionalElements();

#if (UNITY_EDITOR)
            //Apply a style sheet to the node.
            styleSheets.Add(Glob.GetInstance().NodeStyleSheet);

            //Refresh the node, and apply all changes.
            RefreshNode();

            //Set the position of the node.
            SetPosition(new Rect(position, Glob.GetInstance().DefaultNodeSize));

            GetContainer(NodeContainers.TitleContainer).style.height = Glob.GetInstance().NodeTitleContainerHeight;
#endif
        }

        protected abstract void InitializeInputPorts();
        protected abstract void InitializeOutputPorts();
        protected abstract void InitializeAdditionalElements();

        //========== Node methods ==========


        /// <summary>
        /// Generate a new input or output port on this node.
        /// </summary>
        /// <param name="portName">The name to display next to the port.</param>
        /// <param name="portDirection">Is this an input or an output port.</param>
        /// <param name="uniqueGuidExtension">The unique GUID extension for this port. Can be anything, but once you set this value, you should not change it. If you do change it, all GraphData objects will fail to load any saved connections to this port.</param>
        /// <param name="capacity">Can this port only hold 1 connection, or multiple.</param>
        /// <param name="isMandatory">Is a connection to this port mandatory for the parent node to execute.</param>
        /// <param name="tooltip">Text that is shown when hovering over the port.</param>
        protected Port<T> GeneratePort<T>(string portName, PortDirection portDirection, string uniqueGuidExtension, PortCapacity capacity = PortCapacity.Single, bool isMandatory = true, string tooltip = "")
        {
            Port<T> newPort = new Port<T>(this, portName, portDirection, typeof(T), capacity, isMandatory, tooltip);

            InitializePort<T>(newPort, portDirection, uniqueGuidExtension);

            return newPort;
        }

        /// <summary>
        /// Generate a new input or output port with a field on this node.
        /// </summary>
        /// <param name="portName">The name to display next to the port.</param>
        /// <param name="portDirection">Is this an input or an output port.</param>
        /// <param name="defaultValue">The default value of the port field.</param>
        /// <param name="uniqueGuidExtension">The unique GUID extension for this port. Can be anything, but once you set this value, you should not change it. If you do change it, all GraphData objects will fail to load any saved connections to this port.</param>
        /// <param name="capacity">Can this port only hold 1 connection, or multiple.</param>
        /// <param name="isMandatory">Is a connection to this port mandatory for the parent node to execute.</param>
        /// <param name="tooltip">Text that is shown when hovering over the port.</param>
        protected PortWithField<T> GeneratePortWithField<T>(string portName, PortDirection portDirection, T defaultValue, string uniqueGuidExtension, PortCapacity capacity = PortCapacity.Single, bool isMandatory = false, string tooltip = "")
        {
            PortWithField<T> newPort = new PortWithField<T>(this, portName, portDirection, typeof(T), defaultValue, capacity, isMandatory, tooltip);

            InitializePort<T>(newPort, portDirection, uniqueGuidExtension);

            return newPort;
        }

        private void InitializePort<T>(Port<T> port, PortDirection portDirection, string uniqueGuidExtension)
        {
            //Construct a GUID for the new port, consisting of the node GUID and the uniqueGuidExtension that was passed in.
            string newPortGuid = Guid + "_" + uniqueGuidExtension;

            //Iterate over all the ports on this node, and check if they have the exact same GUID.
            List<Port_Abstract> checkForDuplicateGUID = this.GetPorts(PortDirection.Input).GetRange(0, this.GetPorts(PortDirection.Input).Count);
            checkForDuplicateGUID.AddRange(this.GetPorts(PortDirection.Output));

            foreach (Port_Abstract otherPort in checkForDuplicateGUID)
            {
                //If another port has the exact same GUID
                if (otherPort.Guid == newPortGuid)
                {
                    //Warn the user, and abort the initialization
                    Glob.GetInstance().DebugString("Port '" + port.name + "' on Node '" + this.GetTitle() + "' has the exact same GUID as Port '" + otherPort.name + "'. Make sure all of the GeneratePort<T>() and GeneratePortWithField<T>() calls on this node have a different, non-randomized, 'uniqueGuidExtension' argument.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    return;
                }
            }

            port.Guid = newPortGuid;

            //The port has been succesfully created, so we can create PortData and store it in a GraphData object.
            PortData portData = port.GetPortData();
            if (portData != null)
            {
                if (!string.IsNullOrEmpty(Guid) && !string.IsNullOrEmpty(port.Guid))
                {
                    portData.NodeGUID = Guid;
                    portData.PortGUID = port.Guid;
                    EventManager.GetInstance().RaiseEvent(new PortCreatedEvent().Init(portData));
                }
            }

            //Actually draw the port on screen.
            switch (portDirection)
            {
                case PortDirection.Input:
#if (UNITY_EDITOR)
                    GetContainer(NodeContainers.InputContainer).Add(port);
#endif
                    inputPorts.Add(port);
                    break;
                case PortDirection.Output:
#if (UNITY_EDITOR)
                    GetContainer(NodeContainers.OutputContainer).Add(port);
#endif
                    outputPorts.Add(port);
                    break;
                default:
                    break;
            }
        }

        protected void SetTooltip(string text)
        {
            GetContainer(NodeContainers.TitleContainer).tooltip = text;
        }
        protected VisualElement GetContainer(NodeContainers container)
        {
#if (UNITY_EDITOR)
            switch (container)
            {
                case NodeContainers.TitleContainer:
                    return titleContainer;
                case NodeContainers.InputContainer:
                    return inputContainer;
                case NodeContainers.OutputContainer:
                    return outputContainer;
                case NodeContainers.ExtensionContainer:
                    return extensionContainer;
                default:
                    return null;
            }
#else
            return new VisualElement();
#endif
        }
        public string GetTitle()
        {
#if (UNITY_EDITOR)
            return title;
#else
            return GetType().ToString();
#endif
        }
        public void SetTitle(string text)
        {
#if (UNITY_EDITOR)
            title = text;
#endif
        }
        public void RefreshNode()
        {
#if (UNITY_EDITOR)
            //Refresh the node, and apply all changes.
            RefreshExpandedState();
            RefreshPorts();
#endif
        }

#if (UNITY_EDITOR)
        public NodeSearchEntry GetSearchMenuEntry(SearchWindowContext context)
        {
            if (searchMenuEntry == null || searchMenuEntry.Length == 0)
            {
                return null;
            }
            NodeSearchEntry entry = new NodeSearchEntry(Glob.GetInstance().DefaultNodeNames[nodeType], GetNodeDescription(), nodeType, context, searchMenuEntry);

            return entry;
        }

        public virtual string[] GetNodeDescription()
        {
            string description = "";
            //Add the description of this node
            if (!string.IsNullOrEmpty(GetContainer(NodeContainers.TitleContainer).tooltip))
            {
                description = GetContainer(NodeContainers.TitleContainer).tooltip;
            }

            string input = "";
            //Add all input port descriptions
            var inputChildren = GetContainer(NodeContainers.InputContainer).Children().Where(x => x.GetType().IsSubclassOf(typeof(Port_Abstract))).GetEnumerator();
            input += "Input:";
            while (inputChildren.MoveNext())
            {
                input += "\n- " + ((Port_Abstract)inputChildren.Current).GetPortDescription();
            }

            string output = "";
            //Add all output port descriptions
            var outputChildren = GetContainer(NodeContainers.OutputContainer).Children().Where(x => x.GetType().IsSubclassOf(typeof(Port_Abstract))).GetEnumerator();
            output += "Output:";
            while (outputChildren.MoveNext())
            {
                output += "\n- " + ((Port_Abstract)outputChildren.Current).GetPortDescription();
            }

            return new string[3] { description, input, output };
        }

        public GraphView GetGraphView()
        {
            return (GraphView)Glob.GetInstance().GetFirstParentVisualElementOfType(this, typeof(GraphView));
        }

        public void DeleteNode()
        {
            if (capabilities.HasFlag(Capabilities.Deletable))
            {
                //NodeData myNodeData = GetNodeData();
                //myNodeData.GUID = Guid;
                //myNodeData.NodeName = GetTitle();
                //myNodeData.Position = GetNodePosition().min;
                //myNodeData.NodeType = nodeType;
                EventManager.GetInstance().RaiseEvent(new NodeDeletedEvent().Init(GetNodeData()));

                List<VisualElement> ports = new List<VisualElement>();
                
                ports.AddRange(GetPorts(PortDirection.Input));
                ports.AddRange(GetPorts(PortDirection.Output));

                foreach (Port_Abstract port in ports)
                {
                    PortData portData = port.GetPortData();
                    portData.NodeGUID = Guid;
                    portData.PortGUID = port.Guid;
                    EventManager.GetInstance().RaiseEvent(new PortDeletedEvent().Init(portData));

                    //Get all the edges connected to the port
                    List<UnityEditor.Experimental.GraphView.Edge> edges = port.port.connections.ToList();

                    //For each edge
                    foreach (UnityEditor.Experimental.GraphView.Edge edge in edges)
                    {
                        //Disconnect the edge from the input and output
                        Port_Abstract inputPort = edge.input.parent as Port_Abstract;
                        Port_Abstract outputPort = edge.output.parent as Port_Abstract;

                        inputPort.HandleDisconnected(edge);
                        outputPort.HandleDisconnected(edge);

                        //Delete the edge
                        edge.RemoveFromHierarchy();
                    }
                }

                //Delete this node
                RemoveFromHierarchy();
            }
        }
#endif

        public Rect GetNodePosition()
        {
#if (UNITY_EDITOR)
            return base.GetPosition();
#else
            //The position of the node is not relevant outside of the unity editor, so simply place every node at 0,0
            return Rect.zero;
#endif
        }

        //========== Port data passing ==========

        public virtual void ResetNodeVariables()
        {
            List<Port_Abstract> ports = new List<Port_Abstract>();
            //Gets all the input ports
            ports.AddRange(GetPorts(PortDirection.Input));
            //Gets all the output ports
            ports.AddRange(GetPorts(PortDirection.Output));

            foreach (Port_Abstract port in ports)
            {
                port.ResetPort();
            }
        }

        public virtual void StopNodeExecution()
        {
            
        }

        public List<T> GetPortVariables<T>(List<object> objects)
        {
            List<T> portVariables = new List<T>();
            for (int i = 0; i < objects.Count; i++)
            {
                portVariables.Insert(i, (T)objects[i]);
            }

            return portVariables;
        }

        public List<Flow_Node> GetNearestFlowNodes()
        {
            List<Flow_Node> nearestFlowNodes = new List<Flow_Node>();

            //If this node is a Flow_Node
            if (this is Flow_Node)
            {
                //Return only this node, as this is always the nearest Flow_Node
                nearestFlowNodes.Add(this as Flow_Node);
            }
            else
            {
                //Get all the output ports of this node
                List<Port_Abstract> portsToCheck = new List<Port_Abstract>();
                portsToCheck.AddRange(GetPorts(PortDirection.Output));

                //Create lists to keep track of which nodes and ports we have checked already
                List<Port_Abstract> checkedPorts = new List<Port_Abstract>();
                List<Node> checkedNodes = new List<Node>() { this };

                //As long as there is a port to check
                while (portsToCheck.Count > 0)
                {
                    //Get the first port in the list
                    Port_Abstract port = portsToCheck[0];

                    //If this port has already been checked
                    if (checkedPorts.Contains(port))
                    {
                        //Remove the port from the portsToCheck list
                        portsToCheck.Remove(port);
                        //Move on to the next port
                        continue;
                    }

                    //If the port is connected to at least one other node
                    if (port.connections.Count > 0)
                    {
                        //For every connected node
                        foreach (Port_Abstract connection in port.connections)
                        {
                            //If the node has already been checked
                            if (checkedNodes.Contains(connection.node))
                            {
                                //Skip this node
                                continue;
                            }

                            //If the other node is a Flow_Node
                            if (connection.node is Flow_Node)
                            {
                                //If the Flow_Node is not yet in the return list
                                if (!nearestFlowNodes.Contains(connection.node))
                                {
                                    //Add the connected node to the return list
                                    nearestFlowNodes.Add(connection.node as Flow_Node);
                                }
                            }
                            //If it is not a Flow_Node
                            else
                            {
                                //Add all the output ports on the node to the portsToCheck list
                                portsToCheck.AddRange(connection.node.GetPorts(PortDirection.Output));
                                //Add the node to the checkedNodes list
                                checkedNodes.Add(connection.node);
                            }
                        }
                    }

                    checkedPorts.Add(port);
                    portsToCheck.Remove(port);
                }
            }

            return nearestFlowNodes;
        }

        //========== Edge management ==========
#if (UNITY_EDITOR)
        //When an edge is dropped outside a port.
        public virtual void OnDropOutsidePort(UnityEditor.Experimental.GraphView.Edge edge, Vector2 position)
        {
            GraphView graphView = Graph.Instance.GetGraphView();

            if (graphView != null)
            {
                var scaledWorldPos = Graph.Instance.GetGraphView().contentViewContainer.LocalToWorld(position / Graph.Instance.GetGraphView().scale);
                var viewportPos = Graph.Instance.rootVisualElement.parent.ChangeCoordinatesTo(Graph.Instance.GetGraphView().viewport, scaledWorldPos);
                var viewportPosOffset = viewportPos + Graph.Instance.position.position - (Vector2)Graph.Instance.GetGraphView().viewTransform.position;

                graphView.GetSearchWindow().Open(new SearchWindowContext(viewportPosOffset), Graph.Instance);

                string search = "";
                if (edge.input != null)
                {
                    search = ":output: ";
                    search += edge.input.portType.Name;
                }
                else
                {
                    search = ":input: ";
                    search += edge.output.portType.Name;
                }

                graphView.GetSearchWindow().GetSearchBar().value = "";
                graphView.GetSearchWindow().GetSearchBar().value = search;
            }
        }
        //When an edge is connected to a port.
        public virtual void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, UnityEditor.Experimental.GraphView.Edge edge)
        {
            Port_Abstract inputPort = edge.input.parent as Port_Abstract;
            Port_Abstract outputPort = edge.output.parent as Port_Abstract;

            outputPort.AddConnection(inputPort);
            inputPort.AddConnection(outputPort);
        }

        public virtual void HandlePortConnected(Port_Abstract connectedPort, Port_Abstract otherPort)
        {

        }
        public virtual void HandlePortDisconnected(Port_Abstract disconnectedPort)
        {

        }

        public void DisconnectAllPorts()
        {
            //Get all Ports
            List<Port_Abstract> allPorts = new List<Port_Abstract>();
            allPorts.AddRange(GetPorts(PortDirection.Input));
            allPorts.AddRange(GetPorts(PortDirection.Output));

            //For each port
            foreach (Port_Abstract port in allPorts)
            {
                //Tell the port to disconnect all edges
                port.DisconnectAllEdges();
            }
        }
#endif
        public List<Port_Abstract> GetPorts(PortDirection direction)
        {
            switch (direction)
            {
                case PortDirection.Input:
                    return inputPorts;
                case PortDirection.Output:
                    return outputPorts;
                default:
                    return null;
            }
        }

#if (UNITY_EDITOR)
        public void CopyPortValues(Node copyFrom)
        {
            List<Port_Abstract> inputPorts = new List<Port_Abstract>();
            inputPorts.AddRange(copyFrom.GetPorts(PortDirection.Input));
            List<Port_Abstract> outputPorts = new List<Port_Abstract>();
            outputPorts.AddRange(copyFrom.GetPorts(PortDirection.Output));

            List<Port_Abstract> myInputPorts = new List<Port_Abstract>();
            myInputPorts.AddRange(GetPorts(PortDirection.Input));
            List<Port_Abstract> myOutputPorts = new List<Port_Abstract>();
            myOutputPorts.AddRange(GetPorts(PortDirection.Output));

            if (inputPorts.Count == myInputPorts.Count && outputPorts.Count == myOutputPorts.Count)
            {
                for (int i = 0; i < inputPorts.Count; i++)
                {
                    if (inputPorts[i].GetType() == myInputPorts[i].GetType() && inputPorts[i].port.portName == myInputPorts[i].port.portName)
                    {
                        myInputPorts[i].CopyPort(inputPorts[i]);
                    }
                }
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            if (nodeType != Glob.NodeTypes.Default)
            {
                EventManager.GetInstance().RaiseEvent(new NodeChangedEvent().Init(GetNodeData()));
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            
        }
#endif

        //========== NodeData saving and loading ==========

        public virtual NodeData GetNodeData(NodeData nodeData = null)
        {
            if (nodeData == null)
            {
                return new NodeData()
                {
                    GUID = Guid,
                    NodeName = GetTitle(),
                    Position = GetNodePosition().min,
                    NodeType = nodeType
                };
            }
            else
            {
                nodeData.GUID = Guid;
                nodeData.NodeName = GetTitle();
                nodeData.Position = GetNodePosition().min;
                nodeData.NodeType = nodeType;

                return nodeData;
            }
        }
        public virtual void LoadNodeData(NodeData data)
        {

        }
    }
}
