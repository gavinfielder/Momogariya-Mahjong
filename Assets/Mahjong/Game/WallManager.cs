using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    public class WallManager : MonoBehaviour, ISecured
    {

        private int _accessKey;
        private UnsortedTileCollection _tiles = new UnsortedTileCollection();
        public UnsortedTileCollection Tiles
        {
            get
            {
                return _tiles;
            }
            private set
            {
                _tiles = Tiles;
            }
        }

        private int kanReplacementDraws = 0;
        private int numberOfRegularDoras = 0;

        public int NumberOfDeadTiles {
            get
            {
                return 14 - kanReplacementDraws; 
            }
        }
        public int NumberOfHiddenDeadTiles {
            get
            {
                return 14 - kanReplacementDraws - numberOfRegularDoras;
            }
        }
        public int NumberDrawsRemaining
        {
            get
            {
                return Tiles.Count - 14 - kanReplacementDraws;
            }
        }

        private List<TileID> doras = new List<TileID>();
        public List<TileID> Doras
        {
            get
            {
                return doras;
            }
        }


        //***********************************************************************
        //******************************** Setup ********************************
        //***********************************************************************

        //Builds the wall for a new game
        public void Build(UnsortedTileCollection tiles, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;

            //Reset status conditions
            doras.Clear();
            kanReplacementDraws = 0;
            numberOfRegularDoras = 0;

            //Reset tiles and wall
            Tiles = tiles;
            Tiles.SetOwner(_accessKey);
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tiles[i].SetOwner(_accessKey);
                Tiles[i].SetVisibility(TileVisibility.FaceDown, _accessKey);
            }

            //Shuffle
            ShuffleRange(0, Tiles.Count - 1);

            //Arrange wall
            ArrangeWall();

            //Done
            EventManager.FlagEvent("Wall Built");
        }

        //Builds a wall while forcing the first N draws and dora/kandora. For testing only.
        public void Build_ForceOrder(UnsortedTileCollection tiles, List<TileID> firstDraws, List<TileID> firstDoras, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;

            //Reset status conditions
            doras.Clear();
            kanReplacementDraws = 0;
            numberOfRegularDoras = 0;

            //Reset tiles and wall
            Tiles = tiles;
            Tiles.SetOwner(_accessKey);
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tiles[i].SetOwner(_accessKey);
                Tiles[i].SetVisibility(TileVisibility.FaceDown, _accessKey);
            }

            //Set first draws
            for (int i = 0; i < firstDraws.Count; i++)
            {
                ForceTileAt(firstDraws[i], i, i + 1, Tiles.Count - 1);
            }
            //Set first doras
            for (int i = 0; i < firstDoras.Count; i++)
            {
                ForceTileAt(firstDoras[i], Tiles.Count - 1 - i, firstDraws.Count, Tiles.Count - 1 - i - 1);
            }

            //Shuffle
            ShuffleRange(firstDraws.Count, Tiles.Count - 1 - firstDoras.Count);
            //Done
            EventManager.FlagEvent("Wall Built");
        }

        //Breaks the wall and reveals the first dora
        public void Break(int wallNumber, int accessKey = 0)
        {
            if ((accessKey != _accessKey) && _accessKey != 0)
                return;

            //Align draw position (zero-index) accordingly
            int[] rotateBy = { 6, 40, 74, 108 };
            RotateWallTiles(rotateBy[wallNumber - 1]);
            //Render the wall as broken
            ArrangeBreakWall(wallNumber);
            //Raise event when done breaking wall
            EventManager.FlagEvent("Wall Broken");
            //Reveal the first dora
            NewDora();
        }

        //***********************************************************************
        //******************************* Gameplay ******************************
        //***********************************************************************


        //Draws a single tile from the wall
        public Tile Draw()
        {
            if (NumberDrawsRemaining == 0) return null;
            Tile tile = Tiles[0];
            Tiles.RemoveAt(0);
            tile.ReleaseOwnership(_accessKey);
            return tile;
        }

        //Draws a kan replacement tile from the dead wall
        public TileID DrawKanReplacement()
        {
            //kanReplacementDraws++;
            return TileID.Invalid;
        }

        //Flips a new dora. 
        public TileID NewDora()
        {
            Tile indicator = Tiles[Tiles.Count - numberOfRegularDoras * 2 - 2];
            indicator.ReleaseOwnership(_accessKey);
            indicator.SetVisibility(TileVisibility.FaceUp);
            doras.Add(indicator.Query().GetDoraFromIndicator());
            numberOfRegularDoras++;
            EventManager.FlagEvent("New Dora");
            return doras[doras.Count - 1];
        }

        //Returns the number of ura dora
        public int GetNumberOfUradora(RuleSet rules)
        {
            int r = 0;
            if (rules.Uradora == true) r = 1;
            if (rules.Kanuradora == true) r = doras.Count;
            return r;
        }

        //***********************************************************************
        //******************************* Shuffling *****************************
        //***********************************************************************

        //Shuffles the tile at the specified index into a random location over a range
        private void ShuffleTile(int index, int low, int high)
        {
            Tile placeholder = new Tile();
            placeholder.SetOwner(_accessKey);
            placeholder.Set(TileID.Suits.Kaze, 0);

            Tile shuffling = Tiles[index];
            //Use placeholder to not mess up the range
            Tiles[index] = placeholder;
            System.Random rnd = new System.Random();
            int to = rnd.Next(low, high);
            Tiles.Insert(to, shuffling, _accessKey);
            Tiles.Remove(placeholder, _accessKey);
        }
        
        //Shuffles all the tiles over the specified range
        private void ShuffleRange(int low, int high)
        {
            System.Random rnd = new System.Random();
            int i, j, n;
            //Shuffle each tile sequentially
            for (n = low; n <= high; n++)
            {
                j = rnd.Next(low, high + 1);
                Swap2(n, j);
            }
            int shuffles = (high - low + 1) * 37;
            //Do random shuffles
            for (n = 0; n < shuffles; n++)
            {
                i = rnd.Next(low, high + 1);
                j = rnd.Next(low, high + 1);
                Swap2(i, j);
            }
        }

        //Performs an in-place swap of two tiles
        private void Swap2(int i, int j)
        {
            if (i == j) return;

            Tile placeholder = new Tile();
            placeholder.SetOwner(_accessKey);
            placeholder.Set(TileID.Suits.Kaze, 0);

            Tile wasi = Tiles[i];
            Tile wasj = Tiles[j];

            Tiles[j] = placeholder;
            Tiles[i] = wasj;
            Tiles[j] = wasi;
        }

        //Forces a tile at the specified index, shuffling into the given range until achieved
        private void ForceTileAt(TileID id, int index, int low, int high)
        {
            while (Tiles[index].Query(_accessKey) != id)
            {
                ShuffleTile(index, low, high);
            }
        }

        //***********************************************************************
        //****************************** Arrangement ****************************
        //***********************************************************************

        //Puts all the tiles in their proper spot.
        private void ArrangeWall()
        {
            /*
            Wall is built in general order of draw. When the wall is broken,
            the list will be rotated so that the zero index always points
            to the next normal wall draw. Following this function, the zero
            index will point to the top tile on the bottom wall's right end.

            If player 1 wall (bottom) is broken, it will need to rotate by 6.
            If player 4 wall (left) is broken, it will need to rotate by 6+34=40.
            If player 3 wall (top) is broken, it will need to rotate by 74.
            If player 2 wall (right) is broken, it will need to rotate by 108.

            This is implemented in the next function, BreakWall()
            */
            
            GameObject area;
            TileRenderer tr;
            float x, y;
            int j = 0; //current tile in collection

            //Build player 1 wall (bottom of screen, right to left)
            area = GameObject.Find("Wall 1 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int i = -8; i <= 8; i++)
            {
                //tile on top
                tr = Tiles[j].Renderer;
                tr.Layer = 2;
                tr.Position = FormWallOffset(x, y + Constants.TILE_HEIGHT, i, TileOrientation.Bottom);
                tr.Orientation = TileOrientation.Bottom;
                j++;

                //tile on bottom
                tr = Tiles[j].Renderer;
                tr.Layer = 1;
                tr.Position = FormWallOffset(x, y, i, TileOrientation.Bottom);
                tr.Orientation = TileOrientation.Bottom;
                j++;
            }

            //Build player 4 wall (left of screen, bottom to top
            area = GameObject.Find("Wall 4 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int i = -8; i <= 8; i++)
            {
                //tile on top
                tr = Tiles[j].Renderer;
                tr.Layer = 2;
                tr.Position = FormWallOffset(x, y + Constants.TILE_HEIGHT, i, TileOrientation.Left);
                tr.Orientation = TileOrientation.Left;
                j++;

                //tile on bottom
                tr = Tiles[j].Renderer;
                tr.Layer = 1;
                tr.Position = FormWallOffset(x, y, i, TileOrientation.Left);
                tr.Orientation = TileOrientation.Left;
                j++;
            }

            //Build player 3 wall (top of screen, left to right)
            area = GameObject.Find("Wall 3 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int i = -8; i <= 8; i++)
            {
                //tile on top
                tr = Tiles[j].Renderer;
                tr.Layer = 2;
                tr.Position = FormWallOffset(x, y + Constants.TILE_HEIGHT, i, TileOrientation.Top);
                tr.Orientation = TileOrientation.Top;
                j++;

                //tile on bottom
                tr = Tiles[j].Renderer;
                tr.Layer = 1;
                tr.Position = FormWallOffset(x, y, i, TileOrientation.Top);
                tr.Orientation = TileOrientation.Top;
                j++;
            }

            //Build player 2 wall (right of screen, top to bottom)
            area = GameObject.Find("Wall 2 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int i = -8; i <= 8; i++)
            {
                //tile on top
                tr = Tiles[j].Renderer;
                tr.Layer = 2;
                tr.Position = FormWallOffset(x, y + Constants.TILE_HEIGHT, i, TileOrientation.Right);
                tr.Orientation = TileOrientation.Right;
                j++;

                //tile on bottom
                tr = Tiles[j].Renderer;
                tr.Layer = 1;
                tr.Position = FormWallOffset(x, y, i, TileOrientation.Right);
                tr.Orientation = TileOrientation.Right;
                j++;
            }

        }

        //Helper function for ArrangeWall
        private Vector2 FormWallOffset(float x, float y, int numTileOffset, TileOrientation dir)
        {
            Vector2 v = new Vector2(x, y);
            switch (dir)
            {
                case TileOrientation.Bottom:
                    v.x -= numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileOrientation.Right:
                    v.y -= numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileOrientation.Top:
                    v.x += numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileOrientation.Left:
                    v.y += numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileOrientation.Player:
                    break;
            }
            return v;
        }

        //Arranges the wall broken
        private void ArrangeBreakWall(int wallNumber)
        {
            GameObject area;
            TileRenderer tr;
            float x, y;

            //Move last 6 of dead wall to the center
            int i = Tiles.Count - 1;
            area = GameObject.Find("Dora Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int j = 1; j >= -1; j--)
            {
                //Bottom layer
                tr = Tiles[i].Renderer;
                tr.Position = new Vector2(x + j * Constants.ADJ_TILE_SPACING, y);
                tr.Orientation = TileOrientation.Bottom;
                i--;

                //Top Layer
                tr = Tiles[i].Renderer;
                tr.Position = new Vector2(x + j * Constants.ADJ_TILE_SPACING, y + Constants.TILE_HEIGHT);
                tr.Orientation = TileOrientation.Bottom;
                i--;
            }

            //Move the other 8 dead wall tiles to the upper right corner
            area = GameObject.Find("Dead Wall 2 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int j = 1; j >= -2; j--)
            {
                //Bottom layer
                tr = Tiles[i].Renderer;
                tr.Position = new Vector2(x + j * Constants.ADJ_TILE_SPACING, y);
                tr.Orientation = TileOrientation.Bottom;
                i--;

                //Top layer
                tr = Tiles[i].Renderer;
                tr.Position = new Vector2(x + j * Constants.ADJ_TILE_SPACING, y + Constants.TILE_HEIGHT);
                tr.Orientation = TileOrientation.Bottom;
                i--;
            }
        }
        
        //Rotates the collection with items in front moving to the back, n times
        //Helper function used to align the current draw with the front of the list
        private void RotateWallTiles(int n)
        {
            Tile temp;
            for (int i = 0; i < n; i++)
            {
                temp = Tiles[0];
                Tiles.RemoveAt(0);
                Tiles.Add(temp);
            }
        }


        //***********************************************************************
        //************************** Access Management **************************
        //***********************************************************************

        //Sets the access key. Only works if no owner (current key is zero)
        public void SetOwner(int newAccessKey)
        {
            if (_accessKey == 0) {
                _accessKey = newAccessKey;
                Tiles.SetOwner(_accessKey);
            }
            else
            {
                Tiles.ReleaseOwnership(_accessKey);
                _accessKey = newAccessKey;
                Tiles.SetOwner(_accessKey);
            }
        }

        //Resets the access key to no owner (0)
        public void ReleaseOwnership(int accessKey)
        {
            if (accessKey == _accessKey)
            {
                _accessKey = 0;
                Tiles.ReleaseOwnership(_accessKey);
            }
        }
        
    }
}
