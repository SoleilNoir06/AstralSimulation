using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Astral_simulation
{
    public static class VelocityVerlet
    {
        private const float _G = 6.67430e-11f; // Gravitational constant
        // Vars used for Velocity Verlet algorythm.

        private static int _iCpt = 0;
        private static int _astralObjectCount = 0; // Number of astral objects
        private static float _dt = 0; // Delta time
        private static float _t = 0; // Current instant
        private static float _next_t = _t + _dt; // Instant at next frame
        private static Vector3[,] _gravitationnalForces; // Forces matrice
        private static List<Vector3> _previousAcceleration = new List<Vector3>();
        private static List<AstralObject> _astralObjects = new List<AstralObject>(); // Astral objects list

        /// <summary>
        /// Update verlet vars useful in formulas.
        /// </summary>
        /// <param name="system">System.</param>
        public static void Init(System system)
        {
            // Clearing and filling astral objects list
            _astralObjects.Clear();
            _astralObjectCount = system.Count;
            system.ForEach(obj => { _astralObjects.Add(obj); });
            _gravitationnalForces = new Vector3[_astralObjectCount, _astralObjectCount];      

            //Compute initial acceleration
            ComputeAcceleration();      
        } 
        
        public static void Update()
        {
            _previousAcceleration.Clear();
            //Time management
            _dt = GetFrameTime();

            //Verlet formulas, 1st equation
            for(int i = 0; i < _astralObjectCount; i++)
            {
                AstralObject obj = _astralObjects[i];
                //Compute new position of object
                obj.Position = obj.Position + obj.Velocity * _dt + 0.5f * obj.Acceleration * _dt * _dt;

                //Save acceleration for 3rd equation
                _previousAcceleration.Add(obj.Acceleration);
            }

            //2nd equation, compute new gravitationnal forces between objects
            ComputeAcceleration();

            //3rd equation
            for(int i = 0; i < _astralObjectCount; i++)
            {
                AstralObject obj = _astralObjects[i];
                //Compute new velocity of object
                obj.Velocity = obj.Velocity + 0.5f *(_previousAcceleration[i] + obj.Acceleration) * _dt;
            }
        }

        /// <summary>
        /// Using Newton's universal gravitation formula, compute force between astrak objects.
        /// </summary>
        /// <param name="system">System.</param>
        public static void ComputeGravitationnalForce()
        {            
            // Filling matrice of forces
            Parallel.For(0, _astralObjectCount, i =>
            {
                // Compute distance between objects
                AstralObject obj1 = _astralObjects[i];
                for (int j = i + 1; j < _astralObjectCount; j++)
                {
                    AstralObject obj2 = _astralObjects[j];
                    float distance = Vector3.Distance(obj1.Position, obj2.Position);

                    // Compute force vector between objects
                    float forceMagnitude = _G * obj1.Mass * obj2.Mass / (distance * distance);

                    // Compute direction vector from obj1 to obj2 and normalize it
                    Vector3 direction = obj2.Position - obj1.Position;
                    direction = Vector3.Normalize(direction);

                    // Compute force vector and store it in matrice
                    Vector3 force = forceMagnitude * direction;
                    _gravitationnalForces[i, j] = force;
                    _gravitationnalForces[j, i] = -force;
                }                
            });
        }

        /// <summary>
        /// Compute object's acceleration.
        /// </summary>
        public static void ComputeAcceleration()
        {
            ComputeGravitationnalForce();

            for (int i = 0; i < _astralObjectCount; i++)
            {
                AstralObject obj = _astralObjects[i];
                Vector3 totalForce = Vector3.Zero;
                for (int j = 0; j < _astralObjectCount; j ++)
                    totalForce += _gravitationnalForces[i, j];
                
                obj.Acceleration = totalForce / obj.Mass;
            }
        }
    }
}