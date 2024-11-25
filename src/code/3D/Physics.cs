using Astral_simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astral_simulation
{
    public static class Physics
    {
        private const float GRAVITY_CONSTANT = 6.67430f * 10E-11f;
        public static float UpdateGravity(float massA, float massB, float radius)
        {
            return GRAVITY_CONSTANT * (massA * massB) / (float)Math.Pow(2 * radius, 2);
        }
    }
}
