using RayGUI_cs;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using static Raylib_cs.Raylib;

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
            Components.Clear();
            Container c = new Container(10, 10, 500, GetScreenHeight() - 20);
            c.BaseColor = new Color(22, 22, 22, 20);
            Components.Add(c);
            Components.Add(new Textbox(10, 10, 500, 50, $"{obj.Name}"));
            Components.Add(new Textbox(10, 60, 500, 50, $"Type: {obj.Type}"));
            Components.Add(new Textbox(10, 110, 500, 50, $"Mass: {obj.Mass}e24 kg"));
            Components.Add(new Textbox(10, 160, 500, 50, $"Radius: {obj.Radius * 150000f}km"));
            Components.Add(new Textbox(10, 210, 500, 50, $"Volume: {obj.Volume}km^3"));
            Components.Add(new Textbox(10, 260, 500, 50, $"Velocity: {obj.InitialVelocity}km/h"));
        }
    }
}