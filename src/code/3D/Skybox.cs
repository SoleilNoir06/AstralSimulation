using static Raylib_cs.Raylib;

namespace Raylib_cs.Complements
{
    /// <summary>Represents an instance of <see cref="Skybox"/>.</summary>
    public struct Skybox
    {
        /// <summary>Default Mesh of the skyboxes instances.</summary>
        public static readonly Mesh Mesh = GenMeshCube(1f, 1f, 1f);

        public Material Material;

        /// <summary>Creates an empty skybox object.</summary>
        public Skybox()
        {
            Material = LoadMaterialDefault();
        }
    }
}