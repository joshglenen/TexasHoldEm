using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldEm
{
    class TexasAI
    {
        public int PlayerGoesFirst { get; private set; }
        protected bool IsNewGame;
        public string buffer;
        public bool PlayerHasRaised;
        protected int PlayersTurn;

        public TexasAI(int index = 0)
        {
            ResetAI();
            PlayerGoesFirst = index-1;
            PlayersTurn = index;
        }

        public void ResetAI()
        {
            PlayerHasRaised = false;
            IsNewGame = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">"Fold","Hold","Match","Raise"</param>
        /// <returns>"Raise or Hold or Fold","Match or Fold"</returns>
        public string TexasStateMachineForAI(string state, Game myGame)
        {
            buffer = "Raise or Hold or Fold";

            switch (state)
            {
                case "Fold":
                    break;

                case "Hold":
                    break;

                case "Match":
                    ContinueMatchCheck();
                    break;

                case "Raise":
                    PlayerHasRaised = true;
                    ContinueMatchCheck();
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
                        PlayerGoesFirst++;
                    }

                    else
                    {
                        //
                    }

                    break;
            }
            return buffer;

        }

        private void AI_Fold(Game myGame, int playerIndex)
        {
            myGame.TakeTurn("Fold", playerIndex);
        }

        private void AI_Hold(Game myGame, int playerIndex)
        {
            myGame.TakeTurn("Hold", playerIndex);
        }

        private void AI_Match(Game myGame, int playerIndex)
        {
            myGame.TakeTurn("Match", playerIndex);
        }

        private void AI_Raise(Game myGame, int playerIndex, int amount)
        {
            myGame.TakeTurn("Raise", playerIndex, amount);
        }

        private void ContinueMatchCheck()
        {
            if(PlayerHasRaised)
            {

            }
        }
    }
}
