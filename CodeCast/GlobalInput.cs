using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeCast
{
    public static class GlobalInput
    {
        public static Point GetKeyboardCaretPosition()
        {
            // This does not work
            throw new NotImplementedException();

            var point = new Point();
            GetCaretPos(out point);

            var activeWindowRect = ScreenCapture.GetActiveWindowRectangle();

            return new Point(activeWindowRect.Left + point.X, activeWindowRect.Top + point.Y);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCaretPos(out Point lpPoint);


        public static Point GetKeyboardCaretPosition2()
        {
            // This does not work
            throw new NotImplementedException();

            var activeWindow = ScreenCapture.GetActiveWindow();

            // FROM: http://stackoverflow.com/questions/3072974/how-to-call-getguithreadinfo-in-c-sharp
            uint lpdwProcessId;
            uint threadId = GetWindowThreadProcessId(activeWindow, out lpdwProcessId);

            // FROM: http://www.codeproject.com/Articles/34520/Getting-Caret-Position-Inside-Any-Application
            var guiInfo = new GUITHREADINFO();
            guiInfo.cbSize = Marshal.SizeOf(guiInfo);

            // Get GuiThreadInfo into guiInfo
            GetGUIThreadInfo(threadId, out guiInfo);

            var caretPosition = new Point();

            ClientToScreen(guiInfo.hwndCaret, out caretPosition);

            return caretPosition;
        }



        public static Point GetMousePosition()
        {
            throw new NotImplementedException();
        }



        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GUITHREADINFO
        {
            public int cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;
        }

        [DllImport("user32.dll")]
        private static extern bool GetGUIThreadInfo(uint idThread, out GUITHREADINFO lpgui);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, out Point position);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

    }
}
