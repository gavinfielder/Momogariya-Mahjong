﻿using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{

    //Handles a multi-round game session between the same 4 players
    public class GameController : MonoBehaviour
    {
        //***********************************************************************
        //********************************* Data ********************************
        //***********************************************************************

        //Public information
        public string Round { get; private set; }
        public string Bakaze { get { return Round.Substring(0, 5).Trim(' ', '-', ':'); } }

        //Game data
        public Player[] Players = new Player[4];
        public Player CurrentTurnPlayer { get; private set; }
        public GameBoard board;
        private List<GameObject> allTiles = new List<GameObject>();


        //Object references
        #pragma warning disable CS0649
        public GameObject TileBase;
        #pragma warning restore CS0649

        //***********************************************************************
        //************************** General Functions **************************
        //***********************************************************************

        private void Start()
        {
            InitializeStateMachine();
            SetState("Awaiting Setup");
        }

        private void Update()
        {
            //Process the current state
            ProcessState();
        }

        //Advances to the next round
        private void BeginRound(string round)
        {
            //We can skip checks if this is going to be private
            //string bakazeCheck = round.Substring(0, 5).Trim(' ', '-', ':').ToLower();
            //if (bakazeCheck != "east" && bakazeCheck != "south" && bakazeCheck != "west" && bakazeCheck != "north") return;
            
            Round = round;

        }


        //***********************************************************************
        //************************** Tile Instantiation *************************
        //***********************************************************************

        //Creates all the tiles used in the game
        public void CreateTileSet(RuleSet rules)
        {
            ClearTiles();

            for (int i = 0; i < 4; i++)
            {
                AddTile(TileID.Suits.Man, 1);
                AddTile(TileID.Suits.Man, 2);
                AddTile(TileID.Suits.Man, 3);
                AddTile(TileID.Suits.Man, 4);
                AddTile(TileID.Suits.Man, 6);
                AddTile(TileID.Suits.Man, 7);
                AddTile(TileID.Suits.Man, 8);
                AddTile(TileID.Suits.Man, 9);

                AddTile(TileID.Suits.Pin, 1);
                AddTile(TileID.Suits.Pin, 2);
                AddTile(TileID.Suits.Pin, 3);
                AddTile(TileID.Suits.Pin, 4);
                AddTile(TileID.Suits.Pin, 6);
                AddTile(TileID.Suits.Pin, 7);
                AddTile(TileID.Suits.Pin, 8);
                AddTile(TileID.Suits.Pin, 9);

                AddTile(TileID.Suits.Sou, 1);
                AddTile(TileID.Suits.Sou, 2);
                AddTile(TileID.Suits.Sou, 3);
                AddTile(TileID.Suits.Sou, 4);
                AddTile(TileID.Suits.Sou, 6);
                AddTile(TileID.Suits.Sou, 7);
                AddTile(TileID.Suits.Sou, 8);
                AddTile(TileID.Suits.Sou, 9);

                AddTile(TileID.Suits.Kaze, TileID.TON);
                AddTile(TileID.Suits.Kaze, TileID.NAN);
                AddTile(TileID.Suits.Kaze, TileID.SHAA);
                AddTile(TileID.Suits.Kaze, TileID.PEI);

                AddTile(TileID.Suits.Sangen, TileID.CHUN);
                AddTile(TileID.Suits.Sangen, TileID.HAKU);
                AddTile(TileID.Suits.Sangen, TileID.HATSU);
            }
            //Add in 5's depending on whether akadora is specified
            if (rules.Akadora == true)
            {
                AddTile(TileID.Suits.Man, 5);
                AddTile(TileID.Suits.Man, 5);
                AddTile(TileID.Suits.Man, 5);
                AddTile(TileID.Suits.Man, 5, true);

                AddTile(TileID.Suits.Pin, 5);
                AddTile(TileID.Suits.Pin, 5);
                AddTile(TileID.Suits.Pin, 5);
                AddTile(TileID.Suits.Pin, 5, true);

                AddTile(TileID.Suits.Sou, 5);
                AddTile(TileID.Suits.Sou, 5);
                AddTile(TileID.Suits.Sou, 5);
                AddTile(TileID.Suits.Sou, 5, true);
            }
            else
            {
                AddTile(TileID.Suits.Man, 5);
                AddTile(TileID.Suits.Man, 5);
                AddTile(TileID.Suits.Man, 5);
                AddTile(TileID.Suits.Man, 5);

                AddTile(TileID.Suits.Pin, 5);
                AddTile(TileID.Suits.Pin, 5);
                AddTile(TileID.Suits.Pin, 5);
                AddTile(TileID.Suits.Pin, 5);

                AddTile(TileID.Suits.Sou, 5);
                AddTile(TileID.Suits.Sou, 5);
                AddTile(TileID.Suits.Sou, 5);
                AddTile(TileID.Suits.Sou, 5);

            }
        }

        //Helper function for CreateTileSet
        private void AddTile(TileID.Suits suit, byte number, bool aka = false)
        {
            GameObject tileObj = Instantiate(TileBase);
            Tile tile = tileObj.GetComponent<Tile>();
            tile.Set(suit, number, aka);
            tileObj.SetActive(true);
            allTiles.Add(tileObj);
        }

        //Clears the tile collection
        private void ClearTiles()
        {
            while (allTiles.Count > 0)
            {
                allTiles[0].SetActive(false);
                allTiles.RemoveAt(0);
            }
        }

        //***********************************************************************
        //*********************** State Machine Functions ***********************
        //***********************************************************************

        //State machine
        public string State { get; private set; }
        private delegate void StateProcess();
        private StateProcess ProcessState;
        private Dictionary<string, StateProcess> stateProcesses;

        //Initializes the state dictionary
        private void InitializeStateMachine()
        {
            stateProcesses = new Dictionary<string, StateProcess>();
            stateProcesses.Add("Null", Process_Null);
            stateProcesses.Add("Executing Test", Process_Test);
            stateProcesses.Add("Awaiting Setup", Process_AwaitingSetup);
            stateProcesses.Add("Player 1 Turn", Process_Player1Turn);
            stateProcesses.Add("Player 2 Turn", Process_Player2Turn);
            stateProcesses.Add("Player 3 Turn", Process_Player3Turn);
            stateProcesses.Add("Player 4 Turn", Process_Player4Turn);
            stateProcesses.Add("Awaiting Player Action", Process_Null);
        }

        //Sets the internal state
        private void SetState(string state)
        {
            StateProcess prev = ProcessState;
            string prevStr = State;
            bool found = stateProcesses.TryGetValue(state, out ProcessState);
            if (found)
            {
                State = state;
                Debug.Log("GameController new state: " + state);
            }
            else
            {
                Debug.LogError("Invalid state requested: '" + state + "'. Returning to previous state: '" + prevStr + "'.");
                ProcessState = prev;
            }
        }
        
        //***********************************************************************
        //*************************** State Processors **************************
        //***********************************************************************

        private void Process_Null() { }

        private void Process_Test()
        {
            //Test script
            if (Input.GetKeyDown("g"))
            {

                Debug.Log("Executing test script...");
                CreateTileSet(RuleSets.DefaultRules);
                board.Wall.Build(ref allTiles);

                System.Random rand = new System.Random();
                int r = rand.Next(1, 5);
                if (r == 5) r = 4;
                //r = 4;

                board.Wall.Break(r);
                
                Hand[] hands = new Hand[4];
                hands[0] = GameObject.Find("Hand 1").GetComponent<Hand>();
                hands[1] = GameObject.Find("Hand 2").GetComponent<Hand>();
                hands[2] = GameObject.Find("Hand 3").GetComponent<Hand>();
                hands[3] = GameObject.Find("Hand 4").GetComponent<Hand>();

                GameObject.Find("Player 1").AddComponent<UserPlayer>();
                GameObject.Find("Player 2").AddComponent<UserPlayer>();
                GameObject.Find("Player 3").AddComponent<UserPlayer>();
                GameObject.Find("Player 4").AddComponent<UserPlayer>();

                Players[0] = GameObject.Find("Player 1").GetComponent<UserPlayer>();
                Players[1] = GameObject.Find("Player 2").GetComponent<UserPlayer>();
                Players[2] = GameObject.Find("Player 3").GetComponent<UserPlayer>();
                Players[3] = GameObject.Find("Player 4").GetComponent<UserPlayer>();


                Players[0].Setup(1, ref hands[0], ref board.Ponds[0], this, ref board);
                Players[1].Setup(2, ref hands[1], ref board.Ponds[1], this, ref board);
                Players[2].Setup(3, ref hands[2], ref board.Ponds[2], this, ref board);
                Players[3].Setup(4, ref hands[3], ref board.Ponds[3], this, ref board);

                Players[0].enabled = true;
                Players[1].enabled = true;
                Players[2].enabled = true;
                Players[3].enabled = true;

                for (int i = 0; i < 13; i++)
                {
                    hands[0].Deal(board.Wall.Draw());
                    hands[1].Deal(board.Wall.Draw());
                    hands[2].Deal(board.Wall.Draw());
                    hands[3].Deal(board.Wall.Draw());
                }
                EventManager.FlagEvent("Hands Dealt");

                SetState("Player 1 Turn");
                

                //No more actions
                //Debug.Log("Test completed. Entering null state...");
                //SetState("Null");
            }
        }

        private void Process_AwaitingSetup()
        {
            Debug.Log("Entering test state. Press G to begin test.");
            SetState("Executing Test");
        }

        //Player 1 Turn
        private void Process_Player1Turn()
        {
            Players[0].StartTurn(TurnArgs.Default);
            EventManager.Subscribe("Hand 1 Discard", OnPlayer1Discard);
            SetState("Awaiting Player Action");
        }
        private void OnPlayer1Discard()
        {
            EventManager.Unsubscribe("Hand 1 Discard", OnPlayer1Discard);
            SetState("Player 2 Turn");
        }

        //Player 2 Turn
        private void Process_Player2Turn()
        {
            Players[1].StartTurn(TurnArgs.Default);
            EventManager.Subscribe("Hand 2 Discard", OnPlayer2Discard);
            SetState("Awaiting Player Action");
        }
        private void OnPlayer2Discard()
        {
            EventManager.Unsubscribe("Hand 2 Discard", OnPlayer2Discard);
            SetState("Player 3 Turn");
        }

        //Player 3 Turn
        private void Process_Player3Turn()
        {
            Players[2].StartTurn(TurnArgs.Default);
            EventManager.Subscribe("Hand 3 Discard", OnPlayer3Discard);
            SetState("Awaiting Player Action");
        }
        private void OnPlayer3Discard()
        {
            EventManager.Unsubscribe("Hand 3 Discard", OnPlayer3Discard);
            SetState("Player 4 Turn");
        }

        //Player 4 Turn
        private void Process_Player4Turn()
        {
            Players[3].StartTurn(TurnArgs.Default);
            EventManager.Subscribe("Hand 4 Discard", OnPlayer4Discard);
            SetState("Awaiting Player Action");
        }
        private void OnPlayer4Discard()
        {
            EventManager.Unsubscribe("Hand 4 Discard", OnPlayer4Discard);
            SetState("Player 1 Turn");
        }



        //***********************************************************************
        //***************************** Event Handlers **************************
        //***********************************************************************

        public void OnPonButtonClick()
        {
            EventManager.FlagEvent("Pon Called");
            Debug.Log("Pon Called");
        }
        public void OnChiiButtonClick()
        {
            EventManager.FlagEvent("Chii Called");
            Debug.Log("Chii Called");
        }
        public void OnRonButtonClick()
        {
            EventManager.FlagEvent("Ron Called");
            Debug.Log("Ron Called");
        }
        public void OnKanButtonClick()
        {
            EventManager.FlagEvent("Kan Called");
            Debug.Log("Kan Called");
        }
        public void OnTsumoButtonClick()
        {
            EventManager.FlagEvent("Tsumo Called");
            Debug.Log("Tsumo Called");
        }
        public void OnContinueButtonClick()
        {
            EventManager.FlagEvent("Continue Called");
            Debug.Log("Continue Called");
        }


    }





    //Describes the result of a player's turn
    public struct TurnResult
    {
        public TileID Discard { get; set; }
    }

    //Player turn arguments based on type of draw
    public enum TurnArgs : byte
    {
        Default = 1,
        Chii = 2,
        Pon = 3,
        Daiminkan = 4,
        KanContinue = 5
    }

    //Types of Naki. Enumerated by priority low to high
    public enum NakiType : byte
    {
        Nashi = 0,
        Chii = 2,
        Pon = 3,
        Daiminkan = 4,
        Ron = 6
    }

    //Describes a tile discard call request
    public struct Naki
    {
        public NakiType Type;
        public Player Requestor;
    }


}
