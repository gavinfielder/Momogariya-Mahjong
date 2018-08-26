using System.Collections.Generic;


namespace Mahjong
{
    //This class represents the pond of one player
    public class Kawa
    {
        public List<Tile> Tiles { get; private set; }

        //Adds a discarded tile to the pond
        public void Add(Tile tile)
        {

        }

        //Steals the last discard from this pond. 
        //Note the stolen tile is still in the pond
        public Tile Steal()
        {
            if (Tiles.Count == 0) return Tile.Invalid;
            Tiles[Tiles.Count - 1].StolenDiscard = true;
            return Tiles[Tiles.Count - 1];
        }


    }
}
