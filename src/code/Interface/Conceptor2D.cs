using RayGUI_cs;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using static Raylib_cs.Raylib;
using astral_simulation;

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
            Components.Add(new Textbox(10, 10, 500, 50, $"{obj.Name}"));
            Components.Add(new Textbox(10, 60, 500, 50, $"Mass : {obj.Mass}e24 kg"));
            Components.Add(new Textbox(10, 110, 500, 50, $"Radius : {obj.Radius * 150000} km"));
        }
    }
}