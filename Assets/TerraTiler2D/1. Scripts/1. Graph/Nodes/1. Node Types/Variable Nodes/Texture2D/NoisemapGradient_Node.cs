using UnityEngine;

namespace TerraTiler2D
{
    public class NoisemapGradient_Node : Noisemap_Node
    {
        private PortWithField<Vector2> gradientPositionPort;
        private PortWithField<Vector2> gradientDirectionPort;
        private PortWithField<float> gradientSizePort;
        private Port<Texture2D> gradientSizeOffsetNoisemapPort;
        private PortWithField<float> gradientSizeOffsetPort;
        private Port<Texture2D> gradientPositionOffsetNoisemapPort;
        private PortWithField<float> gradientPositionOffsetPort;

        //========== Initialization ==========

        public NoisemapGradient_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.NoisemapGradient;
            SetTooltip("Generates randomized gradient noisemaps.");
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            gradientPositionPort = GeneratePortWithField<Vector2>("Position", PortDirection.Input, new Vector2(0.5f, 0.5f), "Pos", PortCapacity.Single, false, "Position of the gradient in the texture. \n(0, 0) is bottom left, (1, 1) is top right.");

            gradientDirectionPort = GeneratePortWithField<Vector2>("Direction", PortDirection.Input, new Vector2(0, -1), "Dir", PortCapacity.Single, false,   "Direction of the gradient in the texture. \n" +
                                                                                                                                                    "A positive X value points right, and a negative X value left. A positive Y value points up, and a negative Y value down.");

            gradientSizePort = GeneratePortWithField<float>("Size", PortDirection.Input, 10, "GradientSize", PortCapacity.Single, false, "The amount of pixels it takes to fade from color A to color B.");

            gradientSizeOffsetNoisemapPort = GeneratePort<Texture2D>("Size offset texture", PortDirection.Input, "SizeOffsetNoisemap", PortCapacity.Single, false, "The amount of pixels it takes to fade from color A to color B is offset based on the brightness of the pixels in this Texture2D.");

            gradientSizeOffsetPort = GeneratePortWithField<float>("Size offset", PortDirection.Input, 30, "SizeOffset", PortCapacity.Single, false, "The amount of pixels to offset the size with.");

            gradientPositionOffsetNoisemapPort = GeneratePort<Texture2D>("Position offset texture", PortDirection.Input, "PositionOffsetNoisemap", PortCapacity.Single, false, "The position of the gradient is offset based on the brightness of the pixels in this Texture2D.");

            gradientPositionOffsetPort = GeneratePortWithField<float>("Position offset", PortDirection.Input, 30, "PositionOffset", PortCapacity.Single, false, "The amount of pixels to offset the position with.");
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
            NoisemapGradient noisemapGenerator = new NoisemapGradient();

            //object gradientPositionVariable = GetPortVariable(gradientPositionPort);
            object gradientPositionVariable = gradientPositionPort.GetPortVariable();
            if (gradientPositionVariable != null)
            {
                noisemapGenerator.SetGradientPosition((Vector2)gradientPositionVariable);
            }
            //object gradientDirectionVariable = GetPortVariable(gradientDirectionPort);
            object gradientDirectionVariable = gradientDirectionPort.GetPortVariable();
            if (gradientDirectionVariable != null)
            {
                noisemapGenerator.SetGradientDirection((Vector2)gradientDirectionVariable);
            }
            //object gradientSizeVariable = GetPortVariable(gradientSizePort);
            object gradientSizeVariable = gradientSizePort.GetPortVariable();
            if (gradientSizeVariable != null)
            {
                noisemapGenerator.SetGradientSize((float)gradientSizeVariable);
            }
            //object gradientSizeOffsetNoisemapVariable = GetPortVariable(gradientSizeOffsetNoisemapPort);
            object gradientSizeOffsetNoisemapVariable = gradientSizeOffsetNoisemapPort.GetPortVariable();
            if (gradientSizeOffsetNoisemapVariable != null)
            {
                noisemapGenerator.SetGradientSizeOffsetNoisemap((Texture2D)gradientSizeOffsetNoisemapVariable);
            }
            //object gradientSizeOffsetVariable = GetPortVariable(gradientSizeOffsetPort);
            object gradientSizeOffsetVariable = gradientSizeOffsetPort.GetPortVariable();
            if (gradientSizeOffsetVariable != null)
            {
                noisemapGenerator.SetGradientSizeOffset((float)gradientSizeOffsetVariable);
            }
            //object gradientPositionOffsetNoisemapVariable = GetPortVariable(gradientPositionOffsetNoisemapPort);
            object gradientPositionOffsetNoisemapVariable = gradientPositionOffsetNoisemapPort.GetPortVariable();
            if (gradientPositionOffsetNoisemapVariable != null)
            {
                noisemapGenerator.SetGradientOffsetNoisemap((Texture2D)gradientPositionOffsetNoisemapVariable);
            }
            //object gradientPositionOffsetVariable = GetPortVariable(gradientPositionOffsetPort);
            object gradientPositionOffsetVariable = gradientPositionOffsetPort.GetPortVariable();
            if (gradientPositionOffsetVariable != null)
            {
                noisemapGenerator.SetGradientOffset((float)gradientPositionOffsetVariable);
            }

            return GetNoisemap(noisemapGenerator);
        }
    }
}
