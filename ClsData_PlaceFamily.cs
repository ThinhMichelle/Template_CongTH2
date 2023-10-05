using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcreteLiningBeam
{
    public class ClsData_PlaceFamily
    {
        public static List<string> lst_block_name = new List<string>();
        public static List<XYZ> list_PointBlock = new List<XYZ>();
        public static string layer_block;
        public static List<GeometryInstance> list_block = new List<GeometryInstance>();
        public static List<string> list_check = new List<string>();
        public static ExternalCommandData cmData;
    }
}
