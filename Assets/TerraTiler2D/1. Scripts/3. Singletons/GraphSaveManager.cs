using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if (UNITY_EDITOR)
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace TerraTiler2D
{
    ///<summary>Handles the saving and loading of GraphData objects.</summary>
    public class GraphSaveManager : Singleton<GraphSaveManager>
    {
        private GraphData currentlyLoadedGraphData;
        private GraphData tempGraphData;

        protected override void Initialize()
        {
            base.Initialize();

#if (UNITY_EDITOR)
            EventManager.GetInstance().AddListener<EdgeConnectedEvent>(HandleEdgeConnected);
            EventManager.GetInstance().AddListener<EdgeDisconnectedEvent>(HandleEdgeDisconnected);

            EventManager.GetInstance().AddListener<NodeCreatedEvent>(HandleNodeCreated);
            EventManager.GetInstance().AddListener<NodeChangedEvent>(HandleNodeChanged);
            EventManager.GetInstance().AddListener<NodeDeletedEvent>(HandleNodeDeleted);

            EventManager.GetInstance().AddListener<PortCreatedEvent>(HandlePortCreated);
            EventManager.GetInstance().AddListener<PortChangedEvent>(HandlePortChanged);
            EventManager.GetInstance().AddListener<PortDeletedEvent>(HandlePortDeleted);

            EventManager.GetInstance().AddListener<PropertyCreatedEvent>(HandlePropertyCreated);
            EventManager.GetInstance().AddListener<PropertyChangedEvent>(HandlePropertyChanged);
            EventManager.GetInstance().AddListener<PropertyDeletedEvent>(HandlePropertyDeleted);

            EventManager.GetInstance().AddListener<KeyDownInGraphEvent>(HandleKeyDownEvents);

            Undo.undoRedoPerformed += handleUndoOperation;
#endif
        }
#if (UNITY_EDITOR)
        private void HandleKeyDownEvents(KeyDownInGraphEvent evt)
        {
            //If CTRL + S is pressed (Save)
            if (evt.keyEvent.keyCode == KeyCode.S && evt.keyEvent.ctrlKey)
            {
                Graph.Instance.RequestDataOperation(Graph.SaveOperation.Save);
            }
        }

        private void handleUndoOperation()
        {
            Graph.Instance.RequestDataOperation(Graph.SaveOperation.Refresh, GetTempGraphData());

            EditorUtility.SetDirty(GetCurrentlyLoadedGraphData());
        }

        //Copy the values of the tempGraphData object into the currently opened GraphData object, and write the changes to disk
        public void SaveGraph()
        {
            //Create a save file
            var graphData = ScriptableObject.CreateInstance<GraphData>();
            graphData.ToolVersion = Glob.GetInstance().ToolVersion;

            //Attempt to write data from all nodes to the save file.
            if (!SaveNodes(graphData, Graph.Instance.GetGraphView()))
            {
                //If writing failed, return.
                return;
            }

            //Save information from the blackboard, and all the properties on it.
            SaveProperties(graphData, Graph.Instance.GetGraphView());

            GraphData existingData = GetCurrentlyLoadedGraphData();

            //The default save location is in the resources folder.
            string assetPath = Glob.GetInstance().GetDefaultGraphSavePath() + Graph.Instance.GetFileName() + ".asset";

            //If there is already a save file
            if (existingData != null)
            {
                assetPath = AssetDatabase.GetAssetPath(existingData);

                Glob.GetInstance().DebugString("Saving graph " + Graph.Instance.GetFileName() + " at: " + assetPath, Glob.DebugCategories.Data, Glob.DebugLevel.Low, Glob.DebugTypes.Default);

                graphData.SetGUID(existingData.GetGUID());
                graphData.SetGraphDataVersion(existingData.GetGraphDataVersion());
                graphData.SetFileName(existingData.GetFileName());

                //Copy the data to the existing save file
                existingData.SetData(graphData);
                existingData.HandleGraphDataVersion();

                //Write the changes to disk, to make sure the changes persist upon exiting Unity.
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(existingData);
                AssetDatabase.SaveAssets();

            }
            else
            {
                Glob.GetInstance().DebugString("Something went wrong with finding the file path to the existing GraphData file. To prevent data loss, the graph has been saved as a new GraphData instance at: " + assetPath, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                
                //If there is no GraphData object yet, create a new one.
                AssetDatabase.CreateAsset(graphData, assetPath);
            }

            SaveTempGraphData();
        }

        public void SaveTempGraphData()
        {
            Glob.GetInstance().DebugString("Saving temp graph data.", Glob.DebugCategories.Data, Glob.DebugLevel.Medium, Glob.DebugTypes.Default);

            if (GetCurrentlyLoadedGraphData() == null)
            {
                return;
            }

            GetTempGraphData().SetGUID(GetCurrentlyLoadedGraphData().GetGUID());
            GetTempGraphData().SetGraphDataVersion(GetCurrentlyLoadedGraphData().GetGraphDataVersion());
            GetTempGraphData().SetFileName(GetCurrentlyLoadedGraphData().GetFileName());
            GetTempGraphData().SetData(GetCurrentlyLoadedGraphData());

            AssetDatabase.Refresh();
            EditorUtility.SetDirty(GetTempGraphData());
            AssetDatabase.SaveAssets();
            return;
        }

        public void LoadTempGraphData(Graph targetGraph)
        {
            Glob.GetInstance().DebugString("Loading temp graph data.", Glob.DebugCategories.Data, Glob.DebugLevel.Medium, Glob.DebugTypes.Default);

            //Get the temp graph data.
            GraphData tempGraphData = GetTempGraphData();

            if (tempGraphData != null)
            {
                LoadGraph(tempGraphData, targetGraph.GetGraphView());
                targetGraph.SetFileName(tempGraphData.GetFileName());
            }
        }
        public GraphData GetTempGraphData()
        {
            //If there is no temp graph data assigned yet
            if (tempGraphData == null)
            {
                //Try to load the temp graph data
                tempGraphData = AssetDatabase.LoadAssetAtPath<GraphData>(Glob.GetInstance().GetTempGraphDataPath());

                //If no temp graph data has been created yet
                if (tempGraphData == null)
                {
                    //Create a new empty graph data object as temp graph data.
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GraphData>(), Glob.GetInstance().GetTempGraphDataPath());

                    //Load the temp graph data again.
                    tempGraphData = AssetDatabase.LoadAssetAtPath<GraphData>(Glob.GetInstance().GetTempGraphDataPath());
                }
            }

            //Return the temp graph data
            return tempGraphData;
        }

        //Attempt to write data from all nodes to the save file.
        private bool SaveNodes(GraphData graphData, GraphView targetGraph)
        {
            List<Node> Nodes = targetGraph.GetAllNodes();

            List<UnityEditor.Experimental.GraphView.Edge> Edges = targetGraph.edges.ToList().Cast<UnityEditor.Experimental.GraphView.Edge>().ToList();

            //Get all the edges that are connected to an input port, and then put the Entry node at the start of the list.
            //var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            UnityEditor.Experimental.GraphView.Edge[] connectedPorts = Edges.Where(x => x.input != null && x.output != null && x.input.node != null).OrderByDescending(x => ((Node)(x.output.node)).nodeType == Glob.NodeTypes.Entry).ToArray();

            //For each edge
            for (int i = 0; i < connectedPorts.Length; i++)
            {
                //Get the input and output nodes.
                var outputNode = connectedPorts[i].output.node as Node;
                var inputNode = connectedPorts[i].input.node as Node;

                //Add a node link to the save file.
                graphData.HandleNodeLink(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.Guid,
                    BasePortGuid = (connectedPorts[i].output.parent as Port_Abstract).Guid,
                    TargetNodeGuid = inputNode.Guid,
                    TargetPortGuid = (connectedPorts[i].input.parent as Port_Abstract).Guid,

                    PortType = connectedPorts[i].output.portType
                }, GraphData.DataOperation.Add, false);
            }

            //For every node
            foreach (var node in Nodes)
            {
                NodeData nodeData;
                //Let the node write its own NodeData.
                nodeData = node.GetNodeData();

                //Add node data to the save file.
                graphData.HandleNodeData(nodeData, GraphData.DataOperation.Add, false);

                List<Port_Abstract> nodePorts = new List<Port_Abstract>();
                //Gets all the input ports
                nodePorts.AddRange(node.GetPorts(PortDirection.Input));
                //Gets all the output ports
                nodePorts.AddRange(node.GetPorts(PortDirection.Output));

                //For every port on the node
                foreach (Port_Abstract port in nodePorts)
                {
                    PortData portData;
                    portData = port.GetPortData();

                    if (portData != null)
                    {
                        portData.NodeGUID = node.Guid;
                        portData.PortGUID = port.Guid;

                        graphData.HandlePortData(portData, GraphData.DataOperation.Add, false);
                    }
                }
            }

            //Return true if data writing was succesful.
            return true;
        }

        //Save information from the blackboard, and all the properties on it.
        private void SaveProperties(GraphData graphData, GraphView targetGraph)
        {
            List<Blackboard_Property_Abstract> properties = targetGraph.GetGraphBlackboard().GetProperties();
            foreach (Blackboard_Property_Abstract property in properties)
            {
                graphData.HandlePropertyData(property.GetPropertyData(), GraphData.DataOperation.Add, false);
            }
        }
#endif

        //Loads a graph, including all the nodes, connections, and properties.
        public void LoadGraph(GraphData graphData, GraphView targetGraph)
        {
            //Check if the save file exists.
            if (graphData == null)
            {
#if (UNITY_EDITOR)
                EditorUtility.DisplayDialog("File not found!", "Target TerraTiler2D GraphData file does not exist!", "OK");
#endif
                return;
            }

            //Create properties, based on the save file.
            CreateExposedProperties(graphData, targetGraph);
            //Create nodes based on the save file
            CreateNodes(graphData, targetGraph);
            //Connect the nodes, based on the save file.
            ConnectNodes(graphData, targetGraph);
        }

        //Create properties, based on the save file.
        private void CreateExposedProperties(GraphData graphData, GraphView targetGraph)
        {
            //Clear existing properties on hot-reload, since we don't want the old and new properties to merge.
            targetGraph.GetGraphBlackboard().ClearBlackboard();
            //Add properties from save file.
            foreach (PropertyData_Abstract propertyData in graphData.GetAllPropertyData())
            {
                //Add a property to the blackboard.
                targetGraph.GetGraphBlackboard().LoadProperty(propertyData);
            }
        }

        //Create nodes based on the save file
        private void CreateNodes(GraphData graphData, GraphView targetGraph)
        {
            //If this is a brand new graph, and there are no nodes in this graph yet
            if (graphData.GetAllNodeData().Count <= 0)
            {
                //Create a new entry node
                targetGraph.CreateNode(Glob.NodeTypes.Entry, Glob.GetInstance().DefaultEntryNodePosition);
                return;
            }

            //For each bit of node data in the save file
            foreach (var nodeData in graphData.GetAllNodeData())
            {
                //Tell the GraphView to create a node of NodeType, and let the new Node handle the rest of the nodeData.
                Node targetNode = targetGraph.CreateNode(nodeData.NodeType, nodeData.Position, nodeData.NodeName, nodeData.GUID);
                if (targetNode == null)
                {
                    Debug.LogWarning("Node of type '" + nodeData.NodeType + "' is NULL, continuing");
                    continue;
                }
                targetNode.LoadNodeData(nodeData);

                //Get all the PortData related to this node.
                List<PortData> nodePortData = graphData.GetPortDataWithGUID(nodeData.GUID);

                List<Port_Abstract> nodePorts = new List<Port_Abstract>();
                //Gets all the input ports
                nodePorts.AddRange(targetNode.GetPorts(PortDirection.Input));
                //Gets all the output ports
                nodePorts.AddRange(targetNode.GetPorts(PortDirection.Output));

                //For every bit of PortData
                foreach (PortData portData in nodePortData)
                {
                    //Get the target port
                    Port_Abstract targetPort = nodePorts.Find(x => x.Guid == portData.PortGUID);

                    if (targetPort != null)
                    {
                        //Load the PortData into the target port
                        targetPort.LoadPortData(portData);
                    }
                }
            }
        }

        //Connect the nodes, based on the save file.
        private void ConnectNodes(GraphData graphData, GraphView targetGraph)
        {
            var Nodes = targetGraph.GetAllNodes();

            //For each node
            for (int i = 0; i < Nodes.Count; i++)
            {
                //Fetch node link data for this node.
                var connections = graphData.GetNodeLinks().Where(x => x.BaseNodeGuid == Nodes[i].Guid).ToList();

                //For each connection to this node
                for (var j = 0; j < connections.Count; j++)
                {
                    Node targetNode = null;

                    if (Nodes.Any(x => x.Guid == connections[j].TargetNodeGuid))
                    {
                        targetNode = Nodes.First(x => x.Guid == connections[j].TargetNodeGuid);
                    }
                    else
                    {
                        Glob.GetInstance().DebugString("Failed to find Node with GUID: " + connections[j].TargetNodeGuid, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                        continue;
                    }

                    List<Port_Abstract> targetPorts = targetNode.GetPorts(PortDirection.Input);

                    Port_Abstract outputPort = null;
                    Port_Abstract inputPort = null;

                    //Find the ports with the matching guids.
                    if (Nodes[i].GetPorts(PortDirection.Output).Any(x => x.Guid == connections[j].BasePortGuid))
                    {
                        outputPort = Nodes[i].GetPorts(PortDirection.Output).First(x => x.Guid == connections[j].BasePortGuid);
                    }
                    else
                    {
                        Glob.GetInstance().DebugString("Failed to find output port with GUID " + connections[j].BasePortGuid + " on Node with GUID " + connections[j].TargetNodeGuid, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                        continue;
                    }
                    //Find the ports with the matching guids.
                    if (targetPorts.Any(x => x.Guid == connections[j].TargetPortGuid))
                    {
                        inputPort = targetPorts.First(x => x.Guid == connections[j].TargetPortGuid);
                    }
                    else
                    {
                        Glob.GetInstance().DebugString("Failed to find input port with GUID " + connections[j].TargetPortGuid + " on Node with GUID " + connections[j].TargetNodeGuid, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                        continue;
                    }

                    //Create a connection between this port and the target port.
                    LinkNodes(outputPort, inputPort, targetGraph);
                }
            }
        }

        //Create a connection between two ports.
        private void LinkNodes(Port_Abstract output, Port_Abstract input, GraphView targetGraph)
        {
#if (UNITY_EDITOR)
            //Create a new connection.
            var tempEdge = new UnityEditor.Experimental.GraphView.Edge
            {
                input = input.port,
                output = output.port
            };

            //Connect the edge to the input and output ports.
            if (tempEdge != null)
            {
                tempEdge.input.Connect(tempEdge);
                tempEdge.output.Connect(tempEdge);
            }

            //Add the connection to the graph.
            targetGraph.Add(tempEdge);
#endif

            input.AddConnection(output);
            output.AddConnection(input);
        }

#if (UNITY_EDITOR)
        private GraphData GetCurrentlyLoadedGraphData()
        {
            //If no TerraTiler2D graph is currently open
            if (Graph.Instance == null)
            {
                return null;
            }

            //Get the open window
            Graph targetGraph = Graph.Instance;

            //If the currently loaded graph data was already found
            if (currentlyLoadedGraphData != null)
            {
                //If the file names match
                if (currentlyLoadedGraphData.GetFileName() == targetGraph.GetFileName())
                {
                    return currentlyLoadedGraphData;
                }
            }

            //Get all the assets with the same name and type as this graph.
            string[] assetPaths = AssetDatabase.FindAssets(targetGraph.GetFileName() + " t:GraphData");

            //For all the assets that match the search criteria
            for (int i = 0; i < assetPaths.Length; i++)
            {
                //Get the asset path
                string path = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
                //If the file name of the asset matches the GraphData we are looking for
                if (path.Split('/')[path.Split('/').Length-1] == (targetGraph.GetFileName() + ".asset"))
                {
                    //Load the GraphData
                    currentlyLoadedGraphData = AssetDatabase.LoadAssetAtPath<GraphData>(path);

                    //and return the current GraphData object
                    return currentlyLoadedGraphData;
                }
            }

            return null;
        }

        private void HandleEdgeConnected(EdgeConnectedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandleNodeLink(evt.nodeLink, GraphData.DataOperation.Add);
            }
        }
        private void HandleEdgeDisconnected(EdgeDisconnectedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandleNodeLink(evt.nodeLink, GraphData.DataOperation.Remove);
            }
        }

        private void HandleNodeCreated(NodeCreatedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandleNodeData(evt.nodeData, GraphData.DataOperation.Add);
            }
        }
        private void HandleNodeChanged(NodeChangedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandleNodeData(evt.nodeData, GraphData.DataOperation.Change);
            }
        }
        private void HandleNodeDeleted(NodeDeletedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandleNodeData(evt.nodeData, GraphData.DataOperation.Remove);
            }
        }

        private void HandlePortCreated(PortCreatedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandlePortData(evt.portData, GraphData.DataOperation.Add);
            }
        }
        private void HandlePortChanged(PortChangedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandlePortData(evt.portData, GraphData.DataOperation.Change);
            }
        }
        private void HandlePortDeleted(PortDeletedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandlePortData(evt.portData, GraphData.DataOperation.Remove);
            }
        }

        private void HandlePropertyCreated(PropertyCreatedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandlePropertyData(evt.propertyData, GraphData.DataOperation.Add);
            }
        }
        private void HandlePropertyChanged(PropertyChangedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandlePropertyData(evt.propertyData, GraphData.DataOperation.Change);
            }
        }
        private void HandlePropertyDeleted(PropertyDeletedEvent evt)
        {
            if (GetTempGraphData() != null)
            {
                GetTempGraphData().HandlePropertyData(evt.propertyData, GraphData.DataOperation.Remove);
            }
        }
#endif
    }
}
