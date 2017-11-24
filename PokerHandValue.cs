using System.Collections.Generic;
using System.Linq;

namespace TexasHoldEm
{
    class PokerHandValue
    {
        public static bool CheckForFlush(string[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                if (args[0] != args[i]) return false;
            }
            return true;

        } //determines if string array is filled with duplicate strings

        public static bool CheckForStraight(int[] args)
        {
            int[] sortedArgs = args.OrderBy(i => i).ToArray();
            int[] specialCase = { 1, 10, 11, 12, 13 };
            if (sortedArgs.SequenceEqual(specialCase)) return true;
            for (int i = 1; i < sortedArgs.Length; i++)
            {
                if (sortedArgs[i] - sortedArgs[i - 1] != 1) return false;
            }
            return true;

        } //determines if a series of ints are serialized when sorted

        public static int[] CheckForPairs(int[] args)
        {
                int[] sortedArgs = args.OrderBy(i => i).ToArray();
                int[] k = new int[sortedArgs.Length];

                //finds every matching integer
                for (int l = 0; l < sortedArgs.Length-1; l++)
                {
                    for (int i = l+1; i < sortedArgs.Length; i++)
                    {
                        if (sortedArgs[l] == sortedArgs[i]) k[l]++;
                    }
                }

                //determines number of duplicates per integer and returns an array
                var buffer = new List<int>();
                for (int i = 0; i < sortedArgs.Length; i++)
                {
                if (k[i] != 0)
                {
                    buffer.Add(k[i]);
                    buffer.Add(sortedArgs[i]);
                }
                    i += k[i];
                }
                int[] Buffer = buffer.ToArray();
                return Buffer;

        }  //input card values, outputs number of duplicates followed by card value for each unique card value in input

        public static bool CheckFullHouse(int[] args)
        {
            int[] check = CheckForPairs(args);
            if (check[0] + check[2] == 5) return true;
            return false;
        } 

        public static int CheckHighCard(int[] args)
        {
            return args.Max();
        }

        public static bool CheckRoyalFlush(int[] val, string[] suit)
        {
            if (!CheckForFlush(suit)) return false;
            int[] sortedArgs = val.OrderBy(i => i).ToArray();
            int[] specialCase = { 1, 10, 11, 12, 13 };
            if (sortedArgs.SequenceEqual(specialCase)) return true;
            return false;
        }

        /// <summary>
        /// Calculates the value of a poker hand of five cards
        /// </summary>
        /// <param name="values">The value of each card in hand where ace is one and king is 13</param>
        /// <param name="suits">The name of one of four suits; names must be consistant</param>
        /// <returns>The value which can be compared to other values of different hands to find the winning hand</returns>
        ///<vals>
        ///high 14
        ///one pair high 2 130 000
        ///two pair high 4 131 300 
        ///3oKind high 5 130 000
        ///straight  6 000 000
        ///flush 7 000 000
        ///fokinf high 8 130 000
        ///royal	100 000 000
        ///</vals>
        public static int Calculate(int[] values, string[] suits)
        {
            int handValue = 0;
            if (CheckForFlush(suits)) handValue += 7000000;
            if (CheckForStraight(values)) handValue += 6000000;
            if (CheckRoyalFlush(values, suits)) handValue += 1000000;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == 1) values[i] = 14;

            } //fix for ace high issue
            handValue += CheckHighCard(values);

            //check for pairs, and XofaKinds with highest set first
            int[] pairBuffer = new int[4];
            pairBuffer = CheckForPairs(values);
            if(pairBuffer.Length<3)
            {
                List<int> zeroBuffer = pairBuffer.ToList();
                zeroBuffer.Add(0);
                zeroBuffer.Add(0);
                zeroBuffer.Add(0);
                zeroBuffer.Add(0);
                pairBuffer = zeroBuffer.ToArray();
            }
            if((pairBuffer[0]<pairBuffer[2])||((pairBuffer[0] == pairBuffer[2])&&(pairBuffer[1] < pairBuffer[3])))
            {
                int[] pairBufferBuffer = pairBuffer;
                pairBuffer[0] = pairBufferBuffer[2];
                pairBuffer[1] = pairBufferBuffer[3];
                pairBuffer[2] = pairBufferBuffer[0];
                pairBuffer[3] = pairBufferBuffer[1];
            }
            if(pairBuffer[0]!=0)
            {
                handValue += pairBuffer[1] * 10000;
                switch (pairBuffer[0])
                {
                    case 1:
                        handValue += 2000000;
                        break;
                    case 2:
                        handValue += 5000000;
                        break;
                    case 3:
                        handValue += 8000000;
                        break;
                }
            }
            if (pairBuffer[2] != 0)
            {
                handValue += pairBuffer[3] * 100;
                switch (pairBuffer[2])
                {
                    case 1:
                        handValue += 2000000;
                        break;
                    case 2:
                        handValue += 5000000;
                        break;
                    case 3:
                        handValue += 8000000;
                        break;
                }
            }
       
            return handValue;
        } 
    }
}
