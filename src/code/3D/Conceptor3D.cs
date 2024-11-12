using Raylib_cs;
using static Raylib_cs.Raylib;
using Raylib_cs.Complements;
using static Raylib_cs.Complements.Raylib;
using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="Conceptor3D"/>.</summary>
    public static class Conceptor3D
    {
        // -----------------------------------------------------------
        // Private instances
        // -----------------------------------------------------------

        private static Mesh _sphereMesh = GenMeshSphere(5, 50, 50); // Default planet mesh
        private static Camera3D _camera;
        private static CameraMotion _cameraMotion = new CameraMotion();
        private static Skybox _skybox;

        public static List<AstralObject> AstralObjects = new List<AstralObject>();

        /// <summary>Initializes the 3D environnment of the application.</summary>
        public static void Init()
        {
            _camera = new Camera3D() // Init 3D camera
            {
                Position = new Vector3(1f, 1f, 1f),
                Target = Vector3.UnitX,
                Up = Vector3.UnitY,
                FovY = 45f,
                Projection = CameraProjection.Perspective
            };
            _cameraMotion = new CameraMotion(10, (short)GetScreenWidth(), (short)GetScreenHeight());
            _skybox = LoadSkybox("assets/shaders/skyboxes/HDR_blue_nebulae-1.hdr");

            // Load default system
        }

        /// <summary>Draws the 3D environnement of the application.</summary>
        public static void Draw()
        {
            // -----------------------------------------------------------
            // Camera updates
            // -----------------------------------------------------------
            if (IsMouseButtonReleased(MouseButton.Middle))
            {
                _cameraMotion.Mouse = _cameraMotion.FakePosition;
                _cameraMotion.MouseOrigin = Vector2.Zero;
            }
            if (IsMouseButtonDown(MouseButton.Middle))
            {

                if (IsKeyDown(KeyboardKey.LeftShift))
                {
                    Vector3 movX = GetCameraRight(ref _camera) * GetMouseDelta().X * (_cameraMotion.Distance / 200);
                    Vector3 movY = GetCameraUp(ref _camera) * GetMouseDelta().Y * (_cameraMotion.Distance / 200);

                    _camera.Position -= movX * 0.2f;
                    _camera.Target -= movX * 0.2f;

                    _camera.Position += movY * 0.2f;
                    _camera.Target += movY * 0.2f;
                }
                else
                {
                    if (_cameraMotion.MouseOrigin == Vector2.Zero) { _cameraMotion.MouseOrigin = GetMousePosition(); }
                    _cameraMotion.FakePosition = MoveCamera(_cameraMotion.Distance, ref _camera, _camera.Target, _cameraMotion.YOffset, false, _cameraMotion.Mouse, _cameraMotion.MouseOrigin);
                }
            }
            else
            {
                _cameraMotion.Distance -= GetMouseWheelMove() * 2f * Raymath.Vector3Distance(_camera.Position, _camera.Target) / 10;
                MoveCamera(_cameraMotion.Distance, ref _camera, _camera.Target, _cameraMotion.YOffset, true, _cameraMotion.Mouse, _cameraMotion.MouseOrigin);
            }

            // -----------------------------------------------------------
            // Draw calls
            // -----------------------------------------------------------

            BeginMode3D(_camera);
            DrawSkybox(_skybox);
            
            // System rendering
            foreach (AstralObject obj in AstralObjects)
            {
                DrawMesh(_sphereMesh, obj.Material1, obj.Transform);
            }

            EndMode3D();
        }

        /// <summary>Closes the conceptor by unloading all its data.</summary>
        public static void Close()
        {
            UnloadSkybox(_skybox);
        }

        /// <summary>Moves the conceptor's camera.</summary>
        /// <param name="distance">Distance from the target.</param>
        /// <param name="camera">3D camera of the editor.</param>
        /// <param name="targetPosition">Target of the camera.</param>
        /// <param name="yOffset">Well I don't even remember what this is.</param>
        /// <param name="zoom">Is zoom possible ?</param>
        /// <param name="mousePos">Last position of the mouse</param>
        /// <param name="mouseOrigin">First position of the mouse when interacting with movement</param>
        /// <returns></returns>
        static Vector2 MoveCamera(float distance, ref Camera3D camera, Vector3 targetPosition, float yOffset, bool zoom, Vector2 mousePos, Vector2 mouseOrigin)
        {
            float alpha = 0;
            float beta = 0;
            Vector2 verticalPosition = CalculateVerticalPosition(distance, targetPosition, ref alpha, zoom, mousePos, mouseOrigin);
            Vector2 HorizontalPosition = CalculateHorizontalPosition(distance, targetPosition, ref beta, zoom, mousePos, mouseOrigin);
            camera.Position.Y = verticalPosition.Y + yOffset;
            camera.Position.X = HorizontalPosition.X;
            camera.Position.Z = HorizontalPosition.Y;

            return mousePos - (mouseOrigin - GetMousePosition());
        }

        /// <summary>Calculate the vertical position of the conceptor's camera.</summary>
        /// <param name="distance">Distance from the target</param>
        /// <param name="targetPosition">Target of the camera</param>
        /// <param name="alpha">Alpha angle</param>
        /// <param name="zoom">Is zoom possible ?</param>
        /// <param name="m">Last position of the mouse</param>
        /// <param name="mO">First position of the mouse when interacting with movement</param>
        /// <returns></returns>
        static Vector2 CalculateVerticalPosition(float distance, Vector3 targetPosition, ref float alpha, bool zoom, Vector2 m, Vector2 mO)
        {
            if (!zoom) alpha = (m.Y - (mO.Y - GetMousePosition().Y)) * 0.005f;
            else alpha = m.Y * 0.005f;
            float offsetZ = (float)(distance * Math.Cos(alpha));
            float offsetY = (float)(distance * Math.Sin(alpha));
            float posY = targetPosition.Y + offsetY;
            float posZ = targetPosition.Z + offsetZ;
            return new Vector2(posZ, posY);
        }

        /// <summary>Calculates the horizontal position of the conceptor's camera.</summary>
        /// <param name="distance">Distance from the target</param>
        /// <param name="targetPosition">Target of the camera</param>
        /// <param name="beta">Beta angle</param>
        /// <param name="zoom">Is zoom possible ?</param>
        /// <param name="m">Last position of the mouse</param>
        /// <param name="mO">First position of the mouse when interacting with movement</param>
        /// <returns></returns>
        static Vector2 CalculateHorizontalPosition(float distance, Vector3 targetPosition, ref float beta, bool zoom, Vector2 m, Vector2 mO)
        {
            if (!zoom) beta = (m.X - (mO.X - GetMousePosition().X)) * 0.005f;
            else beta = m.X * 0.005f;
            float offsetX = (float)(distance * Math.Cos(beta));
            float offsetZ = (float)(distance * Math.Sin(beta));
            float posX = targetPosition.X + offsetX;
            float posZ = targetPosition.Z + offsetZ;
            return new Vector2(posX, posZ);
        }
    }
}