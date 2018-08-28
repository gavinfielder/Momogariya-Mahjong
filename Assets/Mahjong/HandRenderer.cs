using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mahjong
{
    public class HandRenderer : MonoBehaviour
    {
        public TileRenderer.TileOrientation Orientation;
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
                    EventManager.Unsubscribe("Hand " + _playerNumber + " Draw", OnTileDraw);
                }
                _playerNumber = value;
                EventManager.Subscribe("Hand " + _playerNumber + " Discard", OnDiscard);
                EventManager.Subscribe("Hand " + _playerNumber + " Draw", OnTileDraw);
            }
        }

        private List<GameObject> tiles;
        private List<GameObject> drawContainer;

        private void Start()
        {
            hand = gameObject.GetComponent<Hand>();
            EventManager.Subscribe("Hands Dealt", OnDeal);
            /*
            //When the wall is built, render the wall
            EventManager.Subscribe("Wall Built", RenderWall);
            EventManager.Subscribe("Wall Broken", BreakWall);
            EventManager.Subscribe("New Dora", RenderDoraIndicators);
            */
        }

        //Sets references to the tile collections in the Hand
        public void SyncWithHand(ref List<GameObject> handTiles, ref List<GameObject> drawContainerRef)
        {
            tiles = handTiles;
            drawContainer = drawContainerRef;
        }
        

        //Occurs when a tile is drawn into the hand
        private void OnDraw()
        {
            /*
            float x = DrawArea.transform.position.x;
            float y = DrawArea.transform.position.y;
            TileID face = hand.GetDraw();
            if (face == TileID.Hidden) face = TileID.HiddenHand;
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
            */
        }

        //Arranges the hand tiles in correct position. 
        //Will arrange a draw in the hand, so this is only to be called after deal and discard.
        private void UpdateHandArrangement()
        {
            for (int i = -6; i+6 < hand.Count; i++)
            {
                tiles[i+6].GetComponent<TileRenderer>().Position = FormOffset(
                    HandArea.transform.position.x, HandArea.transform.position.y, i, Orientation);
            }
            
        }

        //Arranges the drawn tile in the correct position.
        private void UpdateDrawArrangement()
        {
            drawContainer[0].GetComponent<TileRenderer>().Position
                = new Vector2(DrawArea.transform.position.x, DrawArea.transform.position.y);
        }

        //Helper function for UpdateHandArrangement
        private Vector2 FormOffset(float x, float y, int numTileOffset, TileRenderer.TileOrientation dir)
        {
            Vector2 v = new Vector2(x, y);
            switch (Orientation)
            {
                case TileRenderer.TileOrientation.Bottom:
                    v.x = v.x + numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileRenderer.TileOrientation.Right:
                    v.y += numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileRenderer.TileOrientation.Top:
                    v.x -= numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileRenderer.TileOrientation.Left:
                    v.y -= numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileRenderer.TileOrientation.Player:
                    break;
            }
            return v;
        }


        //Occurs when a tile is discarded
        private void OnDiscard()
        {
            UpdateHandArrangement();
        }

        //Occurs when the controller finishes dealing the initial 13 tiles
        private void OnDeal()
        {
            UpdateHandArrangement();
        }

        //Occurs when a tile is drawn. Arranges the tile in the correct position.
        private void OnTileDraw()
        {
            UpdateDrawArrangement();
        }

        //Sets up a tile with the right orientation for the hand. Called by Hand after drawing/dealing.
        public void UpdateOrientation(GameObject tile)
        {
            tile.GetComponent<TileRenderer>().Orientation = Orientation;
            //The following Visible line only works if the Tile's access key is the public read key
            if (Visible) tile.GetComponent<Tile>().SetVisibility(TileVisibility.FaceUp);
            else tile.GetComponent<Tile>().SetVisibility(TileVisibility.InHand);
        }


    }
}
