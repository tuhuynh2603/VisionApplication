
using System.Windows;
using System.Windows.Input;
namespace VisionApplication.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static MainWindow mainWindow;
        //public delegate void StateWindow(WindowState state);
        //public static StateWindow changeStateWindow;

        //List<KeyBinding> hotkey = new List<KeyBinding>();


        //public static string BaseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            //DataContext = new ViewModel.MainWindowVM();                           
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)  // Only allow left-click drag                     
            {
                this.DragMove();  // Moves the window
            }
        }
    }
}
