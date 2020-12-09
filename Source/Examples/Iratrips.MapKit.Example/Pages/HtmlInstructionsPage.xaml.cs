using Iratrips.MapKit.Overlays;
using Xamarin.Forms;

namespace Iratrips.MapKit.Example.Pages
{
    public partial class HtmlInstructionsPage : ContentPage
    {
        public HtmlInstructionsPage(MKRoute route)
        {
            InitializeComponent();

            BindingContext = new HtmlInstructionsViewModel(route);
        }
    }
}
