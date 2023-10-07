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
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (obj is Window window)
            {
                window.Hide();
                try
                {
                    FilteredElementCollector collector = new FilteredElementCollector(doc);
                    IList<Element> symbols = collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().ToElements();
                    FamilySymbol symbol = null;
                    // load family
                    foreach (FamilySymbol sym in symbols)
                    {
                        if (sym.FamilyName == "DBIM Hanger_Horizontal_Pipe" && sym.Name == "Standard")
                        {
                            symbol = sym as FamilySymbol;
                            ClsData_PlaceFamily.symbol = "exist";
                            break;

                        }
                    }
                    if (ClsData_PlaceFamily.symbol == "Not exist")
                    {
                        Load_Family(commandData);
                    }
                    foreach (var item in ClsData_PlaceFamily.list_PointBlock)
                    {                      
                        Place_Family(commandData,item);

                    }




                }
                catch (Exception ex)
                {

                    System.Windows.MessageBox.Show("Failed to execute the external event.\n" + ex.Message, "Execute Event", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
               
            }
        }

        public void Load_Family(ExternalCommandData commandData)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;          
            Document doc = uidoc.Document;         
            // Find Family 
            using (Transaction t = new Transaction(doc, "Place family"))
            {
                t.Start();

                string filename = ClsData_PlaceFamily.Family_name;
                Family family = null;
                doc.LoadFamily(filename, out family);
                t.Commit();
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
            using (Transaction t = new Transaction(doc, "Place family"))
            {
                t.Start();
                foreach (FamilySymbol sym in symbols)
                {
                    if (sym.FamilyName == ClsData_PlaceFamily.Family_name)
                    {
                        symbol = sym as FamilySymbol;
                        break;
                    }
                }

                if (!symbol.IsActive)
                {
                    symbol.Activate();
                }
                // place family
                FamilyInstance hanger = doc.Create.NewFamilyInstance(p, symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);              
                t.Commit();
            }

        }
    }


}

