using Iratrips.MapKit;
using Iratrips.MapKit.Overlays;
using Xamarin.Forms;

namespace Iratrips.MapKit.Example
{
    public class HtmlInstructionsViewModel : MKBase
    {
        public HtmlWebViewSource Instructions { get;  set; }
        

        public HtmlInstructionsViewModel(MKRoute route)
        {
            Instructions = new HtmlWebViewSource();
            Instructions.Html = @"<html><body>";
            foreach (var s in route.Steps)
            {
                Instructions.Html += string.Format("<b>{0}km:</b> {1}<br /><hr />", s.Distance / 1000, s.Instructions);
            }
            Instructions.Html += @"</body></html>";
        }
    }
}
