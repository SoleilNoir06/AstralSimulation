using Newtonsoft.Json.Linq;
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
        private string _name;
        private float _initialVelocity;
        private float _semiMajorAxis;
        private float _semiMinorAxis;
        private float _orbitalEccentricity;
        private float _perihelion;
        private float _aphelion;
        private float _tilt;
        private float _perihelionLongitude;

        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------

        public float RotationSpeed;
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
                UpdateRotation();
            }
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
                UpdateRotation();
            }
        }

        /// <summary>Y axis rotation.</summary>
        public float Yaw
        {
            get { return _rotation.Y; }
            set
            {
                _rotation.Y = value;
                UpdateRotation();
            }
        }

        /// <summary>Z axis rotation.</summary>
        public float Roll
        {
            get { return _rotation.Z; }
            set
            {
                _rotation.Z = value;
                UpdateRotation();
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

        /// <summary>Semi-major axis of orbital ellipse</summary>
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

        /// <summary>Semi-minor axis of orbital ellipse</summary>
        public float SemiMinorAxis
        {
            get
            {
                return _semiMinorAxis;
            }
            set
            {
                _semiMinorAxis = value;
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
                UpdateScale();
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

        /// <summary>Updates the transform rotation of the object.</summary>
        protected void UpdateRotation()
        {
            Matrix4x4 nm = Raymath.MatrixRotateXYZ(_rotation / Raylib.RAD2DEG);
            nm.M14 = Position.X;
            nm.M24 = Position.Y; // Keep positions
            nm.M34 = Position.Z;
            Transform = nm;
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

        protected void UpdateScale()
        {
            Matrix4x4 mat = Raymath.MatrixScale(_radius, _radius, _radius);

            mat.M14 = Transform.M14;
            mat.M24 = Transform.M24;
            mat.M34 = Transform.M34;

            Transform = mat;
        }
    }
}