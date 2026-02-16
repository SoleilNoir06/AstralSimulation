using System.Numerics;
using Raylib_cs;

namespace Astral_simulation
{
    /// <summary>Defines the current movement state of the camera.</summary>
    public enum CameraState
    {
        Free,
        Focused,
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
        // Angular movement related attributes
        private float _yawSpeed;
        private float _targetYawSpeed;
        private float _pitchSpeed;
        private float _targetPitchSpeed;
        // Linear movement related attributes
        private Vector3 _targetPosition;
        private Vector3 _targetView;

        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------

        /// <summary>Yaw rotation of the camera.</summary>
        public float Yaw;

        /// <summary>Pitch rotation of the camera.</summary>
        public float Pitch;

        /// <summary>Defines whether the camera is locked with the focused object or not.</summary>
        public bool AstralLock;

        /// <summary>Defines the position at which the camera must tend when approaching an object.</summary>
        public Vector3 ApprochedTarget;

        /// <summary>Defines the direction at which the camera must tend when approaching an object.</summary>
        public Vector3 ApproachedDirection;

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

        internal string Infos { get {return $"Yaw Speed : {_yawSpeed}\nTarget Yaw Speed : {_targetYawSpeed}\nPitch Speed : {_pitchSpeed}\nTarget Pitch Speed : {_targetPitchSpeed}";} }

        /// <summary>Creates an empty <see cref="CameraMotion"/> instance.</summary>
        public CameraMotion()
        {
            State = CameraState.Free;
            AstralLock = false;
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
            Vector3 _targetViewVector = _targetView - _targetPosition; // used for interpolation

            // Rotate view vector around rotation axis
            view = Raymath.Vector3RotateByAxisAngle(view, camera.Up, _yawSpeed);
            _targetViewVector = Raymath.Vector3RotateByAxisAngle(_targetViewVector, camera.Up, _yawSpeed);

            // Set updated position
            camera.Position = camera.Target - view;
            _targetPosition = _targetView - _targetViewVector;
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
            Vector3 _targetViewVector = _targetView - _targetPosition; // used for interpolation

            // Rotation axis
            Vector3 right = Raylib.GetCameraRight(ref camera);

            // Rotate view vector around right axis (with clamped angle)
            view = Raymath.Vector3RotateByAxisAngle(view, right, _pitchSpeed);
            _targetViewVector = Raymath.Vector3RotateByAxisAngle(_targetViewVector, right, _pitchSpeed);

            // Update the camera's up vector to prevent clipping when flipping over 
            camera.Up = Raymath.Vector3CrossProduct(right, view);

            // Set updated position
            camera.Position = camera.Target - view;
            _targetPosition = _targetView - _targetViewVector;
        }

        /// <summary>Updates the movement of the camera using values intended for linear interpolations only.</summary>
        /// <param name="camera">The camera to update.</param>
        public void UpdateLinearMovement(ref Camera3D camera)
        {
            // Update linear camera movements using an interpolation
            // (Linear movements refer to the camera zoom as well as object paths)
            camera.Position = Raymath.Vector3Lerp(camera.Position, _targetPosition, (float)Raylib.GetFrameTime() * SMOOTH_FACTOR);    
            camera.Target = Raymath.Vector3Lerp(camera.Target, _targetView, (float)Raylib.GetFrameTime());
        }

        /// <summary>Defines the next zoom level to be applied.</summary>
        /// <param name="camera">The camera to update.</param>
        /// <param name="zoom">The next zoom level to apply</param>
        public void DefineZoomLevel(Camera3D camera, float zoom)
        {
            Vector3 direction = camera.Target - camera.Position;
            _targetPosition = camera.Position + Math.Sign(zoom)*direction/3;
            _targetView = camera.Target;
        }

        /// <summary>Defines the target for the probe.</summary>
        public void DefineObjectTarget()
        {
            State = CameraState.Focused;
            Target = Conceptor3D.System.GetObject(TargetId); // Get next target
            Conceptor2D.DisplayObject(Target);
        }

        /// <summary> Registers the initial paramters for the camera.</summary>
        /// <param name="camera">The camera to register.</param>
        public void RegisterInitialPosition(ref Camera3D camera)
        {
            // Set initial tilt without implying interpolation
            Vector3 view = camera.Target - camera.Position;
            Vector3 right = Raylib.GetCameraRight(ref camera);
            view = Raymath.Vector3RotateByAxisAngle(view, right, INITIAL_TILT*Raylib.DEG2RAD);
            // Update the camera's up vector to prevent clipping when flipping over 
            camera.Up = Raymath.Vector3CrossProduct(right, view);
            camera.Position = camera.Target - view;

             // Set initial values
            _targetPosition = camera.Position;
        }
    }
}