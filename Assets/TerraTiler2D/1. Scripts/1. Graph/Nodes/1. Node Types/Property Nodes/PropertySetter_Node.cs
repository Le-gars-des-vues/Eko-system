using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public abstract class PropertySetter_Node<T> : Flow_Node
    {
        private Port<T> propertyPort;
        private PortWithField<T> valuePort;

        private Blackboard_Property<T> property;

        public PropertySetter_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
#if (UNITY_EDITOR)
            //Handle name changes, and property deletion
            EventManager.GetInstance().AddListener<PropertyChangedEvent>(UpdateNodeName);
            EventManager.GetInstance().AddListener<PropertyDeletedEvent>(HandlePropertyDeleted);

            if (Glob.GetInstance().TypeColors.ContainsKey(typeof(T)))
            {
                GetContainer(NodeContainers.TitleContainer).style.backgroundColor = Glob.GetInstance().TypeColors[typeof(T)];
            }

            ((Label)Glob.GetInstance().GetFirstChildVisualElementOfType(GetContainer(NodeContainers.TitleContainer), typeof(Label))).style.color = Color.black;
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
            }
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            valuePort = GeneratePortWithField("New value", PortDirection.Input, default(T), "Value", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();

            string name = "";
            name = GetTitle();

            propertyPort = GeneratePort<T>(name, PortDirection.Output, "PropertyOutput", PortCapacity.Multi, false);
            propertyPort.SetOutputPortMethod(GetPropertyValue);
        }

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            if (property != null)
            {

                //bool isPersistent = (bool)persistentPort.GetPortVariable();

                property.SetPropertyValue((T)valuePort.GetPortVariable());//, isPersistent);

                //if (isPersistent)
                //{
                //    //TODO: Find a way to save the graph without referencing Graph.Instance, as that does not work during runtime.
                //    //Save the graph after a property was set persistently
                //    Graph.Instance.RequestDataOperation(true);
                //}
            }

            base.ApplyBehaviour(flow, trickleDown);
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
                    if (evt.propertyData.PropertyName == property.PropertyName)
                    {
                        EventManager.GetInstance().RaiseEvent(new NodeChangedEvent().Init(this.GetNodeData()));
                    }
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
