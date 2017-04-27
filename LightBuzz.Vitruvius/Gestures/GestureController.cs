
using LightBuzz.Vitruvius.Gestures;
using Microsoft.Kinect;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LightBuzz.Vitruvius
{
    public class GestureController : BaseController<Body>
    {
        #region Members

        /// <summary>
        /// A list of all the gestures the controller is searching for.
        /// </summary>
        private List<Gesture> _gestures = new List<Gesture>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="GestureController"/> with all of the available gesture types.
        /// </summary>
        public GestureController()
        {
            foreach (GestureType t in Enum.GetValues(typeof(GestureType)))
            {
                AddGesture(t);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GestureController"/> with the specified gesture type.
        /// </summary>
        /// <param name="type">The gesture type to recognize.</param>
        public GestureController(GestureType type)
        {
            AddGesture(type);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a gesture is recognized.
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureRecognized;

        #endregion

        #region Methods

        /// <summary>
        /// Updates all gestures.
        /// </summary>
        /// <param name="body">The body data to search for gestures.</param>
        public override void Update(Body body)
        {
            base.Update(body);

            foreach (Gesture gesture in _gestures)
            {
                gesture.Update(body);
            }
        }

        /// <summary>
        /// Adds the specified gesture for recognition.
        /// </summary>
        /// <param name="type">The predefined <see cref="GestureType" />.</param>
        public void AddGesture(GestureType type)
        {
            // Check whether the gesure is already added.
            if (_gestures.Where(g => g.GestureType == type).Count() > 0) return;

            IGestureSegment[] segments = null;

            // DEVELOPERS: If you add a new predefined gesture with a new GestureType,
            // simply add the proper segments to the switch statement here.
            switch (type)
            {
                case GestureType.CrossedHands:
                    segments = new IGestureSegment[8];

                    CrossHandsSegment1 segment = new CrossHandsSegment1();
                    for (int i = 0; i < 8; i++)
                    {
                        segments[i] = segment;
                    }
                    break;
                case GestureType.HandsAboveHead:
                    segments = new IGestureSegment[8];

                    HandsAboveHeadSegment1 segment_hand_head = new HandsAboveHeadSegment1();
                    for (int i = 0; i < 8; i++)
                    {
                        segments[i] = segment_hand_head;
                    }
                    break;
                case GestureType.SwipeLeftReversed:
                    segments = new IGestureSegment[3];

                    segments[0] = new SwipeLeftReversedSegment3();
                    segments[1] = new SwipeLeftReversedSegment2();
                    segments[2] = new SwipeLeftReversedSegment1();
                    break;
                case GestureType.SwipeLeftSupport:
                    segments = new IGestureSegment[3];

                    segments[0] = new SwipeLeftReversedSegment1();
                    segments[1] = new SwipeLeftReversedSegment2();
                    segments[2] = new SwipeLeftReversedSegment3();
                    break;
                case GestureType.SwipeLeft:
                    segments = new IGestureSegment[3];

                    segments[0] = new SwipeLeftSegment1();
                    segments[1] = new SwipeLeftSegment2();
                    segments[2] = new SwipeLeftSegment3();
                    break;
                case GestureType.SwipeRightReversed:
                    segments = new IGestureSegment[3];

                    segments[0] = new SwipeRightSegment3();
                    segments[1] = new SwipeRightSegment2();
                    segments[2] = new SwipeRightSegment1();
                    break;
                case GestureType.SwipeRight:
                    segments = new IGestureSegment[3];

                    segments[0] = new SwipeRightSegment1();
                    segments[1] = new SwipeRightSegment2();
                    segments[2] = new SwipeRightSegment3();
                    break;
                case GestureType.ZoomIn:
                    segments = new IGestureSegment[3];

                    segments[0] = new ZoomInSegment1();
                    segments[1] = new ZoomInSegment2();
                    segments[2] = new ZoomInSegment3();
                    break;
                case GestureType.ZoomOut:
                    segments = new IGestureSegment[3];

                    segments[0] = new ZoomOutSegment1();
                    segments[1] = new ZoomOutSegment2();
                    segments[2] = new ZoomOutSegment3();
                    break;
                default:
                    break;
            }

            if (segments != null)
            {
                Gesture gesture = new Gesture(type, segments);
                gesture.GestureRecognized += OnGestureRecognized;

                _gestures.Add(gesture);
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Handles the GestureRecognized event of the g control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs"/> instance containing the event data.</param>
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            if (GestureRecognized != null)
            {
                GestureRecognized(this, e);
            }

            foreach (Gesture gesture in _gestures)
            {
                gesture.Reset();
            }
        }

        #endregion
    }
}
