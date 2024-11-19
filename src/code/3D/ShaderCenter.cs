using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="ShaderCenter"/>.</summary>
    public static class ShaderCenter
    {
        private static int sunPosLoc;

        /// <summary>Cubemap loading shader.</summary>
        public static Shader CubemapShader;

        /// <summary>Skybox rendering shader.</summary>
        public static Shader SkyboxShader;

        /// <summary>Default lighting shader.</summary>
        public static Shader LightingShader;

        /// <summary>Sun light shader.</summary>
        public static Shader SunShader;

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

            // Load sun shader
            SunShader = LoadShader(null, "assets/shaders/sun.fs");
            sunPosLoc = GetShaderLocation(SunShader, "lightPosition"); // Get sun position loc
            int sunColorLoc = GetShaderLocation(SunShader, "lightColor"); // Get color loc
            int sunLightIntensity = GetShaderLocation(SunShader, "intensity"); // Get intensity loc
            // Set values
            SetShaderValue(SunShader, sunColorLoc, new Vector3(1), ShaderUniformDataType.Vec3);
            SetShaderValue(SunShader, sunLightIntensity, 0.5f, ShaderUniformDataType.Float);
        }

        /// <summary>Closes the shader center by unloading every program shader from the vRAM.</summary>
        public static void Close()
        {
            // Unload shader programs from vRAM
            UnloadShader(CubemapShader);
            UnloadShader(SkyboxShader);
        }

        /// <summary>Updates screen resolution to shaders.</summary>
        /// <param name="width">Screen width</param>
        /// <param name="height">Screen height.</param>
        public static void UpdateResolution(int width, int height)
        {
            int resLoc = GetShaderLocation(SunShader, "resolution");
            SetShaderValue(SunShader, resLoc, new Vector2(width, height), ShaderUniformDataType.Vec2);
        }

        /// <summary>Updates sun position to shader.</summary>
        /// <param name="position">Sun position.</param>
        public static void UpdateSun(Vector3 position, float radius)
        {
            SetShaderValue(SunShader, sunPosLoc, GetWorldToScreen(position, Conceptor3D.Camera), ShaderUniformDataType.Vec2);
        }
    }
}