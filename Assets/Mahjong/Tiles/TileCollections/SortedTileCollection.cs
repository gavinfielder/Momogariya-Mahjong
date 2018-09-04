using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    //Maintains parallel sorted collections of Tile GameObjects and TileIDs
    public class SortedTileCollection : BaseTileCollection
    {
        //***********************************************************************
        //**************************** Basic Methods ****************************
        //***********************************************************************

        //Adds a tile to the collection
        //Note: if the tile is locked with a different access key, will not add
        new public void Add(Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            if (Contains(tile, _accessKey)) return;
            TileID id = tile.Query(_accessKey);
            if (id == TileID.Hidden) return;
            //Insert tile into sorted position
            int i = 0;
            while (i < tileIDs.Count && id > tileIDs[i])
                i++;
            tiles.Insert(i, tile);
            tileIDs.Insert(i, id);
        }

        //Removes the tile from the collection
        new public void Remove(Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            int i = IndexOf(tile, _accessKey);
            if (i == -1) return;
            tiles.RemoveAt(i);
            tileIDs.RemoveAt(i);
        }

        //***********************************************************************
        //**************************** Constructors *****************************
        //***********************************************************************

        //Creates an empty collection
        public SortedTileCollection() { }

        //Creates a collection using a list and an access key for the tiles in that list
        public SortedTileCollection(List<Tile> list, int accessKey = 0)
        {
            //Note the base constructor is called here, which means the list was populated
            //using the Add() of the unsorted base class. We'll need to clear and re-do it.
            _accessKey = accessKey;
            Clear(_accessKey);
            for (int i = 0; i < list.Count; i++)
                Add(list[i], _accessKey);
        }

        //***********************************************************************
        //************************* Collection Queries **************************
        //***********************************************************************

        //Returns whether the collection contains any matching tile
        new public bool Contains(TileID id, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return false;
            return (IndexOf(id, accessKey) != -1);
        }

        //Returns the index of the first matching tile or -1 if not found
        new public int IndexOf(TileID id, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return -1;
            int i = 0;
            while (i < tileIDs.Count && tileIDs[i] != id)
                i++;
            if (i < tileIDs.Count) return i;
            else return -1;
        }

        //Returns the number of matching tiles in the collection
        new public int CountOf(TileID id, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return -1;
            int index = IndexOf(id, _accessKey);
            if (index == -1) return 0;
            int count = 1;
            while (index + count < this.Count && tileIDs[index + count] == id)
                count++;
            return count;
        }        

    }

}
