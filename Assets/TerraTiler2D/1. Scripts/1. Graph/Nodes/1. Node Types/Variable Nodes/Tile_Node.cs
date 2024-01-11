using UnityEngine;
using UnityEngine.Tilemaps;

namespace TerraTiler2D
{
    public class Tile_Node : Node
    {
        private PortWithField<TileBase> tilePort;

        public Tile_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Tile;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            tilePort = GeneratePortWithField<TileBase>("", PortDirection.Output, null, "TileOutput", PortCapacity.Multi, false);
        }

        protected override void InitializeAdditionalElements()
        {

        }
    }
}
