using UnityEngine;

namespace TerraTiler2D
{
    public class Vector4Min_Node : Math_Node
    {
        private PortWithField<Vector4> portA;
        private PortWithField<Vector4> portB;

        private Port<Vector4> outputPort;

        public Vector4Min_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector4Min;
            SetTooltip("Returns the Vector4 with the shortest length.");
            searchMenuEntry = new string[] { "Math", "Vector4" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector4>("A", PortDirection.Input, Vector4.zero, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<Vector4>("B", PortDirection.Input, Vector4.zero, "B", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector4>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Vector4 A = (Vector4)portA.GetPortVariable();
            Vector4 B = (Vector4)portB.GetPortVariable();

            if (A.magnitude < B.magnitude)
            {
                return A;
            }
            else if (A.magnitude > B.magnitude)
            {
                return B;
            }
            else
            {
#if (UNITY_EDITOR)
                Glob.GetInstance().DebugString("Vector4 A and Vector4 B of node '" + name + "' have the same length. Returning Vector4 A.", Glob.DebugCategories.Node, Glob.DebugLevel.Low, Glob.DebugTypes.Warning);
#endif
                return A;
            }
        }
    }
}
