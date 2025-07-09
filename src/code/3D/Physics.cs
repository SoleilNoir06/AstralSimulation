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
        private static float _periode; // orbital period 
        private static float _n; // angular velocity

        /// <summary>
        /// Compute distance between Sun and object
        /// </summary>
        /// <param name="position">Object's position</param>
        /// <returns><see langword="float"/>: Radial distance</returns>
        public static float ComputeRadialDistance(AstralObject obj)
        {
            return Vector3.Distance(_sunPosition, obj.Position);
        }
        
        /// <summary>
        /// Compute angular velocity of object
        /// </summary>
        /// <param name="period">Object's revolution period</param>
        /// <returns><see langword="float"/>: Angular velocity of object</returns>
        public static float ComputeAngularVelocity(AstralObject obj)
        {
            return 2 * MathF.PI / obj.Revolution;
        }

        /// <summary>
        /// Compute and update position of object at every 0.1f time
        /// </summary>
        /// <param name="period">Object's period</param>
        /// <param name="position">Object's position</param>
        /// <returns><see langword="Vector3"/>: The updated position of object</returns>
        public static Vector3 ComputePositionAtTime(AstralObject obj)
        {
            float r = ComputeRadialDistance(obj);

            float angularPos = _initialAngle + ComputeAngularVelocity(obj) * _timeScale;

            float x = r * MathF.Cos(angularPos);
            float z = r * MathF.Sin(angularPos);

            _timeScale -= 0.01f;

            return new Vector3(x, 0, z);
        }

        public static float ComputeRotationPeriod(AstralObject obj)
        {
            return (2 * MathF.PI * obj.Radius) / obj.RotationSpeed;
        }

        public static void ComputeRotation(AstralObject obj)
        {
            obj.Roll += obj.RotationSpeed * (float)0.00001;
        }

        /*
         * ===============================================
         * ============ASTRAL OBJECTS MOVEMENT============
         * ===============================================
         */

        //public static void UpdateProperties(AstralObject obj)
        //{
        //    _a = obj.SemiMajorAxis;
        //    _e = obj.OrbitalEccentricity;
        //    _i = obj.Tilt
        //}
    }
}
