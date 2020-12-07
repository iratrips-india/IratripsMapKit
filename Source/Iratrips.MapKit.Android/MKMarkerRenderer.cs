using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Java.Lang;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Clustering.View;
using Android.Gms.Ui;
using Android.Gms.Clustering;

namespace Iratrips.MapKit.Droid
{
    public class MKMarkerRenderer : DefaultClusterRenderer
    {
        Context _context;
        GoogleMap _googleMap;
        MKCustomMapRenderer _mapRenderer;
        IconGenerator _iconGenerator;

        public MKMarkerRenderer(Context context, GoogleMap googleMap, ClusterManager clusterManager, MKCustomMapRenderer mapRenderer) :
            base(context, googleMap, clusterManager)
        {
            _context = context;
            _googleMap = googleMap;
            _mapRenderer = mapRenderer;
            _iconGenerator = new IconGenerator(context);
        }

        protected async override void OnBeforeClusterItemRendered(Java.Lang.Object p0, MarkerOptions p1)
        {
            var tkMarker = p0 as MKMarker;

            if (tkMarker == null) return;

            await tkMarker.InitializeMarkerOptionsAsync(p1);
        }
        protected override void OnClusterItemRendered(Java.Lang.Object p0, Marker p1)
        {
            base.OnClusterItemRendered(p0, p1);

            var tkMarker = p0 as MKMarker;

            if (tkMarker == null) return;

            tkMarker.Marker = p1;
        }

        protected async override void OnBeforeClusterRendered(ICluster p0, MarkerOptions p1)
        {
            base.OnBeforeClusterRendered(p0, p1);

            var customPin = _mapRenderer.FormsMap.GetClusteredPin?.Invoke(null, p0.Items.OfType<MKMarker>().Select(i => i.Pin));

            if (customPin == null)
            {
                p1.SetIcon(BitmapDescriptorFactory.FromBitmap(_iconGenerator.MakeIcon(p0.Size.ToString())));
            }
            else
            {
                var tempMarker = new MKMarker(customPin, _context);
                
                await tempMarker.InitializeMarkerOptionsAsync(p1, false);
            }
        }

        protected override void OnClusterRendered(ICluster p0, Marker p1)
        {
            base.OnClusterRendered(p0, p1);

            var tkMarker = p0 as MKMarker;

            if (tkMarker == null) return;

            tkMarker.Marker = p1;
        }
    }
}