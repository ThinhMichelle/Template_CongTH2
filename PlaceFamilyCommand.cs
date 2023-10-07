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
using System.Windows.Controls;
using System.Windows;

namespace ConcreteLiningBeam
{
    [Transaction(TransactionMode.Manual)]
    public class PlaceFamilyCommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ClsData_PlaceFamily.cmData = commandData;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            #region"select linkcad" 
            doc = uidoc.Document;
            Reference r = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select linkcad");
            Element importInst = doc.GetElement(r);
            Options op = uidoc.Application.ActiveUIDocument.Document.Application.Create.NewGeometryOptions();
            GeometryElement geoElem1 = importInst.get_Geometry(op);
            try
            {
                if (geoElem1 != null)
                {
                    foreach (var item in geoElem1)
                    {
                        GeometryInstance instance = item as GeometryInstance;
                        foreach (GeometryObject instObj in instance.SymbolGeometry)
                        {

                            if (instObj.GetType().Name == "GeometryInstance")
                            {
                                GeometryInstance gi_block = instObj as GeometryInstance;
                                // điểm chèn block 
                                Transform transform = gi_block.Transform;
                                XYZ origin = transform.Origin;
                                // layer block 
                                GraphicsStyle gStyle = doc.GetElement(instObj.GraphicsStyleId) as GraphicsStyle;
                                string Layer = gStyle.GraphicsStyleCategory.Name;
                                ClsData_PlaceFamily.list_check.Clear();
                                if (ClsData_PlaceFamily.lst_block_name.Count == 0)
                                {
                                    ClsData_PlaceFamily.lst_block_name.Add(Layer);
                                    ClsData_PlaceFamily.list_block.Add(gi_block);
                                    ClsData_PlaceFamily.list_PointBlock.Add(origin);

                                }
                                else
                                {
                                    foreach (var item1 in ClsData_PlaceFamily.lst_block_name)
                                    {
                                        if (item1 != Layer)
                                        {
                                            ClsData_PlaceFamily.list_check.Add("1");
                                        } else;


                                    }
                                 
                                        if (ClsData_PlaceFamily.lst_block_name.Count == ClsData_PlaceFamily.list_check.Count)
                                        {
                                            ClsData_PlaceFamily.lst_block_name.Add(Layer);
                                        ClsData_PlaceFamily.list_block.Add(gi_block);
                                        
                                        }
                                        ClsData_PlaceFamily.list_PointBlock.Add(origin);

                                }

                               


                            }
                        }

                    }
                }
                var vm = new PlaceFamilyViewModel(uidoc);

                var view = new PlaceFamily() { DataContext = vm };

                view.ShowDialog();

               
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show("Failed to execute the external event.\n" + ex.Message, "Execute Event", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            #endregion

            return Result.Succeeded;
        }
    }
}
