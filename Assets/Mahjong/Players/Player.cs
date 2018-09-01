using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    public abstract class Player : MonoBehaviour, IPlayer
    {
        protected Hand hand;
        protected Kawa kawa;
        protected GameBoard board;
        public int PlayerNumber { get; protected set; }
        protected int accessKey = 0;
        protected GameController controller;
        public Tile Discard
        {
            get
            {
                if (hand == null) return null;
                return hand.Discard;
            }
        }

        //Takes a turn. 
        public abstract void StartTurn(TurnArgs args);

        //Queries this player to see if they want a discarded tile
        public abstract void Offer(TileID tile);

        //Gives a hand to the player to set the references and access key
        public void Setup(int playerNumber, ref Hand h, ref Kawa k, GameController control, ref GameBoard gameBoard)
        {
            //accessKey = Security.AccessKeyHash(Security.GetRandomAccessKey());
            hand = h;
            kawa = k;
            board = gameBoard;
            controller = control;
            PlayerNumber = playerNumber;
            hand.SetOwner(accessKey);
            hand.PlayerNumber = playerNumber;
        }
        
        
        
    }
}
