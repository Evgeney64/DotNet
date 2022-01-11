using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Video;

namespace VideoCapture
{
    public class Accord
    {
        public void Start()
        {
            // http://accord-framework.net/docs/html/T_Accord_Video_ScreenCaptureStream.htm
            // get entire desktop area size
            //Rectangle screenArea = Rectangle.Empty;
            //foreach (System.Windows.Forms.Screen screen in
            //          System.Windows.Forms.Screen.AllScreens)
            //{
            //    screenArea = Rectangle.Union(screenArea, screen.Bounds);
            //}
            Rectangle screenArea = new Rectangle
            {
                Width = 1200,
                Height = 720,
            };

            // create screen capture video source
            ScreenCaptureStream stream = new ScreenCaptureStream(screenArea);

            // set NewFrame event handler
            stream.NewFrame += new NewFrameEventHandler(video_NewFrame);

            // start the video source
            stream.Start();

            stream.Stop();
            //for (int i = 0; i <= 100000000; i++) ;
            // ...
            // signal to stop
            //stream.SignalToStop();
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap bitmap = eventArgs.Frame;
            // process the frame
        }
    }
}
