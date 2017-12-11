using System.Collections.Generic;
using System.Linq;
using System;

namespace TexasHoldEm
{

    class HandValueCalculator
    {
        /// <summary>
        /// Calculates the value of a poker hand of five cards
        /// </summary>
        /// <param name="values">The value of each card in hand where ace is one and king is 13</param>
        /// <param name="suits">The name of one of four suits; names must be consistant</param>
        /// <returns>The value which can be compared to other values of different hands to find the winning hand</returns>
        ///<vals>
        ///high 14
        ///one pair high 2 140 000
        ///two pair high 4 141 300 
        ///3oKind high 5 140 000
        ///straight  6 000 000
        ///flush 7 000 000
        ///f o kind high 8 140 000
        ///FH 10 141 300
        ///royal	100 000 000
        ///</vals>
        public static int Calculate(int[] values, string[] suits)
        {
            if ((values.Length != 5)|| (suits.Length != 5)) throw new UnauthorizedAccessException("This method only deals with the best five cards");

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
            if (pairBuffer.Length < 3)
            {
                List<int> zeroBuffer = pairBuffer.ToList();
                zeroBuffer.Add(0);
                zeroBuffer.Add(0);
                zeroBuffer.Add(0);
                zeroBuffer.Add(0);
                pairBuffer = zeroBuffer.ToArray();
            }

            //switch so highest pair is first 1112
            if ((pairBuffer[0] < pairBuffer[2]) || ((pairBuffer[0] == pairBuffer[2]) && (pairBuffer[1] < pairBuffer[3])))
            {
                //assignment operator not copying as expected
                int[] pairBufferBuffer = new int[pairBuffer.Length];
                for (int i = 0; i < pairBuffer.Length; i++)
                {

                    pairBufferBuffer[i] = pairBuffer[i];
                }
                pairBuffer[0] = pairBufferBuffer[2];
                pairBuffer[1] = pairBufferBuffer[3];
                pairBuffer[2] = pairBufferBuffer[0];
                pairBuffer[3] = pairBufferBuffer[1];
            }

            //add the pair's value to the hand value 1212   1211
            if (pairBuffer[0] != 0)
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

        /// <summary>
        /// Calculates the value of a poker hand of five cards
        /// Brute force check of best combination of 3 of 5 dealer cards. 
        /// Caution: slow.
        /// </summary>
        /// <param name="values">the values of the dealers 5 cards followed by the players two cards in a 7 integer array</param>
        /// <param name="suits">Suits corresponding to the values based on index</param>
        /// <returns>A 6 integer zero indexed array of the best value followed by the values of that hand sorted in decending order</returns>
        public static int BOF_Calculate(int[] values, string[] suits)
        {
            int[] CheckForMaxValue = new int[10];
            int[] bufferValues = new int[5];
            string[] bufferSuits = new string[5];

            bufferSuits[3] = suits[5];
            bufferValues[3] = values[5];
            bufferSuits[4] = suits[6];
            bufferValues[4] = values[6];

            //6
            bufferSuits[0] = suits[0];
            bufferValues[0] = values[0];
            {
                bufferSuits[1] = suits[1];
                bufferValues[1] = values[1];
                {
                    bufferSuits[2] = suits[2];
                    bufferValues[2] = values[2];
                    CheckForMaxValue[0] = Calculate(bufferValues, bufferSuits);
                }
                {
                    bufferSuits[2] = suits[3];
                    bufferValues[2] = values[3];
                    CheckForMaxValue[1] = Calculate(bufferValues, bufferSuits);
                }
                {
                    bufferSuits[2] = suits[4];
                    bufferValues[2] = values[4];
                    CheckForMaxValue[2] = Calculate(bufferValues, bufferSuits);
                }
                bufferSuits[1] = suits[2];
                bufferValues[1] = values[2];
                {
                    bufferSuits[2] = suits[3];
                    bufferValues[2] = values[3];
                    CheckForMaxValue[3] = Calculate(bufferValues, bufferSuits);
                }
                {
                    bufferSuits[2] = suits[4];
                    bufferValues[2] = values[4];
                    CheckForMaxValue[4] = Calculate(bufferValues, bufferSuits);
                }
                bufferSuits[1] = suits[3];
                bufferValues[1] = values[3];
                {
                    bufferSuits[2] = suits[4];
                    bufferValues[2] = values[4];
                    CheckForMaxValue[5] = Calculate(bufferValues, bufferSuits);
                }
            }

            //3
            bufferSuits[0] = suits[1];
            bufferValues[0] = values[1];
            {
                bufferSuits[1] = suits[2];
                bufferValues[1] = values[2];
                {
                    bufferSuits[2] = suits[3];
                    bufferValues[2] = values[3];
                    CheckForMaxValue[6] = Calculate(bufferValues, bufferSuits);
                }
                {
                    bufferSuits[2] = suits[4];
                    bufferValues[2] = values[4];
                    CheckForMaxValue[7] = Calculate(bufferValues, bufferSuits);
                }
                bufferSuits[1] = suits[3];
                bufferValues[1] = values[3];
                {
                    bufferSuits[2] = suits[4];
                    bufferValues[2] = values[4];
                    CheckForMaxValue[8] = Calculate(bufferValues, bufferSuits);
                }
            }

            //1
            {
                bufferSuits[0] = suits[2];
                bufferValues[0] = values[2];
                bufferSuits[1] = suits[3];
                bufferValues[1] = values[3];
                bufferSuits[2] = suits[4];
                bufferValues[2] = values[4];
                CheckForMaxValue[9] = Calculate(bufferValues, bufferSuits);
            }
            bufferSuits = null;
            bufferValues = null;
            return CheckForMaxValue.Max();
        }

        //internal use for now
        private static bool CheckForFlush(string[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                if (args[0] != args[i]) return false;
            }
            return true;

        } //determines if string array is filled with duplicate strings
        private static bool CheckForStraight(int[] args)
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
        private static int[] CheckForPairs(int[] args)
        {
            int[] sortedArgs =  args.OrderBy(i => i).ToArray();
            int[] k = new int[sortedArgs.Length];
            
            //finds every matching integer. 1,1,1,2,2
            for (int l = 0; l < sortedArgs.Length-1; l++)
                {
                    for (int i = l+1; i < sortedArgs.Length; i++)
                    {
                        if (sortedArgs[l] == sortedArgs[i]) k[l]++;

                     }
                }
            
            //determines number of duplicates per integer and returns an array. 2,1,0,1,0
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



            //1,1,1,2
            return Buffer;

        }  //input card values, outputs number of duplicates followed by card value for each unique card value in input
        private static bool CheckFullHouse(int[] args)
        {
            int[] check = CheckForPairs(args);
            if (check[0] + check[2] == 5) return true;
            return false;
        }
        private static int CheckHighCard(int[] args)
        {
            return args.Max();
        }
        private static bool CheckRoyalFlush(int[] val, string[] suit)
        {
            if (!CheckForFlush(suit)) return false;
            int[] sortedArgs = val.OrderBy(i => i).ToArray();
            int[] specialCase = { 1, 10, 11, 12, 13 };
            if (sortedArgs.SequenceEqual(specialCase)) return true;
            return false;
        }
    }
}
