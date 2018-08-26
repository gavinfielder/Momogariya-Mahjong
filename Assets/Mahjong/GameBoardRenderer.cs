using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mahjong
{
    class GameBoardRenderer : MonoBehaviour
    {


        //Constants
        const float ADJ_TILE_SPACING = 0.41f;
        const float WALL_DISTANCE_FROM_CENTER = 3.8f;
        const float DEAD_WALL_DISTANCE = 4.5f;
        const float TILE_HEIGHT = 0.12f;

        //Graphics objects
        private List<GameObject> wallTiles = new List<GameObject>(); 

        //Object references
        #pragma warning disable CS0649
        public GameObject TileBase;
        #pragma warning restore CS0649
        private GameBoard Board;

        //Initialization
        private void Start()
        {
            //Retrieve a reference to this object's GameBoard instance
            Board = gameObject.GetComponent<GameBoard>();
            //When the wall is built, render the wall
            EventManager.Subscribe("Wall Built", RenderWall);
            EventManager.Subscribe("Wall Broken", BreakWall);
            EventManager.Subscribe("New Dora", RenderDoraIndicators);
            EventManager.Subscribe("Normal Draw", OnNormalDraw);
        }

        //Renders the wall
        private void RenderWall()
        {
            Debug.Log("Rendering wall...");
            ClearWall();

            float offset = 0f;
            TileRenderer tr;

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
            float x, y;

            //Build player 1 wall (bottom of screen, right to left)
            for (int i = 8; i >= -8; i--)
            {
                offset = i * ADJ_TILE_SPACING;

                area = GameObject.Find("Wall 1 Area");
                x = area.transform.position.x;
                y = area.transform.position.y;

                //tile on top
                wallTiles.Add(Instantiate(TileBase));
                tr = wallTiles[wallTiles.Count - 1].GetComponent<TileRenderer>();
                tr.Face = Tile.Hidden;
                tr.Layer = 2;
                wallTiles[wallTiles.Count - 1].transform.SetPositionAndRotation
                    (new Vector3(x + offset, y + TILE_HEIGHT),
                    Quaternion.identity);
                wallTiles[wallTiles.Count - 1].SetActive(true);

                //tile on bottom
                wallTiles.Add(Instantiate(TileBase));
                tr = wallTiles[wallTiles.Count - 1].GetComponent<TileRenderer>();
                tr.Face = Tile.Hidden;
                tr.Layer = 1;
                wallTiles[wallTiles.Count - 1].transform.SetPositionAndRotation
                    (new Vector3(x + offset, y),
                    Quaternion.identity);
                wallTiles[wallTiles.Count - 1].SetActive(true);
            }

            //Build player 4 wall (left of screen, bottom to top
            for (int i = 8; i >= -8; i--)
            {
                offset = i * ADJ_TILE_SPACING;

                area = GameObject.Find("Wall 4 Area");
                x = area.transform.position.x;
                y = area.transform.position.y;

                //top layer
                wallTiles.Add(Instantiate(TileBase));
                tr = wallTiles[wallTiles.Count - 1].GetComponent<TileRenderer>();
                tr.Face = Tile.Hidden;
                tr.Layer = 2;
                wallTiles[wallTiles.Count - 1].transform.SetPositionAndRotation
                    (new Vector3(x, y - offset + TILE_HEIGHT),
                    Quaternion.AngleAxis(90, Vector3.forward));
                wallTiles[wallTiles.Count - 1].SetActive(true);

                //bottom layer
                wallTiles.Add(Instantiate(TileBase));
                tr = wallTiles[wallTiles.Count - 1].GetComponent<TileRenderer>();
                tr.Face = Tile.Hidden;
                tr.Layer = 1;
                wallTiles[wallTiles.Count - 1].transform.SetPositionAndRotation
                    (new Vector3(x, y - offset),
                    Quaternion.AngleAxis(90, Vector3.forward));
                wallTiles[wallTiles.Count - 1].SetActive(true);
            }

            //Build player 3 wall (top of screen, left to right)
            for (int i = -8; i <= 8; i++)
            {
                offset = i * ADJ_TILE_SPACING;

                area = GameObject.Find("Wall 3 Area");
                x = area.transform.position.x;
                y = area.transform.position.y;

                //top layer
                wallTiles.Add(Instantiate(TileBase));
                tr = wallTiles[wallTiles.Count - 1].GetComponent<TileRenderer>();
                tr.Face = Tile.Hidden;
                tr.Layer = 2;
                wallTiles[wallTiles.Count - 1].transform.SetPositionAndRotation
                    (new Vector3(x + offset, y + TILE_HEIGHT),
                    Quaternion.identity);
                wallTiles[wallTiles.Count - 1].SetActive(true);

                //bottom layer
                wallTiles.Add(Instantiate(TileBase));
                tr = wallTiles[wallTiles.Count - 1].GetComponent<TileRenderer>();
                tr.Face = Tile.Hidden;
                tr.Layer = 1;
                wallTiles[wallTiles.Count - 1].transform.SetPositionAndRotation
                    (new Vector3(x + offset, y),
                    Quaternion.identity);
                wallTiles[wallTiles.Count - 1].SetActive(true);
            }

            //Build player 2 wall (right of screen, top to bottom)
            for (int i = 8; i>= -8; i--)
            {
                offset = i * ADJ_TILE_SPACING;

                area = GameObject.Find("Wall 2 Area");
                x = area.transform.position.x;
                y = area.transform.position.y;

                //top layer
                wallTiles.Add(Instantiate(TileBase));
                tr = wallTiles[wallTiles.Count - 1].GetComponent<TileRenderer>();
                tr.Face = Tile.Hidden;
                tr.Layer = 2;
                wallTiles[wallTiles.Count - 1].transform.SetPositionAndRotation
                    (new Vector3(x, y + offset + TILE_HEIGHT),
                    Quaternion.AngleAxis(90, Vector3.forward));
                wallTiles[wallTiles.Count - 1].SetActive(true);

                //bottom layer
                wallTiles.Add(Instantiate(TileBase));
                tr = wallTiles[wallTiles.Count - 1].GetComponent<TileRenderer>();
                tr.Face = Tile.Hidden;
                tr.Layer = 1;
                wallTiles[wallTiles.Count - 1].transform.SetPositionAndRotation
                    (new Vector3(x, y + offset), 
                    Quaternion.AngleAxis(90,Vector3.forward));
                wallTiles[wallTiles.Count - 1].SetActive(true);
            }
        }

        //Renders the wall broken
        private void BreakWall()
        {
            //Fetch the wall number that should be broken
            int wallNumber = Board.Wall.BrokenAt;
            //Align draw position (zero-index) accordingly
            int[] rotateBy = { 6, 40, 74, 108 };
            RotateWallTiles(rotateBy[wallNumber - 1]);

            GameObject area;
            float x, y;

            //Move last 6 of dead wall to the center
            int i = wallTiles.Count - 1;
            area = GameObject.Find("Dora Area");
            x = area.transform.position.x;
            y = area.transform.position.y;


            for (int j = 1; j >= -1; j--)
            {
                //Bottom layer
                wallTiles[i].transform.SetPositionAndRotation
                    (new Vector3(x + j * ADJ_TILE_SPACING, y),
                    Quaternion.identity);
                i--;

                //Top layer
                wallTiles[i].transform.SetPositionAndRotation
                    (new Vector3(x + j * ADJ_TILE_SPACING, y + TILE_HEIGHT),
                    Quaternion.identity);
                i--;
            }

            //Move the other 8 dead wall tiles to the upper right corner
            area = GameObject.Find("Dead Wall 2 Area");
            x = area.transform.position.x;
            y = area.transform.position.y;
            for (int j = 1; j >= -2; j--)
            {
                //Bottom layer
                wallTiles[i].transform.SetPositionAndRotation
                    (new Vector3(x + j * ADJ_TILE_SPACING, y), 
                    Quaternion.identity);
                i--;

                //Top layer
                wallTiles[i].transform.SetPositionAndRotation
                    (new Vector3(x + j * ADJ_TILE_SPACING, y + TILE_HEIGHT), 
                    Quaternion.identity);
                i--;
            }
        }

        //Reveals all the current dora indicators
        private void RenderDoraIndicators()
        {
            List<Tile> indicators = Board.Wall.GetDoraIndicators();
            int numberOfDora = indicators.Count;
            //Reveal the ones in the center
            int j = wallTiles.Count - 2;
            TileRenderer tr;
            for (int i = 0; i < 3 && i < numberOfDora; i++)
            {
                tr = wallTiles[j].GetComponent<TileRenderer>();
                tr.Face = indicators[i];
                j -= 2;
            }
            //TODO: if there's a 4th dora, reveal it from the dead wall
            //TODO: if there's a 5th dora, reveal it from the dead wall
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
