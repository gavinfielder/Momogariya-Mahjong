using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mahjong
{
    public class HandRenderer : MonoBehaviour
    {
        //TODO: this is defined in multiple places. Move to a single location.
        const float ADJ_TILE_SPACING = 0.41f;

        //private List<Tile> tiles = new List<Tile>();
        public enum HandRendererType
        {
            Bottom,
            Player,
            Right,
            Top,
            Left
        }
        public HandRendererType Type;
        public bool Visible;
        #pragma warning disable CS0649
        public GameObject HandArea;
        public GameObject DrawArea;
        public GameObject TileBase;
        #pragma warning restore CS0649
        private Hand hand;
        public HandSortingMethod SortingMethod { private get; set; }
        private int _playerNumber = -1;
        public int PlayerNumber {
            get
            {
                return _playerNumber;
            }
            set
            {
                if (_playerNumber != -1)
                {
                    EventManager.Unsubscribe("Hand " + _playerNumber + " Discard", OnDiscard);
                    EventManager.Unsubscribe("Hand " + _playerNumber + " Draw", OnDraw);
                }
                _playerNumber = value;
                EventManager.Subscribe("Hand " + _playerNumber + " Discard", OnDiscard);
                EventManager.Subscribe("Hand " + _playerNumber + " Draw", OnDraw);
            }
        }
        private bool initialized = false;

        private List<GameObject> tiles = new List<GameObject>();
        private GameObject draw;

        private void Start()
        {
            Initialize();
            EventManager.Subscribe("Hands Dealt", OnDeal);
            /*
            //When the wall is built, render the wall
            EventManager.Subscribe("Wall Built", RenderWall);
            EventManager.Subscribe("Wall Broken", BreakWall);
            EventManager.Subscribe("New Dora", RenderDoraIndicators);
            */
        }

        private void Initialize()
        {
            if (initialized) return;
            hand = gameObject.GetComponent<Hand>();
        }
        

        //Occurs when a tile is drawn into the hand
        private void OnDraw()
        {
            float x = DrawArea.transform.position.x;
            float y = DrawArea.transform.position.y;
            Tile face = hand.GetDraw();
            if (face == Tile.Hidden) face = Tile.HiddenHand;
            draw = Instantiate(TileBase);
            draw.GetComponent<TileRenderer>().Face = face;

            if (Type == HandRendererType.Bottom)
                draw.transform.SetPositionAndRotation(
                    new Vector3(x, y),
                    Quaternion.AngleAxis(0, Vector3.forward));
            else if (Type == HandRendererType.Right)
                draw.transform.SetPositionAndRotation(
                    new Vector3(x, y),
                    Quaternion.AngleAxis(90, Vector3.forward));
            else if (Type == HandRendererType.Top)
                draw.transform.SetPositionAndRotation(
                    new Vector3(x, y),
                    Quaternion.AngleAxis(0, Vector3.forward));
            else if (Type == HandRendererType.Left)
                draw.transform.SetPositionAndRotation(
                    new Vector3(x, y),
                    Quaternion.AngleAxis(270, Vector3.forward));
            else if (Type == HandRendererType.Player)
                draw.transform.SetPositionAndRotation(
                    new Vector3(x, y),
                    Quaternion.AngleAxis(0, Vector3.forward));
            
            draw.SetActive(true);
        }

        //Deactivates and clears the tile GameObjects
        private void ClearHand()
        {
            while (tiles.Count > 0)
            {
                tiles[0].SetActive(false);
                tiles.RemoveAt(0);
            }

        }

        //Renders the 13 tiles of the hand.
        private void RenderHand()
        {
            ClearHand();
            float x = HandArea.transform.position.x;
            float y = HandArea.transform.position.y;

            int i;
            TileRenderer tr;
            Tile face;
            if (Type == HandRendererType.Bottom)
            {
                for (i = -6; i < 6; i++)
                {
                    tiles.Add(Instantiate(TileBase));
                    tr = tiles[tiles.Count - 1].GetComponent<TileRenderer>();
                    face = hand.Query(i + 6);
                    if (face == Tile.Hidden) face = Tile.HiddenHand;
                    tr.Face = face;

                    tiles[tiles.Count - 1].transform.SetPositionAndRotation(
                        new Vector3(x + i * ADJ_TILE_SPACING, y), 
                        Quaternion.AngleAxis(0f, Vector3.forward));
                    
                    tiles[tiles.Count - 1].SetActive(true);
                }
            }
            else if (Type == HandRendererType.Right)
            {
                for (i = -6; i < 6; i++)
                {
                    tiles.Add(Instantiate(TileBase));
                    tr = tiles[tiles.Count - 1].GetComponent<TileRenderer>();
                    face = hand.Query(i + 6);
                    if (face == Tile.Hidden) face = Tile.HiddenHand;
                    tr.Face = face;

                    tiles[tiles.Count - 1].transform.SetPositionAndRotation(
                        new Vector3(x, y + i * ADJ_TILE_SPACING),
                        Quaternion.AngleAxis(90f, Vector3.forward));

                    tiles[tiles.Count - 1].SetActive(true);
                }
            }
            else if (Type == HandRendererType.Top)
            {
                for (i = -6; i < 6; i++)
                {
                    tiles.Add(Instantiate(TileBase));
                    tr = tiles[tiles.Count - 1].GetComponent<TileRenderer>();
                    face = hand.Query(i + 6);
                    if (face == Tile.Hidden) face = Tile.HiddenHand;
                    tr.Face = face;

                    tiles[tiles.Count - 1].transform.SetPositionAndRotation(
                        new Vector3(x + i * ADJ_TILE_SPACING, y),
                        Quaternion.AngleAxis(0f, Vector3.forward));

                    tiles[tiles.Count - 1].SetActive(true);
                }
            }
            else if (Type == HandRendererType.Left)
            {
                for (i = -6; i < 6; i++)
                {
                    tiles.Add(Instantiate(TileBase));
                    tr = tiles[tiles.Count - 1].GetComponent<TileRenderer>();
                    face = hand.Query(i + 6);
                    if (face == Tile.Hidden) face = Tile.HiddenHand;
                    tr.Face = face;

                    tiles[tiles.Count - 1].transform.SetPositionAndRotation(
                        new Vector3(x, y - i * ADJ_TILE_SPACING),
                        Quaternion.AngleAxis(270f, Vector3.forward));

                    tiles[tiles.Count - 1].SetActive(true);
                }
            }
            else if (Type == HandRendererType.Player)
            {

            }


        }


        //Occurs when a tile is discarded
        private void OnDiscard()
        {
            //Remove the draw
            draw.SetActive(false);
            draw = null;
            //Check if we need to redraw the hand
            if (hand.GetDraw() != hand.RecentDiscard)
            {
                RenderHand();
            }
        }


        //Occurs when the hand is initially dealt
        private void OnDeal()
        {
            RenderHand();
        }


    }
}
