using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCast
{
    public static class BitmapDifferences
    {
        public static List<BitmapPart> GetDifferences(ScreenCapture.BitmapWithRaw bitmapA, ScreenCapture.BitmapWithRaw bitmapB)
        {
            var DISTANCE = 3;

            var a = bitmapA.Hbitmap;
            var b = bitmapB.Hbitmap;

            var aWidth = bitmapA.Bitmap.Width;
            var bWidth = bitmapB.Bitmap.Width;
            var aHeight = bitmapA.Bitmap.Height;
            var bHeight = bitmapB.Bitmap.Height;

            var aCount = bitmapA.Bitmap.Width * bitmapA.Bitmap.Height;
            var bCount = bitmapB.Bitmap.Width * bitmapB.Bitmap.Height;

            List<Point> changedPixels = new List<Point>();

            for (int i = 0; i < aWidth || i < bWidth; i++)
            {
                for (int j = 0; j < aHeight || j < bHeight; j++)
                {
                    if (i >= aWidth
                        || i >= bWidth
                        || j >= aHeight
                        || j >= bHeight)
                    {
                        changedPixels.Add(new Point(i, j));
                        continue;
                    }

                    var aPtr = a + i + j * aWidth;
                    var bPtr = b + i + j * bWidth;

                    var aVal = System.Runtime.InteropServices.Marshal.ReadInt32(aPtr);
                    var bVal = System.Runtime.InteropServices.Marshal.ReadInt32(bPtr);

                    if (aVal != bVal)
                    {
                        changedPixels.Add(new Point(i, j));
                    }
                }
            }

            //for (int i = 0; i < a.Width || i < b.Width; i++)
            //{
            //    for (int j = 0; j < a.Height || j < b.Height; j++)
            //    {
            //        if (i >= a.Width
            //            || i >= b.Width
            //            || j >= a.Height
            //            || j >= b.Height)
            //        {
            //            changedPixels.Add(new Point(i, j));
            //            continue;
            //        }

            //        if (a.GetPixel(i, j) != b.GetPixel(i, j))
            //        {
            //            changedPixels.Add(new Point(i, j));
            //        }
            //    }
            //}


            List<PixelRegion> regions = new List<PixelRegion>();
            var curRegion = new PixelRegion();
            regions.Add(curRegion);

            foreach (var p in changedPixels)
            {
                if (curRegion.IsNear(p, DISTANCE))
                {
                    curRegion.AddPixel(p);
                }
            }

            // Join regions
            for (int iRegion = 0; iRegion < regions.Count; iRegion++)
            {
                var region = regions[iRegion];
                var connectedRegions = regions.Skip(iRegion).Where(oRegion => oRegion.Positions.Any(p => region.IsNear(p, DISTANCE)));

                // Merge
                if (connectedRegions.Any())
                {
                    foreach (var cRegion in connectedRegions)
                    {
                        region.Merge(cRegion);
                        regions.Remove(cRegion);
                    }

                    // Repeat with this region again
                    iRegion--;
                }
            }

            // Get bitmaps for regions

            return regions.Select(r => new BitmapPart(bitmapB.Bitmap, r)).ToList();
        }
    }

    public class PixelRegion
    {
        public List<Point> Positions { get; private set; }

        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }

        public void AddPixel(Point position)
        {
            Positions.Add(position);

            Left = Math.Min(Left, position.X);
            Right = Math.Max(Right, position.X);
            Top = Math.Min(Top, position.Y);
            Bottom = Math.Max(Bottom, position.Y);
        }

        public bool IsNear(Point position, int distance)
        {
            if (Left == int.MaxValue)
            {
                return true;
            }

            return position.X > Left - distance
                && position.X < Right + distance
                && position.Y > Top - distance
                && position.Y < Bottom + distance;
        }

        public PixelRegion()
        {
            Positions = new List<Point>();
            Left = int.MaxValue;
            Right = int.MinValue;
            Top = int.MaxValue;
            Bottom = int.MinValue;
        }

        public void Merge(PixelRegion cRegion)
        {
            Positions.AddRange(cRegion.Positions);

            Left = Positions.Min(p => p.X);
            Right = Positions.Min(p => p.Y);
            Top = Positions.Max(p => p.X);
            Bottom = Positions.Max(p => p.Y);
        }
    }

    public class BitmapPart
    {
        public Bitmap Image { get; set; }
        public Point Position { get; set; }
        public Size Size { get; set; }

        public BitmapPart(Bitmap b, PixelRegion r)
        {
            Position = new Point(r.Left, r.Top);
            Size = new Size(r.Right - r.Left, r.Bottom - r.Top);

            Image = b.Clone(new Rectangle(Position, Size), b.PixelFormat);
        }

    }
}
