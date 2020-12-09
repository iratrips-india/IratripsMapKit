using System;
using System.Collections.Generic;
using System.Text;

namespace Iratrips.MapKit
{
    /// <summary>
    /// Custom callout view information 
    /// </summary>
    public class MKCallout : MKBase
    {
        string _title;
        string _subtitle;
        bool _isClickable;
        bool _customView;

        /// <summary>
        /// Gets/Sets title of the callout
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetField(ref _title, value); }
        }

        /// <summary>
        /// Gets/Sets the subtitle of the callout
        /// </summary>
        public string Subtitle
        {
            get { return _subtitle; }
            set { SetField(ref _subtitle, value); }
        }

        /// <summary>
        /// Gets/Sets whether the callout needs to show customize view. Implement GetCalloutView Func property to return View.
        /// </summary>
        public bool HasCustomView
        {
            get { return _customView; }
            set { SetField(ref _customView, value); }
        }

        /// <summary>
        /// Gets/Sets whether the callout is clickable or not. This adds/removes the accessory control on iOS
        /// </summary>
        public bool IsClickable
        {
            get { return _isClickable; }
            set { SetField(ref _isClickable, value); }
        }
    }
}
