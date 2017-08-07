using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace KaamShaam.AppUtils
{
    public static class Common
    {
        public static string ResizeImage(int maxWidth, int maxHeight, string path, string newParm)
        {
            string fileRelativePath;
            try
            {
                var image = System.Drawing.Image.FromFile(path);              
                var newImage = new Bitmap(maxWidth, maxHeight);
                Graphics thumbGraph = Graphics.FromImage(newImage);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;

                thumbGraph.DrawImage(image, 0, 0, maxWidth, maxHeight);
                image.Dispose();
                newImage.Save(newParm, newImage.RawFormat);
            }
            catch (Exception excep)
            {
                Console.WriteLine("Exception occurred");
                Console.WriteLine(excep.Message);
                fileRelativePath = null;
            }
            return path;
        }

        public static void GenerateImages(string uid, string baseUrl)
        {
            var imgUrl = baseUrl + uid ;
            AppUtils.Common.ResizeImage(110, 110, imgUrl+".png", imgUrl + "_110.png");
            AppUtils.Common.ResizeImage(35, 35, imgUrl + ".png", imgUrl + "_35.png");
            File.Delete(imgUrl + ".png");
        }

        public static string ReturnImage(string url, string size)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "HEAD";
            try
            {
                request.GetResponse();
                return url;
            }
            catch
            {
                return "http://via.placeholder.com/"+size;
            }
        }
    }
}