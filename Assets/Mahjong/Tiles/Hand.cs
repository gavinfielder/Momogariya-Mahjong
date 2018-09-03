using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Mahjong
{
    public class Hand : MonoBehaviour, ISecured
    {
        private HandTileCollection _tiles = new HandTileCollection();
        public HandTileCollection Tiles
        {
            get
            {
                return _tiles;
            }
        }

        //Public data accessors
        public Tile Discard { get; private set; }
        private int _playerNumber;
        public int PlayerNumber
        {
            get
            {
                return _playerNumber;
            }
            set
            {
                _playerNumber = value;
                handRenderer.PlayerNumber = value;
            }
        }
        private List<OpenMeld> _openMelds = new List<OpenMeld>();
        public List<OpenMeld> OpenMelds {
            get
            {
                return _openMelds;
            }
        }
        public Tile Draw;
        
        //Internal Data
        private int _accessKey = 0;

        //Object references
        private HandRenderer handRenderer;

        //Initialization
        private void Start()
        {
            handRenderer = gameObject.GetComponent<HandRenderer>();
        }
        
        //***********************************************************************
        //*************************** Public Methods ****************************
        //***********************************************************************
        
        //Adds a tile to the hand. Does not trigger a draw event--used for dealing.
        public void Deal(Tile tile)
        {
            AddTile(tile);
        }

        //Adds a tile to the hand and sets that tile as the draw.
        public void AddDraw(Tile tile)
        {
            AddTile(tile);
            Draw = tile;
            EventManager.FlagEvent("Hand " + PlayerNumber + " Draw");
        }

        //Adds a tile to the hand but resets to no owner and sets to face up
        public void AddNaki(Tile tile)
        {
            AddTile(tile); //Note: sets owner
            Draw = tile;
            tile.ReleaseOwnership(_accessKey); //Resets to no owner
            tile.SetVisibility(TileVisibility.FaceUp);
        }
        
        //Removes the first occurring matching tile from the hand and sets it as a recent discard
        public void RemoveDiscard(Tile tile, Kawa kawa, int accessKey = 0)
        {
            if (accessKey != _accessKey && _accessKey != 0) return;
            Tiles.Remove(tile, _accessKey);
            Discard = tile;
            tile.ReleaseOwnership(_accessKey);
            tile.StolenFrom = PlayerNumber;
            kawa.Add(Discard.gameObject); //TODO: revisit when kawa is refactored to use Tile instead of GameObject
            EventManager.FlagEvent("Hand " + PlayerNumber + " Discard");
        }

        //Forms an open meld that matches the potential meld. Cannot be used for shouminkan.
        public void FormOpenMeld(PotentialMeld meld, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;
            TileID discardID = Kawa.MostRecentDiscard.Query();
            List<Tile> with = new List<Tile>();
            int start = 0;
            Meld.MeldType type = Meld.MeldType.Minkou;
            if (meld.Type == Meld.MeldType.Kantsu)
            {
                start = Tiles.IndexOf(discardID);
                with.Add(Tiles[start]);
                with.Add(Tiles[start + 1]);
                with.Add(Tiles[start + 2]);
                with.Add(Tiles[start + 3]);
                if (meld.Completed) type = Meld.MeldType.Ankan;
                else type = Meld.MeldType.Daiminkan;
            }
            else if (meld.Type == Meld.MeldType.Koutsu)
            {
                start = Tiles.IndexOf(discardID);
                with.Add(Tiles[start]);
                with.Add(Tiles[start + 1]);
                with.Add(Tiles[start + 2]);
                type = Meld.MeldType.Minkou;
            }
            else if (meld.Type == Meld.MeldType.Shuntsu)
            {
                //Use the self-sorting property of PotentialMeld to get the first ID
                PotentialMeld completed = new PotentialMeld(meld.IDs);
                completed.Add(discardID);
                with.Add(Tiles[Tiles.IndexOf(completed.IDs[0])]);
                with.Add(Tiles[Tiles.IndexOf(completed.IDs[1])]);
                with.Add(Tiles[Tiles.IndexOf(completed.IDs[2])]);
                type = Meld.MeldType.Minjun;
            }
            //All the tiles are found, now form the open meld
            Tiles.OpenTiles(with, _accessKey);
            OpenMeld open = new OpenMeld(type, with, PlayerNumber);
            OpenMelds.Add(open);
            EventManager.FlagEvent("Hand " + PlayerNumber + " Open Meld");
        }

        //***********************************************************************
        //************************** Internal Methods ***************************
        //***********************************************************************

        private void AddTile(Tile tile)
        {
            tile.SetOwner(_accessKey);
            Tiles.Add(tile);
            //Set tile rendering orientation
            handRenderer.UpdateOrientation(tile);
        }

        //***********************************************************************
        //**************** Setup and Reference Accessor Methods *****************
        //***********************************************************************

        //Sets the access key
        public void SetOwner(int newAccessKey)
        {
            if (_accessKey != 0) return;
            _accessKey = newAccessKey;
            Tiles.SetOwner(_accessKey);
        }

        //Resets the access key to 0 (no owner)
        public void ReleaseOwnership(int accessKey)
        {
            if (accessKey == _accessKey)
            {
                Tiles.ReleaseOwnership(_accessKey);
                _accessKey = 0;
            }
        }

    }
}
