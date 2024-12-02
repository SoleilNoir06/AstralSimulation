using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace astral_simulation
{
    public static class Physics
    {
        private const float G = 6.67430f * 10e-11f; //Gravitatonal constant (m^3 kg^1 s^-2)
        private const float MU = 39.48f;
        private const float TOLERANCE = 1e-6f;
        private const float TIME_SCALE = 100.0f;
        private static Vector3 _sunPosition = Vector3.Zero; 

        public static float ComputeRadialDistance(Vector3 planetPosition)
        {
            return Vector3.Distance(_sunPosition, planetPosition);
        }

        

/*
        /// <summary>
        /// Compute average anomaly
        /// </summary>
        /// <param name="semiMajorAxis">Planet's semi-major axis</param>
        /// <param name="currentTime">Current time</param>
        /// <param name="refTime">Reference time</param>
        /// <returns><see langword="float"/>: Average anomaly</returns>
        public static float ComputeAverageAnomaly(float semiMajorAxis, float currentTime, float refTime)
        {
            float scaledTime = currentTime * TIME_SCALE;
            float n = MathF.Sqrt(MU / MathF.Pow(semiMajorAxis, 3)); //Average movement
            float M = n * (scaledTime - refTime); //Average anomaly

            return M;
        }

        /// <summary>
        /// Resolve Kepler's equation
        /// </summary>
        /// <param name="eccentricity">Planet's eccentricity</param>
        /// <param name="semiMajorAxis">Planet's semi-major axis</param>
        /// <param name="currentTime">Current time</param>
        /// <param name="refTime">Reference time</param>
        /// <returns><see langword="float"/>: Resolves Kepler's equation and returns eccentric anomaly</returns>
        public static float SolveKeplerEquation(float eccentricity, float semiMajorAxis, float currentTime, float refTime)
        {
            float E = ComputeAverageAnomaly(semiMajorAxis, currentTime, refTime); //Init with M as first estimation
            float delta = 1.0f;

            while (delta > TOLERANCE)
            {
                float newE = E - (E - eccentricity * MathF.Sin(E) - ComputeAverageAnomaly(semiMajorAxis, currentTime, refTime)) / (1 - eccentricity * MathF.Cos(E)); //Compute new E
                delta = MathF.Abs(newE - E);
                E = newE; //Update E
            }

            return E;
        }

        /// <summary>
        /// Compute true anomaly v
        /// </summary>
        /// <param name="eccentricity">Planet's eccentricity</param>
        /// <param name="semiMajorAxis">Planet's semi-major axis</param>
        /// <param name="currentTime">Current time</param>
        /// <param name="refTime">Reference time</param>
        /// <returns><see langword="float"/>: Returns true anomaly</returns>
        public static float ComputeTrueAnomalyV(float eccentricity, float semiMajorAxis, float currentTime, float refTime)
        {
            float E = SolveKeplerEquation(eccentricity, semiMajorAxis, currentTime, refTime); //Solve Kepler's equation
            float nu = 2.0f * MathF.Atan2(MathF.Sqrt(1 + eccentricity) * MathF.Sin(E / 2.0f), MathF.Sqrt(1 - eccentricity) * MathF.Cos(E / 2.0f)); //Compute true anomaly

            return nu;
        }

        /// <summary>
        /// Compute distance between Sun and object
        /// </summary>
        /// <param name="eccentricity">Planet's eccentricity</param>
        /// <param name="semiMajorAxis">Planet's semi-major axis</param>
        /// <param name="currentTime">Current time</param>
        /// <param name="refTime">Reference time</param>
        /// <returns><see langword="float"/>: Radial distance (distance between sun and object)</returns>
        public static float ComputeRadialDistance(float eccentricity, float semiMajorAxis, float currentTime, float refTime)
        {
            float E = SolveKeplerEquation(eccentricity, semiMajorAxis, currentTime, refTime);
            float r = semiMajorAxis * (1 - eccentricity * MathF.Cos(E));

            return r;
        }

        /// <summary>
        /// Compute orbital coordinates of object
        /// </summary>
        /// <param name="eccentricity">Planet's eccentricity</param>
        /// <param name="semiMajorAxis">Planet's semi-major axis</param>
        /// <param name="currentTime">Current time</param>
        /// <param name="refTime">Reference time</param>
        /// <returns><see langword="Vector3"/>: Orbital coordinates of object</returns>
        public static Vector3 ComputeOrbitalCoordinates(float eccentricity, float semiMajorAxis, float currentTime, float refTime)
        {
            float r = ComputeRadialDistance(eccentricity, semiMajorAxis, currentTime, refTime);
            float nu = ComputeTrueAnomalyV(eccentricity, semiMajorAxis, currentTime, refTime);
            float xOrbital = r * MathF.Cos(nu);
            float yOrbital = r * MathF.Sin(nu);

            return new Vector3(xOrbital, yOrbital, 0);
        }

        /// <summary>
        /// Convert orbital coordinates to heliocentric coordinates
        /// </summary>
        /// <param name="perihelionLongitude">Planet's perihelion longitude</param>
        /// <param name="tilt">Planet's tilt</param>
        /// <param name="eccentricity">Planet's eccentricity</param>
        /// <param name="semiMajorAxis">Planet's semi-major axis</param>
        /// <param name="currentTime">Current time</param>
        /// <param name="refTime">Reference time</param>
        /// <returns><see langword="Vector3"/>: Heliocentric coordinates</returns>
        public static Vector3 ConvertOrbitalCooToHelioCoo(float perihelionLongitude, float tilt, float eccentricity, float semiMajorAxis, float currentTime, float refTime)
        {
            perihelionLongitude = perihelionLongitude * DEG2RAD; //Convert to radians
            tilt = tilt * DEG2RAD;

            float xOrbital = ComputeOrbitalCoordinates(eccentricity, semiMajorAxis, currentTime, refTime).X; //Compute orbital coordinates
            float yOrbital = ComputeOrbitalCoordinates(eccentricity, semiMajorAxis, currentTime, refTime).Y;

            //Convert to heliocentric coordinates
            float xHelio = xOrbital * (MathF.Cos(perihelionLongitude) * MathF.Cos(perihelionLongitude) - MathF.Sin(perihelionLongitude) * MathF.Sin(perihelionLongitude) * MathF.Cos(tilt))
                        - yOrbital * (MathF.Sin(perihelionLongitude) * MathF.Cos(perihelionLongitude) + MathF.Cos(perihelionLongitude) * MathF.Sin(perihelionLongitude) * MathF.Cos(tilt));
            
            float yHelio = xOrbital * (MathF.Cos(perihelionLongitude) * MathF.Sin(perihelionLongitude) + MathF.Sin(perihelionLongitude) * MathF.Cos(perihelionLongitude) * MathF.Cos(tilt))
                            - yOrbital * (MathF.Sin(perihelionLongitude) * MathF.Sin(perihelionLongitude) - MathF.Cos(perihelionLongitude) * MathF.Cos(perihelionLongitude) * MathF.Cos(tilt));
            
            float zHelio = xOrbital * MathF.Sin(perihelionLongitude) * MathF.Sin(tilt) + yOrbital * MathF.Cos(perihelionLongitude) * MathF.Sin(tilt);

            Vector3 heliocentricCoo = new Vector3(xHelio * 100000, yHelio * 100000, zHelio * 100000);

            return heliocentricCoo;

        }
*/

    }
}
