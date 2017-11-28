using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldEm

    //TODO: fix raise system, start choice backend, add manditory small bet system
{
    class TexasAI
    {
        public int PlayerGoesFirst { get; private set; }
        protected bool IsNewGame;
        private string _ai_stage;
        public string AI_Stage { get { return _ai_stage; } set { _ai_stage = value; Console.WriteLine("!AI! Stage now set to: " + _ai_stage); } }

        protected int PlayersTurn;

        public TexasAI(int index = 0)
        {
            ResetAI();
            IsNewGame = true;
            PlayerGoesFirst = index-1;
            PlayersTurn = index;
        }

        public void ResetAI()
        {
        AI_Stage = null;
        PlayersTurn = 0;
        IsNewGame = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">"Fold","Hold","Match","Raise"</param>
        /// <returns>"Raise or Hold or Fold","Match or Fold"</returns>
        public string TexasStateMachineForAI(string state, Game myGame)
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
                        if (PlayerGoesFirst == myGame.numPlayers - 1)
                        {
                            Console.WriteLine("Here i am");
                            PlayerGoesFirst = 0;
                            return null;
                        }
                        PlayersTurn = PlayerGoesFirst;
                        PlayerGoesFirst++;
                    }
                    else
                    {
                        RaiseOrHoldOrFold(myGame);
                    }
                    break;
            }
            return AI_Stage;

        }

        private void RaiseOrHoldOrFold(Game myGame)
        {
            if(PlayersTurn != myGame.numPlayers)
            {
                //TODO: always hold for now
                AI_Hold(myGame, PlayersTurn);

                PlayersTurn++;
                RaiseOrHoldOrFold(myGame);
            }
            PlayersTurn = 1;
        }

        private void AI_Fold(Game myGame, int playerIndex)
        {
            myGame.TakeTurn("Fold", playerIndex);
        }
        private void AI_Hold(Game myGame, int playerIndex)
        {
            myGame.TakeTurn("Hold", playerIndex);
        }
        private bool AI_Match(Game myGame, int playerIndex)
        {
            return myGame.TakeTurn("Match", playerIndex);
        }
        private void AI_Raise(Game myGame, int playerIndex, int amount)
        {
            myGame.TakeTurn("Raise", playerIndex, amount);
        }

        private void ContinueMatchCheck(Game myGame)
        {
                bool buffer = true;
                while (buffer)
                {
                    if (PlayersTurn == myGame.numPlayers) { PlayersTurn = 1; AI_Stage = "Match or Fold"; return; }

                    //TODO: assume match always for now
                    buffer = !AI_Match(myGame, PlayersTurn);
                    PlayersTurn++;
                }
            }
        
    }
}
