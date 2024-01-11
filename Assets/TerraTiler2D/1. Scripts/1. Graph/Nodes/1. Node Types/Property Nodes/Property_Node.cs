using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public abstract class Property_Node<T> : Node
    {
        private Port<T> propertyPort;

        private Blackboard_Property<T> property;

        public Property_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            //GraphView graphView = (GraphView)Glob.GetInstance().GetFirstParentVisualElementOfType(this, typeof(GraphView));

            //if (graphView != null)
            //{
            //    property = graphView.GetGraphBlackboard().GetProperty(nodeName) as Blackboard_Property<T>;
            //}

            //Handle name changes, and property deletion
#if (UNITY_EDITOR)
            EventManager.GetInstance().AddListener<PropertyChangedEvent>(UpdateNodeName);
            EventManager.GetInstance().AddListener<PropertyDeletedEvent>(HandlePropertyDeleted);

            //Make the padding smaller
            GetContainer(NodeContainers.OutputContainer).style.marginLeft = -4;

            GetContainer(NodeContainers.OutputContainer).style.height = Glob.GetInstance().NodeTitleContainerHeight;

            GetContainer(NodeContainers.OutputContainer).style.backgroundImage = Glob.GetInstance().Property_9Tile_Icon;
            if (Glob.GetInstance().TypeColors.ContainsKey(typeof(T)))
            {
                GetContainer(NodeContainers.OutputContainer).style.unityBackgroundImageTintColor = Glob.GetInstance().TypeColors[typeof(T)];
            }

            //Remove the header
            GetContainer(NodeContainers.TitleContainer).RemoveFromHierarchy();
#endif
        }
        public void SetProperty(Blackboard_Property_Abstract property)
        {
            this.property = property as Blackboard_Property<T>;

            //Set the name of the port to the property name
            if (property != null)
            {
#if (UNITY_EDITOR)
                UpdateNodeName(null);
#endif
                //EventManager.GetInstance().RaiseEvent(new PropertyChangedEvent().Init(property.GetPropertyData()));
            }
        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            string name = "";

            propertyPort = GeneratePort<T>(name, PortDirection.Output, "PropertyOutput", PortCapacity.Multi, false);
            propertyPort.SetOutputPortMethod(GetPropertyValue);

#if (UNITY_EDITOR)
            name = GetTitle();
            ((Label)Glob.GetInstance().GetFirstChildVisualElementOfType(propertyPort.port, typeof(Label))).style.color = Color.black;
#endif
        }

        private object GetPropertyValue()
        {
            if (property == null)
            {
                return null;
            }
            return property.GetPropertyValue();
        }

#if (UNITY_EDITOR)
        private void UpdateNodeName(PropertyChangedEvent evt)
        {
            if (property != null)
            {
                propertyPort.port.portName = property.PropertyName;

                SetTitle(property.PropertyName);

                if (evt != null)
                {
                    EventManager.GetInstance().RaiseEvent(new NodeChangedEvent().Init(this.GetNodeData()));
                }
            }
        }
        private void HandlePropertyDeleted(PropertyDeletedEvent evt)
        {
            if (property != null)
            {
                //If the deleted property was this node's property
                if (evt.propertyData.GUID == property.Guid)
                {
                    DeleteNode();
                }
            }
        }
#endif

        protected override void InitializeAdditionalElements()
        {

        }
    }
}
