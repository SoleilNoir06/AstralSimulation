using System.Numerics;
using Raylib_cs;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="CameraMotion"/> which mainly contains additonal camera parameters.</summary>
    public class CameraMotion
    {
        // Constants
        public const float SENSITIVITY = 0.003f;
        public const float INITIAL_TILT = -50f;

        // Private attributes
        private int _targetId;
        private Vector3 _initialPosition;

        // Public attributes

        /// <summary>Yaw rotation of the camera.</summary>
        public float Yaw;

        /// <summary>Pitch rotation of the camera.</summary>
        public float Pitch;

        /// <summary>Defines if the camera is focused on a single planet.</summary>
        public bool Focus;


        public Vector3 TargetOffset;

        /// <summary>Defines if camera moving</summary>
        public bool Moving;

        /// <summary>Target object.</summary>
        public AstralObject? Target;

        /// <summary>ID of the target object.</summary>
        public int TargetId { get { return _targetId; } 
            set 
            {
                if (value >= Conceptor3D.System.Count) _targetId = 0;
                else if (value < 0) _targetId = Conceptor3D.System.Count - 1;
                else _targetId = value;
            } 
        }

        /// <summary>Initial position of the camera used for interpolations.</summary>
        public Vector3 InitialPosition { get { return _initialPosition; } }

        /// <summary>Creates an empty <see cref="CameraMotion"/> instance.</summary>
        public CameraMotion()
        {
            Focus = false;
            _targetId = -1;
        }

        /// <summary>Rotates a camera around its up vector. Yaw is looking "left" and "right". NOTE : angle must be in radians.</summary>
        /// <param name="camera">The camera to update.</param>
        /// <param name="angle">The angle to set (in radians).</param>
        public void UpdateYaw(ref Camera3D camera, float angle)
        {
            // View vector
            Vector3 view = camera.Target - camera.Position;

            // Rotate view vector around rotation axis
            view = Raymath.Vector3RotateByAxisAngle(view, camera.Up, angle);

            // Set updated position
            camera.Position = camera.Target - view;
        }

        /// <summary>Rotates a camera around its right vector. Pitch is looking "up" and "down". NOTE : angle must be in radians.</summary>
        /// <param name="camera">The camera to update.</param>
        /// <param name="angle">The angle to set (in radians).</param>
        public void UpdatePitch(ref Camera3D camera, float angle)
        {   
            // View vector 
            Vector3 view = camera.Target - camera.Position;

            // Rotation axis
            Vector3 right = Raylib.GetCameraRight(ref camera);

            // Rotate view vector around right axis (with clamped angle)
            // Update current pitch
            float prevAngle = Pitch;
            Pitch += angle*Raylib.RAD2DEG;
            if (angle > 0 && Pitch < 89 || angle < 0 && Pitch > -89)
            {
                view = Raymath.Vector3RotateByAxisAngle(view, right, angle);
            }
            else
            {
                Pitch = prevAngle;
            }

            // Set updated position
            camera.Position = camera.Target - view;
        }

        /// <summary> Registers a position as the initial camera position.</summary>
        /// <param name="position">Position to register as the initial.</param>
        public void RegisterInitialPosition(Vector3 position)
        {
            _initialPosition = position;
        }

        /// <summary>Defines the target for the probe.</summary>
        public void DefineTarget()
        {
            Focus = true;
            Target = Conceptor3D.System.GetObject(TargetId); // Get next target
            Conceptor2D.DisplayObject(Target);
        }
    }
}