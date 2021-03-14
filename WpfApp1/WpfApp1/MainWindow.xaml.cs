using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Intel.RealSense;



namespace WpfApp1
{

    public partial class MainWindow : Window
    {
        private Pipeline pipeline;
        private Colorizer colorizer;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private static string mainPathDcam;
        private static string mainPathDcolor;
        static int tick = 0;

        static Action<VideoFrame> UpdateImage(Image img)
        {

            //SaveImageToJPEG(img, @"C:\Users\Stack\source\repos\WindowsFormsApp4\WindowsFormsApp4\bin\Debug\i.jpg");
            var wbmp = img.Source as WriteableBitmap;
            return new Action<VideoFrame>(
                frame =>
                {
                    using (System.IO.FileStream stream5 = new System.IO.FileStream(mainPathDcolor + "\\" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + " " + tick + ".png", System.IO.FileMode.Create))
                    {
                        PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                        encoder5.Frames.Add(BitmapFrame.Create((img.Source as WriteableBitmap).Clone()));
                        encoder5.Save(stream5);
                    }

                    var rect = new Int32Rect(0, 0, frame.Width, frame.Height);
                    wbmp.WritePixels(rect, frame.Data, frame.Stride * frame.Height, frame.Stride);
                });


        }

        static Action<VideoFrame> UpdateDepthImage(Image img)
        {
            var wbmp = img.Source as WriteableBitmap;
            return new Action<VideoFrame>(
                frame =>
                {
                    var rect = new Int32Rect(0, 0, 640, 480);//220, 140, 200, 200);
                    wbmp.WritePixels(rect, frame.Data, frame.Stride * frame.Height, frame.Stride);
                });
        }

        private static void SaveImageToJPEG(Image ImageToSave, string Location)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)ImageToSave.Source.Width,
                                                                           (int)ImageToSave.Source.Height,
                                                                           1000, 1000, PixelFormats.Default);
            renderTargetBitmap.Render(ImageToSave);
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (System.IO.FileStream fileStream = new System.IO.FileStream(Location, System.IO.FileMode.Create))
            {
                jpegBitmapEncoder.Save(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
        }

        public MainWindow()
        {
            var args = Environment.GetCommandLineArgs();
            var mainPath = @"C:\Users\Stack\Desktop\все\LogFile\dcam " + args[1];
            mainPathDcam = mainPath + "\\depth";
            mainPathDcolor = mainPath + "\\color";

            System.IO.Directory.CreateDirectory(mainPath);
            System.IO.Directory.CreateDirectory(mainPathDcam);
            System.IO.Directory.CreateDirectory(mainPathDcolor);



            InitializeComponent();


            try
            {
                this.MouseDown += MainWindow_MouseDown;
                Action<VideoFrame> updateDepth;
                Action<VideoFrame> updateColor;

                // The colorizer processing block will be used to visualize the depth frames.
                colorizer = new Colorizer();

                // Create and config the pipeline to strem color and depth frames.
                pipeline = new Pipeline();

                var cfg = new Config();
                cfg.EnableStream(Stream.Depth, 640, 480);
                cfg.EnableStream(Stream.Color, 640, 480);

                var selection = pipeline.Start(cfg);
                // coordinateManager.DepthCamMetrix = selection.GetStream<VideoStreamProfile>(Stream.Depth).GetIntrinsics();

                Sensor sensor = selection.Device.Sensors[0];
                float scale = sensor.DepthScale;

                // metrics
                var metrix = selection.GetStream<VideoStreamProfile>(Stream.Depth).GetIntrinsics();

                var converter = new PixelToVecto3Converter(metrix);
                var selected_device = selection.Device;
                var depth_sensor = selected_device.Sensors[0];

                if (depth_sensor.Options.Supports(Option.LaserPower))
                {
                    var laserPower = depth_sensor.Options[Option.LaserPower];
                    laserPower.Value = laserPower.Max; // Set max power
                }


                SetupWindow(selection, out updateDepth, out updateColor);

                //Task.Factory.StartNew(() =>
                //{
                //    using (System.IO.StreamWriter nmeaWriter = new System.IO.StreamWriter(@"C:\Users\TERYA\OneDrive\Рабочий стол\WpfApp1\WpfApp1\nmeaLog.txt", true))
                //    {
                //        SerialPort port = new SerialPort("COM5", 115200);
                //        long tick = 0;

                //        nmeaWriter.WriteLine("Systime: " + DateTime.Now);
                //        port.Open();

                //        while (!tokenSource.Token.IsCancellationRequested)
                //        {
                //            nmeaWriter.WriteLine(port.ReadLine());
                //            tick++;
                //            if (tick % 11 == 0)
                //            {
                //                nmeaWriter.WriteLine("\nSystime: " + DateTime.Now);
                //            }
                //        }
                //    }
                //}, tokenSource.Token);

                Task.Factory.StartNew(() =>
                {
                    while (!tokenSource.Token.IsCancellationRequested)
                    {
                        // We wait for the next available FrameSet and using it as a releaser object that would track
                        // all newly allocated .NET frames, and ensure deterministic finalization
                        // at the end of scope. 
                        using (var frames = pipeline.WaitForFrames())
                        {
                            var colorFrame = frames.ColorFrame.DisposeWith(frames);
                            var depthFrame = frames.DepthFrame.DisposeWith(frames);

                            Frame = depthFrame;
                            // We colorize the depth frame for visualization purposes
                            var colorizedDepth = colorizer.Process<VideoFrame>(depthFrame).DisposeWith(frames);

                            // Render the frames.
                            Dispatcher.Invoke(DispatcherPriority.Render, updateDepth, colorizedDepth);
                            Dispatcher.Invoke(DispatcherPriority.Render, updateColor, colorFrame);

                            Dispatcher.Invoke(new Action(() =>
                            {
                                String depth_dev_sn = depthFrame.Sensor.Info[CameraInfo.SerialNumber];
                                txtTimeStamp.Text = "Frames: " + tick;//+ coord.Process(depthFrame, selectArea); //distanceManager.CalculateArea(depthFrame, selectArea); // + " \n " +
                                //                                    coordinateManager.CalculateArea(depthFrame, selectArea);   // depth_dev_sn + " : " + String.Format("{0,-20:0.00}", depthFrame.Timestamp) + "(" + depthFrame.TimestampDomain.ToString() + ")";
                            }));



                            //Console.WriteLine(i + " sss");
                            //i++;

                            WriteMatrixToStream(depthFrame, converter);

                            tick++;
                        }
                    }
                }, tokenSource.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
        }


        public class PixelToVecto3Converter
        {
            private Intrinsics intrinsics;

            public PixelToVecto3Converter(Intrinsics intrinsics)
            {
                this.intrinsics = intrinsics;
            }

            public float ConverX(int x, float depth)
            {
                return (x - intrinsics.ppx) / intrinsics.fx * depth;
            }

            public float ConverY(int y, float depth)
            {
                return (y - intrinsics.ppy) / intrinsics.fy * depth;
            }


            public Vector3Coordinate ConvertFrom(int x, int y, float depth)
            {
                float newX = ConverX(x, depth);
                float newY = ConverY(y, depth);
                return new Vector3Coordinate(newX, newY, depth);
            }
        }


        public struct Vector3Coordinate 
        {
            public float X;
            public float Y;
            public float Z;

            public Vector3Coordinate(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        private void MainWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //WriteMatrixToStream(Frame);
            Count++;
        }

        static DepthFrame Frame;
        static int Count = 0;

        //private System.IO.StreamWriter writer = new System.IO.StreamWriter(@"C:\Users\Stack\Desktop\все\WpfApp1\WpfApp1\log.txt", true);
        private void WriteMatrixToStream(DepthFrame frame, PixelToVecto3Converter converter)
        {
            try
            {
                var stream = System.IO.File.Create(mainPathDcam + "\\" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + " " + tick);
                using (var writer = new System.IO.StreamWriter(stream))
                {
                    for (int i = 0; i < 640; i++)
                    {
                        for (int j = 0; j < 480; j++)
                        {
                            var point = converter.ConvertFrom(i, j, frame.GetDistance(i, j));

                            //writer.WriteLine(String.Format("{0},{1},{2}", point.X, point.Y, point.Z));
                            writer.Write("{0} ", frame.GetDistance(i, j));
                        }
                        writer.WriteLine();
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception)
            {
                Count--;
            }
        }

        //private System.IO.StreamWriter nmeaWriter = new System.IO.StreamWriter(@"C:\Users\Lev\source\repos\WpfApp1\WpfApp1\nmeaLog.txt", true);



        private void control_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tokenSource.Cancel();
        }

        private void SetupWindow(PipelineProfile pipelineProfile, out Action<VideoFrame> depth, out Action<VideoFrame> color)
        {
            using (var p = pipelineProfile.GetStream(Stream.Depth).As<VideoStreamProfile>())
                imgDepth.Source = new WriteableBitmap(p.Width, p.Height, 96d, 96d, PixelFormats.Rgb24, null);
            depth = UpdateDepthImage(imgDepth);

            using (var p = pipelineProfile.GetStream(Stream.Color).As<VideoStreamProfile>())
                imgColor.Source = new WriteableBitmap(p.Width, p.Height, 96d, 96d, PixelFormats.Rgb24, null);
            color = UpdateImage(imgColor);
        }
    }

}



