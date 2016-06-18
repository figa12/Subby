using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Subby
{
    public partial class Form1 : Form
    {
        private const string _versionUrl = "http://figz.dk/subbyversion";
        private string path;
        private string _filename;
        private string[] args;
        private bool _allowshowdisplay = false;
        private bool _updateAvailable = false;

        public Form1(string[] args)
        {
            
            this.args = args;
            InitializeComponent();
            Init();
            //this.CheckUpdate(url);
            _filename = Path.GetFullPath(this.args[0]);

            DownloadSubtitle();

            ExitApplication();
        }

        private void Init()
        {
            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            #if DEBUG
            args = new string[] { @"E:\Serier\The.Fosters.2013.S03E12.HDTV.x264-KILLERS[ettv]\The.Fosters.2013.S03E12.HDTV.x264-KILLERS[ettv].mp4" };
            path = @"C:\Projects\Subby";
            #endif

            notifyIcon.Icon = new System.Drawing.Icon(path + "\\subby.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "Subby";

            
        }

        private List<SubDownloader> GetSubdownloaders()
        {
            return new List<SubDownloader>()
            {
                new Subdb(_filename),
                new Opensub(_filename)
            };

        } 

        private void DownloadSubtitle()
        {
            notifyIcon.ShowBalloonTip(1000, "Subby", "Downloading subtitle...", ToolTipIcon.Info);

            var subDownloaders = GetSubdownloaders();

            if (subDownloaders.Any(subDownloader => subDownloader.Run()))
            {
                return;
            }

            notifyIcon.ShowBalloonTip(2500, "Subby", "No subtitle exists for this file.", ToolTipIcon.Info);
            System.Threading.Thread.Sleep(2500);
        }

        public void CheckUpdate()
        {
            string version = StripNewLine((new WebClient()).DownloadString(_versionUrl));
            string currentVersion = Application.ProductVersion.Split('.')[0];

            if (int.Parse(currentVersion) < int.Parse(version))
            {
                _allowshowdisplay = true;
                Visible = !Visible;
                _updateAvailable = true;
            }
        }

        public static string StripNewLine(string htmlString)
        {
            string pattern = @"\n";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(_allowshowdisplay ? value : _allowshowdisplay);
        }

        private void ExitApplication()
        {
            notifyIcon.Dispose();
            Close();
            Application.Exit();
            Environment.Exit(1);
        }
    }
}
