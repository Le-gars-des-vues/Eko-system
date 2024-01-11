using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TerraTiler2D
{
    [Serializable]
    public class PortData : ICloneable
    {
        [SerializeField]
        public string PortGUID;
        [SerializeField]
        public string NodeGUID;

        public virtual void CopyPortDataFrom(PortData otherData)
        {
            PortGUID = otherData.PortGUID;
            NodeGUID = otherData.NodeGUID;
        }

        public virtual object Clone()
        {
            return new PortData()
            {
                PortGUID = this.PortGUID,
                NodeGUID = this.NodeGUID,
            };
        }
    }

    [Serializable]
    public class PortWithFieldData<T> : PortData
    {
        [SerializeField]
        public T value;

        public static PortData GetSerializablePortData(T variable)
        {
            if (typeof(T).IsSerializable)
            {
                return new PortWithFieldData<T>()
                {
                    value = variable
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Vector2))
            {
                return new PortWithFieldData<Serializable.Vector2>()
                {
                    value = (Vector2)(variable as object)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Vector3))
            {
                return new PortWithFieldData<Serializable.Vector3>()
                {
                    value = (Vector3)(variable as object)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Vector4))
            {
                return new PortWithFieldData<Serializable.Vector4>()
                {
                    value = (Vector4)(variable as object)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Color))
            {
                return new PortWithFieldData<Serializable.Color>()
                {
                    value = (Color)(variable as object)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Gradient))
            {
                return new PortWithFieldData<Serializable.Gradient>()
                {
                    value = (variable as Gradient)
                };
            }
            else if (typeof(T) == typeof(UnityEngine.Texture2D))
            {
                return new PortWithFieldData<Serializable.Texture2D>()
                {
                    value = (variable as Texture2D)
                };
            }
            else if (typeof(T) == typeof(TileBase))
            {
                return new PortWithFieldData<Serializable.TileBase>()
                {
                    value = (variable as TileBase)
                };
            }
            else if (typeof(T) == typeof(World))
            {
                //Ports of World type will never have a field, but the PropertySetter_Node requires the input value port to be a PortWithField, which will cause this method to throw warnings.
                //This return is only here to prevent those specific warnings from appearing.
                return new PortWithFieldData<World>()
                {
                    value = (variable as World)
                };
            }
            else
            {
                Glob.GetInstance().DebugString("Port with field of type " + typeof(T) + " is not serializable, and the field value can therefore not be saved.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                return null;
            }
        }
        public static PortWithFieldData<T> DeserializePortData(PortData data)
        {
            if (typeof(T).IsSerializable)
            {
                return data as PortWithFieldData<T>;
            }
            else if (typeof(T) == typeof(UnityEngine.Vector2))
            {
                return new PortWithFieldData<UnityEngine.Vector2>()
                {
                    value = (data as PortWithFieldData<Serializable.Vector2>).value.GetValue()
                } as PortWithFieldData<T>;
            }
            else if (typeof(T) == typeof(UnityEngine.Vector3))
            {
                return new PortWithFieldData<UnityEngine.Vector3>()
                {
                    value = (data as PortWithFieldData<Serializable.Vector3>).value.GetValue()
                } as PortWithFieldData<T>;
            }
            else if (typeof(T) == typeof(UnityEngine.Vector4))
            {
                return new PortWithFieldData<UnityEngine.Vector4>()
                {
                    value = (data as PortWithFieldData<Serializable.Vector4>).value.GetValue()
                } as PortWithFieldData<T>;
            }
            else if (typeof(T) == typeof(UnityEngine.Color))
            {
                return new PortWithFieldData<UnityEngine.Color>()
                {
                    value = (data as PortWithFieldData<Serializable.Color>).value.GetValue()
                } as PortWithFieldData<T>;
            }
            else if (typeof(T) == typeof(UnityEngine.Gradient))
            {
                return new PortWithFieldData<UnityEngine.Gradient>()
                {
                    value = (data as PortWithFieldData<Serializable.Gradient>).value.GetValue()
                } as PortWithFieldData<T>;
            }
            else if (typeof(T) == typeof(UnityEngine.Texture2D))
            {
                return new PortWithFieldData<UnityEngine.Texture2D>()
                {
                    value = (data as PortWithFieldData<Serializable.Texture2D>).value.GetValue()
                } as PortWithFieldData<T>;
            }
            else if (typeof(T) == typeof(TileBase))
            {
                return new PortWithFieldData<TileBase>()
                {
                    value = (data as PortWithFieldData<Serializable.TileBase>).value.GetValue()
                } as PortWithFieldData<T>;
            }
            else if (typeof(T) == typeof(World))
            {
                //Ports of World type will never have a field, but the PropertySetter_Node requires the input value port to be a PortWithField, which will cause this method to throw warnings.
                //This return is only here to prevent those specific warnings from appearing.
                return data as PortWithFieldData<T>;
            }
            else
            {
                Debug.LogWarning("Port with field of type " + typeof(T) + " is not serializable, and the field value can therefore not be loaded.");
                return null;
            }
        }

        public override void CopyPortDataFrom(PortData otherData)
        {
            base.CopyPortDataFrom(otherData);

            value = (otherData as PortWithFieldData<T>).value;
        }

        public override object Clone()
        {
            return new PortWithFieldData<T>()
            {
                PortGUID = this.PortGUID,
                NodeGUID = this.NodeGUID,
                value = this.value,
            };
        }
    }
}
