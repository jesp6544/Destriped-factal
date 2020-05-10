using System.Drawing;
using Core;
using Core.Models;

namespace Gui_client
{
    class Logic
    {
        private readonly IRender _render;
        private Job _currentJob;
        
        public Logic(int imgWidth, int imgHeight)
        {
            _render = new Render(255);
            _currentJob = new Job
            {
                Height = imgHeight,
                Width = imgWidth,
                XMax = 1,
                XMin = -3,
                YMax = 2,
                YMin = -2
            };
        }

        public void Zoom(double percentFromRight, double percertFromTop)
        {
            var temp = (_currentJob.XMax - _currentJob.XMin) / 4;
            var tempX = _currentJob.XMin + (_currentJob.XMax - _currentJob.XMin) * percentFromRight;
            var tempY = _currentJob.YMin + (_currentJob.YMax - _currentJob.YMin) * percertFromTop;
            _currentJob = new Job
            {
                Width = _currentJob.Width,
                Height = _currentJob.Height,
                XMin = tempX - temp,
                XMax = tempX + temp,
                YMin = tempY - temp,
                YMax = tempY + temp
            };
        }

        public void ModifyJob(int xMax, int xMin, int yMax, int yMin)
        {
            _currentJob = new Job
            {
                Height = _currentJob.Height,
                Width = _currentJob.Width,
                XMax = xMax,
                XMin = xMin,
                YMax = yMax,
                YMin = yMin
            };
        }

        public Bitmap GetImage(int? overwriteWidth = null, int? overwriteHeight = null)
        {
            var job = _currentJob;
            if (overwriteHeight != null)
            {
                job.Height = (int) overwriteHeight;
            }
            if (overwriteWidth != null)
            {
                job.Width = (int) overwriteWidth;
            }
            return _render.GetMandlebrot(job);
        }
    }
}
