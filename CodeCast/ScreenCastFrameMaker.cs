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
        private Point _oldFocalPoint = new Point();
        private Rectangle _oldChangeBounds = new Rectangle();

        private Pen diffPen;
        private Pen zoomPen;

        public ScreenCastFrameMaker()
        {
            diffPen = new Pen(new SolidBrush(Color.FromArgb(50, 255, 0, 0)), 3);
            zoomPen = new Pen(new SolidBrush(Color.FromArgb(255, 0, 0, 255)), 3);
        }

        public Bitmap CreateFrame(Size targetSize, Bitmap wholeScreen, BitmapDiff diff, string comment, Font font)
        {
            // Full image (Based on 480x320)
            // 4 Areas:
            // Top Left - Comment (~280x60)
            // Top Right - Full Screen (~200x60) 
            // Middle - Half Size (480x200)
            // Bottom - Actual Size (480x60)

            var wholeScreenRect = new Rectangle(0, 0, wholeScreen.Width, wholeScreen.Height);

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

            // Get focal point
            var changeBounds = diff.ChangeBounds;
            var focalPoint = _oldFocalPoint;

            if (!diff.IsEmpty)
            {
                focalPoint = diff.FocalPoint;

                //if (changeBounds.Contains(_oldFocalPoint))
                //{
                //    focalPoint = _oldFocalPoint;
                //}

                // Just make it top left
                focalPoint = new Point(diff.ChangeBounds.Left + 30, diff.ChangeBounds.Top + 30);

                // Override focal point with keyboard position
                //focalPoint = GlobalInput.GetKeyboardCaretPosition();

                // Round focalPoint
                var ROUND = 16;
                focalPoint = new Point(ROUND * (focalPoint.X / ROUND), ROUND * (focalPoint.Y / ROUND));

            }

            _oldFocalPoint = focalPoint;
            _oldChangeBounds = changeBounds;




            // Create frame
            var frame = new Bitmap(targetSize.Width, targetSize.Height);

            using (var g = Graphics.FromImage(frame))
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
                    focalPoint.X - (int)(wWhole / 2.0 / middleScale),
                    focalPoint.Y - (int)(hMiddle / 2.0 / middleScale),
                    (int)(wWhole / middleScale),
                    (int)(hMiddle / middleScale));

                var middleTopLeft = new Point(0, hSmall);
                var middlePartDestRect = new Rectangle(middleTopLeft.X, middleTopLeft.Y, wWhole, hMiddle);

                middlePartSourceRect = MoveInsideBounds(middlePartSourceRect, wholeScreenRect);

                g.DrawImage(wholeScreen, middlePartDestRect, middlePartSourceRect, GraphicsUnit.Pixel);

                // Draw bottom
                var bottomPartSourceRect = new Rectangle(
                    focalPoint.X - (int)(wWhole / 2.0 / bottomScale),
                    focalPoint.Y - (int)(hSmall / 2.0 / bottomScale),
                    (int)(wWhole / bottomScale),
                    (int)(hSmall / bottomScale));

                var bottomTopLeft = new Point(0, hSmall + hMiddle);
                var bottomPartDestRect = new Rectangle(bottomTopLeft.X, bottomTopLeft.Y, wWhole, hSmall);

                bottomPartSourceRect = MoveInsideBounds(bottomPartSourceRect, wholeScreenRect);

                g.DrawImage(wholeScreen, bottomPartDestRect, bottomPartSourceRect, GraphicsUnit.Pixel);



                // Draw diff on map
                g.DrawRectangle(diffPen, ScaleRect(diffRect, mapScale, mapTopLeft));
                // Zoom on map
                g.DrawRectangle(zoomPen, ScaleRect(middlePartSourceRect, mapScale, mapTopLeft));
                g.DrawRectangle(zoomPen, ScaleRect(bottomPartSourceRect, mapScale, mapTopLeft));

                // Diff on middle
                g.DrawRectangle(diffPen, ClipToBounds(
                    ScaleRect(new Rectangle(
                            diffRect.X - middlePartSourceRect.X,
                            diffRect.Y - middlePartSourceRect.Y,
                            diffRect.Width,
                            diffRect.Height
                        ), middleScale, middleTopLeft),
                    middlePartDestRect));

                // Diff on bottom
                g.DrawRectangle(diffPen, ClipToBounds(
                    ScaleRect(new Rectangle(
                            diffRect.X - bottomPartSourceRect.X,
                            diffRect.Y - bottomPartSourceRect.Y,
                            diffRect.Width,
                            diffRect.Height
                        ), bottomScale, bottomTopLeft),
                    bottomPartDestRect));



                // Draw comment
                using (var fontToUse = new Font(font.FontFamily, (float)(font.Size * hScale)))
                {
                    g.DrawString(comment, fontToUse, Brushes.White, 0, 0);
                }
            }

            return frame;
        }

        private Rectangle MoveInsideBounds(Rectangle r, Rectangle bounds)
        {
            var left = Math.Max(r.Left, bounds.Left);
            var top = Math.Max(r.Top, bounds.Top);

            if (left > bounds.Right - r.Width)
            {
                left = bounds.Right - r.Width;
            }

            if (top > bounds.Bottom - r.Height)
            {
                top = bounds.Bottom - r.Height;
            }


            return new Rectangle(
                left,
                top,
                r.Width,
                r.Height
                );
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

        public static Rectangle ClipToBounds(Rectangle r, Rectangle bounds)
        {
            var left = Math.Max(r.Left, bounds.Left);
            var right = Math.Min(r.Right, bounds.Right);
            var top = Math.Max(r.Top, bounds.Top);
            var bottom = Math.Min(r.Bottom, bounds.Bottom);

            return new Rectangle(
                left,
                top,
                right - left,
                bottom - top
                );
        }
    }
}
