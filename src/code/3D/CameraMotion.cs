using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="CameraMotion"/>.</summary>
    public class CameraMotion
    {
        public const float SPEED = 0.000005f;

        /// <summary>Velocity of the camera.</summary>
        public Vector3 Velocity;

        /// <summary>Defines if camera moving</summary>
        public bool Moving;

        public float Yaw;
        public float Pitch;

        /// <summary>Creates an instance of <see cref="CameraMotion"/>.</summary>
        /// <param name="distance">Distance of the camera to its target</param>
        /// <param name="width">Screen width</param>
        /// <param name="height">Screen height</param>
        public CameraMotion(float distance, short width, short height)
        {
            Velocity = Vector3.Zero;
        }

        public CameraMotion()
        {
            Velocity = Vector3.Zero;
        }
    }
}