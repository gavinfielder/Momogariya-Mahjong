using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    //This class represents the pond of one player
    public class Kawa : MonoBehaviour
    {
        private UnsortedTileCollection _tiles = new UnsortedTileCollection();
        public UnsortedTileCollection Tiles
        {
            get
            {
                return _tiles;
            }
        }

        public static Tile MostRecentDiscard { get; set; }
        public static Kawa MostRecentKawa { get; set; }

        public TileOrientation Orientation;
        public GameObject Area;
        public int PlayerNumber;

        private int numberStolen = 0;
        

        //Adds a discarded tile to the pond
        public void Add(Tile tile)
        {
            Tiles.Add(tile);
            //Arrange in pond
            tile.Renderer.Position = GetNextPosition();
            tile.Renderer.Orientation = Orientation;
            tile.SetVisibility(TileVisibility.FaceUp);
            //Set static information accessors
            MostRecentDiscard = tile;
            MostRecentKawa = this;
        }

        //Increases steal count and returns a reference to the last discarded tile
        public Tile Steal()
        {
            if (Tiles.Count - numberStolen == 0) return null;
            numberStolen++;
            Tiles[Tiles.Count - 1].StolenDiscard = true;
            return Tiles[Tiles.Count - 1];
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
