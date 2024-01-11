using UnityEngine;

namespace TerraTiler2D
{
    public class AddLayerToWorld_Node : Flow_Node
    {
        private Port<World> worldInputPort;
        private Port<TileLayer> tileLayerPort;
        private PortWithField<Vector2> positionPort;
        private Port<int> zIndexPort;
        private PortWithField<int> collisionLayerPort;

        private Port<World> worldOutputPort;

        private NodePreview nodePreview;

        public AddLayerToWorld_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.AddLayerToWorld;
            SetTooltip("Adds a TileLayer to a World object.");
            searchMenuEntry = new string[] { "Flow" };
            nodePreview = new NodePreview(this, GetPreviewTexture);
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            worldInputPort = GeneratePort<World>("World", PortDirection.Input, "World", PortCapacity.Single, true, "The World to add the TileLayer to.");
            tileLayerPort = GeneratePort<TileLayer>("Tile layer", PortDirection.Input, "TileLayer", PortCapacity.Single, true, "The TileLayer to add to the World.");
            positionPort = GeneratePortWithField<Vector2>("Position", PortDirection.Input, Vector2.zero, "Pos", PortCapacity.Single, false, "The position of the TileLayer in the World.");
            zIndexPort = GeneratePortWithField<int>("Z index", PortDirection.Input, 0, "zIndex", PortCapacity.Single, false, "The Z index of the TileLayer in the world. A TileLayer with a high Z index will be placed in front of TileLayers with a lower Z index.");
            collisionLayerPort = GeneratePortWithField<int>("Collision Layer", PortDirection.Input, 0, "CollLayer", PortCapacity.Single, false, "The collision layer of this TileLayer.");
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();

            //Create and add an output port to the node
            worldOutputPort = GeneratePort<World>("World", PortDirection.Output, "WorldOutput", PortCapacity.Multi, false);
            worldOutputPort.SetOutputPortMethod(GetWorldOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            World inputWorld = (World)worldInputPort.GetPortVariable();

            inputWorld.AddTileLayer(
                (TileLayer)tileLayerPort.GetPortVariable(), 
                (Vector2)positionPort.GetPortVariable(), 
                (int)zIndexPort.GetPortVariable(), 
                (int)collisionLayerPort.GetPortVariable()
            );

            if (nodePreview.ShouldPreview() && flow.direction == Flow.Direction.Forwards)
            {
                nodePreview.ShowTexture(inputWorld.GetWorldPreviewTexture());
            }

            base.ApplyBehaviour(flow, trickleDown);
        }

        private Texture2D GetPreviewTexture()
        {
#if (UNITY_EDITOR)
            //Run the graph up to this node
            ApplyBehaviour(new Flow(Flow.Direction.Backwards), false);

            return ((World)GetWorldOutput()).GetWorldPreviewTexture();
#else
            return null;
#endif
        }

        public object GetWorldOutput()
        {
            World inputWorld = (World)worldInputPort.GetPortVariable();

            return inputWorld;
        }

        //This node has a NodePreview, so we need to save a Preview_NodeData instead of the standard NodeData
        public override NodeData GetNodeData(NodeData nodeData = null)
        {
            return base.GetNodeData(new Preview_NodeData()
            {
                PreviewToggle = nodePreview.ShouldPreview()
            });
        }
        //This node has a NodePreview, so we need to load a Preview_NodeData instead of the standard NodeData, and apply the extra data to the NodePreview
        public override void LoadNodeData(NodeData data)
        {
            Preview_NodeData nodeData = data as Preview_NodeData;

            if (nodeData != null && nodePreview != null)
            {
                nodePreview.SetShouldPreview(nodeData.PreviewToggle);
            }
        }
    }
}
