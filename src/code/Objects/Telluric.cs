using Astral_simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astral_simulation
{
    public enum FloorElement
    {
        Silicate,
        Limestone,
        Sand, 
        Iron,
        Clay,
        Silt,
        Water,
        Lava,
        Dirt,
        Aluminum,
        Rocks
    }
    public enum AtmosphereElement
    {
        Hydrogen,
        Oxygen,
        Nitrogen,
        CarboneDioxyde,
        Dust
    }
    public class Telluric : AstralObject
    {
        public Telluric(long mass, long radius, float orbitPeriod, float rotationPeriod, float tiltAngle) : base (mass, radius, orbitPeriod, rotationPeriod, tiltAngle)
        {
        }
    }
}
