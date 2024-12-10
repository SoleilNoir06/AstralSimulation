using Raylib_cs;
using RayGUI_cs;
using static Raylib_cs.Raylib;
using Astral_simulation.DatFiles;

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

            // DONT CHANGE ORDER
            RLoading.Init(); // Inits the RLoading instance (loads crypto keys)
            ShaderCenter.Init(); // Load shader programs
            Conceptor3D.Init(); // Inits the 3D environnment
            AudioCenter.Init();
            RayGUI.InitGUI(new Color(75, 79, 87, 255), new Color(31, 33, 36, 255), LoadFont("assets/fonts/Poppins/Poppins-Medium.ttf"));

#if DEBUG
            Conceptor3D.System = new System();
#endif
            ShaderCenter.SetResolution(GetScreenWidth(), GetScreenHeight()); // Set new resolution

            Conceptor3D.ToggleConceptorMode();

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
    }
}