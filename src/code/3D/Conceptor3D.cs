using Raylib_cs;
using static Raylib_cs.Raylib;
using Raylib_cs.Complements;
using static Raylib_cs.Complements.Raylib;
using System.Numerics;
using Astral_Simulation;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="Conceptor3D"/>.</summary>
    public unsafe static class Conceptor3D
    {
        public const int SCALE = 15000000; // UA
        public const float VOYAGER_SCALE = 4; // Voyager mode scale
        public const float VOYAGER_DISTANCE_SCALE = 20;

        public static readonly Vector4 SUN_COLOR = new Vector4(0.5f, 0.41f, 0.3f, 1.0f); // Normalized

        // -----------------------------------------------------------
        // Private instances
        // -----------------------------------------------------------
        private static Mesh _sphereMesh = GenMeshSphere(1f, 50, 50); // Default planet mesh
        private static Mesh _rings = GenMeshPlane(8, 8, 1, 1);
        private static Material _ringsMat = LoadMaterialDefault();
        private static Matrix4x4 saturnRings = Matrix4x4.Identity;
        private static Skybox _skybox;
        private static RenderTexture2D _renderTexture = LoadRenderTexture(GetScreenWidth(), GetScreenHeight());
        private static Rectangle _srcRectangle = new Rectangle(Vector2.Zero, GetScreenWidth(), -GetScreenHeight());
        private static Rectangle _destRectangle = new Rectangle(Vector2.Zero, GetScreenWidth(), GetScreenHeight());
        private static bool TrailsOn = true;

        public static System System = new System(); // Init default system
        public static CameraMotion CameraParams = new CameraMotion(); // Init default probe
        public static Camera3D Camera;

        /// <summary>Initializes the 3D environnment of the application.</summary>
        public static void Init()
        {
            // Create camera object with base parameters
            Camera = new Camera3D() 
            {
                Position = new Vector3(40f, 40f, 0f),
                Target = Vector3.Zero,
                Up = Vector3.UnitZ,
                FovY = 45f,
                Projection = CameraProjection.Perspective
            };

            // Create additional camera parameters
            CameraParams = new CameraMotion();

            // Set base camera orientation
            CameraParams.UpdatePitch(ref Camera, CameraMotion.INITIAL_TILT*DEG2RAD);
            CameraParams.RegisterInitialPosition(Camera.Position);

            _skybox = LoadSkybox("assets/shaders/skyboxes/HDR_blue_nebulae-1.hdr");
            SetMaterialTexture(ref _ringsMat, MaterialMapIndex.Diffuse, LoadTexture("assets/textures/saturn_ring.png"));
            _ringsMat.Shader = ShaderCenter.LightingShader;
        }

        /// <summary>Draws the 3D environnement of the application.</summary>
        public static void Draw()
        {
            // Move camera
            MoveCamera();

            // Update planet click
            ClickAstralObject();

            // -----------------------------------------------------------
            // Occlusion map rendering 
            // -----------------------------------------------------------
            BeginTextureMode(ShaderCenter.OcclusionMap);

            ClearBackground(Color.Black);

            BeginMode3D(Camera);

            System.ForEach(obj =>
            {
                //if (obj.Name == "Saturn") DrawMesh(_rings, _ringsMat, obj.Transform);
                DrawSphere(obj.Position, obj.Radius, Color.White);
            });

            EndMode3D();

            EndTextureMode();

            // -----------------------------------------------------------
            // Draw calls
            // -----------------------------------------------------------

            BeginTextureMode(_renderTexture);

            ClearBackground(Color.Black);

            BeginMode3D(Camera);

            DrawSkybox(_skybox);

            // System rendering
            System.ForEach(obj =>
            {
                Physics.Update(obj);
                Physics.DrawOrbitPath(obj);
                DrawMesh(_sphereMesh, obj.Material1, obj.Transform);

                //if (TrailsOn) DrawCircle3D(Vector3.Zero, obj.Position.Length(), Vector3.UnitX, 90, Color.White);
            });

            EndMode3D();

            DrawText(Camera.Position.ToString(), 40, 100, 20, Color.White);
            DrawText(CameraParams.Pitch.ToString(), 40, 140, 20, Color.White);

            EndTextureMode();

            // Update post-pro shader
            UpdatePostProcessingShader();

            if (IsKeyPressed(KeyboardKey.R)) TrailsOn = !TrailsOn;
        }

        /// <summary>Checks for a click on astra object and opens modal info if clicked.</summary>
        public static void ClickAstralObject()
        {
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                bool click = false;
                int index = 0;
                Ray mouse = GetScreenToWorldRay(GetMousePosition(), Camera); // Get mouse ray
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
                            CameraParams.TargetId = index;
                            CameraParams.DefineTarget();
                            click = true;
                        }
                    }
                    index++;
                });
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
            // -----------------------------------------------------------
            // Constant camera-movement options
            // -----------------------------------------------------------
            
            // Allow free movement only when mouse is pressed and when not in focused camera-mode
            if (IsMouseButtonDown(MouseButton.Left) && !CameraParams.Focus)
            {
                Vector2 mouseDelta = GetMouseDelta();

                CameraParams.UpdateYaw(ref Camera, -mouseDelta.X*CameraMotion.SENSITIVITY);
                CameraParams.UpdatePitch(ref Camera, -mouseDelta.Y*CameraMotion.SENSITIVITY);   
            }

            // Define movement when in focused camera-mode
            if (CameraParams.Focus)
            {
                if (Raymath.Vector3Subtract(Camera.Position, CameraParams.Target.Position).Length() > 0.02f)
                {
                    Camera.Position = Raymath.Vector3Lerp(Camera.Position, CameraParams.Target.Position + Vector3.UnitY * 0.5f + (CameraParams.Target.Radius * Vector3.Subtract(Camera.Position, CameraParams.Target.Position)), (float)GetFrameTime() * 2);
                    Camera.Target = Raymath.Vector3Lerp(Camera.Position, CameraParams.Target.Position + Vector3.UnitY * 0.05f + (CameraParams.Target.Radius * Vector3.Subtract(Camera.Position, CameraParams.Target.Position)), (float)GetFrameTime() * 2);
                }
                else
                {
                    CameraParams.Focus = false;
                }
            }

            // -----------------------------------------------------------
            // Single-time camera events 
            // -----------------------------------------------------------

            // Enable focused camera-mode with right element
            if (IsKeyPressed(KeyboardKey.Right))
            {
                CameraParams.TargetId++;
                CameraParams.DefineTarget();
            }

            // Enable focused camera-mode with left element
            if (IsKeyPressed(KeyboardKey.Left))
            {
                CameraParams.TargetId--;
                CameraParams.DefineTarget();
            }

            // Escape focused camera-mode
            if (IsKeyPressed(KeyboardKey.Escape))
            {
                CameraParams.Focus = false;
                Conceptor2D.Components.Clear();
                // Set camera-target to point at the sun
                Camera.Target = Vector3.Zero;
                Camera.Position = CameraParams.InitialPosition;
            }
        }

        /// <summary>Updates the post-processing shader values.</summary>
        public static void UpdatePostProcessingShader()
        {
            // Draw texture
            //BeginShaderMode(ShaderCenter.SunShader);
            // Calculate new values
            // Determine if sun behind or not
            Vector3 camDirection = Vector3.Normalize(Vector3.Subtract(Camera.Target, Camera.Position));
            Vector3 sunDirection = Vector3.Normalize(-Camera.Position);
            float dotProduct = Raymath.Vector3DotProduct(camDirection, sunDirection);
            bool isSunVisible = dotProduct > 0;
            if (isSunVisible) 
            {
                float camDist = Raymath.Clamp(25 / MathF.Log(Camera.Position.Length() + 1), 3, 25);
                ShaderCenter.UpdateShine(Camera, camDist);
            }

            DrawTexturePro(_renderTexture.Texture, _srcRectangle, _destRectangle, Vector2.Zero, 0, Color.White);
            EndShaderMode();

            //DrawCircle((int)sunPos.X, (int)sunPos.Y, shineSize * 1000, Color.Blue);
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