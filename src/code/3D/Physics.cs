using Astral_simulation;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Astral_Simulation
{
    public static class Physics
    {
        private static Vector3 _sunPosition = Vector3.Zero;
        private static float _initialAngle = 0.0f;
        private static float _timeScale = 0.1f;

        // Astral Object's properties
        private static float _a; // semi-major axis (big diameter of ellipse)
        private static float _e; // excentricity (orbit flattening)
        private static float _i; // tilt (orbital plane angle)
        private static float _omega; // perihelion argument - rotation in the orbital plane
        private static float _Omega; // longitude of ascending node - rotation of orbital plane in space
        private static float _M0; // mean anomalia
        private static float _period; // orbital period 
        private static float _n; // angular velocity

        ///// <summary>
        ///// Compute distance between Sun and object
        ///// </summary>
        ///// <param name="position">Object's position</param>
        ///// <returns><see langword="float"/>: Radial distance</returns>
        //public static float ComputeRadialDistance(AstralObject obj)
        //{
        //    return Vector3.Distance(_sunPosition, obj.Position);
        //}
        
        ///// <summary>
        ///// Compute angular velocity of object
        ///// </summary>
        ///// <param name="period">Object's revolution period</param>
        ///// <returns><see langword="float"/>: Angular velocity of object</returns>
        //public static float ComputeAngularVelocity(AstralObject obj)
        //{
        //    return 2 * MathF.PI / obj.Revolution;
        //}

        ///// <summary>
        ///// Compute and update position of object at every 0.1f time
        ///// </summary>
        ///// <param name="period">Object's period</param>
        ///// <param name="position">Object's position</param>
        ///// <returns><see langword="Vector3"/>: The updated position of object</returns>
        //public static Vector3 ComputePositionAtTime(AstralObject obj)
        //{
        //    float r = ComputeRadialDistance(obj);

        //    float angularPos = _initialAngle + ComputeAngularVelocity(obj) * _timeScale;

        //    float x = r * MathF.Cos(angularPos);
        //    float z = r * MathF.Sin(angularPos);

        //    _timeScale -= 0.01f;

        //    return new Vector3(x, 0, z);
        //}

        //public static float ComputeRotationPeriod(AstralObject obj)
        //{
        //    return (2 * MathF.PI * obj.Radius) / obj.RotationSpeed;
        //}

        //public static void ComputeRotation(AstralObject obj)
        //{
        //    obj.Roll += obj.RotationSpeed * (float)0.00001;
        //}

        /*
         * ===============================================
         * ============ASTRAL OBJECTS MOVEMENT============
         * ===============================================
         */

        /// <summary>
        /// Update properties of the object.
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void UpdateProperties(AstralObject obj)
        {
            _a = obj.SemiMajorAxis;
            _e = obj.OrbitalEccentricity;
            _i = MathF.PI / 180 * obj.OrbitalInclination; //Convert deg into rad
            _omega = MathF.PI / 180 * obj.PerihelionLongitude;
            _Omega = MathF.PI / 180 * obj.AscendingNodeLongitude;
            _M0 = MathF.PI / 180 * obj.MeanAnomaly;
            _period = obj.Revolution;
            _n = 2 * MathF.PI / _period; 
        }

        /// <summary>
        /// Update.
        /// </summary>
        public static void Update(AstralObject obj)
        {
            UpdateProperties(obj);
            // Mean anomaly at frame
            float M = _M0 + _n * GetFrameTime();
            M %= 2 * MathF.PI;

            // Excentric anomaly computed with Kepler equation
            float E = SolveKepler(M, _e);

            // Real anomaly, real angle between the object and the sun
            float v = 2 * MathF.Atan2(MathF.Sqrt(1 + _e) * MathF.Sin(E / 2), MathF.Sqrt(1 - _e) * MathF.Cos(E / 2));

            // Distance object-Sun
            float r = _a * (1 - _e * MathF.Cos(E));

            // Position in orbital plane
            float x_orb = r * MathF.Cos(v);
            float y_orb = r * MathF.Sin(v);
            float z_orb = 0;

            obj.Position = OrbitalTo3D(new Vector3(x_orb, y_orb, z_orb));
        }

        /// <summary>
        /// Convert orbital plane coordinates to real 3D position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector3 OrbitalTo3D(Vector3 pos)
        {
            pos = RotateZ(pos, _omega);
            pos = RotateX(pos, _i);
            pos = RotateZ(pos, _Omega);

            return pos;
        }

        /// <summary>
        /// Rotate object on X axis using rotation matrix
        /// </summary>
        /// <param name="v"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 RotateX(Vector3 v, float angle)
        {
            float cosa = MathF.Cos(angle);
            float sina = MathF.Sin(angle);

            return new Vector3(
                v.X,
                v.Y * cosa - v.Z * sina,
                v.Y * sina + v.Z + cosa
            );
        }

        /// <summary>
        /// Rotate object on Z axis using rotation matrix
        /// </summary>
        /// <param name="v"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 RotateZ(Vector3 v, float angle)
        {
            float cosa = MathF.Cos(angle);
            float sina = MathF.Sin(angle);

            return new Vector3(
                v.X * cosa - v.Y * sina,
                v.X * sina + v.Y * cosa,
                v.Z
            );
        }        

        /// <summary>
        /// Solve Kepler equation with Newton-Raphson method
        /// </summary>
        /// <param name="M">Mean anomaly of the object.</param>
        /// <param name="e">Excentricity of the object.</param>
        /// <returns>Excentric anomaly of the object.</returns>
        public static float SolveKepler(float M, float e)
        {
            float E = M;
            
            for (int i = 0; i < 10; i++)
            {
                float f = E - e * MathF.Sin(E) - M;
                float f_prime = 1 - e * MathF.Cos(E);
                E -= f / f_prime;
            }

            return E;
        }

        public static void DrawOrbitPath(AstralObject obj, int segments = 360)
        {
            Vector3[] points = new Vector3[segments];

            for (int i = 0; i < segments; i++)
            {
                float M = i / (float)segments * 2 * MathF.PI;
                float E = SolveKepler(M, _e);
                float v = 2 * MathF.Atan2(MathF.Sqrt(1 + _e) * MathF.Sin(E / 2), MathF.Sqrt(1 - _e) * MathF.Cos(E / 2));
                float r = _a * (1 - _e * MathF.Cos(E));
                Vector3 pos = new Vector3(r * MathF.Cos(v), r * MathF.Sin(v), 0);
                points[i] = OrbitalTo3D(pos);
            }

            for (int i = 0; i < segments - 1; i++)
                DrawLine3D(points[i], points[i + 1], Raylib_cs.Color.RayWhite);
        }
    }
}
