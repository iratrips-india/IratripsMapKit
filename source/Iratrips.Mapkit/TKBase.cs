using Microsoft.Maui.Controls;

namespace Iratrips.Mapkit
{
    public partial class TKBase : BindableObject
    {
        private partial void InvokedOnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}
