using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class GetArrayIndex_Node : DynamicType_Node
    {
        private Port<List<object>> arrayInputPort;
        private Port<object> objectInputPort;

        private Port<int> indexPort;

        public GetArrayIndex_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.GetArrayIndex;
            SetTooltip("Gets the index of an item in an array.");
            searchMenuEntry = new string[] { "Control"};
        }

        protected override void InitializeInputPorts()
        {
            arrayInputPort = GeneratePort<List<object>>("Array", PortDirection.Input, "Array", PortCapacity.Single, true);
            objectInputPort = GeneratePort<object>("Object", PortDirection.Input, "Object", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            indexPort = GeneratePort<int>("Index", PortDirection.Output, "Index", PortCapacity.Multi, false, "The index of the item in the array.");
            indexPort.SetOutputPortMethod(GetIndex);
        }

        protected override void InitializeAdditionalElements()
        {

        }

#if (UNITY_EDITOR)
        protected override void setInputPortType(System.Type newType)
        {
            setPortType(arrayInputPort, newType);
            setPortType(objectInputPort, newType);
        }
        protected override void setOutputPortType(Type newType)
        {
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
            }

            base.HandleTypeChange(allPorts);
        }

        protected override bool IsDynamicInputPort(Port_Abstract otherPort)
        {
            return otherPort == arrayInputPort || otherPort == objectInputPort;
        }
        protected override bool IsDynamicOutputPort(Port_Abstract otherPort)
        {
            return false;
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
            return typeof(object);
        }
#endif

        public object GetIndex()
        {
            List<object> array = (List<object>)arrayInputPort.GetPortVariable();
            object arrayObject = (object)objectInputPort.GetPortVariable();

            if (array.Contains(arrayObject))
            {
                return array.IndexOf(arrayObject);
            }

            Glob.GetInstance().DebugString("Array does not contain object '" + arrayObject + "', and can not return an index. Returning NULL.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
            return null;
        }
    }
}