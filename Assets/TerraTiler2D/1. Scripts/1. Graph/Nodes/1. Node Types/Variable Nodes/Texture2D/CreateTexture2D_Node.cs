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
    public class CreateTexture2D_Node : Node
    {
        private PortWithField<Vector2> textureSizePort;
        private PortWithField<Color> colorPort;

        public CreateTexture2D_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.CreateTexture2D;
            SetTooltip("Creates an new Texture2D.");
            searchMenuEntry = new string[] { "Texture2D" };
        }

        protected override void InitializeInputPorts()
        {
            textureSizePort = GeneratePortWithField<Vector2>("Size", PortDirection.Input, new Vector2(100, 100), "TextureSize", PortCapacity.Single, false, "The size of the texture in pixels.");
            colorPort = GeneratePortWithField<Color>("Color", PortDirection.Input, new Color(), "Color", PortCapacity.Single, false, "The fill color of the texture.");
        }

        protected override void InitializeOutputPorts()
        {
            //Create an input port.
            Port<Texture2D> texturePort = GeneratePort<Texture2D>("Texture2D", PortDirection.Output, "TextureOutput", PortCapacity.Multi, false, "The newly created texture.");
            texturePort.SetOutputPortMethod(GetTextureOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetTextureOutput()
        {
            Vector2 textureSize = (Vector2)textureSizePort.GetPortVariable();
            textureSize.x = Mathf.Max(textureSize.x, 0);
            textureSize.y = Mathf.Max(textureSize.y, 0);

            Texture2D returnTexture = new Texture2D((int)textureSize.x, (int)textureSize.y);

            for (int i = 0; i < returnTexture.width; i++)
            {
                for (int j = 0; j < returnTexture.height; j++)
                {
                    returnTexture.SetPixel(i, j, (Color)colorPort.GetPortVariable());
                }
            }

            return returnTexture;
        }
    }
}
