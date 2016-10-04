namespace SimpleSample.ViewModels.Abstract
{
    public abstract class BaseViewModel:BindableBase
    {
        #region Fields
        bool _isBusy;
        #endregion

        #region Constructor
        #endregion

        #region Properties
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        
                
        #endregion
    }
}
