using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    internal static class InputAutomation
    {
        public static void LeftClick(Point point)
        {
            if (!SetCursorPos(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)))
                throw new InvalidOperationException("Unable to move mouse to " + point + ".");
            LeftClick();
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
        }

        public static void LeftClick()
        {
            ClickMouse(MouseEvent.LeftButtonDown);
            ClickMouse(MouseEvent.LeftButtonUp);
        }

        private static void ClickMouse(MouseEvent mouseEvent)
        {
            Input mouseInput = new Input
            {
                InputType = (int) InputType.Mouse,
                MouseInput = new MouseInput
                {
                    dwFlags = (int) mouseEvent
                }
            };

            Input[] inputs =
            {
                mouseInput
            };

            uint result = SendInput((uint) inputs.Length, inputs, Marshal.SizeOf(mouseInput));
            if (result == 0)
                throw new InvalidOperationException("Unable to click left mouse button.");
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);

        private struct Input
        {
            public uint InputType;
            public MouseInput MouseInput;
        }

        private enum InputType
        {
            Mouse = 0
        }

        private enum MouseEvent
        {
            LeftButtonDown = 0x2,
            LeftButtonUp = 0x4
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            private readonly int DeltaX;
            private readonly int DeltaY;
            private readonly int MouseData;
            public int dwFlags;
            private readonly int TimeStamp;
            private readonly IntPtr dwExtraInfo;
        }
    }
}