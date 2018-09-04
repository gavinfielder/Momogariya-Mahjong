using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class CustomSortedTileCollection : BaseTileCollection
    {
        private HandSortingMethod sorting;
        
        //***********************************************************************
        //***************************** New Methods *****************************
        //***********************************************************************

        //Sets the sorting method and re-sorts the hand
        public void SetSortingMethod(HandSortingMethod sortingMethod, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            List<Tile> temp = GetTileList();
            Clear(_accessKey);
            sorting = sortingMethod;
            for (int i = 0; i < temp.Count; i++)
            {
                Add(temp[i], _accessKey);
            }
        }

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
            while (i < tileIDs.Count && sorting.GreaterThan(id, tileIDs[i]))
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
        public CustomSortedTileCollection(HandSortingMethod sortingMethod)
        {
            sorting = sortingMethod;
        }

        //Creates a collection using a list and an access key for the tiles in that list
        public CustomSortedTileCollection(HandSortingMethod sortingMethod, List<Tile> list, int accessKey = 0)
        {
            _accessKey = accessKey;
            sorting = sortingMethod;
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
