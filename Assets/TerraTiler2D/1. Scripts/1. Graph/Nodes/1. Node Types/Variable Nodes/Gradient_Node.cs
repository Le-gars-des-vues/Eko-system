using UnityEngine;

namespace TerraTiler2D
{
    /// <summary>
    /// A node with a gradient field.
    /// </summary>
    public class Gradient_Node : Node
    {
        private PortWithField<Gradient> gradientPort;

        public Gradient_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Gradient;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            Gradient defaultValue = new Gradient()
                                    {
                                        alphaKeys = new GradientAlphaKey[]
                                        {
                                            new GradientAlphaKey(1.0f, 0.0f),
                                            new GradientAlphaKey(1.0f, 1.0f),
                                        },
                                        colorKeys = new GradientColorKey[]
                                        {
                                            new GradientColorKey(Color.black, 0.0f),
                                            new GradientColorKey(Color.white, 1.0f),
                                        }
                                    };

            //Create an input port.
            gradientPort = GeneratePortWithField<Gradient>("Gradient", PortDirection.Output, defaultValue, "GradientOutput", PortCapacity.Multi, false);

        }

        protected override void InitializeAdditionalElements()
        {

        }
    }
}
