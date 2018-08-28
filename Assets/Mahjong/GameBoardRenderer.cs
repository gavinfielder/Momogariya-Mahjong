using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mahjong
{
    class GameBoardRenderer : MonoBehaviour
    {


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
            //EventManager.Subscribe("Wall Built", RenderWall);
            //EventManager.Subscribe("Wall Broken", BreakWall);
            //EventManager.Subscribe("New Dora", RenderDoraIndicators);
            //EventManager.Subscribe("Normal Draw", OnNormalDraw);
        }


    }
}
