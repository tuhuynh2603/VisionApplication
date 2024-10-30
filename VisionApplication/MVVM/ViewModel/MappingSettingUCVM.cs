﻿using Microsoft.Xaml.Behaviors.Core;
using System;

using System.Windows.Input;
using System.Windows;
using VisionApplication.Model;
using VisionApplication.Helper;
using System.IO;
using VisionApplication.Model;
using static VisionApplication.MVVM.ViewModel.MappingCanvasVM;


namespace VisionApplication.MVVM.ViewModel
{
    public class MappingSetingUCVM : BaseVM, ICustomUserControl
    {
        public MainWindowVM _mainWindowVM { get; set; }
        private DragDropUserControlVM _dragDropVM { set; get; }
        public void RegisterUserControl()
        {
            _dragDropVM.RegisterMoveGrid();
            _dragDropVM.RegisterResizeGrid();
        }
        public MappingSetingUCVM(DragDropUserControlVM dragDropVM, MainWindowVM mainWindowVM, DatabaseContext databaseContext)
        {
            _mainWindowVM = mainWindowVM;
            _dragDropVM = dragDropVM;
            RegisterUserControl();
            //categoriesMappingParam = Application.categoriesMappingParam;
        }




        public static Dictionary<string, string> LoadMappingParamFromFile()
        {
            Dictionary<string, string> dictParam = new Dictionary<string, string>();

            string pathFile = Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "MappingParameters.cfg");
            IniFile ini = new IniFile(pathFile);


            FileHelper.ReadLine_Magnus(CatergoryMappingParameters.CategoryMappingOrder, ExceedToolkit.GetDisplayName<CatergoryMappingParameters>(nameof(CatergoryMappingParameters.M_NumberDeviceX)), ini, dictParam);
            FileHelper.ReadLine_Magnus(CatergoryMappingParameters.CategoryMappingOrder, ExceedToolkit.GetDisplayName<CatergoryMappingParameters>(nameof(CatergoryMappingParameters.M_NumberDeviceY)), ini, dictParam);
            FileHelper.ReadLine_Magnus(CatergoryMappingParameters.CategoryMappingOrder, ExceedToolkit.GetDisplayName<CatergoryMappingParameters>(nameof(CatergoryMappingParameters.M_NumberDevicePerLot)), ini, dictParam);
            return dictParam;
        
        }

        public void WriteMappingParam()
        {
            string pathFile = Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "MappingParameters.cfg");
            IniFile ini = new IniFile(pathFile);
            FileHelper.WriteLine(CatergoryMappingParameters.CategoryMappingOrder, ExceedToolkit.GetDisplayName<CatergoryMappingParameters>(nameof(CatergoryMappingParameters.M_NumberDeviceX)), ini, categoriesMappingParam.M_NumberDeviceX.ToString());
            FileHelper.WriteLine(CatergoryMappingParameters.CategoryMappingOrder, ExceedToolkit.GetDisplayName<CatergoryMappingParameters>(nameof(CatergoryMappingParameters.M_NumberDeviceY)), ini, categoriesMappingParam.M_NumberDeviceY.ToString());
            FileHelper.WriteLine(CatergoryMappingParameters.CategoryMappingOrder, ExceedToolkit.GetDisplayName<CatergoryMappingParameters>(nameof(CatergoryMappingParameters.M_NumberDevicePerLot)), ini, categoriesMappingParam.M_NumberDevicePerLot.ToString());
        }




        private CatergoryMappingParameters _categoriesMappingParam = new CatergoryMappingParameters();
        public CatergoryMappingParameters categoriesMappingParam
        {
            set
            {
                _categoriesMappingParam = value;
                OnPropertyChanged(nameof(categoriesMappingParam));
            }
            get => _categoriesMappingParam;
        }





        private ActionCommand saveClick;

        public ICommand SaveClick
        {
            get
            {
                if (saveClick == null)
                {
                    saveClick = new ActionCommand(PerformSaveClick);
                }

                return saveClick;
            }
        }

        private void PerformSaveClick()
        {
            SaveParameterMappingDefault();
            initCanvasMappingDelegateAsync?.Invoke(0);
        }

        private ActionCommand cancelClick;

        public ICommand CancelClick
        {
            get
            {
                if (cancelClick == null)
                {
                    cancelClick = new ActionCommand(PerformCancelClick);
                }

                return cancelClick;
            }
        }

        private void PerformCancelClick()
        {
            //UpdateMappingParamFromDictToUI(Application.dictMappingParam);
            _mainWindowVM.mMappingSettingUCVM.isVisible = Visibility.Collapsed;
        }


        public bool UpdateMappingParamFromDictToUI(Dictionary<string, string> dictParam)
        {
            bool bSuccess = FileHelper.UpdateParamFromDictToUI<CatergoryMappingParameters>(dictParam, categoriesMappingParam);
            return bSuccess;
        }

        public bool SaveParameterMappingDefault()
        {
            try
            {
                //Mouse.OverrideCursor = Cursors.Wait;
                //UpdateDictionaryParam();
                //MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                WriteMappingParam();
                //Dictionary<string,string> dict =  LoadMappingParamFromFile();
                //UpdateMappingParamFromDictToUI(dict);
                _mainWindowVM.mMappingSettingUCVM.isVisible = Visibility.Collapsed;
                //Mouse.OverrideCursor = null;
                //mainWindow.btn_mapping_parameters.IsChecked = false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}
