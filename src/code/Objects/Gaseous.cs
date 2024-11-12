using Astral_simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astral_simulation
{
    public class Gaseous : AstralObject
    {
        public enum FloorElement
        {
            Rocks,
            Ice,
            Methane,
            MetallicHydrogen,
            Silicates,
            Iron

        }
        public enum AtmosphereElement
        {
            Hydrogen,
            Helium,
            Hydrocarbon,
            Nitrogen,
            Ammonia,
            Methane,
            Water
        }
        public Gaseous(long mass, long radius, float orbitPeriod, float rotationPeriod, float tiltAngle) : base(mass, radius, orbitPeriod, rotationPeriod, tiltAngle) 
        { 

        }

    }
}
