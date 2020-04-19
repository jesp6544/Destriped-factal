using System.Collections.Generic;

namespace Core.Models
{
    public class Piece
    {
        public ushort Id { get; set; }  //same as Job ID
        //TODO: might add offset to enable transfer while going
        public List<Pixel> Pixels { get; set; }
    }
}