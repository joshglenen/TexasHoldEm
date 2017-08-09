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

        public CardBase[] Deck;

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

           // for (int v = 0; v < 52; v++)
           // {
           //     if (Deck[v].Value >= 11)
           //     {
           //         Deck[v].Special = true;
           //     }
           // }

            KnuthShuffle<CardBase>(Deck);
        }

        public CardBase DrawTop()
        {
            if(Deck.Length>0)
            {
                CardBase Card = Deck[Deck.Length - 1];

                CardBase[] Temp = new CardBase[Deck.Length - 1];

                for (int i = 0; i<Deck.Length-1; i++)
                {
                    Temp[i] = Deck[i];
                }

                Deck = Temp;

                return Card;
            }

            throw new Exception("DrawTop will not draw from an empty deck!");
        }

        private static void KnuthShuffle<T>(T[] array)
        {
            System.Random random = new System.Random();
            for (int i = 0; i < array.Length; i++)
            {
                int j = random.Next(i, array.Length); // Don't select from the entire array on subsequent loops
                T temp = array[i]; array[i] = array[j]; array[j] = temp;
            }
        }

        public void Shuffle()
        {
            KnuthShuffle<CardBase>(Deck);
        }
        
        public override string ToString()
        {
            string temp = "";
            foreach(CardBase card in Deck)
            {
                temp += card.Value + " " + card.Suit + ", ";
            }
            return temp;
        }

        public void AddJoker()
        {
            try
            {
                Array.Resize<CardBase>(ref Deck, Deck.Length + 1);
                Deck[Deck.Length-1] = new CardBase(0, "Joker", null); //TODO
            }
            catch
            {
                throw new Exception("Cannot add joker");
            }
        }

    }

}
