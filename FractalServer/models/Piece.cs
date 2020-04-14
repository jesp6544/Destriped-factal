public class Piece
	{
		public ushort ID { get; set; }  //same as Job ID  //Can handle up to 16383 total cores in renderfarm  
		public List<Pixel> pixels { get; set; }
		public bool done { get; set; }  //Used to send to the host if this is the last part of the piece
		public int xLenth { get; set; }
	}