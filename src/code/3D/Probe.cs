using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="Probe"/>.</summary>
    public class Probe
    {
        public static float IMMERSIVE_SPEED = 0.000005f;
        public static float VOYAGER_SPEED = 0.005f;

        public static float SPEED = IMMERSIVE_SPEED; // Default speed (= ~24 * Light Speed)

        private int _targetId;

        /// <summary>Velocity of the camera.</summary>
        public Vector3 Velocity;

        /// <summary>Defines if camera moving</summary>
        public bool Moving;

        /// <summary>Yaw rotation of the camera.</summary>
        public float Yaw;

        /// <summary>Pitch rotation of the camera.</summary>
        public float Pitch;

        /// <summary>Defines if the camera is in transit.</summary>
        public bool InTransit;

        /// <summary>Target object.</summary>
        public AstralObject? Target;

        /// <summary>ID of the target object.</summary>
        public int TargetId { get { return _targetId; } 
            set 
            {
                if (value >= Conceptor3D.System.Count - 1) _targetId = 0;
                else if (value < 0) _targetId = Conceptor3D.System.Count - 1;
                else _targetId = value;
            } 
        }

        /// <summary>Creates an instance of <see cref="CameraMotion"/>.</summary>
        /// <param name="distance">Distance of the camera to its target</param>
        /// <param name="width">Screen width</param>
        /// <param name="height">Screen height</param>
        public Probe(float distance, short width, short height)
        {
            Velocity = Vector3.Zero;
            InTransit = false;
        }

        /// <summary>Creates an empty <see cref="CameraMotion"/> instance.</summary>
        public Probe()
        {
            Velocity = Vector3.Zero;
            InTransit = false;
        }
    }
}