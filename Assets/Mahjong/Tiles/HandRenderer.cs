using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mahjong
{
    public class HandRenderer : MonoBehaviour
    {
        public TileOrientation Orientation;
        public bool Visible;
        #pragma warning disable CS0649
        public GameObject HandArea;
        public GameObject DrawArea;
        public GameObject OpenMeldArea;
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
                    EventManager.Unsubscribe("Hand " + _playerNumber + " Open Meld", OnOpenMeld);
                }
                _playerNumber = value;
                EventManager.Subscribe("Hand " + _playerNumber + " Discard", OnDiscard);
                EventManager.Subscribe("Hand " + _playerNumber + " Draw", OnTileDraw);
                EventManager.Subscribe("Hand " + _playerNumber + " Open Meld", OnOpenMeld);
            }
        }
        
        private void Start()
        {
            hand = gameObject.GetComponent<Hand>();
            EventManager.Subscribe("Hands Dealt", OnDeal);
        }

        //Sets up a tile with the right orientation for the hand. Called by Hand after drawing/dealing.
        public void UpdateOrientation(Tile tile)
        {
            tile.Renderer.Orientation = Orientation;
            //The following Visible line only works if the Tile's access key is the public read key
            if (Visible) tile.SetVisibility(TileVisibility.FaceUp);
            else tile.SetVisibility(TileVisibility.InHand);
        }

        //Arranges the hand tiles in correct position. Will arrange a draw in the hand, 
        //so this is only to be called after deal and discard. 
        private void UpdateHandArrangement()
        {
            int i = 0;
            int offset = -6;
            while (i < hand.Tiles.Count)
            {
                if (!(hand.Tiles[i].InOpenMeld))
                {
                    hand.Tiles[i].GetComponent<TileRenderer>().Position = FormOffset(
                        HandArea.transform.position.x, 
                        HandArea.transform.position.y, 
                        offset, Orientation);
                    offset++;
                }
                i++;
            }
        }

        //Arranges the most recently declared open meld and moves the draw area to accomodate
        private void UpdateLatestOpenMeldArrangement()
        {
            OpenMeld latest = hand.OpenMelds[hand.OpenMelds.Count - 1];
            float position = GetTotalOpenMeldWidth() - (0.5f * latest.Width);
            GameObject container = new GameObject();
            OpenMeld.ArrangeOpenMeldGroup(ref container, latest);
            switch (Orientation)
            {
                case TileOrientation.Bottom:
                    container.transform.SetPositionAndRotation(
                        (new Vector3(-position, 0f) + OpenMeldArea.transform.position), 
                        Quaternion.AngleAxis(0f, Vector3.forward));
                    DrawArea.transform.Translate(new Vector3(-latest.Width * Constants.DRAW_POSITION_ADJUST_MULT, 0f));
                    break;
                case TileOrientation.Right:
                    container.transform.SetPositionAndRotation(
                        (new Vector3(0f, -position) + OpenMeldArea.transform.position),
                        Quaternion.AngleAxis(90f, Vector3.forward));
                    DrawArea.transform.Translate(new Vector3(-latest.Width * Constants.DRAW_POSITION_ADJUST_MULT, 0f));
                    break;
                case TileOrientation.Top:
                    container.transform.SetPositionAndRotation(
                        (new Vector3(position, 0f) + OpenMeldArea.transform.position),
                        Quaternion.AngleAxis(180f, Vector3.forward));
                    DrawArea.transform.Translate(new Vector3(-latest.Width * Constants.DRAW_POSITION_ADJUST_MULT, 0f));
                    break;
                case TileOrientation.Left:
                    container.transform.SetPositionAndRotation(
                        (new Vector3(0f, position) + OpenMeldArea.transform.position),
                        Quaternion.AngleAxis(270f, Vector3.forward));
                    DrawArea.transform.Translate(new Vector3(-latest.Width * Constants.DRAW_POSITION_ADJUST_MULT, 0f));
                    break;
                case TileOrientation.Player:
                    //TODO: what to do about Player orientation?
                    break;
            }
        }

        //Returns the total width of all the open melds
        private float GetTotalOpenMeldWidth()
        {
            float width = 0;
            for (int i = 0; i < hand.OpenMelds.Count; i++)
                width += hand.OpenMelds[i].Width;
            return width;
        }

        //Arranges the drawn tile in the correct position.
        private void UpdateDrawArrangement()
        {
            
            hand.Draw.Renderer.Position
                = new Vector2(DrawArea.transform.position.x, DrawArea.transform.position.y);
        }

        //Helper function for UpdateHandArrangement
        private Vector2 FormOffset(float x, float y, int numTileOffset, TileOrientation dir)
        {
            Vector2 v = new Vector2(x, y);
            switch (dir)
            {
                case TileOrientation.Bottom:
                    v.x = v.x + numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileOrientation.Right:
                    v.y += numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileOrientation.Top:
                    v.x -= numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileOrientation.Left:
                    v.y -= numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileOrientation.Player:
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

        //Occurs when an open meld is formed
        private void OnOpenMeld()
        {
            UpdateLatestOpenMeldArrangement();
            UpdateHandArrangement();
        }
    }
}
