using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcreteLiningBeam.Model
{
   internal class ElementSortMax : IComparer<CurveLoop>
    {
        int IComparer<CurveLoop>.Compare(CurveLoop poly1, CurveLoop poly2)
        {

            CurveLoop Poly1 = poly1 as CurveLoop;

            CurveLoop Poly2 = poly2 as CurveLoop;

            if (Poly1.GetExactLength() > Poly2.GetExactLength())
            {
                return -1;
            }
            else if (Poly1.GetExactLength() == Poly2.GetExactLength())
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
