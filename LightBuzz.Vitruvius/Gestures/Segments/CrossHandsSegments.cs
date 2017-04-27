
using Microsoft.Kinect;

namespace LightBuzz.Vitruvius.Gestures
{
    /// <summary>
    /// The first part of a <see cref="GestureType.CrossHands"/> gesture.
    /// </summary>
    public class CrossHandsSegment1 : IGestureSegment
    {
        /// <summary>
        /// Updates the current gesture.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        /// <returns>A GesturePartResult based on whether the gesture part has been completed.</returns>
        public GesturePartResult Update(Body body)
        {
            // Hands very close
            if (body.Joints[JointType.HandLeft].Position.X - body.Joints[JointType.HandRight].Position.X > 0.15)
            {
                if(body.Joints[JointType.HandLeft].Position.Y > body.Joints[JointType.SpineMid].Position.Y 
                     && body.Joints[JointType.HandRight].Position.Y > body.Joints[JointType.SpineMid].Position.Y)
                {
                    return GesturePartResult.Succeeded;
                }
                return GesturePartResult.Undetermined;
            }

            return GesturePartResult.Failed;
        }
    }
}
