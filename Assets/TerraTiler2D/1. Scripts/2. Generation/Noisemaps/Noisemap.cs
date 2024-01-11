using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    /// <summary>
    /// Base class for noisemap types.	
    /// </summary>
    public abstract class Noisemap
    {
        protected Vector2 noisemapSize = new Vector2(100, 100);
        protected bool generatePerConnection = true;

        protected Vector2 hashOffset = new Vector2(10, 10);
        protected Gradient coloring = new Gradient()
        {
            // The number of keys must be specified in this array initialiser
            colorKeys = new GradientColorKey[2] {
            // Add your colour and specify the stop point
            new GradientColorKey(Color.black, 0),
            new GradientColorKey(Color.white, 1)
        },
            // This sets the alpha to 1 at both ends of the gradient
            alphaKeys = new GradientAlphaKey[2] {
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1)
        }
        };

        public virtual Texture2D CreateNoisemap()
        {
            return CreateNoisemap(noisemapSize);
        }
        protected virtual Texture2D CreateNoisemap(Vector2 pNoisemapSize)
        {
            hashOffset = new Vector2(Random.Range(0, Noise.HashMaskLength()), Random.Range(0, Noise.HashMaskLength()));

            //Make sure the noisemap is at least 1 pixel in each axis.
            int sizeX = Mathf.Max((int)pNoisemapSize.x, 1);
            int sizeY = Mathf.Max((int)pNoisemapSize.y, 1);
            Texture2D texture;

            texture = new Texture2D(sizeX, sizeY, TextureFormat.RGB24, true);
            texture.name = "Procedural Noisemap";
            texture.wrapMode = TextureWrapMode.Clamp;

            if (texture.width != sizeX || texture.height != sizeY)
            {
                texture.Reinitialize(sizeX, sizeY);
            }

            return texture;
        }

        public Vector2 GetNoisemapSize()
        {
            return noisemapSize;
        }
        public void SetNoisemapSize(Vector2 newSize)
        {
            newSize.x = Mathf.Max(newSize.x, 0);
            newSize.y = Mathf.Max(newSize.y, 0);

            noisemapSize = newSize;
        }
        public bool GetUpdateOnChange()
        {
            return generatePerConnection;
        }
        public void SetGeneratePerConnection(bool toggle)
        {
            generatePerConnection = toggle;
        }
        public Gradient GetColoring()
        {
            return coloring;
        }
        public void SetColoring(Gradient newGradient)
        {
            coloring = newGradient;
        }
    }
}
