using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Mahjong
{
    public class WallManager : MonoBehaviour
    {

        private List<GameObject> wallTiles = new List<GameObject>();

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
                return wallTiles.Count - 14;
            }
        }

        private int accessKey = 0;

        private List<TileID> doras = new List<TileID>();
        public List<TileID> Doras
        {
            get
            {
                return doras;
            }
        }

        //Builds the wall for a new game
        public void Build(ref List<GameObject> allTiles)
        {
            //Initialize access key
            if (accessKey == 0) accessKey = Security.AccessKeyHash(
                Security.GetRandomAccessKey());

            //Reset status conditions
            doras.Clear();
            kanReplacementDraws = 0;
            numberOfRegularDoras = 0;

            //Reset tiles and wall
            Tile tile;
            wallTiles.Clear();
            for (int i = 0; i < allTiles.Count; i++)
            {
                tile = allTiles[i].GetComponent<Tile>();
                tile.SetOwner(accessKey);
                tile.SetVisibility(TileVisibility.FaceDown, accessKey);
                wallTiles.Add(allTiles[i]);
            }

            //Shuffle
            ShuffleRange(0, wallTiles.Count - 1);

            //Arrange wall
            ArrangeWall();

            //Done
            EventManager.FlagEvent("Wall Built");
        }

        //Builds a wall while forcing the first N draws and dora/kandora. For testing only.
        public void Build_ForceOrder(ref List<GameObject> allTiles, List<TileID> firstDraws, List<TileID> firstDoras)
        {
            EventManager.FlagEvent("Debug Function Invoked");
            Debug.Log("Forcing wall build order...");

            //Initialize access key
            if (accessKey == 0) accessKey = Security.AccessKeyHash(
                Security.GetRandomAccessKey());

            //Reset status conditions
            doras.Clear();
            kanReplacementDraws = 0;
            numberOfRegularDoras = 0;

            //Reset tiles and wall
            Tile tile;
            wallTiles.Clear();
            for (int i = 0; i < allTiles.Count; i++)
            {
                tile = allTiles[i].GetComponent<Tile>();
                tile.SetOwner(accessKey);
                tile.SetVisibility(TileVisibility.FaceDown, accessKey);
                wallTiles.Add(allTiles[i]);
            }
            
            //Set first draws
            for (int i = 0; i < firstDraws.Count; i++)
            {
                ForceTileAt(firstDraws[i], i, i + 1, wallTiles.Count - 1);
            }
            //Set first doras
            for (int i = 0; i < firstDoras.Count; i++)
            {
                ForceTileAt(firstDoras[i], wallTiles.Count - 1 - i, firstDraws.Count, wallTiles.Count - 1 - i - 1);
            }

            //Shuffle
            ShuffleRange(firstDraws.Count, wallTiles.Count - 1 - firstDoras.Count);
            //Done
            EventManager.FlagEvent("Wall Built");
        }

        //Breaks the wall and reveals the first dora
        public void Break(int wallNumber)
        {
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

        //Draws a single tile from the wall
        public GameObject Draw()
        {
            if (NumberDrawsRemaining == 0) return null;
            GameObject tile = wallTiles[0];
            wallTiles.RemoveAt(0);
            tile.GetComponent<Tile>().ReleaseOwnership(accessKey);
            //EventManager.FlagEvent("Normal Draw");
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
            Tile indicator = wallTiles[wallTiles.Count - numberOfRegularDoras * 2 - 2].GetComponent<Tile>();
            indicator.SetVisibility(TileVisibility.FaceUp, accessKey);
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

        //Shuffles the tile at the specified index into a random location over a range
        private void ShuffleTile(int index, int low, int high)
        {
            GameObject placeholder = new GameObject();
            GameObject shuffling = wallTiles[index];
            //Use placeholder to not mess up the range
            wallTiles[index] = placeholder;
            System.Random rnd = new System.Random();
            int to = rnd.Next(low, high);
            wallTiles.Insert(to, shuffling);
            wallTiles.Remove(placeholder);
        }
        
        //Shuffles all the tiles over the specified range
        private void ShuffleRange(int low, int high)
        {
            int shuffles = (high - low + 1) * 37;
            System.Random rnd = new System.Random();
            int i, j;
            for (int n = 0; n < shuffles; n++)
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
            GameObject temp = wallTiles[i];
            wallTiles[i] = wallTiles[j];
            wallTiles[j] = temp;
        }

        //Forces a tile at the specified index, shuffling into the given range until achieved
        private void ForceTileAt(TileID tile, int index, int low, int high)
        {
            while (wallTiles[index].GetComponent<Tile>().Query(accessKey) != tile)
            {
                ShuffleTile(index, low, high);
            }
        }

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
                tr = wallTiles[j].GetComponent<TileRenderer>();
                tr.Layer = 2;
                tr.Position = FormWallOffset(x, y + Constants.TILE_HEIGHT, i, TileRenderer.TileOrientation.Bottom);
                tr.Orientation = TileRenderer.TileOrientation.Bottom;
                j++;

                //tile on bottom
                tr = wallTiles[j].GetComponent<TileRenderer>();
                tr.Layer = 1;
                tr.Position = FormWallOffset(x, y, i, TileRenderer.TileOrientation.Bottom);
                tr.Orientation = TileRenderer.TileOrientation.Bottom;
                j++;
            }

            //Build player 4 wall (left of screen, bottom to top
            area = GameObject.Find("Wall 4 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int i = -8; i <= 8; i++)
            {
                //tile on top
                tr = wallTiles[j].GetComponent<TileRenderer>();
                tr.Layer = 2;
                tr.Position = FormWallOffset(x, y + Constants.TILE_HEIGHT, i, TileRenderer.TileOrientation.Left);
                tr.Orientation = TileRenderer.TileOrientation.Left;
                j++;

                //tile on bottom
                tr = wallTiles[j].GetComponent<TileRenderer>();
                tr.Layer = 1;
                tr.Position = FormWallOffset(x, y, i, TileRenderer.TileOrientation.Left);
                tr.Orientation = TileRenderer.TileOrientation.Left;
                j++;
            }

            //Build player 3 wall (top of screen, left to right)
            area = GameObject.Find("Wall 3 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int i = -8; i <= 8; i++)
            {
                //tile on top
                tr = wallTiles[j].GetComponent<TileRenderer>();
                tr.Layer = 2;
                tr.Position = FormWallOffset(x, y + Constants.TILE_HEIGHT, i, TileRenderer.TileOrientation.Top);
                tr.Orientation = TileRenderer.TileOrientation.Top;
                j++;

                //tile on bottom
                tr = wallTiles[j].GetComponent<TileRenderer>();
                tr.Layer = 1;
                tr.Position = FormWallOffset(x, y, i, TileRenderer.TileOrientation.Top);
                tr.Orientation = TileRenderer.TileOrientation.Top;
                j++;
            }

            //Build player 2 wall (right of screen, top to bottom)
            area = GameObject.Find("Wall 2 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int i = -8; i <= 8; i++)
            {
                //tile on top
                tr = wallTiles[j].GetComponent<TileRenderer>();
                tr.Layer = 2;
                tr.Position = FormWallOffset(x, y + Constants.TILE_HEIGHT, i, TileRenderer.TileOrientation.Right);
                tr.Orientation = TileRenderer.TileOrientation.Right;
                j++;

                //tile on bottom
                tr = wallTiles[j].GetComponent<TileRenderer>();
                tr.Layer = 1;
                tr.Position = FormWallOffset(x, y, i, TileRenderer.TileOrientation.Right);
                tr.Orientation = TileRenderer.TileOrientation.Right;
                j++;
            }

        }

        //Helper function for ArrangeWall
        private Vector2 FormWallOffset(float x, float y, int numTileOffset, TileRenderer.TileOrientation dir)
        {
            Vector2 v = new Vector2(x, y);
            switch (dir)
            {
                case TileRenderer.TileOrientation.Bottom:
                    v.x -= numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileRenderer.TileOrientation.Right:
                    v.y -= numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileRenderer.TileOrientation.Top:
                    v.x += numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileRenderer.TileOrientation.Left:
                    v.y += numTileOffset * Constants.ADJ_TILE_SPACING;
                    break;
                case TileRenderer.TileOrientation.Player:
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
            int i = wallTiles.Count - 1;
            area = GameObject.Find("Dora Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int j = 1; j >= -1; j--)
            {
                //Bottom layer
                tr = wallTiles[i].GetComponent<TileRenderer>();
                tr.Position = new Vector2(x + j * Constants.ADJ_TILE_SPACING, y);
                tr.Orientation = TileRenderer.TileOrientation.Bottom;
                i--;

                //Top Layer
                tr = wallTiles[i].GetComponent<TileRenderer>();
                tr.Position = new Vector2(x + j * Constants.ADJ_TILE_SPACING, y + Constants.TILE_HEIGHT);
                tr.Orientation = TileRenderer.TileOrientation.Bottom;
                i--;
            }

            //Move the other 8 dead wall tiles to the upper right corner
            area = GameObject.Find("Dead Wall 2 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int j = 1; j >= -2; j--)
            {
                //Bottom layer
                tr = wallTiles[i].GetComponent<TileRenderer>();
                tr.Position = new Vector2(x + j * Constants.ADJ_TILE_SPACING, y);
                tr.Orientation = TileRenderer.TileOrientation.Bottom;
                i--;

                //Top layer
                tr = wallTiles[i].GetComponent<TileRenderer>();
                tr.Position = new Vector2(x + j * Constants.ADJ_TILE_SPACING, y + Constants.TILE_HEIGHT);
                tr.Orientation = TileRenderer.TileOrientation.Bottom;
                i--;
            }
        }
        
        //Rotates the collection with items in front moving to the back, n times
        //Helper function used to align the current draw with the front of the list
        private void RotateWallTiles(int n)
        {
            GameObject temp;
            for (int i = 0; i < n; i++)
            {
                temp = wallTiles[0];
                wallTiles.RemoveAt(0);
                wallTiles.Add(temp);
            }
        }

        //Clears all the rendered tiles of the wall
        private void ClearWall()
        {
            for (int i = 0; i < wallTiles.Count; i++)
            {
                wallTiles[i].SetActive(false);
                wallTiles[i] = null;
            }
            wallTiles.Clear();
        }

        //Occurs when a tile is drawn from the wall
        private void OnNormalDraw()
        {
            wallTiles[0].SetActive(false);
            wallTiles.RemoveAt(0);
        }






    }
}
