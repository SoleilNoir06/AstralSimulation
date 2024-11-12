using System.Numerics;
using static Raylib_cs.Complements.Raylib;
using static Raylib_cs.Raylib;
using Astral_simulation;

namespace Raylib_cs.Complements
{
    /// <summary>Represents the extended version of <see cref="Raylib_cs.Raylib"/>.</summary>
    public unsafe partial class Raylib
    {
        /// <summary>Loads a <see cref="Skybox"/> object.</summary>
        /// <param name="path">Path to the skybox file.</param>
        public static Skybox LoadSkybox(string path)
        {
            Skybox skybox = new Skybox();
            Texture2D panorama;
            Texture2D cubemap;
            switch (path.Split('.').Last())
            {
                case "hdr": // Work on HDR files
                    panorama = LoadTexture(path); // Load HDR texture
                    cubemap = GenTextureCubemap(panorama, 1024, PixelFormat.UncompressedR8G8B8A8); // Load cubemap texture
                    SetMaterialTexture(ref skybox.Material, MaterialMapIndex.Cubemap, cubemap); // Set cubemap texture to skybox
                    UnloadTexture(panorama); // Unload unused texture
                    skybox.Material.Shader = ShaderCenter.SkyboxShader;
                    return skybox;
            }
            return new Skybox(); // Return empty object
        }

        /// <summary>Draws a <see cref="Skybox"/> object. (To call before any other draw call).</summary>
        /// <param name="skybox">Skybox to draw.</param>
        public static void DrawSkybox(Skybox skybox)
        {
            Rlgl.DisableBackfaceCulling();
            Rlgl.DisableDepthMask();
            DrawMesh(Skybox.Mesh, skybox.Material, Raymath.MatrixIdentity());
            Rlgl.EnableBackfaceCulling();
            Rlgl.EnableDepthMask();
        }

        /// <summary>Loads a cubemap texture.</summary>
        /// <param name="panorama">Panorama 2D texture to use.</param>
        /// <param name="size">Size of the texture.</param>
        /// <param name="format">Pixel formal of the texture.</param>
        /// <returns></returns>
        private static Texture2D GenTextureCubemap(Texture2D panorama, int size, PixelFormat format)
        {
            Texture2D cubemap;

            // Disable Backface culling to render inside the cube
            Rlgl.DisableBackfaceCulling();

            // Setup frame buffer
            uint rbo = Rlgl.LoadTextureDepth(size, size, true);
            cubemap.Id = Rlgl.LoadTextureCubemap(null, size, format);

            uint fbo = Rlgl.LoadFramebuffer(size, size);
            Rlgl.FramebufferAttach(fbo, rbo, FramebufferAttachType.Depth, FramebufferAttachTextureType.Renderbuffer, 0);
            Rlgl.FramebufferAttach(fbo, cubemap.Id, FramebufferAttachType.ColorChannel0, FramebufferAttachTextureType.CubemapPositiveX, 0);

            // Check if framebuffer is valid

            if (Rlgl.FramebufferComplete(fbo))
            {
                Console.WriteLine($"FBO: [ID {fbo}] Framebuffer object created successfully");
            }

            // Draw to framebuffer
            Rlgl.EnableShader(ShaderCenter.CubemapShader.Id);

            // Define projection matrix and send it to the shader
            Matrix4x4 matFboProjection = Raymath.MatrixPerspective(90.0f * DEG2RAD, 1.0f, Rlgl.CULL_DISTANCE_NEAR, Rlgl.CULL_DISTANCE_FAR);
            Rlgl.SetUniformMatrix(ShaderCenter.CubemapShader.Locs[(int)ShaderLocationIndex.MatrixProjection], matFboProjection);

            // Define view matrix for every side of the cube
            Matrix4x4[] fboViews =
            {
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3(-1.0f,  0.0f,  0.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 1.0f,  0.0f,  0.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  1.0f,  0.0f), new Vector3( 0.0f,  0.0f, -1.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f, -1.0f,  0.0f), new Vector3( 0.0f,  0.0f, 1.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  0.0f, -1.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  0.0f,  1.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
            };

            // Set viewport to current fbo dimensions
            Rlgl.Viewport(0, 0, size, size);

            // Activate and enable texture for drawing to cubemap faces
            Rlgl.ActiveTextureSlot(0);
            Rlgl.EnableTexture(panorama.Id);

            for (int i = 0; i < 6; i++)
            {
                // Set the view matrix for current face
                Rlgl.SetUniformMatrix(ShaderCenter.CubemapShader.Locs[(int)ShaderLocationIndex.MatrixView], fboViews[i]);

                // Select the current cubemap face attachment for the fbo
                Rlgl.FramebufferAttach(fbo, cubemap.Id, FramebufferAttachType.ColorChannel0, FramebufferAttachTextureType.CubemapPositiveX + i, 0);
                Rlgl.EnableFramebuffer(fbo);

                Rlgl.ClearScreenBuffers();
                Rlgl.LoadDrawCube();
            }

            // Unload framebuffer and reset state
            Rlgl.DisableShader();
            Rlgl.DisableTexture();
            Rlgl.DisableFramebuffer();

            Rlgl.UnloadFramebuffer(fbo);

            Rlgl.Viewport(0, 0, Rlgl.GetFramebufferWidth(), Rlgl.GetFramebufferHeight());
            Rlgl.EnableBackfaceCulling();

            cubemap.Width = size;
            cubemap.Height = size;
            cubemap.Mipmaps = 1;
            cubemap.Format = format;

            return cubemap;
        }
    }
}