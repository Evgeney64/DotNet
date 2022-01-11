using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Captura;
using VideoCapture;
//using Load_Images;

namespace VideoCapture
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //var rec = new Recorder(new RecorderParams("out.avi", 10, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, 70));
            Accord accord = new Accord();
            accord.Start();
        }

        static private Image CaptureScreen()
        {
            //https://studassistent.ru/charp/zahvat-video-s-ekrana-c
            Bitmap bmp = new Bitmap(1200, 720);
            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(0, 0, 0, 0, new Size(bmp.Width, bmp.Height));
            //pictureBox1.Image = bmp;
            //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            return bmp;
        }

        static private Image CaptureScreen1()
        {
            Rectangle screenSize = new Rectangle
            {
                Width = 1200,
                Height = 720,
            };
            Bitmap target = new Bitmap(screenSize.Width, screenSize.Height);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.CopyFromScreen(0, 0, 0, 0, new Size(screenSize.Width, screenSize.Height));
            }
            return target;
        }
    }
}
