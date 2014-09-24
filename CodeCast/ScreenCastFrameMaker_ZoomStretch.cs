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
            var shouldDrawPartitions = true;
            if (shouldDrawPartitions)
            {
                var frame = new Bitmap(wholeScreen);
                using (var g = Graphics.FromImage(frame))
                {
                    foreach (var p in partitions)
                    {
                        g.DrawRectangle(diffPen, p.Whole);
                        g.DrawRectangle(diffHighPen, p.Item);
                    }
                }

                return frame;
            }

            // TODO: Expand inside each partition (undistort items back to original aspect ratio)

            //throw new NotImplementedException();

            return wholeScreen;
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
                        var l = Math.Min(a.Left, b.Left);
                        var t = Math.Min(a.Top, b.Top);
                        var r = Math.Max(a.Right, b.Right);
                        var bot = Math.Max(a.Bottom, b.Bottom);
                        changeRects[i] = new Rectangle(l, t, r - l, bot - t);
                        changeRects.RemoveAt(j);
                        j--;
                    }
                }
            }

            return changeRects;
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
                throw new Exception("LOGIC ERROR!");
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
