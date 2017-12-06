using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TexasHoldEm.PokerGame;

namespace TexasHoldEm

    //TODO: fix raise system, start choice backend, add manditory small bet system
{
    class TexasAI
    {
        public int PlayerGoesFirst { get; private set; }
        public int PlayersTurn { get; private set; }
        public bool IsNewGame;
        private string _ai_stage;
        public string AI_Stage { get { return _ai_stage; } set { _ai_stage = value; Console.WriteLine("AI Stage now set to: " + _ai_stage); } }

        public TexasAI()
        {
            PlayerGoesFirst = 0;
            PlayersTurn = 0;
            ResetAI();
        }

        public void ResetAI()
        {
        AI_Stage = null;
        IsNewGame = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">"Fold","Hold","Match","Raise"</param>
        /// <returns>"Raise or Hold or Fold","Match or Fold"</returns>
        public string TexasStateMachineForAI(string state, PokerGame myGame)
        {
            AI_Stage = "Raise or Hold or Fold";

            switch (state)
            {
                case "Fold":
                    RaiseOrHoldOrFold(myGame);
                    break;

                case "Hold":
                    RaiseOrHoldOrFold(myGame);
                    break;

                case "Match":
                    ContinueMatchCheck(myGame);
                    break;

                case "Raise":
                    ContinueMatchCheck(myGame);
                    break;

                case "Raise or Hold or Fold":
                    if(IsNewGame)
                    {
                        IsNewGame = false;
                        PlayersTurn = PlayerGoesFirst;
                        BlindRounds(myGame);

                        PlayerGoesFirst++;
                        if (PlayerGoesFirst == myGame._players.Length)
                        {
                            PlayerGoesFirst = 0;
                        }
                    }
                    RaiseOrHoldOrFold(myGame);
                    break;
            }
            return AI_Stage;

        }
        private void RaiseOrHoldOrFold(PokerGame myGame)
        {
            if (PlayersTurn == 0)
            {
                Console.WriteLine("@ my turn!");
                PlayersTurn++;
                return;
            }
            Console.WriteLine("@" + myGame._players[PlayersTurn].Name + "'s turn!");

            //TODO: not always hold
            AI_Hold(myGame, PlayersTurn);

            //repeat full round
            PlayersTurn++;
            if (PlayersTurn == myGame._players.Length)
            {
                PlayersTurn = 0;
            }

            RaiseOrHoldOrFold(myGame);
            return;
        }
        private void BlindRounds(PokerGame myGame)
        {
            Console.WriteLine("@@ BLIND ROUND");
            myGame.TakeTurn("Blind", PlayersTurn);
            PlayersTurn++;
            if (PlayersTurn == myGame._players.Length)
            {
                PlayersTurn = 0;
            }
            PlayersTurn++;
            if (PlayersTurn == myGame._players.Length)
            {
                PlayersTurn = 0;
            }
        }
        private void AI_Fold(PokerGame myGame, int playerIndex)
        {
            myGame.TakeTurn("Fold", playerIndex);
        }
        private void AI_Hold(PokerGame myGame, int playerIndex)
        {
            myGame.TakeTurn("Hold", playerIndex);
        }
        private bool AI_Match(PokerGame myGame, int playerIndex)
        {
            return myGame.TakeTurn("Match", playerIndex);
        }
        private void AI_Raise(PokerGame myGame, int playerIndex, int amount)
        {
            Console.WriteLine("$$ AI has raised whose index is " + playerIndex.ToString());
            myGame.TakeTurn("Raise", playerIndex, amount);
        }
        private void ContinueMatchCheck(PokerGame myGame)
        {
                bool buffer = true;
                while (buffer)
                {
                Console.WriteLine("TEXASAI -> 129 -> " + myGame._players[PlayersTurn].Name + " has matched!");
                    if (PlayersTurn == myGame._numPlayers) { PlayersTurn = 1; AI_Stage = "Match or Fold"; return; }

                    //TODO: assume match always for now
                    buffer = !AI_Match(myGame, PlayersTurn);
                    PlayersTurn++;
                }
            }
        
    }
}
