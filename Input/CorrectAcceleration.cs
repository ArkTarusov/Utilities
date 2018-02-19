using UnityEngine;

namespace AiryCat.Utilities.Input
{
    public class CorrectAcceleration
    {
        private const float AccelerometerUpdateInterval = 1.0f / 60.0f;
        private readonly float _lowPassFilterFactor;
        private Vector3 _lowPassValue;

        public CorrectAcceleration(float lowPassKernelWidthInSeconds = 1.0f)
        {
            _lowPassFilterFactor = AccelerometerUpdateInterval / lowPassKernelWidthInSeconds; // tweakable

            _lowPassValue = Average();
            Debug.Log("Correct Acceleration start");
        }

        //Высчитываем среднее значение акселерометра.
        private static Vector3 Average()
        {
            float period = 0f;
            Vector3 acc = Vector3.zero;
            foreach (AccelerationEvent evnt in Input.accelerationEvents)
            {
                acc += evnt.acceleration * evnt.deltaTime;
                period += evnt.deltaTime;
            }

            if (period > 0)
                acc *= 1.0f / period;
            return acc;
        }

        //Фильтруем шумы

        public Vector3 Update()
        {
            _lowPassValue = Vector3.Lerp(_lowPassValue, Average(), _lowPassFilterFactor);
            return _lowPassValue;
        }
    }
}

