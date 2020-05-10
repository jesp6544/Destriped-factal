using System.Drawing;
using Core.Models;

namespace Core
{
    public interface IRender
    {
        public Bitmap GetMandlebrot(Job job);

    }
}