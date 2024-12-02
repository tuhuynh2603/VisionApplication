using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using VisionApplication.MVVM.ViewModel;
using static VisionApplication.MVVM.ViewModel.MappingCanvasVM;

namespace VisionApplication.MVVM.View
{
    /// <summary>
    /// Interaction logic for MappingCanvasView.xaml
    /// </summary>
    public partial class MappingCanvasView : System.Windows.Controls.UserControl
    {
        public MappingCanvasView()
        {
            InitializeComponent();
        }

        private async void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //OutputLogVM.AddLineOutputLog($"Start1 {DateTime.Now}");

           // Console.WriteLine("SizeChanged Start");
            await setMappingSizeDelegate.Invoke(e.NewSize.Width, e.NewSize.Height);
          //  Console.WriteLine("SizeChanged End");
          //  OutputLogVM.AddLineOutputLog($"Start2 {DateTime.Now}");

        }
    }
}
