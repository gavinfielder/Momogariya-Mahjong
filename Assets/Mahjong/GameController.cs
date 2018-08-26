using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{

    //Describes the result of a player's turn
    public struct TurnResult
    {
        public Tile Discard { get; set; }
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

    //Event Args
    /* 
    //Describes a player's discard. 
    public class DiscardEventArgs : EventArgs
    {
        public Player Discarder { get; private set; }
        public Tile Discard { get; private set; }
        public DiscardEventArgs(Tile discard, Player discarder)
        {
            Discard = discard;
            Discarder = discarder;
        }
    }

    //Describes a player's kan
    public class KanEventArgs : EventArgs
    {
        public Player Declarator { get; private set; }
        public Tile KanTile { get; private set; }
        public KanEventArgs(Player declarator, Tile kanTile)
        {
            Declarator = declarator;
            KanTile = kanTile;
        }
    }

    //Describes a player's pon, chii, daiminkan, and ron calls
    public class ClaimEventArgs : EventArgs
    {
        public string Call { get; private set; }
        public Player Claimant { get; private set; }
        public ClaimEventArgs(Player claimaint, string call)
        {
            Claimant = claimaint;
            Call = Call;
        }
    }

    //Describes a player's tsumo
    public class TsumoEventArgs : EventArgs
    {
        public Player Declarator { get; private set; }
        public TsumoEventArgs(Player declarator)
        {
            Declarator = declarator;
        }
    }*/

    //Handles a multi-round game session between the same 4 players
    public class GameController : MonoBehaviour
    {
        //Public information
        public string Round { get; private set; }
        public string Bakaze { get { return Round.Substring(0, 5).Trim(' ', '-', ':'); } }

        //Game data
        public Player[] Players = new Player[4];
        public GameBoard board;

        //Other fields
        private List<Naki> claims = new List<Naki>();

        //State machine
        public string State { get; private set; }
        private delegate void StateProcess();
        private StateProcess ProcessState;
        private Dictionary<string, StateProcess> stateProcesses;


        private void Start()
        {
            InitializeStateMachine();
            SetState("Awaiting Setup");
        }

        private void Update()
        {
            ProcessState();
        }

        private void InitializeStateMachine()
        {
            stateProcesses = new Dictionary<string, StateProcess>();
            stateProcesses.Add("Null", Process_Null);
            stateProcesses.Add("Executing Test", Process_Test);
            stateProcesses.Add("Awaiting Setup", Process_AwaitingSetup);
            stateProcesses.Add("East's Turn - Awaiting Draw", Process_EastTurnAwaitingDraw);
            stateProcesses.Add("East's Turn - Awaiting Discard", Process_EastTurnAwaitingDiscard);
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


        //Advances to the next round
        private void BeginRound(string round)
        {
            //We can skip checks if this is going to be private
            //string bakazeCheck = round.Substring(0, 5).Trim(' ', '-', ':').ToLower();
            //if (bakazeCheck != "east" && bakazeCheck != "south" && bakazeCheck != "west" && bakazeCheck != "north") return;
            
            Round = round;

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
                board.Wall.Build(RuleSets.DefaultRules);
                //int r = (int)(Random.value * 4 + 1);
                int r = 4;
                Debug.Log("Breaking wall " + r + ".");
                board.Wall.Break(r);

                Hand[] hands = new Hand[4];
                hands[0] = GameObject.Find("Hand 1").GetComponent<Hand>();
                hands[1] = GameObject.Find("Hand 2").GetComponent<Hand>();
                hands[2] = GameObject.Find("Hand 3").GetComponent<Hand>();
                hands[3] = GameObject.Find("Hand 4").GetComponent<Hand>();
                hands[0].SetOwner(0, "");
                hands[1].SetOwner(1, "");
                hands[2].SetOwner(2, "");
                hands[3].SetOwner(3, "");

                for (int i = 0; i < 13; i++)
                {
                    hands[0].Add_NoDrawEvent(board.Wall.Draw());
                    hands[1].Add_NoDrawEvent(board.Wall.Draw());
                    hands[2].Add_NoDrawEvent(board.Wall.Draw());
                    hands[3].Add_NoDrawEvent(board.Wall.Draw());
                }
                EventManager.FlagEvent("Hands Dealt");

                hands[0].AddDraw(board.Wall.Draw());
                SetState("East's Turn - Awaiting Discard");


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

        private void Process_EastTurnAwaitingDraw()
        {

        }

        private void Process_EastTurnAwaitingDiscard()
        {

        }


        
    }
}
