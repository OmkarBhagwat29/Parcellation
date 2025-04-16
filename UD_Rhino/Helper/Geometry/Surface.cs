using Rhino.Geometry;


namespace Parcellation.Helper.Geometry
{
    public static class SurfaceHelper
    {
        public static List<Surface> IsoTrimSurface(this Surface srf, int uCount, int vCount)
        {
            var results = new List<Surface>();

            var uDomain = srf.Domain(0);
            var vDomain = srf.Domain(1);

            double uStep = (uDomain.T1 - uDomain.T0) / uCount;
            double vStep = (vDomain.T1 - vDomain.T0) / vCount;

            for (int i = 0; i < uCount; i++)
            {
                for (int j = 0; j < vCount; j++)
                {
                    double u0 = uDomain.T0 + i * uStep;
                    double u1 = u0 + uStep;
                    double v0 = vDomain.T0 + j * vStep;
                    double v1 = v0 + vStep;

                    var trimmed = srf.Trim(new Interval(u0, u1),new Interval( v0, v1));
                    if (trimmed != null)
                        results.Add(trimmed);
                }
            }

            return results;
        }
    }
}
