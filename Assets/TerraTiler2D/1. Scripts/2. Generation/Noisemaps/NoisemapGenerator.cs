using UnityEngine;

//Big thanks to Jasper Flick for the tutorial:
//https://catlikecoding.com/unity/tutorials/noise/

namespace TerraTiler2D
{
    public delegate float NoiseMethod(Vector2 point, float frequencyX, float frequencyY, bool smooth = true);

    /// <summary>
    /// Static library of noise functions.   	
    /// </summary>
    public static class Noise
    {
        private static int[] hash = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };
        private const int hashMask = 255;
        public static int HashMaskLength()
        {
            return hashMask;
        }

        public static float Sum(NoiseMethod method, Vector2 point, float frequencyX, float frequencyY, int octaves, float lacunarity, float persistence, bool smooth = true)
        {
            float sum = method(point, frequencyX, frequencyY, smooth);
            float amplitude = 1f;
            float range = 1f;
            for (int o = 1; o < octaves; o++)
            {
                frequencyX *= lacunarity;
                frequencyY *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += method(point, frequencyX, frequencyY, smooth) * amplitude;
            }
            return sum / range;
        }

        private static float sqr2 = Mathf.Sqrt(2f);

        private static float Smooth(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }
        private static float Dot(Vector2 gradient, float x, float y)
        {
            return gradient.x * x + gradient.y * y;
        }

        public static NoiseMethod[] valueMethods = {
            Value1D_X,
            Value1D_Y,
            Value2D,
        };
        public static NoiseMethod[] perlinMethods = {
            Perlin1D_X,
            Perlin1D_Y,
            Perlin2D,
        };

        private static float[] gradients1D = {
            1f, -1f
        };

        private const int gradientsMask1D = 1;

        private static Vector2[] gradients2D = {
            new Vector2( 1f, 0f),
            new Vector2(-1f, 0f),
            new Vector2( 0f, 1f),
            new Vector2( 0f,-1f),

            new Vector2( 1f, 1f).normalized,
            new Vector2(-1f, 1f).normalized,
            new Vector2( 1f,-1f).normalized,
            new Vector2(-1f,-1f).normalized,

            new Vector2( 0.5f, 1f).normalized,
            new Vector2( 1f, 0.5f).normalized,
            new Vector2(-0.5f, 1f).normalized,
            new Vector2(-1f, 0.5f).normalized,
            new Vector2( 0.5f,-1f).normalized,
            new Vector2( 1f,-0.5f).normalized,
            new Vector2(-0.5f,-1f).normalized,
            new Vector2(-1f,-0.5f).normalized
        };

        private const int gradientsMask2D = 15;


        public static float Value1D_X(Vector2 point, float frequencyX, float frequencyY, bool smooth = true)
        {
            //Apply the frequency.
            point.x *= frequencyX;
            point.y *= frequencyY;

            //Store the passed in X coordinate.
            int currentCoordinate = Mathf.FloorToInt(point.x);
            //t for smoothing.
            float t = point.x - currentCoordinate;
            //Make sure the value falls within the length of the hashMask.
            currentCoordinate &= hashMask;

            //Get the hash representing this value.
            int currentHash = hash[currentCoordinate];

            //Should the result be smoothed
            if (smooth)
            {
                //Get the next hash value, and interpolate between the current and the next value.
                int nextCoordinate = currentCoordinate + 1;
                int nextHash = hash[nextCoordinate];
                t = Smooth(t);
                return Mathf.Lerp(currentHash, nextHash, t) * (1f / hashMask);
            }
            else
            {
                return currentHash * (1f / hashMask);
            }
        }

        public static float Value1D_Y(Vector2 point, float frequencyX, float frequencyY, bool smooth = true)
        {
            //Apply the frequency.
            point.x *= frequencyX;
            point.y *= frequencyY;
            //Store the passed in Y coordinate.
            int currentCoordinate = Mathf.FloorToInt(point.y);
            //t for smoothing.
            float t = point.y - currentCoordinate;
            //Make sure the value falls within the length of the hashMask.
            currentCoordinate &= hashMask;

            //Get the hash representing this value.
            int currentHash = hash[currentCoordinate];

            //Should the result be smoothed
            if (smooth)
            {
                //Get the next hash value, and interpolate between the current and the next value.
                int nextCoordinate = currentCoordinate + 1;
                int nextHash = hash[nextCoordinate];
                t = Smooth(t);
                return Mathf.Lerp(currentHash, nextHash, t) * (1f / hashMask);
            }
            else
            {
                return currentHash * (1f / hashMask);
            }
        }

        public static float Value2D(Vector2 point, float frequencyX, float frequencyY, bool smooth = true)
        {
            //Apply the frequency.
            point.x *= frequencyX;
            point.y *= frequencyY;
            //Store the passed in X and Y coordinates.
            int currentCoordinateX = Mathf.FloorToInt(point.x);
            int currentCoordinateY = Mathf.FloorToInt(point.y);
            //t for smoothing both X and Y.
            float tx = point.x - currentCoordinateX;
            float ty = point.y - currentCoordinateY;
            //Make sure the values fall within the length of the hashMask.
            currentCoordinateX &= hashMask;
            currentCoordinateY &= hashMask;

            //Get the hashes representing this value along X and Y.
            int currentHashX = hash[currentCoordinateX];
            int currentHashXY = hash[currentHashX + currentCoordinateY];

            //Should the result be smoothed
            if (smooth)
            {
                //Get the next hash value along X and Y, and interpolate between the current and the next value in all ways possible.
                int nextCoordinateX = currentCoordinateX + 1;
                int nextCoordinateY = currentCoordinateY + 1;
                int nextHashX = hash[nextCoordinateX];
                int nextHashXY = hash[nextHashX + nextCoordinateY];
                int currentHashXNextHashY = hash[currentHashX + nextCoordinateY];
                int nextHashXCurrentHashY = hash[nextHashX + currentCoordinateY];

                tx = Smooth(tx);
                ty = Smooth(ty);

                return Mathf.Lerp(
                    Mathf.Lerp(currentHashXY, nextHashXCurrentHashY, tx),
                    Mathf.Lerp(currentHashXNextHashY, nextHashXY, tx),
                    ty) * (1f / hashMask);
            }
            else
            {
                return currentHashXY * (1f / hashMask);
            }
        }

        public static float Perlin1D_X(Vector2 point, float frequencyX, float frequencyY, bool smooth = true)
        {
            //Apply the frequency.
            point.x *= frequencyX;
            point.y *= frequencyY;

            //Store the passed in X coordinate.
            int currentCoordinate = Mathf.FloorToInt(point.x);
            //t for smoothing the current and the next coordinate.
            float t1 = point.x - currentCoordinate;
            float t2 = t1 - 1f;
            //Make sure the value falls within the length of the hashMask.
            currentCoordinate &= hashMask;
            //Get the next coordinate after checking the length. (Gives errors otherwise)
            int nextCoordinate = currentCoordinate + 1;

            //Get the hash representing this value and the next.
            int currentHash = hash[currentCoordinate];
            int nextHash = hash[nextCoordinate];

            //Get the gradient corresponding to the hash value.
            float currentHashGradient = gradients1D[currentHash & gradientsMask1D];
            float nextHashGradient = gradients1D[nextHash & gradientsMask1D];

            //Get the value from the gradient corresponding to this coordinate and the next.
            float currentGradientValue = currentHashGradient * t1;
            float nextGradientValue = nextHashGradient * t2;

            float t = Smooth(t1);
            //Create a gradient between the current gradient and the next.
            return Mathf.Lerp(currentGradientValue, nextGradientValue, t) * 2f;
        }

        public static float Perlin1D_Y(Vector2 point, float frequencyX, float frequencyY, bool smooth = true)
        {
            //Apply the frequency.
            point.x *= frequencyX;
            point.y *= frequencyY;

            //Store the passed in Y coordinate.
            int currentCoordinate = Mathf.FloorToInt(point.y);
            //t for smoothing the current and the next coordinate.
            float t1 = point.y - currentCoordinate;
            float t2 = t1 - 1f;
            //Make sure the value falls within the length of the hashMask.
            currentCoordinate &= hashMask;
            //Get the next coordinate after checking the length. (Gives errors otherwise)
            int nextCoordinate = currentCoordinate + 1;

            //Get the hash representing this value and the next.
            int currentHash = hash[currentCoordinate];
            int nextHash = hash[nextCoordinate];

            //Get the gradient corresponding to the hash value.
            float currentHashGradient = gradients1D[currentHash & gradientsMask1D];
            float nextHashGradient = gradients1D[nextHash & gradientsMask1D];

            //Get the value from the gradient corresponding to this coordinate and the next.
            float currentGradientValue = currentHashGradient * t1;
            float nextGradientValue = nextHashGradient * t2;

            float t = Smooth(t1);
            //Create a gradient between the current gradient and the next.
            return Mathf.Lerp(currentGradientValue, nextGradientValue, t) * 2f;
        }

        public static float Perlin2D(Vector2 point, float frequencyX, float frequencyY, bool smooth = true)
        {
            //Apply the frequency.
            point.x *= frequencyX;
            point.y *= frequencyY;

            //Store the passed in Y coordinate.
            int currentCoordinateX = Mathf.FloorToInt(point.x);
            int currentCoordinateY = Mathf.FloorToInt(point.y);
            //t for smoothing the current and the next coordinate.
            float t1X = point.x - currentCoordinateX;
            float t1Y = point.y - currentCoordinateY;
            float t2X = t1X - 1f;
            float t2Y = t1Y - 1f;
            //Make sure the value falls within the length of the hashMask.
            currentCoordinateX &= hashMask;
            currentCoordinateY &= hashMask;
            //Get the next coordinate after checking the length. (Gives errors otherwise)
            int nextCoordinateX = currentCoordinateX + 1;
            int nextCoordinateY = currentCoordinateY + 1;

            //Get the hash representing this value and the next.
            int currentHashX = hash[currentCoordinateX];
            int nextHashX = hash[nextCoordinateX];

            //Get the gradient corresponding to the hash value.
            Vector2 hashGradientCurrentXCurrentY = gradients2D[hash[currentHashX + currentCoordinateY] & gradientsMask2D];
            Vector2 hashGradientNextXCurrentY = gradients2D[hash[nextHashX + currentCoordinateY] & gradientsMask2D];
            Vector2 hashGradientCurrentXNextY = gradients2D[hash[currentHashX + nextCoordinateY] & gradientsMask2D];
            Vector2 hashGradientNextXNextY = gradients2D[hash[nextHashX + nextCoordinateY] & gradientsMask2D];

            //Get the value from the gradient corresponding to this coordinate and the next.
            float gradientValueCurrentXCurrentY = Dot(hashGradientCurrentXCurrentY, t1X, t1Y);
            float gradientValueNextXCurrentY = Dot(hashGradientNextXCurrentY, t2X, t1Y);
            float gradientValueCurrentXNextY = Dot(hashGradientCurrentXNextY, t1X, t2Y);
            float gradientValueNextXNextY = Dot(hashGradientNextXNextY, t2X, t2Y);

            float tX = Smooth(t1X);
            float tY = Smooth(t1Y);
            //Create a gradient between the current gradient and the next.
            return Mathf.Lerp(
                Mathf.Lerp(gradientValueCurrentXCurrentY, gradientValueNextXCurrentY, tX),
                Mathf.Lerp(gradientValueCurrentXNextY, gradientValueNextXNextY, tX),
                tY) * sqr2;
        }
    }
}