using Android.Content;
using Android.Gms.Clustering;
using Android.Gms.Maps.Model;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;

namespace Iratrips.MapKit.Droid
{
    /// <summary>
    /// Internal Marker extension class for clustering
    /// </summary>
    internal class MKMarker : Java.Lang.Object, IClusterItem
    {
        Context _context;
        /// <summary>
        /// Creates a new instance of <see cref="MKMarker"/>
        /// </summary>
        /// <param name="pin">The intnernal pin</param>
        /// <param name="context">Android context</param>
        public MKMarker(MKCustomMapPin pin, Context context)
        {
            Pin = pin;
            _context = context;
        }
        /// <summary>
        /// Gets/Sets the custom pin
        /// </summary>
        public MKCustomMapPin Pin { get;  set; }
        /// <summary>
        /// Gets the current pin position
        /// </summary>
        public LatLng Position => Pin.Position.ToLatLng();
        /// <summary>
        /// Gets the current snippet
        /// </summary>
        public string Snippet => Pin.Callout?.Subtitle;
        /// <summary>
        /// Gets the current title
        /// </summary>
        public string Title => Pin.Callout?.Title;
        /// <summary>
        /// Gets the <see cref="Marker"/>
        /// </summary>
        public Marker Marker { get; internal set; }
        /// <summary>
        /// Handles the property changed event
        /// </summary>
        /// <param name="e">Event arguments</param>
        /// <param name="isDragging">If the pin is dragging or not</param>
        /// <returns>Task</returns>
        public async Task HandlePropertyChangedAsync(PropertyChangedEventArgs e, bool isDragging)
        {
            switch (e.PropertyName)
            {
                case nameof(MKCustomMapPin.Callout):
                    Marker.Title = Pin.Callout?.Title;
                    Marker.Snippet = Pin.Callout?.Subtitle;
                    break;
                case nameof(MKCustomMapPin.Image):
                    await UpdateImageAsync();
                    break;
                case nameof(MKCustomMapPin.DefaultPinColor):
                    await UpdateImageAsync();
                    break;
                case nameof(MKCustomMapPin.Position):
                    if (!isDragging)
                    {
                        Marker.Position = new LatLng(Pin.Position.Latitude, Pin.Position.Longitude);
                    }
                    break;
                case nameof(MKCustomMapPin.IsVisible):
                    Marker.Visible = Pin.IsVisible;
                    break;
                case nameof(MKCustomMapPin.Anchor):
                    if (Pin.Image != null)
                    {
                        Marker.SetAnchor((float)Pin.Anchor.X, (float)Pin.Anchor.Y);
                    }
                    break;
                case nameof(MKCustomMapPin.IsDraggable):
                    Marker.Draggable = Pin.IsDraggable;
                    break;
                case nameof(MKCustomMapPin.Rotation):
                    Marker.Rotation = (float)Pin.Rotation;
                    break;
            }
        }
        /// <summary>
        /// initializes the <see cref="MarkerOptions"/>
        /// </summary>
        /// <param name="markerOptions">Instance of the marker options</param>
        /// <param name="setPosition">if <value>true</value>, the position will be updated</param>
        /// <returns><see cref="Task"/></returns>
        public async Task InitializeMarkerOptionsAsync(MarkerOptions markerOptions, bool setPosition = true)
        {
            if (setPosition)
            {
                markerOptions.SetPosition(new LatLng(Pin.Position.Latitude, Pin.Position.Longitude));
            }

            if (Pin.Callout != null && !string.IsNullOrWhiteSpace(Pin.Callout.Title))
                markerOptions.SetTitle(Pin.Callout.Title);
            
            if (Pin.Callout != null && !string.IsNullOrWhiteSpace(Pin.Callout.Subtitle))
                markerOptions.SetSnippet(Pin.Callout.Subtitle);

            await UpdateImageAsync(markerOptions);
            markerOptions.Draggable(Pin.IsDraggable);
            markerOptions.Visible(Pin.IsVisible);
            markerOptions.SetRotation((float)Pin.Rotation);
            if (Pin.Image != null)
            {
                markerOptions.Anchor((float)Pin.Anchor.X, (float)Pin.Anchor.Y);
            }
        }
        /// <summary>
        /// Updates the image of a pin
        /// </summary>
        /// <param name="pin">The forms pin</param>
        /// <param name="markerOptions">The native marker options</param>
        async Task UpdateImageAsync()
        {
            BitmapDescriptor bitmap;
            try
            {
                if (Pin.Image != null)
                {
                    bitmap = BitmapDescriptorFactory.FromBitmap(await Pin.Image.ToBitmap(_context));
                }
                else
                {
                    if (Pin.DefaultPinColor != Xamarin.Forms.Color.Default)
                    {
                        var hue = Pin.DefaultPinColor.ToAndroid().GetHue();
                        bitmap = BitmapDescriptorFactory.DefaultMarker(System.Math.Min(hue, 359.99f));
                    }
                    else
                    {
                        bitmap = BitmapDescriptorFactory.DefaultMarker();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Error("MapKit", ex.Message + "\n\n" + ex.StackTrace);
                bitmap = BitmapDescriptorFactory.DefaultMarker();
            }

            Marker.SetIcon(bitmap);
        }
        /// <summary>
        /// Updates the image of a pin
        /// </summary>
        /// <param name="pin">The forms pin</param>
        /// <param name="markerOptions">The native marker options</param>
        async Task UpdateImageAsync(MarkerOptions markerOptions)
        {
            BitmapDescriptor bitmap;
            try
            {
                if (Pin.Image != null)
                {
                    bitmap = BitmapDescriptorFactory.FromBitmap(await Pin.Image.ToBitmap(_context));
                }
                else
                {
                    if (Pin.DefaultPinColor != Xamarin.Forms.Color.Default)
                    {
                        var hue = Pin.DefaultPinColor.ToAndroid().GetHue();
                        bitmap = BitmapDescriptorFactory.DefaultMarker(System.Math.Min(hue, 359.99f));
                    }
                    else
                    {
                        bitmap = BitmapDescriptorFactory.DefaultMarker();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Error("MapKit", ex.Message + "\n\n" + ex.StackTrace);
                bitmap = BitmapDescriptorFactory.DefaultMarker();
            }
            markerOptions.SetIcon(bitmap);
        }
    }
}