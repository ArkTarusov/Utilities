using UnityEngine;

namespace AiryCat.UtilitiesForUnity.InputHelper
{
    public class CorrectAcceleration
    {
        private const float ACCELEROMETER_UPDATE_INTERVAL = 1.0f / 60.0f;
        private readonly float _lowPassFilterFactor;
        private Vector3 _lowPassValue;

        public CorrectAcceleration(float lowPassKernelWidthInSeconds = 1.0f)
        {
            _lowPassFilterFactor = ACCELEROMETER_UPDATE_INTERVAL / lowPassKernelWidthInSeconds; // tweakable

            _lowPassValue = Average();
            Debug.Log("[CorrectAcceleration] Create");
        }

        //Calculate the average value of the accelerometer.
        private static Vector3 Average()
        {
            var period = 0f;
            var acc = Vector3.zero;
            foreach (var evnt in Input.accelerationEvents)
            {
                acc += evnt.acceleration * evnt.deltaTime;
                period += evnt.deltaTime;
            }

            if (period > 0)
                acc *= 1.0f / period;
            return acc;
        }

        //Smooth noise
        public Vector3 Update()
        {
            _lowPassValue = Vector3.Lerp(_lowPassValue, Average(), _lowPassFilterFactor);
            return _lowPassValue;
        }
    }
}

