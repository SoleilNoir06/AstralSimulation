using Raylib_cs;
using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="AstralObject"/>.</summary>
    public unsafe abstract class AstralObject
    {
        // -----------------------------------------------------------
        // Private instances
        // -----------------------------------------------------------

        private long _mass;
        private long _radius;
        private float _gravitationPull;
        private Vector3 _rotation;
        private Vector3 _velocity;

        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------

        public float RotationSpeed;
        public string Name { get; set; }
        public float RotationPeriod { get; set; } // On itslef
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

        /// <summary>Radius of the object.</summary>
        public long Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                UpdateGravitationPull();
            }
        }

        /// <summary>Mass of the object.</summary>
        public long Mass 
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
        public long Volume { get { return (long)(4 * Math.PI * Math.Pow(_radius, 3) / 3); } }

        /// <summary>Vectorial speed of the object.</summary>
        public Vector3 Velocity { get {  return _velocity; } }

        /// <summary>Gravition pull</summary>
        public float GravitationPull { get { return _gravitationPull; } }

        /// <summary>Creates an instance of <see cref="AstralObject"/>.</summary>
        /// <param name="mass">Mass of the object.</param>
        /// <param name="radius">Radius of the object.</param>
        /// <param name="orbitPeriod">Obritation period of the object.</param>
        /// <param name="rotationPeriod">Rotatino period of the object.</param>
        public AstralObject(long mass, long radius, float orbitPeriod, float rotationPeriod)
        {
            _mass = mass;
            _radius = radius;
            OrbitPeriod = orbitPeriod;
            RotationPeriod = rotationPeriod;

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
    }
}