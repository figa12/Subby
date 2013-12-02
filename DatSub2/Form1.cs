using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Subby
{
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;

    public partial class Form1 : Form
    {

        private bool allowshowdisplay = false;

        private bool updateAvailable = false;

        public Form1(string[] args)
        {
            string url = "http://figz.dk/subbyversion";
            this.CheckUpdate(url);

            InitializeComponent();
            #if DEBUG
            args = new string[]{@"C:\dexter.mp4"};
            #endif

            string filename = Path.GetFullPath(args[0]);

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            notifyIcon1.Icon = new System.Drawing.Icon(path + "\\subby.ico");
            notifyIcon1.Visible = true;
            notifyIcon1.Text = "Subby";

            WebClient wClient = new WebClient();
            wClient.Headers.Add("user-agent", "SubDB/1.0 (Figa/0.1; http://github.com/figa12/DatSub)");

            try
            {
                notifyIcon1.ShowBalloonTip(1000, "Subby", "Downloading subtitle...", ToolTipIcon.Info);
                wClient.DownloadFile(new Uri("http://api.thesubdb.com/?action=download&hash=" + this.GetHash(filename) + "&language=en"), Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename)) + ".srt");
                if (this.updateAvailable == false)
                {
                    this.Close();
                    Environment.Exit(1);
                }
            }
            catch (Exception e)
            {
                notifyIcon1.ShowBalloonTip(2500, "Subby", "No subtitle exists for this file.", ToolTipIcon.Info);
                System.Threading.Thread.Sleep(2500);
                if (this.updateAvailable == false)
                {
                    this.Close();
                    Environment.Exit(1);
                }
            }

        }

        private string GetHash(string name)
        {
            string md5Final;
            int readsize = 64 * 1024;
            long read = 0;
            byte[] buffer = new byte[readsize * 2];
            MD5 md5 = new MD5CryptoServiceProvider();

            using (var stream = new FileStream(name, FileMode.Open, FileAccess.Read))
            {
                read = stream.Read(buffer, 0, readsize);
                stream.Seek(-readsize, SeekOrigin.End);
                read += stream.Read(buffer, readsize, readsize);

                md5.TransformFinalBlock(buffer, 0, buffer.Length);
                md5Final = string.Join(string.Empty, md5.Hash.Select(x => x.ToString("x2")));
            }

            return md5Final;
        }

        public void CheckUpdate(string url)
        {
            string version = StripNewLine((new WebClient()).DownloadString(url));
            string currentVersion = Application.ProductVersion.Split('.')[0];

            if (int.Parse(currentVersion) < int.Parse(version))
            {
                this.allowshowdisplay = true;
                this.Visible = !this.Visible;
                this.updateAvailable = true;
            }
        }

        public static string StripNewLine(string htmlString)
        {
            string pattern = @"\n";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("http://figa12.github.io/Subby/");
            this.Close();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(allowshowdisplay ? value : allowshowdisplay);
        }
    }
}
