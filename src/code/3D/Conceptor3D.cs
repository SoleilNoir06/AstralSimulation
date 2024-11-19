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

        public static System System = new System(); // Init default system

        // Debug
        public static Vector3 targetPosition = Vector3.Zero;

        /// <summary>Initializes the 3D environnment of the application.</summary>
        public static void Init()
        {
            _camera = new Camera3D() // Init 3D camera
            {
                Position = new Vector3(2f, 2f, 2f),
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
            // Move camera
            MoveCamera();

            // Update planet click
            ClickAstralObject();

            if (IsKeyPressed(KeyboardKey.M))
            {
                _cameraMotion.InTransit = true;
                targetPosition = System.GetObject("Mars").Position + System.GetObject("Mars").Radius * 5 * Raymath.Vector3Normalize(Raymath.Vector3Subtract(System.GetObject("Sun").Position, System.GetObject("Mars").Position));
                _cameraMotion.Velocity = Vector3.Zero;
            }

            if (_cameraMotion.InTransit)
            {
                if (Raymath.Vector3Subtract(_camera.Position, targetPosition).Length() > 0.02f)
                {
                    _camera.Position = Raymath.Vector3Lerp(_camera.Position, targetPosition, (float)GetFrameTime());
                    _camera.Target = Raymath.Vector3Lerp(_camera.Target, targetPosition, (float)GetFrameTime());
                }
                else
                {
                    _cameraMotion.InTransit = false;
                }
            }

            // -----------------------------------------------------------
            // Draw calls
            // -----------------------------------------------------------

            BeginMode3D(_camera);
            DrawSkybox(_skybox);
            
            // System rendering
            System.ForEach(obj =>
            {
                DrawMesh(_sphereMesh, obj.Material1, obj.Transform);
            });

            EndMode3D();
        }

        /// <summary>Checks for a click on astra object and opens modal info if clicked.</summary>
        public static void ClickAstralObject()
        {
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                bool click = false;
                Ray mouse = GetMouseRay(GetMousePosition(), _camera); // Get mouse ray
                bool collision = false; // Init collision detection object
                System.ForEach(obj =>
                {
                    if (!click)
                    {
                        //RayCollision currentCollision = GetRayCollisionSphere(mouse, obj.Position, obj.Radius);
                        bool currentCollision = CheckRaySphereIntersection(mouse, obj.Position, obj.Radius);
                        if (currentCollision) 
                        {
                            collision = currentCollision;
                            Conceptor2D.DisplayObject(obj); // Display object infos
                            click = true;
                        }
                    }
                });
                if (!click) Conceptor2D.Components.Clear();
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
            }
            if (IsKeyDown(KeyboardKey.S))
            {
                _cameraMotion.Velocity -= CameraMotion.SPEED * GetCameraForward(ref _camera);
            }
            if (IsKeyDown(KeyboardKey.A))
            {
                _cameraMotion.Velocity -= CameraMotion.SPEED * GetCameraRight(ref _camera);
            }
            if (IsKeyDown(KeyboardKey.D))
            {
                _cameraMotion.Velocity += CameraMotion.SPEED * GetCameraRight(ref _camera);
            }
            if (IsKeyDown(KeyboardKey.F))
            {
                _cameraMotion.Velocity -= CameraMotion.SPEED * GetCameraUp(ref _camera);
            }
            if (IsKeyDown(KeyboardKey.Space))
            {
                _cameraMotion.Velocity += CameraMotion.SPEED * GetCameraUp(ref _camera);
            }
        }

        /// <summary>Checks for non-inverse sphere-ray collision.</summary>
        /// <param name="ray">Ray to use.</param>
        /// <param name="sphereCenter">Sphere center.</param>
        /// <param name="sphereRadius">Sphere radius.</param>
        /// <returns><see langword="true"/> if collision occurs. <see langword="false"/> otherwise.</returns>
        public static bool CheckRaySphereIntersection(Ray ray, Vector3 sphereCenter, float sphereRadius)
        {
            Vector3 m = Raymath.Vector3Subtract(ray.Position, sphereCenter);
            float b = Raymath.Vector3DotProduct(m, ray.Direction);
            float c = Raymath.Vector3DotProduct(m, m) - sphereRadius * sphereRadius;

            if (c > 0.0f && b > 0.0f) return false;

            float discr = b * b - c;
            return discr >= 0.0f;
        }
    }
}