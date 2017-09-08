namespace TexasHoldEm
{

    class Player
    {
        public string Name;
        public int Bet = 0;
        public int HandIndex = 0;
        public static int NumCards = 5;
        
        private int Funds { get; set; }
        private CardBase[] myHand = new CardBase[NumCards];

        public Player(string name)
        {
            Name = name;
            Funds = 100;
        }

        public void DrawCard(CardBase Card)
        {
            myHand[HandIndex] = Card;
        } 

        public int CheckPoints()
        {
            if (Bet == 0) return 0;
            


        }

        public void ResetHand()
        {
            myHand = new CardBase[NumCards];
            HandIndex = 0;
            Bet = 0;
        } //clear array and reset attributes

        public bool NewBet(int bet) 
        {
            if (bet <= Funds)
            {
                Bet += bet;
                Funds -= bet;
                return true;
            }

            else
            {
                return false;
            }

        } //takes initial bet or raise, changes class attributes, and returns success boolean

        public void CashIn(int Multiplier = 1)
        {
            Bet = Bet * Multiplier;
            Funds += Bet;
        } //add winnings to funds
    }
}
