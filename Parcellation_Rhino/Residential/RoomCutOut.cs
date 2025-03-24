using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Residential
{
    public class RoomCutOut
    {
        public Curve RoomCutBoundary { get; set; }

        public Plane CutOutPlane { get; set; }

        public RoomCutOut(Plane _cutoutPlane, Curve _roomCutBoundary)
        {
            this.CutOutPlane = _cutoutPlane;
            this.RoomCutBoundary = _roomCutBoundary;
        }
    }
}
