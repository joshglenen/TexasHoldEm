using System;

namespace TexasHoldEm
{
    public struct CardBase
    {
        public int Value;
        public string Suit;
        public string Asset;

        public CardBase(int Value, string Suit, String Asset)
        {
            this.Value = Value; this.Suit = Suit; this.Asset = Asset;
        }
    }

    class DeckOfCards 
    {
        
        private string pathToDir;

        public CardBase[] Deck {get; protected set;}

        public DeckOfCards()
        {
            pathToDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Assets\\";
            
            Deck = new CardBase[52];
            int i = 0;
            for (int v = 1; v < 14; v++)
            {
                string str = pathToDir;
                str += v.ToString();
                str += "h.png";
                Deck[i] = new CardBase(v, "Hearts", str);
                i++;
            }
            for (int v = 1; v < 14; v++)
            {
                string str = pathToDir;
                str += v.ToString();
                str += "s.png";
                Deck[i] = new CardBase(v, "Spades", str);
                i++;
            }
            for (int v = 1; v < 14; v++)
            {
                string str = pathToDir;
                str += v.ToString();
                str += "d.png";
                Deck[i] = new CardBase(v, "Diamonds", str);
                i++;
            }
            for (int v = 1; v < 14; v++)
            {
                string str = pathToDir;
                str += v.ToString();
                str += "c.png";
                Deck[i] = new CardBase(v, "Clubs", str);
                i++;
            }
            KnuthShuffle<CardBase>(Deck);
        } //constructor

        protected CardBase DrawTop()
        {
            if (Deck.Length <= 0)
            {
                DeckOfCards Buffer = new DeckOfCards();
                Deck = Buffer.Deck;
                Console.WriteLine("ERROR: NEW DECK CREATED");

            } 

                CardBase Card = Deck[Deck.Length - 1];

                CardBase[] Temp = new CardBase[Deck.Length - 1];

                for (int i = 0; i<Deck.Length-1; i++)
                {
                    Temp[i] = Deck[i];
                }

                Deck = Temp;
                return Card;

        }  //draws last element in array

        private static void KnuthShuffle<T>(T[] array)
        {
            System.Random random = new System.Random();
            for (int i = 0; i < array.Length; i++)
            {
                int j = random.Next(i, array.Length); // Don't select from the entire array on subsequent loops
                T temp = array[i]; array[i] = array[j]; array[j] = temp;
            }
        } //interior code

        protected void Shuffle()
        {
            KnuthShuffle<CardBase>(Deck);
        } //random shuffle
        
        public override string ToString()
        {
            string temp = "";
            foreach(CardBase card in Deck)
            {
                temp += card.Value + " " + card.Suit + ", ";
            }
            return temp;
        } //debug

    }
}
