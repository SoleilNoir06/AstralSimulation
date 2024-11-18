namespace Astral_simulation
{
    /// <summary>Represents an instance of <see cref="Star"/>.</summary>
    public class Star : AstralObject
    {
        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------

        public float Temperature;
        public float Brightness;
        public float Luminosity;

        /// <summary>Creates an instance of <see cref="Star"/>.</summary>
        /// <param name="temperature">Temperature of star</param>
        /// <param name="brightness"> Brightness of star</param>
        /// <param name="luminosity">Luminosity of star</param>
        /// <param name="mass">Mass of star</param>
        /// <param name="radius">Radius of star</param>
        /// <param name="orbitPeriod">Orbit period of star</param>
        /// <param name="rotationPeriod">Rotation period of star</param>
        public Star(float temperature, float brightness, float luminosity, Int128 mass, long radius, float orbitPeriod, float rotationPeriod): base (mass, radius, orbitPeriod, rotationPeriod)
        { 
            Temperature = temperature;
            Brightness = brightness;
            Luminosity = luminosity;
        }
    }
}