using Emgu.CV.Util;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using VisionApplication.Algorithm;
using VisionApplication.Helper;
using VisionApplication.Interface;
using VisionApplication.Model;
using VisionApplication.Model;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Application = VisionApplication.AppMagnus;

namespace VisionApplication.MVVM.ViewModel
{
    public class TeachParameterVM : BaseVM, ICustomUserControl, IParameter
    {


        public int nDefectROIIndex = 0;

        private int _SelectedCameraIndex = -1;
        public int SelectedCameraIndex
        {
            get => _SelectedCameraIndex;
            set
            {
                //if (_SelectedCameraIndex != value)
                //{
                    _SelectedCameraIndex = value;
                    categoriesTeachParam = ReloadTeachParameterUI(SelectedCameraIndex);
                    OnPropertyChanged(nameof(SelectedCameraIndex));
                    // Add logic to handle camera selection change if necessary
               // }
            }
        }



        private CategoryTeachParameter _categoriesTeachParam = new CategoryTeachParameter();
        public CategoryTeachParameter categoriesTeachParam
        {
            get => _categoriesTeachParam;
            set
            {
                _categoriesTeachParam = value;
                OnPropertyChanged(nameof(categoriesTeachParam));
            }
        }

        private double _TeachParameterHeight = 400;
        public double TeachParameterHeight
        {
            get => _TeachParameterHeight;
            set
            {
                _TeachParameterHeight = value;
                OnPropertyChanged(nameof(TeachParameterHeight));
            }
        }


        private double _scrollViewerHeight;
        public double scrollViewerHeight
        {
            get => _scrollViewerHeight;
            set
            {
                _scrollViewerHeight = value;
                OnPropertyChanged(nameof(scrollViewerHeight));
            }
        }


        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand PropertyChangedCommand { set; get; }

        CategoryTeachParameterRepository categoryTeachParameterRepository { set; get; }
        IParameterService<CategoryTeachParameter> categoryTeachParameterService { set; get; }
        private DatabaseContext _databaseContext { set; get; }


        private DragDropUserControlVM _dragDropVM { get; set; }
        public void RegisterUserControl()
        {
            _dragDropVM.RegisterMoveGrid();
            _dragDropVM.RegisterResizeGrid();
        }

        public TeachParameterVM(DragDropUserControlVM dragDropVM, DatabaseContext databaseContext)
        {
            _dragDropVM = dragDropVM;
            RegisterUserControl();
            scrollViewerHeight = _dragDropVM.GridHeight;

            _databaseContext = databaseContext;
            categoryTeachParameterRepository = new CategoryTeachParameterRepository(_databaseContext);
            categoryTeachParameterService = new CategoryTeachParameterService(categoryTeachParameterRepository);

            SaveCommand = new RelayCommand<TeachParameterVM>((p) => { return true; },
                                         async (p) =>
                                         {

                                             SaveTeachParameter();
                                             //var data = await categoryTeachParameterService.GetParameterById(SelectedCameraIndex + 1);
                                             //if (data != null)
                                             //{
                                             //    await categoryTeachParameterService.UpdateParameter(categoriesTeachParam);
                                             //}
                                             //else
                                             //    await categoryTeachParameterService.AddParameter(categoriesTeachParam);
                                         });

            CancelCommand = new RelayCommand<TeachParameterVM>((p) => { return true; },
                             (p) =>
                             {
                                 categoriesTeachParam = ReloadTeachParameterUI(SelectedCameraIndex);
                             });

            InitCategory(0);
        }

        public void InitCategory(int nTrack)
        {
            SelectedCameraIndex = nTrack;
            categoriesTeachParam = ReloadTeachParameterUI(SelectedCameraIndex);
        }

        public bool SaveTeachParameter()
        {
            try
            {
                //Mouse.OverrideCursor = Cursors.Wait;
                
                MainWindowVM.master.m_Tracks[SelectedCameraIndex].m_InspectionCore.UpdateTeachParamFromUIToInspectionCore(categoriesTeachParam, SelectedCameraIndex);
                MainWindowVM.master.WriteTeachParam(SelectedCameraIndex);



                //Mouse.OverrideCursor = null;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static CategoryTeachParameter ReloadTeachParameterUI(int nTrack)
        {
            Dictionary<string, string> dictTeachParam;

            dictTeachParam = LoadTeachParamFromFileToDict(nTrack);
            CategoryTeachParameter category = new CategoryTeachParameter();
            FileHelper.UpdateParamFromDictToUI(dictTeachParam, category);
            return category;

        }
        //public class AreaComboBox : IItemsSource
        //{
        //    Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection m_ComboBox_Area = new Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection();

        //    public Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection GetValues()
        //    {
        //        m_ComboBox_Area.Add("Area 1");
        //        m_ComboBox_Area.Add("Area 2");
        //        m_ComboBox_Area.Add("Area 3");
        //        m_ComboBox_Area.Add("Area 4");
        //        m_ComboBox_Area.Add("Area 5");

        //        return m_ComboBox_Area;
        //    }
        //}


        public static Dictionary<string, string> LoadTeachParamFromFileToDict(int nTrack)
        {
            if (AppMagnus.currentRecipe == null || AppMagnus.pathRecipe == null)
                return null;


            Dictionary<string, string> dictTeachParam = new Dictionary<string, string>();
            string strFileName = "TeachParameters_Track" + (nTrack + 1).ToString() + ".cfg";
            string pathFile = Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, strFileName);
            IniFile ini = new IniFile(pathFile);

            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_DeviceLocationRoi)), ini, dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_LocationEnable)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_ThresholdType)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_ObjectColor)), ini, dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_lowerThreshold)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_upperThreshold)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_lowerThresholdInnerChip)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_upperThresholdInnerChip)), ini,  dictTeachParam);

            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_OpeningMask)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_DilationMask)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_MinWidthDevice)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_MinHeightDevice)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_TemplateRoi)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_NumberSide)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_ScaleImageRatio)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_MinScore)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_CornerIndex)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_NumberROILocation)), ini,  dictTeachParam);

            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_EnableCheck)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_lowerThreshold)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_upperThreshold)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_OpeningMask)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_DilationMask)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_MinWidthDevice)), ini,  dictTeachParam);
            FileHelper.ReadLine_Magnus(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_MinHeightDevice)), ini,  dictTeachParam);
            return dictTeachParam;
        }

        public void WriteTeachParam(int nTrack)
        {

            string strFileName = "TeachParameters_Track" + (nTrack + 1).ToString() + ".cfg";
            string pathFile = Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, strFileName);

            string strDateTime = string.Format("({0}.{1}.{2}_{3}.{4}.{5})", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"), DateTime.Now.ToString("ss"));
            string backup_path = Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "Backup_Teach Parameter");
            if (!Directory.Exists(backup_path))
                Directory.CreateDirectory(backup_path);

            string backup_fullpath = Path.Combine(backup_path, $"TeachParameters_Track{nTrack + 1} {strDateTime}" + ".cfg");
            FileInfo file = new FileInfo(pathFile);

            if (!file.Exists)
                file.Create();

            file.MoveTo(backup_fullpath);
            file.Create();

            IniFile ini = new IniFile(pathFile);
            InspectionCore inspectionCore = MainWindowVM.master.m_Tracks[nTrack].m_InspectionCore;

            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_DeviceLocationRoi)), ini, TypeConverterHelper.ConvertRectanglesToString(inspectionCore.m_DeviceLocationParameter.m_L_DeviceLocationRoi));
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_LocationEnable)), ini, inspectionCore.m_DeviceLocationParameter.m_L_LocationEnable.ToString());

            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_ThresholdType)), ini, inspectionCore.m_DeviceLocationParameter.m_L_ThresholdType.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_ObjectColor)), ini, inspectionCore.m_DeviceLocationParameter.m_L_ObjectColor.ToString());

            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_lowerThreshold)), ini, inspectionCore.m_DeviceLocationParameter.m_L_lowerThreshold.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_upperThreshold)), ini, inspectionCore.m_DeviceLocationParameter.m_L_upperThreshold.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_lowerThresholdInnerChip)), ini, inspectionCore.m_DeviceLocationParameter.m_L_lowerThresholdInnerChip.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_upperThresholdInnerChip)), ini, inspectionCore.m_DeviceLocationParameter.m_L_upperThresholdInnerChip.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_OpeningMask)), ini, inspectionCore.m_DeviceLocationParameter.m_L_OpeningMask.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_DilationMask)), ini, inspectionCore.m_DeviceLocationParameter.m_L_DilationMask.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_MinWidthDevice)), ini, inspectionCore.m_DeviceLocationParameter.m_L_MinWidthDevice.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_MinHeightDevice)), ini, inspectionCore.m_DeviceLocationParameter.m_L_MinHeightDevice.ToString());

            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_TemplateRoi)), ini, TypeConverterHelper.ConvertRectanglesToString(inspectionCore.m_DeviceLocationParameter.m_L_TemplateRoi));
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_ScaleImageRatio)), ini, inspectionCore.m_DeviceLocationParameter.m_L_ScaleImageRatio.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_MinScore)), ini, inspectionCore.m_DeviceLocationParameter.m_L_MinScore.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_CornerIndex)), ini, inspectionCore.m_DeviceLocationParameter.m_L_CornerIndex.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_LOCATION, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.L_NumberROILocation)), ini, inspectionCore.m_DeviceLocationParameter.m_DR_NumberROILocation.ToString());


            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_EnableCheck)), ini, inspectionCore.m_blackChipParameter.m_OC_EnableCheck.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_lowerThreshold)), ini, inspectionCore.m_blackChipParameter.m_OC_lowerThreshold.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_upperThreshold)), ini, inspectionCore.m_blackChipParameter.m_OC_upperThreshold.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_OpeningMask)), ini, inspectionCore.m_blackChipParameter.m_OC_OpeningMask.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_DilationMask)), ini, inspectionCore.m_blackChipParameter.m_OC_DilationMask.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_MinWidthDevice)), ini, inspectionCore.m_blackChipParameter.m_OC_MinWidthDevice.ToString());
            FileHelper.WriteLine(CategoryTeachParameter.CATEGORY_OPPOSITE_CHIP, ExceedToolkit.GetDisplayName<CategoryTeachParameter>(nameof(CategoryTeachParameter.OC_MinHeightDevice)), ini, inspectionCore.m_blackChipParameter.m_OC_MinHeightDevice.ToString());




            if (MainWindowVM.master.m_Tracks[nTrack].m_InspectionCore.m_TemplateImage.Gray == null)
                return;

            var buffer = new VectorOfByte();
            var bufferTempplate = new VectorOfByte();
            CvInvoke.Imencode(".jpg", MainWindowVM.master.m_Tracks[nTrack].m_InspectionCore.m_TeachImage.Gray, buffer);
            CvInvoke.Imencode(".jpg", MainWindowVM.master.m_Tracks[nTrack].m_InspectionCore.m_TemplateImage.Gray, bufferTempplate);

            categoriesTeachParam.teachImage = buffer.ToArray();
            categoriesTeachParam.templateImage = bufferTempplate.ToArray();
            categoriesTeachParam.cameraID = nTrack + 1;
            categoriesTeachParam.dateChanged = DateTime.Now;

        }



        /// <summary>
        /// Model For PropertyGrid
        /// </summary>


    }
}
