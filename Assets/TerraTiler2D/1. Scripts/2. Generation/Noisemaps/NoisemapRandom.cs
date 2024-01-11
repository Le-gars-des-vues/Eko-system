using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class NoisemapRandom : Noisemap
    {
        public NoisemapRandom()
        {

        }

        protected override Texture2D CreateNoisemap(Vector2 pNoisemapSize)
        {
            Texture2D texture = base.CreateNoisemap(pNoisemapSize);

            int sizeX = texture.width;
            int sizeY = texture.height;

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    texture.SetPixel(x, y, coloring.Evaluate(Random.value));
                }
            }
            texture.Apply();

            return texture;
        }
    }
}
