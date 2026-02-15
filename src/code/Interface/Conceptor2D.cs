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
        public const int DEFAULT_FONT_SIZE = 30;
        public const float COLOR_BRIGTHNESS_OVERLAY = 0.35f;
        const float TARGET_PERIMETER_RADIUS = 10;
        const float ACTIVE_TARGET_PERIMTER_RADIUS = 15;
        const float SMOOTH_FACTOR = 3.5f;

        // Colors definition
        public static Color PASSIVE_TEXT_COLOR = new Color(191, 191, 191);
        public static Color ACTIVE_TEXT_COLOR = Color.White;

        // Private attributes
        private static Font _topLayerFont;
        private static AstralObject? _lastActiveObject;
        private static float _targetCircleOverlayRadius = TARGET_PERIMETER_RADIUS;

        // Color A : rgba(75, 79, 87, 255)
        // Color B : rgba(31, 33, 36, 255)
        public static GuiContainer Components = new GuiContainer();

        public static void Init(){

            // Font loading
            _topLayerFont = LoadFontEx("assets/fonts/Poppins/Poppins-SemiBold.ttf", 25, null, 0);
            
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
            bool activity = false;
            Conceptor3D.System.ForEach(obj =>
            {
                Vector2? space = ValidateWorldToScreen(obj.Position, Conceptor3D.Camera);
                if (space is not null)
                {
                    // Display object name if near enough
                    Vector2 textPos = new Vector2( (int)space.Value.X + TARGET_PERIMETER_RADIUS*2, (int)space.Value.Y - TARGET_PERIMETER_RADIUS*2);
                    Vector2 txtSize = MeasureTextEx(_topLayerFont, obj.Name, 25, 4f);

                    // Define colors to use (taking account of selected objects and distance)
                    Color attributeColor = obj.AttributeColor;
                    Color passiveTextColor = PASSIVE_TEXT_COLOR;
                    Color activeTextColor = ACTIVE_TEXT_COLOR;

                    if (Conceptor3D.CameraParams.Target == obj)
                    {
                        float dist = Raymath.Vector3Subtract(Conceptor3D.CameraParams.ApprochedTarget, Conceptor3D.Camera.Position).Length() / obj.Radius;
                        attributeColor = ColorAlpha(obj.AttributeColor, Raymath.Normalize(dist, 150, 300)); // }
                        passiveTextColor = ColorAlpha(passiveTextColor, Raymath.Normalize(dist, 150, 300)); // <- Don't question theses values, found em while debugging
                        activeTextColor = ColorAlpha(activeTextColor, Raymath.Normalize(dist, 150, 300)); //   }
                    }

                    // Define text state to display
                    if (Hover((int)textPos.X, (int)textPos.Y, (int)txtSize.X, (int)txtSize.Y))
                    {
                        // Draw appropriate text
                        DrawTextEx(_topLayerFont, obj.Name, textPos, 25, 4f, activeTextColor);
                        
                        // Apply double layering on planet circles
                        _targetCircleOverlayRadius = Raymath.Lerp(_targetCircleOverlayRadius, ACTIVE_TARGET_PERIMTER_RADIUS, (float)GetFrameTime()*SMOOTH_FACTOR);
                        DrawCircleLinesV(space.Value, _targetCircleOverlayRadius, ColorBrightness(attributeColor, COLOR_BRIGTHNESS_OVERLAY));
                        DrawCircleLinesV(space.Value, _targetCircleOverlayRadius - 1, ColorBrightness(attributeColor, COLOR_BRIGTHNESS_OVERLAY));
                        
                        obj.UIActive = true;
                        activity = true;
                        _lastActiveObject = obj;
                    }
                    else
                    {
                        // Draw appropriate text
                        DrawTextEx(_topLayerFont, obj.Name, textPos, 25, 4f, passiveTextColor);
                        
                        // Apply double layering on planet circles
                        float radius = _lastActiveObject == obj ? _targetCircleOverlayRadius : TARGET_PERIMETER_RADIUS;
              
                        DrawCircleLinesV(space.Value, radius, attributeColor);
                        DrawCircleLinesV(space.Value, radius - 1, attributeColor);

                        obj.UIActive = false;
                    }
                }
            });

            // Move back the active circle interpolator if no activity is detected
            if (!activity) _targetCircleOverlayRadius = Raymath.Lerp(_targetCircleOverlayRadius, TARGET_PERIMETER_RADIUS, GetFrameTime()*SMOOTH_FACTOR);
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