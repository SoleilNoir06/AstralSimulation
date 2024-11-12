using Astral_simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astral_simulation
{
    public class Stars : AstralObject
    {
        public float Temperature;
        public float Brightness;
        public float Luminosity;

        /// <summary>
        /// Creates an instance of star
        /// </summary>
        /// <param name="temperature">Temperature of star</param>
        /// <param name="brightness"> Brightness of star</param>
        /// <param name="luminosity">Luminosity of star</param>
        /// <param name="mass">Mass of star</param>
        /// <param name="radius">Radius of star</param>
        /// <param name="orbitPeriod">Orbit period of star</param>
        /// <param name="rotationPeriod">Rotation period of star</param>
        public Stars(float temperature, float brightness, float luminosity, long mass, long radius, float orbitPeriod, float rotationPeriod): base (mass, radius, orbitPeriod, rotationPeriod)
        { 
            Temperature = temperature;
            Brightness = brightness;
            Luminosity = luminosity;
        }
    }
}
