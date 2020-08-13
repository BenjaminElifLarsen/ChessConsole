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
        private static readonly string pathway = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Chess Game\\Error Reports\\";
        private static readonly string filename = "Error";
        private static readonly string filetype = ".txt";

        /// <summary>
        /// Writes out <paramref name="e"/> to a text file. 
        /// </summary>
        /// <param name="e"></param>
        public static void Report(Exception e)
        {

            DateTime time = DateTime.Now;
            //string newFileName = $"{filename} {time.Year}-{time.Month}-{time.Day}-{time.Hour}-{time.Minute}-{time.Second}-{time.Millisecond}{filetype}";
            string newFileName = filename + filetype;
            string timePoint = $"{filename} {time.Year}-{time.Month}-{time.Day}-{time.Hour}-{time.Minute}-{time.Second}-{time.Millisecond}";
            CreateFolder(pathway);
            CreateFile(pathway, newFileName);
            string pathFile = Path.Combine(pathway, newFileName);
            using (StreamWriter file = new StreamWriter(pathFile))
            {
                file.WriteLine(timePoint);
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

        /// <summary>
        /// If the error folder <paramref name="path"/> does not exist create it.
        /// </summary>
        /// <param name="path">Path and name of the error folder.</param>
        private static void CreateFolder(string path)
        {
            Debug.WriteLine(path);
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        
        /// <summary>
        /// If the error filer <paramref name="file"/> does not exist create it at <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the error file.</param>
        /// <param name="file">Name and filetype of the error file.</param>
        private static void CreateFile(string path, string file)
        {
            string pathFile = Path.Combine(path, file);
            if (!File.Exists(pathFile))
                using (FileStream fs = File.Create(pathFile)) ;
        }
    }
}
