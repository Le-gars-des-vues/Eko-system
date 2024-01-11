using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TerraTiler2D
{
    [Serializable]
    public abstract class PropertyData_Abstract : IComparable<PropertyData_Abstract>, ICloneable
    {
        //Name of the property.
        [SerializeField]
        public string GUID;

        //Name of the property.
        [SerializeField]
        public string PropertyName;

        //Sorting index of the visual element that displays this property in the blackboard. Does nothing if accessed during runtime.
        //TODO: Implement this. Currently does nothing as the value is never set.
        [SerializeField]
        public int SortingIndex;

        public int CompareTo(PropertyData_Abstract property)
        {
            //If there is nothing to compare to, this object is greater.
            if (property == null)
            {
                return 1;
            }
            else
            {
                return this.SortingIndex.CompareTo(property.SortingIndex);
            }
        }

        public static PropertyData_Abstract GetSerializablePropertyData<T>(T variable)
        {
            if (typeof(T) == typeof(UnityEngine.Vector2))
            {
                return new PropertyData<Serializable.Vector2>()
                {
                    PropertyValue = (Vector2)(variable as object)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Vector3))
            {
                return new PropertyData<Serializable.Vector3>()
                {
                    PropertyValue = (Vector3)(variable as object)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Vector4))
            {
                return new PropertyData<Serializable.Vector4>()
                {
                    PropertyValue = (Vector4)(variable as object)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Color))
            {
                return new PropertyData<Serializable.Color>()
                {
                    PropertyValue = (Color)(variable as object)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Gradient))
            {
                return new PropertyData<Serializable.Gradient>()
                {
                    PropertyValue = (Gradient)(variable as object)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Texture2D))
            {
                return new PropertyData<Serializable.Texture2D>()
                {
                    PropertyValue = (Texture2D)(variable as object)
                };
            }
            else if (typeof(T) == typeof(TileBase))
            {
                return new PropertyData<Serializable.TileBase>()
                {
                    PropertyValue = (TileBase)(variable as object)
                };
            }
            else if (typeof(T) == typeof(World))
            {
                return new PropertyData_Cloneable<Serializable.World>()
                {
                    PropertyValue = (World)(variable as object)
                };
            }
            else if (typeof(T) == typeof(string))
            {
                return new PropertyData_Cloneable<string>()
                {
                    PropertyValue = (string)(variable as object)
                };
            }
            else if (typeof(T).IsSerializable)
            {
                return new PropertyData<T>()
                {
                    PropertyValue = variable
                };
            }
            else
            {
                Debug.LogWarning("Property of type " + typeof(T) + " is not serializable, and the field value can therefore not be saved.");
                return null;
            }
        }
        public static PropertyData_Abstract DeserializePropertyData<T>(PropertyData_Abstract data)
        {
            if (typeof(T) == typeof(Serializable.Vector2))
            {
                return new PropertyData<UnityEngine.Vector2>()
                {
                    PropertyValue = (data as PropertyData<Serializable.Vector2>).PropertyValue.GetValue(),
                    GUID = data.GUID,
                    PropertyName = data.PropertyName,
                    SortingIndex = data.SortingIndex,
                };
            }
            else if (typeof(T) == typeof(Serializable.Vector3))
            {
                return new PropertyData<UnityEngine.Vector3>()
                {
                    PropertyValue = (data as PropertyData<Serializable.Vector3>).PropertyValue.GetValue(),
                    GUID = data.GUID,
                    PropertyName = data.PropertyName,
                    SortingIndex = data.SortingIndex,
                };
            }
            else if (typeof(T) == typeof(Serializable.Vector4))
            {
                return new PropertyData<UnityEngine.Vector4>()
                {
                    PropertyValue = (data as PropertyData<Serializable.Vector4>).PropertyValue.GetValue(),
                    GUID = data.GUID,
                    PropertyName = data.PropertyName,
                    SortingIndex = data.SortingIndex,
                };
            }
            else if (typeof(T) == typeof(Serializable.Color))
            {
                return new PropertyData<UnityEngine.Color>()
                {
                    PropertyValue = (data as PropertyData<Serializable.Color>).PropertyValue.GetValue(),
                    GUID = data.GUID,
                    PropertyName = data.PropertyName,
                    SortingIndex = data.SortingIndex,
                };
            }
            else if (typeof(T) == typeof(Serializable.Gradient))
            {
                return new PropertyData<UnityEngine.Gradient>()
                {
                    PropertyValue = (data as PropertyData<Serializable.Gradient>).PropertyValue.GetValue(),
                    GUID = data.GUID,
                    PropertyName = data.PropertyName,
                    SortingIndex = data.SortingIndex,
                };
            }
            else if (typeof(T) == typeof(Serializable.Texture2D))
            {
                return new PropertyData<UnityEngine.Texture2D>()
                {
                    PropertyValue = (data as PropertyData<Serializable.Texture2D>).PropertyValue.GetValue(),
                    GUID = data.GUID,
                    PropertyName = data.PropertyName,
                    SortingIndex = data.SortingIndex,
                };
            }
            else if (typeof(T) == typeof(Serializable.TileBase))
            {
                return new PropertyData<TileBase>()
                {
                    PropertyValue = (data as PropertyData<Serializable.TileBase>).PropertyValue.GetValue(),
                    GUID = data.GUID,
                    PropertyName = data.PropertyName,
                    SortingIndex = data.SortingIndex,
                };
            }
            else if (typeof(T) == typeof(Serializable.World))
            {
                return new PropertyData_Cloneable<World>()
                {
                    PropertyValue = (data as PropertyData_Cloneable<Serializable.World>).PropertyValue.GetValue(),
                    GUID = data.GUID,
                    PropertyName = data.PropertyName,
                    SortingIndex = data.SortingIndex,
                };
            }
            else if (typeof(T) == typeof(string))
            {
                return new PropertyData_Cloneable<string>()
                {
                    PropertyValue = (data as PropertyData_Cloneable<string>).PropertyValue,
                    GUID = data.GUID,
                    PropertyName = data.PropertyName,
                    SortingIndex = data.SortingIndex,
                };
            }
            else if (typeof(T).IsSerializable)
            {
                return data as PropertyData<T>;
            }
            else
            {
                Debug.LogWarning("Port with field of type " + typeof(T) + " is not serializable, and the field value can therefore not be loaded.");
                return null;
            }
        }

        public abstract Blackboard_Property_Abstract LoadProperty();

        public virtual void CopyPropertyDataFrom(PropertyData_Abstract otherData)
        {
            GUID = otherData.GUID;
            PropertyName = otherData.PropertyName;
            SortingIndex = otherData.SortingIndex;
        }

        public abstract object Clone();
    }

    [Serializable]
    public class PropertyData<T> : PropertyData_Abstract
    {
        //Value of the property.
        [SerializeField]
        public T PropertyValue;

        public override Blackboard_Property_Abstract LoadProperty()
        {
            //Convert Serializable variables to their original counterpart to keep coloring consistent
            if (typeof(T).DeclaringType == typeof(Serializable))
            {
                PropertyData_Abstract deserializedData = PropertyData_Abstract.DeserializePropertyData<T>(this);

                return deserializedData.LoadProperty();
            }
            else
            {
                Blackboard_Property_Abstract property = new Blackboard_Property<T>(PropertyName, PropertyValue, GUID);

                property.LoadPropertyData(this);

                return property;
            }
        }

        public override void CopyPropertyDataFrom(PropertyData_Abstract otherData)
        {
            base.CopyPropertyDataFrom(otherData);

            PropertyValue = (otherData as PropertyData<T>).PropertyValue;
        }

        public override object Clone()
        {
            return new PropertyData<T>()
            {
                GUID = this.GUID,
                PropertyName = this.PropertyName,
                SortingIndex = this.SortingIndex,
                PropertyValue = this.PropertyValue,
            };
        }
    }

    [Serializable]
    public class PropertyData_Cloneable<T> : PropertyData<T> where T : ICloneable
    {
        public override Blackboard_Property_Abstract LoadProperty()
        {
            //Convert Serializable variables to their original counterpart to keep coloring consistent
            if (typeof(T).DeclaringType == typeof(Serializable))
            {
                PropertyData_Abstract deserializedData = PropertyData_Abstract.DeserializePropertyData<T>(this);

                return deserializedData.LoadProperty();
            }
            else
            {
                Blackboard_Property_Abstract property = new Blackboard_Property_Cloneable<T>(PropertyName, PropertyValue, GUID);

                property.LoadPropertyData(this);

                return property;
            }
        }

        public override void CopyPropertyDataFrom(PropertyData_Abstract otherData)
        {
            base.CopyPropertyDataFrom(otherData);

            PropertyValue = (T)(otherData as PropertyData_Cloneable<T>).PropertyValue.Clone();
        }

        public override object Clone()
        {
            return new PropertyData_Cloneable<T>()
            {
                GUID = this.GUID,
                PropertyName = this.PropertyName,
                SortingIndex = this.SortingIndex,
                PropertyValue = this.PropertyValue,
            };
        }
    }
}
