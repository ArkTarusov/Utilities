using UnityEngine;

namespace AiryCat.UtilitiesForUnity.InputHelper
{
    #region Example

/*
    Vector3 ac = _ac.Update();
    if (SupportGyro && !_gyro.enabled)_gyro.enabled = true;
    else
    {
        if (ac.magnitude < _maxAccel)
        {
            if (_counter > 0)
            {
                --_counter;
                return;
            }

            _madgwickAhrs.Quaternionz[0] = _gyro.attitude.w;
            _madgwickAhrs.Quaternionz[1] = _gyro.attitude.x;
            _madgwickAhrs.Quaternionz[2] = _gyro.attitude.y;
            _madgwickAhrs.Quaternionz[3] = _gyro.attitude.z;

            Quaternion oi = _madgwickAhrs.Update(_gyro.gravity.x, _gyro.gravity.y, _gyro.gravity.z,
                ac.x, ac.y, ac.z//,
                //_cmp.rawVector.x, _cmp.rawVector.y, _cmp.rawVector.z
            );

            Rotation = Quaternion.Euler(90, 0, 0) * oi;
        }
        else
            _counter = MaxCounter;
    }
    */

    #endregion

    public class MadgwickAHRS
    {
        private float SamplePeriod { get; set; }
        private float Beta { get; set; }
        private float[] Quaternionz { get; set; }

        // Init constructors
        public MadgwickAHRS(float samplePeriod, float beta = 1f)
        {
            SamplePeriod = samplePeriod;
            Beta = beta;
            Quaternionz = new[] {1f, 0f, 0f, 0f};
        }

        // Algorithm AHRS update method. Requires accelerometer, gyroscope and magnetometer.
        // Gyroscope axis measurement in radians/s: gx, gy, gz
        // Accelerometer axis measurement in any calibrated units: ax, ay, az
        // Magnetometer xis measurement in any calibrated units: mx, my, mz
        public Quaternion
            Update(float gx, float gy, float gz,
                float ax, float ay, float az,
                float mx, float my, float mz
            )
        {
            float q1 = Quaternionz[0], q2 = Quaternionz[1], q3 = Quaternionz[2], q4 = Quaternionz[3];

            // Auxiliary variables to avoid repeated arithmetic
            var _2Q1 = 2f * q1;
            var _2Q2 = 2f * q2;
            var _2Q3 = 2f * q3;
            var _2Q4 = 2f * q4;
            var _2Q1Q3 = 2f * q1 * q3;
            var _2Q3Q4 = 2f * q3 * q4;
            var q1Q1 = q1 * q1;
            var q1Q2 = q1 * q2;
            var q1Q3 = q1 * q3;
            var q1Q4 = q1 * q4;
            var q2Q2 = q2 * q2;
            var q2Q3 = q2 * q3;
            var q2Q4 = q2 * q4;
            var q3Q3 = q3 * q3;
            var q3Q4 = q3 * q4;
            var q4Q4 = q4 * q4;

            // Normalise accelerometer measurement
            var norm = Mathf.Sqrt(ax * ax + ay * ay + az * az);
            norm = 1 / norm; // use reciprocal for division
            ax *= norm;
            ay *= norm;
            az *= norm;

            // Normalise magnetometer measurement
            norm = Mathf.Sqrt(mx * mx + my * my + mz * mz);
            norm = 1 / norm; // use reciprocal for division
            mx *= norm;
            my *= norm;
            mz *= norm;

            // Reference direction of Earth's magnetic field
            var _2Q1Mx = 2f * q1 * mx;
            var _2Q1My = 2f * q1 * my;
            var _2Q1Mz = 2f * q1 * mz;
            var _2Q2Mx = 2f * q2 * mx;
            var hx = mx * q1Q1 - _2Q1My * q4 + _2Q1Mz * q3 + mx * q2Q2 + _2Q2 * my * q3 + _2Q2 * mz * q4 - mx * q3Q3 -
                     mx * q4Q4;
            var hy = _2Q1Mx * q4 + my * q1Q1 - _2Q1Mz * q2 + _2Q2Mx * q3 - my * q2Q2 + my * q3Q3 + _2Q3 * mz * q4 -
                     my * q4Q4;

            // Gradient decent algorithm corrective step
            var s1 = -_2Q3 * (2f * q2Q4 - _2Q1Q3 - ax) + _2Q2 * (2f * q1Q2 + _2Q3Q4 - ay) -
                     (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                      mz * q4Q4) * q3 *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (0.5f - q3Q3 - q4Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (q2Q4 - q1Q3) - mx) +
                     (-Mathf.Sqrt(hx * hx + hy * hy) * q4 +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * q2) *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (q2Q3 - q1Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (q1Q2 + q3Q4) - my) + Mathf.Sqrt(hx * hx + hy * hy) * q3 *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (q1Q3 + q2Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (0.5f - q2Q2 - q3Q3) - mz);
            var s2 = _2Q4 * (2f * q2Q4 - _2Q1Q3 - ax) + _2Q1 * (2f * q1Q2 + _2Q3Q4 - ay) -
                     4f * q2 * (1 - 2f * q2Q2 - 2f * q3Q3 - az) +
                     (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                      mz * q4Q4) * q4 *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (0.5f - q3Q3 - q4Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (q2Q4 - q1Q3) - mx) +
                     (Mathf.Sqrt(hx * hx + hy * hy) * q3 +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * q1) *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (q2Q3 - q1Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (q1Q2 + q3Q4) - my) +
                     (Mathf.Sqrt(hx * hx + hy * hy) * q4 - 2f *
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * q2) * (Mathf.Sqrt(hx * hx + hy * hy) * (q1Q3 + q2Q4) +
                                           (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 +
                                            _2Q3 * my * q4 - mz * q3Q3 + mz * q4Q4) * (0.5f - q2Q2 - q3Q3) - mz);
            var s3 = -_2Q1 * (2f * q2Q4 - _2Q1Q3 - ax) + _2Q4 * (2f * q1Q2 + _2Q3Q4 - ay) -
                     4f * q3 * (1 - 2f * q2Q2 - 2f * q3Q3 - az) +
                     (-(2f * Mathf.Sqrt(hx * hx + hy * hy)) * q3 -
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * q1) *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (0.5f - q3Q3 - q4Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (q2Q4 - q1Q3) - mx) +
                     (Mathf.Sqrt(hx * hx + hy * hy) * q2 +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * q4) *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (q2Q3 - q1Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (q1Q2 + q3Q4) - my) +
                     (Mathf.Sqrt(hx * hx + hy * hy) * q1 - 2f *
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * q3) * (Mathf.Sqrt(hx * hx + hy * hy) * (q1Q3 + q2Q4) +
                                           (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 +
                                            _2Q3 * my * q4 - mz * q3Q3 + mz * q4Q4) * (0.5f - q2Q2 - q3Q3) - mz);
            var s4 = _2Q2 * (2f * q2Q4 - _2Q1Q3 - ax) + _2Q3 * (2f * q1Q2 + _2Q3Q4 - ay) +
                     (-(2f * Mathf.Sqrt(hx * hx + hy * hy)) * q4 +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * q2) *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (0.5f - q3Q3 - q4Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (q2Q4 - q1Q3) - mx) +
                     (-Mathf.Sqrt(hx * hx + hy * hy) * q1 +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * q3) *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (q2Q3 - q1Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (q1Q2 + q3Q4) - my) + Mathf.Sqrt(hx * hx + hy * hy) * q2 *
                     (Mathf.Sqrt(hx * hx + hy * hy) * (q1Q3 + q2Q4) +
                      (-_2Q1Mx * q3 + _2Q1My * q2 + mz * q1Q1 + _2Q2Mx * q4 - mz * q2Q2 + _2Q3 * my * q4 - mz * q3Q3 +
                       mz * q4Q4) * (0.5f - q2Q2 - q3Q3) - mz);
            norm = 1f / Mathf.Sqrt(s1 * s1 + s2 * s2 + s3 * s3 + s4 * s4); // normalise step magnitude
            s1 *= norm;
            s2 *= norm;
            s3 *= norm;
            s4 *= norm;

            // Compute rate of change of quaternion
            var qDot1 = 0.5f * (-q2 * gx - q3 * gy - q4 * gz) - Beta * s1;
            var qDot2 = 0.5f * (q1 * gx + q3 * gz - q4 * gy) - Beta * s2;
            var qDot3 = 0.5f * (q1 * gy - q2 * gz + q4 * gx) - Beta * s3;
            var qDot4 = 0.5f * (q1 * gz + q2 * gy - q3 * gx) - Beta * s4;

            // Integrate to yield quaternion
            q1 += qDot1 * SamplePeriod;
            q2 += qDot2 * SamplePeriod;
            q3 += qDot3 * SamplePeriod;
            q4 += qDot4 * SamplePeriod;
            norm = 1f / Mathf.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4); // normalise quaternion
            Quaternionz[0] = q1 * norm;
            Quaternionz[1] = q2 * norm;
            Quaternionz[2] = q3 * norm;
            Quaternionz[3] = q4 * norm;

            return new Quaternion(Quaternionz[1], Quaternionz[2], Quaternionz[3], Quaternionz[0]);
        }

        // Algorithm AHRS update method. Requires accelerometer and gyroscope.
        // Gyroscope axis measurement in radians/s: gx, gy, gz
        // Accelerometer axis measurement in any calibrated units: ax, ay, az
        public Quaternion
            Update(float gx, float gy, float gz, float ax, float ay, float az)
        {
            float q1 = Quaternionz[0],
                q2 = Quaternionz[1],
                q3 = Quaternionz[2],
                q4 = Quaternionz[3]; // short name local variable for readability
            float norm;
            float s1, s2, s3, s4;
            float qDot1, qDot2, qDot3, qDot4;

            // Auxiliary variables to avoid repeated arithmetic
            var _2Q1 = 2f * q1;
            var _2Q2 = 2f * q2;
            var _2Q3 = 2f * q3;
            var _2Q4 = 2f * q4;
            var _4Q1 = 4f * q1;
            var _4Q2 = 4f * q2;
            var _4Q3 = 4f * q3;
            var _8Q2 = 8f * q2;
            var _8Q3 = 8f * q3;
            var q1Q1 = q1 * q1;
            var q2Q2 = q2 * q2;
            var q3Q3 = q3 * q3;
            var q4Q4 = q4 * q4;

            // Normalise accelerometer measurement
            norm = Mathf.Sqrt(ax * ax + ay * ay + az * az);
            norm = 1 / norm; // use reciprocal for division
            ax *= norm;
            ay *= norm;
            az *= norm;

            // Gradient decent algorithm corrective step
            s1 = _4Q1 * q3Q3 + _2Q3 * ax + _4Q1 * q2Q2 - _2Q2 * ay;
            s2 = _4Q2 * q4Q4 - _2Q4 * ax + 4f * q1Q1 * q2 - _2Q1 * ay - _4Q2 + _8Q2 * q2Q2 + _8Q2 * q3Q3 + _4Q2 * az;
            s3 = 4f * q1Q1 * q3 + _2Q1 * ax + _4Q3 * q4Q4 - _2Q4 * ay - _4Q3 + _8Q3 * q2Q2 + _8Q3 * q3Q3 + _4Q3 * az;
            s4 = 4f * q2Q2 * q4 - _2Q2 * ax + 4f * q3Q3 * q4 - _2Q3 * ay;
            norm = 1f / Mathf.Sqrt(s1 * s1 + s2 * s2 + s3 * s3 + s4 * s4); // normalise step magnitude
            s1 *= norm;
            s2 *= norm;
            s3 *= norm;
            s4 *= norm;

            // Compute rate of change of quaternion
            qDot1 = 0.5f * (-q2 * gx - q3 * gy - q4 * gz) - Beta * s1;
            qDot2 = 0.5f * (q1 * gx + q3 * gz - q4 * gy) - Beta * s2;
            qDot3 = 0.5f * (q1 * gy - q2 * gz + q4 * gx) - Beta * s3;
            qDot4 = 0.5f * (q1 * gz + q2 * gy - q3 * gx) - Beta * s4;

            // Integrate to yield quaternion
            q1 += qDot1 * SamplePeriod;
            q2 += qDot2 * SamplePeriod;
            q3 += qDot3 * SamplePeriod;
            q4 += qDot4 * SamplePeriod;
            norm = 1f / Mathf.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4); // normalise quaternion
            Quaternionz[0] = q1 * norm;
            Quaternionz[1] = q2 * norm;
            Quaternionz[2] = q3 * norm;
            Quaternionz[3] = q4 * norm;

            return new Quaternion(Quaternionz[1], Quaternionz[2], Quaternionz[3], Quaternionz[0]);
        }
    }
}
