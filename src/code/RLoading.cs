using Astral_simulation.DatFiles;
using Newtonsoft.Json;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="RLoading"/>.</summary>
    internal static class RLoading
    {
        public static byte[] EncryptionKey = new byte[32];
        public static byte[] SymmetricalVector = new byte[16];

        /// <summary>Loads the default solar system.</summary>
        /// <returns>The list of astral objects of the default solar system.</returns>
        public static List<AstralObject> LoadDefaultSystem()
        {
            List<AstralObject> objs = DatEncoder.DecodeSystem("assets/json/Solar.DAT", EncryptionKey, SymmetricalVector); // Load default solar system from .DAT
            return objs;
        }

        /// <summary>Inits the RLoading class.</summary>
        public static void Init()
        {
            // Define encryption key
            for (int i = 0; i < EncryptionKey.Length; i++)
            {
                EncryptionKey[i] = 2; // Default temp value
            }

            // Define symmetrical vector
            for (int i = 0; i < SymmetricalVector.Length - 1; i++)
            {
                SymmetricalVector[i] = 2; // Default temp value
            }
        }
    }
}