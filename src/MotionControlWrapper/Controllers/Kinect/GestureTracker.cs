namespace NTNU.MotionControlWrapper.Controllers.Kinect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;

    public class GestureTracker : IDisposable
    {
        private VisualGestureBuilderFrameSource _gestureSource;
        private VisualGestureBuilderFrameReader _gestureReader;

        public GestureTracker(KinectSensor sensor, string gesturesDB)
        {
            _gestureSource = new VisualGestureBuilderFrameSource(sensor, 0);
            _gestureReader = _gestureSource.OpenReader();

            IsPaused = true;

            using (VisualGestureBuilderDatabase db = new VisualGestureBuilderDatabase(gesturesDB))
            {
                _gestureSource.AddGestures(db.AvailableGestures);
            }
        }

        ~GestureTracker()
        {
            Dispose();
        }

        public ulong TrackingId
        {
            get { return _gestureSource.TrackingId; }
            set { _gestureSource.TrackingId = value; }
        }

        public bool IsPaused
        {
            get { return _gestureReader.IsPaused; }
            set { _gestureReader.IsPaused = value; }
        }

        public void Dispose()
        {
            _gestureReader?.Dispose();
            _gestureReader = null;

            _gestureSource?.Dispose();
            _gestureSource = null;
        }

        public IList<GestureResult> PollMostRecentGestureFrame()
        {
            IList<GestureResult> result = new List<GestureResult>();

            using (VisualGestureBuilderFrame frame = _gestureReader.CalculateAndAcquireLatestFrame())
            {
                IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults
                    = frame?.DiscreteGestureResults;

                if (discreteResults == null)
                {
                    return result;
                }

                foreach (Gesture gesture in _gestureSource.Gestures.Where(
                    gesture => discreteResults.ContainsKey(gesture)))
                {
                    DiscreteGestureResult discreteGesture = discreteResults[gesture];

                    result.Add(new GestureResult(
                        gesture.Name,
                        discreteGesture.Confidence,
                        discreteGesture.Detected));
                }
            }

            return result;
        }
    }
}
