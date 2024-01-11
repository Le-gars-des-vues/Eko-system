using UnityEngine;

namespace TerraTiler2D
{
    public abstract class Math_Node : Node
    {
        public Math_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName.Split('(')[0], position, guid)
        {

        }
    }
}
