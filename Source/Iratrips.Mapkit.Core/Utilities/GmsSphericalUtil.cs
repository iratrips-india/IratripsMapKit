/*
 * Copyright 2013 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace Iratrips.Mapkit.Utilities
{
    /// <summary>
    /// This is a java to c# port from 
    /// https://github.com/googlemaps/android-maps-utils/blob/dba3b0d8a9657ebab8c67a4f50bd731437a229bc/library/src/com/google/maps/android/SphericalUtil.java
    /// </summary>
    public static class GmsSphericalUtil
    {
        /// <summary>
        ///  Returns the LatLng resulting from moving a distance from an origin
        ///  in the specified heading (expressed in degrees clockwise from north).
        /// </summary>
        /// <param name="from">The LatLng from which to start.</param>
        /// <param name="distance">The distance to travel</param>
        /// <param name="heading">The heading in degrees clockwise from north.</param>
        /// <returns>Position with offset</returns>
        public static Position ComputeOffset(Position from, double distance, double heading)
        {
            distance /= GmsMathUtils.EarthRadius;
            heading = heading.ToRadian();
            // http://williams.best.vwh.net/avform.htm#LL
            double fromLat = from.Latitude.ToRadian();
            double fromLng = from.Longitude.ToRadian();
            double cosDistance = Math.Cos(distance);
            double sinDistance = Math.Sin(distance);
            double sinFromLat = Math.Sin(fromLat);
            double cosFromLat = Math.Cos(fromLat);
            double sinLat = cosDistance * sinFromLat + sinDistance * cosFromLat * Math.Cos(heading);
            double dLng = Math.Atan2(
                    sinDistance * cosFromLat * Math.Sin(heading),
                    cosDistance - sinFromLat * sinLat);
            return new Position(Math.Asin(sinLat).ToDegrees(), (fromLng + dLng).ToDegrees());
        }

        /**
    * Returns distance on the unit sphere; the arguments are in radians.
    */
        private static double DistanceRadians(double lat1, double lng1, double lat2, double lng2)
        {
            return GmsMathUtils.ArcHav(GmsMathUtils.HavDistance(lat1, lat2, lng1 - lng2));
        }

        /**
         * Returns the angle between two LatLngs, in radians. This is the same as the distance
         * on the unit sphere.
         */
        private static double ComputeAngleBetween(Position from, Position to)
        {
            return DistanceRadians(from.Latitude.ToRadian(), from.Longitude.ToRadian(), to.Latitude.ToRadian(), to.Longitude.ToRadian());
        }

        private static double ComputeAngleBetween(double fromLat, double fromLng, double toLat, double toLng)
        {
            return DistanceRadians(fromLat.ToRadian(), fromLng.ToRadian(), toLat.ToRadian(), toLng.ToRadian());
        }

        /**
         * Returns the distance between two LatLngs, in meters.
         */
        public static double ComputeDistanceBetween(Position from, Position to)
        {
            return ComputeAngleBetween(from, to) * GmsMathUtils.EarthRadius;
        }

        public static double ComputeDistanceBetween(double fromLat, double fromLng, double toLat, double toLng)
        {
            return ComputeAngleBetween(fromLat, fromLng, toLat, toLng) * GmsMathUtils.EarthRadius;
        }

        /**
     * Returns the heading from one LatLng to another LatLng. Headings are
     * expressed in degrees clockwise from North within the range [-180,180).
     * @return The heading in degrees clockwise from north.
     */
        public static double ComputeHeading(Position from, Position to)
        {
            // http://williams.best.vwh.net/avform.htm#Crs
            double fromLat = from.Latitude.ToRadian();
            double fromLng = from.Longitude.ToRadian();
            double toLat = to.Latitude.ToRadian();
            double toLng = to.Longitude.ToRadian();
            double dLng = toLng - fromLng;
            double heading = Math.Atan2(
                Math.Sin(dLng) * Math.Cos(toLat),
                Math.Cos(fromLat) * Math.Sin(toLat) - Math.Sin(fromLat) * Math.Cos(toLat) * Math.Cos(dLng));
            return GmsMathUtils.Wrap(heading.ToDegrees(), -180, 180);
        }
    }
}
