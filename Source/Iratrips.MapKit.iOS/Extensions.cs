﻿using System.Threading.Tasks;
using CoreLocation;
using MapKit;
using Iratrips.MapKit.Overlays;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.iOS.ClusterKit;

namespace Iratrips.MapKit.iOSUnified
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convert <see cref="Position" /> to <see cref="CLLocationCoordinate2D"/>
        /// </summary>
        /// <param name="self">Self instance</param>
        /// <returns>iOS coordinate</returns>
        public static CLLocationCoordinate2D ToLocationCoordinate(this Position self)
        {
            return new CLLocationCoordinate2D(self.Latitude, self.Longitude);
        }
        /// <summary>
        /// Convert <see cref="CLLocationCoordinate2D" /> to <see cref="Position"/>
        /// </summary>
        /// <param name="self">Self instance</param>
        /// <returns>Forms position</returns>
        public static Position ToPosition(this CLLocationCoordinate2D self)
        {
            return new Position(self.Latitude, self.Longitude);
        }
        /// <summary>
        /// Convert <see cref="MKDirectionsTransportType"/> to <see cref="TKRouteTravelMode"/>
        /// </summary>
        /// <param name="self">Self instance</param>
        /// <returns>The map kit transport type</returns>
        public static MKDirectionsTransportType ToTransportType(this TKRouteTravelMode self)
        {
            switch (self)
            {
                case TKRouteTravelMode.Driving:
                    return MKDirectionsTransportType.Automobile;
                case TKRouteTravelMode.Walking:
                    return MKDirectionsTransportType.Walking;
                case TKRouteTravelMode.Any:
                    return MKDirectionsTransportType.Any;
                default:
                    return MKDirectionsTransportType.Automobile;
            }
        }
        /// <summary>
        /// Converts an <see cref="ImageSource"/> to the native iOS <see cref="UIImage"/>
        /// </summary>
        /// <param name="source">Self intance</param>
        /// <returns>The UIImage</returns>
        public static async Task<UIImage> ToImage(this ImageSource source)
        {
            if (source is FileImageSource)
            {
                return await new FileImageSourceHandler().LoadImageAsync(source);
            }
            if (source is UriImageSource)
            {
                return await new ImageLoaderSourceHandler().LoadImageAsync(source);
            }
            if (source is StreamImageSource)
            {
                return await new StreamImagesourceHandler().LoadImageAsync(source);
            }
            return null;
        }
        /// <summary>
        /// Convert <see cref="CGPoint"/> to <see cref="Point"/>
        /// </summary>
        /// <param name="point">Self</param>
        /// <returns>iOS Point</returns>
        public static CGPoint ToCGPoint(this Point point)
        {
            return new CGPoint(point.X, point.Y);
        }
        /// <summary>
        /// Gets the current map region as <see cref="MapSpan"/>
        /// </summary>
        /// <param name="mapView">the mapview instance</param>
        /// <returns>The map region</returns>
        public static MapSpan GetCurrentMapRegion(this MKMapView mapView) => 
            new MapSpan(new Position(mapView.Region.Center.Latitude, mapView.Region.Center.Longitude), mapView.Region.Span.LatitudeDelta, mapView.Region.Span.LongitudeDelta);

        /// <summary>
        /// Gets the non clustered annotations
        /// </summary>
        /// <param name="annotations">All annotations</param>
        /// <returns>Enuerable of non clustered annotations</returns>
        internal static IEnumerable<TKCustomMapAnnotation> GetNonClusteredAnnotations(this IEnumerable<IMKAnnotation> annotations)
        {
            var nonClusteredAnnotations = annotations.OfType<CKCluster>().Where(i => i.Annotations.Count() == 1 && i.FirstAnnotation is TKCustomMapAnnotation);
            return nonClusteredAnnotations?.Any() == true ? nonClusteredAnnotations.Select(i => i.FirstAnnotation as TKCustomMapAnnotation) : Enumerable.Empty<TKCustomMapAnnotation>(); 
        }
        internal static CKCluster GetCluster(this IEnumerable<IMKAnnotation> annotations, TKCustomMapAnnotation annotation)
        {
            return annotations.OfType<CKCluster>().FirstOrDefault(i => i.Annotations.Count() == 1 && i.FirstAnnotation.Equals(annotation));
        }
    }
}
