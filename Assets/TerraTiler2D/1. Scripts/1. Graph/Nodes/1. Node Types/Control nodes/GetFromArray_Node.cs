using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class GetFromArray_Node : DynamicType_Node
    {
        private Port<List<object>> inputPort;
        private PortWithField<int> indexPort;

        private Port<object> outputPort;

        public GetFromArray_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.GetFromArray;
            SetTooltip("Gets an object from an array.");
            searchMenuEntry = new string[] { "Control"};
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<List<object>>("Array", PortDirection.Input, "Array", PortCapacity.Single, true);

            indexPort = GeneratePortWithField<int>("Index", PortDirection.Input, 0, "Index", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            outputPort = GeneratePort<object>("Object", PortDirection.Output, "Object", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetObjectOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

#if (UNITY_EDITOR)
        protected override void setInputPortType(System.Type newType)
        {
            setPortType(inputPort, newType);
        }
        protected override void setOutputPortType(Type newType)
        {
            setPortType(outputPort, newType);
        }

        protected override void HandleTypeChange(List<Port_Abstract> ports = null)
        {
            List<Port_Abstract> allPorts = new List<Port_Abstract>();
            if (ports != null)
            {
                allPorts = ports;
            }
            else
            {
                allPorts.Add(inputPort);
                allPorts.Add(outputPort);
            }

            base.HandleTypeChange(allPorts);
        }

        protected override bool IsDynamicInputPort(Port_Abstract otherPort)
        {
            return otherPort == inputPort;
        }
        protected override bool IsDynamicOutputPort(Port_Abstract otherPort)
        {
            return otherPort == outputPort;
        }

        protected override Type GetDynamicInputPortType()
        {
            return GetPortTypeDefinition(inputPort);
        }
        protected override Type GetDynamicOutputPortType()
        {
            return GetPortTypeDefinition(outputPort);
        }
#endif

        public object GetObjectOutput()
        {
            return ((List<object>)inputPort.GetPortVariable())[(int)indexPort.GetPortVariable()];
        }
    }
}