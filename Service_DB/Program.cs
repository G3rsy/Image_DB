using System;
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Drawing;

namespace Service_DB{
    class Program{
        public static HttpListener listener;
        public static string url = "http://*:8080/";

        public static string path = "Files/";

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

        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            try
            {
                listener.Start();
                Console.WriteLine("Listening for connections on {0}", url);

                String response = null;

                while (true)
                {

                    // Will wait here until we hear from a connection
                    HttpListenerContext ctx = listener.GetContext();

                    // Peel out the requests and response objects
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse resp = ctx.Response;

                    // Print out some info about the request
                    Console.WriteLine(req.Url.ToString());
                    Console.WriteLine(req.HttpMethod);
                    Console.WriteLine(req.UserHostName);
                    Console.WriteLine(req.UserAgent);
                    Console.WriteLine();


                    // Client send an image -- so that the program store the image and refresh list of images
                    if (req.HttpMethod == "POST")
                    {
                        Console.WriteLine("POST");

                        if(req.HasEntityBody && req.ContentType=="image/jpg")
                        {
                            Bitmap image = new Bitmap(req.InputStream);

                            //Name generator that avoid conflicts
                            string name=(DateTime.Now.ToString("yyyyMMddHHmmssffff"))+".jpg";

                            image.Save(path+name);
                            image.Dispose();    
                            
                            //Client will receive the new image name
                            response = "Posted image "+name;
                        }else{
                            responde = "Wrong input format";
                        }
                    }
                    //Client will receive the list of images in the server
                    else if (req.HttpMethod == "GET")
                    {
                        Console.WriteLine("GET");
                        //Construction of the string with the list of images
                        string[] constr = Directory.GetFiles(path);
                        string outStr = "";

                        //Verify if some images are present in the server
                        if(constr.Length != 0){

                            for (int i=0; i<constr.Length; i++){
                                string name;
                                int widht;
                                int height; 

                                Bitmap imag = new Bitmap(constr[i]);
                                widht = imag.Width;
                                height = imag.Height;
                                imag.Dispose();

                                name= new FileInfo(constr[i]).Name;

                                outStr = outStr + System.Environment.NewLine + "Name: "+name+ " " +widht +"x"+ height;
                            }
                        }else{
                            outStr = "No Images found";
                        }
                        response = outStr;
                    }
                    // Server will delete the image with the name in the path
                    else if (req.HttpMethod == "DELETE")
                    {
                        Console.WriteLine("DELETE");
                        string imageName = "";
                        

                        if(req.HasEntityBody && req.ContentType=="text/html")
                        {
                            //Translate input to string
                            imageName = HTTPBodyRequestReader(req);
                            String toDelete = path+imageName;
                            
                            if(File.Exists(toDelete)){
                                File.Delete(toDelete);
                                response = "Succesfully deleted";
                            }else{
                                response = "Image don't found";
                            }
                            
                        }else{
                            response = "Wrong input format";
                        }
                    }

                    // Sending response to client
                    HTTPResponser(response, resp); 
                }
            }finally{
                listener.Close();
            }
        }
    }
}
