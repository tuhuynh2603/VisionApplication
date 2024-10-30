
using System.Windows.Controls;

namespace VisionApplication.MVVM.View
{
    /// <summary>
    /// Interaction logic for MappingSetingUC.xaml
    /// </summary>
    /// 
    public partial class MappingSetingUC : System.Windows.Controls.UserControl
    {

        //private Dictionary<string, string> _dictMappingParam = new Dictionary<string, string>();
        public MappingSetingUC()
        {
            InitializeComponent();

        }


        //private Rectangles GetRectangles(string str)
        //{
        //    if (str == "")
        //        return new Rectangles(0, 0, 0, 0);
        //    string[] value = str.Split(':');
        //    if (value.Length == 4)
        //        return new Rectangles(double.Parse(value[0]), double.Parse(value[1]), double.Parse(value[2]), double.Parse(value[3]));
        //    else
        //        return new Rectangles(double.Parse(value[0]), double.Parse(value[1]), double.Parse(value[2]), double.Parse(value[3]), double.Parse(value[4]));

        //}
        //private List<int> ConverStringToList(string str)
        //{
        //    List<int> list = new List<int>();
        //    string[] value = str.Split(':');
        //    foreach (string s in value)
        //    {
        //        list.Add(int.Parse(s));
        //    }
        //    return list;
        //}

    }
}
