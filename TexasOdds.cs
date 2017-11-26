using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldEm
{
    /// <summary>
    /// Creates float[] values and int[] index which is are a decending-sorted float array and its corresponding indexes respectively.
    /// </summary>
    class UnsignedSortedFloatArray
    {
        public int[] _index;
        public float[] _values;

        public UnsignedSortedFloatArray(float[] args)
        {
            _index = new int[args.Length];
            _values = args;
            for (int i = 0; i < args.Length; i++)
            {
                _index[i] = i;
            }

            USFA_Sort();
        }

        private void USFA_Sort()
        {
            float[] buffer = new float[_values.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = _values[i];

            } //Need to assign based on values since array assignment operator makes arrays point to the same memory and thus not unique.

            //sort arrays
            for (int k = 0; k < _values.Length; k++)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    float temp = buffer.Max();
                    if (buffer[i]==temp)
                    {
                        _values[k] = buffer.Max();
                        buffer[i] = -1;
                        _index[k] = i;
                        break;
                    }
                }
            }
        }

    }

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
        public static float CheckProbability(Game myGame, int playerIndex)
        {
            return 0;
        }

        /// <summary>
        /// Determines the probability of winning based on the cards currently dealt
        /// </summary>
        /// <param name="myGame">The current game class</param>
        /// <returns></returns>
        public static float CheckProbabilityOmnicient(Game myGame, int playerIndex)
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
        public static UnsignedSortedFloatArray CheckWinner(Game myGame)
        {
            float[] buffer = new float[myGame.numPlayers];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = CheckProbabilityOmnicient(myGame, i);
            }
            UnsignedSortedFloatArray output = new UnsignedSortedFloatArray(buffer);
            return output;
        }

    }
}
