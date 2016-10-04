using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleSample.ViewModels.Abstract
{
    public abstract class BindableBase:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
