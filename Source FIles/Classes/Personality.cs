using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldEm
{
    class Personality
    {
        public bool _ignores_bluffs = false;
        public bool _high_roller = false;
        public bool _cautious_better = false;
        public bool _passive_player = false;
        public bool _random_decisions = false;
        public double _maxBetFactor = 1;
        public double _minBetFactor = 1;

        /// <summary>
        /// Defines variables that influence TexasAI based on preset named characters 
        /// </summary>
        /// <param name="personality">Granny, Timmy, Johnny, Jake</param>
        public Personality(string personality = null)
        {
            switch (personality)
            {
                case null:
                    {
                        _random_decisions = true;
                    }
                    break;

                case "Granny": //plays for fun but calls bluffs
                    {
                        _ignores_bluffs = true;
                        _passive_player = true;
                        _minBetFactor = 1.5;
                    }
                    break;

                case "Timmy": //plays for big swing turns and cautiously with bad hands
                    {
                        _high_roller = true;
                        _cautious_better = true;
                        _minBetFactor = 1.75;
                    }
                    break;

                case "Johnny": //plays cautiously always but calls bluffs
                    {
                        _ignores_bluffs = true;
                        _cautious_better = true;
                        _maxBetFactor = 0.75;
                    }
                    break;

                case "Jake": //plays to win but doesn't call bluffs
                    {
                        _cautious_better = true;
                        _minBetFactor = 1.25;
                    }
                    break;
            }
        }
    }
}
