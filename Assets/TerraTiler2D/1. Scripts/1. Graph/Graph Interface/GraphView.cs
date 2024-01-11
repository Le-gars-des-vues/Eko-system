using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
#if (UNITY_EDITOR)
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace TerraTiler2D
{
    public class GraphView
#if (UNITY_EDITOR)
    : UnityEditor.Experimental.GraphView.GraphView
#endif
    {
#if (!UNITY_EDITOR)
        private List<Node> allNodes = new List<Node>();
#endif

        private Blackboard myBlackboard;

#if (UNITY_EDITOR)
        private NodeSearchWindow mySearchWindow;

        public GraphView(EditorWindow editorWindow)
        {
            //Load the style sheet.
            //Use the .uss files in the resources folder to edit colors, shapes, etc.
            styleSheets.Add(Glob.GetInstance().GraphStyleSheet);
            //Enable zooming for the graph.
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            //Enable dragging and selecting of objects.
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(new ContextualMenuManipulator(BuildContextualGraphViewMenu));

            //Add a background grid. To customize this grid, use the .uss files in the resources folder.
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            //Handle key down events.
            RegisterCallback<KeyDownEvent>(HandleKeyDownEvents);

            if (editorWindow != null)
            {
                //Add a search window to add different nodes that appears when you right click.
                AddSearchWindow(editorWindow);

                //Stretch the graph.
                this.StretchToParentSize();
                //Add the graph to this window.
                editorWindow.rootVisualElement.Add(this);
            }
        }
#endif
        public GraphView()
        {

        }

        public GraphOutput RunGraph()
        {
            //Get the entry node.
            Entry_Node entryNode = GetAllNodes().Find(x => x.nodeType is Glob.NodeTypes.Entry) as Entry_Node;

            return entryNode.RunGraph(GetGraphBlackboard().GetProperties());
        }

#if (UNITY_EDITOR)
        public bool RemoveGraphView()
        {
            RemoveFromHierarchy();

            return true;
        }

        //Get all the compatible ports for startPort.
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            //Get all the ports that are:
            //  not part to the same node as the startPort, 
            //  and that do not have the same direction as the startPort,
            //  and that have the same type as the startPort.
            //  and that are not already connected to the startPort.
            var ttPorts = this.Query<Port_Abstract>();

            ttPorts.ForEach(port =>
            {
                //Is not part of the same node as the startPort 
                if (startPort.node != port.port.node)                           
                {
                    //Does not have the same direction as the startPort
                    if (port.port.direction != startPort.direction)             
                    {
                        //Has the same type as the startPort
                        if (port.port.portType == startPort.portType)           
                        {
                            compatiblePorts = AddCompatiblePort(compatiblePorts, port.port, startPort);
                        }
                        //Is either port is an array type
                        else if (Glob.GetInstance().IsList(startPort.portType) || Glob.GetInstance().IsList(port.port.portType))                    
                        {
                            //If both ports are an array type
                            if (Glob.GetInstance().IsList(startPort.portType) && Glob.GetInstance().IsList(port.port.portType))
                            {
                                //If either port is of type 'object[]'
                                if (port.port.portType == typeof(List<object>) ||       
                                    startPort.portType == typeof(List<object>))
                                {
                                    compatiblePorts = AddCompatiblePort(compatiblePorts, port.port, startPort);
                                }
                            }
                        }
                        //If either port is of type 'object'
                        else if (port.port.portType == typeof(object) ||
                                startPort.portType == typeof(object))       
                        {
                            //If neither port is a Flow port
                            if (port.port.portType != typeof(Flow) && startPort.portType != typeof(Flow))     
                            {
                                //If either port is of type 'Flow', it is not compatible. We do not want Flow ports to connect to object ports.
                                compatiblePorts = AddCompatiblePort(compatiblePorts, port.port, startPort);
                            }
                        }
                    }
                }
            });

            return compatiblePorts;
        }
        private List<Port> AddCompatiblePort(List<Port> compatiblePorts, Port port, Port startPort)
        {
            if (compatiblePorts.Contains(port))
            {
                return compatiblePorts;
            }
            //If the port has a connection
            if (port.connected)
            {
                //Get any connection between this port and the startPort
                var edgeConnectedToStartPort = port.connections.FirstOrDefault(x => x != null && (x.input == startPort || x.output == startPort));
                //If there is no connection to the startPort yet
                if (edgeConnectedToStartPort == null)
                {
                    //This is a compatible port
                    compatiblePorts.Add(port);
                }
            }
            else
            {
                //This is a compatible port
                compatiblePorts.Add(port);
            }

            return compatiblePorts;
        }

        //Add a search window to add different nodes that appears when you right click.
        private void AddSearchWindow(EditorWindow editorWindow)
        {
            if (mySearchWindow == null)
            {
                mySearchWindow = new NodeSearchWindow();
            }

            nodeCreationRequest = context =>
                mySearchWindow.Open(new SearchWindowContext(context.screenMousePosition), (Graph)editorWindow);
        }
        public NodeSearchWindow GetSearchWindow()
        {
            return mySearchWindow;
        }

        public Vector2 GetGraphViewCenter()
        {
            return (((Vector2)(-viewTransform.position)) + (localBound.size / 2)) / viewTransform.scale;
        }
#endif

        public void AddBlackboard(Blackboard blackboard)
        {
#if (UNITY_EDITOR)
            this.Add(blackboard);
#endif
            this.myBlackboard = blackboard;
        }
        public Blackboard GetGraphBlackboard()
        {
            if (myBlackboard == null)
            {
                AddBlackboard(new Blackboard(this, Glob.GetInstance().BlackboardSize, new Vector2(10, 30)));
            }
            return myBlackboard;
        }

        //Create and add a node to the graph.
        public Node CreateNode(Glob.NodeTypes nodeType, Vector2 position, string nodeName = null, string guid = null)
        {
            //If no name was passed into this method, use the default node names.
            if (string.IsNullOrEmpty(nodeName) && Glob.GetInstance().DefaultNodeNames.ContainsKey(nodeType))
            {
                nodeName = Glob.GetInstance().DefaultNodeNames[nodeType];
            }

            Node newNode = null;

            switch (nodeType)
            {
                case Glob.NodeTypes.Entry:
                    //If this is an already existing entry node
                    if (!string.IsNullOrEmpty(guid))
                    {
                        //If there is already an entry node in the graph
                        if (GetAllNodes().Exists(x => x.nodeType == Glob.NodeTypes.Entry))
                        {
                            //If an entry node is created manually, and not being loaded from a GraphData object.
                            if (guid == null)
                            {
                                Glob.GetInstance().DebugString("There can only be one entry node. Destroying the old entry node, and creating a new one.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                            }
                            var existingEntryNode = GetAllNodes().Find(x => x.nodeType == Glob.NodeTypes.Entry);
                            EventManager.GetInstance().RaiseEvent(new NodeDeletedEvent().Init(existingEntryNode.GetNodeData()));
#if (UNITY_EDITOR)
                            existingEntryNode.RemoveFromHierarchy();
#else
                            allNodes.Remove(existingEntryNode);
#endif
                        }
                    }
                    //If there is already an entry node in the graph
                    else if (GetAllNodes().Exists(x => x.nodeType == Glob.NodeTypes.Entry))
                    {
                        //Do not make a new one
                        return null;
                    }

                    if (string.IsNullOrEmpty(guid))
                    {
#if (UNITY_EDITOR)
                        guid = GUID.Generate().ToString();
#endif
                    }

                    newNode = new Entry_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.Int:
                    newNode = new Int_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntBetween:
                    newNode = new IntBetween_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Float:
                    newNode = new Float_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatBetween:
                    newNode = new FloatBetween_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Bool:
                    newNode = new Bool_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2:
                    newNode = new Vector2_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3:
                    newNode = new Vector3_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4:
                    newNode = new Vector4_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.String:
                    newNode = new String_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Color:
                    newNode = new Color_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Gradient:
                    newNode = new Gradient_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Rect:
                    newNode = new Rect_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Tile:
                    newNode = new Tile_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.TileLayer:
                    newNode = new TileLayer_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.World:
                    newNode = new World_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.ToString:
                    newNode = new ToString_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntToFloat:
                    newNode = new IntToFloat_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.RoundToInt:
                    newNode = new RoundToInt_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloorToInt:
                    newNode = new FloorToInt_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.CeilToInt:
                    newNode = new CeilToInt_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.BreakVector2:
                    newNode = new BreakVector2_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.BreakVector3:
                    newNode = new BreakVector3_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.BreakVector4:
                    newNode = new BreakVector4_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2ToVector3:
                    newNode = new Vector2ToVector3_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2ToVector4:
                    newNode = new Vector2ToVector4_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3ToVector2:
                    newNode = new Vector3ToVector2_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3ToVector4:
                    newNode = new Vector3ToVector4_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4ToVector2:
                    newNode = new Vector4ToVector2_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4ToVector3:
                    newNode = new Vector4ToVector3_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.IntAdd:
                    newNode = new IntAdd_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntSubtract:
                    newNode = new IntSubtract_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntMultiply:
                    newNode = new IntMultiply_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntDivide:
                    newNode = new IntDivide_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntModulo:
                    newNode = new IntModulo_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntPow:
                    newNode = new IntPow_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntEquals:
                    newNode = new IntEquals_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntGreaterThan:
                    newNode = new IntGreaterThan_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntEqualsOrGreaterThan:
                    newNode = new IntEqualsOrGreaterThan_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntSqrt:
                    newNode = new IntSqrt_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntClamp:
                    newNode = new IntClamp_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntMin:
                    newNode = new IntMin_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntMax:
                    newNode = new IntMax_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntCos:
                    newNode = new IntCos_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntSin:
                    newNode = new IntSin_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IntTan:
                    newNode = new IntTan_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.FloatAdd:
                    newNode = new FloatAdd_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatSubtract:
                    newNode = new FloatSubtract_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatMultiply:
                    newNode = new FloatMultiply_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatDivide:
                    newNode = new FloatDivide_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatModulo:
                    newNode = new FloatModulo_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatPow:
                    newNode = new FloatPow_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatEquals:
                    newNode = new FloatEquals_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatGreaterThan:
                    newNode = new FloatGreaterThan_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatEqualsOrGreaterThan:
                    newNode = new FloatEqualsOrGreaterThan_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatSqrt:
                    newNode = new FloatSqrt_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatClamp:
                    newNode = new FloatClamp_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatMin:
                    newNode = new FloatMin_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatMax:
                    newNode = new FloatMax_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatCos:
                    newNode = new FloatCos_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatSin:
                    newNode = new FloatSin_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.FloatTan:
                    newNode = new FloatTan_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.Vector2Add:
                    newNode = new Vector2Add_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2Subtract:
                    newNode = new Vector2Subtract_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2Multiply:
                    newNode = new Vector2Multiply_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2Divide:
                    newNode = new Vector2Divide_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2Length:
                    newNode = new Vector2Length_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2SetLength:
                    newNode = new Vector2SetLength_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2Clamp:
                    newNode = new Vector2Clamp_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2Min:
                    newNode = new Vector2Min_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2Max:
                    newNode = new Vector2Max_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector2Dot:
                    newNode = new Vector2Dot_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.Vector3Add:
                    newNode = new Vector3Add_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3Subtract:
                    newNode = new Vector3Subtract_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3Multiply:
                    newNode = new Vector3Multiply_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3Divide:
                    newNode = new Vector3Divide_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3Length:
                    newNode = new Vector3Length_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3SetLength:
                    newNode = new Vector3SetLength_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3Clamp:
                    newNode = new Vector3Clamp_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3Min:
                    newNode = new Vector3Min_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3Max:
                    newNode = new Vector3Max_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector3Dot:
                    newNode = new Vector3Dot_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.Vector4Add:
                    newNode = new Vector4Add_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4Subtract:
                    newNode = new Vector4Subtract_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4Multiply:
                    newNode = new Vector4Multiply_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4Divide:
                    newNode = new Vector4Divide_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4Length:
                    newNode = new Vector4Length_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4SetLength:
                    newNode = new Vector4SetLength_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4Clamp:
                    newNode = new Vector4Clamp_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4Min:
                    newNode = new Vector4Min_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4Max:
                    newNode = new Vector4Max_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Vector4Dot:
                    newNode = new Vector4Dot_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.PI:
                    newNode = new PI_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.TilePlacerNoisemap:
                    newNode = new TilePlacerNoisemap_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.TilePlacerRectangles:
                    newNode = new TilePlacerRectangles_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.TilePlacerEllipses:
                    newNode = new TilePlacerEllipses_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.TilePlacerTriangles:
                    newNode = new TilePlacerTriangles_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.TilePlacerTunnelerAStar:
                    newNode = new TilePlacerTunnelerAStar_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.TilePlacerTunnelerShortestPath:
                    newNode = new TilePlacerTunnelerShortestPath_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.SetSeed:
                    newNode = new SetSeed_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.AddLayerToWorld:
                    newNode = new AddLayerToWorld_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.MergeTileLayers:
                    newNode = new MergeTileLayers_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.NoisemapPerlin:
                    newNode = new NoisemapPerlin_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.NoisemapValue:
                    newNode = new NoisemapValue_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.NoisemapGradient:
                    newNode = new NoisemapGradient_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.NoisemapRandom:
                    newNode = new NoisemapRandom_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.Texture2D:
                    newNode = new Texture2D_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.CreateTexture2D:
                    newNode = new CreateTexture2D_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.MultiplyTexture2D:
                    newNode = new MultiplyTexture2D_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.ExtractTexture2D:
                    newNode = new ExtractTexture2D_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.InverseTexture2D:
                    newNode = new InverseTexture2D_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.ClampTexture2D:
                    newNode = new ClampTexture2D_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.SetColorTexture2D:
                    newNode = new SetColorTexture2D_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.GetSizeTexture2D:
                    newNode = new GetSizeTexture2D_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.SetSizeTexture2D:
                    newNode = new SetSizeTexture2D_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.TileMask:
                    newNode = new TileMask_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.NoisemapDimension:
                    newNode = new NoisemapDimension_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.IntProperty:
                    newNode = new Int_PropertyNode(nodeName, position, guid);
                    (newNode as Int_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.IntPropertySet:
                    newNode = new Int_PropertySetNode(nodeName, position, guid);
                    (newNode as Int_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.FloatProperty:
                    newNode = new Float_PropertyNode(nodeName, position, guid);
                    (newNode as Float_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.FloatPropertySet:
                    newNode = new Float_PropertySetNode(nodeName, position, guid);
                    (newNode as Float_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.BoolProperty:
                    newNode = new Bool_PropertyNode(nodeName, position, guid);
                    (newNode as Bool_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.BoolPropertySet:
                    newNode = new Bool_PropertySetNode(nodeName, position, guid);
                    (newNode as Bool_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.Vector2Property:
                    newNode = new Vector2_PropertyNode(nodeName, position, guid);
                    (newNode as Vector2_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.Vector2PropertySet:
                    newNode = new Vector2_PropertySetNode(nodeName, position, guid);
                    (newNode as Vector2_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.Vector3Property:
                    newNode = new Vector3_PropertyNode(nodeName, position, guid);
                    (newNode as Vector3_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.Vector3PropertySet:
                    newNode = new Vector3_PropertySetNode(nodeName, position, guid);
                    (newNode as Vector3_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.Vector4Property:
                    newNode = new Vector4_PropertyNode(nodeName, position, guid);
                    (newNode as Vector4_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.Vector4PropertySet:
                    newNode = new Vector4_PropertySetNode(nodeName, position, guid);
                    (newNode as Vector4_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.StringProperty:
                    newNode = new String_PropertyNode(nodeName, position, guid);
                    (newNode as String_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.StringPropertySet:
                    newNode = new String_PropertySetNode(nodeName, position, guid);
                    (newNode as String_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.ColorProperty:
                    newNode = new Color_PropertyNode(nodeName, position, guid);
                    (newNode as Color_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.ColorPropertySet:
                    newNode = new Color_PropertySetNode(nodeName, position, guid);
                    (newNode as Color_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.GradientProperty:
                    newNode = new Gradient_PropertyNode(nodeName, position, guid);
                    (newNode as Gradient_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.GradientPropertySet:
                    newNode = new Gradient_PropertySetNode(nodeName, position, guid);
                    (newNode as Gradient_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.Texture2DProperty:
                    newNode = new Texture2D_PropertyNode(nodeName, position, guid);
                    (newNode as Texture2D_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.Texture2DPropertySet:
                    newNode = new Texture2D_PropertySetNode(nodeName, position, guid);
                    (newNode as Texture2D_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.TileProperty:
                    newNode = new Tile_PropertyNode(nodeName, position, guid);
                    (newNode as Tile_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.TilePropertySet:
                    newNode = new Tile_PropertySetNode(nodeName, position, guid);
                    (newNode as Tile_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.WorldProperty:
                    newNode = new World_PropertyNode(nodeName, position, guid);
                    (newNode as World_PropertyNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;
                case Glob.NodeTypes.WorldPropertySet:
                    newNode = new World_PropertySetNode(nodeName, position, guid);
                    (newNode as World_PropertySetNode).SetProperty(GetGraphBlackboard().GetProperty(nodeName));
                    break;

                case Glob.NodeTypes.IfBranch:
                    newNode = new IfBranch_Node(nodeName, position, guid);
                    break;
                //case Glob.NodeTypes.SwitchBranch:
                //    newNode = new SwitchBranch_Node(nodeName, position, guid);
                //    break;
                case Glob.NodeTypes.ForLoop:
                    newNode = new ForLoop_Node(nodeName, position, guid);
                    break;
                //case Glob.NodeTypes.ForEachLoop:
                //    newNode = new ForEachLoop_Node(nodeName, position, guid);
                //    break;
                case Glob.NodeTypes.WhileLoop:
                    newNode = new WhileLoop_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Redirect:
                    newNode = new Redirect_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.Array:
                    newNode = new Array_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.GetFromArray:
                    newNode = new GetFromArray_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.AddToArray:
                    newNode = new AddToArray_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.ArrayLength:
                    newNode = new ArrayLength_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.GetArrayIndex:
                    newNode = new GetArrayIndex_Node(nodeName, position, guid);
                    break;
                case Glob.NodeTypes.IsNull:
                    newNode = new IsNull_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.DebugLog:
                    newNode = new DebugLog_Node(nodeName, position, guid);
                    break;

                case Glob.NodeTypes.Default:
                    break;
                default:
                    Glob.GetInstance().DebugString("Invalid node type " + nodeType + "! Did you forget to add this node type to this switch statement?", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    break;
            }

            if (newNode != null)
            {

#if (UNITY_EDITOR)
                AddElement(newNode);
#else
                allNodes.Add(newNode);
#endif

                NodeData newNodeData = newNode.GetNodeData();
                newNodeData.GUID = newNode.Guid;
                newNodeData.NodeName = nodeName;
                newNodeData.Position = position;
                newNodeData.NodeType = nodeType;
                EventManager.GetInstance().RaiseEvent(new NodeCreatedEvent().Init(newNodeData));
            }
            else
            {
                Glob.GetInstance().DebugString("Failed to create a node of type '" + nodeType + "' with GUID '" + guid + "'.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Error);
            }

            return newNode;
        }

        public List<Node> GetAllNodes()
        {
#if (UNITY_EDITOR)
            return nodes.ToList().Cast<Node>().ToList();
#else
            return allNodes;
#endif
        }
        public void ResetNodeVariables()
        {
            foreach (Node node in GetAllNodes())
            {
                node.ResetNodeVariables();
            }
        }
        public void ResetBlackboardProperties()
        {
            if (GetGraphBlackboard() !=  null)
            {
                List<Blackboard_Property_Abstract> properties = GetGraphBlackboard().GetProperties();
                foreach (Blackboard_Property_Abstract property in properties)
                {
                    property.ResetProperty();
                }
            }
        }

        public void StopGraph()
        {
            foreach (Node node in GetAllNodes())
            {
                node.StopNodeExecution();
            }
        }

#if (UNITY_EDITOR)
        private void BuildContextualGraphViewMenu(ContextualMenuPopulateEvent evt)
        {
            List<DropdownMenuItem> items = evt.menu.MenuItems();
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is DropdownMenuAction)
                {
                    if (((DropdownMenuAction)items[i]).name == "Delete" ||
                        ((DropdownMenuAction)items[i]).name == "Cut" ||
                        ((DropdownMenuAction)items[i]).name == "Copy" ||
                        ((DropdownMenuAction)items[i]).name == "Paste" ||
                        ((DropdownMenuAction)items[i]).name == "Duplicate")
                    {
                        evt.menu.RemoveItemAt(i);
                    }
                }
                else
                {
                    evt.menu.RemoveItemAt(i);
                }
            }

            DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal;
            if (GetSelection().Count <= 0)
            {
                status = DropdownMenuAction.Status.Disabled;
            }

            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Delete", x => { DeleteGraphElements(GetSelection()); }, status);
            evt.menu.AppendAction("Cut", x => { AddSelectionToClipboard(); DeleteGraphElements(GetSelection()); }, status);
            evt.menu.AppendAction("Copy", x => { AddSelectionToClipboard(); }, status);
            evt.menu.AppendAction("Paste", x => { CopyGraphElements(ClipboardManager.GetInstance().GetClipboard()); });
            evt.menu.AppendAction("Duplicate", x => { CopyGraphElements(GetSelection()); }, status);
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Disconnect all", x => { DisconnectGraphElements(GetSelection()); }, status);
        }

        //Handle key down events.
        private void HandleKeyDownEvents(KeyDownEvent evt)
        {
            EventManager.GetInstance().RaiseEvent(new KeyDownInGraphEvent().Init(evt));

            //If the delete button is pressed (Delete)
            if (evt.keyCode == KeyCode.Delete)
            {
                List<GraphElement> elements = GetSelection();

                DeleteGraphElements(elements);

                //Prevent any default behaviour tied to the Escape key being pressed down.
                evt.StopImmediatePropagation();
            }
            //If CTRL + C is pressed (Copy)
            else if (evt.keyCode == KeyCode.C && evt.ctrlKey)
            {
                //Add the selection to the clipboard
                AddSelectionToClipboard();
            }
            //If CTRL + V is pressed (Paste)
            else if (evt.keyCode == KeyCode.V && evt.ctrlKey)
            {
                //Copy all the graph elements in the clipboard
                CopyGraphElements(ClipboardManager.GetInstance().GetClipboard());
            }
            //If CTRL + X is pressed (Cut)
            else if (evt.keyCode == KeyCode.X && evt.ctrlKey)
            {
                AddSelectionToClipboard();

                List<GraphElement> elements = GetSelection();

                DeleteGraphElements(elements);
            }
            //If CTRL + D is pressed (Duplicate)
            else if (evt.keyCode == KeyCode.D && evt.ctrlKey)
            {
                List<GraphElement> elements = GetSelection();

                //Copy all the selected elements
                CopyGraphElements(elements);
            }
        }

        private List<GraphElement> GetSelection()
        {
            //Create an empty list of GraphElements
            List<GraphElement> elements = new List<GraphElement>();
            //Store all the currently selected graph elements in the list. This is needed to prevent the selection list from changing while removing elements.
            foreach (var item in selection)
            {
                elements.Add(item as GraphElement);
            }

            return elements;
        }
        private void AddSelectionToClipboard()
        {
            List<GraphElement> elements = GetSelection();

            //Add all the items to the clipboard.
            ClipboardManager.GetInstance().SetClipboard(elements);
        }
        private void DeleteGraphElements(List<GraphElement> elements)
        {
            //Remove all the selected elements from the graph
            foreach (GraphElement item in elements)
            {
                //If the item is an Edge
                if (item is UnityEditor.Experimental.GraphView.Edge)
                {
                    //Disconnect the edge, and remove it from the graph view
                    DisconnectEdge(item as UnityEditor.Experimental.GraphView.Edge);
                }
                //If the item is a Node
                else if (item is Node)
                {
                    //Is the Node copiable
                    if (item.IsCopiable())
                    {
                        //Delete the Node
                        ((Node)item).DeleteNode();
                    }
                }
                //Any other element
                else
                {
                    //Delete
                    RemoveElement(item);
                }
            }
        }
        private List<GraphElement> CopyGraphElements(List<GraphElement> elements)
        {
            //Clear the selection, so that we can add the newly created elements to the selection
            ClearSelection();

            //Create an empty return list
            List<GraphElement> newElements = new List<GraphElement>();

            //Add a list to hold all new nodes, which can be used to connect them afterwards.
            List<Node> newNodes = new List<Node>();
            //A dictionary that handles cross-references between the original node and their copy
            Dictionary<string, string> oldGuids = new Dictionary<string, string>();
            
            //Empty vector2 to hold the center position
            Vector2 centerPos = Vector2.zero;

            //Copy all the nodes in the list
            foreach (var item in elements)
            {
                //For every node in the list
                if (item is Node)
                {
                    Node node = item as Node;
                    //If the node is copiable
                    if (node.IsCopiable())
                    {
                        //If the center position has not been initialized yet
                        if (centerPos == Vector2.zero)
                        {
                            //Set this node as the center
                            centerPos = node.GetNodePosition().position;
                        }

                        //Spawn this position in the center of the graph view, but offset it based on the node offset of the original node
                        Vector2 nodePosition = GetGraphViewCenter() + (node.GetNodePosition().position - centerPos);
                        //Give the new node a new guid
                        string newGuid = GUID.Generate().ToString();
                        //Create a new node using the values of the original node
                        Node newNode = CreateNode(node.nodeType, nodePosition, node.title, newGuid);
                        newNode.CopyPortValues(node);

                        //Add the new node to both lists
                        newNodes.Add(newNode);
                        newElements.Add(newNode);
                        //Store the cross-reference between the original node and the new node
                        oldGuids.Add(newGuid, node.Guid);

                        //Add the new node to the selection
                        AddToSelection(newNode);
                    }
                }
            }

            //Copy all the edges in the list, and connect the new nodes
            foreach (var item in elements)
            {
                //If the item is an Edge
                if (item is UnityEditor.Experimental.GraphView.Edge)
                {
                    UnityEditor.Experimental.GraphView.Edge edge = item as UnityEditor.Experimental.GraphView.Edge;

                    //Get the new input and output nodes by using the cross-references saved earlier
                    Node inputNode = newNodes.Find(x => oldGuids[x.Guid] == (edge.input.node as Node).Guid);
                    Node outputNode = newNodes.Find(x => oldGuids[x.Guid] == (edge.output.node as Node).Guid);

                    if (inputNode == null || outputNode == null)
                    {
                        continue;
                    }

                    //Get the new input and output ports by comparing GUID length.
                    Port_Abstract inputPort = inputNode.GetPorts(PortDirection.Input).Find(x => x.Guid.Split('_')[1] == (edge.input.parent as Port_Abstract).Guid.Split('_')[1]);
                    Port_Abstract outputPort = outputNode.GetPorts(PortDirection.Output).Find(x => x.Guid.Split('_')[1] == (edge.output.parent as Port_Abstract).Guid.Split('_')[1]);

                    //Create a new edge, and connect it to the input and output port
                    UnityEditor.Experimental.GraphView.Edge newEdge = ConnectEdge(inputPort, outputPort);

                    //Add the new edge to the return list
                    newElements.Add(newEdge);

                    //Add the edge to the selection
                    selection.Add(newEdge);
                    //Tell the edge it is selected, so that it shows the correct color
                    newEdge.selected = true;
                }
            }
            //Return the newly created elements
            return newElements;
        }
        private void DisconnectGraphElements(List<GraphElement> elements)
        {
            //For each graph element
            foreach (GraphElement item in elements)
            {
                //If the item is a Node
                if (item is Node)
                {
                    //Tell the Node to disconnect all ports
                    ((Node)item).DisconnectAllPorts();
                }
            }
        }

        private UnityEditor.Experimental.GraphView.Edge ConnectEdge(Port_Abstract input, Port_Abstract output)
        {
            var tempEdge = new UnityEditor.Experimental.GraphView.Edge
            {
                input = input.port,
                output = output.port
            };
            //Connect the edge to the input and output ports.
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);

            //Add the connection to the graph.
            Add(tempEdge);

            input.AddConnection(output);
            output.AddConnection(input);

            return tempEdge;
        }
        private void DisconnectEdge(UnityEditor.Experimental.GraphView.Edge edge)
        {

            (edge.input.parent as Port_Abstract).HandleDisconnected(edge);
            (edge.output.parent as Port_Abstract).HandleDisconnected(edge);

            edge.RemoveFromHierarchy();
        }
#endif
    }
}