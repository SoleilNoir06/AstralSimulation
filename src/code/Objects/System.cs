namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="System"/>.</summary>
    public class System
    {
        public const string DEFAULT_SYSTEM_NAME = "Solar System"; // Default system name

        private List<AstralObject> _objects; // system objects
        private string _name; // system name

        /// <summary>Name of system</summary>
        public string Name { get { return _name; } set { _name = value; } }

        public int Count { get { return _objects.Count; } }

        /// <summary>Creates an empty <see cref="System"/>.</summary>
        public System()
        {
            _objects = RLoading.LoadDefaultSystem();
            _name = "Solar";
        }

        /// <summary>Creates an new <see cref="System"/>.</summary>
        /// <param name="objects">Objects of the system.</param>
        /// <param name="name">Name of the system.</param>
        public System(List<AstralObject> objects, string name)
        {
            _objects = objects;
            _name = name;
        }

        /// <summary>Iterates over the </summary>
        /// <param name="action"></param>
        public void ForEach(Action<AstralObject> action)
        {
            _objects.ForEach(action);
        }

        /// <summary>Gets the first matching object in system.</summary>
        /// <param name="name">Name of the object to search for.</param>
        /// <returns>Corresponding <see cref="AstralObject"/>.</returns>
        public AstralObject GetObject(string name)
        {
            return _objects.Where(x => x.Name == name).ToArray()[0]; // Return 1st element matching
        }

        public override string ToString()
        {
            return $"The system {_name} contains {_objects.Count}";
        }
    }
}