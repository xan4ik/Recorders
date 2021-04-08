using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VisualLogger
{
    public partial class Form1 : Form
    {
        private string GNSS_exePath;
        private string DCAM_exePath;
        private string logDirrectoryPath;
        private List<Process> processes;

        public Form1()
        {
            InitializeComponent();
            LoadEXE();

            processes = new List<Process>();
            comBox.SelectedIndex = 0;
            setBox.SelectedIndex = 0;
            logDirrectoryPath = null;

            FormClosing += Form1_FormClosing; 
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShutDown();
        }

        private void LoadEXE()
        {
            try
            {
                using (StreamReader reader = new StreamReader("config.txt"))
                {
                    GNSS_exePath = reader.ReadLine();
                    DCAM_exePath = reader.ReadLine();
                }
            }
            catch
            {
                MessageBox.Show("Проверте файл конфигурации");
                Application.Exit();
            }
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
            try
            {       
                recordButton.Enabled = false;
                stop.Enabled = true;

                StartRecording();
                LogString("Запись начата", Color.Green);
            }
            catch (Exception exc) 
            {
                recordButton.Enabled = true;
                stop.Enabled = false;

                ShutDown();
                LogError(exc.Message);
            }
        }

        private void StartRecording() 
        {
            processes.Clear();

            var recordPath = CreateLogDirrectory();
            LogString("Папка записи создана " + recordPath, Color.Green);

            var gnss = Process.Start(GNSS_exePath, recordPath + " " + comBox.SelectedItem.ToString());
                gnss.EnableRaisingEvents = true;
                gnss.Exited += OnProcessClose;
            processes.Add(gnss);
            LogString("GNSS - процесс запущен: " + gnss.Id, Color.Green);

            var dcam = Process.Start(DCAM_exePath, recordPath + " " + setBox.SelectedItem.ToString());
                dcam.EnableRaisingEvents = true;
                dcam.Exited += OnProcessClose;
            processes.Add(dcam);
            LogString("DCAM - процесс запущен: " + dcam.Id, Color.Green);
        }

        private string CreateLogDirrectory()
        {
            try
            {
                if (logDirrectoryPath == null)
                {
                    throw new NullReferenceException();
                }

                var dir = Directory.CreateDirectory(logDirrectoryPath + "\\LogFrom_" + TimeString());
                return dir.FullName;
            }
            catch
            {
                throw new Exception("Ошибка создания папки записи");
            }
        }


        private void stop_Click(object sender, EventArgs e)
        {
            recordButton.Enabled = true;
            stop.Enabled = false;

            ShutDown();    
        }

        private void ShutDown()
        {
            foreach (Process process in processes)
            {
                if (process.IsRunning())
                {
                    process.Kill();
                }
            }
            processes.Clear();
            LogString("Запись остановлена", Color.Green);
        }

        private void OnProcessClose(object sender, EventArgs e)
        {
            this.Invoke(
                new Action<String>(LogError),
                new object[] { ("Процесс " + ((Process)sender).Id + " остановлен") }
                );
        }

        private void pathButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK) 
            {
                logDirrectoryPath = dialog.SelectedPath;
                LogString("Путь записи выбран: " + logDirrectoryPath, Color.Green);
            }
        }

        private string TimeString()
        {
            return DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
        }

        private void LogString(string message, Color color)
        {
            textBox1.ForeColor = color;
            textBox1.AppendText("[LOG] " + message + Environment.NewLine);
            textBox1.ForeColor = Color.Black;
        }

        private void LogError(string message)
        {
            textBox1.ForeColor = Color.Red;
            textBox1.AppendText("[LOG ERROR] " + message + Environment.NewLine);
            textBox1.ForeColor = Color.Black;        
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
