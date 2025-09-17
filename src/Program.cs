using Raylib_cs;
using static RayGUI_cs.RayGUI;
using static Raylib_cs.Raylib;
using Astral_simulation.DatFiles;
using Astral_Simulation;
using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of the running program.</summary>
    public class Program
    {
        /// <summary>Enters the program's entrypoint.</summary>
        /// <param name="args">Arguments passed from outside the program.</param>
        public static void Main(string[] args)
        {
            InitWindow(0, 0, "Astra Simulation");
            InitAudioDevice();
            SetWindowState(ConfigFlags.UndecoratedWindow);

            // Fullscreen window
            SetWindowState(ConfigFlags.ResizableWindow);
            SetWindowState(ConfigFlags.MaximizedWindow);

            // Draw Splash
            DrawSplash();

            // DONT CHANGE ORDER
            RLoading.Init(); // Inits the RLoading instance (loads crypto keys)
            ShaderCenter.Init(); // Load shader programs
            Conceptor3D.Init(); // Inits the 3D environnment
            AudioCenter.Init();
            HardRessource.Init();
            Conceptor2D.Init(); // Load GUI basics

#if DEBUG
            Conceptor3D.System = new System();
#endif
            ShaderCenter.SetResolution(GetScreenWidth(), GetScreenHeight()); // Set new resolution

            Conceptor3D.ToggleConceptorMode();

            Conceptor3D.PrepareCamera();

            SetExitKey(KeyboardKey.Null);
            SetTargetFPS(120);

            while (!WindowShouldClose()) // Main game loop
            {
                AudioCenter.Update();

                BeginDrawing();

                ClearBackground(Color.Black);

                Conceptor3D.Draw(); // Draw 3D environnment

                Conceptor2D.Draw(); // Draw 2D interface 

                DrawFPS(0, 0);

                EndDrawing();
            }

            CloseWindow();

            DatEncoder.EncodeSystem(Conceptor3D.System);

            // Unloading
            ShaderCenter.Close();
            Conceptor3D.Close();
        }

        static void DrawSplash()
        {
            BeginDrawing();
            ClearBackground(Color.Black);
            EndDrawing();
        }
    }
}