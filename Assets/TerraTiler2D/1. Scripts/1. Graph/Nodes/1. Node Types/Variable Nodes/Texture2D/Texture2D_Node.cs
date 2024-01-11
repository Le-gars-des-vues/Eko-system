using System;
using UnityEngine;
using UnityEngine.UIElements;
#if (UNITY_EDITOR)
using UnityEditor.UIElements;
#endif

namespace TerraTiler2D
{
    /// <summary>
    /// A node with a gradient field.
    /// </summary>
    public class Texture2D_Node : Node
    {
        private PortWithField<Texture2D> texturePort;

        public Texture2D_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Texture2D;
            SetTooltip("Imports a Texture2D.");
            searchMenuEntry = new string[] { "Texture2D" };
        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            texturePort = GeneratePortWithField<Texture2D>("", PortDirection.Output, null, "TextureOutput", PortCapacity.Multi, false, "The imported Texture2D.");
        }

        protected override void InitializeAdditionalElements()
        {

        }
    }
}
