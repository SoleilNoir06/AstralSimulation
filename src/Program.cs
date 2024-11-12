using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace Astral_simulation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitWindow(0, 0, "Astra Simulation");
            SetWindowState(ConfigFlags.ResizableWindow);
            SetWindowState(ConfigFlags.MaximizedWindow);

            while (!WindowShouldClose())
            {
                BeginDrawing();

                ClearBackground(Color.White);

                EndDrawing();
            }

            CloseWindow();

            // Unloading
        }
    }
}