using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Text.RegularExpressions;

// Mouse Control
using System.Runtime.InteropServices;

//Kinect Libraries
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Wpf.Controls;
using LightBuzz.Vitruvius;

using CefSharp;
using CefSharp.Wpf;

namespace OpenSenseMap
{
    public partial class MainWindow : Window
    {

        #region MouseControl
        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {

            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);


        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        #endregion


        void switchVisibility()
        {
            if(!swipe_mode)
            {
                webControl.Opacity = 0.2;
                webControl.Background = Brushes.Black;
                light_image.Visibility = Visibility.Visible;
                swipeAnimation.Visibility = Visibility.Visible;
                zoomAnimation.Visibility = Visibility.Hidden;
                viewer.Visibility = Visibility.Hidden;

                swipe_mode = true;
                Mouse.OverrideCursor = Cursors.None;
            }
            else
            {
                webControl.Opacity = 1;
                webControl.Background = null;
                light_image.Visibility = Visibility.Hidden;
                swipeAnimation.Visibility = Visibility.Hidden;
                zoomAnimation.Visibility = Visibility.Visible;
                viewer.Visibility = Visibility.Visible;

                swipe_mode = false;
                Mouse.OverrideCursor = Cursors.Arrow;
                current_picture = -1;
            }
        }


        KinectSensor sensor;
        MultiSourceFrameReader reader;
        PlayersController playersController;

        //GestureController
        GestureController gestureController;

        //Utlity Controllers: Crowd - for calculating closest person;
        CrowdController crowdController = new CrowdController();

        // Data got from those sources
        Body[] bodies = null;

        MouseController mouseController;
        MouseState mouseState = new MouseState();
        KinectPointerPoint kinectPointerPoint;

        // List of pictures
        List<string> sensors_pictures;
        int current_picture = -1;

        // Flag Values
        bool wasZoomed = false;
        bool swipe_mode = false;

        string base_folder = "";


        int sinceLastGesture = 0;

        public MainWindow()
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            Cef.Initialize(settings);

            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            sensor = KinectSensor.GetDefault();
            //Mouse.OverrideCursor = Cursors.None;

            if (sensor != null)
            {
                sensor.Open();

                reader = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

                //Initialize gestureController
                gestureController = new GestureController();
                gestureController.Start();
                gestureController.GestureRecognized += gestureController_GestureRecognized;

                //Initialize spyGlassController
                mouseController = new MouseController((int)SystemParameters.PrimaryScreenWidth / 2, (int)SystemParameters.PrimaryScreenHeight/2);   //961,600 for Display / 829,586 for local testing

                playersController = new PlayersController();
                playersController.BodyEntered += UserReporter_BodyEntered;
                playersController.BodyLeft += UserReporter_BodyLeft;
                playersController.Start();

                KinectCoreWindow kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
                kinectCoreWindow.PointerMoved += kinectCoreWindow_PointerMoved;

                string folder = System.IO.Directory.GetCurrentDirectory().ToString();

                for (int i = 0; i < folder.Length; i++)
                {
                    if (folder[i] == '\\')
                    {
                        base_folder += '\\';
                        base_folder += '\\';
                    }
                    else
                        base_folder += folder[i];
                }
                base_folder += "\\Images";

                Console.WriteLine(base_folder);
            }
        }

        // Close the window on ESC
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void kinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs args)
        {
            if (crowdController.mainBody != null)
            {
                if (args.CurrentPoint.Properties.BodyTrackingId == crowdController.mainBody.TrackingId)
                {
                    kinectPointerPoint = args.CurrentPoint;
                }
            }
        }

        void gestureController_GestureRecognized(object sender, GestureEventArgs e)
        {
            if (e.GestureType == GestureType.ZoomIn)
            {
                SetCursorPos(10, 70);
                mouseState.MouseClick(10, 60);
                wasZoomed = true;
                zoomAnimation.Visibility = Visibility.Hidden;
            }
            else if (e.GestureType == GestureType.ZoomOut)
            {
                SetCursorPos(10, 100);
                mouseState.MouseClick(10, 90);
                wasZoomed = true;
                zoomAnimation.Visibility = Visibility.Hidden;
            }
            else if(e.GestureType == GestureType.CrossedHands)
            {
                if (sinceLastGesture == 0)
                {
                    if (!isRefreshed)
                    {
                        restart_view();
                    }
                    sinceLastGesture = 45; // Wait 100 frames until we can swith view again
                }
            }
            else if (e.GestureType == GestureType.HandsAboveHead)
            {
                if(sinceLastGesture == 0)
                {
                    switchVisibility();

                    if (swipe_mode)
                    {
                        // Get all the pictures from the folder
                        try
                        {
                            sensors_pictures = new List<string>(System.IO.Directory.EnumerateFiles(base_folder, "*.jpg"));

                            if (sensors_pictures.Count != 0)
                            {
                                Console.WriteLine(sensors_pictures.Count);
                                current_picture = 0;
                                light_image.Source = new BitmapImage(new Uri(sensors_pictures[current_picture]));
                            }
                        }
                        catch { }
                    }

                    sinceLastGesture = 45; // Wait 100 frames until we can swith view again
                }
            }
            else if (e.GestureType == GestureType.SwipeLeft || e.GestureType == GestureType.SwipeRightReversed || e.GestureType == GestureType.SwipeLeftSupport)
            {
                Console.WriteLine(e.GestureType);
                if (current_picture != -1)
                {
                    current_picture++;
                    current_picture %= sensors_pictures.Count;
                    light_image.Source = new BitmapImage(new Uri(sensors_pictures[current_picture]));
                }
            }
            else if (e.GestureType == GestureType.SwipeRight || e.GestureType == GestureType.SwipeLeftReversed)
            {

                if (current_picture != -1)
                {
                    current_picture--;
                    if (current_picture == -1)
                    {
                        current_picture = sensors_pictures.Count - 1;
                    }
                    light_image.Source = new BitmapImage(new Uri(sensors_pictures[current_picture]));
                }
            }
        }

        #region Utility Functions, which help controlling mouse
        public static void SetCursorPosition(int X, int Y)
        {
            SetCursorPos(X, Y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        #endregion

        public bool isRefreshed = true;

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (viewer.Visualization == Visualization.Color)
                    {
                        viewer.Image = frame.ToBitmap();
                    }
                }
            }

            // Depth
            using (var frame = reference.DepthFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (viewer.Visualization == Visualization.Depth)
                    {
                        viewer.Image = frame.ToBitmap();
                    }
                }
            }

            // Infrared
            using (var frame = reference.InfraredFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (viewer.Visualization == Visualization.Infrared)
                    {
                        viewer.Image = frame.ToBitmap();
                    }
                }
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    // Wait 100 frames until we can detect new gesture
                    if (sinceLastGesture > 0)
                    {
                        sinceLastGesture--;
                    }

                    bodies = new Body[frame.BodyFrameSource.BodyCount];
                    frame.GetAndRefreshBodyData(bodies);
                    playersController.Update(bodies);

                    foreach (Body body in bodies)
                    {
                        viewer.DrawBody(body);
                    }


                    int trackedBodies = bodies.Count(x => x.IsTracked == true);


                    if (trackedBodies > 0)
                    {
                       // text1.Text = (bodies.Where(x => x.IsTracked == true).ToList()[0].Joints[JointType.HandRight].Position.X - bodies.Where(x => x.IsTracked == true).ToList()[0].Joints[JointType.HandLeft].Position.X).ToString();
                        isRefreshed = false;
                        crowdController.Update(bodies);
                        gestureController.Update(crowdController.DetermineMainPerson());

                        if (!swipe_mode)
                            setCursor();
                    }
                    else
                    {
                        if (!isRefreshed)
                        {
                            restart_view();
                        }
                    }
                }
            }
        }


        public double lastX = 0;
        public double lastY = 0;
        List<int> positionsX = new List<int>();
        List<int> positionsY = new List<int>();

        public void restart_view()
        {
            Regex _regex = new Regex(@"/\bexplore\b/");
            Match match = _regex.Match(webControl.Address.ToString().ToLower());

            if (match.Success)
            {
                webControl.Back();
                webControl.Reload(false);
            }
            else
            {
                webControl.Reload(false);
            }

            isRefreshed = true;
            zoomAnimation.Visibility = Visibility.Visible;
        }


        public void setCursor()
        {
            mouseController.Update(crowdController.DetermineMainPerson(), sensor);

            JointType mainHand = mouseController.Update(crowdController.DetermineMainPerson(), sensor) ? JointType.HandRight : JointType.HandLeft;

            // 3D space point
            CameraSpacePoint jointPosition = crowdController.DetermineMainPerson().Joints[mainHand].Position;
            ColorSpacePoint colorPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);

            if (wasZoomed)
            {
                SetCursorPosition(mouseController.initX, mouseController.initY);
                mouseState.Press(mouseController.initX, mouseController.initY);

                mouseController.curX = mouseController.initX;
                mouseController.curY = mouseController.initY;
                wasZoomed = false;
                return;
            }


            if (mouseController.isPressedPosition)
            {
                if (!mouseState.isPressed)
                {
                    SetCursorPosition(mouseController.initX, mouseController.initY);
                    mouseState.Press(mouseController.initX, mouseController.initY);

                    mouseController.curX = mouseController.initX;
                    mouseController.curY = mouseController.initY;
                }
                else
                {
                    double x = colorPoint.X;
                    double y = colorPoint.Y;

                    int diffX = (int)((x - lastX) * 1.3);
                    int diffY = (int)((y - lastY) * 1.3);

                    mouseController.curX += diffX;
                    mouseController.curY += diffY;

                   // text1.Text = diffX.ToString() + " - " + diffY.ToString();

                    SetCursorPosition(mouseController.curX, mouseController.curY);
                }
            }
            else
            {
                mouseState.Release(mouseController.curX, mouseController.curY);
                mouseController.curX = mouseController.initX;
                mouseController.curY = mouseController.initY;
            }


            lastX = colorPoint.X;
            lastY = colorPoint.Y;


        }



        public bool isSorted(List<double> Positions)
        {
            bool order = Positions.First() < Positions.Last() ? true : false;  // If first element is less than last: than order is ascending. Otherwise descending

            for (int i = 1; i < Positions.Count; i++)
            {
                if (order)
                {
                    if (Positions[i] < Positions[i - 1])
                        return false;
                }
                else
                {
                    if (Positions[i] > Positions[i - 1])
                        return false;
                }
            }
            return true;
        }


        void UserReporter_BodyEntered(object sender, UsersControllerEventArgs e)
        {
            // A new user has entered the scene.
        }

        void UserReporter_BodyLeft(object sender, UsersControllerEventArgs e)
        {
            // A user has left the scene.
            viewer.Clear();
        }
    }
}
