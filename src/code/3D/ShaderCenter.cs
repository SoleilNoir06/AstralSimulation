using static Raylib_cs.Raylib;
using Raylib_cs;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="ShaderCenter"/>.</summary>
    public static class ShaderCenter
    {
        /// <summary>Cubemap loading shader.</summary>
        public static Shader CubemapShader;

        /// <summary>Skybox rendering shader.</summary>
        public static Shader SkyboxShader;

        /// <summary>Default lighting shader.</summary>
        public static Shader LightingShader;

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
        }

        /// <summary>Closes the shader center by unloading every program shader from the vRAM.</summary>
        public static void Close()
        {
            // Unload shader programs from vRAM
            UnloadShader(CubemapShader);
            UnloadShader(SkyboxShader);
        }
    }
}