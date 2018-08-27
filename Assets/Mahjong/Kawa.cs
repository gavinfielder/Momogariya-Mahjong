using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    //This class represents the pond of one player
    public class Kawa : MonoBehaviour
    {
        public List<TileID> Tiles { get; private set; }

        //Adds a discarded tile to the pond
        public void Add(TileID tile)
        {

        }

        //Steals the last discard from this pond. 
        //Note the stolen tile is still in the pond
        public TileID Steal()
        {
            if (Tiles.Count == 0) return TileID.Invalid;
            Tiles[Tiles.Count - 1].StolenDiscard = true;
            return Tiles[Tiles.Count - 1];
        }


    }
}
