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
            Components.Add(new Button(obj.Name.ToString(), 0, 0, 100, 100));
            Components.Add(new Button($"Radius : {obj.Radius}", 0, 200, 500, 100));
            Components.Add(new Button($"Revolution : {obj.Revolution}", 0, 300, 700, 100));
            Components.Add(new Button($"Position : {obj.Position}", 0, 400, 700, 100));
        }
    }
}