//using Android.Content;
//using Android.Gms.Maps;
//using Android.Gms.Maps.Model;
//using Android.Gms.Maps.Utils.Clustering;
//using Android.Gms.Maps.Utils.Clustering.View;
//using Android.Gms.Maps.Utils.UI;
//using System.Linq;

//namespace Iratrips.Mapkit.Droid
//{
//    public class TKMarkerRenderer : DefaultClusterRenderer
//    {
//        Context _context;
//        GoogleMap _googleMap;
//        TKCustomMapHandler _mapHandler;
//        IconGenerator _iconGenerator;

//        public TKMarkerRenderer(Context context, GoogleMap googleMap, ClusterManager clusterManager, TKCustomMapHandler mapHandler) :
//            base(context, googleMap, clusterManager)
//        {
//            _context = context;
//            _googleMap = googleMap;
//            _mapHandler = mapHandler;
//            _iconGenerator = new IconGenerator(context);
//        }

//        protected override void OnBeforeClusterItemRendered(Java.Lang.Object p0, MarkerOptions p1)
//        {
//            var tkMarker = p0 as TKMarker;

//            if (tkMarker == null) return;

//            tkMarker.InitializeMarkerOptions(p1);
//        }
//        protected override void OnClusterItemRendered(Java.Lang.Object p0, Marker p1)
//        {
//            base.OnClusterItemRendered(p0, p1);

//            var tkMarker = p0 as TKMarker;

//            if (tkMarker == null) return;

//            tkMarker.Marker = p1;
//        }

//        protected override void OnBeforeClusterRendered(ICluster p0, MarkerOptions p1)
//        {
//            base.OnBeforeClusterRendered(p0, p1);

//            var customPin = _mapHandler.FormsMap.GetClusteredPin?.Invoke(null, p0.Items.OfType<TKMarker>().Select(i => i.Pin));

//            if (customPin == null)
//            {
//                p1.SetIcon(BitmapDescriptorFactory.FromBitmap(_iconGenerator.MakeIcon(p0.Size.ToString())));
//            }
//            else
//            {
//                var tempMarker = new TKMarker(customPin, _context);
//                tempMarker.InitializeMarkerOptions(p1, false);
//            }
//        }

//        protected override void OnClusterRendered(ICluster p0, Marker p1)
//        {
//            base.OnClusterRendered(p0, p1);

//            var tkMarker = p0 as TKMarker;

//            if (tkMarker == null) return;

//            tkMarker.Marker = p1;
//        }
//    }
//}