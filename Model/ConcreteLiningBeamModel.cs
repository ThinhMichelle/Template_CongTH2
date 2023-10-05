using Autodesk.Revit.DB;
using ConcreteLiningBeam.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using RevitApiUtils.WarningUtils;

namespace ConcreteLiningBeam.Model
{
 internal class ConcreteLiningBeamModel
    {

        public Element Ele;

        public ConcreteLiningBeamViewModel Clbvm;

        //Constructor tạo các dữ liệu ban đầu cho đối tượng 
        public ConcreteLiningBeamModel(Element ele,ConcreteLiningBeamViewModel clbvm)
        {

            Ele = ele;
            Clbvm = clbvm;

        }

        public void Excute()
        {
            //List solid lay cho dam
            List<Solid> solis = (Ele as FamilyInstance).GetAllSolids().Select(SolidUtils.SplitVolumes).Flatten().Cast<Solid>().ToList();

            //level of element
            Level lev = Ele.GetLevel(Clbvm.uidoc);

            foreach (var soli in solis)
            {

                //get planarface 
                List<PlanarFace> faces = soli.Faces.Flatten().Where(x => x is PlanarFace).Cast<PlanarFace>().ToList();

                //get bottom
                var faceBottom = GetBotFaceOrigin(faces);

               //Get curve bottom
                var cls = faceBottom.GetEdgesAsCurveLoops();

                // lấy đương curve max nhất: more linq
                var cl = cls.OrderByDescending(x => x.GetExactLength()).FirstOrDefault();

                //get facenormal
                XYZ faceNor = faceBottom.FaceNormal;

                //ve cac duong curve 
                CurveLoop cursArray = cl.GetCurveLoopOffset(Clbvm.Offset, faceNor);

                SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);            
                //Xay dung hinh khoi
                //CreateExtrusionGeometry : extrude các curve loop
                Solid extrusionSoild = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { cursArray }, faceNor, 0.0032808399, options);

                try
                {

                    //lay ve location curce
                    //Check not null roi moi chay tiep

                    var line = (Ele.Location as LocationCurve)?.Curve as Line;

                    //lay ve toa do cua line

                    var beamDir = line.Direction;

                    // điểm chính giữa mặt phẳng

                    XYZ center = faceBottom.GetCenterPointOfFace();

                    foreach (Curve cur in cl)
                    {
                        //nếu direction của cur  không song song vs direction của dầm
                        if ((cur as Line).Direction.IsParallel(beamDir) == false)
                        {

                            //normal = toạ độ của curve nhân có hướng với basisz
                            XYZ normal = (cur as Line).Direction.CrossProduct(XYZ.BasisZ);

                            XYZ vector = center - (cur.GetEndPoint(0) + cur.GetEndPoint(1)) / 2;

                            double dot = normal.DotProduct(vector);

                            if (dot < 0)
                            {
                                normal = -normal;
                            }

                            var plane = Plane.CreateByNormalAndOrigin(normal, (cur.GetEndPoint(0) + cur.GetEndPoint(1)) / 2);

                            BooleanOperationsUtils.CutWithHalfSpaceModifyingOriginalSolid(extrusionSoild, plane);

                        }

                    }

                    //Get planarface of extrusionSolid
                    var newFaces = extrusionSoild.Faces.FaceArryToPlanarFaces();

                    //get new bottomface of new face
                    var newBotFace = GetBotFaceOrigin(newFaces);

                    //get length of beam
                    CurveLoop newCl = newBotFace.GetEdgesAsCurveLoops().OrderByDescending(x => x.GetExactLength()).FirstOrDefault();

                    //create curarray for floor
                    var curarry = newCl.CurveLoopToCurveArray();

                    using(Transaction ts = new Transaction(Clbvm.uidoc.Document))
                    {

                        ts.Start("ok");

                        DeleteWarningSuper waringsuper = new DeleteWarningSuper();

                        FailureHandlingOptions failOpt = ts.GetFailureHandlingOptions();

                        failOpt.SetFailuresPreprocessor(waringsuper);

                        ts.SetFailureHandlingOptions(failOpt);

                        Floor btl = null;

                        //btl = Clbvm.uidoc.Document.Create.NewFloor(curarry, Clbvm.Fltype, lev, true, faceNor);

                        ts.Commit();

                    }                 
                    // foreach (Curve c in curarry)
                    // {
                    //    var sk = SketchPlane.Create(AC.Document, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, c.SP()));
                    //    AC.Document.Create.NewModelCurve(c, sk);
                    // }
                    //ElementTransformUtils.MoveElement(AC.Document,btl.Id,XYZ.BasisZ*);
                }
                catch
                {
                    
                }
            }

        }

        public PlanarFace GetBotFaceOrigin(List<PlanarFace> faces)
        {
            //lay ve face co facenormal(0,0,1)sap xep tang dan
            var horizontalFaces = faces.Where(x => x.FaceNormal.IsParallel(XYZ.BasisZ)).OrderBy(x => x.Origin.Z);
            //lay cai dau tien la mat co facebottom
            var bot = horizontalFaces.FirstOrDefault();
            return bot;
        }
       
    }
}
