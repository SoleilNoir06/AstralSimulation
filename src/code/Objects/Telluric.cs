using Raylib_cs;

namespace Astral_simulation
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
        public Telluric(long mass, long radius, float orbitPeriod, float rotationPeriod, float tiltAngle, Texture2D texture) : base (mass, radius, orbitPeriod, rotationPeriod, tiltAngle)
        {
            Raylib.SetMaterialTexture(ref Material1, MaterialMapIndex.Diffuse, texture); // Set texture to planet mesh material
        }
    }
}
