using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    [Serializable]
    public class Array_NodeData : NodeData
    {
        public int portCount;

        public override object Clone()
        {
            return new Array_NodeData()
            {
                GUID = this.GUID,
                NodeName = this.NodeName,
                Position = this.Position,
                NodeType = this.NodeType,
                portCount = this.portCount,
            };
        }
    }

    public class Array_Node : DynamicType_Node
    {
        private List<Port<object>> arrayInputPorts = new List<Port<object>>();

        private Port<List<object>> outputPort;

        private List<object> myArray;

        public Array_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Array;
            SetTooltip("Creates an array.");
            searchMenuEntry = new string[] { "Control" };
        }

        protected override void InitializeInputPorts()
        {
#if (UNITY_EDITOR)
            Button addButton = new Button(() => { AddDynamicInputPort(); });
            addButton.text = "+";
            GetContainer(NodeContainers.InputContainer).Add(addButton);

            Button removeButton = new Button(() => { RemoveDynamicInputPort(); });
            removeButton.text = "-";
            GetContainer(NodeContainers.InputContainer).Add(removeButton);
#endif
        }

        protected override void InitializeOutputPorts()
        {
            outputPort = GeneratePort<List<object>>("Array", PortDirection.Output, "OutputArray", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        private void AddDynamicInputPort()
        {
            Port<object> newPort = GeneratePort<object>("[" + arrayInputPorts.Count.ToString() + "]", PortDirection.Input, (arrayInputPorts.Count + 1).ToString(), PortCapacity.Single, false);
            arrayInputPorts.Add(newPort);

#if (UNITY_EDITOR)
            if (currentType != typeof(object))
            {
                setPortType(newPort, currentType);
            }
#endif
        }
        private void RemoveDynamicInputPort()
        {
#if (UNITY_EDITOR)
            arrayInputPorts[arrayInputPorts.Count - 1].DisconnectAllEdges();
#endif
            arrayInputPorts[arrayInputPorts.Count - 1].RemoveFromHierarchy();
            arrayInputPorts.RemoveAt(arrayInputPorts.Count - 1);
        }

#if (UNITY_EDITOR)
        protected override void HandleTypeChange(List<Port_Abstract> ports = null)
        {
            //Get all input and output ports related to this node type
            List<Port_Abstract> allPorts = new List<Port_Abstract>();
            if (ports != null)
            {
                allPorts = ports;
            }
            else
            {
                allPorts.AddRange(arrayInputPorts);
                allPorts.Add(outputPort);
            }
            
            //Let the base class handle the type changes
            base.HandleTypeChange(allPorts);
        }

        //For every dynamic input port that should have its type changed, call setPortType.
        protected override void setInputPortType(System.Type newType)
        {
            foreach (Port_Abstract port in arrayInputPorts)
            {
                setPortType(port, newType);
            }
        }
        //For every dynamic output port that should have its type changed, call setPortType.
        protected override void setOutputPortType(System.Type newType)
        {
            setPortType(outputPort, newType);
        }

        //Is port otherPort a dynamic input port from this node?
        protected override bool IsDynamicInputPort(Port_Abstract otherPort)
        {
            foreach (Port_Abstract port in arrayInputPorts)
            {
                if (port == otherPort)
                {
                    return true;
                }
            }
            return false;
        }
        //Is port otherPort a dynamic output port from this node?
        protected override bool IsDynamicOutputPort(Port_Abstract otherPort)
        {
            return otherPort == outputPort;
        }

        //Do any of the dynamic input ports from this node define a type, or is it still blank (object type)
        protected override Type GetDynamicInputPortType()
        {
            //For each input port
            foreach (Port_Abstract port in arrayInputPorts)
            {
                //If the port has a different type than object, return it
                Type portType = GetPortTypeDefinition(port);
                if (portType != typeof(object))
                {
                    return portType;
                }
            }

            //No type defining connections were found, so return object type
            return typeof(object);
        }
        //Do any of the dynamic output ports from this node define a type, or is it still blank (object type)
        protected override Type GetDynamicOutputPortType()
        {
            return GetPortTypeDefinition(outputPort);
        }
#endif

        public override void ResetNodeVariables()
        {
            myArray = null;

            base.ResetNodeVariables();
        }

        public object GetOutput()
        {
            if (myArray != null)
            {
                return myArray;
            }

            myArray = new List<object>();
            for (int i = 0; i < arrayInputPorts.Count; i++)
            {
                myArray.Insert(i, arrayInputPorts[i].GetPortVariable());
            }

            return myArray;
        }

        public override NodeData GetNodeData(NodeData nodeData = null)
        {
            return base.GetNodeData(new Array_NodeData()
            {
                portCount = this.arrayInputPorts.Count
            });
        }
        public override void LoadNodeData(NodeData data)
        {
            Array_NodeData nodeData = data as Array_NodeData;

            if (nodeData != null)
            {
                for (int i = arrayInputPorts.Count; i < nodeData.portCount; i++)
                {
                    AddDynamicInputPort();
                }
            }
        }
    }
}