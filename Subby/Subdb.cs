using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Subby
{
    class Subdb : SubDownloader
    {
        private WebClient wClient;

        public Subdb(string filename) : base(filename)
        {
            this.wClient = new WebClient();
            this.wClient.Headers.Add("user-agent", "SubDB/1.0 (Figa/0.1; https://github.com/figa12/Subby)");
        }

        public override bool Run()
        {
            try
            {
                this.wClient.DownloadFile(new Uri("http://api.thesubdb.com/?action=download&hash=" + this.GetHash(Filename) + "&language=en"), Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename)) + ".srt");
                return true;
            }
            catch (Exception)
            {
                return false;
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
    }
}