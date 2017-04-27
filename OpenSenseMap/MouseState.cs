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
using LightBuzz.Vitruvius;


namespace OpenSenseMap
{
    public class MouseState
    {
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public bool isPressed = false;

        public static int framesFromClick = 0;

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

        public MouseState()
        {
        }

        public void MouseClick(int X, int Y)
        {
            mouse_event((int)MouseEventFlags.LeftDown, X, Y, 0, 0);
            mouse_event((int)MouseEventFlags.LeftUp, X, Y, 0, 0);
        }


        public void Press(int X, int Y)
        {
            mouse_event((int)MouseEventFlags.LeftDown, X, Y, 0, 0);
            isPressed = true;
        }

        // Release Events
        public void Release(int X, int Y)
        {
            mouse_event((int)MouseEventFlags.LeftUp, X, Y, 0, 0);
            isPressed = false;
        }

        public void Move(int X, int Y)
        {
            mouse_event((int)MouseEventFlags.Move, X, Y, 0, 0);
        }


        // Zooming event
        public void ZoomIn(int X, int Y)
        {
            MouseClick(X, Y);
        }

        public void ZoomOut(int X, int Y)
        {
            MouseClick(X, Y);
        }

    }
}
