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
        // FROM: http://stackoverflow.com/questions/3072349/capture-screenshot-including-semitransparent-windows-in-net
        public static BitmapWithRaw CaptureAllScreens()
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

            return new BitmapWithRaw(bmp, hBmp, hOldBmp, hDest, hSrce);
        }

        public class BitmapWithRaw : IDisposable
        {
            public Bitmap Bitmap { get; private set; }
            public IntPtr Hbitmap { get; private set; }
            private IntPtr _hOldBmp;
            private IntPtr _hDest;
            private IntPtr _hSrce;

            public BitmapWithRaw(Bitmap bitmap, IntPtr hbitmap, IntPtr hOldBmp, IntPtr hDest, IntPtr hSrce)
            {
                Hbitmap = hbitmap;
                Bitmap = bitmap;

                _hOldBmp = hOldBmp;
                _hDest = hDest;
                _hSrce = hSrce;
            }

            public void Dispose()
            {
                SelectObject(_hDest, _hOldBmp);
                DeleteObject(Hbitmap);
                DeleteDC(_hDest);
                ReleaseDC(_hDest, _hSrce);
                Bitmap.Dispose();
            }
        }

        // P/Invoke declarations
        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
        wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr ptr);
    }

    
}
