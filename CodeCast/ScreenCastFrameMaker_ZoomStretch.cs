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

        private List<Rectangle> _lastChangeRects = new List<Rectangle>();

        public Bitmap CreateFrameZoomStretch(Size targetSize, Bitmap wholeScreen, BitmapDiff diff, string comment, Font font)
        {
            // Divide the screen into splits for each change rect
            var changeRects = diff.Parts.Where(p => p.HighContrastRect.Width > 0).Select(p => p.HighContrastRect).ToList();

            // Use last change rects if empty
            if (!changeRects.Any())
            {
                changeRects = _lastChangeRects;
            }

            if (!changeRects.Any())
            {
                return null;
            }

            _lastChangeRects = changeRects;

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
            var MAXITEMRATIO = 0.75;

            var avWidth = p.WholeTarget.Width * MAXITEMRATIO;
            var avHeight = p.WholeTarget.Height * MAXITEMRATIO;

            // Draw 9 (3x3) squares with middle stretched as much as possible
            var itemRect = p.Item;

            // Get destination sizes
            var scale = 2.0;
            var dItemWidth = (int)(itemRect.Width * scale);
            var dItemHeight = (int)(itemRect.Height * scale);

            while (dItemWidth > avWidth
                || dItemHeight > avHeight)
            {
                scale *= 0.5;

                dItemWidth = (int)(itemRect.Width * scale);
                dItemHeight = (int)(itemRect.Height * scale);
            }

            // Expand item to available space (in source and destination)
            //if (scale >= 1)
            //{
            // TODO: Fix bug in this!
            var nItemWidth = avWidth / scale;
            var nItemHeight = avHeight / scale;

            var exItemWidth = (int)(nItemWidth - itemRect.Width);
            var exItemHeight = (int)(nItemHeight - itemRect.Height);

            var nLeft = (int)Math.Max(0, itemRect.Left - exItemWidth * 0.5f);
            var nTop = (int)Math.Max(0, itemRect.Top - exItemHeight * 0.5f);
            var nRight = (int)Math.Min(p.Whole.Right, itemRect.Right + exItemWidth * 0.5f);
            var nBottom = (int)Math.Min(p.Whole.Bottom, itemRect.Bottom + exItemHeight * 0.5f);

            var nWidth = nRight - nLeft;
            var nHeight = nBottom - nTop;

            itemRect = new Rectangle(nLeft, nTop, nWidth, nHeight);

            dItemWidth = (int)(itemRect.Width * scale);
            dItemHeight = (int)(itemRect.Height * scale);
            //}

            // Get values from partition
            var sWhole = p.Whole;
            var dWhole = p.WholeTarget;

            // Verify some logic
            if (dItemHeight < 0 || dItemWidth < 0)
            {
                throw new ArgumentException("Item size cannot be negative!");
            }

            if (itemRect.Width >= sWhole.Width || itemRect.Height >= sWhole.Height)
            {
                throw new ArgumentException("Item size cannot be as large as source!");
            }

            if (Math.Abs(1.0 * itemRect.Width / dItemWidth) - (1.0 * itemRect.Height / dItemHeight) > 0.0001)
            {
                throw new ArgumentException("dItem ratio is not 1:1");
            }

            DrawPartitionInner(g, image, sWhole, dWhole, itemRect, dItemWidth, dItemHeight);
        }

        private void DrawPartitionInner(Graphics g, Bitmap image, Rectangle sWhole, Rectangle dWhole, Rectangle itemRect, int dItemWidth, int dItemHeight)
        {
            // Calculate the other figures
            var sPadL = itemRect.Left - sWhole.Left;
            var sPadT = itemRect.Top - sWhole.Top;
            var sPadR = -itemRect.Right + sWhole.Right;
            var sPadB = -itemRect.Bottom + sWhole.Bottom;

            var sPadWidth = sPadL + sPadR;
            var sPadHeight = sPadT + sPadB;
            var sItemWidth = itemRect.Width;
            var sItemHeight = itemRect.Height;


            var dPadWidth = dWhole.Width - dItemWidth;
            var dPadHeight = dWhole.Height - dItemHeight;

            var dScaleWidth = 1.0 * dPadWidth / sPadWidth;
            var dScaleHeight = 1.0 * dPadHeight / sPadHeight;

            var dPadL = (int)(sPadL * dScaleWidth);
            var dPadT = (int)(sPadT * dScaleHeight);
            var dPadR = dPadWidth - dPadL;
            var dPadB = dPadHeight - dPadT;

            if (dPadL + dPadR + dItemWidth != dWhole.Width
                || dPadT + dPadB + dItemHeight != dWhole.Height)
            {
                throw new Exception("LOGIC ERROR");
            }


            var sx = sWhole.Left;
            var sy = sWhole.Top;

            var dx = dWhole.Left;
            var dy = dWhole.Top;

            var sw0 = sPadL;
            var sw1 = sItemWidth;
            var sw2 = sPadR;

            var sx0 = sx;
            var sx1 = sx0 + sw0;
            var sx2 = sx1 + sw1;
            var sx3 = sx2 + sw2;

            var sh0 = sPadT;
            var sh1 = sItemHeight;
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


            if (sw0 + sw1 + sw2 != sWhole.Width
                || sh0 + sh1 + sh2 != sWhole.Height
                || dw0 + dw1 + dw2 != dWhole.Width
                || dh0 + dh1 + dh2 != dWhole.Height)
            {
                throw new Exception("LOGIC ERROR");
            }

            // Top Row
            DrawPart(g, image, sw0, sx0, dw0, dx0, sh0, sy0, dh0, dy0);
            DrawPart(g, image, sw1, sx1, dw1, dx1, sh0, sy0, dh0, dy0);
            DrawPart(g, image, sw2, sx2, dw2, dx2, sh0, sy0, dh0, dy0);

            // Middle Row
            DrawPart(g, image, sw0, sx0, dw0, dx0, sh1, sy1, dh1, dy1);
            DrawPart(g, image, sw1, sx1, dw1, dx1, sh1, sy1, dh1, dy1);
            DrawPart(g, image, sw2, sx2, dw2, dx2, sh1, sy1, dh1, dy1);

            // Bottom Row
            DrawPart(g, image, sw0, sx0, dw0, dx0, sh2, sy2, dh2, dy2);
            DrawPart(g, image, sw1, sx1, dw1, dx1, sh2, sy2, dh2, dy2);
            DrawPart(g, image, sw2, sx2, dw2, dx2, sh2, sy2, dh2, dy2);


            // Highlite center
            g.DrawRectangle(diffHighPen, new Rectangle(dx1, dy1, dw1, dh1));
        }

        private static void DrawPart(Graphics g, Bitmap image, int sw, int sx, int dw, int dx, int sh, int sy, int dh, int dy)
        {
            if (sw == 0 && sw != dw)
            {
                throw new ArgumentException("Edges must be 0 together");
            }

            if (sh == 0 && sh != dh)
            {
                throw new ArgumentException("Edges must be 0 together");
            }

            if (sw > 0 && sh > 0)
            {
                g.DrawImage(image,
                    new Rectangle(dx, dy, dw, dh),
                    new Rectangle(sx, sy, sw, sh),
                    GraphicsUnit.Pixel);
            }
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
