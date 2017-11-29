using static TexasHoldEm.CardBase;

namespace TexasHoldEm
{
    class Player
    {
        //Session information
        public string Name;
        public static int NumCards = 0;
        public int Bet { get; private set; }
        public int Funds { get; private set; }
        public int OriginalFunds { get; private set; }
        public int Score;

        //internal variables
        public CardBase[] _myHand { get; private set; }
        public int HandIndex { get; private set; }
        public bool Playing;
        public bool AllIn;

        public Player(string name, int numberOfCardsInHand = 2, int funds = 100)
        {
            Score = 0;
            OriginalFunds = funds;
            Name = name;
            Funds = funds;
            NumCards = numberOfCardsInHand;
            ResetHand();
        }

        public int[] GetValues()
        {
            int[] buffer = new int[5];
            for (int i = 0; i < _myHand.Length; i++)
            {
                buffer[i] = _myHand[i].Value;
            }
            return buffer;
        }

        public string[] GetSuits()
        {
            string[] buffer = new string[5];
            for (int i = 0; i < _myHand.Length; i++)
            {
                buffer[i] = _myHand[i].Suit;
            }
            return buffer;
        }

        public void DrawCard(CardBase Card)
        {
            _myHand[HandIndex] = Card;
            HandIndex++;
        }

        public void ResetHand()
        {
            Playing = true;
            AllIn = false;
            _myHand = new CardBase[NumCards];
            HandIndex = 0;
            Bet = 0;
        } //clear array and reset attributes

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

        } //replaces a bet or creates one

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

        } //adds to a bet or creates one

        public void Fold()
        {
            _myHand = null;
            Playing = false;
            AllIn = false;
        }

        public void SetFunds(int income)
        {
            OriginalFunds += income - Bet;
            Funds += income;
            Playing = false;
            AllIn = false;
        } //add winnings to funds

    }

}
