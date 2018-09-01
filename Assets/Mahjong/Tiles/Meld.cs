using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    public abstract class Meld
    {
        public enum MeldType : byte
        {
            Anjun,
            Minjun,
            Ankou,
            Minkou,
            Ankan,
            Shouminkan,
            Daiminkan,
            Jantou,
            Koutsu,
            Kantsu,
            Shuntsu
        }
        public MeldType Type;
        protected List<TileID> _ids = new List<TileID>();
        public int Count
        {
            get
            {
                return _ids.Count;
            }
        }
        public List<TileID> IDs
        {
            get
            {
                return _ids;
            }
        }
        public abstract TileID this[int index] { get; } //TODO: should this retrieve the Tile instead?

    }

    //Potential Melds are for hand strategy
    public class PotentialMeld : Meld
    {
        private List<TileID> _waits = new List<TileID>();
        public List<TileID> Waits
        {
            get
            {
                return _waits;
            }
        }
        public bool Completed = false;
        public int Fungibility = 1; //used by AI. Set to negative to lock (as in open meld)
        public float Value = 0f; //used by AI.
        public bool Overlaps = false; //used by AI. true if the wait completes 2 or more melds, 
                                      //or if the completed meld overlaps with another completed meld

        //Insert tile into sorted position
        public void Add(TileID tile)
        {
            int i = 0;
            while (i < _ids.Count && tile > _ids[i])
                i++;
            _ids.Insert(i, tile);
        }

        //Removes the first occurrence of the tile
        public void Remove(TileID tile)
        {
            _ids.Remove(tile);
        }

        //Indexer
        public override TileID this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) return TileID.Invalid;
                else return _ids[index];
            }
        }

        //Comparison operators
        public static bool operator ==(PotentialMeld m1, PotentialMeld m2)
        {
            if (m1.Count != m2.Count) return false;
            for (int i = 0; i < m1.Count; i++)
                if (m1[i] != m2[i]) return false;
            return true;
        }
        public static bool operator !=(PotentialMeld m1, PotentialMeld m2)
        {
            return !(m1 == m2);
        }
        public override bool Equals(object other)
        {
            if (other == null) return false;
            return (this == (PotentialMeld)other);
        }
        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < _ids.Count; i++)
            {
                hash |= (((int)_ids[i].Suit)) << (i * 6);
                hash |= (((int)_ids[i].Number - 1) << 3) << (i * 6);
            }
            return hash;
        }
    }

    //Open melds involve a stolen discard
    public class OpenMeld : Meld
    {

        private TileCollection _tiles = new TileCollection();
        public TileCollection Tiles
        {
            get
            {
                return _tiles;
            }
        }
        public int PlayerNumber { get; private set; }
        //Should always be sorted and also match the unordered game object list
        new public List<TileID> IDs
        {
            get
            {
                return Tiles.GetTileIDList();
            }
        }
        new public int Count
        {
            get
            {
                return Tiles.Count;
            }
        }
        public float Width
        {
            get
            {
                switch (Type)
                {
                    case MeldType.Minjun:
                        return (2 * Constants.ADJ_TILE_SPACING + Constants.TILE_LENGTH);
                    case MeldType.Minkou:
                        return (2 * Constants.ADJ_TILE_SPACING + Constants.TILE_LENGTH);
                    case MeldType.Shouminkan:
                        return (2 * Constants.ADJ_TILE_SPACING + Constants.TILE_LENGTH);
                    case MeldType.Daiminkan:
                        return (3 * Constants.ADJ_TILE_SPACING + Constants.TILE_LENGTH);
                    default:
                        return 0f;
                }
            }
        }
        private int _stolenFrom = -1;
        public int StolenFrom
        {
            get
            {
                if (_stolenFrom == -1)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (Tiles[i].GetComponent<Tile>().StolenDiscard)
                            _stolenFrom = Tiles[i].GetComponent<Tile>().StolenFrom;
                    }
                }
                return _stolenFrom;
            }
        }
        
        //Constructor
        public OpenMeld(MeldType type, List<Tile> tiles, int playerNumber)
        {
            Type = type;
            _tiles = new TileCollection(tiles);
            PlayerNumber = playerNumber;
            for (int i = 0; i < Tiles.Count; i++)
                Tiles[i].InOpenMeld = true;
        }

        //Retrieves the tile ID 
        //TODO: should this retrieve the Tile instead?
        public override TileID this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) return null;
                return Tiles[index].Query();
            }
        }

        //Turns a minkou into a shouminkan
        public void MakeShouminkan(Tile draw)
        {
            Type = MeldType.Shouminkan;
            Tiles.Add(draw);
            draw.GetComponent<Tile>().InOpenMeld = true;
            GameObject container = Tiles[0].gameObject.transform.parent.gameObject;
            ArrangeOpenMeldGroup(ref container, this);
        }

        //Creates a PotentialMeld version of itself
        public PotentialMeld ConvertToPotentialMeld()
        {
            PotentialMeld meld = new PotentialMeld();
            foreach (TileID tile in IDs)
                meld.Add(tile);
            meld.Type = Type;
            meld.Completed = true;
            meld.Fungibility = -1;
            return meld;
        }

        //Arranges an open meld within the transform of the container
        public static void ArrangeOpenMeldGroup(ref GameObject container, OpenMeld meld)
        {
            meld.PlayerNumber = meld.PlayerNumber;
            int i = 0;
            List<TileRenderer> tr = new List<TileRenderer>(4);
            for (i = 0; i < meld.Count; i++)
            {
                meld.Tiles[i].transform.parent = container.transform;
                tr[i] = meld.Tiles[i].GetComponent<TileRenderer>();
            }
            TileRenderer low = null;
            TileRenderer mid = null;
            TileRenderer high = null;
            Tile a = null;
            Tile b = null;
            Tile c = null;
            TileRenderer stolen = null;

            switch (meld.Type)
            {
                case MeldType.Ankan:
                    tr[0].Orientation = TileOrientation.Bottom;
                    tr[1].Orientation = TileOrientation.Bottom;
                    tr[2].Orientation = TileOrientation.Bottom;
                    tr[3].Orientation = TileOrientation.Bottom;
                    tr[0].LowerRight = new Vector2(0.5f * meld.Width, 0f);
                    tr[1].LowerRight = tr[0].LowerLeft;
                    tr[2].LowerRight = tr[1].LowerLeft;
                    tr[3].LowerRight = tr[2].LowerLeft;
                    meld.Tiles[1].GetComponent<Tile>().SetVisibility(TileVisibility.FaceDown);
                    meld.Tiles[2].GetComponent<Tile>().SetVisibility(TileVisibility.FaceDown);
                    break;

                case MeldType.Minjun:
                    for (i = 0; i < 3; i++)
                    {
                        if (meld.Tiles[i].GetComponent<Tile>().StolenDiscard)
                        {
                            stolen = meld.Tiles[i].GetComponent<TileRenderer>();
                        }
                        else if (a == null) a = meld.Tiles[i];
                        else b = meld.Tiles[i];
                    }
                    if (a.GetComponent<Tile>().Query() < b.GetComponent<Tile>().Query())
                    {
                        low = a.GetComponent<TileRenderer>();
                        high = b.GetComponent<TileRenderer>();
                    }
                    else
                    {
                        low = b.GetComponent<TileRenderer>();
                        high = a.GetComponent<TileRenderer>();
                    }
                    //Always stolen from left (kamicha)
                    stolen.Orientation = TileOrientation.Right;
                    low.Orientation = TileOrientation.Bottom;
                    high.Orientation = TileOrientation.Bottom;
                    high.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                    low.LowerRight = high.LowerRight;
                    stolen.LowerRight = low.LowerLeft;
                    break;

                case MeldType.Minkou:
                    for (i = 0; i < 3; i++)
                    {
                        if (meld.Tiles[i].GetComponent<Tile>().StolenDiscard)
                        {
                            stolen = meld.Tiles[i].GetComponent<TileRenderer>();
                        }
                        else if (a == null) a = meld.Tiles[i];
                        else b = meld.Tiles[i];
                    }
                    low = a.GetComponent<TileRenderer>();
                    high = b.GetComponent<TileRenderer>();
                    low.Orientation = TileOrientation.Bottom;
                    high.Orientation = TileOrientation.Bottom;
                    stolen.Orientation = TileOrientation.Right;
                    if ((meld.StolenFrom - meld.PlayerNumber + 4) % 4 == 1)
                    {
                        //Stole from right (shimocha)
                        stolen.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                        low.LowerRight = stolen.LowerLeft;
                        high.LowerRight = low.LowerLeft;
                    }
                    else if ((meld.StolenFrom - meld.PlayerNumber + 4) % 4 == 2)
                    {
                        //stolen from across (toimen)
                        low.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                        stolen.LowerRight = low.LowerLeft;
                        high.LowerRight = stolen.LowerLeft;
                    }
                    else if ((meld.StolenFrom - meld.PlayerNumber + 4) % 4 == 3)
                    {
                        //stolen from left (kamicha)
                        low.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                        high.LowerRight = low.LowerLeft;
                        stolen.LowerRight = high.LowerLeft;
                    }
                    break;

                case MeldType.Daiminkan:
                    for (i = 0; i < 4; i++)
                    {
                        if (meld.Tiles[i].GetComponent<Tile>().StolenDiscard)
                        {
                            stolen = meld.Tiles[i].GetComponent<TileRenderer>();
                        }
                        else if (a == null) a = meld.Tiles[i];
                        else if (b == null) b = meld.Tiles[i];
                        else c = meld.Tiles[i];
                    }
                    low = a.Renderer;
                    high = b.Renderer;
                    mid = c.Renderer;
                    low.Orientation = TileOrientation.Bottom;
                    high.Orientation = TileOrientation.Bottom;
                    mid.Orientation = TileOrientation.Right;
                    stolen.Orientation = TileOrientation.Right;
                    if ((meld.StolenFrom - meld.PlayerNumber + 4) % 4 == 1)
                    {
                        //Stole from right (shimocha)
                        stolen.Orientation = TileOrientation.Left;
                        stolen.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                        low.LowerRight = stolen.LowerLeft;
                        high.LowerRight = low.LowerLeft;
                        mid.LowerRight = stolen.UpperRight;
                    }
                    else if ((meld.StolenFrom - meld.PlayerNumber + 4) % 4 == 2)
                    {
                        //stolen from across (toimen)
                        low.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                        stolen.LowerRight = low.LowerLeft;
                        high.LowerRight = stolen.LowerLeft;
                        mid.LowerRight = stolen.UpperRight;
                    }
                    else if ((meld.StolenFrom - meld.PlayerNumber + 4) % 4 == 3)
                    {
                        //stolen from left (kamicha)
                        low.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                        high.LowerRight = low.LowerLeft;
                        stolen.LowerRight = high.LowerLeft;
                        mid.LowerRight = stolen.UpperRight;
                    }
                    break;

                case MeldType.Shouminkan:
                    for (i = 0; i < 4; i++)
                    {
                        if (meld.Tiles[i].GetComponent<Tile>().StolenDiscard)
                        {
                            stolen = meld.Tiles[i].Renderer;
                        }
                        else if (a == null) a = meld.Tiles[i];
                        else if (b == null) b = meld.Tiles[i];
                        else c = meld.Tiles[i];
                    }
                    low = a.Renderer;
                    high = b.Renderer;
                    mid = c.Renderer;
                    low.Orientation = TileOrientation.Bottom;
                    mid.Orientation = TileOrientation.Bottom;
                    high.Orientation = TileOrientation.Bottom;
                    stolen.Orientation = TileOrientation.Right;
                    if ((meld.StolenFrom - meld.PlayerNumber + 4) % 4 == 1)
                    {
                        //Stole from right (shimocha)
                        stolen.Orientation = TileOrientation.Left;
                        stolen.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                        low.LowerRight = stolen.LowerLeft;
                        high.LowerRight = low.LowerLeft;
                        mid.LowerRight = high.LowerLeft;
                    }
                    else if ((meld.StolenFrom - meld.PlayerNumber + 4) % 4 == 2)
                    {
                        //stolen from across (toimen)
                        low.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                        stolen.LowerRight = low.LowerLeft;
                        high.LowerRight = stolen.LowerLeft;
                        mid.LowerRight = high.LowerLeft;
                    }
                    else if ((meld.StolenFrom - meld.PlayerNumber + 4) % 4 == 3)
                    {
                        //stolen from left (kamicha)
                        low.LowerRight = new Vector2(0.5f * meld.Width, 0f);
                        mid.LowerRight = low.LowerLeft;
                        high.LowerRight = mid.LowerLeft;
                        stolen.LowerRight = high.LowerLeft;
                    }
                    break;
            }
        }



    }
}
