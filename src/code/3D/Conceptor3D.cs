using Raylib_cs;
using static Raylib_cs.Raylib;
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
        private static RenderTexture2D _renderTexture = LoadRenderTexture(GetScreenWidth(), GetScreenHeight());
        private static Rectangle _srcRectangle = new Rectangle(Vector2.Zero, GetScreenWidth(), -GetScreenHeight());
        private static Rectangle _destRectangle = new Rectangle(Vector2.Zero, GetScreenWidth(), GetScreenHeight());
        private static bool TrailsOn = true;

        public static System System = new System(); // Init default system
        public static CameraMotion CameraParams = new CameraMotion(); // Create additional camera parameters
        public static Camera3D Camera; // Create an empty instance of a camera

        /// <summary>Initializes the 3D environnment of the application.</summary>
        public static void Init()
        {
            // Create camera object with base parameters
            Camera = new Camera3D() 
            {
                Position = new Vector3(60f, 60f, 0f),
                Target = Vector3.Zero,
                Up = Vector3.UnitZ,
                FovY = 45f,
                Projection = CameraProjection.Perspective
            };

            // Update physics engine at launch
            System.ForEach(Physics.Update);
            AstralObject _sun = System.GetObject("Sun");
            Camera.Target = _sun.Position; // Set initial target position

            // Set base camera orientation
            CameraParams.RegisterInitialSettings(ref Camera);

            // Enter focused mode
            CameraParams.State = CameraState.Focused;
            CameraParams.AstralLock = true;
            CameraParams.Target = _sun;
            CameraParams.ApprochedTarget = _sun.Position + GetCameraRight(ref Camera) * _sun.Radius*6;

            SetMaterialTexture(ref _ringsMat, MaterialMapIndex.Diffuse, LoadTexture("assets/textures/saturn_ring.png"));
            _ringsMat.Shader = ShaderCenter.LightingShader;
        }

        /// <summary>Draws the 3D environnement of the application.</summary>
        public static void Draw()
        {
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

            // System rendering
            System.ForEach(obj =>
            {
                Physics.Update(obj);
                Physics.DrawOrbitPath(obj);
                DrawMesh(_sphereMesh, obj.Material1, obj.Transform);
            });

            // Update camera movement
            MoveCamera();

            EndMode3D();

            Conceptor2D.DisplayUITopLayer();

            EndTextureMode();

            // Update post-pro shader
            UpdatePostProcessingShader();

            if (IsKeyPressed(KeyboardKey.R)) TrailsOn = !TrailsOn;
        }

        /// <summary>Checks for a click on astra object and opens modal info if clicked.</summary>
        public static void ClickAstralObject()
        {
            if (IsMouseButtonReleased(MouseButton.Left))
            {
                int index = 0;
                System.ForEach(obj =>
                {
                    if (obj.UIActive)
                    {
                        // Play click sound
                        AudioCenter.PlaySound("button_2");
                        CameraParams.AstralLock = false;
                        CameraParams.ApproachedDirection = GetCameraRight(ref Camera);
                        CameraParams.Target = obj;
                    }
                    index++;
                });
            }
        }

        /// <summary>Moves the conceptor's camera.</summary>
        static void MoveCamera()
        {
            // -----------------------------------------------------------
            // Constant camera-movement options
            // -----------------------------------------------------------
            
            switch (CameraParams.State){
                // Allow free movement only when mouse is pressed and when not in focused camera-mode
                case CameraState.Free:
                    UpdateCameraFreeMode();
                break;
                
                // Define movement when in focused camera-mode
                case CameraState.Focused:
                    
                    // Compute exponential interpolator
                    float dist = Raymath.Vector3Subtract(Camera.Position, CameraParams.Target.Position).Length();
                    float smoothing = Raymath.Clamp(1/dist, 3, float.PositiveInfinity); // vitesse de rattrapage
                    float t = 1 - MathF.Exp(-smoothing * GetFrameTime());
                    
                    // Constantly lerp camera target
                    CameraParams.ApprochedTarget = CameraParams.Target.Position + CameraParams.ApproachedDirection * CameraParams.Target.Radius*6;
                    // Enable free mode when close enough
                    Camera.Target = Raymath.Vector3Lerp(Camera.Target, CameraParams.Target.Position, t);

                    if (!CameraParams.AstralLock && (CameraParams.ApprochedTarget - Camera.Position).Length() > 0.001)
                    {
                        Camera.Position = Raymath.Vector3Lerp(Camera.Position, CameraParams.ApprochedTarget, GetFrameTime()*2);
                    }
                    else
                    {
                        // Make the camera position follow the object as weel
                        // Reset zoom level to state that the new location is base zoom level
                        if (!CameraParams.AstralLock) CameraParams.DefineZoomLevel(Camera, 0);
                        CameraParams.AstralLock = true;
                        // Enable free mode arround the object
                        UpdateCameraFreeMode();
                        // Increment horizontal angle to give an automatic orbital movement (when not moving)
                        if (IsMouseButtonUp(MouseButton.Left)) 
                        {
                            float d = Raymath.Vector3Subtract(CameraParams.ApprochedTarget, Camera.Position).Length() / CameraParams.Target.Radius;
                            float a = 1 - Raymath.Clamp(Raymath.Normalize(d, 150, 300), 0, 1); // <- Don't question theses values, found em while debugging
                            CameraParams.UpdateYaw(ref Camera, GetFrameTime()*CameraMotion.SENSITIVITY*10*a);
                        }
                    }
                break;
            }
        }

        /// <summary>Defines the movement for the free camera-mode. (Used in total freedom and around objects)</summary>
        private static void UpdateCameraFreeMode()
        {
            // Set the target speed to whatever values the user inputs (mouse movement)
            if (IsMouseButtonDown(MouseButton.Left))
            {
                Vector2 mouseDelta = GetMouseDelta();
                CameraParams.UpdateYaw(ref Camera, -mouseDelta.X*CameraMotion.SENSITIVITY);
                CameraParams.UpdatePitch(ref Camera, -mouseDelta.Y*CameraMotion.SENSITIVITY);   
            }
            // Set the target speed to zero so that the camera slows down smoothly 
            else
            {
                CameraParams.UpdateYaw(ref Camera, 0);
                CameraParams.UpdatePitch(ref Camera, 0);
            }

            // Control camera zoom
            float zoom = GetMouseWheelMove();
            if (zoom != 0) CameraParams.DefineZoomLevel(Camera, zoom);
            
            // Update linear movements (in this case only the zoom is affected)
            CameraParams.UpdateLinearMovement(ref Camera);
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