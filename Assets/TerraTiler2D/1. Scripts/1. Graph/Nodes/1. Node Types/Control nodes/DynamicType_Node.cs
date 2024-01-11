using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public abstract class DynamicType_Node : Node
    {
        protected Type currentType = typeof(object);

#if (UNITY_EDITOR)
        private bool hasBeenChecked = false;
#endif

        public DynamicType_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {

        }

#if (UNITY_EDITOR)
        public override void HandlePortConnected(Port_Abstract connectedPort, Port_Abstract otherPort)
        {
            //If something got connected to an input or output port
            if (IsDynamicInputPort(connectedPort) || IsDynamicOutputPort(connectedPort))
            {
                //If the currentType is 'object' (If no type has been defined yet)
                if (currentType == typeof(object))
                {
                    //Get the type of the connected port
                    Type newType = otherPort.port.portType;
                    //If the type is a list
                    if (Glob.GetInstance().IsList(newType))
                    {
                        //Get the type of the list
                        newType = newType.GetGenericArguments()[0];
                    }

                    //Set the type of this node to the type of the connected port
                    SetCurrentType(newType);
                }
            }
        }
        public override void HandlePortDisconnected(Port_Abstract disconnectedPort)
        {
            //If something got disconnected from an input or output port
            if (IsDynamicInputPort(disconnectedPort) || IsDynamicOutputPort(disconnectedPort))
            {
                //If there is nothing connected to any inputPort or outputPort that defines a type for this node
                if (!IsTypeDefined())
                {
                    SetCurrentType(typeof(object));
                }
            }
        }

        public void SetCurrentType(Type newType)
        {
            //If the newType is the same as the currentType, dont change anything
            if (currentType == newType)
            {
                return;
            }

            //Set the portType of any inputPort and outputPort
            setInputPortType(newType);
            setOutputPortType(newType);

            //Store the new type
            currentType = newType;

            //Handle the type change
            HandleTypeChange();
        }
        protected virtual void HandleTypeChange(List<Port_Abstract> ports = null)
        {
            if (hasBeenChecked || ports == null)
            {
                return;
            }

            hasBeenChecked = true;

            //For each port
            foreach (Port_Abstract port in ports)
            {
                //Get all the connections to the port
                var enumerator = port.port.connections.GetEnumerator();
                //For every connection
                while (enumerator.MoveNext())
                {
                    //Get the port and node it is connected to
                    Node connectedNode;

                    if (port.direction == PortDirection.Input)
                    {
                        connectedNode = (Node)enumerator.Current.output.node;
                    }
                    else
                    {
                        connectedNode = (Node)enumerator.Current.input.node;
                    }

                    //If the connected node is a dynamic type node
                    if (connectedNode.GetType().IsSubclassOf(typeof(DynamicTypeFlow_Node)))
                    {
                        //Tell the other node to update its type
                        ((DynamicTypeFlow_Node)connectedNode).SetCurrentType(currentType);
                    }
                    else if (connectedNode.GetType().IsSubclassOf(typeof(DynamicType_Node)))
                    {
                        //Tell the other node to update its type
                        ((DynamicType_Node)connectedNode).SetCurrentType(currentType);
                    }
                }
            }

            hasBeenChecked = false;
        }

        protected abstract void setInputPortType(Type newType);
        protected abstract void setOutputPortType(Type newType);
        protected void setPortType(Port_Abstract targetPort, Type newType)
        {
            //If the port type is the same as the new type
            if (targetPort.port.portType == newType || targetPort.port.portType == Glob.GetInstance().GetListType(newType))
            {
                //Dont change anything
                return;
            }
            //If the new type has a color
            if (Glob.GetInstance().TypeColors.ContainsKey(newType))
            {
                //Apply the new type color to the port
                targetPort.SetPortColor(Glob.GetInstance().TypeColors[newType]);
            }

            //If the port is an array type
            if (Glob.GetInstance().IsList(targetPort.port.portType))
            {
                //Turn the new type into an array
                newType = Glob.GetInstance().GetListType(newType);
            }

            //Set the portType of the port
            targetPort.port.portType = newType;
        }

        protected abstract bool IsDynamicInputPort(Port_Abstract otherPort);
        protected abstract bool IsDynamicOutputPort(Port_Abstract otherPort);

        protected bool IsTypeDefined()
        {
            if (GetTypeDefinition() != typeof(object))
            {
                return true;
            }

            return false;
        }

        protected Type GetTypeDefinition()
        {
            if (hasBeenChecked)
            {
                return currentType;
            }
            hasBeenChecked = true;

            Type returnType = GetDynamicOutputPortType();

            if (returnType != typeof(object))
            {
                hasBeenChecked = false;
                return returnType;
            }

            returnType = GetDynamicInputPortType();

            hasBeenChecked = false;
            return returnType;
        }
        protected abstract Type GetDynamicInputPortType();
        protected abstract Type GetDynamicOutputPortType();
        protected Type GetPortTypeDefinition(Port_Abstract port)
        {
            //Set the currentType temporarily to object, to prevent infinite loops between array nodes
            Type typeHolder = currentType;
            currentType = typeof(object);

            //Is the port connected
            if (port.port.connected)
            {
                //Get all the connections to the port
                var enumerator = port.port.connections.GetEnumerator();
                //For every connection
                while (enumerator.MoveNext())
                {
                    //Get the port and node it is connected to
                    Port_Abstract connectedPort;
                    Node connectedNode;

                    if (port.direction == PortDirection.Input)
                    {
                        connectedPort = (Port_Abstract)enumerator.Current.output.parent;
                        connectedNode = (Node)enumerator.Current.output.node;
                    }
                    else
                    {
                        connectedPort = (Port_Abstract)enumerator.Current.input.parent;
                        connectedNode = (Node)enumerator.Current.input.node;
                    }

                    //If the other node is an Array node
                    if (connectedNode.GetType().IsSubclassOf(typeof(DynamicTypeFlow_Node)) || connectedNode.GetType().IsSubclassOf(typeof(DynamicType_Node)))
                    {
                        Type nodeType;
                        if (connectedNode.GetType().IsSubclassOf(typeof(DynamicTypeFlow_Node)))
                        {
                            nodeType = ((DynamicTypeFlow_Node)connectedNode).GetCurrentType();
                        }
                        else
                        {
                            nodeType = ((DynamicType_Node)connectedNode).GetCurrentType();
                        }
                        //If that array node has a type defined
                        if (nodeType != typeof(object))
                        {
                            //This node should have that same type defined, so return the type.
                            currentType = typeHolder;
                            return nodeType;
                        }
                    }
                    else
                    {
                        //If the other port is not of type object
                        if (connectedPort.port.portType != typeof(object))
                        {
                            //There is still a connection that defines the type for this array node, so return the type.
                            currentType = typeHolder;
                            return connectedPort.port.portType;
                        }
                    }
                }
            }

            //No type defining connections were found, so return the object type.
            currentType = typeHolder;
            return typeof(object);
        }

        public Type GetCurrentType()
        {
            if (hasBeenChecked)
            {
                return currentType;
            }

            return GetTypeDefinition();
        }
#endif
    }
}