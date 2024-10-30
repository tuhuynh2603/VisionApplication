
using System.Windows.Input;


namespace VisionApplication.MVVM.View
{
    /// <summary>
    /// Interaction logic for DefectInfor.xaml
    /// </summary>
    public partial class StepDebugView : System.Windows.Controls.UserControl
    {

        public StepDebugView()
        {
            InitializeComponent();
        }
        private void lvDefect_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //ScrollViewer scv = (ScrollViewer)sender;
            scv_StepDebugScrollView.ScrollToVerticalOffset(scv_StepDebugScrollView.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void scv_StepDebugScrollView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scv_StepDebugScrollView.ScrollToVerticalOffset(scv_StepDebugScrollView.VerticalOffset - e.Delta);
            e.Handled = true;
        }


    }
}
