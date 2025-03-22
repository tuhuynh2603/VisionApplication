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
using Xceed.Wpf.Toolkit.Primitives;

namespace VisionApplication.MVVM.ViewModel
{
    public class VisionParameterVM :BaseVM, ICustomUserControl, IParameter
    {

        private DragDropUserControlVM _dragDropVM { get; set; }

        private int selectedPVIAreaIndex = 0;
        public int SelectedPVIAreaIndex
        {
            get => selectedPVIAreaIndex;
            set
            {
                selectedPVIAreaIndex = value;
                OnPropertyChanged(nameof(SelectedPVIAreaIndex));
                categoriesVisionParam = ReloadAreaParameterUI(_SelectedCameraIndex, selectedPVIAreaIndex);

            }
        }


        private int _SelectedCameraIndex;
        public int SelectedCameraIndex
        {
            get => _SelectedCameraIndex;
            set
            {

                OnPropertyChanged(nameof(SelectedCameraIndex));
                categoriesVisionParam = ReloadAreaParameterUI(_SelectedCameraIndex, selectedPVIAreaIndex);
                    // Add logic to handle camera selection change if necessary

            }
        }

        private CategoryVisionParameter _categoriesVisionParam = new CategoryVisionParameter();
        public CategoryVisionParameter categoriesVisionParam
        {
            get => _categoriesVisionParam;
            set
            {
                _categoriesVisionParam = value;
                OnPropertyChanged(nameof(categoriesVisionParam));
            }

        }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand PropertyChangedCommand { set; get; }

        public void RegisterUserControl()
        {
            _dragDropVM.RegisterMoveGrid();
            _dragDropVM.RegisterResizeGrid();

        }


        CategoryVisionParameterRepository categoryVisionParameterRepository { set; get; }



        IParameterService<CategoryVisionParameter> categoryVisionParameterService { set; get; }

        private DatabaseContext _databaseContext { set; get; }

        public VisionParameterVM(DragDropUserControlVM dragDropVM, DatabaseContext databaseContext)
        {
            _dragDropVM = dragDropVM;
            RegisterUserControl();

            _databaseContext = databaseContext;
            categoryVisionParameterRepository = new CategoryVisionParameterRepository(_databaseContext);
            categoryVisionParameterService = new CategoryVisionParameterService(categoryVisionParameterRepository);

            SaveCommand = new RelayCommand<VisionParameterVM>((p) => { return true; },

                                         async (p) =>
                                         {
                                             CategoryVisionParameter param = new CategoryVisionParameter();
                                             param = categoriesVisionParam;
                                             SaveParameterDefault(param, SelectedCameraIndex, selectedPVIAreaIndex);
                                             //var data = await categoryVisionParameterService.GetParameterById(SelectedCameraIndex + 1, selectedPVIAreaIndex + 1);
                                             //if (data != null)
                                             //{
                                             //    try
                                             //    {
                                             //        await categoryVisionParameterService.UpdateParameter(param);

                                             //    }
                                             //    catch (Exception ex) { }
                                             //}
                                             //else
                                             //{
                                             //    try
                                             //    {
                                             //        await categoryVisionParameterService.AddParameter(param);
                                             //    }
                                             //    catch (Exception ex) {
                                                     
                                             //    }

                                             //}
                                         });


            CancelCommand = new RelayCommand<TeachParameterVM>((p) => { return true; },
            (p) =>
            {
                //for (int area = 0; area < Application.TOTAL_AREA; area++)
                //{
                categoriesVisionParam = ReloadAreaParameterUI(SelectedCameraIndex, selectedPVIAreaIndex);
                //}
            });

            InitCategory(SelectedCameraIndex, selectedPVIAreaIndex);
        }

        public delegate void InitCategoryDelegate(int nTrack, int nArea);
        public void InitCategory(int nTrack, int nArea)
        {
            SelectedCameraIndex = nTrack;
            SelectedPVIAreaIndex = nArea;
            //categoriesVisionParam = ReloadAreaParameterUI(nTrack, nArea);

        }



        public bool SaveParameterDefault(CategoryVisionParameter categoryVisionParameter, int nTrack, int nArea)
        {
            try
            {
                //Mouse.OverrideCursor = Cursors.Wait;
                MainWindowVM.master.m_Tracks[nTrack].m_InspectionCore.UpdateAreaParameterFromUIToInspectionCore(categoryVisionParameter, nArea);
                WritePVIAreaParam(nTrack, nArea);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static CategoryVisionParameter ReloadAreaParameterUI(int nTrack, int nArea = 0)
        {
            Dictionary<string, string> dictPVIAreaParam = LoadAreaParamFromFileToDict(nTrack, nArea);
            CategoryVisionParameter categoryVisionParameter = new CategoryVisionParameter();
            bool bSuccess = FileHelper.UpdateParamFromDictToUI<CategoryVisionParameter>(dictPVIAreaParam, categoryVisionParameter);

            return categoryVisionParameter;
        }

        /// <summary>
        /// Model For PropertyGrid
        /// </summary>


        public static Dictionary<string, string> LoadAreaParamFromFileToDict(int nTrack, int nAreaIndex)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (AppMagnus.currentRecipe == null || AppMagnus.pathRecipe == null)
                return dict;

            string strFileName = $"PVIAreaParameters_Track{nTrack + 1}_Area{nAreaIndex + 1}" + ".cfg";
            string pathFile = Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, strFileName);
            IniFile ini = new IniFile(pathFile);

            FileHelper.ReadLine_Magnus(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_AreaEnable)), ini,  dict);
            FileHelper.ReadLine_Magnus(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_lowerThreshold)), ini,  dict);
            FileHelper.ReadLine_Magnus(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_upperThreshold)), ini,  dict);
            FileHelper.ReadLine_Magnus(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_OpeningMask)), ini,  dict);
            FileHelper.ReadLine_Magnus(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_DilationMask)), ini,  dict);
            FileHelper.ReadLine_Magnus(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_ObjectCoverPercent)), ini,  dict);
            FileHelper.ReadLine_Magnus(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_DefectROILocation)), ini,  dict);

            return dict;
            //}
        }

        public void WritePVIAreaParam(int nTrack, int nAreaIndex)
        {


            string strFileName = $"PVIAreaParameters_Track{nTrack + 1}_Area{nAreaIndex + 1}" + ".cfg";
            string pathFile = Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, strFileName);

            string strDateTime = string.Format("({0}.{1}.{2}_{3}.{4}.{5})", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"), DateTime.Now.ToString("ss"));
            string backup_path = Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe, "Backup_Teach Parameter");
            if (!Directory.Exists(backup_path))
                Directory.CreateDirectory(backup_path);

            string backup_fullpath = Path.Combine(backup_path, $"PVIAreaParameters_Track{nTrack + 1}_Area{nAreaIndex + 1} {strDateTime}" + ".cfg");
            FileInfo file = new FileInfo(pathFile);

            if (!file.Exists)
                file.Create();

            file.MoveTo(backup_fullpath);
            file.Create();

            IniFile ini = new IniFile(pathFile);
            InspectionCore inspectionCore = MainWindowVM.master.m_Tracks[nTrack].m_InspectionCore;
            FileHelper.WriteLine(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_AreaEnable)), ini, inspectionCore.m_SurfaceDefectParameter[nAreaIndex].m_DR_AreaEnable.ToString());
            FileHelper.WriteLine(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_lowerThreshold)), ini, inspectionCore.m_SurfaceDefectParameter[nAreaIndex].m_LD_lowerThreshold.ToString());
            FileHelper.WriteLine(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_upperThreshold)), ini, inspectionCore.m_SurfaceDefectParameter[nAreaIndex].m_LD_upperThreshold.ToString());
            FileHelper.WriteLine(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_OpeningMask)), ini, inspectionCore.m_SurfaceDefectParameter[nAreaIndex].m_LD_OpeningMask.ToString());
            FileHelper.WriteLine(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_DilationMask)), ini, inspectionCore.m_SurfaceDefectParameter[nAreaIndex].m_LD_DilationMask.ToString());
            FileHelper.WriteLine(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_ObjectCoverPercent)), ini, inspectionCore.m_SurfaceDefectParameter[nAreaIndex].m_LD_ObjectCoverPercent.ToString());
            FileHelper.WriteLine(CategoryVisionParameter.CAETEGORYORDER_LABEL_DEFECT, ExceedToolkit.GetDisplayName<CategoryVisionParameter>(nameof(CategoryVisionParameter.LD_DefectROILocation)), ini, TypeConverterHelper.ConvertRectanglesToString(inspectionCore.m_SurfaceDefectParameter[nAreaIndex].m_DR_DefectROILocations));

            var buffer = new VectorOfByte();
            var bufferTempplate = new VectorOfByte();

            categoriesVisionParam.cameraID = nTrack + 1;
            categoriesVisionParam.areaID = nAreaIndex + 1;
            categoriesVisionParam.dateChanged = DateTime.Now;

        }

    }
}
