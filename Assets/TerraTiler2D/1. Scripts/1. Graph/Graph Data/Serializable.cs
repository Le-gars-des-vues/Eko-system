using System;
using UnityEditor;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public static class Serializable
    {
        [Serializable]
        public abstract class SerializableVariable<T>
        {
            public abstract T GetValue();

            public override string ToString()
            {
                if (GetValue() == null)
                {
                    return "null (" + this.GetType() + ") (Serializable)";
                }
                return GetValue().ToString() + " (Serializable)";
            }

            public static implicit operator T(SerializableVariable<T> value)
            {
                return value.GetValue();
            }
        }

        [Serializable]
        public class Vector2 : SerializableVariable<UnityEngine.Vector2>
        {
            [SerializeField]
            public float x, y;

            public Vector2() { x = y = 1f; }

            public Vector2(UnityEngine.Vector2 vector) : this(vector.x, vector.y) { }

            public Vector2(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public override UnityEngine.Vector2 GetValue()
            {
                return new UnityEngine.Vector2(x, y);
            }

            public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
            public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
            public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);
            public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

            public static implicit operator Vector2(UnityEngine.Vector2 value)
            {
                return new Vector2(value);
            }
        }

        [Serializable]
        public class Vector3 : SerializableVariable<UnityEngine.Vector3>
        {
            public float x, y, z;

            public Vector3() { x = y = z = 1f; }

            public Vector3(UnityEngine.Vector3 vector) : this(vector.x, vector.y, vector.z) { }

            public Vector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public override UnityEngine.Vector3 GetValue()
            {
                return new UnityEngine.Vector3(x, y, z);
            }

            public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
            public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
            public static Vector3 operator /(Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

            public static implicit operator Vector3(UnityEngine.Vector3 value)
            {
                return new Vector3(value);
            }
        }

        [Serializable]
        public class Vector4 : SerializableVariable<UnityEngine.Vector4>
        {
            public float x, y, z, w;

            public Vector4() { x = y = z = w = 1f; }

            public Vector4(UnityEngine.Vector4 vector) : this(vector.x, vector.y, vector.z, vector.w) { }

            public Vector4(float x, float y, float z, float w = 1f)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            public override UnityEngine.Vector4 GetValue()
            {
                return new UnityEngine.Vector4(x, y, z, w);
            }

            public static Vector4 operator +(Vector4 a, Vector4 b) => new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
            public static Vector4 operator -(Vector4 a, Vector4 b) => new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
            public static Vector4 operator *(Vector4 a, Vector4 b) => new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
            public static Vector4 operator /(Vector4 a, Vector4 b) => new Vector4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);

            public static implicit operator Vector4(UnityEngine.Vector4 value)
            {
                return new Vector4(value);
            }
        }

        [Serializable]
        public class Color : SerializableVariable<UnityEngine.Color>
        {
            public float r, g, b, a;

            public Color() { r = g = b = a = 1f; }

            public Color(UnityEngine.Color color) : this(color.r, color.g, color.b, color.a) { }

            public Color(float r, float g, float b, float a = 1f)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }

            public override UnityEngine.Color GetValue()
            {
                return new UnityEngine.Color(r, g, b, a);
            }

            public static implicit operator Color(UnityEngine.Color value)
            {
                return new Color(value);
            }
        }

        [Serializable]
        public class Gradient : SerializableVariable<UnityEngine.Gradient>
        {
            [Serializable]
            public struct SerializableGradientAlphaKey
            {
                //
                // Summary:
                //     Alpha channel of key.
                public float alpha;
                //
                // Summary:
                //     Time of the key (0 - 1).
                public float time;

                public SerializableGradientAlphaKey(float alpha, float time)
                {
                    this.time = time;
                    this.alpha = alpha;
                }

                public static implicit operator SerializableGradientAlphaKey(GradientAlphaKey value)
                {
                    return new SerializableGradientAlphaKey(value.alpha, value.time);
                }
                public static implicit operator GradientAlphaKey(SerializableGradientAlphaKey value)
                {
                    return new GradientAlphaKey(value.alpha, value.time);
                }
            }
            [Serializable]
            public struct SerializableGradientColorKey
            {
                //
                // Summary:
                //     Color channel of key.
                public Serializable.Color color;
                //
                // Summary:
                //     Time of the key (0 - 1).
                public float time;

                public SerializableGradientColorKey(Serializable.Color color, float time)
                {
                    this.time = time;
                    this.color = color;
                }
                public SerializableGradientColorKey(UnityEngine.Color color, float time)
                {
                    this.time = time;
                    this.color = color;
                }

                public static implicit operator SerializableGradientColorKey(GradientColorKey value)
                {
                    return new SerializableGradientColorKey(value.color, value.time);
                }
                public static implicit operator GradientColorKey(SerializableGradientColorKey value)
                {
                    return new GradientColorKey(value.color, value.time);
                }
            }

            [SerializeField]
            public SerializableGradientAlphaKey[] alphaKeys;
            [SerializeField]
            public SerializableGradientColorKey[] colorKeys;

            public Gradient()
            {
                alphaKeys = new SerializableGradientAlphaKey[] 
                {
                    new SerializableGradientAlphaKey(1, 0),
                    new SerializableGradientAlphaKey(1, 1),
                };
                colorKeys = new SerializableGradientColorKey[]                
                {
                    new SerializableGradientColorKey(new Color(0, 0, 0, 1), 0),
                    new SerializableGradientColorKey(new Color(1, 1, 1, 1), 1),
                };
            }


            public Gradient(UnityEngine.Gradient gradient)
            {
                if (gradient == null)
                {
                    this.alphaKeys = new Gradient().alphaKeys;
                    this.colorKeys = new Gradient().colorKeys;
                    return;
                }

                this.alphaKeys = GetSerializableAlphaKeys(gradient.alphaKeys);
                this.colorKeys = GetSerializableColorKeys(gradient.colorKeys);
            }

            public override UnityEngine.Gradient GetValue()
            {
                return new UnityEngine.Gradient()
                {
                    alphaKeys = GetAlphaKeys(),
                    colorKeys = GetColorKeys()
                };
            }

            private GradientAlphaKey[] GetAlphaKeys()
            {
                //if (alphaKeys == null)
                //{
                //    alphaKeys = new SerializableGradientAlphaKey[0];
                //}

                GradientAlphaKey[] keys = new GradientAlphaKey[alphaKeys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i] = alphaKeys[i];
                }

                return keys;
            }
            private GradientColorKey[] GetColorKeys()
            {
                GradientColorKey[] keys = new GradientColorKey[colorKeys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i] = colorKeys[i];
                }

                return keys;
            }
            private SerializableGradientAlphaKey[] GetSerializableAlphaKeys(GradientAlphaKey[] keys)
            {
                SerializableGradientAlphaKey[] serializedKeys = new SerializableGradientAlphaKey[keys.Length];
                for (int i = 0; i < serializedKeys.Length; i++)
                {
                    serializedKeys[i] = keys[i];
                }

                return serializedKeys;
            }
            private SerializableGradientColorKey[] GetSerializableColorKeys(GradientColorKey[] keys)
            {
                SerializableGradientColorKey[] serializedKeys = new SerializableGradientColorKey[keys.Length];
                for (int i = 0; i < serializedKeys.Length; i++)
                {
                    serializedKeys[i] = keys[i];
                }

                return serializedKeys;
            }

            public static implicit operator Gradient(UnityEngine.Gradient value)
            {
                return new Gradient(value);
            }
        }

        [Serializable]
        public class Texture2D : SerializableVariable<UnityEngine.Texture2D>
        {
            [SerializeField]
            public string fileName;

            public Texture2D()
            {

            }

            public Texture2D(UnityEngine.Texture2D texture)
            {
                if (texture == null)
                {
                    return;
                }

                this.fileName = Glob.GetInstance().GetGraphResourcesPath() + "/" + texture.name;

                UnityEngine.Texture2D loadedTexture = (Resources.Load(fileName) as UnityEngine.Texture2D);
                if (loadedTexture == null)
                {
                    Glob.GetInstance().DebugString("Failed to serialize Texture2D object '" + texture.name + "'. Make sure the Texture2D object is located at '.../Resources/" + fileName + "'. \nThe next time you load this graph, this object reference will be lost.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                }
            }

            public override UnityEngine.Texture2D GetValue()
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    UnityEngine.Texture2D texture = (Resources.Load(fileName) as UnityEngine.Texture2D);

                    if (texture == null)
                    {
                        Glob.GetInstance().DebugString("No Texture2D object found in the Resources folder at '.../Resources/" + fileName + "'. Make sure the Texture2D object is located at '.../Resources/" + Glob.GetInstance().GetGraphResourcesPath() + "'.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    }

                    return texture;
                }

                return null;
            }

            public static implicit operator Texture2D(UnityEngine.Texture2D value)
            {
                return new Texture2D(value);
            }
        }

        [Serializable]
        public class TileBase : SerializableVariable<UnityEngine.Tilemaps.TileBase>
        {
            [SerializeField]
            public string fileName;

            public TileBase()
            {

            }

            public TileBase(UnityEngine.Tilemaps.TileBase tile)
            {
                if (tile == null)
                {
                    return;
                }

                this.fileName = Glob.GetInstance().GetGraphResourcesPath() + "/" + tile.name;

                UnityEngine.Tilemaps.TileBase loadedTile = (Resources.Load(fileName) as UnityEngine.Tilemaps.TileBase);
                if (loadedTile == null)
                {
                    Glob.GetInstance().DebugString("Failed to serialize Tile object '" + tile.name + "'. Make sure the Tile object is located at '.../Resources/" + fileName + "'. \nThe next time you load this graph, this object reference will be lost.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                }
            }

            public override UnityEngine.Tilemaps.TileBase GetValue()
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    UnityEngine.Tilemaps.TileBase tile = (Resources.Load(fileName) as UnityEngine.Tilemaps.TileBase);

                    if (tile == null)
                    {
                        Glob.GetInstance().DebugString("No Tile object found in the Resources folder at '.../Resources/" + fileName + "'. Make sure the Tile object is located at '.../Resources/" + Glob.GetInstance().GetGraphResourcesPath() + "'.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    }

                    return tile;
                }

                return null;
            }

            public static implicit operator TileBase(UnityEngine.Tilemaps.TileBase value)
            {
                return new TileBase(value);
            }
        }

        [Serializable]
        public class World : SerializableVariable<TerraTiler2D.World>, ICloneable
        {
            public World()
            {

            }

            public World(TerraTiler2D.World world)
            {

            }

            public object Clone()
            {
                return (Serializable.World)GetValue();
            }

            public override TerraTiler2D.World GetValue()
            {
                return new TerraTiler2D.World();
            }

            public static implicit operator World(TerraTiler2D.World value)
            {
                return new World(value);
            }
        }
    }
}