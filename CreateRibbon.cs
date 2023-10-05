using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using System;

namespace ConcreteLiningBeam
{
    [Transaction(TransactionMode.Manual)]
    class CreateRibbon : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {


            string nameTab1 = "ConcreteBeam";
            application.CreateRibbonTab(nameTab1);

            //tao panel
            RibbonPanel panel = application.CreateRibbonPanel(nameTab1, "ConcreteBeam");

            //create button
            string path = Assembly.GetExecutingAssembly().Location;
            PushButtonData button = new PushButtonData("ConcreteBeam", "Create", path, "ConcreteLiningBeam.ConcreteLiningBeamCommand");

            //add button to panel
            PushButton btn = panel.AddItem(button) as PushButton;

            //add icon to button
            //Uri uriSoucre = new Uri(@"F:\CV\ThucHanh\Revit-API\CBIM.PNG");
            //BitmapImage image = new BitmapImage(uriSoucre);
            //btn.LargeImage = image;

            return Result.Succeeded;

        }
    }
}
