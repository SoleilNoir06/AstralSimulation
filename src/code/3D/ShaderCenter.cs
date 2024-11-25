using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="ShaderCenter"/>.</summary>
    public static unsafe class ShaderCenter
    {
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
            SunShader = LoadShader(null, "assets/shaders/flares.fs");
            int shineTexLoc = GetShaderLocation(SunShader, "shineTexture");
            Texture2D shineTexture = LoadTexture("assets/shaders/textures/shine.png");
            SetShaderValueTexture(SunShader, shineTexLoc, shineTexture);

            //// Load frame buffer
            //uint frameBuffer = Rlgl.LoadFramebuffer(shineTexture.Width, shineTexture.Height); // Load texture
            //Rlgl.EnableFramebuffer(frameBuffer); // Enable frame buffer
            //uint texId = Rlgl.LoadTexture(null, shineTexture.Width, shineTexture.Height, PixelFormat.UncompressedR8G8B8A8, 1); // Load texture

            //Rlgl.ActiveDrawBuffers(1);
            //Rlgl.FramebufferAttach(frameBuffer, texId, FramebufferAttachType.ColorChannel0, FramebufferAttachTextureType.Texture2D, 0);
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