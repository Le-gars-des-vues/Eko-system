using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class AddToArray_Node : DynamicTypeFlow_Node
    {
        private Port<List<object>> arrayInputPort;
        private Port<object> objectInputPort;
        private PortWithField<int> indexPort;

        private Port<List<object>> arrayOutputPort;

        public AddToArray_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.AddToArray;
            SetTooltip("Adds an object to an array.");
            searchMenuEntry = new string[] { "Control"};
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            arrayInputPort = GeneratePort<List<object>>("Array", PortDirection.Input, "Array", PortCapacity.Single, true);
            objectInputPort = GeneratePort<object>("Object", PortDirection.Input, "Object", PortCapacity.Single, true);

            indexPort = GeneratePortWithField<int>("Index", PortDirection.Input, 0, "Index", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();

            arrayOutputPort = GeneratePort<List<object>>("Array", PortDirection.Output, "OutputArray", PortCapacity.Multi, false);
            arrayOutputPort.SetOutputPortMethod(GetArrayOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            List<object> array = (List<object>)arrayInputPort.GetPortVariable();
            object targetObject = (object)objectInputPort.GetPortVariable();
            int targetIndex = (int)indexPort.GetPortVariable();

            if (array.Count > targetIndex)
            {
                if (array[targetIndex] != null)
                {
                    Glob.GetInstance().DebugString("Node '" + this.GetTitle() + "' is adding object '" + targetObject + "' to array at index '" + targetIndex + "', but the array already holds object '" + array[targetIndex] + "' at that index. The value will be overwritten.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                }

                array[targetIndex] = targetObject;
            }
            else
            {
                array.Insert(targetIndex, targetObject);
            }

            base.ApplyBehaviour(flow, trickleDown);
        }

#if (UNITY_EDITOR)
        protected override void setInputPortType(System.Type newType)
        {
            setPortType(arrayInputPort, newType);
            setPortType(objectInputPort, newType);
        }
        protected override void setOutputPortType(Type newType)
        {
            setPortType(arrayOutputPort, newType);
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
                allPorts.Add(arrayInputPort);
                allPorts.Add(objectInputPort);
                allPorts.Add(arrayOutputPort);
            }

            base.HandleTypeChange(allPorts);
        }

        protected override bool IsDynamicInputPort(Port_Abstract otherPort)
        {
            return otherPort == arrayInputPort || otherPort == objectInputPort;
        }
        protected override bool IsDynamicOutputPort(Port_Abstract otherPort)
        {
            return otherPort == arrayOutputPort;
        }

        protected override Type GetDynamicInputPortType()
        {
            Type returnType = GetPortTypeDefinition(arrayInputPort);
            if (returnType == typeof(object))
            {
                returnType = GetPortTypeDefinition(objectInputPort);
            }
            return returnType;
        }
        protected override Type GetDynamicOutputPortType()
        {
            return GetPortTypeDefinition(arrayOutputPort);
        }
#endif

        public object GetArrayOutput()
        {
            return (List<object>)arrayInputPort.GetPortVariable();
        }
    }
}