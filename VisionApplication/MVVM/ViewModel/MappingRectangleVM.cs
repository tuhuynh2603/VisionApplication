using ABI.System;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VisionApplication.MVVM.ViewModel
{
    public class MappingRectangleVM : BaseVM
    {
        public int trackID { set; get; }
        private ImageSource imageSource1 = new BitmapImage(new System.Uri(@"/Resources/gray-chip.png", UriKind.Relative));

        public ImageSource imageSource
        {
            get =>  imageSource1;
            set
            {
                imageSource1 = value;
                OnPropertyChanged(nameof(imageSource));

            }
        }

        private double imageWidth1 = 50;

        public double imageWidth
        {
            get => imageWidth1;
            set
            {
                imageWidth1 = value;
                OnPropertyChanged(nameof(imageWidth));

            }
        }

        private double imageHeight1 = 50;

        public double imageHeight
        {
            get => imageHeight1;
            set
            {
                imageHeight1 = value;
                OnPropertyChanged(nameof(imageHeight));

            }
        }

        private object mappingID1 = "1";

        public object mappingID
        {
            get => mappingID1;
            set
            {
                mappingID1 = value;
                OnPropertyChanged(nameof(mappingID));

            }
        }

        private double fontMappingSize1 = 20;

        public double fontMappingSize
        {
            get => fontMappingSize1;
            set
            {
                fontMappingSize1 = value;
                OnPropertyChanged(nameof(fontMappingSize));

            }
        }

        private double minMappingWidth1 = 50;

        public double minMappingWidth
        {
            get => minMappingWidth1;
            set
            {
                minMappingWidth1 = value;
                OnPropertyChanged(nameof(minMappingWidth));

            }
        }

        private double minMappingHeight1 = 50;

        public double minMappingHeight
        {
            get => minMappingHeight1;
            set
            {
                minMappingHeight1 = value;
                OnPropertyChanged(nameof(minMappingHeight));

            }
        }

        private double imageLeft1;

        public double imageLeft
        {
            get => imageLeft1;
            set
            {
                imageLeft1 = value;
                OnPropertyChanged(nameof(imageLeft));

            }
        }

        private double imageTop1;

        public double imageTop
        {
            get => imageTop1;
            set
            {
                imageTop1 = value;
                OnPropertyChanged(nameof(imageTop));

            }
        }

        private double labelLeft1;

        public double labelLeft
        {
            get => labelLeft1;
            set
            {
                labelLeft1 = value;
                OnPropertyChanged(nameof(labelLeft));

            }
        }

        private double labelTop1;

        public double labelTop
        {
            get => labelTop1;
            set
            {
                labelTop1 = value;
                OnPropertyChanged(nameof(labelTop));

            }
        }

        private System.Windows.Visibility isBorderVisible1 = System.Windows.Visibility.Visible;
        public System.Windows.Visibility isBorderVisible
        {
            get => isBorderVisible1;
            set
            {
                isBorderVisible1 = value;
                OnPropertyChanged(nameof(isBorderVisible));
            }
        }

        public MappingRectangleVM()
        {

        }

        private ActionCommand canvasMouseLeftButtonDownCommand;

        public ICommand CanvasMouseLeftButtonDownCommand
        {
            get
            {
                if (canvasMouseLeftButtonDownCommand == null)
                {
                    canvasMouseLeftButtonDownCommand = new ActionCommand(CanvasMouseLeftButtonDown);
                }

                return canvasMouseLeftButtonDownCommand;
            }
        }

        private void CanvasMouseLeftButtonDown()
        {
            isBorderVisible = System.Windows.Visibility.Visible;
            if (MainWindowVM.m_bSequenceRunning)
                return;

            //MainWindowVM.master.m_Tracks[nTrackId].CheckInspectionOnlineThread();
            MainWindowVM.master.m_Tracks[trackID].m_nCurrentClickMappingID = int.Parse(mappingID.ToString()) - 1;

            if (InspectionTabVM.bEnableOfflineInspection)
                Master.m_OfflineTriggerSnapEvent[trackID].Set();
            else
                Master.InspectEvent[trackID].Set();
        }

        private ActionCommand canvasMouseMoveCommand;

        public ICommand CanvasMouseMoveCommand
        {
            get
            {
                if (canvasMouseMoveCommand == null)
                {
                    canvasMouseMoveCommand = new ActionCommand(CanvasMouseMove);
                }

                return canvasMouseMoveCommand;
            }
        }

        private void CanvasMouseMove()
        {
            isBorderVisible = System.Windows.Visibility.Visible;

        }

        private ActionCommand canvasMouseLeaveCommand;

        public ICommand CanvasMouseLeaveCommand
        {
            get
            {
                if (canvasMouseLeaveCommand == null)
                {
                    canvasMouseLeaveCommand = new ActionCommand(CanvasMouseLeave);
                }

                return canvasMouseLeaveCommand;
            }
        }

        private void CanvasMouseLeave()
        {
            isBorderVisible = System.Windows.Visibility.Collapsed;
        }


        private ActionCommand canvasMouseEnterCommand;

        public ICommand CanvasMouseEnterCommand
        {
            get
            {
                if (canvasMouseEnterCommand == null)
                {
                    canvasMouseEnterCommand = new ActionCommand(CanvasMouseEnter);
                }

                return canvasMouseEnterCommand;
            }
        }

        private void CanvasMouseEnter()
        {
            isBorderVisible = System.Windows.Visibility.Visible;
        }



    }
}
