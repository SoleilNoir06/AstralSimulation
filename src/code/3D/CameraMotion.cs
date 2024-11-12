using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="CameraMotion"/>.</summary>
    public class CameraMotion
    {
        /// <summary>Y offset of the motion.</summary>
        public float YOffset;
        /// <summary>Distance of the camera to its target.</summary>
        
        public float Distance;
        /// <summary>Current position of the mouse.</summary>

        public Vector2 Mouse;
        /// <summary>Mouse origin position.</summary>

        public Vector2 MouseOrigin;

        /// <summary>Fake position of the mouse.</summary>
        public Vector2 FakePosition;

        /// <summary>Creates an instance of <see cref="CameraMotion"/>.</summary>
        /// <param name="distance">Distance of the camera to its target</param>
        /// <param name="width">Screen width</param>
        /// <param name="height">Screen height</param>
        public CameraMotion(float distance, short width, short height)
        {
            Distance = distance;
            YOffset = 0;
            // Center the intial position of the mouse to the center of the screen
            Mouse = new Vector2(width / 2, height / 2);
            // Set other vector variables to zero
            MouseOrigin = Vector2.Zero;
            FakePosition = Vector2.Zero;
        }

        public CameraMotion()
        {
            Distance = 0f;
            YOffset = 0f;
            Mouse = Vector2.Zero;
            MouseOrigin = Vector2.Zero;
            FakePosition = Vector2.Zero;
        }
    }
}