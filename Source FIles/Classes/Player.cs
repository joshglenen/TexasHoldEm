using System;
using static TexasHoldEm.CardBase;
using static TexasHoldEm.Personality;

namespace TexasHoldEm
{
    class Player : Personality
    {
        public string Name;
        public static int NumCards = 0;
        public int Funds { get; private set; }
        public int OriginalFunds { get; private set; }
        public int Score;
        public CardBase[] _myHand { get; private set; }

        private bool _tookTurn;
        public int Bet { get; private set; }
        public int HandIndex { get; private set; }
        public bool Playing;
        public bool AllIn;
        public bool TookTurn { get { return _tookTurn; } set { _tookTurn = value; Console.WriteLine("Player -> " + Name + " just finished thier turn."); } }
        public bool RoseBetThisTurn;

        /// <summary>
        /// A person who is playing in a session of games.
        /// </summary>
        /// <param name="name">A UI identifier. Ideally should have unique names for each player.</param>
        /// <param name="numberOfCardsInHand">The amount of cards a player can hold. Designed for any amount but should be two for a texas holdem player.</param>
        /// <param name="funds">The total amount of money a person is bringing to the table. Ideally should be the same for all players.</param>
        /// <param name="personality">The style of play which is unnecessary for human player's</param>
        public Player(string name, int numberOfCardsInHand = 2, int funds = 100, string personality = null) : base(personality)
        {
            Score = 0;
            OriginalFunds = funds;
            Name = name;
            Funds = funds;
            NumCards = numberOfCardsInHand;
            ResetHand();
        }

        /// <summary>
        /// Allows to read from a hand
        /// </summary>
        /// <returns>The values of the player's hand; index sorted</returns>
        public int[] GetValues()
        {
            try{
                int[] buffer = new int[_myHand.Length];
                for (int i = 0; i < _myHand.Length; i++)
                {
                    buffer[i] = _myHand[i].Value;
                }
                return buffer;
            }
            catch
            {
                throw new System.Exception("Player's hand does not currently exist for " + Name);
            }
        }

        /// <summary>
        /// A debug method to create a hand from an array
        /// </summary>
        public void SetValues(int[] cardValues)
        {
            try
            {
                for (int i = 0; i < cardValues.Length; i++)
                {
                    _myHand[i].Value = cardValues[i];
                }
            }
            catch
            {
                throw new Exception("Player -> SetValues -> input number of cards " + cardValues.Length + " / actual number of cards " + _myHand.Length);
            }
        }

        /// <summary>
        /// Allows to read from a hand
        /// </summary>
        /// <returns>The suits of the player's hand; index sorted</returns>
        public string[] GetSuits()
        {
            string[] buffer = new string[5];
            for (int i = 0; i < _myHand.Length; i++)
            {
                buffer[i] = _myHand[i].Suit;
            }
            return buffer;
        }

        /// <summary>
        /// A debug method to create a hand from an array
        /// </summary>
        public void SetSuits(string[] cardSuits)
        {
            try
            {
                for (int i = 0; i < cardSuits.Length; i++)
                {
                    _myHand[i].Suit = cardSuits[i];
                }
            }
            catch
            {
                throw new Exception("Player -> SetSuits -> input number of cards " + cardSuits.Length + " / actual number of cards " + _myHand.Length);
            }
        }

        /// <summary>
        /// Draws a card from a deck of cards
        /// </summary>
        /// <param name="Card">a card of type CardBase from the deck of type DeckOfCards</param>
        public void DrawCard(CardBase Card)
        {
            _myHand[HandIndex] = Card;
            HandIndex++;
        }

        /// <summary>
        /// To be called at the start of a game as it returns the game variables to thier starting state. Errors will occur otherwise.
        /// </summary>
        public void ResetHand()
        {
            Playing = true;
            AllIn = false;
            TookTurn = false;
            RoseBetThisTurn = false;
            _myHand = new CardBase[NumCards];
            HandIndex = 0;
            Bet = 0;
        } 

        /// <summary>
        /// replaces a bet or creates one
        /// </summary>
        /// <param name="bet">amount to decrease from funds</param>
        /// <returns>False when a player is all in</returns>
        public bool NewBet(int bet)
        {
            if (AllIn) return false;
            if (bet < Funds)
            {
                Funds += Bet;
                Bet = bet;
                Funds -= Bet;
                return true;
            }

            else
            {
                AllIn = true;
                Bet = Funds;
                Funds = 0;
                return true;
            }

        } //

        //TODO: fix raise, it will not resolve in an end of round.

        /// <summary>
        /// adds to a bet or creates one
        /// </summary>
        /// <param name="bet">amount to decrease from funds</param>
        /// <returns>False when player is all in</returns>
        public bool RaiseBet(int bet)
        {
            if (AllIn) return false;
            if (bet < Funds)
            {
                Bet += bet;
                Funds -= bet;
                return true;
            }

            else
            {
                AllIn = true;
                Bet += Funds;
                Funds = 0;
                return true;
            }

        }

        /// <summary>
        /// Player leaves game instead of betting more but remains in the session.
        /// </summary>
        public void Fold()
        {
            _myHand = null;
            Playing = false;
            AllIn = false;
        }

        /// <summary>
        /// add winnings to funds
        /// </summary>
        /// <param name="income">value of the pool that is added to the player's funds</param>
        public void SetFunds(int income)
        {
            Funds += income;
            Playing = false;
            AllIn = false;
        }

    }

}
