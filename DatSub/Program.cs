using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace DatSub
{
    class Program
    {
        

        static void Main(string[] args)
        {

            string filename = Path.GetFullPath(args[0]);
            

            WebClient wClient = new WebClient();
            wClient.Headers.Add("user-agent", "SubDB/1.0 (Figa/0.1; http://github.com/figa12/DatSub)");

            try
            {
                Console.WriteLine("Downloading subtitle...");
                wClient.DownloadFile("http://api.thesubdb.com/?action=download&hash=" + getHash(filename) + "&language=en", Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename)) + ".srt");
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine("No subtitle exists for this file.");
                System.Threading.Thread.Sleep(2500);
            }
        }

        public static string getHash(string name)
        {
            string md5Final;
            int readsize = 64*1024;
            long read = 0;
            byte[] buffer = new byte[readsize*2];
            MD5 md5 = new MD5CryptoServiceProvider();

            using (var stream = new FileStream(name, FileMode.Open, FileAccess.Read))
            {
                read = stream.Read(buffer, 0, readsize);
                stream.Seek(-readsize, SeekOrigin.End);
                read += stream.Read(buffer, readsize, readsize);

                md5.TransformFinalBlock(buffer, 0, buffer.Length);
                md5Final = String.Join("", md5.Hash.Select(x => x.ToString("x2")));
            }
            
            return md5Final;
        }

    }
}
