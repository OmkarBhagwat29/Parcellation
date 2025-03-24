using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Logic
{
    public class Cut
    {
        private double length;
        private Polyline cutAway;
        private Polyline cutRemain;

        public Cut(double lengthOfCut, Polyline cutAway, Polyline cutRemain)
        {
            this.length = lengthOfCut;
            this.cutAway = cutAway;
            this.cutRemain = cutRemain;
        }

        public double getLength()
        {
            return length;
        }
        public Polyline getCutAway()
        {
            return cutAway;
        }
        public Polyline getCutRemain()
        {
            return cutRemain;
        }


        public static Polyline split(Polyline polyline, List<Polyline> resultList, double singlePartArea)
        {
            List<Line> segments = polyline.GetSegments().ToList();
            List<Cut> possibleCuts = new List<Cut>();
            AreaMassProperties amp = AreaMassProperties.Compute(polyline.ToNurbsCurve());

            for (int i = 0; i < segments.Count() - 2; i++)
            {
                for (int j = i + 2; j < segments.Count(); j++)
                {
                    int segmentsCovered = j - i + 1;
                    if (segments.Count() == segmentsCovered)
                    {
                        break;
                    }
                    Line edgeA = segments[i];
                    Line edgeB = segments[j];

                    EdgePair edgePair = new EdgePair(edgeA, edgeB);
                    EdgePairSubpolylines subpolyline = edgePair.getSubpolylines();
                    List<Cut> cutForCurrentEdgePair = subpolyline.getCuts(polyline, singlePartArea);
                    possibleCuts.AddRange(cutForCurrentEdgePair);
                }
            }

            if (possibleCuts.Count > 0)
            {
                Cut shortestCut = possibleCuts.Aggregate((minItem, nextItem) => minItem.getLength() < nextItem.getLength() ? minItem : nextItem);
                resultList.Add(shortestCut.getCutAway());

                return shortestCut.getCutRemain();
            }
            else return polyline;
        }

    }
}
