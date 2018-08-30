using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    //This class represents the pond of one player
    public class Kawa : MonoBehaviour
    {
        private List<GameObject> _tiles = new List<GameObject>();
        private List<TileID> _tileIDs = new List<TileID>();
        public List<GameObject> Tiles
        {
            get
            {
                return _tiles;
            }
        }
        public List<TileID> TileIDs
        {
            get
            {
                return _tileIDs;
            }
        }

        public TileOrientation Orientation;
        public GameObject Area;

        private int numberStolen = 0;
        

        //Adds a discarded tile to the pond
        public void Add(GameObject tileObj)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            TileRenderer tr = tileObj.GetComponent<TileRenderer>();
            //Put in pond
            tr.Position = GetNextPosition();
            tr.Orientation = Orientation;
            tile.SetVisibility(TileVisibility.FaceUp);
            //Add to lists
            _tiles.Add(tileObj);
            _tileIDs.Add(tile.Query());
        }

        //Increases steal count and returns a reference to the last discarded tile
        public GameObject Steal()
        {
            if (_tiles.Count - numberStolen == 0) return null;
            numberStolen++;
            return _tiles[_tiles.Count - 1];
        }

        //Gets the next position to place a tile in the pond
        private Vector2 GetNextPosition()
        {
            int n = _tiles.Count - numberStolen;
            int row, col; //-1 for first row to 1 for third row. 0 to 6 left to right for col
            if (n < 6)
            {
                row = -1;
                col = n;
            }
            else if (n < 12)
            {
                row = 0;
                col = n - 6;
            }
            else
            {
                row = 1;
                col = n - 12;
            }
            Vector2 offset = FormOffset(row, col);
            return new Vector2(Area.transform.position.x + offset.x, Area.transform.position.y + offset.y);
        }
        private Vector2 FormOffset(int row, int col)
        {
            Vector2 v = new Vector2();
            switch (Orientation)
            {
                case TileOrientation.Bottom:
                    v.x = Constants.ADJ_TILE_SPACING * (-2.5f + col);
                    v.y = -Constants.TILE_LENGTH * row;
                    break;
                case TileOrientation.Right:
                    v.x = Constants.TILE_LENGTH * row;
                    v.y = Constants.ADJ_TILE_SPACING * (-2.5f + col);
                    break;
                case TileOrientation.Top:
                    v.x = Constants.ADJ_TILE_SPACING * (2.5f - col);
                    v.y = Constants.TILE_LENGTH * row;
                    break;
                case TileOrientation.Left:
                    v.x = -Constants.TILE_LENGTH * row;
                    v.y = -Constants.ADJ_TILE_SPACING * (-2.5f + col);
                    break;
                case TileOrientation.Player:
                    Debug.LogError("Kawa orientation of 'Player' is invalid.");
                    break;
            }
            return v;
        }
    }
}
