﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private DirectoryInfo _backupDir;

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

                    var dir = dlgSaveFile.FileName + "_BACKUP";
                    if (!Directory.Exists(dir))
                    {
                        _backupDir = Directory.CreateDirectory(dir);
                    }
                    else
                    {
                        _backupDir = new DirectoryInfo(dir);

                        while (File.Exists(GetBackupFrameFilename()))
                        {
                            _nextFrameIndex++;
                        }
                    }

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

        private int _nextFrameIndex;

        private string GetBackupFrameFilename()
        {
            return Path.Combine(_backupDir.FullName, "FRAME" + _nextFrameIndex + ".png");
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

                    var scale = 0.7 * frame.Width / txtComment.Width;
                    using (var fontToUse = new Font(txtComment.Font.FontFamily, (float)(txtComment.Font.Size * scale)))
                    {
                        g.DrawString(txtComment.Text, fontToUse, Brushes.White, new Point(150, 50));
                    }

                    var avatar = GetAvatarFromText(txtComment.Text);
                    g.DrawImage(avatar, new Rectangle(10, 50, 100, 100));
                }
            }
            else
            {
                //frame = _maker.CreateFrame(frameSize, bitmap, diff, txtComment.Text, txtComment.Font);
                frame = _maker.CreateFrameZoomStretch(frameSize, bitmap, diff, txtComment.Text, txtComment.Font, GetAvatarFromText(txtComment.Text));

                if (frame == null)
                {
                    frame = _lastFrame;
                }
            }

            if (chkPreview.Checked)
            {
                picOutput.Image = frame;
            }

            if (_recorder != null)
            {
                // Backup image
                frame.Save(GetBackupFrameFilename(), System.Drawing.Imaging.ImageFormat.Png);
                _nextFrameIndex++;

                _recorder.SaveFrame(frame);
            }

            if (_lastFrame != null && _lastFrame != frame)
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
                //_backupDir.Delete(true);
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

        private AvatarChooser _avatarChooser = new AvatarChooser();

        private void btnChooseAvatars_Click(object sender, EventArgs e)
        {
            _avatarChooser.Show();
            _avatarChooser.FormClosing += (object s, FormClosingEventArgs eIner) =>
            {
                _avatarChooser.Hide();
                eIner.Cancel = true;
            };
        }

        private Image GetAvatar(AvatarType type)
        {
            return _avatarChooser.GetAvatar(type);
        }

        private Image GetAvatarFromText(string text)
        {
            if (text.Contains(":-)")
                || text.Contains(":)"))
            {
                return GetAvatar(AvatarType.Happy);
            }
            else if (text.Contains(":-(")
                || text.Contains(":("))
            {
                return GetAvatar(AvatarType.Sad);
            }
            else if (text.Contains("?"))
            {
                return GetAvatar(AvatarType.Question);
            }
            else if (text.Contains("!"))
            {
                return GetAvatar(AvatarType.Exclaim);
            }
            else 
            {
                return GetAvatar(AvatarType.Normal);
            }
        }

    }
}
