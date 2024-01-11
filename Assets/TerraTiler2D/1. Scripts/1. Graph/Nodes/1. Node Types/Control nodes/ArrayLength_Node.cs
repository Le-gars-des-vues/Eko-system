using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class ArrayLength_Node : Node
    {
        private Port<List<object>> arrayInputPort;
        private Port<int> lengthPort;

        public ArrayLength_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.ArrayLength;
            SetTooltip("Gets the length of an array.");
            searchMenuEntry = new string[] { "Control"};
        }

        protected override void InitializeInputPorts()
        {
            arrayInputPort = GeneratePort<List<object>>("Array", PortDirection.Input, "Array", PortCapacity.Single, true, "The array to get the length of.");
        }

        protected override void InitializeOutputPorts()
        {
            lengthPort = GeneratePort<int>("Length", PortDirection.Output, "ArrayLength", PortCapacity.Multi, false, "The length of the array.");
            lengthPort.SetOutputPortMethod(GetArrayLength);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetArrayLength()
        {
            return ((List<object>)arrayInputPort.GetPortVariable()).Count;
        }
    }
}