using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Mahjong
{
    public class Hand : MonoBehaviour
    {
        private List<Tile> tiles = new List<Tile>();
        private Tile draw;
        public Tile RecentDiscard { get; private set; }
        public int PlayerNumber { get; private set; }
        private HandRenderer handRenderer;
        public int Count { get { return tiles.Count; } }
        private int accessHash = 0;
        private bool initialized = false;


        //Initialization
        private void Start()
        {
            Initialize();
        }

        //Initialization
        private void Initialize()
        {
            if (initialized) return;
            handRenderer = gameObject.GetComponent<HandRenderer>();
        }

        //Adds a tile to the hand. Does not trigger a draw event--used for dealing.
        public void Add_NoDrawEvent(Tile tile)
        {
            int i = 0; 
            while (i < tiles.Count && tile > tiles[i])
                i++;
            tiles.Insert(i, tile);
        }
        //Adds a tile to the hand and sets that tile as the draw.
        public void AddDraw(Tile tile)
        {
            Add_NoDrawEvent(tile);
            draw = tile;
            EventManager.FlagEvent("Hand " + PlayerNumber + " Draw");
        }
        
        //Removes the first occurring matching tile from the hand and sets it as a recent discard
        public void RemoveDiscard(Tile tile)
        {
            tiles.Remove(tile);
            RecentDiscard = tile;
            EventManager.FlagEvent("Hand " + PlayerNumber + " Discard");
        }

     

        //Queries the hand. If the Visible flag of the hand renderer is
        //set to false and the tile is not revealed, will return hidden.
        //Players can read their own hands by using their access key.
        //Note in this case the tile is not a copy.
        public Tile Query(int index, string accessKey = "")
        {
            if (index < 0 || index >= tiles.Count)
                return Tile.Invalid;
            else if (accessKey != "" && Common.SecurityManager.AccessKeyHash(accessKey) == accessHash)
                return tiles[index];
            else if (tiles[index].Revealed || handRenderer.Visible)
                return tiles[index].Copy();
            else
                return Tile.Hidden;
        }

        //Like Query, but returns the draw if visible or with access key
        public Tile GetDraw(string accessKey = "")
        {
            if (accessKey != "" && Common.SecurityManager.AccessKeyHash(accessKey) == accessHash)
                return draw;
            else if (handRenderer.Visible)
                return draw.Copy();
            else
                return Tile.Hidden;
        }

        //Sets the access key and hand name and clears the hand
        public void SetOwner(int playerNumber, string accessKey)
        {
            //if (!(initialized)) Initialize();
            PlayerNumber = playerNumber;
            handRenderer.PlayerNumber = playerNumber;
            accessHash = Common.SecurityManager.AccessKeyHash(accessKey);
            tiles.Clear();
        }
    }
}
