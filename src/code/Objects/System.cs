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

        /// <summary>Creates an empty <see cref="System"/>.</summary>
        public System()
        {
            _objects = RLoading.LoadDefaultSystem(); // Load default solar system
            _name = DEFAULT_SYSTEM_NAME;
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

        public override string ToString()
        {
            return $"The system {_name} contains {_objects.Count}";
        }
    }
}