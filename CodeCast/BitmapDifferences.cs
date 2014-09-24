using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCast
{
    public static class BitmapDifferences
    {
        public static BitmapDiff GetDifferences(Bitmap a, Bitmap b, Rectangle areaToCheck)
        {
            //var DISTANCE = 3;
            // FORCE ONE REGION
            var DISTANCE = 250;
            var JUMPPIXELS = 7;
            //var MINCHANGESIZE = 0;

            var changedPixels = new List<PixelInfo>();

            unsafe
            {
                BitmapData aData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, a.PixelFormat);
                BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, b.PixelFormat);

                var iOffset = 0;
                var jOffset = 0;

                for (int iOuter = JUMPPIXELS / 2; iOuter < aData.Width || iOuter < bData.Width; iOuter += JUMPPIXELS)
                {
                    if (iOuter < areaToCheck.Left)
                    {
                        iOuter = areaToCheck.Left;
                    }

                    if (iOuter > areaToCheck.Right)
                    {
                        iOuter = aData.Width;
                        continue;
                    }

                    jOffset++;
                    jOffset %= JUMPPIXELS;

                    for (int j = jOffset; j < aData.Height || j < bData.Height; j += JUMPPIXELS)
                    {
                        var i = iOuter + iOffset;

                        if (j < areaToCheck.Top)
                        {
                            j = areaToCheck.Top;
                        }

                        if (j > areaToCheck.Bottom)
                        {
                            j = aData.Height;
                            continue;
                        }

                        if (i >= aData.Width
                            || i >= bData.Width
                            || j >= aData.Height
                            || j >= bData.Height)
                        {
                            changedPixels.Add(new PixelInfo(i, j, 255));
                            continue;
                        }

                        var aVal = GetPixelByte(aData, i, j, PixelBytePart.Blue);
                        var bVal = GetPixelByte(bData, i, j, PixelBytePart.Blue);

                        if (aVal != bVal)
                        {
                            changedPixels.Add(new PixelInfo(i, j, GetContrast(bData, i, j)));
                            //var n = ((byte*)bData.Scan0 + ((j +-3) * bData.Stride))[
                        }

                        iOffset++;
                        iOffset %= JUMPPIXELS;
                    }
                }

                a.UnlockBits(aData);
                b.UnlockBits(bData);
            }


            List<PixelRegion> regions = new List<PixelRegion>();
            var curRegion = new PixelRegion();
            regions.Add(curRegion);

            // Group the pixels into squares to create better regions
            var sqSize = (int)(DISTANCE * 0.5);
            var ordered = changedPixels
                .OrderBy(c => (int)(c.Y / sqSize))
                .ThenBy(c => (int)(c.X / sqSize));

            foreach (var p in ordered)
            {
                if (curRegion.IsNear(p, DISTANCE, true))
                {
                    curRegion.AddPixel(p);
                }
                else
                {
                    curRegion = new PixelRegion();
                    curRegion.AddPixel(p);
                    regions.Add(curRegion);
                }
            }

            //// Join regions
            //for (int iRegion = 0; iRegion < regions.Count; iRegion++)
            //{
            //    var region = regions[iRegion];
            //    var connectedRegions = regions.Skip(iRegion + 1).Where(oRegion => oRegion.Positions.Any(p => region.IsNear(p, DISTANCE))).ToList();

            //    // Merge
            //    if (connectedRegions.Any())
            //    {
            //        foreach (var cRegion in connectedRegions)
            //        {
            //            region.Merge(cRegion);
            //            regions.Remove(cRegion);
            //        }

            //        // Repeat with this region again
            //        iRegion--;
            //    }
            //}

            // Get bitmaps for regions

            // Remove too small of changes
            //regions = regions.Where(r => r.Right - r.Left > MINCHANGESIZE || r.Bottom - r.Top > MINCHANGESIZE).ToList();

            // Add JUMPPIXELS size border
            foreach (var r in regions)
            {
                if (r.IsEmpty)
                {
                    continue;
                }

                r.AddPixel(new PixelInfo(r.ChangeRect.Left - JUMPPIXELS, r.ChangeRect.Top - JUMPPIXELS, 0));
                r.AddPixel(new PixelInfo(r.ChangeRect.Right + JUMPPIXELS, r.ChangeRect.Bottom + JUMPPIXELS, 0));
            }

            return new BitmapDiff(regions, b);
        }

        private static byte GetContrast(BitmapData data, int x, int y)
        {
            var a = GetPixelByte(data, x - 4, y - 4, PixelBytePart.Gray);
            var b = GetPixelByte(data, x + 3, y + 3, PixelBytePart.Gray);
            var c = GetPixelByte(data, x - 2, y + 2, PixelBytePart.Gray);
            var d = GetPixelByte(data, x + 1, y - 1, PixelBytePart.Gray);
            var e = GetPixelByte(data, x, y, PixelBytePart.Gray);

            var min = Math.Min(a,
                Math.Min(b,
                Math.Min(c,
                Math.Min(d, e))));

            var max = Math.Max(a,
                Math.Max(b,
                Math.Max(c,
                Math.Max(d, e))));

            return (byte)(max - min);
        }

        unsafe private static byte GetPixelByte(BitmapData data, int x, int y, PixelBytePart part)
        {
            if (x < 0
                || y < 0
                || x >= data.Width
                || y >= data.Height)
            {
                return 0;
            }

            // This assumes 32bpp BGR_
            int PixelSize = 4;

            var row = (byte*)data.Scan0 + (y * data.Stride);
            var start = x * PixelSize;

            if (part != PixelBytePart.Gray)
            {
                return row[start + (int)part];
            }
            else
            {
                // FROM: http://stackoverflow.com/questions/6251599/how-to-convert-pixel-formats-from-32bpprgb-to-16bpp-grayscale-in-c-sharp
                return (byte)(row[start] * 0.11
                    + row[start + 1] * 0.59
                    + row[start + 2] * 0.3);
            }
        }

        private enum PixelBytePart
        {
            Blue = 0,
            Green = 1,
            Red = 2,
            Gray = 7
        }
    }

    public class PixelInfo
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public byte Contrast { get; private set; }

        public PixelInfo(int x, int y, byte contrast)
        {
            X = x;
            Y = y;
            Contrast = contrast;
        }
    }

    public class PixelRegion
    {
        public const byte HIGHCONTRASTLEVEL = 50;

        public List<PixelInfo> Pixels { get; private set; }

        public Rectangle ChangeRect { get; private set; }
        public Rectangle HighContrastRect { get; private set; }

        public bool IsEmpty { get { return ChangeRect.Width == 0; } }

        public void AddPixel(PixelInfo pixel)
        {
            Pixels.Add(pixel);

            ChangeRect = ExtendRect(ChangeRect, pixel.X, pixel.Y);

            if (pixel.Contrast >= HIGHCONTRASTLEVEL)
            {
                HighContrastRect = ExtendRect(HighContrastRect, pixel.X, pixel.Y);
            }

        }

        private Rectangle ExtendRect(Rectangle r, int x, int y)
        {
            if (r.Width == 0)
            {
                return new Rectangle(x, y, 1, 1);
            }

            var left = Math.Min(r.Left, x);
            var right = Math.Max(r.Right, x);
            var top = Math.Min(r.Top, y);
            var bottom = Math.Max(r.Bottom, y);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        public bool IsNear(PixelInfo pixel, int distance, bool shouldConsiderContrast)
        {
            if (ChangeRect.Width == 0)
            {
                return true;
            }

            if (shouldConsiderContrast && pixel.Contrast > HIGHCONTRASTLEVEL)
            {
                return pixel.X > HighContrastRect.Left - distance
                     && pixel.X < HighContrastRect.Right + distance
                     && pixel.Y > HighContrastRect.Top - distance
                     && pixel.Y < HighContrastRect.Bottom + distance;
            }
            else
            {
                return pixel.X > ChangeRect.Left - distance
                    && pixel.X < ChangeRect.Right + distance
                    && pixel.Y > ChangeRect.Top - distance
                    && pixel.Y < ChangeRect.Bottom + distance;
            }
        }

        public PixelRegion()
        {
            Pixels = new List<PixelInfo>();
            ChangeRect = new Rectangle();
            HighContrastRect = new Rectangle();
        }

        public void Merge(PixelRegion cRegion)
        {
            Pixels.AddRange(cRegion.Pixels);
            var highContrastPixels = Pixels.Where(p => p.Contrast >= HIGHCONTRASTLEVEL).ToList();

            ChangeRect = ExtendRect(ChangeRect, Pixels.Min(p => p.X), Pixels.Min(p => p.Y));
            ChangeRect = ExtendRect(ChangeRect, Pixels.Max(p => p.X), Pixels.Max(p => p.Y));

            HighContrastRect = ExtendRect(ChangeRect, highContrastPixels.Min(p => p.X), highContrastPixels.Min(p => p.Y));
            HighContrastRect = ExtendRect(ChangeRect, highContrastPixels.Max(p => p.X), highContrastPixels.Max(p => p.Y));
        }
    }

    //public class BitmapPart
    //{
    //    public Bitmap Image
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public Point Position { get; set; }
    //    public Size Size { get; set; }
    //    public bool IsEmpty { get { return Size.Width <= 0 || Size.Height <= 0; } }

    //    public BitmapPart(Bitmap b, PixelRegion r)
    //    {
    //        if (r.IsEmpty)
    //        {
    //            Position = new Point();
    //            Size = new Size();

    //            return;
    //        }

    //        var left = Math.Max(0, r.Left);
    //        var top = Math.Max(0, r.Top);
    //        var right = Math.Min(b.Width, r.Right);
    //        var bottom = Math.Min(b.Height, r.Bottom);

    //        Position = new Point(left, top);
    //        Size = new Size(right - left, bottom - top);

    //        //Image = b.Clone(new Rectangle(Position, Size), b.PixelFormat);
    //    }

    //}

    public class BitmapDiff
    {
        public List<PixelRegion> Parts { get; private set; }
        public Point FocalPoint { get; private set; }
        public Rectangle ChangeBounds { get; set; }

        public bool IsEmpty { get; private set; }

        public BitmapDiff(List<PixelRegion> regions, Bitmap b)
        {
            var parts = regions.Where(r => !r.IsEmpty).ToList();

            parts = parts.Where(p => !p.IsEmpty).ToList();
            Parts = parts;

            var hcParts = parts.Where(p => p.HighContrastRect.Width > 0).ToList();

            // Calculate focal point
            if (!hcParts.Any())
            {
                ChangeBounds = new Rectangle();
                FocalPoint = new Point();
                IsEmpty = true;
                return;
            }

            IsEmpty = false;

            
            //// Focus on the average of the greatest distribution that will fit in the region
            //var allChangedPixels = regions.SelectMany(r => r.Pixels).ToList();
            //var xDist = GetDistribution(allChangedPixels.Select(p => p.X));
            //var yDist = GetDistribution(allChangedPixels.Select(p => p.Y));

            //FocalPoint = new Point(xDist.Val_50, yDist.Val_50);
            //ChangeBounds = new Rectangle(xDist.Val_0, yDist.Val_0, xDist.Val_100, yDist.Val_100);


            var left = hcParts.Min(p => p.HighContrastRect.Left);
            var top = hcParts.Min(p => p.HighContrastRect.Top);
            var right = hcParts.Max(p => p.HighContrastRect.Right);
            var bottom = hcParts.Max(p => p.HighContrastRect.Bottom);

            ChangeBounds = new Rectangle(left, top, right - left, bottom - top);
            FocalPoint = new Point(ChangeBounds.Left, ChangeBounds.Top);

        }



        private static DistributedValues<T> GetDistribution<T>(IEnumerable<T> values)
        {
            var ordered = values.OrderBy(v => v).ToList();
            var count = ordered.Count;

            Func<double, T> doGetValueAtIndexRatio = (ratio) =>
            {
                var index = (int)(ratio * count);

                if (index >= count)
                {
                    index = count - 1;
                }

                return ordered[index];
            };

            return new DistributedValues<T>()
            {
                Val_0 = doGetValueAtIndexRatio(0),
                Val_10 = doGetValueAtIndexRatio(0.1),
                Val_30 = doGetValueAtIndexRatio(0.3),
                Val_50 = doGetValueAtIndexRatio(0.5),
                Val_70 = doGetValueAtIndexRatio(0.7),
                Val_90 = doGetValueAtIndexRatio(0.9),
                Val_100 = doGetValueAtIndexRatio(1),
            };
        }

        private struct DistributedValues<T>
        {
            public T Val_0;
            public T Val_10;
            public T Val_30;
            public T Val_50;
            public T Val_70;
            public T Val_90;
            public T Val_100;
        }
    }
}
