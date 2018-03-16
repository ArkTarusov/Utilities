using UnityEngine;

namespace AiryCat.UtilitiesForUnity.InputHelper
{
    public static class ConverCoordinate
    {
        //SOURCE: http://stackoverflow.com/questions/4953150/convert-lat-longs-to-x-y-co-ordinates

        private const float GdSemiMajorAxis = 6378137.000000f;
        private const float GdTranMercB = 6356752.314245f;
        private const float GdGeocentF = 0.003352810664f;

        public static void GeodeticOffsetInv(float refLat, float refLon,
            float lat, float lon,
            out float xOffset, out float yOffset)
        {
            const float a = GdSemiMajorAxis;
            const float b = GdTranMercB;
            const float f = GdGeocentF;

            var l = lon - refLon;
            var u1 = Mathf.Atan((1 - f) * Mathf.Tan(refLat));
            var u2 = Mathf.Atan((1 - f) * Mathf.Tan(lat));
            var sinU1 = Mathf.Sin(u1);
            var cosU1 = Mathf.Cos(u1);
            var sinU2 = Mathf.Sin(u2);
            var cosU2 = Mathf.Cos(u2);

            var lambda = l;
            float lambdaP;
            float sinSigma;
            float sigma;
            float cosSigma;
            float cosSqAlpha;
            float cos2SigmaM;
            float sinLambda;
            float cosLambda;
            var iterLimit = 100;
            do
            {
                sinLambda = Mathf.Sin(lambda);
                cosLambda = Mathf.Cos(lambda);
                sinSigma = Mathf.Sqrt((cosU2 * sinLambda) * (cosU2 * sinLambda) +
                                      (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) *
                                      (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
                if (sinSigma == 0)
                {
                    xOffset = 0.0f;
                    yOffset = 0.0f;
                    return;  // co-incident points
                }
                cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
                sigma = Mathf.Atan2(sinSigma, cosSigma);
                var sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
                cosSqAlpha = 1 - sinAlpha * sinAlpha;
                cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / cosSqAlpha;
                
                var C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
                lambdaP = lambda;
                lambda = l + (1 - C) * f * sinAlpha *
                         (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
            } while (Mathf.Abs(lambda - lambdaP) > 1e-12 && --iterLimit > 0);

            if (iterLimit == 0)
            {
                xOffset = 0.0f;
                yOffset = 0.0f;
                return;  // formula failed to converge
            }

            var uSq = cosSqAlpha * (a * a - b * b) / (b * b);
            var A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            var B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
            var deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) -
                                                                   B / 6 * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));
            var s = b * A * (sigma - deltaSigma);

            var bearing = Mathf.Atan2(cosU2 * sinLambda, cosU1 * sinU2 - sinU1 * cosU2 * cosLambda);
            xOffset = Mathf.Sin(bearing) * s;
            yOffset = Mathf.Cos(bearing) * s;
        }
    }
}
