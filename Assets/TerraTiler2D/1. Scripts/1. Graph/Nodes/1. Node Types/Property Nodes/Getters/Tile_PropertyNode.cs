using UnityEngine;
using UnityEngine.Tilemaps;

namespace TerraTiler2D
{
    public class Tile_PropertyNode : Property_Node<TileBase>
    {
        public Tile_PropertyNode(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TileProperty;
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();
        }

        protected override void InitializeAdditionalElements()
        {

        }
    }
}
