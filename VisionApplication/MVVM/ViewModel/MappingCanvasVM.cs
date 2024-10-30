﻿using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VisionApplication.Define;
using VisionApplication.Model;

namespace VisionApplication.MVVM.ViewModel
{
    public class MappingCanvasVM : BaseVM
    {

        private ObservableCollection<MappingRectangleVM> _mappingRectangles;

        public ObservableCollection<MappingRectangleVM> mappingRectangles
        {
            get => _mappingRectangles;
            set
            {
                _mappingRectangles = value;
                OnPropertyChanged(nameof(mappingRectangles));
            }
        }


        public delegate void ResetMappingResultDelegate(int nTrack);
        public static ResetMappingResultDelegate ResetMappingResultDeleagate;

        public delegate void UpdateMappingResultPageDelegate(int nTrack);
        public static UpdateMappingResultPageDelegate updateMappingResultPageDelegate;


        public delegate void UpdateMappingResultDelegate(VisionResultData resultData, int nTrack, int nDeviceID);
        public static UpdateMappingResultDelegate updateMappingResultDelegate;

        public delegate Task InitCanvasMappingDelegateAsync(int n);
        public static InitCanvasMappingDelegateAsync initCanvasMappingDelegateAsync;

        public delegate void SetMappingPageDelegate(int nTrack, int nPage);
        public static SetMappingPageDelegate setMappingPageDelegate;

        public delegate void SetMappingSizeDelegate(double w, double h);
        public static SetMappingSizeDelegate setMappingSizeDelegate;

        public async void SetMappingSize(double w, double h)
        {
            await Task.Run(() =>
            {

                App.Current.Dispatcher.Invoke(() =>
                {
                    MappingCanvasWidth = w;
                    MappingCanvasHeight = h;
                });
            });
        }


        private void SetMappingPage(int nTrack, int nPage) => m_nPageID[nTrack] = nPage;
        public MappingCanvasVM(CatergoryMappingParameters catergoryMappingParameters)
        {
            _catergoryMappingParameters = catergoryMappingParameters;

            ResetMappingResultDeleagate = ResetMappingResult;
            updateMappingResultPageDelegate = UpdateMappingResultPage;
            updateMappingResultDelegate = UpdateMappingResult;
            setMappingPageDelegate = SetMappingPage;
            setMappingSizeDelegate = SetMappingSize;
            initCanvasMappingDelegateAsync = InitCanvasMappingAsync;
        }

        private CatergoryMappingParameters _catergoryMappingParameters { set; get; } = new CatergoryMappingParameters();
        private CatergoryMappingParameters _catergoryMappingParametersTemp { set; get; } = new CatergoryMappingParameters();


        public async Task InitCanvasMappingAsync(int n)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Restart();
            int nMaxDeviceStep = _catergoryMappingParameters.M_NumberDeviceX > _catergoryMappingParameters.M_NumberDeviceY ? _catergoryMappingParameters.M_NumberDeviceX : _catergoryMappingParameters.M_NumberDeviceY;
            double dPage = Math.Ceiling((_catergoryMappingParameters.M_NumberDevicePerLot * 1.0) / (_catergoryMappingParameters.M_NumberDeviceX * 1.0) / _catergoryMappingParameters.M_NumberDeviceY * 1.0);
            m_nNumberMappingPage = (int)dPage;

            int nWidthgrid = MappingCanvasWidth > 0 ? (int)MappingCanvasWidth : 500;

            double m_nWidthMappingRect = (int)(nWidthgrid / nMaxDeviceStep / 2.2);
            if (m_nWidthMappingRect > 100)
                m_nWidthMappingRect = 100;
            if (m_nWidthMappingRect < 25)
                m_nWidthMappingRect = 25;


            
            double m_nStepMappingRect = m_nWidthMappingRect + 1;
            string path = @"/Resources/gray-chip.png";

            List<Task<MappingRectangleVM>> mappingRectangleVMTasks = new List<Task<MappingRectangleVM>>();
            for (int nTrack = 0; nTrack < 2; nTrack++)
            {

                for (int nDeviceX = 0; nDeviceX < _catergoryMappingParameters.M_NumberDeviceX; nDeviceX++)
                {
                    for (int nDeviceY = 0; nDeviceY < _catergoryMappingParameters.M_NumberDeviceY; nDeviceY++)
                    {
                        int nTrackTemp = nTrack;
                        int nDeviceXTemp = nDeviceX;
                        int nDeviceYTemp = nDeviceY;

                        MappingRectangleVM rec = new MappingRectangleVM();
                        //mappingRectangles.Add(rec);
                        mappingRectangleVMTasks.Add(Task.Run(
                        async () =>
                        {
                            //return await Application.Current.Dispatcher.InvokeAsync(() =>
                            //{

                            //    //rec.imageSource = new BitmapImage(new Uri(path, UriKind.Relative));
                            //});
                            rec.trackID = nTrackTemp;
                            rec.imageWidth = 0.95 * m_nWidthMappingRect;
                            rec.imageHeight = 0.95 * m_nWidthMappingRect;
                            rec.mappingID = nDeviceXTemp + 1 + nDeviceYTemp * _catergoryMappingParameters.M_NumberDeviceX + m_nPageID[nTrackTemp] * _catergoryMappingParameters.M_NumberDeviceX * _catergoryMappingParameters.M_NumberDeviceY;
                            rec.fontMappingSize = 0.95 * m_nWidthMappingRect / 3;
                            rec.minMappingWidth = 0.95 * m_nWidthMappingRect;
                            rec.minMappingHeight = 0.95 * m_nWidthMappingRect;
                            rec.imageLeft = m_nStepMappingRect * nDeviceXTemp + nTrackTemp * m_nWidthMappingRect * (_catergoryMappingParameters.M_NumberDeviceX + 1);
                            rec.imageTop = m_nStepMappingRect * nDeviceYTemp;
                            rec.labelLeft = m_nStepMappingRect * nDeviceXTemp + nTrackTemp * m_nWidthMappingRect * (_catergoryMappingParameters.M_NumberDeviceX + 1);
                            rec.labelTop = m_nStepMappingRect * nDeviceYTemp + 0.95 * m_nWidthMappingRect / 9;
                            return rec;
                        }
                        ));



                    }
                }
            }

            Console.WriteLine("Start");
            var result = await Task.WhenAll(mappingRectangleVMTasks);
            Console.WriteLine("End");

            await Task.Run(
                () =>
                {
                    updateMappingFcn(new ObservableCollection<MappingRectangleVM>(result));
                }
                );

            return;
        }

        public void updateMappingFcn(ObservableCollection<MappingRectangleVM> result)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                mappingRectangles = new ObservableCollection<MappingRectangleVM>();
                foreach (var rec in result)
                {
                    mappingRectangles.Add(rec);

                }

                if (MainWindowVM.mainWindowVM != null)
                    MainWindowVM.loadAllStatisticDelegate?.Invoke(false);
            }));

            
        }

        public void UpdateMappingResultPage(int nTrack)
        {


            if (mappingRectangles == null)
                return;

            string path = @"/Resources/gray-chip.png";
            string pathFail = @"/Resources/red-chip.png";
            string pathPass = @"/Resources/green-chip.png";

            int nResultTotal;
            //for(int nTrack = 0; nTrack < 2; nTrack++)
            //{
            int nDevice = _catergoryMappingParameters.M_NumberDeviceX * _catergoryMappingParameters.M_NumberDeviceY;
            for (int nID = 0; nID < _catergoryMappingParameters.M_NumberDeviceX * _catergoryMappingParameters.M_NumberDeviceY;  nID++)
            {
                if (nID >= mappingRectangles.Count())
                    return;

                nResultTotal = MainWindowVM.master.m_Tracks[nTrack].m_VisionResultDatas[nID + m_nPageID[nTrack] * nDevice].m_nResult;

                switch (nResultTotal)
                {
                    case -(int)ERROR_CODE.NOT_INSPECTED:
                        mappingRectangles[nID].imageSource = new BitmapImage(new Uri(path, UriKind.Relative));
                        break;

                    case -(int)ERROR_CODE.PASS:
                        mappingRectangles[nID].imageSource = new BitmapImage(new Uri(pathPass, UriKind.Relative));
                        break;
                    default:
                        mappingRectangles[nID].imageSource = new BitmapImage(new Uri(pathFail, UriKind.Relative));
                        break;
                }

                mappingRectangles[nID].mappingID = (nID + m_nPageID[nTrack] * nDevice + 1).ToString();

            }

            //}

            if (nTrack == 0)
                txtPage1 = (m_nPageID[nTrack] + 1).ToString();
            else
                txtPage2 = (m_nPageID[nTrack] + 1).ToString();


        }



        public void UpdateMappingResult(VisionResultData resultData, int nTrack, int nDeviceID)
        {
            int nDevice = _catergoryMappingParameters.M_NumberDeviceX * _catergoryMappingParameters.M_NumberDeviceY;

            if (nDeviceID >= (m_nPageID[nTrack] + 1) * nDevice)
            {
                m_nPageID[nTrack]++;
                UpdateMappingResultPage(nTrack);
            }

            string path = @"/Resources/gray-chip.png";
            switch (resultData.m_nResult)
            {
                case -(int)ERROR_CODE.NOT_INSPECTED:
                    path = @"/Resources/gray-chip.png";
                    break;

                case -(int)ERROR_CODE.PASS:
                    path = @"/Resources/green-chip.png";
                    break;
                default:
                    path = @"/Resources/red-chip.png";
                    break;
            }
        }


        public void ResetMappingResult(int nTrackID = (int)CAMERATYPE.CAMERA1)
        {
            int nDevice = _catergoryMappingParameters.M_NumberDeviceX * _catergoryMappingParameters.M_NumberDeviceY;

            string path = @"/Resources/gray-chip.png";
            for (int nID = 0; nID < nDevice; nID++)
                mappingRectangles[nID].imageSource = new BitmapImage(new Uri(path, UriKind.Relative));
        }


        private ActionCommand btnPageClickCommand1;

        public ICommand btnPageClickCommand
        {
            get
            {
                if (btnPageClickCommand1 == null)
                {
                    btnPageClickCommand1 = new ActionCommand(btnPageClick);
                }

                return btnPageClickCommand1;
            }
        }

        private void btnPageClick()
        {
            previousPage(0);
        }

        private ActionCommand btnNextPageCommand1;

        public ICommand btnNextPageCommand
        {
            get
            {
                if (btnNextPageCommand1 == null)
                {
                    btnNextPageCommand1 = new ActionCommand(btnNextPage);
                }

                return btnNextPageCommand1;
            }
        }

        private void btnNextPage()
        {
            NextPage(0);

        }

        private void NextPage(int nTrack)
        {
            m_nPageID[nTrack]++;
            if (m_nPageID[nTrack] >= m_nNumberMappingPage)
            {
                m_nPageID[nTrack] = m_nNumberMappingPage - 1;
                return;
            }
            if (nTrack == 0)
                txtPage1 = (m_nPageID[nTrack] + 1).ToString();
            else
                txtPage2 = (m_nPageID[nTrack] + 1).ToString();

            UpdateMappingResultPage(nTrack);

        }

        private ActionCommand btnPage2ClickCommand1;

        public ICommand btnPage2ClickCommand
        {
            get
            {
                if (btnPage2ClickCommand1 == null)
                {
                    btnPage2ClickCommand1 = new ActionCommand(btnPage2Click);
                }

                return btnPage2ClickCommand1;
            }
        }

        private void btnPage2Click()
        {
            previousPage(1);
        }

        public void previousPage(int nTrack)
        {
            m_nPageID[nTrack]--;
            if (m_nPageID[nTrack] < 0)
            {
                m_nPageID[nTrack] = 0;
                return;
            }

            if (nTrack == 0)
                txtPage1 = (m_nPageID[nTrack] + 1).ToString();
            else
                txtPage2 = (m_nPageID[nTrack] + 1).ToString();

            UpdateMappingResultPage(nTrack);
        }

        private ActionCommand btnPage2NextCommand1;

        public ICommand btnPage2NextCommand
        {
            get
            {
                if (btnPage2NextCommand1 == null)
                {
                    btnPage2NextCommand1 = new ActionCommand(btnPage2Next);
                }

                return btnPage2NextCommand1;
            }
        }

        private void btnPage2Next()
        {
            NextPage(1);
        }

        private double borderLeft1;

        public double borderLeft
        {
            get => borderLeft1;
            set
            {
                borderLeft1 = value;
                OnPropertyChanged(nameof(borderLeft));

            }
        }

        private double borderTop1;

        public double borderTop
        {
            get => borderTop1;
            set
            {
                borderTop1 = value;
                OnPropertyChanged(nameof(borderTop));

            }
        }

        private System.Windows.Visibility borderVisible1;

        public System.Windows.Visibility borderVisible {
            get => borderVisible1;
            set
            {
                borderVisible1 = value;
                OnPropertyChanged(nameof(borderVisible));

            }
        }

        private string txtPage11 = "1";

        public string txtPage1
        {
            get => txtPage11;
            set
            {
                txtPage11 = value;
                OnPropertyChanged(nameof(txtPage1));

            }
        }

        private string txtPage21 = "1";
        private int[] m_nPageID { set; get; } = { 0, 0 };
        private int m_nNumberMappingPage;

        public string txtPage2
        {
            get => txtPage21;
            set
            {
                txtPage21 = value;
                OnPropertyChanged(nameof(txtPage2));

            }
        }

        private double mappingCanvasWidth = 200;
        public double MappingCanvasWidth
        {
            get => mappingCanvasWidth;
            set
            {
                mappingCanvasWidth = value;
                OnPropertyChanged(nameof(MappingCanvasWidth));

            }
        }

     private double mappingCanvasHeight = 300;

        int n = 0;
        public double MappingCanvasHeight
        {
            get => mappingCanvasHeight;
            set
            {
                mappingCanvasHeight = value;
                OnPropertyChanged(nameof(MappingCanvasHeight));
                InitCanvasMappingAsync(n++);

            }

        }


    }
}
