using Raylib_cs;
using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="AstralObject"/>.</summary>
    public unsafe class AstralObject
    {
        // -----------------------------------------------------------
        // Private instances
        // -----------------------------------------------------------

        private float _mass;
        private float _radius;
        private string _type;
        private float _gravitationPull;
        private float _revolution;
        private Vector3 _rotation;
        private Vector3 _velocity;
        private Color _attributeColor;
        private string _description;
        private float _rotationSpeed;
        private string _name;
        private float _initialVelocity;
        private float _semiMajorAxis;
        private float _orbitalEccentricity;
        private float _perihelion;
        private float _aphelion;
        private float _tilt;
        private float _orbitalInclination;
        private float _perihelionLongitude;
        private float _ascendingNodeLongitude;
        private float _meanLongitude;
        private float _meanAnomaly;

        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------

        public float RotationSpeed
        {
            get
            {
                return _rotationSpeed;
            }
            set
            {
                _rotationSpeed = value;
            }
        }

        public string Name { get { return _name; } 
            set 
            {
                _name = value;
                // Load corresponding texture
                Raylib.SetMaterialTexture(ref Material1, MaterialMapIndex.Diffuse, Raylib.LoadTexture($"assets/textures/{Name}.png"));
            } 
        }
        public float RotationPeriod { get; set; } // On itself
        public float OrbitPeriod { get; set; } // Around parent object

        public bool UIActive { get; set; } = false; // Defines whether the object is active in the UI overlay

        public Material Material1; // Generic material used for planet mesh
        public Material Material2; // Material used for external rings mesh
        public Matrix4x4 Transform; // Transform matrix used to define object properties

        /// <summary>Spatial position.</summary>
        public Vector3 Position
        {
            get
            {
                return new Vector3(Transform.M14, Transform.M24, Transform.M34);
            }
            set
            {
                Transform.M14 = value.X;
                Transform.M24 = value.Y;
                Transform.M34 = value.Z;
            }
        }

        /// <summary>Vectorial rotation of the object.</summary>
        public Vector3 Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                UpdateTransform();
            }
        }

        /// <summary>The attribute color stands for the color used in various GUI options related to the object.</summary>
        public Color AttributeColor
        {
            get { return _attributeColor; }
            set
            {
                _attributeColor.R = value.R;
                _attributeColor.G = value.G;
                _attributeColor.B = value.B;
                _attributeColor.A = value.A;
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>Time for the objec to revolut</summary>
        public float Revolution
        {
            get
            {
                return _revolution;
            }
            set
            {
                _revolution = value;
            }
        }

        /// <summary>X axis rotation.</summary>
        public float Pitch
        {
            get { return _rotation.X; }
            set
            {
                _rotation.X = value;
                UpdateTransform();
            }
        }

        /// <summary>Y axis rotation.</summary>
        public float Yaw
        {
            get { return _rotation.Y; }
            set
            {
                _rotation.Y = value;
                UpdateTransform();
            }
        }

        /// <summary>Z axis rotation.</summary>
        public float Roll
        {
            get { return _rotation.Z; }
            set
            {
                _rotation.Z = value;
                UpdateTransform();
            }
        }

        /// <summary>Initial velocity of planet</summary>
        public float InitialVelocity
        {
            get
            {
                return _initialVelocity;
            }
            set
            {
                _initialVelocity = value;
            }
        }

        /// <summary>Semimajor axis of orbital ellipse</summary>
        public float SemiMajorAxis
        {
            get
            {
                return _semiMajorAxis;
            }
            set
            {
                _semiMajorAxis = value;
            }
        }

        /// <summary>Orbital eccentricity of the object</summary>
        public float OrbitalEccentricity
        {
            get
            {
                return _orbitalEccentricity;
            }
            set
            {
                _orbitalEccentricity = value;
            }
        }

        /// <summary>Perihelion of the object</summary>
        public float Perihelion
        {
            get
            {
                return _perihelion;
            }
            set
            {
                _perihelion = value;
            }
        }
        /// <summary>Aphelion of the object</summary>
        public float Aphelion
        {
            get
            {
                return _aphelion;
            }
            set
            {
                _aphelion = value;
            }
        }
         /// <summary>Tilt of the object</summary>
        public float Tilt
        {
            get
            {
                return _tilt;
            }
            set
            {
                _tilt = value;
            }
        }

        /// <summary>Orbital inclination of the object.</summary>
        public float OrbitalInclination
        {
            get
            {
                return _orbitalInclination;
            }
            set
            {
                _orbitalInclination = value;
            }
        }

        /// <summary>Perihelion longitude of the object.</summary>
        public float PerihelionLongitude
        {
            get
            {
                return _perihelionLongitude;
            }
            set
            {
                _perihelionLongitude = value;
            }
        }

        /// <summary>Ascending node longitude of the object.</summary>
        public float AscendingNodeLongitude
        {
            get
            {
                return _ascendingNodeLongitude;
            }
            set
            {
                _ascendingNodeLongitude = value;
            }
        }

        /// <summary>Mean longitude of the object.</summary>
        public float MeanLongitude
        {
            get
            {
                return _meanLongitude;
            }
            set
            {
                _meanLongitude = value;
            }
        }

        /// <summary>Mean anomaly of the object</summary>
        public float MeanAnomaly
        {
            get
            {
                return _meanAnomaly;
            }
            set
            {
                _meanAnomaly = value;
            }
        }

        /// <summary>Radius of the object.</summary>
        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value / 2;
                UpdateTransform();
                UpdateGravitationPull();
            }
        }

        /// <summary>Type of the object.</summary>
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>Mass of the object.</summary>
        public float Mass 
        { 
            get
            { 
                return _mass; 
            } 
            set 
            {
                _mass = value;
                UpdateGravitationPull();
            } 
        }

        /// <summary>Gets the volume of the object.</summary>
        public long Volume { get { return (long)(4 * Math.PI * Math.Pow(_radius * 150000, 3) / 3); } }

        /// <summary>Vectorial speed of the object.</summary>
        public Vector3 Velocity { get {  return _velocity; } }

        /// <summary>Gravition pull</summary>
        public float GravitationPull { get { return _gravitationPull; } }

        /// <summary>Creates an instance of <see cref="AstralObject"/>.</summary>
        /// <param name="mass">Mass of the object.</param>
        /// <param name="radius">Radius of the object.</param>
        /// <param name="orbitPeriod">Obritation period of the object.</param>
        /// <param name="rotationPeriod">Rotatino period of the object.</param>
        public AstralObject(float mass, float radius, float orbitPeriod, float rotationPeriod)
        {
            _mass = mass;
            _radius = radius;
            OrbitPeriod = orbitPeriod;
            RotationPeriod = rotationPeriod;
            _name = "";

            Transform = Raymath.MatrixRotateX(90 * Raylib.DEG2RAD); // Set default transform
            Material1 = Raylib.LoadMaterialDefault(); // Load default materials and set default shader
            Material1.Shader = ShaderCenter.LightingShader;
            Material2 = Raylib.LoadMaterialDefault();
            Material2.Shader = ShaderCenter.LightingShader;
        }

        protected void UpdateGravitationPull()
        {
            // Update
        }

        protected void UpdateVectorialSpeed()
        {
            // Update
        }

        protected void UpdateSize()
        {
            // Update
             
        }

        protected void UpdateTransform()
        {
            Matrix4x4 rm = Raymath.MatrixRotateXYZ(_rotation / Raylib.RAD2DEG);
            Matrix4x4 sm = Raymath.MatrixScale(_radius, _radius, _radius);
            Matrix4x4 pm = Raymath.MatrixTranslate(Transform.M14, Transform.M24, Transform.M34);
            // Multiply matrices in order
            Transform = pm * sm * rm;
        }
    }
}