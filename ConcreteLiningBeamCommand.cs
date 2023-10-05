using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using ConcreteLiningBeam.ViewModel;
using ConcreteLiningBeam.View;

namespace ConcreteLiningBeam
{
    [Transaction(TransactionMode.Manual)]
    public class ConcreteLiningBeamCommand : IExternalCommand
    {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            var vm = new ConcreteLiningBeamViewModel(uidoc);

            var view = new ConcrteLiningBeamView() { DataContext = vm };

            view.ShowDialog();

            return Result.Succeeded;

        }
    }
}
