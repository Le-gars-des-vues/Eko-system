using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class NoisemapValue : Noisemap
    {
        protected Vector2 position = new Vector2(0, 0);

        protected NoisemapDimension dimensions = NoisemapDimension.BothXAndY;

        //[Tooltip("How often should the noisemap change color along the X axis. A smaller value will result in bigger shapes.")]
        protected float xFrequency = 10f;

        //[Tooltip("How often should the noisemap change color along the Y axis. A smaller value will result in bigger shapes.")]
        protected float yFrequency = 10f;

        //[Tooltip("How many fractals should the noisemap have.\n" +
        //            "A higher value results in a noisemap with more detail.\n" +
        //            "You can see this as adding a second noisemap on top of the previous.")]
        protected int octaves = 2;

        //[Tooltip("By what factor should the frequency be multiplied, per octave.\n" +
        //            "A higher value will add smaller details per octave.")]
        protected float lacunarity = 2f;

        //[Tooltip("By what factor should the opacity be multiplied, per octave.\n" +
        //            "A higher value will make smaller details more apparent, but may obstruct bigger shapes from early octaves.")]
        protected float persistence = 0.5f;

        protected bool smooth = true;

        protected override Texture2D CreateNoisemap(Vector2 pNoisemapSize)
        {
            Texture2D texture = base.CreateNoisemap(pNoisemapSize);

            if (pNoisemapSize != noisemapSize)
            {
                Glob.GetInstance().DebugString("Target world is not the same size as this noisemap. Scaling the noisemap to the same size.", Glob.DebugCategories.Misc, Glob.DebugLevel.User, Glob.DebugTypes.Default);

                //Make sure the frequency is scaled by the same factor as the noisemap.
                xFrequency *= (pNoisemapSize.x / noisemapSize.x);
                yFrequency *= (pNoisemapSize.y / noisemapSize.y);
            }

            Vector3 point00 = new Vector3(-0.5f, -0.5f) + (Vector3)hashOffset + (Vector3)(position / noisemapSize);
            Vector3 point10 = new Vector3(0.5f, -0.5f) + (Vector3)hashOffset + (Vector3)(position / noisemapSize);
            Vector3 point01 = new Vector3(-0.5f, 0.5f) + (Vector3)hashOffset + (Vector3)(position / noisemapSize);
            Vector3 point11 = new Vector3(0.5f, 0.5f) + (Vector3)hashOffset + (Vector3)(position / noisemapSize);

            NoiseMethod valueMethod = Noise.valueMethods[2];
            switch (dimensions)
            {
                case NoisemapDimension.OnlyX:
                    valueMethod = Noise.valueMethods[0];
                    break;
                case NoisemapDimension.OnlyY:
                    valueMethod = Noise.valueMethods[1];
                    break;
                case NoisemapDimension.BothXAndY:
                    valueMethod = Noise.valueMethods[2];
                    break;
            }

            int sizeX = texture.width;
            int sizeY = texture.height;

            float stepSizeX = 1f / sizeX;
            float stepSizeY = 1f / sizeY;
            for (int y = 0; y < sizeY; y++)
            {
                Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSizeY);
                Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSizeY);
                for (int x = 0; x < sizeX; x++)
                {
                    Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSizeX);
                    float sample = Noise.Sum(valueMethod, point, xFrequency, yFrequency, octaves, lacunarity, persistence, smooth);
                    texture.SetPixel(x, y, coloring.Evaluate(sample));
                }
            }
            texture.Apply();

            return texture;
        }

        public Vector2 GetPosition()
        {
            return position;
        }
        public void SetPosition(Vector2 value)
        {
            position = value;
        }
        public NoisemapDimension GetDimensions()
        {
            return dimensions;
        }
        public void SetDimensions(NoisemapDimension value)
        {
            dimensions = value;
        }
        public float GetXFrequency()
        {
            return xFrequency;
        }
        public void SetXFrequency(float value)
        {
            xFrequency = value;
        }
        public float GetYFrequency()
        {
            return yFrequency;
        }
        public void SetYFrequency(float value)
        {
            yFrequency = value;
        }
        public int GetOctaves()
        {
            return octaves;
        }
        public void SetOctaves(int value)
        {
            octaves = value;
        }
        public float GetLacunarity()
        {
            return lacunarity;
        }
        public void SetLacunarity(float value)
        {
            lacunarity = value;
        }
        public float GetPersistence()
        {
            return persistence;
        }
        public void SetPersistence(float value)
        {
            persistence = value;
        }
        public bool GetSmooth()
        {
            return smooth;
        }
        public void SetSmooth(bool value)
        {
            smooth = value;
        }
    }
}
