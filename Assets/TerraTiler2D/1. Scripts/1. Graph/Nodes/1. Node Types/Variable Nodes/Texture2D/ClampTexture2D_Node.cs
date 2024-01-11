using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class ClampTexture2D_Node : Node
    {
        protected Port<Texture2D> textureInputPort;
        protected PortWithField<float> minPort;
        protected PortWithField<float> maxPort;

        protected Port<Texture2D> textureOutputPort;

        private NodePreview nodePreview;

        public ClampTexture2D_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.ClampTexture2D;
            SetTooltip("Clamps the brightness of pixels in a texture. Changes the darkest pixels in the texture to the 'Min brightness' value, and the brightest pixels to the 'Max brightness' value.");
            searchMenuEntry = new string[] { "Texture2D" };
            nodePreview = new NodePreview(this, ClampTexture);
        }

        protected override void InitializeInputPorts()
        {
            textureInputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Input, "Texture", PortCapacity.Single, true, "Texture2D to clamp.");
            minPort = GeneratePortWithField<float>("Min brightness", PortDirection.Input, 0, "Min", PortCapacity.Single, false, "Minimum value to clamp the texture brightness to.");
            maxPort = GeneratePortWithField<float>("Max brightness", PortDirection.Input, 1, "Max", PortCapacity.Single, false, "Maximum value to clamp the texture brightness to.");
        }

        protected override void InitializeOutputPorts()
        {
            textureOutputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Output, "TextureOutput", PortCapacity.Multi, false, "Texture2D with clamped brightness.");
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

            return ClampTexture();
        }

        private Texture2D ClampTexture()
        {
            Texture2D inputTexture = (Texture2D)textureInputPort.GetPortVariable();
            float min = (float)minPort.GetPortVariable();
            float max = (float)maxPort.GetPortVariable();

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

            if (darkestPixel == brightestPixel)
            {
                Glob.GetInstance().DebugString("The texture passed into node '" + GetTitle() + "' is an even color, clamping will result in a black texture.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Default);
            }

            for (int i = 0; i < clampedPixels.Length; i++)
            {
                float relativeBrightness = GetRelativePixelBrightness(GetPixelBrightness(clampedPixels[i]), darkestPixel, brightestPixel);
                float newBrightness = Mathf.Lerp(min, max, relativeBrightness);
                clampedPixels[i] = SetPixelBrightness(clampedPixels[i], newBrightness);

            }

            clampedTexture.SetPixels(clampedPixels);
            clampedTexture.Apply();

            return clampedTexture;
        }

        private float GetPixelBrightness(Color pixel)
        {
            return (0.375f * pixel.r) + (0.5f * pixel.g) + (0.125f * pixel.b);
        }
        private Color SetPixelBrightness(Color pixel, float brightness)
        {
            Color normalizedColor = pixel * (1 / pixel.maxColorComponent);
            return new Color(normalizedColor.r * brightness, normalizedColor.g * brightness, normalizedColor.b * brightness);
        }

        private float GetRelativePixelBrightness(float pixelBrightness, float min = 0, float max = 1)
        {
            return (pixelBrightness - min) / (max - min);
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
