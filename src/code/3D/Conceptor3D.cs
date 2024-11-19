using Raylib_cs;
using static Raylib_cs.Raylib;
using Raylib_cs.Complements;
using static Raylib_cs.Complements.Raylib;
using System.Numerics;
using RayGUI_cs;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="Conceptor3D"/>.</summary>
    public static class Conceptor3D
    {
        public const int Scale = 15000000; // UA

        // -----------------------------------------------------------
        // Private instances
        // -----------------------------------------------------------

        private static Mesh _sphereMesh = GenMeshSphere(1f, 50, 50); // Default planet mesh
        private static Camera3D _camera;
        private static CameraMotion _cameraMotion = new CameraMotion();
        private static Skybox _skybox;

        public static bool testc = false;

        public static System System = new System(); // Init default system

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
            /*if (IsMouseButtonReleased(MouseButton.Middle))
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
                _cameraMotion.Distance -= GetMouseWheelMove() * 2f * _cameraMotion.LinearVelocity;
                MoveCamera(_cameraMotion.Distance, ref _camera, _camera.Target, _cameraMotion.YOffset, true, _cameraMotion.Mouse, _cameraMotion.MouseOrigin);
            }*/

            // Move camera
            MoveCamera();

            // Update planet click
            ClickAstralObject();

            // -----------------------------------------------------------
            // Draw calls
            // -----------------------------------------------------------

            BeginMode3D(_camera);
            DrawSkybox(_skybox);
            
            // System rendering
            System.ForEach(obj =>
            {
                DrawMesh(_sphereMesh, obj.Material1, obj.Transform);
                //DrawSphere(obj.Position, 2, Color.Red)
                /*if (obj.Name == "Pluto" && !testc)
                {
                    _camera.Position = obj.Position - new Vector3(0.0005f, 0.0005f, 0.0005f);
                    _camera.Target = obj.Position;
                    testc = true;
                }*/
            });

            DrawGrid(500, 500);

            EndMode3D();
        }

        /// <summary>Checks for a click on astra object and opens modal info if clicked.</summary>
        public static void ClickAstralObject()
        {
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                Ray mouse = GetMouseRay(GetMousePosition(), _camera); // Get mouse ray
                RayCollision collision = new RayCollision(); // Init collision detection object
                System.ForEach(obj =>
                {
                    if (obj.Name == "Jupiter") Console.Write("");
                    collision = GetRayCollisionSphere(mouse, obj.Position, obj.Radius);
                    if (collision.Hit)
                    {
                        Conceptor2D.DisplayObject(obj); // Display object infos
                    }
                });
                if (!collision.Hit) Conceptor2D.Components.Clear();
            }
        }

        /// <summary>Closes the conceptor by unloading all its data.</summary>
        public static void Close()
        {
            UnloadSkybox(_skybox);
        }

        /// <summary>Moves the conceptor's camera.</summary>
        static void MoveCamera()
        {
            if (IsMouseButtonDown(MouseButton.Left))
            {
                Vector2 mouse = GetMouseDelta();
                _cameraMotion.Yaw -= mouse.X * 0.003f;
                _cameraMotion.Pitch -= mouse.Y * 0.003f;

                _cameraMotion.Pitch = Math.Clamp(_cameraMotion.Pitch, -1.5f, 1.5f);

                // Calculate camera direction
                Vector3 direction;
                direction.X = (float)(Math.Cos(_cameraMotion.Pitch) * Math.Sin(_cameraMotion.Yaw));
                direction.Y = (float)Math.Sin(_cameraMotion.Pitch);
                //direction.Y = 0;
                direction.Z = (float)(Math.Cos(_cameraMotion.Pitch) * Math.Cos(_cameraMotion.Yaw));

                // Add target
                _camera.Target = Vector3.Add(_camera.Position, direction);
            }

            _camera.Position += _cameraMotion.Velocity;
            _camera.Target += _cameraMotion.Velocity;

            Vector3 zoom = GetMouseWheelMove() * CameraMotion.SPEED * 10000 * GetCameraForward(ref _camera);
            _camera.Position += zoom;
            _camera.Target += zoom;

            // Keys movement
            if (IsKeyDown(KeyboardKey.W))
            {
                _cameraMotion.Velocity += CameraMotion.SPEED * GetCameraForward(ref _camera);
                _cameraMotion.Moving = true;
            }
            if (IsKeyDown(KeyboardKey.S))
            {
                _cameraMotion.Velocity -= CameraMotion.SPEED * GetCameraForward(ref _camera);
                _cameraMotion.Moving = true;
            }
            if (IsKeyDown(KeyboardKey.A))
            {
                _cameraMotion.Velocity -= CameraMotion.SPEED * GetCameraRight(ref _camera);
                _cameraMotion.Moving = true;
            }
            if (IsKeyDown(KeyboardKey.D))
            {
                _cameraMotion.Velocity += CameraMotion.SPEED * GetCameraRight(ref _camera);
                _cameraMotion.Moving = true;
            }
            else if (!_cameraMotion.Moving)
            {
                _cameraMotion.Velocity = Vector3.Zero;
            }
            _cameraMotion.Moving = false;
        }
    }
}