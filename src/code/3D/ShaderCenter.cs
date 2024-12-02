using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="ShaderCenter"/>.</summary>
    public static unsafe class ShaderCenter
    {
        // Post-processing shader locations
        private static int _shinePosLoc;
        private static int _timeLoc;
        private static int _camDistLoc;
        private static int _resolutionLoc;
        private static int _occlusionLoc;

        // Vertex lighting shader locations
        private static int _viewPosLoc;

        /// <summary>Cubemap loading shader.</summary>
        public static Shader CubemapShader;

        /// <summary>Skybox rendering shader.</summary>
        public static Shader SkyboxShader;

        /// <summary>Default lighting shader.</summary>
        public static Shader LightingShader;

        /// <summary>Sun light shader.</summary>
        public static Shader SunShader;

        public static RenderTexture2D OcclusionMap;
        public static Rectangle OcclusionMapSource;
        public static Rectangle OcclusionMapDestination;

        /// <summary>Inits the shader center by loading the shaders of the application.</summary>
        public static void Init()
        {
            // Load skybox shader
            SkyboxShader = LoadShader("assets/shaders/skybox.vs", "assets/shaders/skybox.fs");
            SetShaderValue(SkyboxShader, GetShaderLocation(SkyboxShader, "environmentMap"), (int)MaterialMapIndex.Cubemap, ShaderUniformDataType.Int);
            SetShaderValue(SkyboxShader, GetShaderLocation(SkyboxShader, "doGamma"), 1, ShaderUniformDataType.Int);
            SetShaderValue(SkyboxShader, GetShaderLocation(SkyboxShader, "vflipped"), 1, ShaderUniformDataType.Int);

            // Load cubemap shader
            CubemapShader = LoadShader("assets/shaders/cubemap.vs", "assets/shaders/cubemap.fs");
            SetShaderValue(CubemapShader, GetShaderLocation(CubemapShader, "equirectangularMap"), 0, ShaderUniformDataType.Int);

            // Load lighting shader
            LightingShader = LoadShader("assets/shaders/lighting.vs", "assets/shaders/lighting.fs");
            LightingShader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(LightingShader, "viewPos");
            int lightColorLoc = GetShaderLocation(LightingShader, "lightColor");
            _viewPosLoc = GetShaderLocation(LightingShader, "viewPos");
            SetShaderValue(LightingShader, lightColorLoc, Conceptor3D.SUN_COLOR, ShaderUniformDataType.Vec4);

            // Load sun shader and relative layout locations
            SunShader = LoadShader(null, "assets/shaders/flares.fs");
            _shinePosLoc = GetShaderLocation(SunShader, "sourcePos");
            _resolutionLoc = GetShaderLocation(SunShader, "iResolution");
            _timeLoc = GetShaderLocation(SunShader, "time");
            _camDistLoc = GetShaderLocation(SunShader, "camDist");
            SetShaderValue(SunShader, GetShaderLocation(SunShader, "sunCol"), Conceptor3D.SUN_COLOR, ShaderUniformDataType.Vec4);
            _occlusionLoc = GetShaderLocation(SunShader, "oTexture0");
        }

        /// <summary>Closes the shader center by unloading every program shader from the vRAM.</summary>
        public static void Close()
        {
            // Unload shader programs from vRAM
            UnloadShader(CubemapShader);
            UnloadShader(SkyboxShader);
        }

        /// <summary>Updates shine texture sampler2D (EndShaderMode() forces batch drawing and consequently resets active textures)</summary>
        public static void UpdateShine(Camera3D camera, float camDist)
        {
            // Calculate 2D position of the camera
            Vector2 sunPos = GetWorldToScreen(Vector3.Zero, camera);
            // Set values
            SetShaderValue(SunShader, _shinePosLoc, sunPos, ShaderUniformDataType.Vec2);
            SetShaderValue(SunShader, _timeLoc, GetTime(), ShaderUniformDataType.Float); // Update time
            SetShaderValue(SunShader, _camDistLoc, camDist, ShaderUniformDataType.Float); // Update camera distance to sun
            SetShaderValueTexture(SunShader, _occlusionLoc, OcclusionMap.Texture);
            SetShaderValue(LightingShader, _viewPosLoc, camera.Position, ShaderUniformDataType.Vec3);
        }

        public static void SetResolution(int width, int height)
        {
            SetShaderValue(SunShader, _resolutionLoc, new Vector2(width, height), ShaderUniformDataType.Vec2);
            LoadOcclusionMap(width, height);
        }

        public static void LoadOcclusionMap(int width, int height)
        {
            OcclusionMap = LoadRenderTexture(width, height);
            OcclusionMapSource = new Rectangle(Vector2.Zero, width, -height);
            OcclusionMapDestination = new Rectangle(Vector2.Zero, width, height);
        }
    }
}