using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Helper
{
    public static class MathHelper
    {
        public static double Remap(this double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }
    }
}
