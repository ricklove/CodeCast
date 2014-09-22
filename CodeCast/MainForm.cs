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

        private ScreenCapture.BitmapWithRaw lastBitmap;
        private void CaptureScreenshot()
        {
            var bitmap = ScreenCapture.CaptureAllScreens();
            picPreview.Image = bitmap.Bitmap;

            // Process differences
            if (lastBitmap != null)
            {
                var diffs = BitmapDifferences.GetDifferences(lastBitmap, bitmap);

                // Display differences
                foreach (var d in diffs)
                {
                    lstDiffs.Controls.Add(new PictureBox() { Image = d.Image });
                }
            }

            lastBitmap = bitmap;
        }
    }
}
