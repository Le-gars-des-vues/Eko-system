using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class NoisemapGradient : Noisemap
    {
        protected Vector2 gradientPosition = new Vector2(0.5f, 0.5f);
        protected Vector2 gradientDirection = new Vector2(0, -1);
        protected float gradientSize = 30f;

        protected Texture2D gradientSizeOffsetNoisemap;
        protected float gradientSizeOffset = 10f;
        protected Texture2D gradientOffsetNoisemap;
        protected float gradientOffset = 10f;

        protected override Texture2D CreateNoisemap(Vector2 pNoisemapSize)
        {
            Texture2D texture = base.CreateNoisemap(pNoisemapSize);

            int sizeX = texture.width;
            int sizeY = texture.height;

            Vector2 actualGradientPosition = new Vector2(sizeX * gradientPosition.x, sizeY * gradientPosition.y);
            Vector2 normalizedDirection = gradientDirection.normalized;
            Vector2 rotatedDirection = rotateVector90Degrees(normalizedDirection);
            Vector2 furthestDotValues = GetFurthestDotValues(rotatedDirection, new Vector2(sizeX, sizeY));

            int offsetTextureSize = 0;

            Color[] noiseMapOffsetPixels = null;
            if (gradientOffsetNoisemap != null)
            {
                noiseMapOffsetPixels = gradientOffsetNoisemap.GetPixels(0, 0, gradientOffsetNoisemap.width, gradientOffsetNoisemap.height);

                if (offsetTextureSize == 0 || offsetTextureSize > gradientOffsetNoisemap.width)
                {
                    offsetTextureSize = gradientOffsetNoisemap.width;
                }
            }
            Color[] noiseMapSizeOffsetPixels = null;
            if (gradientSizeOffsetNoisemap != null)
            {
                noiseMapSizeOffsetPixels = gradientSizeOffsetNoisemap.GetPixels(0, 0, gradientSizeOffsetNoisemap.width, gradientSizeOffsetNoisemap.height);

                if (offsetTextureSize == 0 || offsetTextureSize > gradientSizeOffsetNoisemap.width)
                {
                    offsetTextureSize = gradientSizeOffsetNoisemap.width;
                }
            }

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector2 offset = normalizedDirection;
                    float sizeOffset = gradientSize;

                    int targetPixelIndex = GetCorrespondingOffsetNoisemapPixel(x, y, rotatedDirection, furthestDotValues, offsetTextureSize);

                    if (noiseMapSizeOffsetPixels != null)
                    {
                        sizeOffset += ((((noiseMapSizeOffsetPixels[targetPixelIndex].r + noiseMapSizeOffsetPixels[targetPixelIndex].g + noiseMapSizeOffsetPixels[targetPixelIndex].b) / 3f) - 0.5f) * gradientSizeOffset);
                    }

                    if (noiseMapOffsetPixels != null)
                    {
                        offset *= ((((noiseMapOffsetPixels[targetPixelIndex].r + noiseMapOffsetPixels[targetPixelIndex].g + noiseMapOffsetPixels[targetPixelIndex].b) / 3f) - 0.5f) * gradientOffset);
                    }

                    Vector2 distFromPosition = (new Vector2(x, y) + offset) - actualGradientPosition;

                    float pixelDot = Vector2.Dot(gradientDirection.normalized, distFromPosition);

                    float color = 0;
                    if (Mathf.Abs(pixelDot) <= sizeOffset)
                    {
                        color = 0.5f + (pixelDot / sizeOffset);
                    }
                    else if (pixelDot > 0)
                    {
                        color = 1;
                    }

                    texture.SetPixel(x, y, coloring.Evaluate(color));
                }
            }
            texture.Apply();

            return texture;
        }

        private float GetStepAmount(Vector2 direction, Vector2 textureSize)
        {
            float xSteps = textureSize.x / direction.x;
            float ySteps = textureSize.y / direction.y;

            if (Mathf.Abs(xSteps) < Mathf.Abs(ySteps))
            {
                return xSteps;
            }
            else
            {
                return ySteps;
            }
        }

        //Correct values: x, y, direction (unless rounded), furthestNegativeDotValue, maxSize 
        private int GetCorrespondingOffsetNoisemapPixel(int x, int y, Vector2 direction, Vector2 furthestDotValues, int maxSize = 0)
        {
            float returnValue = 0;
            Vector2 normalizedDirection = direction.normalized;
            Vector2 xAndY = new Vector2(x, y);

            float dotDistance = furthestDotValues.y - furthestDotValues.x;

            //If this vector is longer than 0
            if (xAndY.magnitude != 0)
            {
                //Dot the vector against the direction to get the length along the direction.
                returnValue = Vector2.Dot(xAndY, normalizedDirection);
            }

            //If both X and Y are negative
            if (direction.x <= 0 && direction.y <= 0)
            {
                returnValue += dotDistance-1;
            }
            //If only X is negative
            else if (direction.x < 0 && direction.y >= 0)
            {
                returnValue += Mathf.Floor(dotDistance * 0.5f);
            }
            //If only Y is negative
            else if (direction.x >= 0 && direction.y < 0)
            {
                returnValue += (dotDistance * 0.5f);
            }
            //If both X and Y are positive
            else
            {

            }

            float clampedReturnValue = Mathf.Lerp(0, maxSize, Mathf.Clamp(returnValue / dotDistance, 0, 1));

            return Mathf.FloorToInt(clampedReturnValue);
        }
        private Vector2 rotateVector90Degrees(Vector2 target)
        {
            return new Vector2(target.y, -target.x);
        }
        private float GetFurthestDotValue(Vector2 direction, Vector2 textureSize, bool negative = true)
        {
            float returnValue = 0;

            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    float dot = Vector2.Dot(new Vector2(textureSize.x * x, textureSize.y * y), direction);

                    if (negative)
                    {
                        if (dot < returnValue)
                        {
                            returnValue = dot;
                        }
                    }
                    else
                    {
                        if (dot > returnValue)
                        {
                            returnValue = dot;
                        }
                    }
                }
            }

            return returnValue;
        }
        private Vector2 GetFurthestDotValues(Vector2 direction, Vector2 textureSize)
        {
            return new Vector2(GetFurthestDotValue(direction, textureSize, true), GetFurthestDotValue(direction, textureSize, false));
        }

        public Vector2 GetGradientPosition()
        {
            return gradientPosition;
        }
        public void SetGradientPosition(Vector2 value)
        {
            gradientPosition = value;
        }
        public Vector2 GetGradientDirection()
        {
            return gradientDirection;
        }
        public void SetGradientDirection(Vector2 value)
        {
            gradientDirection = value;
        }
        public float GetGradientSize()
        {
            return gradientSize;
        }
        public void SetGradientSize(float value)
        {
            gradientSize = value;
        }
        public Texture2D GetGradientSizeOffsetNoisemap()
        {
            return gradientSizeOffsetNoisemap;
        }
        public void SetGradientSizeOffsetNoisemap(Texture2D value)
        {
            gradientSizeOffsetNoisemap = value;
        }
        public float GetGradientSizeOffset()
        {
            return gradientSizeOffset;
        }
        public void SetGradientSizeOffset(float value)
        {
            gradientSizeOffset = value;
        }
        public Texture2D GetGradientOffsetNoisemap()
        {
            return gradientOffsetNoisemap;
        }
        public void SetGradientOffsetNoisemap(Texture2D value)
        {
            gradientOffsetNoisemap = value;
        }
        public float GetGradientOffset()
        {
            return gradientOffset;
        }
        public void SetGradientOffset(float value)
        {
            gradientOffset = value;
        }
    }
}
