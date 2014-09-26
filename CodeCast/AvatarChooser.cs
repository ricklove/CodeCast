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
    public partial class AvatarChooser : Form
    {
        public AvatarChooser()
        {
            InitializeComponent();
            LoadAvatarSettings();
        }

        private void LoadAvatarSettings()
        {
            var settings = new Properties.Settings();
            var str = settings.Avatars;
            if (!string.IsNullOrEmpty(str))
            {
                var lines = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var l in lines)
                {
                    var parts = l.Split('\t');
                    var type = (AvatarType)Enum.Parse(typeof(AvatarType), parts[0]);
                    AddAvatar(type, parts[1]);
                }
            }
        }

        private void SaveAvatarSettings()
        {
            var text = new StringBuilder();

            foreach (var a in _avatars)
            {
                foreach (var f in a.Value)
                {
                    text.AppendLine(a.Key.ToString() + "\t" + f);
                }
            }

            var settings = new Properties.Settings();
            settings.Avatars = text.ToString();
            settings.Save();
        }

        private Random _rand = new Random();
        public Image GetAvatar(AvatarType type)
        {
            if (!_avatars.ContainsKey(type))
            {
                if (_avatarImages.Any())
                {
                    return _avatarImages.First().Value;
                }
                else
                {
                    return null;
                }
            }

            var a = _avatars[type];
            var i = _rand.Next(a.Count);
            return GetAvatarImage(a[i]);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var typeFromName = GetAvatarTypeFromName((sender as Control).Name);

            if (dlgOpenAvatar.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddAvatar(typeFromName, dlgOpenAvatar.FileName);
            }
        }


        private void lstSad_Enter(object sender, EventArgs e)
        {
            lst_SelectedIndexChanged(sender, e);
        }

        private void lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lstView = (ListView)sender;
            if (lstView.SelectedItems.Count > 0)
            {
                var filename = lstView.SelectedItems[0].Tag as string;
                picPreview.Image = GetAvatarImage(filename);
                picPreview.Tag = filename;
            }
            else
            {
                picPreview.Image = null;
                picPreview.Tag = null;
            }

        }

        private Dictionary<AvatarType, List<string>> _avatars = new Dictionary<AvatarType, List<string>>();
        private Dictionary<string, Bitmap> _avatarImages = new Dictionary<string, Bitmap>();

        private void AddAvatar(AvatarType type, string filename)
        {
            if (!_avatars.ContainsKey(type))
            {
                _avatars.Add(type, new List<string>());
            }

            var lst = _avatars[type];

            lst.Add(filename);

            ShowAvatars();
            SaveAvatarSettings();
        }

        private void ShowAvatars()
        {
            foreach (var pair in _avatars)
            {
                switch (pair.Key)
                {
                    case AvatarType.Normal:
                        ShowAvatars(pair.Value, lstNormal);
                        break;
                    case AvatarType.Exclaim:
                        ShowAvatars(pair.Value, lstExclaim);
                        break;
                    case AvatarType.Question:
                        ShowAvatars(pair.Value, lstQuestion);
                        break;
                    case AvatarType.Happy:
                        ShowAvatars(pair.Value, lstHappy);
                        break;
                    case AvatarType.Sad:
                        ShowAvatars(pair.Value, lstSad);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ShowAvatars(List<string> filenames, ListView lstView)
        {
            var imageList = (ImageList)lstView.Tag;

            if (imageList == null)
            {
                imageList = new ImageList();
                lstView.Tag = imageList;
                lstView.LargeImageList = imageList;
            }

            foreach (var key in imageList.Images.Keys)
            {
                if (!filenames.Contains(key))
                {
                    var image = imageList.Images[key];
                    imageList.Images.RemoveByKey(key);
                    image.Dispose();
                }
            }

            lstView.Items.Clear();

            foreach (var f in filenames)
            {
                if (!imageList.Images.ContainsKey(f))
                {
                    imageList.Images.Add(f, GetAvatarImage(f));
                }

                lstView.Items.Add(new ListViewItem()
                {
                    ImageIndex = imageList.Images.IndexOfKey(f),
                    Tag = f,
                    //Text = System.IO.Path.GetFileName(f)
                });
            }
        }

        private Bitmap GetAvatarImage(string f)
        {
            if (!_avatarImages.ContainsKey(f))
            {
                _avatarImages.Add(f, (Bitmap)Bitmap.FromFile(f));
            }

            return _avatarImages[f];
        }

        private AvatarType GetAvatarTypeFromName(string name)
        {
            foreach (var n in Enum.GetNames(typeof(AvatarType)))
            {
                if (name.Contains(n))
                {
                    return (AvatarType)Enum.Parse(typeof(AvatarType), n);
                }
            }

            return AvatarType.Normal;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var f = picPreview.Tag as string;

            if (f != null)
            {
                foreach (var a in _avatars)
                {
                    a.Value.Remove(f);
                }

                ShowAvatars();
                var image = _avatarImages[f];
                _avatarImages.Remove(f);

                image.Dispose();
                picPreview.Image = null;
                picPreview.Tag = null;

                SaveAvatarSettings();
            }
        }



    }

    public enum AvatarType
    {
        Normal,
        Exclaim,
        Question,
        Happy,
        Sad
    }
}
