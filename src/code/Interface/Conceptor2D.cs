using RayGUI_cs;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Astral_Simulation;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="Conceptor2D"/>.</summary>
    public static class Conceptor2D
    {
        public static List<Component> Components = new List<Component>();

        /// <summary>Draws the 2D user interface.</summary>
        public static void Draw()
        {
            if (Components.Count != 0) // Check if UI exists
            {
                bool focus = false;
                DrawGUIList(Components, ref focus);
                if (!focus) SetMouseCursor(MouseCursor.Default);
            }
        }

        public static void DisplayObject(AstralObject obj)
        {
            SetDefaultFontSize(30);
            Components.Clear();
            Container c = new Container(10, 10, 520, GetScreenHeight() - 20);
            c.BaseColor = new Color(22, 22, 22, 20);
            Components.Add(c);
            Components.Add(new Label(20, 20, 500, 50, $"{obj.Name}"));
            Components.Add(new Label(20, 70, 500, 50, $"Type: {obj.Type}"));
            Components.Add(new Label(20, 120, 500, 50, $"Mass: {obj.Mass}e24 kg"));
            Components.Add(new Label(20, 170, 500, 50, $"Radius: {obj.Radius * 150000f}km"));
            Components.Add(new Label(20, 220, 500, 50, $"Volume: {obj.Volume}km^3"));
            Components.Add(new Label(20, 270, 500, 50, $"Distance from sun : {Physics.ComputeRadialDistance(obj.Position) * 15000000}km"));

            /* Panel p = new Panel(20, 300, 0, LoadTexture("assets/textures/splash.png"));
            p.MaxHeight = 200;
            p.MaxWidth = 200;
            Components.Add(p); */
        }
    }
}