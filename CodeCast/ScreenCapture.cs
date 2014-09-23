using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeCast
{
    public static class ScreenCapture
    {
        public static Rectangle GetActiveWindowRectangle()
        {
            var window = GetActiveWindow();
            var rect = new RECT();
            GetWindowRect(window, out rect);

            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        // FROM: http://stackoverflow.com/questions/5878963/getting-active-window-coordinates-and-height-width-in-c-sharp
        public static IntPtr GetActiveWindow()
        {
            return GetForegroundWindow();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;        // x position of upper-left corner  
            public int Top;         // y position of upper-left corner  
            public int Right;       // x position of lower-right corner  
            public int Bottom;      // y position of lower-right corner  
        }


        // FROM: http://stackoverflow.com/questions/3072349/capture-screenshot-including-semitransparent-windows-in-net
        public static Bitmap CaptureAllScreens()
        {
            Point posTopLeft = new Point(Screen.AllScreens.Min(s => s.Bounds.Left), Screen.AllScreens.Min(s => s.Bounds.Top));
            Point posBottomRight = new Point(Screen.AllScreens.Max(s => s.Bounds.Right), Screen.AllScreens.Max(s => s.Bounds.Bottom));
            Size sz = new Size(posBottomRight.X - posTopLeft.X, posBottomRight.Y - posTopLeft.Y);

            IntPtr hDesk = GetDesktopWindow();
            IntPtr hSrce = GetWindowDC(hDesk);
            IntPtr hDest = CreateCompatibleDC(hSrce);
            IntPtr hBmp = CreateCompatibleBitmap(hSrce, sz.Width, sz.Height);
            IntPtr hOldBmp = SelectObject(hDest, hBmp);
            bool b = BitBlt(hDest, 0, 0, sz.Width, sz.Height, hSrce, posTopLeft.X, posTopLeft.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
            Bitmap bmp = Bitmap.FromHbitmap(hBmp);
            SelectObject(hDest, hOldBmp);
            DeleteObject(hBmp);
            DeleteDC(hDest);
            ReleaseDC(hDesk, hSrce);

            return bmp;
        }

        // P/Invoke declarations
        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
        wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr ptr);
    }


}
