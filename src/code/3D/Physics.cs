using System.Numerics;
using static Raylib_cs.Raylib;

namespace astral_simulation
{
    public static class Physics
    {
        private const float G = 6.67430f * 10e-11f; //Gravitatonal constant (m^3 kg^1 s^-2)
        private const float M = 1988400f; //Sun's mass
        private static Vector3 _sunPosition = Vector3.Zero;

        /// <summary>
        /// Compute average distance between center of the sun and center of a planet
        /// </summary>
        /// <param name="planetPosition">Planet's pos</param>
        /// <returns><see langword="float"/>: Average distance between sun and planet</returns>
        public static float ComputeDistanceBetweenSunAndPlanet(Vector3 planetPosition)
        {
            float distance = Vector3.Distance(planetPosition, _sunPosition);

            return distance;
        }

        /// <summary>
        /// Compute gravitation pull
        /// </summary>
        /// <param name="planetMass">Planet's mass</param>
        /// <param name="planetPosition">Planet's pos</param>
        /// <returns><see langword="float"/>: Gravitation pull</returns>
        public static float ComputeGravitationPull(float planetMass, Vector3 planetPosition)
        {
            float gPull = G * M * planetMass / (MathF.Pow(ComputeDistanceBetweenSunAndPlanet(planetPosition), 2)); //Formula of law of universal gravitation

            return gPull;
        }

        /// <summary>
        /// Compute orbital velocity
        /// </summary>
        /// <param name="planetSemiMajorAxis">Planet's semi-major axis</param>
        /// <param name="planetPosition">Planet's position</param>
        /// <returns><see langword="float"/>: Orbital velocity</returns>
        public static float ComputeOrbitalVelocity(float planetSemiMajorAxis, Vector3 planetPosition)
        {
            float velocity = MathF.Sqrt(G * M * (2f / MathF.Abs(ComputeDistanceBetweenSunAndPlanet(planetPosition) - 1f / planetSemiMajorAxis))); //Equation of computing of orbital velocity

            return velocity;
        }

        //public static float ComputeTrueAnomalia(float eccentricity, Vector3 planetPosition)
        //{
        //    float r = ComputeDistanceBetweenSunAndPlanet(planetPosition);

            
        //}
    }
}
