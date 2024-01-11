using System;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public abstract class Blackboard_Property_Abstract 
    {
        //Unique Guid of the property
        public string Guid;
        
        //Name of the property.
        public string PropertyName;

        //Sorting index of the visual element that displays this property in the blackboard. Does nothing if accessed during runtime.
        public int SortingIndex;

#if (UNITY_EDITOR)
        //Visual element that displays this property in the blackboard. Does nothing if accessed during runtime.
        public VisualElement PropertyElement;
#endif
        public Blackboard_Property_Abstract(string name, string guid = null)
        {
            //If no GUID was passed into this constructor, generate a new unique GUID for this property.
            if (guid == null)
            {
#if (UNITY_EDITOR)
                this.Guid = GUID.Generate().ToString();
#endif
            }
            else
            {
                //Otherwise, use the GUID that was passed into this constructor.
                this.Guid = guid;
            }

            PropertyName = name;

            EventManager.GetInstance().RaiseEvent(new PropertyCreatedEvent().Init(GetPropertyData()));
        }

        public abstract object GetPropertyValueAsObject();

        public abstract void ResetProperty();

        public abstract PropertyData_Abstract GetPropertyData();

        public abstract void LoadPropertyData(PropertyData_Abstract data);

        public abstract Type GetPropertyType();
    }

    public class Blackboard_Property<T> : Blackboard_Property_Abstract
    {
        //Persistent value of the property.
        protected T PropertyValue;

        protected T nonPersistentPropertyValue;

        public Blackboard_Property(string name, T value, string guid = null) : base(name, guid)
        {
            SetPropertyValue(value, true);
        }

        public override object GetPropertyValueAsObject()
        {
            return GetPropertyValue();
        }

        public T GetPropertyValue()
        {
            return nonPersistentPropertyValue;
        }
        public T SetPropertyValue(T value, bool setPersistent = false)
        {
            if (setPersistent)
            {
                PropertyValue = value;

                //If the property value is changed, raise an event to inform the listeners
                EventManager.GetInstance().RaiseEvent(new PropertyChangedEvent().Init(GetPropertyData()));
            }

            setNonPersistentPropertyValue(value);

            return nonPersistentPropertyValue;
        }
        protected virtual T setNonPersistentPropertyValue(T value)
        {
            nonPersistentPropertyValue = value;

            return nonPersistentPropertyValue;
        }

        public override void ResetProperty()
        {
            setNonPersistentPropertyValue(PropertyValue);
        }

        public override PropertyData_Abstract GetPropertyData()
        {
            PropertyData_Abstract data = PropertyData_Abstract.GetSerializablePropertyData(this.PropertyValue);

            data.GUID = this.Guid;
            data.PropertyName = this.PropertyName;
            data.SortingIndex = this.SortingIndex;

            return data;
        }
        public override void LoadPropertyData(PropertyData_Abstract data)
        {
            PropertyData<T> propertyData = data as PropertyData<T>;

            if (propertyData != null)
            {
                this.Guid = propertyData.GUID;
                this.PropertyName = propertyData.PropertyName;
                this.SortingIndex = propertyData.SortingIndex;
                this.SetPropertyValue(propertyData.PropertyValue, true);
            }
        }
        public override Type GetPropertyType()
        {
            return typeof(T);
        }
    }

    public class Blackboard_Property_Cloneable<T> : Blackboard_Property<T> where T : ICloneable
    {
        public Blackboard_Property_Cloneable(string name, T value, string guid = null) : base(name, value, guid)
        {

        }

        protected override T setNonPersistentPropertyValue(T value)
        {
            if (value != null)
            {
                nonPersistentPropertyValue = (T)value.Clone();
            }
            else
            {
                nonPersistentPropertyValue = value;
            }

            return nonPersistentPropertyValue;
        }

        public override PropertyData_Abstract GetPropertyData()
        {
            PropertyData_Abstract data = PropertyData_Abstract.GetSerializablePropertyData(this.PropertyValue);

            data.GUID = this.Guid;
            data.PropertyName = this.PropertyName;
            data.SortingIndex = this.SortingIndex;

            return data;
        }
        public override void LoadPropertyData(PropertyData_Abstract data)
        {
            PropertyData_Cloneable<T> propertyData = data as PropertyData_Cloneable<T>;

            if (propertyData != null)
            {
                this.Guid = propertyData.GUID;
                this.PropertyName = propertyData.PropertyName;
                this.SortingIndex = propertyData.SortingIndex;
                this.SetPropertyValue(propertyData.PropertyValue, true);
            }
        }
    }
}
