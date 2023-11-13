using Android.Animation;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils;
using Android.Gms.Maps.Utils.Clustering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Android.OS;
using Java.Lang;
using Xamarin.Forms.Platform.Android;

namespace Iratrips.Mapkit.Droid
{
    /// <summary>
    /// Internal Marker extension class for clustering
    /// </summary>
    internal class TKMarker : Java.Lang.Object, IClusterItem
    {
        private ValueAnimator _pinAnimator;
        private double? _currentSpeed = null;
        private int _furtherPointIndex = -1;
        private IList<LatLng> _furtherPoints = null;
        private PinAnimatorUpdateListener _pinAnimatorUpdateListener = null;
        private AnimatorListener _animatorListener = null;
        private double? _lastGap = null;

        Context _context;
        /// <summary>
        /// Creates a new instance of <see cref="TKMarker"/>
        /// </summary>
        /// <param name="pin">The intnernal pin</param>
        /// <param name="context">Android context</param>
        public TKMarker(TKCustomMapPin pin, Context context)
        {
            Pin = pin;
            _context = context;
        }
        /// <summary>
        /// Gets/Sets the custom pin
        /// </summary>
        public TKCustomMapPin Pin { get; set; }
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
        public Task HandlePropertyChangedAsync(PropertyChangedEventArgs e, bool isDragging)
        {
            switch (e.PropertyName)
            {
                case nameof(TKCustomMapPin.Callout):
                    Marker.Title = Pin.Callout?.Title;
                    Marker.Snippet = Pin.Callout?.Subtitle;
                    break;
                case nameof(TKCustomMapPin.Image):
                    UpdateImage();
                    break;
                case nameof(TKCustomMapPin.DefaultPinColor):
                    UpdateImage();
                    break;
                case nameof(TKCustomMapPin.Position):
                    if (!isDragging)
                    {
                        var newPosition = new LatLng(Pin.Position.Latitude, Pin.Position.Longitude);

                        if (_pinAnimator is { IsRunning: true })
                        {
                            CancelAnimation();
                            _lastGap = SphericalUtil.ComputeDistanceBetween(Marker.Position, newPosition);
                            if (_lastGap > 50) //Don't animate if the gap is large
                            {
                                AnimateMarkerPosition(Pin.Position);
                                Android.Util.Log.Debug("MapKit", "Large gap, skipping animation");
                            }
                            else
                            {
                                //Skip 
                            }
                        }
                        else
                            Marker.Position = newPosition;
                    }
                    break;
                case nameof(TKCustomMapPin.IsVisible):
                    Marker.Visible = Pin.IsVisible;
                    break;
                case nameof(TKCustomMapPin.Anchor):
                    if (Pin.Image != null)
                    {
                        Marker.SetAnchor((float)Pin.Anchor.X, (float)Pin.Anchor.Y);
                    }
                    break;
                case nameof(TKCustomMapPin.IsDraggable):
                    Marker.Draggable = Pin.IsDraggable;
                    break;
                case nameof(TKCustomMapPin.Rotation):
                    Marker.Rotation = (float)Pin.Rotation;
                    break;
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// initializes the <see cref="MarkerOptions"/>
        /// </summary>
        /// <param name="markerOptions">Instance of the marker options</param>
        /// <param name="setPosition">if <value>true</value>, the position will be updated</param>
        /// <returns><see cref="Task"/></returns>
        public void InitializeMarkerOptions(MarkerOptions markerOptions, bool setPosition = true)
        {
            if (setPosition)
            {
                markerOptions.SetPosition(new LatLng(Pin.Position.Latitude, Pin.Position.Longitude));
            }

            if (Pin.Callout != null && !string.IsNullOrWhiteSpace(Pin.Callout.Title))
                markerOptions.SetTitle(Pin.Callout.Title);

            if (Pin.Callout != null && !string.IsNullOrWhiteSpace(Pin.Callout.Subtitle))
                markerOptions.SetSnippet(Pin.Callout.Subtitle);

            UpdateImage(markerOptions);
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
        void UpdateImage()
        {
            BitmapDescriptor bitmap;
            try
            {
                if (Pin.Image != null)
                {
                    bitmap = BitmapDescriptorFactory.FromBitmap(Pin.Image.ToBitmap(_context));
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
        void UpdateImage(MarkerOptions markerOptions)
        {
            BitmapDescriptor bitmap;
            try
            {
                if (Pin.Image != null)
                {
                    bitmap = BitmapDescriptorFactory.FromBitmap(Pin.Image.ToBitmap(_context));
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

        private void InitAnimation()
        {
            if (_pinAnimator == null)
            {
                _pinAnimatorUpdateListener = new PinAnimatorUpdateListener(this.Marker);
                _animatorListener = new AnimatorListener(this);

                _pinAnimator = ValueAnimator.OfFloat(0, 1);
                if (_pinAnimator == null) return;

                _pinAnimator.SetInterpolator(new Android.Views.Animations.LinearInterpolator());
                _pinAnimator.AddUpdateListener(_pinAnimatorUpdateListener);
                _pinAnimator.AddListener(_animatorListener);
            }
        }


        public void AnimateMarkerPosition(Position newPosition)
        {
            CancelAnimation();
            InitAnimation();

            if (_pinAnimator.IsRunning) _pinAnimator.Pause();

            var newLatLng = newPosition.ToLatLng();
            var remainingDistance = SphericalUtil.ComputeDistanceBetween(this.Marker.Position, newLatLng);

            Android.Util.Log.Debug("MapKit", $"Starting Animation till the new position with distance {remainingDistance} meters.");

            _pinAnimator.SetCurrentFraction(0);

            _animatorListener.EndPosition = newLatLng;
            _pinAnimatorUpdateListener.InitialPosition = this.Marker.Position;
            _pinAnimatorUpdateListener.TotalDistance = remainingDistance;
            _pinAnimatorUpdateListener.CurrentBearing = SphericalUtil.ComputeHeading(this.Marker.Position, newLatLng);

            var duration = 1000;
            _pinAnimator.SetDuration(duration);
            _pinAnimator.Start();
        }

        public void CancelAnimation()
        {
            _currentSpeed = null;
            _furtherPoints = null;
            _furtherPointIndex = -1;

            if (_pinAnimator is { IsRunning: true })
            {
                _pinAnimator.Pause();
            }
        }

        public void AnimateMarkerPosition(double? speed, Position currentPosition, IList<Position> furtherPoints)
        {
            CancelAnimation();
            if (speed <= 0 || furtherPoints == null || furtherPoints.Count == 0)
                return;

            _currentSpeed = speed;
            _furtherPoints = furtherPoints.Select(k => k.ToLatLng()).ToList();
            _furtherPointIndex = -1;

            Android.Util.Log.Debug("MapKit", $"Starting Animation Distance: {SphericalUtil.ComputeLength(furtherPoints.Select(k => k.ToLatLng()).ToList())} meters");

            InitAnimation();

            var currentLatLng = currentPosition.ToLatLng();

            var heading1 = SphericalUtil.ComputeHeading(this.Marker.Position, _furtherPoints[0]);
            var heading2 = SphericalUtil.ComputeHeading(_furtherPoints[0], _furtherPoints[1]);
            if (System.Math.Abs(heading2 - heading1) > 90)
            {
                var polyIndex = LocationIndexOnLine(this.Marker.Position, _furtherPoints);
                if (polyIndex >= 0)
                {
                    var distance = SphericalUtil.ComputeDistanceBetween(currentLatLng, _furtherPoints[0]);
                    if (polyIndex > 0)
                    {
                        for (var i = 0; i <= polyIndex; i++)
                            distance += SphericalUtil.ComputeDistanceBetween(_furtherPoints[i], _furtherPoints[i + 1]);
                    }

                    distance += SphericalUtil.ComputeDistanceBetween(_furtherPoints[polyIndex], _furtherPoints[polyIndex + 1]);
                    _furtherPoints = _furtherPoints.Skip(polyIndex + 1).ToList();
                    
                    AnimateToNextPosition(currentPosition.ToLatLng(), distance);
                }
                else
                    AnimateToNextPosition(currentPosition.ToLatLng());
            }
            else
                AnimateToNextPosition(currentPosition.ToLatLng());
        }

        public void AnimateToNextPosition(LatLng currentPosition, double? actualDistanceEx = null)
        {
            while (true)
            {
                if (_furtherPoints == null || _furtherPoints.Count == 0 || _currentSpeed == null) return;

                if (_furtherPointIndex > _furtherPoints.Count - 1) return;

                _furtherPointIndex++;

                if (_pinAnimator.IsRunning) _pinAnimator.Pause();

                if (_furtherPointIndex > _furtherPoints.Count - 1) return;

                var nextLatLng = _furtherPoints[_furtherPointIndex];

                var remainingDistance = SphericalUtil.ComputeDistanceBetween(this.Marker.Position, nextLatLng);
                var actualDistance = actualDistanceEx ?? SphericalUtil.ComputeDistanceBetween(currentPosition, nextLatLng);

                _lastGap = null;
                if (remainingDistance < 0)
                {
                    //Not sure if this happen but added for safety
                    Android.Util.Log.Debug("MapKit", $"Remaining distance is less than zero.");
                    //Not sure if this happen but added for safety
                    continue;
                }

                _pinAnimator.SetCurrentFraction(0);

                _animatorListener.EndPosition = nextLatLng;
                _pinAnimatorUpdateListener.InitialPosition = this.Marker.Position;
                _pinAnimatorUpdateListener.TotalDistance = remainingDistance;
                _pinAnimatorUpdateListener.CurrentBearing = SphericalUtil.ComputeHeading(this.Marker.Position, nextLatLng);

                var duration = (long)((actualDistance / _currentSpeed) * 1000);
                _pinAnimator.SetDuration(duration);
                _pinAnimator.Start();
                break;
            }
        }

        private class PinAnimatorUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
        {
            private readonly Marker _marker;

            public PinAnimatorUpdateListener(Marker marker)
            {
                _marker = marker;
            }

            public LatLng InitialPosition { get; set; }
            public double TotalDistance { get; set; }
            public double CurrentBearing { get; set; }

            public void OnAnimationUpdate(ValueAnimator animation)
            {
                var change = this.TotalDistance * animation.AnimatedFraction;
                if (change > 0)
                    _marker.Position = SphericalUtil.ComputeOffset(InitialPosition, change, CurrentBearing);
            }
        }

        internal class AnimatorListener : Java.Lang.Object, Animator.IAnimatorListener
        {
            private readonly TKMarker _marker;

            public LatLng EndPosition { get; set; }

            public AnimatorListener(TKMarker marker)
            {
                _marker = marker;
            }

            public void OnAnimationCancel(Animator animation)
            {

            }

            public void OnAnimationEnd(Animator animation)
            {
                //_marker.Marker.Position = EndPosition.ToLatLng();
                _marker.AnimateToNextPosition(_marker.Marker.Position);
            }

            public void OnAnimationRepeat(Animator animation)
            {

            }

            public void OnAnimationStart(Animator animation)
            {

            }
        }

        public int LocationIndexOnLine(LatLng point, IList<LatLng> polyLine)
        {
            int edgeIndex = PolyUtil.LocationIndexOnPath(point, polyLine, true, 1);
            if (edgeIndex >= 0)
                return edgeIndex;

            edgeIndex = PolyUtil.LocationIndexOnPath(point, polyLine, true, 2);
            if (edgeIndex >= 0)
                return edgeIndex;

            edgeIndex = PolyUtil.LocationIndexOnPath(point, polyLine, true, 4);
            if (edgeIndex >= 0)
                return edgeIndex;

            edgeIndex = PolyUtil.LocationIndexOnPath(point, polyLine, true, 8);
            if (edgeIndex >= 0)
                return edgeIndex;

            edgeIndex = PolyUtil.LocationIndexOnPath(point, polyLine, true, 11);
            if (edgeIndex >= 0)
                return edgeIndex;

            return -1;
        }
    }
}