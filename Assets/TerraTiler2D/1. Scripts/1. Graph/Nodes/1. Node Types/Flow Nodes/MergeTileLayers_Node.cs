using UnityEngine;

namespace TerraTiler2D
{
    public class MergeTileLayers_Node : FlowTileLayer_Node
    {
        private Port<TileLayer> tileLayerToMergePort;
        private PortWithField<Vector2> positionPort;

        public MergeTileLayers_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.MergeTileLayers;
            SetTooltip("Merges two TileLayers.");
            searchMenuEntry = new string[] { "Flow" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            tileLayerToMergePort = GeneratePort<TileLayer>("Tile layer to merge", PortDirection.Input, "TileLayerToMerge", PortCapacity.Single, true, "The TileLayer to merge.");
            positionPort = GeneratePortWithField<Vector2>("Position", PortDirection.Input, Vector2.zero, "Pos", PortCapacity.Single, false, "Where to position the TileLayer, relative to the bottom left corner of the original TileLayer.");
        }

        public override TileLayer ApplyBehaviourOnTileLayer(TileLayer tileLayer)
        {
            TileLayer tileLayer1 = (TileLayer)tileLayer.Clone();
            TileLayer tileLayer2 = (TileLayer)((TileLayer)tileLayerToMergePort.GetPortVariable()).Clone();

            Vector2 position = (Vector2)positionPort.GetPortVariable();

            if (!tileLayer1.Merge(tileLayer2, position))
            {
                return tileLayer;
            }

            return tileLayer1;
        }
    }
}
