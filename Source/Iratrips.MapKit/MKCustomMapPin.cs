using System;
using Xamarin.Forms;

namespace Iratrips.MapKit
{
    /// <summary>
    /// A custom map pin
    /// </summary>
    public class MKCustomMapPin : MKBase
    {
        bool _isVisible;
        string _id;
        string _group;
        Position _position;
        ImageSource _image;
        bool _isDraggable;
        Color _defaultPinColor;
        Point _anchor = new Point(0.5, 0.5);
        double _rotation;
        MKCallout _callout;

        /// <summary>
        /// Gets the id of the <see cref="MKCustomMapPin"/>
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets/Sets visibility of a pin
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetField(ref _isVisible, value); }
        }

        /// <summary>
        /// Gets/Sets ID of the pin, used for client app reference (optional)
        /// </summary>
        public string ID
        {
            get { return _id; }
            set { SetField(ref _id, value); }
        }

        /// <summary>
        /// Gets/Sets the position of the pin
        /// </summary>
        public Position Position
        {
            get { return _position; }
            set { SetField(ref _position, value); }
        }

        /// <summary>
        /// Gets/Sets the image of the pin. If null the default is used
        /// UriImageSource is not supported.
        /// </summary>
        public ImageSource Image
        {
            get { return _image; }
            set { SetField(ref _image, value); }
        }

        /// <summary>
        /// Gets/Sets if the pin is draggable
        /// </summary>
        public bool IsDraggable
        {
            get { return _isDraggable; }
            set { SetField(ref _isDraggable, value); }
        }
        /// <summary>
        /// Gets/Sets the color of the default pin. Only applies when no <see cref="Image"/> is set
        /// </summary>
        public Color DefaultPinColor
        {
            get { return _defaultPinColor; }
            set { SetField(ref _defaultPinColor, value); }
        }
        /// <summary>
        /// Gets/Sets the anchor point of the pin when using a custom pin image
        /// </summary>
        public Point Anchor
        {
            get { return _anchor; }
            set { SetField(ref _anchor, value); }
        }
        /// <summary>
        /// Gets/Sets the rotation angle of the pin in degrees
        /// </summary>
        public double Rotation
        {
            get { return _rotation; }
            set { SetField(ref _rotation, value); }
        }

        /// <summary>
        /// Gets/Sets the group identifier
        /// </summary>
        public string Group
        {
            get => _group;
            set { SetField(ref _group, value); }
        }

        /// <summary>
        /// A function which should return a view which will be shown as a popup when user clicks on the pin.
        /// Xamarin binding will not work here. On Runtime, snapshot will be displayed as a popup. 
        /// </summary>
        public MKCallout Callout
        {
            get => _callout;
            set { SetField(ref _callout, value); }
        }

        /// <summary>
        /// Creates a new instance of <see cref="MKCustomMapPin" />
        /// </summary>
        public MKCustomMapPin()
        {
            IsVisible = true;
        }

        /// <summary>
        /// Checks whether the <see cref="Id"/> of the pins match
        /// </summary>
        /// <param name="obj">The <see cref="MKCustomMapPin"/> to compare</param>
        /// <returns>true of the ids match</returns>
        public override bool Equals(object obj)
        {
            var pin = obj as MKCustomMapPin;

            if (pin == null) return false;

            return Id.Equals(pin.Id);
        }
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
