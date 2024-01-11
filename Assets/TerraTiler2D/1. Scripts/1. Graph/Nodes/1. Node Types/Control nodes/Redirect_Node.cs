using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class Redirect_Node : DynamicType_Node
    {
        private Port<object> inputPort;
        private Port<object> outputPort;

        public Redirect_Node(string nodeName, Vector2 position, string guid = null) : base("", position, guid)
        {
            nodeType = Glob.NodeTypes.Redirect;
            searchMenuEntry = new string[] { "Control"};

            //Make the node smaller by removing padding and unnecessary containers
            GetContainer(NodeContainers.OutputContainer).style.width = Glob.GetInstance().NodeTitleContainerHeight * 1.5f;
            GetContainer(NodeContainers.InputContainer).style.width = Glob.GetInstance().NodeTitleContainerHeight * 1.5f;

            GetContainer(NodeContainers.OutputContainer).style.height = Glob.GetInstance().NodeTitleContainerHeight;
            GetContainer(NodeContainers.InputContainer).style.height = Glob.GetInstance().NodeTitleContainerHeight;

            GetContainer(NodeContainers.TitleContainer).RemoveFromHierarchy();
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<object>(" ", PortDirection.Input, "Input", PortCapacity.Single, true);
            inputPort.style.position = Position.Absolute;
            inputPort.transform.position = new Vector3(-2, -2, 0);
        }

        protected override void InitializeOutputPorts()
        {
            outputPort = GeneratePort<object>(" ", PortDirection.Output, "Output", PortCapacity.Single, false);
            outputPort.SetOutputPortMethod(GetObjectOutput);
            outputPort.style.position = Position.Absolute;
            outputPort.transform.position = new Vector3(-4, -2, 0);
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
            return inputPort.GetPortVariable();
        }
    }
}