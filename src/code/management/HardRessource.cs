using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Astral_Simulation
{
    internal static class HardRessource
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Model> Models = new Dictionary<string, Model>();
        public static Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        public static void Init()
        {
            // Load static textures
            Textures = new Dictionary<string, Texture2D>() 
            {
                {"Mercury", LoadTexture("assets/images/Mercury.png") },
                {"Venus", LoadTexture("assets/images/Venus.png") },
                {"Earth", LoadTexture("assets/images/Earth.png") },
                {"Mars", LoadTexture("assets/images/Mars.png") },
                {"Jupiter", LoadTexture("assets/images/Jupiter.png") },
                {"Saturn", LoadTexture("assets/images/Saturn.png") },
                {"Uranus", LoadTexture("assets/images/Uranus.png") },
                {"Neptune", LoadTexture("assets/images/Neptune.png") },
                {"Pluto", LoadTexture("assets/images/Pluto.png") }
            };
            // Load static models
            Models = new Dictionary<string, Model>()
            {
                { "camera", LoadModel("data/camera.m3d")}
            };
            // Load static materials
            Materials = new Dictionary<string, Material>();
            // Camera material
            Material camMat = LoadMaterialDefault();
            SetMaterialTexture(ref camMat, MaterialMapIndex.Albedo, LoadTexture("data/cameraTex.png"));
            Materials.Add("camera", camMat);
        }
    }
}