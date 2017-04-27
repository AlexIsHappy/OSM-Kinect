using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


//Kinect Libraries
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;
using Microsoft.Kinect.Input;
using LightBuzz.Vitruvius;
using System.Windows.Media.Media3D;

namespace OpenSenseMap
{
    public class MouseController
    {
        KinectSensor sensor;

        public Body trackedBody;
        public bool isPressedPosition = false;
        public bool isPressedHandPosition = false;

        public bool isRightHand = false;
        public bool wasUnderControl = false;

        public HandState trackedState;

        // Coordinates of the SpyGlass
        public int initX;
        public int initY;

        public int curX;
        public int curY;


        //Coordinates from the last frame
        public CameraSpacePoint lastHandPosition;

        public bool Update(Body body, KinectSensor sensor)
        {
            trackedBody = body;

            isPressedPosition = isHandPressPosition();

            return determineMainHand();
        }


        // Determine the main hand and If the state is pressed 
        private bool determineMainHand()
        {
            var rightHand = trackedBody.Joints[JointType.HandRight].Position.ToVector3();
            var leftHand = trackedBody.Joints[JointType.HandLeft].Position.ToVector3();

            return rightHand.Y > leftHand.Y ? true : false;
        }

        private bool isHandPressPosition()
        {
            var isRightHand = determineMainHand();
            var spineMid = trackedBody.Joints[JointType.SpineMid].Position.ToVector3();

            var controlHand = isRightHand == true ? trackedBody.Joints[JointType.HandRight].Position.ToVector3() : trackedBody.Joints[JointType.HandLeft].Position.ToVector3();
            var otherHand = isRightHand == true ? trackedBody.Joints[JointType.HandLeft].Position.ToVector3() : trackedBody.Joints[JointType.HandRight].Position.ToVector3();
            var controlShoulder = isRightHand == true ? trackedBody.Joints[JointType.ShoulderRight].Position.ToVector3() : trackedBody.Joints[JointType.ShoulderLeft].Position.ToVector3();


            if (controlHand.Y > spineMid.Y && (otherHand.Y < spineMid.Y) && (Math.Abs(controlHand.Z - controlShoulder.Z) > 0.30))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // Constructors
        public MouseController()
        {
        }

        public MouseController(int X, int Y)
        {
            initX = X;
            initY = Y;

            curX = X;
            curY = Y;
        }

    }
}
