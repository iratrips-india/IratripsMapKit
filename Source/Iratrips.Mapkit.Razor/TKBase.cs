using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Iratrips.Mapkit
{
    public partial class TKBase : INotifyPropertyChanged
    {
        private partial void InvokedOnPropertyChanged(string propertyName)
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
