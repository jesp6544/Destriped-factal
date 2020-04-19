using System.Drawing;

namespace Core.Models
{
    public class Pixel
    {
        public uint Placement { get; set; }         //Int is enough for 1 job to handle 65.000x65.000 picture, no need to go higher really..
        public Color Color { get; set; }        //Is it more efficient to use 3 ints as RGB?  //TODO: test this
    }
}