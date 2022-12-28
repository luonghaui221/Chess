using Resources.Script.ChessPieces;

namespace Resources.Script
{
    public enum MoveType
    {
        Normal,
        Enpassant,
        Promote,
        Castling
    }
    public class History
    {
        public Pieces piece;
        public Location from;
        public Location to;

        public Pieces catched;
        public MoveType type;
        public History(Pieces piece, Location from, Location to,Pieces catched,MoveType type = MoveType.Normal)
        {
            this.piece = piece;
            this.from = from;
            this.to = to;
            this.catched = catched;
            this.type = type;
        }
    }
}