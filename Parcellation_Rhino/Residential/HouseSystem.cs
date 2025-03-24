using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Residential
{
    public class HouseSystem
    {
        public List<RoomSystem> RoomSytems { get; set; } = new List<RoomSystem>();

        public HouseSystem(List<RoomSystem> _rmSys)
        {
            this.RoomSytems = _rmSys;
        }
    }
}
