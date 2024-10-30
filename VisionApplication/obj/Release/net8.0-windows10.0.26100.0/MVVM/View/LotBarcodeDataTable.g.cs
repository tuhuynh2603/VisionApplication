﻿#pragma checksum "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "FDC140BF2D0CAACB9A8923B1FA097C982730DC9C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Xaml.Behaviors;
using Microsoft.Xaml.Behaviors.Core;
using Microsoft.Xaml.Behaviors.Input;
using Microsoft.Xaml.Behaviors.Layout;
using Microsoft.Xaml.Behaviors.Media;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using VisionApplication.MVVM.Behaviors;
using VisionApplication.MVVM.ViewModel;


namespace VisionApplication.MVVM.View {
    
    
    /// <summary>
    /// LotBarcodeDataTable
    /// </summary>
    public partial class LotBarcodeDataTable : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 23 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DatePicker_Date;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ccb_LotSelected_ComboBox;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Load_Lot;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Send_To_Server;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer scv_LotBarcodeDataTableScrollView;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvLotBarCodeData;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VisionApplication;component/mvvm/view/lotbarcodedatatable.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.DatePicker_Date = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 2:
            this.ccb_LotSelected_ComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.btn_Load_Lot = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.btn_Send_To_Server = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.scv_LotBarcodeDataTableScrollView = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 97 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
            this.scv_LotBarcodeDataTableScrollView.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.scv_LotBarcodeDataTableScrollView_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 6:
            this.lvLotBarCodeData = ((System.Windows.Controls.ListView)(target));
            
            #line 100 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
            this.lvLotBarCodeData.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lvLotBarCodeData_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 101 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
            this.lvLotBarCodeData.FocusableChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.lvLotBarCodeData_FocusableChanged);
            
            #line default
            #line hidden
            
            #line 101 "..\..\..\..\..\MVVM\View\LotBarcodeDataTable.xaml"
            this.lvLotBarCodeData.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.lvLotBarCodeData_PreviewMouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

