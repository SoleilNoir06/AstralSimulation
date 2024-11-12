namespace astral_simulation
{
    public class AstralObject
    {
        private long _mass;
        private long _radius;

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

        public AstralObject(long mass, long radius, float orbitPeriod, float rotationPeriod)
        {
            _mass = mass;
            _radius = radius;
            OrbitPeriod = orbitPeriod;
            RotationPeriod = rotationPeriod;
        }


        public long Volume { get { return (long)(4 * Math.PI * Math.Pow(_radius, 3) / 3); } }

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
