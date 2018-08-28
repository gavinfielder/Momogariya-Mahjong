using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    public class Player : MonoBehaviour
    {
        private Hand hand;
        private Kawa kawa;
        public int PlayerNumber { get; private set; }
        private string accessKey = "";

        //Takes a turn. 
        public TurnResult Turn(TurnArgs args)
        {
            return new TurnResult();
        }


        //Returns a list of all revealed tiles of the hand
        public List<TileID> GetRevealed()
        {
            
            return new List<TileID>();
        }

        //Gives a hand to the player to set the references and access key
        public void SetupHand(int playerNumber, ref Hand h, ref Kawa k)
        {
            hand = h;
            kawa = k;
            PlayerNumber = playerNumber;
            hand.SetOwner(PlayerNumber, 0);
        }
    }
}
