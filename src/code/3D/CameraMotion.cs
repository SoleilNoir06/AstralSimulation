using System.Numerics;
using Raylib_cs;

namespace Astral_simulation
{
    /// <summary>Defines the current movement state of the camera.</summary>
    public enum CameraState
    {
        Free,
        Focused,
        Withdrawing
    }

    /// <summary>Represents an instance of <see cref="CameraMotion"/> which mainly contains additonal camera parameters.</summary>
    public class CameraMotion
    {
        // -----------------------------------------------------------
        // Constants
        // -----------------------------------------------------------
        public const float SENSITIVITY = 0.005f;
        public const float SMOOTH_FACTOR = 10.0f;
        public const float INITIAL_TILT = -50f;

        // -----------------------------------------------------------
        // Private attributes
        // -----------------------------------------------------------
        private int _targetId;
        private Vector3 _initialPosition;
        // Angular movement related attributes
        private float _yawSpeed;
        private float _targetYawSpeed;
        private float _pitchSpeed;
        private float _targetPitchSpeed;

        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------

        /// <summary>Yaw rotation of the camera.</summary>
        public float Yaw;

        /// <summary>Pitch rotation of the camera.</summary>
        public float Pitch;

        /// <summary>Defines if the camera is focused on a single planet.</summary>
        public CameraState State;
        
        /// <summary>Target object.</summary>
        public AstralObject? Target;

        // -----------------------------------------------------------
        // Public properties
        // -----------------------------------------------------------

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
            State = CameraState.Free;
            _targetId = -1;
        }

        /// <summary>Rotates a camera around its up vector. Yaw is looking "left" and "right". NOTE : angle must be in radians.</summary>
        /// <param name="camera">The camera to update.</param>
        /// <param name="angle">The angle to set (in radians).</param>
        public void UpdateYaw(ref Camera3D camera, float angle)
        {
            // Update yaw speed and value
            _targetYawSpeed = angle;
            _yawSpeed = Raymath.Lerp(_yawSpeed, _targetYawSpeed, (float)Raylib.GetFrameTime() * SMOOTH_FACTOR);
            Yaw += _yawSpeed * Raylib.RAD2DEG;

            // View vector
            Vector3 view = camera.Target - camera.Position;

            // Rotate view vector around rotation axis
            view = Raymath.Vector3RotateByAxisAngle(view, camera.Up, _yawSpeed);

            // Set updated position
            camera.Position = camera.Target - view;
        }

        /// <summary>Rotates a camera around its right vector. Pitch is looking "up" and "down". NOTE : angle must be in radians.</summary>
        /// <param name="camera">The camera to update.</param>
        /// <param name="angle">The angle to set (in radians).</param>
        public void UpdatePitch(ref Camera3D camera, float angle)
        {   
            // Update pitch speed and value
            _targetPitchSpeed = angle;
            _pitchSpeed = Raymath.Lerp(_pitchSpeed, _targetPitchSpeed, (float)Raylib.GetFrameTime() * SMOOTH_FACTOR);
            Pitch += _pitchSpeed * Raylib.RAD2DEG;

            // View vector 
            Vector3 view = camera.Target - camera.Position;

            // Rotation axis
            Vector3 right = Raylib.GetCameraRight(ref camera);

            // Rotate view vector around right axis (with clamped angle)
            view = Raymath.Vector3RotateByAxisAngle(view, right, _pitchSpeed);

            // Update the camera's up vector to prevent clipping when flipping over 
            camera.Up = Raymath.Vector3CrossProduct(right, view);

            // Set updated position
            camera.Position = camera.Target - view;
        }

        public void UpdateSmoothDownMovement(ref Camera3D camera)
        {
            // View vector
            Vector3 view = camera.Target - camera.Position;

            // Smooth down movement until it reached a speed of zero
            _yawSpeed = Raymath.Lerp(_yawSpeed, 0, (float)Raylib.GetFrameTime());

            // Rotate view vector around rotation axis
            view = Raymath.Vector3RotateByAxisAngle(view, camera.Up, _yawSpeed);

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
            State = CameraState.Focused;
            Target = Conceptor3D.System.GetObject(TargetId); // Get next target
            Conceptor2D.DisplayObject(Target);
        }
    }
}