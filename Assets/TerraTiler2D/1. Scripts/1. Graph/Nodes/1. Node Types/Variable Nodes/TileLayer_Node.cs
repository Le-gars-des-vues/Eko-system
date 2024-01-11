using UnityEngine;

namespace TerraTiler2D
{
    /// <summary>
    /// Starting point of the graph. Gets created automatically.  	
    /// </summary>
    public class TileLayer_Node : Node
    {
        private Port<TileLayer> outputPort;

        private PortWithField<Vector2> layerSizePort;
        private PortWithField<Vector2> tileSizePort;

        private TileLayer outputTileLayer;

        //========== Initialization ==========

        public TileLayer_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TileLayer;
            SetTooltip("Creates a new empty layer of tiles. These layers can be added to a World object to create worlds with multiple layers. Once this node has created a TileLayer, it will return a reference to that same TileLayer on subsequent calls.");
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeAdditionalElements()
        {

        }

        protected override void InitializeInputPorts()
        {
            //Create an output port.
            layerSizePort = GeneratePortWithField<Vector2>("Layer size", PortDirection.Input, new Vector2(100, 100), "LayerSize", PortCapacity.Single, false, "The size of the layer in tiles.");

            //Create an output port.
            tileSizePort = GeneratePortWithField<Vector2>("Tile size", PortDirection.Input, new Vector2(16, 16), "TileSize", PortCapacity.Single, false, "The size of each individual tile in pixels.");
        }

        protected override void InitializeOutputPorts()
        {
            //Create an output port.
            outputPort = GeneratePort<TileLayer>("Tile layer", PortDirection.Output, "TileLayerOutput", PortCapacity.Multi, false, "New empty tile layer. Will return a reference to the same TileLayer on subsequent calls.");
            outputPort.SetOutputPortMethod(GetLayerOutput);
        }

        //========== Node methods ==========


        //========== Port data passing ==========

        public object GetLayerOutput()
        {
            //If an output has already been generated
            if (outputTileLayer != null)
            {
                //Return the saved results
                return outputTileLayer;
            }

            Vector2 layerSize = (Vector2)layerSizePort.GetPortVariable();

            Vector2 tileSize = (Vector2)tileSizePort.GetPortVariable();

            outputTileLayer = new TileLayer(layerSize, tileSize);

            return outputTileLayer;
        }

        public override void ResetNodeVariables()
        {
            outputTileLayer = null;

            base.ResetNodeVariables();
        }

        //========== NodeData saving and loading ==========

    }
}
