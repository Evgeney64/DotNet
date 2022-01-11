using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AForge;
using AForge.Video.DirectShow;
using Accord.Video.FFMPEG;
using System.IO;
using AForge.Video;
using System.Threading;

namespace New_Project_2
{
    public partial class Form1 : Form
    {
        private VideoCaptureDeviceForm captureDevice;
        private FilterInfoCollection videoDevice;

        private VideoCaptureDevice videoSource;

        private VideoFileWriter FileWriter = new VideoFileWriter();

        bool isRecord = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (isRecord == true)
                {
                    pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
                    FileWriter.WriteVideoFrame((Bitmap)eventArgs.Frame.Clone());
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            videoDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            captureDevice = new VideoCaptureDeviceForm();

            if (captureDevice.ShowDialog(this) == DialogResult.OK)
            {
                isRecord = true;

                int h = captureDevice.VideoDevice.VideoResolution.FrameSize.Height;
                int w = captureDevice.VideoDevice.VideoResolution.FrameSize.Width;
                FileWriter.Open("d:\\" + "recorded at " + DateTime.Now.ToString("HH-mm-ss") + ".mp4", w, h, 25, VideoCodec.MPEG4);

                videoSource = captureDevice.VideoDevice;
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                videoSource.Start();


            }
            //videoSource.DisplayPropertyPage(IntPtr.Zero)
        }




        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            isRecord = false;
            FileWriter.Close();


        }

        private void button4_Click(object sender, EventArgs e)
        {


        }

    }
}