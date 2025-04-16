using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Helper.Inputs
{
    public static class RhinoSelect
    {
        public static ObjRef SelectCurve(string message)
        {
          var result =  Rhino.Input.RhinoGet.GetOneObject(message, true, ObjectType.Curve, out ObjRef rhObj);

            if (result == Rhino.Commands.Result.Success && rhObj !=null)
            {
                return rhObj;
            }

            return null;
        }

        public static ObjRef[] SelectCurves(string message)
        {
            var result = Rhino.Input.RhinoGet.GetMultipleObjects(message, true, ObjectType.Curve, out ObjRef[] rhObjs);

            if (result == Rhino.Commands.Result.Success && rhObjs != null)
            {
                return rhObjs;
            }

            return null;
        }

        public static ObjRef SelectObject(string message , ObjectType type)
        {
            var result = Rhino.Input.RhinoGet.GetOneObject(message, true, type, out ObjRef rhObj);

            if (result == Rhino.Commands.Result.Success && rhObj != null)
            {
                return rhObj;
            }

            return null;
        }

        public static ObjRef[] SelectObjects(string message, ObjectType type)
        {
            var result = Rhino.Input.RhinoGet.GetMultipleObjects(message, true, type, out ObjRef[] rhObjs);

            if (result == Rhino.Commands.Result.Success && rhObjs != null)
            {
                return rhObjs;
            }

            return null;
        }
    }
}
