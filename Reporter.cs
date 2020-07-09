using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Class that "reports" errors by writting them out to a text file. 
    /// </summary>
    public class Reporter //need to catch errors
    {
        private static string pathway = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Chess Game\\Error Reports\\";
        private static string filename = "Error";
        private static string filetype = ".txt";

        public static void Report(Exception e)
        {

            DateTime time = DateTime.Now;
            string newFileName = $"{filename} {time.Year}-{time.Month}-{time.Day}-{time.Hour}-{time.Minute}-{time.Second}-{time.Millisecond}{filetype}";
            //string newFileName = filename + filetype;
            CreateFolder(pathway);
            CreateFile(pathway, newFileName);
            string pathFile = Path.Combine(pathway, newFileName);
            using (StreamWriter file = new StreamWriter(pathFile))
            {
                file.WriteLine(time);
                file.Write(Environment.NewLine);
                file.WriteLine(e.Source);
                file.Write(Environment.NewLine);
                file.WriteLine(e.Message);
                file.Write(Environment.NewLine);
                file.WriteLine(e.InnerException);
                file.Write(Environment.NewLine);
                file.WriteLine(e.StackTrace);
                file.Write(Environment.NewLine);
                file.WriteLine(e.TargetSite);
                file.Write(Environment.NewLine);
                file.WriteLine(e);
                file.Write(Environment.NewLine);
                file.Write(Environment.NewLine);
            }
        }

        private static void CreateFolder(string path)
        {
            Debug.WriteLine(path);
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        
        private static void CreateFile(string path, string file)
        {
            string pathFile = Path.Combine(path, file);
            if (!File.Exists(pathFile))
                using (FileStream fs = File.Create(pathFile)) ;

        }
    }
}
