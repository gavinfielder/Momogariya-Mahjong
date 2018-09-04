using System;
using System.Collections.Generic;

namespace Mahjong
{
    public class UnsortedTileCollection : BaseTileCollection
    {
        //***********************************************************************
        //***************************** New Methods *****************************
        //***********************************************************************

        //Inserts the tile at the specified index
        public void Insert(int index, Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            if (tiles.Contains(tile)) return;
            TileID id = tile.Query(_accessKey);
            if (id == TileID.Hidden) return;
            if (index >= Count) Add(tile, _accessKey);
            else
            {
                if (index <= 0) index = 0;
                tiles.Insert(index, tile);
                tileIDs.Insert(index, id);
            }
        }

        //Removes the tile at the specified index
        public void RemoveAt(int index, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            if (index < 0 || index >= Count) return;
            tiles.RemoveAt(index);
            tileIDs.RemoveAt(index);
        }

        //***********************************************************************
        //**************************** Basic Methods ****************************
        //***********************************************************************

        //Indexer, now with set
        new public Tile this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) return null;
                else return tiles[index];
            }
            set
            {
                //Only works if the tile is not already present in the collection and
                //the access key on the collection is the same as the access key on the tile
                if (index < 0 || index >= Count) return;
                if (Contains(value, _accessKey)) return;
                TileID id = value.Query(_accessKey);
                if (id == TileID.Hidden) return;
                tiles[index] = value;
                tileIDs[index] = id;
            }
        }
    }
}
