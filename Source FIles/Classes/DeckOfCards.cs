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
        
        private string assetFolder;
        public string _cardBackMain;
        public string _cardBackFold;
        public string _cardBackGameOver;

        public CardBase[] Deck {get; protected set;}

        public DeckOfCards(string CardBackLocation = "\\cb\\blue.png")
        {
            string _pathToDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            assetFolder = _pathToDirectory + "\\Assets\\";
            _cardBackMain = assetFolder + CardBackLocation;
            _cardBackFold = assetFolder + "\\cb\\fold.png";
            _cardBackGameOver = assetFolder + "\\cb\\gameover.png";

            Deck = new CardBase[52];
            int i = 0;
            for (int v = 1; v < 14; v++)
            {
                string str = assetFolder;
                str += v.ToString();
                str += "h.png";
                Deck[i] = new CardBase(v, "Hearts", str);
                i++;
            }
            for (int v = 1; v < 14; v++)
            {
                string str = assetFolder;
                str += v.ToString();
                str += "s.png";
                Deck[i] = new CardBase(v, "Spades", str);
                i++;
            }
            for (int v = 1; v < 14; v++)
            {
                string str = assetFolder;
                str += v.ToString();
                str += "d.png";
                Deck[i] = new CardBase(v, "Diamonds", str);
                i++;
            }
            for (int v = 1; v < 14; v++)
            {
                string str = assetFolder;
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

        protected CardBase Peek(int cardsIntoDeck = 1)
        {
            if ((Deck.Length < cardsIntoDeck) ||(cardsIntoDeck < 1))
            {
                Console.WriteLine("Can't peek into this deck with those parameters.");
            }

            CardBase Card = Deck[Deck.Length - cardsIntoDeck];
            return Card;

        }  //shows the value of a card for internal purposes

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
