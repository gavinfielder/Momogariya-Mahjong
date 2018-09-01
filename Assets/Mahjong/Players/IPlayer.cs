using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    //Defines the interface by which the game controller interacts with both user players and AI players
    public interface IPlayer
    {
        //Offers a discard to be called as naki
        void Offer(TileID tile);
        //Starts the player's turn
        void StartTurn(TurnArgs args);
        //Sets up the player with a hand
        void Setup(int playerNumber, ref Hand h, ref Kawa k, GameController control, ref GameBoard gameBoard);
    }
}
