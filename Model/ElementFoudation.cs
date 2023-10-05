using Autodesk.Revit.DB;
using ConcreteLiningBeam.Model;
using ConcreteLiningBeam.ViewModel;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcreteLiningBeam.Model;
using Autodesk.Revit.UI;
using RevitApiUtils.WarningUtils;

namespace ConcreteLiningBeam
{

   internal class ElementFoudation
    {
        Element Ele;
        ConcreteLiningBeamViewModel Clbvm;

        public ElementFoudation (Element ele, ConcreteLiningBeamViewModel clbvm)
        {
            Ele = ele;
            Clbvm = clbvm;
        }
        public void Excute()
        {
            List<Solid> solis = (Ele).GetAllSolids().Select(SolidUtils.SplitVolumes).Flatten().Cast<Solid>().ToList();
            //level of element
            Level lev = Ele.GetLevel(Clbvm.uidoc);

            foreach (var soli in solis)
            {

                PlanarFace faceBottom = soli.GetBotFaceVector();
                //Lấy về các đường curve của mặt đáy
                List<CurveLoop> loops = faceBottom.GetCurveLoop();
                //Lấy về đường curve dài nhất
                var maxLoop = loops.OrderByDescending(GetLengthCurveLoop).FirstOrDefault();
                //Lây về facenormal
                XYZ faceNor = faceBottom.FaceNormal;
                //tạo các đường curve và offset ra 1 khoảng
                CurveArray cursArray = maxLoop.GetCurveArrayOffset(Clbvm.Offset, faceNor);   
                
                using (Transaction ts = new Transaction(Clbvm.uidoc.Document))
                {

                    ts.Start("Ok");

                    //Floor btl1 = Floor.Create(Clbvm.uidoc.Document,cursArray, Clbvm.Fltype.Id, lev, true, faceNor);

                    ts.Commit();

                }

            } 
            
        }
        public PlanarFace GetBotFaceOrigin(List<PlanarFace> faces)
        {
            //lay ve face co f}acenormal(0,0,1)sap xep tang dan
            var horizontalFaces = faces.Where(x => x.FaceNormal.IsParallel(XYZ.BasisZ)).OrderBy(x => x.Origin.Z);
            //lay cai dau tien la mat co facebottom
            var bot = horizontalFaces.FirstOrDefault();
            return bot;
        }

        public double GetLengthCurveLoop(CurveLoop loop)
        {

            double length = 0;
            foreach (var l in loop)
            {
                length += l.ApproximateLength;
            }
            return length;

        }

    }

}
