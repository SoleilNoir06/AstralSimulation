using Astral_simulation;
using System.Numerics;
using static Raylib_cs.Raylib;
using Raylib_cs;

namespace Astral_Simulation
{
    public static class Physics
    {
        // Astral Object's properties
        private static float _a; // semi-major axis (big diameter of ellipse)
        private static float _e; // excentricity (orbit flattening)
        private static float _i; // tilt (orbital plane angle)
        private static float _omega; // perihelion argument - rotation in the orbital plane
        private static float _Omega; // longitude of ascending node - rotation of orbital plane in space
        private static float _M0; // mean anomalia
        private static float _period; // orbital period 
        private static float _n; // angular velocity

        //Delta time
        private static float _deltaTime = 0;

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

            _deltaTime += GetFrameTime();
        }

        /// <summary>
        /// Update.
        /// </summary>
        public static void Update(AstralObject obj)
        {
            UpdateProperties(obj);

            // Mean anomaly at frame
            float M = _M0 + _n * _deltaTime / 1000;
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
        /// Convert orbital plane coordinates to real 3D position.
        /// </summary>
        /// <param name="pos">Object's position.</param>
        /// <returns></returns>
        public static Vector3 OrbitalTo3D(Vector3 pos)
        {
            pos = RotateZ(pos, _omega);
            pos = RotateX(pos, _i);
            pos = RotateZ(pos, _Omega);

            return pos;
        }

        /// <summary>
        /// Rotate object on X axis using rotation matrix.
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
        /// Solve Kepler equation with Newton-Raphson method.
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

        /// <summary>
        /// Draw object's path.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="segments">Number of segments of ellipse.</param>
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

            // Define orbit color (based on UI activity)
            Color orbitColor = obj.UIActive ? ColorBrightness(obj.AttributeColor, Conceptor2D.COLOR_BRIGTHNESS_OVERLAY) : obj.AttributeColor;
            for (int i = 0; i < segments - 1; i++)
            {
                DrawLine3D(points[i], points[i + 1], orbitColor);
            }
        }
    }
}
