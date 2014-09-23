using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeCast
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void timerScreenshot_Tick(object sender, EventArgs e)
        {
            if (chkEnabled.Checked)
            {
                CaptureScreenshot();
            }
        }

        private void cboTimelapse_Leave(object sender, EventArgs e)
        {
            UpdateTimerInterval();
        }

        private void cboTimelapse_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTimerInterval();
        }

        private void UpdateTimerInterval()
        {
            var value = 0;

            if (int.TryParse(cboTimelapse.Text, out value))
            {
                timerScreenshot.Interval = value;
            }

            cboTimelapse.Text = timerScreenshot.Interval.ToString();
        }

        private List<BitmapDiff> diffs = new List<BitmapDiff>();
        private Bitmap lastBitmap;
        private Bitmap origin;
        private void CaptureScreenshot()
        {
            var bitmap = ScreenCapture.CaptureAllScreens();
            if (chkPreview.Checked)
            {
                picPreview.Image = bitmap;
            }

            // Process differences
            if (lastBitmap != null)
            {
                var diff = BitmapDifferences.GetDifferences(lastBitmap, bitmap);

                // Display differences
                //lstDiffs.Controls.Clear();

                if (chkPreview.Checked)
                {
                    lstDiffs.View = View.LargeIcon;
                    imgList.ImageSize = new Size(32, 32);
                    lstDiffs.LargeImageList = imgList;

                    diffs.Add(diff);

                    throw new NotImplementedException();

                    //imgList.Images.Add(d.Image);

                    //foreach (var d in diff.Parts)
                    //{

                    //    lstDiffs.Items.Add(new ListViewItem() { ImageIndex = imgList.Images.Count - 1 });
                    //}
                }

                RecordFrame(bitmap, diff);

                if (lastBitmap != origin)
                {
                    lastBitmap.Dispose();
                }
            }
            else
            {
                origin = bitmap;
            }

            lastBitmap = bitmap;
        }

        private void lstDiffs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDiffs.SelectedIndices.Count > 0)
            {
                var i = lstDiffs.SelectedIndices[0];

                throw new NotImplementedException();

                //if (i > 0 && parts.Count > i)
                //{
                //    picDiff.Image = parts[i].Image;
                //    var wholeImage = ReconstructFromOrigin(i, false);
                //    picPreview.Image = wholeImage;
                //    picOutput.Image = ScreenCastFrameMaker.CreateFrame(new Size(480, 320), wholeImage, parts[i], "This is a test!");
                //}
            }

        }

        private Bitmap ReconstructFromOrigin(int iPart, bool shouldMarkDiffs = true)
        {
            throw new NotImplementedException();

            //var copy = new Bitmap(origin);

            //var pen = new Pen(Brushes.Red, 5);

            //Graphics g = Graphics.FromImage(copy);
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            //for (int i = 0; i <= iPart; i++)
            //{
            //    var p = parts[i];
            //    g.DrawImage(p.Image, p.Position);

            //    if (shouldMarkDiffs)
            //    {
            //        g.DrawRectangle(pen, new Rectangle(p.Position.X - 1, p.Position.Y - 1, p.Size.Width + 2, p.Size.Height + 2));
            //    }
            //}

            //g.Dispose();

            //return copy;

        }

        private FrameRecorder _recorder;

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dlgSaveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _recorder = new FrameRecorder(10, dlgSaveFile.FileName, 480, 320);
            }
        }

        private void RecordFrame(Bitmap bitmap, BitmapDiff diff)
        {
            if (_recorder != null)
            {
                var frame = ScreenCastFrameMaker.CreateFrame(new Size(480, 320), bitmap, diff, "This is a test!");
                _recorder.SaveFrame(frame);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopRecorder();
        }

        private void StopRecorder()
        {
            if (_recorder != null)
            {
                _recorder.Close();
                _recorder = null;
            }
        }
    }
}
