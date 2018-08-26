using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    public class GameBoard : MonoBehaviour
    {
        //Data
        public WallManager Wall = new WallManager();
        public List<Tile>[] Melds = new List<Tile>[4];
        public Kawa[] Ponds = new Kawa[4];
        


        //Initialization
        private void Start()
        {
            //Allocate array instances
            for (int i = 0; i < 4; i++)
            {
                Melds[i] = new List<Tile>();
                Ponds[i] = new Kawa();
            }
        }
      
        
    }
}
