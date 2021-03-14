using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private static string GNSS_exePath;
        private static string DCAM_exePath;
        private static string LIDAR_exePath;
        private static string COM;
        private static string LogDirrectoryPath;
        private static List<Process> processes;

        static Program() 
        {
            processes = new List<Process>();
        }


        private static void Main(string[] args)
        {
            try
            {
                LoadEXE();
                LogString("Exe loaded!");

                var name = CreateLogDirrectory();
                LogString("Dirrectory created " + name);

                var gnss = Process.Start(GNSS_exePath, name + " " + COM);
                    gnss.EnableRaisingEvents = true;
                    gnss.Exited += OnProcessClose;
                processes.Add(gnss);
                LogString("GNSS - Process started: " + gnss.Id);

                var dcam = Process.Start(DCAM_exePath, name);
                    dcam.EnableRaisingEvents = true;
                    dcam.Exited += OnProcessClose;
                processes.Add(dcam);
                LogString("DCAM - Process started: " + dcam.Id);


                LogString("Press any button to stop record", ConsoleColor.Yellow);
                Console.ReadKey();

                //ShutDown();
                LogString("Exit");
            }
            catch (Exception exc) 
            {
                LogError("Exit", exc);
                return;
            }
        }

        private static void ShutDown()
        {
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }

        private static void OnProcessClose(object sender, EventArgs e) 
        {
            
            if (sender is Process process) 
            {
                LogError(process.Id + " process closed", new Exception("Shutdown!"));
            }

            foreach (Process proc in processes)
            {
                if (proc.IsRunning() && !proc.HasExited) 
                {
                    proc.Kill();
                }
            }
        }



        private static string TimeString() 
        {
            return DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
        }

        private static string CreateLogDirrectory() 
        {
            try
            {
                var dir = Directory.CreateDirectory(LogDirrectoryPath + "LogFrom_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second);
                return dir.FullName;
            }
            catch (Exception exc) 
            {
                throw new Exception("Can't create dirrectory", exc);
            }
        }

        private static void LoadEXE() 
        {
            try
            {
                using (StreamReader reader = new StreamReader("config.txt"))
                {
                    GNSS_exePath = reader.ReadLine();
                    DCAM_exePath = reader.ReadLine();
                    LIDAR_exePath = reader.ReadLine();
                    LogDirrectoryPath = reader.ReadLine();
                    COM = reader.ReadLine();
                }
            }
            catch (Exception exc)
            {
                throw new Exception("Bad load", exc);
            }
        }


        private static void LogString(string message, ConsoleColor color = ConsoleColor.Green) 
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[LOG] " + message);
            Console.ResetColor();
        }

        private static void LogError(string message, Exception exception) 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[LOG ERROR] " + exception.Message);
            Console.WriteLine("[LOG ERROR] " + message);
            Console.ResetColor();
        }
    }




    public static class ProcessExtensions
    {
        public static bool IsRunning(this Process process)
        {
            try { Process.GetProcessById(process.Id); }
            catch (InvalidOperationException) { return false; }
            catch (ArgumentException) { return false; }
            return true;
        }
    }
}
