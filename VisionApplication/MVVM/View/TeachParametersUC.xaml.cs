using VisionApplication.Algorithm;
using VisionApplication.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Application = VisionApplication.AppMagnus;
using VisionApplication.MVVM.ViewModel;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid;
using VisionApplication.Model;

namespace VisionApplication.MVVM.View
{
    public partial class TeachParametersUC : System.Windows.Controls.UserControl
    {

        public TeachParametersUC()
        {
            InitializeComponent();
            //DataContext = new TeachParameterVM(service);
        }

    }
}
