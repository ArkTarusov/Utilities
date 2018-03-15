using UnityEngine;

namespace AiryCat.Utilities.InputHelper
{
    public class CorrectGPS
    {
        private const float MIN_ACCURACY = 1;
        private readonly float _qMetresPerSecond;
        private double _lat;
        private double _lng;
        private float _variance;
        private float _k;
        public CorrectGPS(float qMetresPerSecond, double startLat, double startLng)
        {
            _qMetresPerSecond = qMetresPerSecond;
            _variance = -1;
            _variance = Input.location.lastData.horizontalAccuracy * 2;
            Input.location.Start(0.1f, 0.1f);
            _lat = startLat;
            _lng = startLng;

        }
        public float get_K() { return _k; }

        public double[] Update()
        {
            double latMeasurement = Input.location.lastData.latitude;
            double lngMeasurement = Input.location.lastData.longitude;
            float accuracy = Input.location.lastData.horizontalAccuracy;

            if (accuracy < MIN_ACCURACY) accuracy = MIN_ACCURACY;
            if (_variance < 0)
            {

                _lat = latMeasurement; _lng = lngMeasurement; _variance = accuracy * accuracy;
            }
            else
            {
                _variance += _qMetresPerSecond * _qMetresPerSecond / 1000;
                // TODO: USE VELOCITY INFORMATION HERE TO GET A BETTER ESTIMATE OF CURRENT POSITION

                _k = _variance / (_variance + accuracy * accuracy);
                _lat += _k * (latMeasurement - _lat);
                _lng += _k * (lngMeasurement - _lng);
                _variance = (1 - _k) * _variance;
            }
            double[] result = { _lat, _lng };
            return result;
        }
    }
}
