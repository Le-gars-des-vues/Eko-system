using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class IsNull_Node : Node
    {
        private Port<object> objectInputPort;
        private Port<bool> isNullPort;

        public IsNull_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IsNull;
            SetTooltip("Checks if an object is null.");
            searchMenuEntry = new string[] { "Control"};
        }

        protected override void InitializeInputPorts()
        {
            objectInputPort = GeneratePort<object>("", PortDirection.Input, "Object", PortCapacity.Single, false, "Check if the connected port returns null.");
        }

        protected override void InitializeOutputPorts()
        {
            isNullPort = GeneratePort<bool>("", PortDirection.Output, "IsNull", PortCapacity.Multi, false, "Is the object null.");
            isNullPort.SetOutputPortMethod(IsObjectNull);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object IsObjectNull()
        {
            return (objectInputPort.GetPortVariable() == null);
        }
    }
}