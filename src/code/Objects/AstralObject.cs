using System.Numerics;

namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="AstralObject"/>.</summary>
    public abstract class AstralObject
    {
        private long _mass;
        private long _radius;

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
                GravitionPull = CalculateGravitation(_mass, _radius);
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
                GravitionPull = CalculateGravitation(_mass, _radius);
            } 
        }

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
        }


        public long Volume { get { return (long)(4 * Math.PI * Math.Pow(_radius, 3) / 3); } }

        public Vector2 Velocity;
        public float RotationSpeed;
        public float RotationPeriod; // On itslef
        public float OrbitPeriod; // Around parent object

        public float GravitionPull;

        public float CalculateGravitation(long mass, long radius)
        {
            return 0f;
        }
    }
}
