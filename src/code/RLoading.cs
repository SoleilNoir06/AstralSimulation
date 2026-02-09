using Raylib_cs;
using Newtonsoft.Json;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="RLoading"/>.</summary>
    internal static class RLoading
    {
        /// <summary>Loads the default solar system.</summary>
        /// <returns>The list of astral objects of the default solar system.</returns>
        public static List<AstralObject> LoadDefaultSystem(string path)
        {

            if (!File.Exists(path)) throw new FileNotFoundException($"JSON file cannot be found at : {path}");

            string jsonStream = File.ReadAllText(path);

            List<AstralObject>? objects = JsonConvert.DeserializeObject<List<AstralObject>>(jsonStream);

            return objects ?? new List<AstralObject>();
        }

        public static Dictionary<string, Music> LoadMusics()
        {
            Dictionary<string, Music> list = new Dictionary<string, Music>()
            {
                {"ambient", Raylib.LoadMusicStream("assets/audio/ambient.mp3")},
            };
            return list;
        }
        public static Dictionary<string, Sound> LoadSounds()
        {
            Dictionary<string, Sound> list = new Dictionary<string, Sound>()
            {
                {"ambient", Raylib.LoadSound("assets/audio/ambient.mp3")},
            };
            return list;
        }
    }
}