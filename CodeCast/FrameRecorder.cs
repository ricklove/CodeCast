using AForge.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCast
{
    class FrameRecorder : IDisposable
    {
        private VideoFileWriter videoWriter;

        public int FramesPerSecond { get; private set; }

        public FrameRecorder(int framesPerSecond, string outputPath, int width, int height)
        {
            int videoBitRate = 1200 * 1000;

            FramesPerSecond = framesPerSecond;
            videoWriter = new VideoFileWriter();
            videoWriter.Open(outputPath, width, height, framesPerSecond, VideoCodec.H264, videoBitRate);
        }

        public void SaveFrame(Bitmap image)
        {
            videoWriter.WriteVideoFrame(image);
        }

        public void Close()
        {
            if (videoWriter != null)
            {
                videoWriter.Close();
                videoWriter.Dispose();
                videoWriter = null;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
