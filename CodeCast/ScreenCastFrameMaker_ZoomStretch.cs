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
            var partitionsGroups = SubdivideIntoGroups(changeRects, new Rectangle(0, 0, wholeScreen.Width, wholeScreen.Height), new Rectangle(new Point(), targetSize));

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
            var frame = new Bitmap(targetSize.Width, targetSize.Height);
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
            var MAXITEMRATIO = 0.8;

            // Draw 9 (3x3) squares with middle stretched as much as possible
            var sPadL = p.Item.Left - p.Whole.Left;
            var sPadT = p.Item.Top - p.Whole.Top;
            var sPadR = -p.Item.Right + p.Whole.Right;
            var sPadB = -p.Item.Bottom + p.Whole.Bottom;

            var sPadWidth = sPadL + sPadR;
            var sPadHeight = sPadT + sPadB;

            // Get destination sizes
            var scale = 2.0;
            var dItemWidth = (int)(p.Item.Width * scale);
            var dItemHeight = (int)(p.Item.Height * scale);

            while (dItemWidth > p.WholeTarget.Width * MAXITEMRATIO
                || dItemHeight > p.WholeTarget.Height * MAXITEMRATIO)
            {
                scale *= 0.5;

                dItemWidth = (int)(p.Item.Width * scale);
                dItemHeight = (int)(p.Item.Height * scale);
            }

            var dPadWidth = p.WholeTarget.Width - dItemWidth;
            var dPadHeight = p.WholeTarget.Height - dItemHeight;

            var dScaleWidth = 1.0 * dPadWidth / sPadWidth;
            var dScaleHeight = 1.0 * dPadHeight / sPadHeight;

            var dPadL = (int)(sPadL * dScaleWidth);
            var dPadT = (int)(sPadT * dScaleHeight);
            var dPadR = dPadWidth - dPadL;
            var dPadB = dPadHeight - dPadT;

            if (dPadL + dPadR + dItemWidth != p.WholeTarget.Width
                || dPadT + dPadB + dItemHeight != p.WholeTarget.Height)
            {
                throw new Exception("LOGIC ERROR");
            }


            // Use the sizes
            var sx = p.Whole.Left;
            var sy = p.Whole.Top;

            var dx = p.WholeTarget.Left;
            var dy = p.WholeTarget.Top;


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
            var dw1 = dItemWidth;
            var dw2 = dPadR;

            var dx0 = dx;
            var dx1 = dx0 + dw0;
            var dx2 = dx1 + dw1;
            var dx3 = dx2 + dw2;

            var dh0 = dPadT;
            var dh1 = dItemHeight;
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
            if (partition.IsLeaf)
            {
                leaves.Add(partition);
            }
            else
            {
                GetPartitionLeaves(partition.SideA, leaves);
                GetPartitionLeaves(partition.SideB, leaves);
            }
        }

        private RectPartition SubdivideIntoGroups(List<Rectangle> items, Rectangle whole, Rectangle wholeTarget)
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
                return new RectPartition(items.First(), whole, wholeTarget);
            }

            var result = SubdivideIntoGroups(items, whole, wholeTarget, false);

            if (result == null)
            {
                result = SubdivideIntoGroups(items, whole, wholeTarget, true);
            }

            if (result == null)
            {
                // The pieces should join as one because they are blocking each other
                return new RectPartition(JoinRects(items), whole, wholeTarget);
            }

            return result;
        }

        private RectPartition SubdivideIntoGroups(List<Rectangle> items, Rectangle whole, Rectangle wholeTarget, bool useXAxis)
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

                //var massA = groupA.Sum(r => r.Width * r.Height);
                //var massB = groupB.Sum(r => r.Width * r.Height);

                //var massARatio = 1.0 * massA / (massA + massB);
                //var massBRatio = 1.0 * massB / (massA + massB);

                var sideAStart = connected[0].a;
                var mid = (int)(sideBStart + sideAEnd * 0.5);
                var midSize = mid - sideAStart;


                // Target size
                var targetStart = useXAxis ? wholeTarget.Left : wholeTarget.Top;
                var targetSize = useXAxis ? wholeTarget.Width : wholeTarget.Height;

                var sizeA = connected[0].b - connected[0].a;
                var sizeB = connected[1].b - connected[1].a;

                var sizeARatio = 1.0 * sizeA / (sizeA + sizeB);
                var sizeBRatio = 1.0 * sizeB / (sizeA + sizeB);

                var tMidSize = (int)(targetSize * sizeARatio);
                var tMid = targetStart + tMidSize;


                Rectangle wholeA;
                Rectangle wholeB;
                Rectangle targetA;
                Rectangle targetB;

                if (useXAxis)
                {
                    wholeA = new Rectangle(whole.Left, whole.Top, midSize, whole.Height);
                    wholeB = new Rectangle(mid, whole.Top, whole.Width - midSize, whole.Height);

                    targetA = new Rectangle(wholeTarget.Left, wholeTarget.Top, tMidSize, wholeTarget.Height);
                    targetB = new Rectangle(tMid, wholeTarget.Top, wholeTarget.Width - tMidSize, wholeTarget.Height);
                }
                else
                {
                    wholeA = new Rectangle(whole.Left, whole.Top, whole.Width, midSize);
                    wholeB = new Rectangle(whole.Left, mid, whole.Width, whole.Height - midSize);

                    targetA = new Rectangle(wholeTarget.Left, wholeTarget.Top, wholeTarget.Width, tMidSize);
                    targetB = new Rectangle(wholeTarget.Left, tMid, wholeTarget.Width, wholeTarget.Height - tMidSize);
                }

                var sideA = SubdivideIntoGroups(groupA, wholeA, targetA);
                var sideB = SubdivideIntoGroups(groupB, wholeB, targetB);

                return new RectPartition(sideA, sideB, whole, wholeTarget);
            }
            else
            {
                return null;
            }
        }

        public class RectPartition
        {
            public Rectangle Item { get; private set; }
            public Rectangle Whole { get; private set; }
            public Rectangle WholeTarget { get; private set; }

            public RectPartition SideA { get; private set; }
            public RectPartition SideB { get; private set; }

            public bool IsLeaf { get { return SideA == null; } }


            public RectPartition(Rectangle item, Rectangle whole, Rectangle wholeTarget)
            {
                SideA = null;
                SideB = null;
                Item = item;

                Whole = whole;
                WholeTarget = wholeTarget;
            }

            public RectPartition(RectPartition sideA, RectPartition sideB, Rectangle whole, Rectangle wholeTarget)
            {
                SideA = sideA;
                SideB = sideB;
                Item = new Rectangle();

                Whole = whole;
                WholeTarget = wholeTarget;
            }

        }
    }
}
