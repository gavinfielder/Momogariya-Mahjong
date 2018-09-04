using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public abstract class BaseTileCollection : ISecured
    {
        protected int _accessKey = 0;
        protected List<Tile> tiles = new List<Tile>();
        protected List<TileID> tileIDs = new List<TileID>();
        public int Count
        {
            get
            {
                return tiles.Count;
            }
        }

        //***********************************************************************
        //**************************** Basic Methods ****************************
        //***********************************************************************

        //Adds the tile to the collection
        //Note: if the tile is locked with a different access key, will not add
        public void Add(Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;

            if (Contains(tile, _accessKey)) return;
            TileID id = tile.Query(_accessKey);
            if (id == TileID.Hidden) return;
            tiles.Add(tile);
            tileIDs.Add(id);
        }

        //Removes the tile from the collection
        public void Remove(Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            if (!(Contains(tile, _accessKey))) return;
            TileID id = tile.Query(_accessKey);
            tiles.Remove(tile);
            tileIDs.Remove(id);
        }

        //Removes all the tiles in the collection
        public void Clear(int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            tiles.Clear();
            tileIDs.Clear();
        }

        //Indexer
        public Tile this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) return null;
                else return tiles[index];
            }
        }


        //***********************************************************************
        //**************************** Constructors *****************************
        //***********************************************************************

        //Creates an empty collection
        public BaseTileCollection() { }

        //Creates a collection using a list and an access key for the tiles in that list
        public BaseTileCollection(List<Tile> list, int accessKey = 0)
        {
            _accessKey = accessKey;
            for (int i = 0; i < list.Count; i++)
                Add(list[i], _accessKey);
        }

        //***********************************************************************
        //************************* Collection Queries **************************
        //***********************************************************************

        //Returns whether the collection contains the tile
        public bool Contains(Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return false;
            return (IndexOf(tile, accessKey) != -1);
        }

        //Returns whether the collection contains any matching tile
        public bool Contains(TileID id, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return false;
            return (IndexOf(id, accessKey) != -1);
        }

        //Returns the index of the Tile or -1 if not found
        public int IndexOf(Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return -1;
            return tiles.IndexOf(tile);
        }

        //Returns the index of the first matching tile or -1 if not found
        public int IndexOf(TileID id, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return -1;
            return tileIDs.IndexOf(id);
        }

        //Returns the number of matching tiles in the collection
        public int CountOf(TileID id, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return -1;
            int count = 0;
            for (int i = 0; i < Count; i++)
                if (tileIDs[i] == id) count++;
            return count;
        }


        //***********************************************************************
        //*********************** List Reference Accessors **********************
        //***********************************************************************

        //Returns a reference to the Tile collection
        public List<Tile> GetTileList()
        {
            return tiles;
        }

        //Returns a reference to the TileID collection
        public List<TileID> GetTileIDList(int accessKey = 0)
        {
            if (_accessKey == 0 || (accessKey == _accessKey))
                return tileIDs;
            else return null;
        }


        //***********************************************************************
        //************************** Access Management **************************
        //***********************************************************************

        //Sets the access key. Only works if no owner (current key is zero)
        public void SetOwner(int newAccessKey)
        {
            if (_accessKey == 0) _accessKey = newAccessKey;
        }

        //Resets the access key to no owner (0)
        public void ReleaseOwnership(int accessKey)
        {
            if (accessKey == _accessKey) _accessKey = 0;
        }

    }
}
