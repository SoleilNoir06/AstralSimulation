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
        // Constants
        const int DEFAULT_FONT_SIZE = 30;

        // Color A : rgba(75, 79, 87, 255)
        // Color B : rgba(31, 33, 36, 255)
        public static GuiContainer Components = new GuiContainer();

        public static void Init(){
            // Color A : rgba(75, 79, 87, 255)
            // Color B : rgba(31, 33, 36, 255)
            // Font : assets/fonts/Poppins/Poppins-Medium.ttf
            Font font = LoadFont("assets/fonts/Poppins/Poppins-Medium.ttf");
            Dictionary<int, Font> fonts = new Dictionary<int, Font>();
            fonts.Add(14, font);
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
    }
}