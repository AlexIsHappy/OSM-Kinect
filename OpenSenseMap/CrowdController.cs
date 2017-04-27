using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Mouse Control
using System.Runtime.InteropServices;

//Kinect Libraries
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Wpf.Controls;
using LightBuzz.Vitruvius;

namespace OpenSenseMap
{
    public class CrowdController
    {
        public Body[] bodies;
        public Body mainBody;

        public bool newBody = false;
        public CrowdController() { }

        public void Update(Body[] trackedBodies)
        {
            bodies = trackedBodies;
        }

        public Body DetermineMainPerson()
        {
            Body currentBody = null;
            foreach (Body body in bodies)
            {
                if (body.IsTracked == false)
                {
                    continue;
                }

                if (mainBody == null)
                    mainBody = body;

                if (currentBody == null)
                    currentBody = body;

                if (EuclDistance(body) < EuclDistance(currentBody))
                {
                    currentBody = body;
                }
                if (mainBody != currentBody)
                {
                    newBody = true;
                }
                else
                {
                    newBody = false;
                }

                mainBody = currentBody;
            }

            return mainBody;
        }

        private double EuclDistance(Body body)
        {
            double distance = Math.Sqrt(Math.Pow(body.Joints[JointType.SpineMid].Position.X, 2) +
                Math.Pow(body.Joints[JointType.SpineMid].Position.Y, 2) +
                Math.Pow(body.Joints[JointType.SpineMid].Position.Z, 2));

            return distance;
        }
    }
}


