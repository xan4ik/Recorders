using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace GNSSLOger
{
    class Program
    {
        private const string FolderName = "\\NMEA";

        static void Main(string[] args)
        {
            string folderPath = args[0] + FolderName;
            SerialPort port;
            
            try
            {
                Directory.CreateDirectory(folderPath);
                port = new SerialPort(args[1], 115200);
                port.Open();
            }
            catch (Exception exc) 
            {
                LogError("Start Error", exc);
                return;
            }

            LogString("Port opened");
            LogString("Start recording");
            
            string line = port.ReadLine();
            while (true)
            {
                var stream = File.Create(folderPath + "\\" + DateTime.Now.Ticks);
                using (StreamWriter nmeaWriter = new StreamWriter(stream))
                {
                    nmeaWriter.WriteLine(line);
                    while (true)
                    {
                        line = port.ReadLine();
                        if (line.StartsWith("$GPRMC"))
                        {
                            break;
                        }
                        else nmeaWriter.WriteLine(line);
                    }
                }
                LogString("File has been writen");
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
}
