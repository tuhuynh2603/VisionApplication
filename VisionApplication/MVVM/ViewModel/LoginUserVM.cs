

namespace VisionApplication.MVVM.ViewModel
{
    public class LoginUserVM:BaseVM, ICustomUserControl
    {

        public MainWindowVM _mainWindowVM { get; set; }
        private DragDropUserControlVM _dragDropVM { set; get; }
        public void RegisterUserControl()
        {
            _dragDropVM.RegisterMoveGrid();
            _dragDropVM.RegisterResizeGrid();
        }

        public LoginUserVM(DragDropUserControlVM dragDropVM, MainWindowVM mainVM)
        {
            _mainWindowVM = mainVM;
            _dragDropVM = (DragDropUserControlVM)dragDropVM;
            RegisterUserControl();
        }


        private bool _isFocused;
        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                _isFocused = value;
                OnPropertyChanged(nameof(IsFocused));
            }
        }



    }
}
