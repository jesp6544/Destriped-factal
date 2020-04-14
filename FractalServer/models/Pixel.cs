public class Pixel
{
	public uint Placement { get; set; }         //Int is enough for 1 job to handle 65.000x65.000 picture, no need to go higherfor now
	public byte color { get; set; }        //0-255 colors, might make this better in the furture
}