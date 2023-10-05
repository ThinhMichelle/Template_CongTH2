using Aspose.Cells.Charts;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcreteLiningBeam.Model
{
   public static class Method
    {
        public static Level GetLevel(this Element Elem, UIDocument uidoc)//this
        {
            Level Lv = null;

            if (Elem.LevelId != ElementId.InvalidElementId)
            {
                Lv = uidoc.Document.GetElement(Elem.LevelId) as Level;
            }
            else
            {
                var levelId = Elem.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
                Lv = uidoc.Document.GetElement(levelId) as Level;
            }

            return Lv;
        }

        public static bool IsParallel(this XYZ p, XYZ q)
        {
            // trả về độ dài tích có hướng của 2 vector
            return p.CrossProduct(q).GetLength() < 0.01;
        }

        public static PlanarFace GetBotFaceVector(this Solid solid)
        {
            List<Face> faces = GetFacesFromSolid(solid);

            var botFace = faces.Where(x => x.ComputeNormal(UV.Zero).IsParallel(XYZ.BasisZ) && x is PlanarFace)
               .Cast<PlanarFace>().OrderBy(x => x.Origin.Z).FirstOrDefault();

            return botFace;
        }

        public static List<CurveLoop> GetCurveLoop(this PlanarFace FaceBottom)
        {

            var List_CuveLoop = FaceBottom.GetEdgesAsCurveLoops().ToList();

            List_CuveLoop.Sort(new ElementSortMax());

            return List_CuveLoop;

        }

        public static List<Face> GetFacesFromSolid(Solid solid)
        {
            List<Face> faces = new List<Face>();

            if (solid.Volume > 0.0000000001)
            {
                foreach (Face face in solid.Faces)
                {
                    if (face.Area > 0.0000001)
                    {
                        faces.Add(face);
                    }
                }
            }
            return faces;
        }

        public static CurveArray GetCurveArrayOffset(this CurveLoop CuLoop, double offset, XYZ nor)
        {
            ////#if version2017 || version2018 || version2019 || version2020
            //double offsetinternal = UnitUtils.Convert(Convert.ToDouble(offset), DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);
            double offsetinternal = UnitUtils.Convert(Convert.ToDouble(offset), UnitTypeId.Millimeters, UnitTypeId.Feet);
            ////#else
            ////         double offsetinternal = unitconverter.millimeterstointernalunits(convert.todouble(offset));
            ////#endif

            CurveLoop CuLoop_Offset = CurveLoop.CreateViaOffset(CuLoop, offsetinternal, nor);

            // nếu offset ngược

            if (CuLoop_Offset.GetExactLength() < CuLoop.GetExactLength())
            {

                CuLoop_Offset = CurveLoop.CreateViaOffset(CuLoop, offsetinternal, -nor);

            }

            return CuLoop_Offset.CurveLoopToCurveArray();

        }

        public static CurveLoop GetCurveLoopOffset(this CurveLoop CuLoop, double offset, XYZ nor)
        {

            double offsetinternal = UnitUtils.Convert(Convert.ToDouble(offset), UnitTypeId.Millimeters, UnitTypeId.Feet);

            CurveLoop CuLoop_Offset = CurveLoop.CreateViaOffset(CuLoop, offsetinternal, nor);

            if (CuLoop_Offset.GetExactLength() < CuLoop.GetExactLength())
            {

                CuLoop_Offset = CurveLoop.CreateViaOffset(CuLoop, offsetinternal, -nor);

            }

            return CuLoop_Offset;

        }

        //lay ve tam cua face
        public static XYZ GetCenterPointOfFace(this PlanarFace face)
        {

            XYZ xyz = new XYZ();

            Mesh mesh = face.Triangulate();

            int count = 0;

            foreach (XYZ vertex in mesh.Vertices)
            {
                xyz += vertex;
                count++;
            }

            XYZ centerpoint = xyz / count;

            return centerpoint;
        }

        //ham lay ve cac planarface cua face
        public static List<PlanarFace> FaceArryToPlanarFaces(this FaceArray FaceArr)
        {
            List<PlanarFace> Flanars = new List<PlanarFace>();
            foreach (var i in FaceArr)
            {

                var Plans = i as PlanarFace;

                if (Plans != null) Flanars.Add(Plans);

            }
            return Flanars;
        }

        public static CurveArray CurveLoopToCurveArray(this CurveLoop CuLoop_Offset)
        {

            CurveArray ListCurves = new CurveArray();

            foreach (Curve item in CuLoop_Offset)
            {

                ListCurves.Append(item);

            }

            return ListCurves;

        }

        public static List<Solid> GetAllSolids(this Element instance, bool transformedSolid = false)
        {
            List<Solid> solidList = new List<Solid>();
            if (instance == null)
                return solidList;

            GeometryElement geometryElement = instance.get_Geometry(new Options()
            {
                ComputeReferences = true
            });

            foreach (GeometryObject geometryObject1 in geometryElement)
            {
                GeometryInstance geometryInstance = geometryObject1 as GeometryInstance;
                if (null != geometryInstance)
                {
                    var tf = geometryInstance.Transform;
                    foreach (GeometryObject geometryObject2 in geometryInstance.GetSymbolGeometry())
                    {
                        Solid solid = geometryObject2 as Solid;
                        if (!(null == solid) && solid.Faces.Size != 0 && solid.Edges.Size != 0)
                        {
                            if (transformedSolid)
                            {
                                solidList.Add(SolidUtils.CreateTransformed(solid, tf));
                            }
                            else
                            {
                                solidList.Add(solid);
                            }
                        }
                    }
                }
                Solid solid1 = geometryObject1 as Solid;
                if (!(null == solid1) && solid1.Faces.Size != 0)
                    solidList.Add(solid1);
            }
            return solidList;

        }
    }
}
