using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Astral_simulation
{
    /// <summary>Represents an instance of the running program.</summary>
    public class Program
    {
        /// <summary>Enters the program's entrypoint.</summary>
        /// <param name="args">Arguments passed from outside the program.</param>
        public static void Main(string[] args)
        {
            InitWindow(500, 300, "Astra Simulation");
            SetWindowState(ConfigFlags.UndecoratedWindow);

            ShaderCenter.Init(); // Load shader programs
            Conceptor3D.Init(); // Inits the 3D environnment

#if DEBUG
            Conceptor3D.AstralObjects.Add(new Telluric(500, 500, 500, 500, 500, LoadTexture("assets/textures/jupiter.png")));
#endif
            // Fullscreen window
            SetWindowState(ConfigFlags.ResizableWindow);
            SetWindowState(ConfigFlags.MaximizedWindow);

            SetTargetFPS(120);
            while (!WindowShouldClose()) // Main game loop
            {
                BeginDrawing();

                ClearBackground(Color.White);

                Conceptor3D.Draw(); // Draws 3D environnment

                EndDrawing();
            }

            CloseWindow();

            // Unloading
            ShaderCenter.Close();
            Conceptor3D.Close();
        }
    }
}