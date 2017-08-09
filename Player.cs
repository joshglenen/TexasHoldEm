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

        public void NewHand()
        {
            myHand = new CardBase[NumCards];
            HandIndex = 0;
            Bet = 0;
        }

        public bool NewBet(int bet)
        {
            if (bet <= Funds)
            {
                if (Bet == 0)
                {
                    Bet = bet;
                    Funds -= bet;

                }

                else
                {
                    Bet += bet;
                    Funds -= bet;
                }

                return true;
            }

            else
            {
                return false;
            }
        }

        public void CashIn(int Multiplier = 2)
        {
            Bet = Bet * Multiplier;
            Funds += Bet;
            NewHand();
        }
    }
}
