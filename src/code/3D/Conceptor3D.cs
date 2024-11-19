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
        private static Skybox _skybox;
        private static RenderTexture2D _renderTexture = LoadRenderTexture(GetScreenWidth(), GetScreenHeight());
        private static Rectangle _srcRectangle = new Rectangle(Vector2.Zero, GetScreenWidth(), -GetScreenHeight());
        private static Rectangle _destRectangle = new Rectangle(Vector2.Zero, GetScreenWidth(), GetScreenHeight());

        public static Probe Probe = new Probe(); // Init default probe
        public static System System = new System(); // Init default system
        public static Camera3D Camera;

        /// <summary>Initializes the 3D environnment of the application.</summary>
        public static void Init()
        {
            Camera = new Camera3D() // Init 3D camera
            {
                Position = new Vector3(2f, 2f, 2f),
                Target = Vector3.UnitX,
                Up = Vector3.UnitY,
                FovY = 45f,
                Projection = CameraProjection.Perspective
            };
            Probe = new Probe(10, (short)GetScreenWidth(), (short)GetScreenHeight());
            //_skybox = LoadSkybox("assets/shaders/skyboxes/HDR_blue_nebulae-1.hdr");
        }

        /// <summary>Draws the 3D environnement of the application.</summary>
        public static void Draw()
        {
            // Update sun to shaders
            ShaderCenter.UpdateSun(System.GetObject(0).Position, System.GetObject(0).Radius);

            // Move camera
            MoveCamera();

            // Update planet click
            ClickAstralObject();

            // Update probe functions
            UpdateProbe();

            // -----------------------------------------------------------
            // Draw calls
            // -----------------------------------------------------------

            BeginTextureMode(_renderTexture);

            ClearBackground(Color.Black);

            BeginMode3D(Camera);

            //DrawSkybox(_skybox);
            
            // System rendering
            System.ForEach(obj =>
            {
                DrawMesh(_sphereMesh, obj.Material1, obj.Transform);
            });

            EndMode3D();

            EndTextureMode();

            // Draw texture
            BeginShaderMode(ShaderCenter.SunShader);
            DrawTexturePro(_renderTexture.Texture, _srcRectangle, _destRectangle, Vector2.Zero, 0, Color.White);
            EndShaderMode();
        }

        /// <summary>Checks for a click on astra object and opens modal info if clicked.</summary>
        public static void ClickAstralObject()
        {
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                bool click = false;
                Ray mouse = GetMouseRay(GetMousePosition(), Camera); // Get mouse ray
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

        /// <summary>Updates the functions of the realstic probe.</summary>
        public static void UpdateProbe()
        {
            if (IsKeyPressed(KeyboardKey.Right))
            {
                Probe.TargetId++;
                Probe.InTransit = true;
                Probe.Target = System.GetObject(Probe.TargetId); // Get next target
                Probe.Velocity = Vector3.Zero;
            }
            if (IsKeyPressed(KeyboardKey.Left))
            {
                Probe.TargetId--;
                Probe.InTransit = true;
                Probe.Target = System.GetObject(Probe.TargetId); // Get next target
                Probe.Velocity = Vector3.Zero;
            }

            if (Probe.InTransit)
            {
                if (Raymath.Vector3Subtract(Camera.Position, Probe.Target.Position).Length() > 0.02f)
                {
                    Camera.Position = Raymath.Vector3Lerp(Camera.Position, Probe.Target.Position, (float)GetFrameTime());
                    Camera.Target = Raymath.Vector3Lerp(Camera.Target, Probe.Target.Position, (float)GetFrameTime());
                }
                else
                {
                    Probe.InTransit = false;
                }
            }
        }

        /// <summary>Moves the conceptor's camera.</summary>
        static void MoveCamera()
        {
            if (IsMouseButtonDown(MouseButton.Left))
            {
                Vector2 mouse = GetMouseDelta();
                Probe.Yaw -= mouse.X * 0.003f;
                Probe.Pitch -= mouse.Y * 0.003f;

                Probe.Pitch = Math.Clamp(Probe.Pitch, -1.5f, 1.5f);

                // Calculate camera direction
                Vector3 direction;
                direction.X = (float)(Math.Cos(Probe.Pitch) * Math.Sin(Probe.Yaw));
                direction.Y = (float)Math.Sin(Probe.Pitch);
                //direction.Y = 0;
                direction.Z = (float)(Math.Cos(Probe.Pitch) * Math.Cos(Probe.Yaw));

                // Add target
                Camera.Target = Vector3.Add(Camera.Position, direction);
            }

            Camera.Position += Probe.Velocity;
            Camera.Target += Probe.Velocity;

            Vector3 zoom = GetMouseWheelMove() * Probe.SPEED * 10000 * GetCameraForward(ref Camera);
            Camera.Position += zoom;
            Camera.Target += zoom;

            // Keys movement
            if (IsKeyDown(KeyboardKey.W))
            {
                Probe.Velocity += Probe.SPEED * GetCameraForward(ref Camera);
            }
            if (IsKeyDown(KeyboardKey.S))
            {
                Probe.Velocity -= Probe.SPEED * GetCameraForward(ref Camera);
            }
            if (IsKeyDown(KeyboardKey.A))
            {
                Probe.Velocity -= Probe.SPEED * GetCameraRight(ref Camera);
            }
            if (IsKeyDown(KeyboardKey.D))
            {
                Probe.Velocity += Probe.SPEED * GetCameraRight(ref Camera);
            }
            if (IsKeyDown(KeyboardKey.F))
            {
                Probe.Velocity -= Probe.SPEED * GetCameraUp(ref Camera);
            }
            if (IsKeyDown(KeyboardKey.Space))
            {
                Probe.Velocity += Probe.SPEED * GetCameraUp(ref Camera);
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