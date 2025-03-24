using Rhino;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using UrbanDesign.Helper;
using UrbanDesign.Ui.Models;

namespace UrbanDesign.Parcellation
{
    public class Parcellation : ParcellationDisplay
    {
        //RhinoDoc doc = RhinoDoc.ActiveDoc;
        private Curve _parcelCurve;
        private List<Curve> _roadNetworkCurves = [];
        List<Curve> _splitMainBoundaryParcels = [];
        List<Curve> _offsetMajorParcels = [];
        List<Curve> _splitMajorParcels = [];
        List<Curve> _subParcels = [];
        List<AreaMassProperties> _subParcelAreaProps = [];
        List<Curve> _buildingParcels = [];
        double _majorRoadWidth = 18;
        double _minorRoadWidth = 9;
        Point3d? _attractor;

        private List<Point3d> _greenPoints = [];

        public List<Point3d> GreenPoints
        {
            get { return _greenPoints; }
            set { _greenPoints = value; if (this.ParcelCurve != null && this.RoadNetworkCurves.Count > 0)
                    this.CalculateParcels();
            }
        }

        List<bool> _greenParcelIndicator = [];
        List<Brep> _greenParcels = [];
        List<Brep> _greenZones = [];

        double _minParcelArea = 400;

        Brep _mainParcelBrep = null;

        List<Brep> _buildingParcelBreps = [];

        double _minDepth = 15;
        double _maxDepth = 60;

        public double MinDepth
        {
            get { return _minDepth; }
            set { _minDepth = value;
            }
        }

        public double MaxDepth
        {
            get { return _maxDepth; }
            set { _maxDepth = value;
            }
        }

        // Property with getter and setter
        public double MinParcelArea
        {
            get { return _minParcelArea; }
            set
            {
                _minParcelArea = value;
                this.SetBuildingParcels();
            }
        }

        public Point3d? Attractor
        {
            get { return _attractor; }
            set
            {
                _attractor = value;
                if(this.ParcelCurve!=null && this.RoadNetworkCurves.Count>0)
                    this.CalculateParcels();
            }
        }

        double _buildingParcelDepth = 20;
       double _buildingParcelWidth = 10;

        public double BuildingParcelWidth
        {
            get { return _buildingParcelWidth; }
            set { _buildingParcelWidth = value; }
        }


        public double BuildingParcelDepth
        {
            get { return _buildingParcelDepth; }
            set { _buildingParcelDepth = value; }
        }


        double _subParcelDepth = 60;

        public double SubParcelDepth
        {
            get { return _subParcelDepth; }
            set { _subParcelDepth = value; }
        }

         double _subParcelWidth = 30;

        public double SubParcelWidth
        {
            get { return _subParcelWidth; }
            set { _subParcelWidth = value; }
        }


        public double MajorRoadWidth
        {
            get => _majorRoadWidth;
            set
            {
                _majorRoadWidth = value;
                this.CalculateParcels();
            }
        }

        public double MinorRoadWidth
        {
            get => _minorRoadWidth; set
            {
                _minorRoadWidth = value;
                this.OffsetSubParcelsByMinorRoadWidth();
            }
        }

        public Curve ParcelCurve
        {
            get => _parcelCurve;
            set
            {
                _parcelCurve = value;

                var brep = Brep.CreatePlanarBreps(value, 0.001);
                if (brep != null && brep.Length > 0)
                {
                    this._mainParcelBrep = brep[0];
                  this._mainParcelBrep.Translate(new Vector3d(0, 0, -1));
                }

                Execute();  // Run Execute when set
            }
        }

        public List<Curve> RoadNetworkCurves
        {
            get => _roadNetworkCurves;
            set
            {
                _roadNetworkCurves = value ?? new List<Curve>();
                Execute();  // Run Execute when set
            }
        }

        public Parcellation()
        {
            this.Enabled = false;
        }

        public void Execute()
        {
            Console.WriteLine("Execute() has been triggered!");
            // Add logic here

            if (ParcelCurve == null || this.RoadNetworkCurves.Count == 0)
                return;

            this.Enabled = true;
            this._splitMainBoundaryParcels = this.SplitParcelWithRoads();
            this.CalculateParcels();

        }

        public void CalculateParcels()
        {
            this.OffsetSubParcelsByMajorRoadWidth();
            this.OffsetSubParcelsByMinorRoadWidth();
        }

        public void OffsetSubParcelsByMajorRoadWidth()
        {
            var greenZones = new List<Brep>();
            List<Curve> subOutlines = [];
            var offsetParcels = new List<Curve>();
            foreach (var item in this._splitMainBoundaryParcels)
            {
                try
                {
                    var cvs = item.Offset(Plane.WorldXY,
               -this.MajorRoadWidth / 2.0, 0.01, CurveOffsetCornerStyle.Sharp);

                    if (cvs != null && cvs.Length == 1)
                    {
                        offsetParcels.AddRange(cvs);
                        //check if its a green zone
                        if (this.GreenPoints
                            .Any(pt => cvs[0].Contains(pt, Plane.WorldXY, 0.001) == PointContainment.Inside))
                        {
                            var brep = Brep.CreatePlanarBreps(cvs[0], 0.001);
                            if (brep != null && brep.Length > 0)
                            {
                                greenZones.Add(brep[0]);
                            }
                        }
                        else
                        {
                            var smallParcelOutlines = SubDivideParcel(cvs[0], this.SubParcelDepth,
                                this.SubParcelWidth);

                            if (smallParcelOutlines != null)
                            {
                                subOutlines.AddRange(smallParcelOutlines);
                            }
                        }
                    }


                }
                catch
                {

                }
            }

            

            this._offsetMajorParcels = offsetParcels;
            this._splitMajorParcels = subOutlines;
            this._greenZones = greenZones;
        }


        public void OffsetSubParcelsByMinorRoadWidth()
        {
            var parcels = new List<Curve>();
            this._splitMajorParcels
                 .ForEach(c =>
                 {
                     var cvs = c.Offset(Plane.WorldXY,
                 -this.MinorRoadWidth / 2.0, 0.01, CurveOffsetCornerStyle.Sharp);

                     if (cvs != null)
                     {
                         if (cvs.Length == 1)
                         {
                             parcels.Add(cvs[0]);
                         }
                     }
                 });

            this._subParcels = parcels;
            this._subParcelAreaProps = [.. this._subParcels.Select(p => AreaMassProperties.Compute(p))];

            this.SetBuildingParcels();
        }

        public string SubParcelsReport()
        {
            if (this._offsetMajorParcels.Count == 0)
                return "No Sub-Parcels found!!!";

            var sb = new StringBuilder();
            for (int i = 0; i < this._offsetMajorParcels.Count; i++)
            {
                sb.Append($"Parcel Id - {i + 1} has " +
                    $"{Math.Round(AreaMassProperties.Compute(this._offsetMajorParcels[i]).Area, 2)} SqMts\n");
            }
            return sb.ToString();
        }


        protected override void DrawForeground(DrawEventArgs e)
        {
            {
                e.Display.DrawBrepShaded(this._mainParcelBrep, new DisplayMaterial(Color.Black));
                foreach (var g in this._greenParcels)
                {
                    e.Display.DrawBrepShaded(g, new DisplayMaterial(Color.LightGreen));
                }


                foreach (var b in this._buildingParcelBreps)
                {
                    e.Display.DrawBrepShaded(b, new DisplayMaterial(Color.WhiteSmoke));
                }

                this._greenZones.ForEach(z => e.Display.DrawBrepShaded(z, new DisplayMaterial(Color.LightGreen)));


                foreach (var c in this._buildingParcels)
                {
                    e.Display.DrawCurve(c, new DisplayPen() { Color = Color.Black, Thickness = 2 });
                }


                for (int i = 0; i < this._subParcels.Count; i++)
                {
                    if (this._greenParcelIndicator[i])
                    {
                        e.Display.DrawCurve(this._subParcels[i],
                        new DisplayPen() { Color = Color.Green, Thickness = 2 });
                    }
                }

           

                if (this.Attractor is not null)
                {
                    e.Display.DrawBrepShaded(new Sphere(this.Attractor.Value, 15).ToBrep(),
                        new DisplayMaterial(Color.Red));
                }

                this._greenPoints.ForEach(zPt => e.Display.DrawBrepShaded(new Sphere(zPt, 15).ToBrep(),
                        new DisplayMaterial(Color.DarkGreen)));

                foreach (var c in this.RoadNetworkCurves)
                {
                    e.Display.DrawCurve(c, new DisplayPen() { Color = Color.BurlyWood, Thickness = 5 });
                }

            }

        }



        List<Curve> SplitParcelWithRoads()
        {
            var allCurves = new List<Curve> { this.ParcelCurve };
            allCurves.AddRange(this.RoadNetworkCurves);


            // Boolean split the regions
            var splitRegions = Curve.CreateBooleanRegions(allCurves, Plane.WorldXY, false, 0.01);

            var closedParcels = new List<Curve>();
            if (splitRegions != null)
            {

                for (int i = 0; i < splitRegions.RegionCount; i++)
                {
                    var regions = splitRegions.RegionCurves(i).ToList();

                    regions.ForEach(r =>
                    {
                        if (r.IsClosed)
                        {
                            var ori = r.ClosedCurveOrientation(new Vector3d(0, 0, 1));
                            if (ori != CurveOrientation.CounterClockwise)
                            {
                                r.Reverse();
                            }
                            closedParcels.Add(r);
                        }
                        //RhinoDoc.ActiveDoc.Objects.AddCurve(r);
                    });
                }
            }

            return closedParcels;
        }

        static List<Curve> SubDivideParcel(Curve parcel,
            double subParcelDepth,
            double subParcelWidth,
            bool limitTo2 = false)
        {

            // Get the longest segment
            Line? seg = parcel.DuplicateSegments()
                .Select(c => new Line(c.PointAtStart, c.PointAtEnd))
                .OrderByDescending(l => l.Length)
                .FirstOrDefault();

            if (seg == null)
                return null;

            // Define X-axis using the segment's direction
            Vector3d xAxis = seg.Value.Direction;
            xAxis.Unitize();

            // Define Y-axis (perpendicular to X, lying in the XY plane)
            Vector3d yAxis = Vector3d.CrossProduct(Vector3d.ZAxis, xAxis);
            yAxis.Unitize();

            // Create a plane with the segment's start point
            Plane plane = new Plane(seg.Value.From, xAxis, yAxis);


            var bbx = parcel.GetBoundingBox(plane);
            //bbx.Transform(Transform.PlaneToPlane(Plane.WorldXY, plane));

            var edges = bbx.GetEdges().Take(4).Select(l =>
            {

                l.Transform(Transform.PlaneToPlane(Plane.WorldXY, plane));
                return l;
            }).OrderByDescending(e => e.Length).ToList();

            var vDim = edges
                [0].Length;
            var uDim = edges[2].Length;

            var vCv_1 = edges[0].ToNurbsCurve();
            var bLn = edges[1];
            bLn.Flip();
            var vCv_2 = bLn.ToNurbsCurve();

            // width 100 //depth 60
            var vCount = (int)Math.Ceiling(uDim / subParcelDepth) - 1;
            var uCount = (int)Math.Ceiling(vDim / subParcelWidth) - 1;

            if (limitTo2)
            {
                vCount = Math.Min((int)Math.Ceiling(uDim / subParcelDepth), 1)-1;
            }


            var vCvs = Curve.CreateTweenCurves(vCv_1, vCv_2, vCount, 0.01);

            var uCv_1 = edges[2].ToNurbsCurve();
            var vLn = edges[3];
            vLn.Flip();
            var uCv_2 = vLn.ToNurbsCurve();


            var uCvs = Curve.CreateTweenCurves(uCv_1, uCv_2, uCount, 0.01);


            var allCurves = new List<Curve>();
            allCurves.AddRange(uCvs);
            allCurves.AddRange(vCvs);
            allCurves.Add(parcel.DuplicateCurve());

            var splitRegions = Curve.CreateBooleanRegions(allCurves, Plane.WorldXY, false, 0.01);

            List<Curve> closedParcels = [];

            if (splitRegions != null)
            {

                for (int i = 0; i < splitRegions.RegionCount; i++)
                {
                    var regions = splitRegions.RegionCurves(i).ToList();

                    regions.ForEach(r =>
                    {
                        if (r.IsClosed)
                        {
                            var center = AreaMassProperties.Compute(r).Centroid;

                            if (parcel.Contains(center, Plane.WorldXY, 0.001) == PointContainment.Inside)
                            {

                                var ori = r.ClosedCurveOrientation(new Vector3d(0, 0, 1));
                                if (ori != CurveOrientation.CounterClockwise)
                                {
                                    r.Reverse();
                                }
                                closedParcels.Add(r);
                                //doc.Objects.AddCurve(r);
                            }

                        }

                    });
                }
            }

            return closedParcels;
        }

        public void SetBuildingParcels()
        {
            var parcels = new List<Curve>();
            double[] remaps = null;
            var buildingParcelBreps = new List<Brep>();

            if (Attractor != null)
            {
                var attractorPoint = Attractor.Value;
                var data = _subParcelAreaProps
                    .Select(prop => new { Center = prop.Centroid, DistanceSquared = prop.Centroid.DistanceToSquared(attractorPoint) })
                    .ToList(); // Materializing the list to avoid multiple enumerations

                double min = data.Min(d => d.DistanceSquared);
                double max = data.Max(d => d.DistanceSquared);

                // Convert to an array for direct indexing in the loop
                remaps = data.Select(d => d.DistanceSquared.Remap(min, max, this.MinDepth, this.MaxDepth)).ToArray();
            }

            var greenIndicator = new List<bool>();
            var greenBreps = new List<Brep>();
            for (int i = 0; i < _subParcels.Count; i++)
            {
                if (this._subParcelAreaProps[i].Area < this.MinParcelArea)
                {
                    greenIndicator.Add(true);
                    var brep = Brep.CreatePlanarBreps(this._subParcels[i],0.001);
                    if (brep != null && brep.Length > 0)
                    {
                        greenBreps.Add(brep[0]);
                    }
                    continue;
                }

                    greenIndicator.Add(false);
    

               var parcel = _subParcels[i];
                
                double width = remaps != null ? remaps[i] : BuildingParcelWidth;

                var subParcels = SubDivideParcel(parcel, BuildingParcelDepth, width, true);

                if (subParcels != null)
                {
                    parcels.AddRange(subParcels);

                }
            }

            parcels.ForEach(s => {

                var brep = Brep.CreatePlanarBreps(s, 0.001);
                if (brep != null && brep.Length > 0)
                {
                    buildingParcelBreps.Add(brep[0]);
                }

            });

            _greenParcelIndicator = greenIndicator;
            _buildingParcels = parcels;
            _greenParcels = greenBreps;
            _buildingParcelBreps = buildingParcelBreps;
        }

    }
}
