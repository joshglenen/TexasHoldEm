using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TexasHoldEm.UnsignedSortedFloatArray;

namespace TexasHoldEm
{
    class TexasOdds
    {
        /// <summary>
        /// Determines the value of the player's hand based on a two card combination only.
        /// </summary>
        /// <param name="myDeck">2 card array using DeckOfCards class that makes up a player's hand</param>
        /// <returns>
        /// A value based on this scale:
        /// 
        /// </returns>
        public static int CheckHandValue(CardBase[] myHand)
        {
            return 0;
        }

        /// <summary>
        /// Determines the probability of winning based on the cards a player can currently see.
        /// </summary>
        /// <param name="myGame">The current game class</param>
        /// <returns></returns>
        public static float CheckProbability(PokerGame myGame, int playerIndex)
        {
            return 0;
        }

        /// <summary>
        /// Determines the probability of winning based on the cards currently dealt
        /// </summary>
        /// <param name="myGame">The current game class</param>
        /// <returns></returns>
        public static float CheckProbabilityOmnicient(PokerGame myGame, int playerIndex)
        {
            return 0;
        }

        /// <summary>
        /// Determines the most likely winner of the game and thier probability; followed by the second, third, etc.
        /// </summary>
        /// <param name="myGame">The current game class</param>
        /// <returns>A class with an index int[] (order of likliest winner) and a values float[] (probabilities of each winner)
        /// EX: float[0] = 0.9 , index[0] = 3 , the fourth player is likely to win with a 90% probability.
        /// </returns>
        public static UnsignedSortedFloatArray CheckWinner(PokerGame myGame)
        {
            float[] buffer = new float[myGame._players.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = CheckProbabilityOmnicient(myGame, i);
            }
            UnsignedSortedFloatArray output = new UnsignedSortedFloatArray(buffer);
            return output;
        }

    }
}
