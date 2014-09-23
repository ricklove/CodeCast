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
        public static BitmapDiff GetDifferences(Bitmap a, Bitmap b)
        {
            //var DISTANCE = 3;
            // FORCE ONE REGION
            var DISTANCE = 250;
            var JUMPPIXELS = 13;

            var changedPixels = new List<Point>();

            unsafe
            {
                BitmapData aData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, a.PixelFormat);
                BitmapData bData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, b.PixelFormat);

                int PixelSize = 4;

                for (int i = 0; i < aData.Width || i < bData.Width; i += JUMPPIXELS)
                {
                    for (int j = 0; j < aData.Height || j < bData.Height; j += JUMPPIXELS)
                    {
                        if (i >= aData.Width
                            || i >= bData.Width
                            || j >= aData.Height
                            || j >= bData.Height)
                        {
                            changedPixels.Add(new Point(i, j));
                            continue;
                        }

                        var aRow = (byte*)aData.Scan0 + (j * aData.Stride);
                        var bRow = (byte*)bData.Scan0 + (j * bData.Stride);

                        var aVal = aRow[i * PixelSize];
                        var bVal = bRow[i * PixelSize];

                        if (aVal != bVal)
                        {
                            changedPixels.Add(new Point(i, j));
                        }
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
                if (curRegion.IsNear(p, DISTANCE))
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

            // Add JUMPPIXELS size border
            foreach (var r in regions)
            {
                r.AddPixel(new Point(r.Left - JUMPPIXELS, r.Top - JUMPPIXELS));
                r.AddPixel(new Point(r.Right + JUMPPIXELS, r.Bottom + JUMPPIXELS));
            }

            return new BitmapDiff(regions.Where(r => r.Right > r.Left && r.Bottom > r.Top).Select(r => new BitmapPart(b, r)).ToList());
        }
    }

    public class PixelRegion
    {
        public List<Point> Positions { get; private set; }

        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }

        public bool IsEmpty { get { return Left == int.MaxValue; } }

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
        public Bitmap Image
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Point Position { get; set; }
        public Size Size { get; set; }
        public bool IsEmpty { get { return Size.Width <= 0 || Size.Height <= 0; } }

        public BitmapPart(Bitmap b, PixelRegion r)
        {
            if (r.IsEmpty)
            {
                Position = new Point();
                Size = new Size();

                return;
            }

            var left = Math.Max(0, r.Left);
            var top = Math.Max(0, r.Top);
            var right = Math.Min(b.Width, r.Right);
            var bottom = Math.Min(b.Height, r.Bottom);

            Position = new Point(left, top);
            Size = new Size(right - left, bottom - top);

            //Image = b.Clone(new Rectangle(Position, Size), b.PixelFormat);
        }

    }

    public class BitmapDiff
    {
        public List<BitmapPart> Parts { get; private set; }
        public Point FocalPoint { get; private set; }
        public Rectangle ChangeBounds { get; set; }

        public BitmapDiff(List<BitmapPart> parts)
        {
            parts = parts.Where(p => !p.IsEmpty).ToList();
            Parts = parts;

            // Calculate focal point
            if (!parts.Any())
            {
                ChangeBounds = new Rectangle();
                FocalPoint = new Point();
                return;
            }

            var left = parts.Min(p => p.Position.X);
            var top = parts.Min(p => p.Position.Y);
            var right = parts.Max(p => p.Position.X + p.Size.Width);
            var bottom = parts.Max(p => p.Position.Y + p.Size.Height);

            ChangeBounds = new Rectangle(left, top, right - left, bottom - top);

            // TODO: Determine a better focal point
            FocalPoint = new Point(ChangeBounds.Left + ChangeBounds.Width / 2, ChangeBounds.Top + ChangeBounds.Height / 2);

            // TODO: Make a method to get the percentile values at (0.1, 0.3, 0.5, 0.7, 0.9) to calculate the distribution
            // Focus on the average of the greatest distribution that will fit in the region

            //var pX = from p in parts
            //         orderby p.Position.X
            //         select p.Position.X;

            //var sX = new { A=p };
        }
    }
}
