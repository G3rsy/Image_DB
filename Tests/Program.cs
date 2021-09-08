using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;

namespace Tests{
    class Program{
        public static HttpClient client = new HttpClient();

        public static string uri = "http://localhost:8080";
        public static string imagePath = @".\\Files\\immagine.jpg";
            
        static string GetImageFileName(string path)
        {
            return new FileInfo(path).Name;
        }
        static byte[] GetImageBinary(string path)
        {
            return File.ReadAllBytes(path);
        }
        public static async void Main(string[] args)
        {
            var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");
            
            var msg = await stringTask;
        }
            
    }
}
