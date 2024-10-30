﻿using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Input;
using VisionApplication.Helper;
using Application = VisionApplication.AppMagnus;
using UserControl = System.Windows.Controls.UserControl;

namespace VisionApplication.MVVM.ViewModel
{

    public class SerialCommunicationVM : BaseVM, ICustomUserControl
    {
        public ICommand DrowDownListCommCommand { get; set; }
        public ICommand DrowDownListBauRateCommand { get; set; }
        public ICommand btn_ConnectSerial_Click { get; set; }
        public ICommand btn_WriteSerialCom_Click { get; set; }
        public ICommand btn_ReadSerialCom_Click { get; set; }
        public ICommand btn_DisconnectSerial_Click { get; set; }

        public ICommand btn_SendLastLot_Click { get; set; }

        #region Properties

        public static ObservableCollection<string> _ListCOMM;

        public ObservableCollection<string> ListCOMM
        {
            get { return _ListCOMM; }
            set
            {
                _ListCOMM = value;
                OnPropertyChanged("listCOMM");
            }
        }


        public static ObservableCollection<string> _ListBauRate;

        public ObservableCollection<string> ListBauRate
        {
            get { return _ListBauRate; }
            set
            {
                _ListBauRate = value;
                OnPropertyChanged("ListBauRate");
            }
        }



        private object _selectedCommLoad;
        public object selectedCommLoad
        {
            get { return _selectedCommLoad; }
            set
            {
                if (selectedCommLoad != value)
                {
                    _selectedCommLoad = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _selectedBauRate;
        public object selectedBauRate
        {
            get { return _selectedBauRate; }
            set
            {
                if (_selectedBauRate != value)
                {
                    _selectedBauRate = value;
                    OnPropertyChanged();
                }
            }
        }



        private string _txtDataWriteString;
        public string txtDataWriteString
        {
            get { return _txtDataWriteString; }
            set
            {
                _txtDataWriteString = value;
                OnPropertyChanged();
            }
        }

        private string _txt_DataReadString;
        public string txt_DataReadString
        {
            get => _txt_DataReadString;
            set
            {
                _txt_DataReadString = value;
                OnPropertyChanged();
            }
        }

        private bool _bSendLotEnable = true;
        public bool bSendLotEnable
        {
            get { return _bSendLotEnable; }
            set
            {
                _bSendLotEnable = value;
                OnPropertyChanged();
            }
        }

        #endregion
        public string strCommSelected = "";
        public int nBaurate = 115200;
        public static Comm.SerialCommunication m_serialCommunication;
        public System.Threading.Thread threadSendLotData;
        public MainWindowVM _mainWindowVM { get; set; }
        private DragDropUserControlVM _dragDropVM { set; get; }
        public void RegisterUserControl()
        {
            _dragDropVM.RegisterMoveGrid();
            _dragDropVM.RegisterResizeGrid();
        }


        public SerialCommunicationVM(DragDropUserControlVM dragDropVM, MainWindowVM mainVM)
        {
            _mainWindowVM = mainVM;
            _dragDropVM = dragDropVM;
            RegisterUserControl();
            selectedCommLoad = FileHelper.GetCommInfo("Serial Communication COM", "COM5", AppMagnus.pathRegistry);
            selectedBauRate = int.Parse(FileHelper.GetCommInfo("Serial Communication BauRate", "115200", AppMagnus.pathRegistry));
            m_serialCommunication = new Comm.SerialCommunication(selectedCommLoad.ToString(), (int)selectedBauRate);
            InitCOMM();
            InitBauRate();

            DrowDownListCommCommand = new RelayCommand<UserControl>((p) => { return true; },
                                         (p) =>
                                         {
                                         });

            DrowDownListBauRateCommand = new RelayCommand<UserControl>((p) => { return true; },
                                         (p) =>
                                         {
                                         });


            btn_ConnectSerial_Click = new RelayCommand<UserControl>((p) => { return true; },
                                         (p) =>
                                         {
                                             if (selectedCommLoad != null && selectedBauRate != null)
                                                 m_serialCommunication.InitializeConnection(selectedCommLoad.ToString(), int.Parse(selectedBauRate.ToString()));
                                         });

            btn_WriteSerialCom_Click = new RelayCommand<UserControl>((p) => { return true; },
                                         (p) =>
                                         {
                                             WriteSerialCom(_txtDataWriteString);
                                         });

            btn_ReadSerialCom_Click = new RelayCommand<UserControl>((p) => { return true; },
                                         (p) =>
                                         {
                                             txt_DataReadString += ReadSerialCom();
                                         });

            btn_DisconnectSerial_Click = new RelayCommand<UserControl>((p) => { return true; },
                                         (p) =>
                                         {
                                             m_serialCommunication.Disconnect();
                                         });

            btn_SendLastLot_Click = new RelayCommand<UserControl>((p) => { return true; },
                                         (p) =>
                                         {
                                             if (threadSendLotData == null)
                                             {
                                                 threadSendLotData = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                                                 {
                                                     bSendLotEnable = false;
                                                     MainWindowVM.master.sendLastLotDataToPID();
                                                     bSendLotEnable = true;
                                                 }
                                                 ));
                                                 threadSendLotData.Start();
                                                 return;
                                             }
                                             else if (!threadSendLotData.IsAlive)
                                             {
                                                 threadSendLotData = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                                                 {
                                                     bSendLotEnable = false;
                                                     MainWindowVM.master.sendLastLotDataToPID();
                                                     bSendLotEnable = true;
                                                 }
                                                 ));
                                                 threadSendLotData.Start();
                                                 return;
                                             }

                                         });
        }


        public void InitCOMM()
        {
            ListCOMM = new ObservableCollection<string>();
            foreach (string s in SerialPort.GetPortNames())
            {
                ListCOMM.Add(s);
            }
        }

        public void InitBauRate()
        {
            ListBauRate = new ObservableCollection<string>();
            ListBauRate.Add("115200");
            ListBauRate.Add("38400");
            ListBauRate.Add("19200");
            ListBauRate.Add("9600");
        }
        public static void WriteSerialCom(string strText)
        {
            Comm.SerialCommunication.WriteData(strText);
        }
        public string ReadSerialCom()
        {
            return Comm.SerialCommunication.ReadData();
        }


        //public string CreateReelCommand(string strLotID)
        //{

        //}
    }
}
