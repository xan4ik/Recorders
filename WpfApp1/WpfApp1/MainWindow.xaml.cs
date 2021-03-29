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
        private const string FolderName = "\\DCAM";
        private static int tick = 0;

        static Action<VideoFrame> UpdateImage(Image img)
        {
            var wbmp = img.Source as WriteableBitmap;
            return new Action<VideoFrame>(
                frame =>
                {
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

      
        public MainWindow()
        {
            var args = Environment.GetCommandLineArgs();           
            var dir = System.IO.Directory.CreateDirectory(args[1] + FolderName);

            InitializeComponent();

            try
            {
                Action<VideoFrame> updateDepth;
                Action<VideoFrame> updateColor;

                // The colorizer processing block will be used to visualize the depth frames.
                colorizer = new Colorizer();

                this.Closing += control_Closing;


                #region PlayBack

                //var ctx = new Context();
                //var playback = ctx.AddDevice(@"C:\Users\Stack\Desktop\log\LogTestFolder\LogFrom_21_6_42\DCAM\637523032062841397.bag");
                //var depth = playback.Sensors[0];
                //var color = playback.Sensors[1];
                //var syncer = new Syncer();


                //depth.Open(depth.StreamProfiles[0]);
                //color.Open(color.StreamProfiles[0]);

                //depth.Start(syncer.SubmitFrame);
                //color.Start(syncer.SubmitFrame);

                //Console.WriteLine(playback.FileName);
                //playback.Realtime = true;

                //SetupWindow(out updateDepth, out updateColor);

                //var start = DateTime.Now;
                ////void Print() => Console.WriteLine($"Real: {DateTime.Now - start} Status: {playback.Status,-7} Playback: {playback.Position}/{playback.Duration}");

                //int framesCount = 0;

                //Task.Factory.StartNew(() =>
                //{
                //    while (!tokenSource.Token.IsCancellationRequested)
                //    {
                //        using (var new_frames = syncer.WaitForFrames())
                //        {
                //            if (new_frames.Count == 2)
                //            {
                //                var depthFrame = new_frames.DepthFrame.DisposeWith(new_frames);
                //                var colorFrame = new_frames.ColorFrame.DisposeWith(new_frames);

                //                var colorizedDepth = colorizer.Process<VideoFrame>(depthFrame).DisposeWith(new_frames);

                //                // Render the frames.
                //                Dispatcher.Invoke(DispatcherPriority.Render, updateDepth, colorizedDepth);
                //                Dispatcher.Invoke(DispatcherPriority.Render, updateColor, colorFrame);
                //                Dispatcher.Invoke(new Action(() =>
                //                {
                //                    txtTimeStamp.Text = "Frames: " + ++framesCount;
                //                    //Console.WriteLine($"#{res.Number} {res.TimestampDomain} {res.Timestamp:F2}");
                //                }));
                //            }
                //        }
                //    }
                //}, tokenSource.Token);
                #endregion
                #region Record

                pipeline = new Pipeline();

                var cfg = new Config();
                cfg.EnableStream(Stream.Depth, 640, 480);
                cfg.EnableStream(Stream.Color, 640, 480);

                var pp = pipeline.Start(cfg);

                SetupWindow(pp, out updateDepth, out updateColor);

                var selected_device = pp.Device;
                var depth_sensor = selected_device.Sensors[0];

                if (depth_sensor.Options.Supports(Option.LaserPower))
                {
                    var laserPower = depth_sensor.Options[Option.LaserPower];
                    laserPower.Value = laserPower.Max; // Set max power
                }


                bool disposed = true;

                Task.Factory.StartNew(() =>
                {
                    while (!tokenSource.Token.IsCancellationRequested)
                    {
                        using (var frames = pipeline.WaitForFrames())
                        {
                            var colorFrame = frames.ColorFrame.DisposeWith(frames);
                            var depthFrame = frames.DepthFrame.DisposeWith(frames);

                            var colorizedDepth = colorizer.Process<VideoFrame>(depthFrame).DisposeWith(frames);

                            Dispatcher.Invoke(DispatcherPriority.Render, updateDepth, colorizedDepth);
                            Dispatcher.Invoke(DispatcherPriority.Render, updateColor, colorFrame);
                            Dispatcher.Invoke(() => { txtTimeStamp.Text = "Frames: " + tick++; });

                        }

                        if (disposed)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                disposed = false;
                                var path = dir.FullName + "\\" + DateTime.Now.Ticks + ".bag";
                                using (new WaitAndDispose(1000, new RecordDevice(selected_device, path))) //$"log\\ros{tick++}.bag"
                                {
                                    disposed = true;
                                    Console.WriteLine("recorded " + path);
                                };

                            });
                        }

                    }
                }, tokenSource.Token);
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
        }

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



    class WaitAndDispose : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public WaitAndDispose(int millisecodsDelay, RecordDevice dev)
        {
            Task.Delay(millisecodsDelay)
                    .ContinueWith(_ => Console.WriteLine(dev.FileName))
                    .ContinueWith(_ => dev.Dispose())
                    .Wait();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}



