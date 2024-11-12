using Raylib_cs;
using static Raylib_cs.Raylib;
using Raylib_cs.Complements;
using static Raylib_cs.Complements.Raylib;
using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="Conceptor3D"/>.</summary>
    public static class Conceptor3D
    {
        // -----------------------------------------------------------
        // Private instances
        // -----------------------------------------------------------

        private static Camera3D _camera;
        private static Skybox _skybox;

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
            _skybox = LoadSkybox("assets/shaders/skyboxes/HDR_blue_nebulae-1.hdr");
        }

        /// <summary>Draws the 3D environnement of the application.</summary>
        public static void Draw()
        {
            BeginMode3D(_camera);

            DrawSkybox(_skybox);

            EndMode3D();
        }
    }
}