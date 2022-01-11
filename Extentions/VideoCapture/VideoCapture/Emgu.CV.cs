using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Load_Images
{
    public partial class Form2 : Form
    {
        //public Form2()
        //{
        //    InitializeComponent();
        //}

        double TotalFrame;
        double Fps;
        int captureFrameNo;

        VideoCapture capture;
        VideoWriter writer;

        bool isCapturing = false;

        public void Start()
        {
            if (isCapturing == false)
            {

                capture = new VideoCapture(0);

                TotalFrame = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);
                Fps = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);


                isCapturing = true;

                int fourcc = fourcc = VideoWriter.Fourcc('H', '2', '6', '4');
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 2048);
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 1024);

                //int fourcc = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FourCC));
                int width = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth));
                int height = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight));

                string destin_path = "D:\\out.mp4";
                writer = new VideoWriter(destin_path, fourcc, Fps, new Size(width, height), true);


                capture.ImageGrabbed += Capture_ImageGrabbed;
                capture.Start();
            }

        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                if (isCapturing == true)
                {

                    if (capture == null)
                    {

                        return;

                    }


                    Mat m = new Mat();
                    capture.Retrieve(m);
                    writer.Write(m);
                    pictureBox1.Image = m.ToImage<Bgr, byte>().Bitmap;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Stop()
        {
            if (isCapturing == true) {

                capture.Stop();
                isCapturing = false;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (capture != null) {

                capture.Pause();

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {


            if (writer.IsOpened)
            {

                isCapturing = false;
                writer.Dispose();

            }
            MessageBox.Show("Completed");
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}