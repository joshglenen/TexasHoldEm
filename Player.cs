namespace TexasHoldEm
{
    class Player
    {
        public string       Name;
        public int          Bet { get; private set;}
        public static int   NumCards = 0;
        public int          Funds { get; private set;}
        private CardBase[]  myHand;
        public bool Playing;
        public bool AllIn;
        private int HandIndex = 0;

        public Player(string name , int numberOfCardsInHand = 2 , int funds = 100)
        {
            Name = name;
            Funds = funds;
            NumCards = numberOfCardsInHand;
            ResetHand();
        }

        public int[] GetValues()
        {
            int[] buffer = new int[5];
            for (int i = 0; i < myHand.Length; i++)
            {
                buffer[i] = myHand[i].Value;
            }
            return buffer;
        }

        public string[] GetSuits()
        {
            string[] buffer = new string[5];
            for (int i = 0; i < myHand.Length; i++)
            {
                buffer[i] = myHand[i].Suit;
            }
            return buffer;
        }

        public void DrawCard(CardBase Card)
        {
            myHand[HandIndex] = Card;
        } 

        public void ResetHand()
        {
            Playing = true;
            AllIn = false;
            myHand = new CardBase[NumCards];
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
            Playing = false;
            AllIn = false;
        }

        public void CashIn(int Multiplier = 1)
        {
            Bet = Bet * Multiplier;
            Funds += Bet;
            Playing = false;
            AllIn = false;
        } //add winnings to funds
    }
}
