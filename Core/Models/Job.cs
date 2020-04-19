namespace Core.Models
{
    public class Job
    {
        public ushort Id { get; set; }
        //Some var(s) that carry the calculation
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}