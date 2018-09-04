using System;
using System.Collections.Generic;

namespace Mahjong
{
    //Also maintains a list of closed tiles
    public class HandTileCollection : SortedTileCollection
    {
        private SortedTileCollection _closed = new SortedTileCollection();
        public SortedTileCollection Closed
        {
            get
            {
                return _closed;
            }
        }

        //Creates an empty hand collection
        public HandTileCollection() { }

        //Initializes the access key on Closed collection
        public HandTileCollection(List<Tile> list, int accessKey = 0)
        {
            //Note: base constructor called first
            Closed.SetOwner(accessKey);
        }


        //***********************************************************************
        //***************************** New Methods *****************************
        //***********************************************************************

        //Opens all of the tiles specified
        public void OpenTiles(List<Tile> list, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            int index;
            for (int i = 0; i < list.Count; i++)
            {
                index = tiles.IndexOf(list[i], _accessKey);
                if (index != -1)
                {
                    tiles[index].ReleaseOwnership(_accessKey);
                    tiles[index].SetVisibility(TileVisibility.FaceUp);
                    Closed.Remove(tiles[index], _accessKey);
                }
            }
        }

        //***********************************************************************
        //**************** Adjusted versions of base functions ******************
        //***********************************************************************

        //Adds a tile to the collection
        //Note: if the tile is locked with a different access key, will not add
        new public void Add(Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            base.Add(tile, accessKey);
            if (tile.InOpenMeld == false) Closed.Add(tile, _accessKey);
        }

        //Removes the tile from the collection
        new public void Remove(Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            base.Remove(tile, accessKey);
            Closed.Remove(tile, _accessKey);
        }

        //Removes all the tiles in the collection
        new public void Clear(int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            base.Clear(_accessKey);
            Closed.Clear(_accessKey);
        }

    }
}
