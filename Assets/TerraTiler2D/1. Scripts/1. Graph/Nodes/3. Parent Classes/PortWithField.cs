using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
#if (UNITY_EDITOR)
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
#endif

namespace TerraTiler2D {
    public class PortWithField<T> : Port<T>
    {
        [SerializeField]
        protected T value;
#if (UNITY_EDITOR)
        protected BaseField<T> valueField;
        protected ObjectField objectField;
#endif

        public PortWithField(Node node, string portName, PortDirection portDirection, System.Type portType, T defaultValue, PortCapacity capacity = PortCapacity.Single, bool isMandatory = true, string tooltip = "") : base(node, portName, portDirection, portType, capacity, isMandatory, tooltip)
        {
#if (UNITY_EDITOR)
            AddFieldToPort(port);
#endif
            if (defaultValue != null)
            {
                SetValue(defaultValue);
            }
        }
#if (UNITY_EDITOR)
        protected void AddFieldToPort(Port port)
        {
            if (typeof(T) == typeof(int))
            {
                valueField = new IntegerField(Glob.GetInstance().MaxFieldInputLength) as BaseField<T>;
            }
            else if (typeof(T) == typeof(float))
            {
                valueField = new FloatField(Glob.GetInstance().MaxFieldInputLength) as BaseField<T>;
            }
            else if (typeof(T) == typeof(string))
            {
                valueField = new TextField
                {
                    multiline = true
                } as BaseField<T>;
            }
            else if (typeof(T) == typeof(bool))
            {
                valueField = new Toggle() as BaseField<T>;
            }
            else if (typeof(T) == typeof(Vector2))
            {
                valueField = new Vector2Field() as BaseField<T>;

                valueField.style.minWidth = Glob.GetInstance().MinNodeFieldSize;
                valueField.style.maxWidth = Glob.GetInstance().MaxNodeFieldSize;
                (valueField as Vector2Field).style.flexDirection = FlexDirection.Column;
            }
            else if (typeof(T) == typeof(Vector3))
            {
                valueField = new Vector3Field() as BaseField<T>;

                valueField.style.minWidth = Glob.GetInstance().MinNodeFieldSize;
                valueField.style.maxWidth = Glob.GetInstance().MaxNodeFieldSize;
                (valueField as Vector3Field).style.flexDirection = FlexDirection.Column;
            }
            else if (typeof(T) == typeof(Vector4))
            {
                valueField = new Vector4Field() as BaseField<T>;

                valueField.style.minWidth = Glob.GetInstance().MinNodeFieldSize;
                valueField.style.maxWidth = Glob.GetInstance().MaxNodeFieldSize;
                (valueField as Vector4Field).style.flexDirection = FlexDirection.Column;
            }
            else if (typeof(T) == typeof(Color))
            {
                valueField = new ColorField() { value = new Color(1,1,1,1) } as BaseField<T>;
                valueField.style.minWidth = Glob.GetInstance().MinNodeFieldSize;
                valueField.style.maxWidth = Glob.GetInstance().MaxNodeFieldSize;
            }
            else if (typeof(T) == typeof(Gradient))
            {
                valueField = new GradientField()
                {
                    value = new Gradient()
                    {
                        alphaKeys = new GradientAlphaKey[]
                        {
                            new GradientAlphaKey(1.0f, 0.0f),
                            new GradientAlphaKey(1.0f, 1.0f),
                        },
                            colorKeys = new GradientColorKey[]
                        {
                            new GradientColorKey(Color.black, 0.0f),
                            new GradientColorKey(Color.white, 1.0f),
                        }
                    }
                } as BaseField<T>;

                valueField.style.minWidth = Glob.GetInstance().MaxNodeFieldSize;
                valueField.style.maxWidth = Glob.GetInstance().MaxNodeFieldSize;
            }
            else if (typeof(T) == typeof(Texture2D))
            {
                objectField = new ObjectField()
                {
                    objectType = typeof(Texture2D)
                };

                objectField.style.minWidth = Glob.GetInstance().MinNodeFieldSize;
                objectField.style.maxWidth = Glob.GetInstance().MaxNodeFieldSize;
            }
            else if (typeof(T) == typeof(TileBase))
            {
                objectField = new ObjectField()
                {
                    objectType = typeof(TileBase)
                };

                objectField.style.minWidth = Glob.GetInstance().MinNodeFieldSize;
                objectField.style.maxWidth = Glob.GetInstance().MaxNodeFieldSize;
            }

            //Rearrange the children, so that the field is placed between the port and the label.
            port.Add(valueField);
            port.Add(objectField);

            //Register a listener, which applies changes to the real variables.
            valueField.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            objectField.RegisterValueChangedCallback(evt => SetValue(evt.newValue));

            SetValue(value);
        }
#endif

        public void SetValue(T newValue)
        {
            //Set the value
            value = newValue;

#if (UNITY_EDITOR)
            if (valueField != null)
            {
                valueField.SetValueWithoutNotify(value);
            }
            else if (objectField != null)
            {
                objectField.SetValueWithoutNotify(value as UnityEngine.Object);
            }

            if (valueField != null || objectField != null)
            {
                //Send an event announcing that this port's value changed
                PortData portData = GetPortData();
                portData.NodeGUID = node.Guid;
                portData.PortGUID = Guid;
                EventManager.GetInstance().RaiseEvent(new PortChangedEvent().Init(portData));
            }
#endif
        }
        public void SetValue(object newValue)
        {
            SetValue((T)newValue);
        }

        public T GetValue()
        {
            return value;
        }

        public override void CopyPort(Port_Abstract copyFrom)
        {
            if (copyFrom.GetType() == typeof(PortWithField<T>))
            {
                SetValue(((PortWithField<T>)copyFrom).GetValue());
            }
        }

        public override object GetPortVariable()
        {
            object returnValue = base.GetPortVariable();

            if (returnValue == null)
            {
                returnValue = GetValue();
            }

            return returnValue;
        }
#if (UNITY_EDITOR)
        public override void HandleConnected(Port_Abstract otherPort)
        {
            base.HandleConnected(otherPort);

            if (port.connected)
            {
                if (port.direction == Direction.Input)
                {
                    if (valueField != null)
                    {
                        valueField.SetEnabled(false);
                    }
                    if (objectField != null)
                    {
                        objectField.SetEnabled(false);
                    }
                }
            }
        }
        public override void HandleDisconnected(UnityEditor.Experimental.GraphView.Edge edge)
        {
            base.HandleDisconnected(edge);

            if (!port.connected)
            {
                if (valueField != null)
                {
                    valueField.SetEnabled(true);
                }
                if (objectField != null)
                {
                    objectField.SetEnabled(true);
                }
            }
        }
#endif

        public override PortData GetPortData()
        {
            return PortWithFieldData<T>.GetSerializablePortData(this.value);
        }
        public override void LoadPortData(PortData data)
        {
            PortWithFieldData<T> portData = PortWithFieldData<T>.DeserializePortData(data);
            if (portData != null)
            {
                if (portData.value != null)
                {
                    SetValue(portData.value);
                }
            }

            base.LoadPortData(data);
        }
    }
}