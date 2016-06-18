using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using CookComputing.XmlRpc;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;

namespace Subby
{
    public class Opensub : SubDownloader
    {
        private string _token;
        private string _filesize;
        private const string UserAgent = "OSTestUserAgent";

        public Opensub(string filename) : base(filename)
        {
            Filename = filename;
            var f = new FileInfo(filename);
            _filesize = f.Length.ToString();
        }

        public override bool Run()
        {
            try
            {
                IOpenSubtitles proxy = XmlRpcProxyGen.Create<IOpenSubtitles>();
                proxy.UserAgent = UserAgent;

                XmlRpcStruct ret = proxy.GetToken("", "", "eng", proxy.UserAgent);
                _token = (string)ret["token"];

                var request = new SearchSubtitlesRequest
                {
                    sublanguageid = "eng",
                    moviehash = ToHexadecimal(ComputeMovieHash(Filename)),
                    moviebytesize = _filesize,
                    imdbid = string.Empty,
                    query = string.Empty
                };

                XmlRpcStruct response = proxy.SearchSubtitles(_token, new[] { request });

                if (response["data"] == null || ((object[])response["data"])[0] == null) return false;

                var results = (object[])response["data"];


                XmlRpcStruct firstSubtitle = (XmlRpcStruct)results[0];
                string subtitleDownloadLink = firstSubtitle["SubDownloadLink"].ToString();

                string destinationfile = Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename)) + ".srt";
                string tempZipName = Path.GetTempFileName();

                try
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(subtitleDownloadLink, tempZipName);

                    UnZipSubtitleFileToFile(tempZipName, destinationfile);

                }

                catch (Exception e)
                {
                    return false;
                }

                finally
                {
                    File.Delete(tempZipName);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        protected static void UnZipSubtitleFileToFile(string zipFileName, string subFileName)
        {
            // Use a 4K buffer. Any larger is a waste.    
            byte[] dataBuffer = new byte[4096];

            using (System.IO.Stream fs = new FileStream(zipFileName, FileMode.Open, FileAccess.Read))
            {
                using (GZipInputStream gzipStream = new GZipInputStream(fs))
                {

                    using (FileStream fsOut = File.Create(subFileName))
                    {
                        StreamUtils.Copy(gzipStream, fsOut, dataBuffer);
                    }
                }
            }
        }

        private byte[] ComputeMovieHash(string filename)
        {
            byte[] result;
            using (Stream input = File.OpenRead(filename))
            {
                result = ComputeMovieHash(input);
            }
            return result;
        }

        private static byte[] ComputeMovieHash(Stream input)
        {
            long lhash, streamsize;
            streamsize = input.Length;
            lhash = streamsize;

            long i = 0;
            byte[] buffer = new byte[sizeof(long)];
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }

            input.Position = Math.Max(0, streamsize - 65536);
            i = 0;
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }
            input.Close();
            byte[] result = BitConverter.GetBytes(lhash);
            Array.Reverse(result);
            return result;
        }

        private string ToHexadecimal(byte[] bytes)
        {
            StringBuilder hexBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hexBuilder.Append(bytes[i].ToString("x2"));
            }
            return hexBuilder.ToString();
        }
    }



    [XmlRpcUrl("http://api.opensubtitles.org/xml-rpc")]
    public interface IOpenSubtitles : IXmlRpcProxy
    {
        [XmlRpcMethod("ServerInfo")]
        XmlRpcStruct GetServerInfo();

        [XmlRpcMethod("LogIn")]
        XmlRpcStruct GetToken(string username, string password, string language, string useragent);

        [XmlRpcMethod("SearchSubtitles")]
        XmlRpcStruct SearchSubtitles(string token, SearchSubtitlesRequest[] request);

        [XmlRpcMethod("DownloadSubtitles")]
        XmlRpcStruct DownloadSubtitle(string token, int[] request);
    }
}