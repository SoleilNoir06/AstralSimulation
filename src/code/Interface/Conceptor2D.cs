using RayGUI_cs;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="Conceptor2D"/>.</summary>
    public static class Conceptor2D
    {
        // Constants
        const int DEFAULT_FONT_SIZE = 30;
        const int TARGET_PERIMETER_RADIUS = 10;

        // Private attributes
        private static Font _topLayerFont;

        // Color A : rgba(75, 79, 87, 255)
        // Color B : rgba(31, 33, 36, 255)
        public static GuiContainer Components = new GuiContainer();

        public static void Init(){

            // Font loading
            _topLayerFont = LoadFont("assets/fonts/Poppins/Poppins-Medium.ttf");
            
            Dictionary<int, Font> fonts = new Dictionary<int, Font>()
            {
              {14, _topLayerFont}  
            };
            LoadGUI(fonts);

            // Create template GUI container
            Components = new GuiContainer(
                new Color(75, 79, 87, 255), 
                new Color(31, 33, 36, 255)
            );

            Components.SetDefaultFontSize(DEFAULT_FONT_SIZE);
        }

        /// <summary>Draws the 2D user interface.</summary>
        public static void Draw()
        {
            // Draw active components from GUI container
            Components.Draw();
        }

        public static void DisplayObject(AstralObject obj)
        {
            Components.Clear();

            // Container c = new Container(10, 10, 520, GetScreenHeight() - 20);
            // c.BaseColor = new Color(22, 22, 22, 20);
            // Components.Add(c);

            Components.Add("name", new Label(20, 20, 500, 50, $"{obj.Name}"));
            Components.Add("position", new Label(20, 70, 500, 50, $"Position: {obj.Position}"));
            Components.Add("mass", new Label(20, 120, 500, 50, $"Mass: {obj.Mass}e24 kg"));
            Components.Add("radius", new Label(20, 170, 500, 50, $"Radius: {obj.Radius * 150000f}km"));
            Components.Add("volume", new Label(20, 220, 500, 50, $"Volume: {obj.Volume}km^3"));
            //Components.Add(new Label(20, 270, 500, 50, $"Distance from sun : {Physics.ComputeRadialDistance(obj) * 15000000}km"));

            // Panel p = new Panel(20, 320, 0, 1, HardRessource.Textures[$"{obj.Name}"]);
            // p.MaxHeight = 270;
            // p.MaxWidth = 480;
            // Components.Add(p);
        }

        /// <summary>Displays the top player of the application's UI.</summary>
        public static void DisplayUITopLayer()
        {
            // GUI-Layer system rendering
            Conceptor3D.System.ForEach(obj =>
            {
                Vector2? space = ValidateWorldToScreen(obj.Position, Conceptor3D.Camera);
                if (space is not null)
                {
                    // Apply double layering on planet circles
                    DrawCircleLinesV(space.Value, TARGET_PERIMETER_RADIUS, obj.AttributeColor);
                    DrawCircleLinesV(space.Value, TARGET_PERIMETER_RADIUS - 1, obj.AttributeColor);

                    // Display object name if near enough
                    Vector2 textPos = new Vector2( (int)space.Value.X + TARGET_PERIMETER_RADIUS*2, (int)space.Value.Y - TARGET_PERIMETER_RADIUS*2);
                    DrawTextEx(_topLayerFont, obj.Name, textPos, 20, 3f, Color.RayWhite);
                }
            });
        }

        /// <summary>Gets the screen position of a 3D position, according to the camera direction.</summary>
        /// <param name="position">3D position to convert.</param>
        /// <param name="camera">3D camera to use.</param>
        /// <returns>2D position if it exists, null otherwise.</returns>
        private static Vector2? ValidateWorldToScreen(Vector3 position, Camera3D camera)
        {
            float dot = Raymath.Vector3DotProduct(position - camera.Position, Raymath.Vector3Normalize(camera.Target - camera.Position));
            if (dot > 0) return GetWorldToScreen(position, camera);
            else return null;
        }
    }
}