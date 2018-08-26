using System.Collections.Generic;

namespace Mahjong
{
    public class Player
    {
        private Hand hand;
        private Kawa kawa;

        //Gives a new hand and kawa for a new hand in the round
        public void Deal(Hand h, Kawa k)
        {
            hand = h;
            kawa = k;
        }

        //Takes a turn. 
        public TurnResult Turn(TurnArgs args)
        {
            return new TurnResult();
        }


        //Returns a list of all revealed tiles of the hand
        public List<Tile> GetRevealed()
        {
            
            return new List<Tile>();
        }
    }
}
