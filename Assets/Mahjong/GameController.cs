using System.Collections.Generic;
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
            stateProcesses.Add("Player 1 Awaiting Draw", Process_Player1AwaitingDraw);
            stateProcesses.Add("Player 1 Awaiting Discard", Process_Player1AwaitingDiscard);
        }

        //Sets the internal state
        private void SetState(string state)
        {
            StateProcess prev = ProcessState;
            bool found = stateProcesses.TryGetValue(state, out ProcessState);
            if (found)
            {
                State = state;
            }
            else
            {
                Debug.LogError("Invalid state requested: '" + state + "'. Returning to previous state: '" + prev + "'.");
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

                
                board.Wall.Break(4);
                
                Hand[] hands = new Hand[4];
                hands[0] = GameObject.Find("Hand 1").GetComponent<Hand>();
                hands[1] = GameObject.Find("Hand 2").GetComponent<Hand>();
                hands[2] = GameObject.Find("Hand 3").GetComponent<Hand>();
                hands[3] = GameObject.Find("Hand 4").GetComponent<Hand>();
                hands[0].SetOwner(0, 0);
                hands[1].SetOwner(1, 0);
                hands[2].SetOwner(2, 0);
                hands[3].SetOwner(3, 0);

                for (int i = 0; i < 13; i++)
                {
                    hands[0].Deal(board.Wall.Draw());
                    hands[1].Deal(board.Wall.Draw());
                    hands[2].Deal(board.Wall.Draw());
                    hands[3].Deal(board.Wall.Draw());
                }
                EventManager.FlagEvent("Hands Dealt");

                hands[0].AddDraw(board.Wall.Draw());
                //SetState("Player 1 Awaiting Discard");
                

                //No more actions
                Debug.Log("Test completed. Entering null state...");
                SetState("Null");
            }
        }

        private void Process_AwaitingSetup()
        {
            Debug.Log("Entering test state. Press G to begin test.");
            SetState("Executing Test");
        }

        private void Process_Player1AwaitingDraw()
        {

        }

        private void Process_Player1AwaitingDiscard()
        {

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
