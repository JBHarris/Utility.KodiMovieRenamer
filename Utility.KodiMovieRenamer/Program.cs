using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace Utility.KodiMovieRenamer
{
    static class Program
    {
        static string movieFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        [STAThread]
        static int Main(string[] args)
        {
            Uri serviceCall = new Uri(Uri.UnescapeDataString(string.Join(@"", args)));

            WebClient client = new WebClient();
            string json = client.DownloadString(serviceCall);

            Model.VideoLibrary.GetMoviesResponse resp = JsonConvert.DeserializeObject<Model.VideoLibrary.GetMoviesResponse>(json);
            Console.WriteLine("#!/bin/bash ");
            int x = 1;
            foreach (Model.VideoLibrary.GetMoviesResponse.Movie movie in resp.result.movies)
            {
                string filePath = movie.file.Replace("/mnt/media/", "file://tmp/mnt/sda1/");
                string ext = System.IO.Path.GetExtension(filePath);
                string fileName = System.IO.Path.GetFileName(filePath);


                string newFilePath = @"/tmp/mnt/sdb1/Movies/" + string.Format("{0} ({1})/", movie.label, movie.year);
                //string newFilePath = filePath.Replace("/tmp/mnt/sda1", "/tmp/mnt/sdb1");
                string newFileName = string.Format("{0} ({1}){2}", movie.label, movie.year, ext);

                string newFileFullPath = newFilePath + newFileName;

                string encFilePath = System.Uri.EscapeUriString(filePath);

                //Console.Write(string.Format("{0:P}  {1}", x / resp.result.movies.Count, newFilePath + newFileName));

                Console.WriteLine(@"echo ""{0:P}         {1} """, (float)x / (float)resp.result.movies.Count, newFilePath + newFileName);
                string cmdLine = string.Format(@"curl -o ""{0}"" ""{1}"" --create-dirs ", newFileFullPath, encFilePath);
                Console.WriteLine(cmdLine);

                //if (!System.IO.File.Exists(newFilePath))
                //{
                //    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(newFilePath));

                //    copy(filePath, newFilePath);

                //    Console.WriteLine("...done");
                //}
                //else { Console.WriteLine("already there - Skipping."); }
                x++;
            }

            return 0;
        }

        static void copy(string filePath, string newFilePath)
        {
            long start = DateTime.Now.Ticks;

            System.IO.FileStream inStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
            System.IO.FileStream outStream = new System.IO.FileStream(newFilePath, System.IO.FileMode.Create);

            long fileSize = inStream.Length;

            int bufSize = (int)(1024 * 1024 * .5);
            byte[] b = new byte[bufSize];
            int readBytes = inStream.Read(b, 0, bufSize);
            string strPercent = String.Format("({0:P2}) ", (double)outStream.Length / (double)fileSize).PadLeft(15, '.');
            Console.Write(strPercent);
            Console.SetCursorPosition(Console.CursorLeft - strPercent.ToCharArray().Length, Console.CursorTop);
            while (readBytes == bufSize)
            {
                strPercent = String.Format("({0:P2}) ", (double)outStream.Length / (double)fileSize).PadLeft(15, '.');
                Console.Write(strPercent);
                outStream.Write(b, 0, readBytes);
                readBytes = inStream.Read(b, 0, bufSize);

                Console.SetCursorPosition(Console.CursorLeft - strPercent.ToCharArray().Length, Console.CursorTop);
            }
            outStream.Write(b, 0, readBytes);
            strPercent = String.Format("({0:P2}) ", (double)outStream.Length / (double)fileSize).PadLeft(15, '.');
            Console.Write(strPercent);

            inStream.Close();
            outStream.Close();
            inStream.Dispose();
            outStream.Dispose();

            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - start);
            string speedStatement = string.Format("@ {0:F2} MB/s", (fileSize / (1024 * 1024) / ts.TotalSeconds));
            Console.Write(speedStatement);
        }

    }
}
