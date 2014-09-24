using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCast
{
    partial class ScreenCastFrameMaker
    {
        public Bitmap CreateFrameZoomStretch(Size targetSize, Bitmap wholeScreen, BitmapDiff diff, string comment, Font font)
        {
            // Divide the screen into splits for each change rect
            var changeRects = diff.Parts.Where(p => p.HighContrastRect.Width > 0).Select(p => p.HighContrastRect).ToList();

            if (!changeRects.Any())
            {
                return null;
            }

            // Round off changerects
            var round = 60;

            changeRects = changeRects.Select(r => new Rectangle(
                round * (r.Left / round),
                round * (r.Top / round),
                round * (int)Math.Ceiling(1.0 * r.Width / round),
                round * (int)Math.Ceiling(1.0 * r.Height / round)
                )).ToList();

            // Join touching changeRects
            changeRects = JoinTouchingRects(changeRects);

            // TODO: Change to target coordinates first

            // Divide into partitions
            var partitionsGroups = SubdivideIntoGroups(changeRects, new Rectangle(0, 0, wholeScreen.Width, wholeScreen.Height));

            if (partitionsGroups == null)
            {
                var breakdance = true;
            }

            List<RectPartition> partitions = new List<RectPartition>();
            GetPartitionLeaves(partitionsGroups, partitions);

            // DEBUG Draw partitions
            var shouldDrawPartitions = false;
            if (shouldDrawPartitions)
            {
                var frameDebug = new Bitmap(wholeScreen);
                using (var g = Graphics.FromImage(frameDebug))
                {
                    foreach (var p in partitions)
                    {
                        g.DrawRectangle(diffPen, p.Whole);
                        g.DrawRectangle(diffHighPen, p.Item);
                    }
                }

                return frameDebug;
            }

            // Expand inside each partition (undistort items back to original aspect ratio)
            var frame = new Bitmap(wholeScreen.Width, wholeScreen.Height);
            using (var g = Graphics.FromImage(frame))
            {
                foreach (var p in partitions)
                {
                    DrawPartition(g, p, wholeScreen);
                }
            }

            return frame;
        }

        private void DrawPartition(Graphics g, RectPartition p, Bitmap image)
        {
            // Draw 9 (3x3) squares with middle stretched as much as possible
            var sPadL = p.Item.Left - p.Whole.Left;
            var sPadT = p.Item.Top - p.Whole.Top;
            var sPadR = -p.Item.Right + p.Whole.Right;
            var sPadB = -p.Item.Bottom + p.Whole.Bottom;

            // TODO: Increment center size to proportional multiple

            var dPadL = (int)(sPadL * 0.75f);
            var dPadT = (int)(sPadT * 0.75f);
            var dPadR = (int)(sPadR * 0.75f);
            var dPadB = (int)(sPadB * 0.75f);

            var sx = p.Whole.Left;
            var sy = p.Whole.Top;

            var dx = sx;
            var dy = sy;

            var sw0 = sPadL;
            var sw1 = p.Item.Width;
            var sw2 = sPadR;

            var sx0 = sx;
            var sx1 = sx0 + sw0;
            var sx2 = sx1 + sw1;
            var sx3 = sx2 + sw2;

            var sh0 = sPadT;
            var sh1 = p.Item.Height;
            var sh2 = sPadB;

            var sy0 = sy;
            var sy1 = sy0 + sh0;
            var sy2 = sy1 + sh1;
            var sy3 = sy2 + sh2;

            var dw0 = dPadL;
            var dw1 = p.Whole.Width - (dPadL + dPadR);
            var dw2 = dPadR;

            var dx0 = dx;
            var dx1 = dx0 + dw0;
            var dx2 = dx1 + dw1;
            var dx3 = dx2 + dw2;

            var dh0 = dPadT;
            var dh1 = p.Whole.Height - (dPadT + dPadB);
            var dh2 = dPadB;

            var dy0 = dy;
            var dy1 = dy0 + dh0;
            var dy2 = dy1 + dh1;
            var dy3 = dy2 + dh2;


            // TOP ROW
            g.DrawImage(image,
                new Rectangle(dx0, dy0, dw0, dh0),
                new Rectangle(sx0, sy0, sw0, sh0),
                GraphicsUnit.Pixel);

            g.DrawImage(image,
                new Rectangle(dx1, dy0, dw1, dh0),
                new Rectangle(sx1, sy0, sw1, sh0),
                GraphicsUnit.Pixel);

            g.DrawImage(image,
                new Rectangle(dx2, dy0, dw2, dh0),
                new Rectangle(sx2, sy0, sw2, sh0),
                GraphicsUnit.Pixel);


            // MIDDLE ROW
            g.DrawImage(image,
                new Rectangle(dx0, dy1, dw0, dh1),
                new Rectangle(sx0, sy1, sw0, sh1),
                GraphicsUnit.Pixel);

            // CENTER
            g.DrawImage(image,
                new Rectangle(dx1, dy1, dw1, dh1),
                new Rectangle(sx1, sy1, sw1, sh1),
                GraphicsUnit.Pixel);

            g.DrawImage(image,
                new Rectangle(dx2, dy1, dw2, dh1),
                new Rectangle(sx2, sy1, sw2, sh1),
                GraphicsUnit.Pixel);


            // BOTTOM ROW
            g.DrawImage(image,
                new Rectangle(dx0, dy2, dw0, dh2),
                new Rectangle(sx0, sy2, sw0, sh2),
                GraphicsUnit.Pixel);

            g.DrawImage(image,
                new Rectangle(dx1, dy2, dw1, dh2),
                new Rectangle(sx1, sy2, sw1, sh2),
                GraphicsUnit.Pixel);

            g.DrawImage(image,
                new Rectangle(dx2, dy2, dw2, dh2),
                new Rectangle(sx2, sy2, sw2, sh2),
                GraphicsUnit.Pixel);

            // Highlite center
            g.DrawRectangle(diffHighPen, new Rectangle(dx1, dy1, dw1, dh1));
        }

        private static List<Rectangle> JoinTouchingRects(List<Rectangle> changeRects)
        {
            changeRects = changeRects.ToList();

            for (int i = 0; i < changeRects.Count; i++)
            {
                for (int j = i + 1; j < changeRects.Count; j++)
                {
                    var a = changeRects[i];
                    var b = changeRects[j];

                    if (a.IntersectsWith(b))
                    {
                        var joined = JoinRects(a, b);
                        changeRects[i] = joined;
                        changeRects.RemoveAt(j);

                        // Start over
                        i = -1;
                        break;
                    }
                }
            }

            for (int i = 0; i < changeRects.Count; i++)
            {
                for (int j = i + 1; j < changeRects.Count; j++)
                {
                    var a = changeRects[i];
                    var b = changeRects[j];

                    if (a.IntersectsWith(b))
                    {
                        throw new Exception("LOGIC ERROR: Not joining all rects correctly");
                    }

                }

            }

            return changeRects;
        }

        private static Rectangle JoinRects(List<Rectangle> rects)
        {
            if (rects.Count == 0)
            {
                throw new ArgumentException("Cannot join no rects");
            }

            // TODO: Just do min and max on each part
            var r = rects.First();

            foreach (var rNext in rects.Skip(1))
            {
                r = JoinRects(r, rNext);
            }

            return r;
        }

        private static Rectangle JoinRects(Rectangle a, Rectangle b)
        {
            var l = Math.Min(a.Left, b.Left);
            var t = Math.Min(a.Top, b.Top);
            var r = Math.Max(a.Right, b.Right);
            var bot = Math.Max(a.Bottom, b.Bottom);
            var joined = new Rectangle(l, t, r - l, bot - t);
            return joined;
        }

        private void GetPartitionLeaves(RectPartition partition, List<RectPartition> leaves)
        {
            if (partition.Item != null)
            {
                leaves.Add(partition);
            }
            else
            {
                GetPartitionLeaves(partition.SideA, leaves);
                GetPartitionLeaves(partition.SideB, leaves);
            }
        }

        private RectPartition SubdivideIntoGroups(List<Rectangle> items, Rectangle whole)
        {
            // Binary space partition (using only horizontal and vertical divisions)
            // Recursive subdivision by hyperplanes (or lines in this case)
            // 3d Tutorial: http://dip.sun.ac.za/~henri/bsp_tutorial.pdf
            // Search: 2d recursive subdivision
            // http://salikscodingblog.wordpress.com/2011/08/04/polygon-subdivision/

            if (items.Count == 0)
            {
                throw new ArgumentException("Must provide items to subdivide");
            }

            if (items.Count == 1)
            {
                return new RectPartition() { Item = items.First(), Whole = whole };
            }

            var result = SubdivideIntoGroups(items, whole, false);

            if (result == null)
            {
                result = SubdivideIntoGroups(items, whole, true);
            }

            if (result == null)
            {
                // The pieces should join as one because they are blocking each other
                return new RectPartition() { Item = JoinRects(items), Whole = whole };
            }

            return result;
        }

        private RectPartition SubdivideIntoGroups(List<Rectangle> items, Rectangle whole, bool useXAxis)
        {
            if (items.Count == 1)
            {
                throw new ArgumentException("Can only subdivide many items");
            }

            // Find a set that can be subdivided

            var solidRangesOnAxis = useXAxis
                ? items.Select(r => new { a = r.Left, b = r.Right }).ToList()
                : items.Select(r => new { a = r.Top, b = r.Bottom }).ToList();

            var connected = solidRangesOnAxis.ToList();
            connected.Clear();

            foreach (var r in solidRangesOnAxis.OrderBy(s => s.a))
            {
                var wasConnected = false;

                for (int i = 0; i < connected.Count; i++)
                {
                    var c = connected[i];

                    if (!(c.a > r.b || r.a > c.b))
                    {
                        var newC = new { a = Math.Min(c.a, r.a), b = Math.Max(c.b, r.b) };
                        connected[i] = newC;
                        wasConnected = true;
                        break;
                    }

                }

                if (!wasConnected)
                {
                    connected.Add(r);
                }
            }

            if (connected.Count > 1)
            {
                var sideAEnd = connected[0].b;
                var sideBStart = connected[1].a;

                List<Rectangle> groupA;
                List<Rectangle> groupB;

                if (useXAxis)
                {
                    groupA = items.Where(r => r.Left < sideAEnd).ToList();
                    groupB = items.Where(r => r.Left > sideAEnd).ToList();
                }
                else
                {
                    groupA = items.Where(r => r.Top < sideAEnd).ToList();
                    groupB = items.Where(r => r.Top > sideAEnd).ToList();
                }

                if (groupA.Count + groupB.Count != items.Count)
                {
                    throw new Exception("LOGIC ERROR: Not subdividing correctly");
                }

                if (groupA.Count == 0 || groupB.Count == 0)
                {
                    throw new Exception("LOGIC ERROR: Not subdividing correctly");
                }

                var massA = groupA.Sum(r => r.Width * r.Height);
                var massB = groupB.Sum(r => r.Width * r.Height);

                var midSize = (sideBStart - sideAEnd) * (massA / (massA + massB));
                var mid = sideAEnd + midSize;

                Rectangle wholeA;
                Rectangle wholeB;

                if (useXAxis)
                {
                    wholeA = new Rectangle(whole.Left, whole.Top, midSize, whole.Height);
                    wholeB = new Rectangle(mid, whole.Top, whole.Width - midSize, whole.Height);
                }
                else
                {
                    wholeA = new Rectangle(whole.Left, whole.Top, whole.Width, midSize);
                    wholeB = new Rectangle(whole.Left, mid, whole.Width, whole.Height - midSize);
                }

                var sideA = SubdivideIntoGroups(groupA, wholeA);
                var sideB = SubdivideIntoGroups(groupB, wholeB);

                return new RectPartition() { Whole = whole, SideA = sideA, SideB = sideB };
            }
            else
            {
                return null;
            }
        }

        public class RectPartition
        {
            public Rectangle Item { get; set; }
            public Rectangle Whole { get; set; }

            public RectPartition SideA { get; set; }
            public RectPartition SideB { get; set; }
        }
    }
}
