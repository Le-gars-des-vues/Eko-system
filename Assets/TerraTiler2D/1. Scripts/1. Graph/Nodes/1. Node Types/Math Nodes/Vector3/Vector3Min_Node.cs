using UnityEngine;

namespace TerraTiler2D
{
    public class Vector3Min_Node : Math_Node
    {
        private PortWithField<Vector3> portA;
        private PortWithField<Vector3> portB;

        private Port<Vector3> outputPort;

        public Vector3Min_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector3Min;
            SetTooltip("Returns the Vector3 with the shortest length.");
            searchMenuEntry = new string[] { "Math", "Vector3" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector3>("A", PortDirection.Input, Vector3.zero, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<Vector3>("B", PortDirection.Input, Vector3.zero, "B", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector3>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Vector3 A = (Vector3)portA.GetPortVariable();
            Vector3 B = (Vector3)portB.GetPortVariable();

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
                Glob.GetInstance().DebugString("Vector3 A and Vector3 B of node '" + name + "' have the same length. Returning Vector3 A.", Glob.DebugCategories.Node, Glob.DebugLevel.Low, Glob.DebugTypes.Warning);
#endif
                return A;
            }
        }
    }
}
