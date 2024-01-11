using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class SetColorTexture2D_Node : Node
    {
        protected Port<Texture2D> textureInputPort;
        protected Port<Gradient> gradientPort;

        protected Port<Texture2D> textureOutputPort;

        private NodePreview nodePreview;

        public SetColorTexture2D_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.SetColorTexture2D;
            SetTooltip("Sets the colors of a Texture2D to target gradient, based on the brightness values of the Texture2D.");
            searchMenuEntry = new string[] { "Texture2D" };
            nodePreview = new NodePreview(this, ColorTexture);
        }

        protected override void InitializeInputPorts()
        {
            textureInputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Input, "Texture", PortCapacity.Single, true, "The texture to set the colors for.");
            gradientPort = GeneratePort<Gradient>("Gradient", PortDirection.Input, "Gradient", PortCapacity.Single, true, "The colors to apply to the texture.");
        }

        protected override void InitializeOutputPorts()
        {
            textureOutputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Output, "TextureOutput", PortCapacity.Multi, false, "The recolored texture.");
            textureOutputPort.SetOutputPortMethod(GetTextureOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetTextureOutput()
        {
            if (nodePreview.ShouldPreview())
            {
                nodePreview.ShowTexture();
            }

            return ColorTexture();
        }

        private Texture2D ColorTexture()
        {
            Texture2D inputTexture = (Texture2D)textureInputPort.GetPortVariable();
            Gradient gradient = (Gradient)gradientPort.GetPortVariable();

            Texture2D clampedTexture = new Texture2D(inputTexture.width, inputTexture.height);
            Color[] clampedPixels = inputTexture.GetPixels();
            float darkestPixel = 1.0f;
            float brightestPixel = 0.0f;

            for (int i = 0; i < clampedPixels.Length; i++)
            {
                float brightness = GetPixelBrightness(clampedPixels[i]);

                if (brightness < darkestPixel)
                {
                    darkestPixel = brightness;
                }

                if (brightness > brightestPixel)
                {
                    brightestPixel = brightness;
                }
            }

            for (int i = 0; i < clampedPixels.Length; i++)
            {
                float pixelBrightness = GetPixelBrightness(clampedPixels[i]);
                clampedPixels[i] = gradient.Evaluate(pixelBrightness);
            }

            clampedTexture.SetPixels(clampedPixels);
            clampedTexture.Apply();

            return clampedTexture;
        }

        private float GetPixelBrightness(Color pixel)
        {
            return ((pixel.r + pixel.g + pixel.b) / 3);
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
