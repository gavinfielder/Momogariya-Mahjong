using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    public class GameBoard : MonoBehaviour
    {
        //Data
        private List<GameObject> allTiles = new List<GameObject>();
        public WallManager Wall = new WallManager();
        
        public List<TileID>[] Melds = new List<TileID>[4];
        public Kawa[] Ponds = new Kawa[4];


        //Initialization
        private void Start()
        {
            //Allocate array instances
            for (int i = 0; i < 4; i++)
            {
                Melds[i] = new List<TileID>();
                Ponds[i] = new Kawa();
            }
        }

        
        //Builds the wall with the current tile set
        //TODO: Since players use board.Wall.Draw(), shouldn't this be simply Wall.Build()? how to refactor?
        //      The issue is that GameController should probably call board.Wall.Build(allTiles) but
        //      allTiles is private to 
        public void BuildWall()
        {

        }
    }
}
