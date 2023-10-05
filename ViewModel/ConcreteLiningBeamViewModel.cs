using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using QuanLyKho.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ConcreteLiningBeam.Model;
using ConcreteLiningBeam.View;


namespace ConcreteLiningBeam.ViewModel
{
    public class ConcreteLiningBeamViewModel : BaseViewModel
    {

        public UIDocument uidoc;
        private Document doc;

        public ICommand OkCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ConcreteLiningBeamViewModel(UIDocument Uidoc)
        {
            uidoc = Uidoc;
            OkCommand = new RelayCommand(Ok);
            CloseCommand = new RelayCommand(Close);

            FoundationType = new FilteredElementCollector(uidoc.Document).OfClass(typeof(FloorType)).Cast<FloorType>().ToList();
           
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

            if (obj is Window window)
            {
                window.Close();
            }

            if (isSelect)
            {
                doc = uidoc.Document;
                Reference r = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select");
                Element element = doc.GetElement(r);
                if (element is ImportInstance) { MessageBox.Show("Link cad :"+ element.Category.Name); }
                //loc danh sach cac dam
                List<ElementId> selected = uidoc.Selection.GetElementIds().
                         Where(e => uidoc.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming
                         || uidoc.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation).ToList();
                    //if (!selected.Any())
                    //{
                    //    chua toi uu bo loc
                    //    selected = uidoc.Selection.PickObjects(ObjectType.Element).Select(e => doc.GetElement(e).Id).ToList();
                    //}
                    var totalElement = selected.Select(e => uidoc.Document.GetElement(e)).ToList();

                    if (totalElement.Count == 0)
                    {
                        MessageBox.Show("Chưa chọn dầm nào", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                ProgressBarView prog = new ProgressBarView();
                prog.Show();

                try 
                {
                    foreach (Element Ele in totalElement)
                    {
                        if (Ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming && Beam)
                        {

                            ConcreteLiningBeamModel CLM = new ConcreteLiningBeamModel(Ele, this);
                            CLM.Excute();
                            //if (!prog.Create(totalElement.Count, "")) break;

                        }
                        if (Ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation)
                        {

                            ElementFoudation ef = new ElementFoudation(Ele, this);
                            ef.Excute();
                            //if (!prog.create(totalelement.count, "")) break;

                        }                      
                    }
                }
                catch
                {

                }              
                }
                else
                {
                
                    FilteredElementCollector Filter = new FilteredElementCollector(uidoc.Document, uidoc.ActiveView.Id);
                    FilteredElementCollector Filter2 = new FilteredElementCollector(uidoc.Document, uidoc.ActiveView.Id);
                var fram = Filter.OfCategory(BuiltInCategory.OST_StructuralFraming).ToElements().ToList();
                var foud = Filter2.OfCategory(BuiltInCategory.OST_StructuralFoundation).ToElements().ToList();

                List<Element> total = fram.Concat(foud).ToList();

                //ProgressBarView prog = new ProgressBarView();
                //prog.Show();

                try
                {
                    foreach (Element ele in total)
                    {
                        if (ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming && Beam)
                        {
                            ConcreteLiningBeamModel CLM = new ConcreteLiningBeamModel(ele, this);
                            CLM.Excute();
                            //if (!prog.Create(fram.Count, "")) break;

                        }
                        if (ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation && Foundation)
                        {
                            if (ele is Floor)
                            {
                                ElementFoudation CLM = new ElementFoudation(ele, this);
                                CLM.Excute();
                                //if (!prog.Create(fram.Count, "")) break;
                            }
                        }
                    }
                }
                catch
                {

                }
               
            }
             
        }

        private bool allElement = false;

        public bool AllElement
        {
            get
            {
                return allElement;
            }
            set
            {
                allElement = value;

                OnPropertyChanged(nameof(AllElement));
            }
        }
        private double offset = 150;

        private bool isSelect = true;
        public bool IsSelect
        {
            get
            {
                return isSelect;
            }
            set
            {
                isSelect = value;
            }
        }

        private bool beam = true;

        public bool Beam
        {
            get
            {
                return beam;
            }
            set
            {
                beam = value;
            }
        }

        private bool foundation = true;
        public bool Foundation
        {
            get
            {
                return foundation;
            }
            set
            {
                foundation = value;
                OnPropertyChanged();
            }
        }

        public double Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        private FloorType fltype;
        public FloorType Fltype
        {
            get
            {
                return fltype;
            }
            set
            {
                if (Fltype != value)
                {

                    fltype = value;
                    OnPropertyChanged(nameof(fltype));

                }
            }
        }

        public List<FloorType> foundationType;

        public List<FloorType> FoundationType
        {
            get
            {
                return foundationType;
            }
            set
            {
                this.foundationType = value;
            }
        }

    }
}
