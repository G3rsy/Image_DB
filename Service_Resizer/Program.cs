using System;
using System.IO;
using System.Text;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;

namespace Service_DB
{

    class RequestSolver
    {
        public static string HTTPBodyRequestReader(HttpListenerRequest req){
            String contentType = req.ContentType;
            String toParse = "";

            byte[] buf;
            int readen, count;
            
            using( System.IO.Stream str = req.InputStream )
            {
                readen = 0;
                count = 0;

                do
                {
                    buf = new byte[1024];
                    count = str.Read(buf, 0, 1024);
                    readen += count;

                    for (int counter = 0; counter < count; counter++)
                    {
                        toParse = toParse + (char)buf[counter];            
                    }

                } while(str.CanRead && count > 0);
            }

            return toParse;
        }
        public static void HTTPResponser(String s, HttpListenerResponse resp){
            byte[] data = Encoding.UTF8.GetBytes(String.Format(s));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream, then close it
                resp.OutputStream.Write(data, 0, data.Length);
                resp.Close();
        }

        public static void HTTPRequestHandler(HttpListenerContext ctx)
        {
            // Peel out the requests a response objects
            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse resp = ctx.Response;

            // Extraxtion of the body from the request
            // Only if the body isn't empty and content type is text 
            if(req.HasEntityBody && req.ContentType=="text/html"){
                    
                // Translate the request into a string
                string toParse = HTTPBodyRequestReader(req);
                
                // Request has to be a String like
                // IMAGE NAME, WIDTH, HEIGHT
                string[] strArr = toParse.Split(' ');
                
                // Response settings
                String response;
                if(strArr.Length == 3)
                {
                    string imageName= strArr[0];
                    int width = Convert.ToInt32(strArr[1]);
                    int height = Convert.ToInt32(strArr[2]);

                    string path ="Files/"+imageName;

                    // Use Threading to solve the resize requets
                    bool success = ResizeImage(path, width, height);

                    if(success)
                        response = "Successfully modified";
                    else
                        response = "Something went wrong";
                }else{
                    response = "Wrong input format";
                }
                

                // Write the response info
                HTTPResponser(response, resp);

            }else{
                HTTPResponser("Wrong input format", resp);
            }
        }
        public static bool ResizeImage(String path, int width, int height)
        {
            Thread.Yield();
            bool result = false;

            //If file exist create a copy of the image with the modified dimension
            //Wrap the old image in the new bitmap
            //override the old image with the new one
            if(File.Exists(path)){
                Bitmap image = new Bitmap(path);
                Bitmap destImage = new Bitmap(width, height);
                var destRect = new Rectangle(0, 0, width, height);            

                if(!(image.Width == width && image.Height == height)){
                    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                        {
                            wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                            graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }

                    image.Dispose();
                    File.Delete(path);

                    destImage.Save(path);
                }
                result=true;
            }else{
                result=false;
            }

            Console.WriteLine(result);
            return result;
        }
    }

    class Program
    {
        public static HttpListener listener;
        public static string url = "http://*:8081/";

        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            
            try
            {
                while (true)
                {
                    // Will wait here until we hear from a connection
                    HttpListenerContext ctx = listener.GetContext();

                    // Print out some info about the request
                    Console.WriteLine(ctx.Request.Url.ToString());
                    Console.WriteLine(ctx.Request.HttpMethod);
                    Console.WriteLine(ctx.Request.UserHostName);
                    Console.WriteLine(ctx.Request.UserAgent);
                    Console.WriteLine();    


                    // Client send name, width, height of an existing image 
                    if (ctx.Request.HttpMethod == "POST")
                    {
                        Thread x = new Thread(() => RequestSolver.HTTPRequestHandler(ctx));
                        x.Start();
                    }
                
                }
            }finally{
                // Close the listener
                listener.Close();
            }
        }
    }
}
