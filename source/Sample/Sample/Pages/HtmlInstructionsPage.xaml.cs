using Iratrips.Mapkit;
using Iratrips.Mapkit.Overlays;
using Xamarin.Forms;

namespace Sample
{
    public partial class HtmlInstructionsPage : ContentPage
    {
        public HtmlInstructionsPage(TKRoute route)
        {
            InitializeComponent();

            BindingContext = new HtmlInstructionsViewModel(route);
        }
    }
}
