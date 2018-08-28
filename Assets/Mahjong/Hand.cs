using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Mahjong
{
    public class Hand : MonoBehaviour
    {
        //Parallel lists 
        private List<GameObject> tiles = new List<GameObject>(); 
        private List<TileID> hand = new List<TileID>();

        private GameObject Draw;
        private TileID DrawID
        {
            get
            {
                return Draw.GetComponent<Tile>().Query(_accessKey);
            }
        }
        //The latest draw is contained in this list. Shared with owning player.
        private List<TileID> DrawIDContainer = new List<TileID>();
        private List<GameObject> DrawContainer = new List<GameObject>(); //for renderer

        public GameObject Discard { get; private set; }
        public int PlayerNumber { get; private set; }
        private HandRenderer handRenderer;
        public int Count { get { return tiles.Count; } }
        private int _accessKey = 0;


        //Initialization
        private void Start()
        {
            handRenderer = gameObject.GetComponent<HandRenderer>();
            handRenderer.SyncWithHand(ref tiles, ref DrawContainer);
        }
        

        //Adds a tile to the hand. Does not trigger a draw event--used for dealing.
        public void Deal(GameObject tileObj)
        {
            //Take ownership of tile
            Tile tile = tileObj.GetComponent<Tile>();
            tile.SetOwner(_accessKey);
            TileID tileID = tile.Query(_accessKey);

            //Insert tile into sorted position
            int i = 0;
            while (i < hand.Count && tileID > hand[i])
                i++;
            hand.Insert(i, tileID);
            tiles.Insert(i, tileObj);

            //Set tile rendering orientation
            handRenderer.UpdateOrientation(tileObj);
        }
        //Adds a tile to the hand and sets that tile as the draw.
        public void AddDraw(GameObject tile)
        {
            Deal(tile);
            Draw = tile;
            DrawContainer.Clear();
            DrawContainer.Add(tile);
            EventManager.FlagEvent("Hand " + PlayerNumber + " Draw");
        }
        
        //Removes the first occurring matching tile from the hand and sets it as a recent discard
        public void RemoveDiscard(TileID tileID)
        {
            int i = hand.IndexOf(tileID);
            Discard = tiles[i];
            tiles.RemoveAt(i);
            hand.RemoveAt(i);
            EventManager.FlagEvent("Hand " + PlayerNumber + " Discard");
        }

        //Returns the tile IDs of the hand so that the owning players can read it
        public List<TileID> GetHandReference(int accessKey = 0)
        {
            if (_accessKey == 0 || accessKey == _accessKey)
                return hand;
            else return null;
        }

        public List<TileID> GetDrawIDContReference(int accessKey = 0)
        {
            if (_accessKey == 0 || accessKey == _accessKey)
                return DrawIDContainer;
            else return null;
        }
     
        //Sets the access key and hand name and clears the hand
        public void SetOwner(int playerNumber, int accessKey)
        {
            PlayerNumber = playerNumber;
            handRenderer.PlayerNumber = playerNumber;
            _accessKey = accessKey;
            tiles.Clear();
            hand.Clear();
            DrawContainer.Clear();
            Draw = null;
            Discard = null;
            //Reset openly readable references to cut any existing links
            hand = new List<TileID>();
            DrawIDContainer = new List<TileID>();
        }
    }
}
