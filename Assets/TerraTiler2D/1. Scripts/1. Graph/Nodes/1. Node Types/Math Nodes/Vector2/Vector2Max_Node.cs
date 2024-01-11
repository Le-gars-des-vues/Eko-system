using UnityEngine;

namespace TerraTiler2D
{
    public class Vector2Max_Node : Math_Node
    {
        private PortWithField<Vector2> portA;
        private PortWithField<Vector2> portB;

        private Port<Vector2> outputPort;

        public Vector2Max_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector2Max;
            SetTooltip("Returns the Vector2 with the biggest length.");
            searchMenuEntry = new string[] { "Math", "Vector2" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector2>("A", PortDirection.Input, Vector2.zero, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<Vector2>("B", PortDirection.Input, Vector2.zero, "B", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector2>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Vector2 A = (Vector2)portA.GetPortVariable();
            Vector2 B = (Vector2)portB.GetPortVariable();

            if (A.magnitude > B.magnitude)
            {
                return A;
            }
            else if (A.magnitude < B.magnitude)
            {
                return B;
            }
            else
            {
#if (UNITY_EDITOR)
                Glob.GetInstance().DebugString("Vector2 A and Vector2 B of node '" + name + "' have the same length. Returning Vector2 A.", Glob.DebugCategories.Node, Glob.DebugLevel.Low, Glob.DebugTypes.Warning);
#endif
                return A;
            }
        }
    }
}
