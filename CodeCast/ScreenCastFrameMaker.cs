using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCast
{
    class ScreenCastFrameMaker
    {
        public static Bitmap CreateFrame(Size targetSize, Bitmap wholeScreen, BitmapDiff diff, string comment)
        {
            // Full image (Based on 480x320)
            // 4 Areas:
            // Top Left - Comment (~280x60)
            // Top Right - Full Screen (~200x60) 
            // Middle - Half Size (480x200)
            // Bottom - Actual Size (480x60)

            var wScale = targetSize.Width / 480.0;
            var hScale = targetSize.Height / 320.0;

            var hSmall = (int)Math.Ceiling(60 * hScale);
            var hMiddle = targetSize.Height - hSmall * 2;

            var wWhole = targetSize.Width;
            var mapScale = 1.0 * hSmall / wholeScreen.Height;
            var wMap = (int)Math.Ceiling(mapScale * wholeScreen.Width);
            var wComment = wWhole - wMap;

            var middleScale = 0.5 * hScale;
            var bottomScale = 1.0 * hScale;


            var focus = diff.FocalPoint;

            var frame = new Bitmap(targetSize.Width, targetSize.Height);

            using (var g = Graphics.FromImage(frame))
            using (var diffPen = new Pen(Brushes.Red, 3))
            using (var zoomPen = new Pen(Brushes.Blue, 3))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;

                g.FillRectangle(Brushes.Black, 0, 0, frame.Width, frame.Height);

                var diffRect = new Rectangle(
                    diff.ChangeBounds.Left - 3,
                    diff.ChangeBounds.Top - 3,
                    diff.ChangeBounds.Width + 6,
                    diff.ChangeBounds.Height + 6);

                // Draw map (whole screen)
                var mapTopLeft = new Point(wComment, 0);
                g.DrawImage(wholeScreen, mapTopLeft.X, mapTopLeft.Y, wMap, hSmall);


                // Draw middle
                var middlePartSourceRect = new Rectangle(
                    focus.X - (int)(wWhole / 2.0 / middleScale),
                    focus.Y - (int)(hMiddle / 2.0 / middleScale),
                    (int)(wWhole / middleScale),
                    (int)(hMiddle / middleScale));

                var middleTopLeft = new Point(0, hSmall);
                var middlePartDestRect = new Rectangle(middleTopLeft.X, middleTopLeft.Y, wWhole, hMiddle);

                g.DrawImage(wholeScreen, middlePartDestRect, middlePartSourceRect, GraphicsUnit.Pixel);

                // Draw bottom
                var bottomPartSourceRect = new Rectangle(
                    focus.X - (int)(wWhole / 2.0 / bottomScale),
                    focus.Y - (int)(hSmall / 2.0 / bottomScale),
                    (int)(wWhole / bottomScale),
                    (int)(hSmall / bottomScale));

                var bottomTopLeft = new Point(0, hSmall + hMiddle);
                var bottomPartDestRect = new Rectangle(bottomTopLeft.X, bottomTopLeft.Y, wWhole, hSmall);

                g.DrawImage(wholeScreen, bottomPartDestRect, bottomPartSourceRect, GraphicsUnit.Pixel);



                // Draw diff on map
                g.DrawRectangle(diffPen, ScaleRect(diffRect, mapScale, mapTopLeft));
                // Zoom on map
                g.DrawRectangle(zoomPen, ScaleRect(middlePartSourceRect, mapScale, mapTopLeft));
                g.DrawRectangle(zoomPen, ScaleRect(bottomPartSourceRect, mapScale, mapTopLeft));

                // Diff on middle
                g.DrawRectangle(diffPen, ScaleRect(new Rectangle(
                    diffRect.X - middlePartSourceRect.X,
                    diffRect.Y - middlePartSourceRect.Y,
                    diffRect.Width,
                    diffRect.Height
                    ), middleScale, middleTopLeft));

                // Diff on bottom
                g.DrawRectangle(diffPen, ScaleRect(new Rectangle(
                    diffRect.X - bottomPartSourceRect.X,
                    diffRect.Y - bottomPartSourceRect.Y,
                    diffRect.Width,
                    diffRect.Height
                    ), bottomScale, bottomTopLeft));



                // Draw comment
                //g.DrawString(comment, 
                //g.Draw
            }

            return frame;
        }

        public static Rectangle ScaleRect(Rectangle r, double scale, Point position)
        {
            return new Rectangle(
                (int)(position.X + r.Left * scale),
                (int)(position.Y + r.Top * scale),
                (int)(r.Width * scale),
                (int)(r.Height * scale)
                );
        }
    }
}
