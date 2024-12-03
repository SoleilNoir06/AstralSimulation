using Astral_simulation.DatFiles;
using Raylib_cs;
using Newtonsoft.Json;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="RLoading"/>.</summary>
    internal static class RLoading
    {
        public static byte[] EncryptionKey = new byte[DatEncoder.AES_KEY_LENGTH];
        public static byte[] SymmetricalVector = new byte[DatEncoder.AES_IV_LENGTH];

        /// <summary>Loads the default solar system.</summary>
        /// <returns>The list of astral objects of the default solar system.</returns>
        public static List<AstralObject> LoadDefaultSystem()
        {
            // Key loading
            /*FileStream stream = new FileStream("external/crypto.env", FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(EncryptionKey); // 32-Bytes
            writer.Write(SymmetricalVector); // 16-Bytes 
            
            StreamReader stream = new StreamReader("assets/json/solarSystem.json");
            string data = stream.ReadToEnd();
            List<AstralObject>? objects = JsonConvert.DeserializeObject<List<AstralObject>>(data);

            DatEncoder.EncodeSystem(new System(objects, "Solar")); 
           */
            List<AstralObject> objs = DatEncoder.DecodeSystem("assets/json/Solar.DAT", EncryptionKey, SymmetricalVector); // Load default solar system from .DAT
            // Fix objects value
            objs.ForEach(obj =>
            {
                obj.Pitch += 90f;
                obj.Radius *= 20;
                obj.Position *= 20;
            });

            return objs;
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

        /// <summary>Inits the RLoading class.</summary>
        public static void Init()
        {
            // Read keys
            FileStream stream = new FileStream("external/crypto.env", FileMode.Open);
            using BinaryReader reader = new BinaryReader(stream);
            EncryptionKey = reader.ReadBytes(DatEncoder.AES_KEY_LENGTH);
            SymmetricalVector = reader.ReadBytes(DatEncoder.AES_IV_LENGTH);
        }
    }
}