using UnityEngine;

namespace TerraTiler2D
{
    public class NoisemapValue_Node : Noisemap_Node
    {
        private Port<Vector2> positionPort;
        private Port<NoisemapDimension> dimensionsPort;
        private Port<float> xFrequencyPort;
        private Port<float> yFrequencyPort;
        private Port<int> layersPort;
        private Port<float> lacunarityPort;
        private Port<float> persistencePort;
        private Port<bool> smoothPort;

        //========== Initialization ==========

        public NoisemapValue_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.NoisemapValue;
            SetTooltip("Generates randomized value noisemaps.");
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            positionPort = GeneratePortWithField<Vector2>("Position", PortDirection.Input, Vector2.zero, "Pos", PortCapacity.Single, false, "The noise offset. The noise will shift by 1 pixel per rounded integer.");

            dimensionsPort = GeneratePort<NoisemapDimension>("Dimensions", PortDirection.Input, "Dim", PortCapacity.Single, false, "The node will generate noise along these axes.");

            xFrequencyPort = GeneratePortWithField<float>("X Frequency", PortDirection.Input, 10, "xFreq", PortCapacity.Single, false, "How often the noisemap changes color along the X axis. A smaller value will result in bigger shapes.");

            yFrequencyPort = GeneratePortWithField<float>("Y Frequency", PortDirection.Input, 10, "yFreq", PortCapacity.Single, false, "How often the noisemap changes color along the Y axis. A smaller value will result in bigger shapes.");

            layersPort = GeneratePortWithField<int>("Layers", PortDirection.Input, 4, "Layers", PortCapacity.Single, false, "The amount of layers of noise that will be generated on top of each other. A higher value will result in a noisemap with more detail. Also known as octaves.");

            lacunarityPort = GeneratePortWithField<float>("Lacunarity", PortDirection.Input, 0.5f, "Lacu", PortCapacity.Single, false, "Per layer, the X and Y frequency are multiplied by this factor. A higher value will add smaller details per layer.");

            persistencePort = GeneratePortWithField<float>("Persistence", PortDirection.Input, 0.5f, "Persi", PortCapacity.Single, false, "Per layer, the opacity is multiplied by this factor (0.0f - 1.0f). A higher value will make smaller details more apparent, but may obstruct bigger shapes from showing.");

            smoothPort = GeneratePortWithField<bool>("Smooth", PortDirection.Input, true, "Smooth", PortCapacity.Single, false, "Should the noisemap be smoothed. An unsmoothed noisemap will look more blocky.");
        }
        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();
        }
        protected override void InitializeAdditionalElements()
        {
            base.InitializeAdditionalElements();
        }

        //========== Node methods ==========

        protected override Texture2D GetNoisemap()
        {
            NoisemapValue noisemapGenerator = new NoisemapValue();

            object positionVariable = positionPort.GetPortVariable();
            if (positionVariable != null)
            {
                noisemapGenerator.SetPosition((Vector2)positionVariable);
            }
            object dimensionsVariable = dimensionsPort.GetPortVariable();
            if (dimensionsVariable != null)
            {
                noisemapGenerator.SetDimensions((NoisemapDimension)dimensionsVariable);
            }
            object xFrequencyVariable = xFrequencyPort.GetPortVariable();
            if (xFrequencyVariable != null)
            {
                noisemapGenerator.SetXFrequency((float)xFrequencyVariable);
            }
            object yFrequencyVariable = yFrequencyPort.GetPortVariable();
            if (yFrequencyVariable != null)
            {
                noisemapGenerator.SetYFrequency((float)yFrequencyVariable);
            }
            object octavesVariable = layersPort.GetPortVariable();
            if (octavesVariable != null)
            {
                noisemapGenerator.SetOctaves((int)octavesVariable);
            }
            object lacunarityVariable = lacunarityPort.GetPortVariable();
            if (lacunarityVariable != null)
            {
                noisemapGenerator.SetLacunarity((float)lacunarityVariable);
            }
            object persistenceVariable = persistencePort.GetPortVariable();
            if (persistenceVariable != null)
            {
                noisemapGenerator.SetPersistence((float)persistenceVariable);
            }
            object smoothVariable = smoothPort.GetPortVariable();
            if (smoothVariable != null)
            {
                noisemapGenerator.SetSmooth((bool)smoothVariable);
            }

            return GetNoisemap(noisemapGenerator);
        }

        //========== Port data passing ==========

        public override void ResetNodeVariables()
        {
            base.ResetNodeVariables();
        }
    }
}
