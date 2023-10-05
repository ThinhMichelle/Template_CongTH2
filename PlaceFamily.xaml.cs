using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConcreteLiningBeam
{
    /// <summary>
    /// Interaction logic for PlaceFamily.xaml
    /// </summary>
    public partial class PlaceFamily : Window
    {
       private ObservableCollection<string> collection = new ObservableCollection<string>();
       
        public PlaceFamily()
        {
            InitializeComponent();
            foreach (var item in ClsData_PlaceFamily.lst_block_name)
            {
                Collection.Add(item);
            }
           
          
        }
        public ObservableCollection<string> Collection
        {
            get { return collection; }
            set
            { collection = value; }
        }

        private void cbb_blockname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string PrinterName;
            //var selectedItem = cbb_blockname.SelectedItem as MyPrinter;
            //PrinterName = selectedItem.ComboBoxItemName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClsData_PlaceFamily.layer_block = cbb_blockname.SelectedItem.ToString();
            MessageBox.Show(ClsData_PlaceFamily.layer_block);
        }
    }
    public class MyPrinter : INotifyPropertyChanged
    {
        private string name;
        public string ComboBoxItemName
        {
            get { return name; }
            set
            {
                if (name == value) return;
                name = value;
                OnPropertyChanged("ComboBoxItemName");
            }
        }
        public MyPrinter(string name, string path)
        {
            this.ComboBoxItemName = name;          
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
