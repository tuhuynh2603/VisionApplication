using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using VisionApplication.Define;
using VisionApplication.MVVM.ViewModel;

namespace VisionApplication.MVVM.View
{
    /// <summary>
    /// Interaction logic for LotBarcodeDataTable.xaml
    /// </summary>
    public partial class LotBarcodeDataTable : System.Windows.Controls.UserControl
    {
        public LotBarcodeDataTable()
        {
            InitializeComponent();
            //this.DataContext = new LotBarcodeDatatableVM();
        }

        private void lvLotBarCodeData_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scv_LotBarcodeDataTableScrollView.ScrollToVerticalOffset(scv_LotBarcodeDataTableScrollView.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void lvLotBarCodeData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lvLotBarCodeData_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void scv_LotBarcodeDataTableScrollView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scv_LotBarcodeDataTableScrollView.ScrollToVerticalOffset(scv_LotBarcodeDataTableScrollView.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
