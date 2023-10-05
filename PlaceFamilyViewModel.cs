using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using QuanLyKho.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ConcreteLiningBeam.Model;
using ConcreteLiningBeam.View;

namespace ConcreteLiningBeam
{
    public class PlaceFamilyViewModel : BaseViewModel
    {
        public UIDocument uidoc;
        private Document doc;
      

        public ICommand OkCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public PlaceFamilyViewModel(UIDocument Uidoc)
        {
            uidoc = Uidoc;
            OkCommand = new RelayCommand(Ok);
            CloseCommand = new RelayCommand(Close);
        
        }
        protected void Close(object obj)
        {

            if (obj is Window window)
            {
                window.Close();
            }

        }
        protected void Ok(object obj)
        {
            ExternalCommandData commandData = ClsData_PlaceFamily.cmData;

            if (obj is Window window)
            {
                window.Hide();
                try
                {
                    foreach (var item in ClsData_PlaceFamily.list_PointBlock)
                    {
                        Place_Family(commandData, item);
                    }
                    
                  
                      
                }
                catch (Exception ex)
                {

                    System.Windows.MessageBox.Show("Failed to execute the external event.\n" + ex.Message, "Execute Event", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
               
            }
        }

        public void Place_Family(ExternalCommandData commandData, XYZ p)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;          
            Document doc = uidoc.Document;
            FamilySymbol symbol = null;
            // Find Family 
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> symbols = collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().ToElements();

           

            if (!symbol.IsActive)
            {
                symbol.Activate();
            }
            // load family
            string filename = "D:\\TeraBoxDownload\\Tools\\DBIM Tools\\Family_HangerDBIM Hanger_Horizontal_Pipe.rfa";
            Family family = null;
            doc.LoadFamily(filename, out family);
            foreach (FamilySymbol sym in symbols)
            {
                if (sym.FamilyName == "DBIM Hanger_Horizontal_Pipe" && sym.Name == "Standard")
                {
                    symbol = sym as FamilySymbol;
                    break;
                }
            }
            // place family
            FamilyInstance hanger = doc.Create.NewFamilyInstance(p, symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

        }
    }


}

