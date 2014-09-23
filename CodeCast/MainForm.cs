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
                var activeWindowRect = ScreenCapture.GetActiveWindowRectangle();

                var borderSize = 40;
                var ignoreBorderRect = new Rectangle(
                    activeWindowRect.Left + borderSize,
                    activeWindowRect.Top + borderSize,
                    activeWindowRect.Width - borderSize * 2,
                    activeWindowRect.Height - borderSize * 2);

                var diff = BitmapDifferences.GetDifferences(lastBitmap, bitmap, ignoreBorderRect);

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

        private FrameRecorder _recorder;

        //private Size frameSize = new Size(480, 320);
        private Size frameSize = new Size(960, 640);

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Text != "Stop")
            {
                if (dlgSaveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var fps = 10;
                    _recorder = new FrameRecorder(fps, dlgSaveFile.FileName, frameSize.Width, frameSize.Height);
                    btnSave.Text = "Stop";
                    chkEnabled.Checked = true;
                }
            }
            else
            {
                btnSave.Text = "Record";
                StopRecorder();
            }
        }

        private ScreenCastFrameMaker _maker = new ScreenCastFrameMaker();

        private Bitmap _lastFrame;

        private void RecordFrame(Bitmap bitmap, BitmapDiff diff)
        {
            // If entering comment
            Bitmap frame;
            if (DateTime.Now < _timeAtTextChange + TimeSpan.FromSeconds(3))
            {
                frame = new Bitmap(frameSize.Width, frameSize.Height);
                using (var g = Graphics.FromImage(frame))
                {
                    g.FillRectangle(Brushes.Black, 0, 0, frame.Width, frame.Height);

                    var scale = 0.9 * frame.Width / txtComment.Width;
                    using (var fontToUse = new Font(txtComment.Font.FontFamily, (float)(txtComment.Font.Size * scale)))
                    {
                        g.DrawString(txtComment.Text, fontToUse, Brushes.White, new Point(100, 100));
                    }
                }
            }
            else
            {
                frame = _maker.CreateFrame(frameSize, bitmap, diff, txtComment.Text, txtComment.Font);
            }

            if (chkPreview.Checked)
            {
                picOutput.Image = frame;
            }

            if (_recorder != null)
            {
                _recorder.SaveFrame(frame);
            }

            if (_lastFrame != null)
            {
                _lastFrame.Dispose();
            }

            _lastFrame = frame;
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


        private DateTime _timeAtTextChange = DateTime.MinValue;
        private void txtComment_TextChanged(object sender, EventArgs e)
        {
            _timeAtTextChange = DateTime.Now;
        }

        private void chkPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkPreview.Checked)
            {
                picOutput.Image = null;
                picPreview.Image = null;

            }
        }
    }
}
