using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    //Maintains parallel sorted collections of Tile GameObjects and TileIDs
    public class TileCollection : ISecured
    {
        protected int _accessKey = 0;
        protected List<GameObject> tileObjs = new List<GameObject>();
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

        //Creates an empty collection
        public TileCollection() { }

        //Creates a collection using a list and an access key for the tiles in that list
        public TileCollection(List<Tile> list, int accessKey = 0)
        {
            _accessKey = accessKey;
            for (int i = 0; i < list.Count; i++)
                Add(list[i], _accessKey);
        }

        //Adds a tile to the collection
        //Note: if the tile is locked with a different access key, will not add
        public void Add(Tile tile, int accessKey = 0)
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
            tileObjs.Insert(i, tile.gameObject);
            tiles.Insert(i, tile);
            tileIDs.Insert(i, id);
        }

        //Removes the tile from the collection
        public void Remove(Tile tile, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            int i = IndexOf(tile, _accessKey);
            if (i == -1) return;
            tileObjs.RemoveAt(i);
            tiles.RemoveAt(i);
            tileIDs.RemoveAt(i);
        }

        //Removes all the tiles in the collection
        public void Clear(int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            tileObjs.Clear();
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
        //************************* Collection Queries **************************
        //***********************************************************************

        //Returns whether the collection contains the Tile GameObject
        public bool Contains(GameObject tileObj, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return false;
            return (IndexOf(tileObj, accessKey) != -1);
        }

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

        //Returns the index of the Tile GameObject or -1 if not found
        public int IndexOf(GameObject tileObj, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return -1;
            return tileObjs.IndexOf(tileObj);
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
            int i = 0;
            while (i < tileIDs.Count && tileIDs[i] != id)
                i++;
            if (i < tileIDs.Count) return i;
            else return -1;
        }

        //Returns the number of matching tiles in the collection
        public int CountOf(TileID id, int accessKey = 0)
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


        //***********************************************************************
        //*********************** List Reference Accessors **********************
        //***********************************************************************

        //Returns a reference to the GameObject collection
        public List<GameObject> GetGameObjectList()
        {
            return tileObjs;
        }

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

    }





    //Also maintains a list of closed tiles
    public class HandTileCollection : TileCollection
    {
        private TileCollection _closed = new TileCollection();
        public TileCollection Closed
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
