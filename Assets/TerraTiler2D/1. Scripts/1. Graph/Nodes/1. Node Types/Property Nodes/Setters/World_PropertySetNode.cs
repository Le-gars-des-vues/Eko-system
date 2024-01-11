using UnityEngine;

namespace TerraTiler2D
{
    public class World_PropertySetNode : PropertySetter_Node<World>
    {
        public World_PropertySetNode(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.WorldPropertySet;
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
