using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Iratrips.MapKit.Example.Pages
{
    public partial class PinListPage : ContentPage
    {
        public event EventHandler<PinSelectedEventArgs> PinSelected;

         readonly IEnumerable<MKCustomMapPin> _pins;


        public PinListPage(IEnumerable<MKCustomMapPin> pins)
        {
            InitializeComponent();

            _pins = pins;
            BindingContext = _pins;

            _lvPins.ItemSelected += (o, e) =>
            {
                if (_lvPins.SelectedItem == null) return;

                OnPinSelected((MKCustomMapPin)_lvPins.SelectedItem);
            };
        }
        protected virtual void OnPinSelected(MKCustomMapPin pin)
        {
            PinSelected?.Invoke(this, new PinSelectedEventArgs(pin));
        }
    }
    public class PinSelectedEventArgs : EventArgs
    {
        public MKCustomMapPin Pin { get;  set; }

        public PinSelectedEventArgs(MKCustomMapPin pin)
        {
            Pin = pin;
        }
        
    }
}
