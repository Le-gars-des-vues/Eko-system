using UnityEngine;
using UnityEngine.Tilemaps;

namespace TerraTiler2D
{
    public class Tile_PropertySetNode : PropertySetter_Node<TileBase>
    {
        public Tile_PropertySetNode(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TilePropertySet;
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
