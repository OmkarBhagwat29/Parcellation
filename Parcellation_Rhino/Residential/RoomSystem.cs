using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Residential
{
    public class RoomSystem
    {

        public static Curve BoundingCurve;

        public Plane BasePlane { get; set; }



        public List<Room> Rooms { get; set; } = new List<Room>();

        public RoomSystem(Plane _basePlane,Curve footPrintBoundary)
        {
            this.BasePlane = _basePlane;

            Curve c;


        }

        public void AddRoom(Room rm)
        {
            this.Rooms.Add(rm);
        }
    }
}
