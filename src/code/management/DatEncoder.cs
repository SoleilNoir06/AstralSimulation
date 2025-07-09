using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Astral_simulation.DatFiles
{
    internal class DatEncoder
    {
        // -----------------------------------------------------------
        // Private and constant instances
        // -----------------------------------------------------------

        internal const int AES_KEY_LENGTH = 32;
        internal const int AES_IV_LENGTH = 16;

        private const string MODELS_SECTION = "MODEL";
        private const string CAMERAS_SECTION = "CAMERA";

        private static int _offset = 0;
        private static List<DatFileEntry> _entries = new List<DatFileEntry>();

        /// <summary>Encodes a scene's information and stores it into a .DAT file.</summary>
        /// <param name="scene">Scene to read data from.</param>
        public static void EncodeSystem(System system)
        {
            // Transform information to JSON object
            string[] jsons = JsonfyGos(system);
            // Open file stream
            FileStream stream = new FileStream($"{system.Name}.DAT", FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);
            // Write placeholder data to the beginning of the file
            writer.Write(system.Count); // Object count placeholder (Int-32)
            writer.Write(_offset); // Table placeholder (Int-32)
            // Move writer after placeholder
            _offset += 8; // Int-32 + Int-32 = 8 bytes
            // Encode and write JSON data       
            for (int i = 0; i < jsons.Length; i++)
            {
                // Encrypt string to byte array
                byte[] data = Encrypt(jsons[i], RLoading.EncryptionKey, RLoading.SymmetricalVector);
                // Create DAT file entry
                DatFileEntry entry = new DatFileEntry(system.Name, _offset, data.Length);
                _entries.Add(entry);
                // Write data to file
                writer.Write(data);
                _offset += data.Length;
            }
            // Write entries table
            int tableOffset = _offset;
            foreach (DatFileEntry entry in _entries)
            {
                writer.Write(entry.Name);
                writer.Write(entry.Index);
                writer.Write(entry.Size);
            }
            // Go back to the beginning of the file to write important data
            stream.Seek(0, SeekOrigin.Begin);
            writer.Write(_entries.Count);
            writer.Write(tableOffset);
            /* Which gives us a similar file structure : 
                * 
                * Entries count
                * Table offset
                * Files data at specific location
                * Entries informations (Name, size, index), aka information table
                * */

            // Reset internal data
            _offset = 0;
            _entries.Clear();
        }

        /// <summary>Reads a .DAT file and decodes the scene informations from it.</summary>
        /// <param name="path">Path to the .DAT file.</param>
        /// <returns>Uniray corresponding Scene.</returns>
        /// <exception cref="Exception">No file found exception.</exception>
        public static List<AstralObject> DecodeSystem(string path, byte[] key, byte[] iv)
        {
            if (!Path.Exists(path)) throw new Exception("No .DAT file was found at the given location");
            // Open file stream
            FileStream datFile = new FileStream(path, FileMode.Open);
            using BinaryReader reader = new BinaryReader(datFile);
            // Read file header data
            int _entryCount = reader.ReadInt32(); // Read object count
            int _tableOffset = reader.ReadInt32(); // Read entry table offset
            // Move to table offset
            datFile.Seek(_tableOffset, SeekOrigin.Begin);
            // Loop over different file entries
            for (int i = 0; i < _entryCount; i++)
            {
                // Read entry data
                string entryName = reader.ReadString();
                int index = reader.ReadInt32();
                int size = reader.ReadInt32();
                _entries.Add(new DatFileEntry(entryName, index, size));
            }
            List<AstralObject> objects = new List<AstralObject>();
            // Read entries data
            foreach (DatFileEntry entry in _entries)
            {
                // Move to entry index
                datFile.Seek(entry.Index, SeekOrigin.Begin);
                // Read encrypted data from the file
                byte[] encryptedData = new byte[entry.Size];
                datFile.Read(encryptedData, 0, encryptedData.Length);
                // Decrypt data
                string text = Decrypt(encryptedData, key, iv);
                List<AstralObject>? _system = JsonConvert.DeserializeObject<List<AstralObject>>(text); // Create object list
                if (_system is not null) objects.AddRange(_system); // Create system
            }
            // Reset entries list
            _entries.Clear();
            // Create scene
            return objects; // Return first system
        }

        /// <summary>Encrypts data with the AES algorithm.</summary>
        /// <param name="data">Data to encrypt.</param>
        /// <param name="key">Encryption key.</param>
        /// <param name="iv">Encryption IV.</param>
        /// <returns>Encrypted data.</returns>
        /// <exception cref="ArgumentNullException">Null data exception.</exception>
        private static byte[] Encrypt(string data, byte[] key, byte[] iv)
        {
            // Null exceptions
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException(nameof(data));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            // Encrypt data
            byte[] encrypted;
            // Start Aes algorithm
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            // Create encryptor based on the key and IV
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using MemoryStream myEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(myEncrypt, encryptor, CryptoStreamMode.Write);
            using BinaryWriter stream = new BinaryWriter(csEncrypt, Encoding.UTF8);
            // Write data to stream
            stream.Write(data);
            // Write data from stream to byte array
            csEncrypt.FlushFinalBlock();
            encrypted = myEncrypt.ToArray();
            return encrypted;
        }

        /// <summary>Decrypts data with the AES algorithm.</summary>
        /// <param name="data">Data to decrypt.</param>
        /// <param name="key">Decryption key.</param>
        /// <param name="iv">Decryption IV.</param>
        /// <returns>Decrypted data under the form of a string.</returns>
        /// <exception cref="ArgumentNullException">Null data exception.</exception>
        private static string Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            // Null exceptions
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException(nameof(data));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            // Decrypted data
            string text;
            // Start Aes algorithm
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            // Create encryptor based on the key and IV
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream myDecrypt = new MemoryStream(data);
            using CryptoStream csDecrypt = new CryptoStream(myDecrypt, decryptor, CryptoStreamMode.Read);
            using BinaryReader stream = new BinaryReader(csDecrypt, Encoding.UTF8);
            // Write data to stream
            text = stream.ReadString();
            return text;
        }

        /// <summary>Transforms a list of <see cref="AstralObject"/> to JSON information.</summary>
        /// <param name="gos">List of <see cref="AstralObject"/> to use.</param>
        /// <returns>JSON object under a string format.</returns>
        private static string[] JsonfyGos(System system)
        {
            // Open the jsons
            string jsonStream = "[";
            // Go through every element of the scene's list
            system.ForEach(obj =>
            {
                jsonStream += "{" + "Name: \"" + obj.Name + "\",Position: {X: " + obj.Position.X + ",Y: " + obj.Position.Y + ",Z: " + obj.Position.Z +
                "}, Rotation: {X: " + obj.Rotation.X + ",Y: " + obj.Rotation.Y + ",Z: " + obj.Rotation.Z + "},RotationSpeed: " + obj.RotationSpeed + ",Revolution: " + obj.Revolution + ",Radius: " + obj.Radius +
                ",Mass: " + obj.Mass + ",InitialVelocity: " + obj.InitialVelocity + ",SemiMajorAxis: " + obj.SemiMajorAxis + ",SemiMinorAxis: " + obj.SemiMinorAxis +
                ",OrbitalEccentricity: " + obj.OrbitalEccentricity + ",Perihelion: " + obj.Perihelion + ",Aphelion: " + obj.Aphelion + ",Tilt: " + obj.Tilt +
                ",OrbitalInclination: " + obj.OrbitalInclination + ",PerihelionLongitude: " + obj.PerihelionLongitude + ",AscendingNodeLongitude: " + obj.AscendingNodeLongitude + ",MeanLongitude: " + obj.MeanLongitude +
                ",MeanAnomaly: " + obj.MeanAnomaly + ",Type: \"" + obj.Type + "\"";
                jsonStream += "}, ";
            });
            
            // Delete the last comma of the jsons

            if (jsonStream != "[") jsonStream = jsonStream.Substring(0, jsonStream.LastIndexOf(','));

            // Close the json
            jsonStream += "]";
            return [jsonStream]; // return json
        }
    }
}