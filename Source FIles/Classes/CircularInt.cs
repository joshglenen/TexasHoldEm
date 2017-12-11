using System;

namespace TexasHoldEm
{
    //TODO: fix errors that occur in unused functions or remove them.

    class CircularInt
    {
        public int _val { get; private set; }
        public int _min { get; private set; }
        public int _max { get; private set; }

        /// <summary>
        /// An integer with a minimum and maximum value that are never exceeded when operated upon.
        /// </summary>
        /// <param name="mimimum">minimum integer</param>
        /// <param name="maximum">maximum integer</param>
        /// <param name="value">current value of the integer</param>
        public CircularInt(int maximum, int mimimum = 0, int value = 0)
        {
            _val = value;
            _min = mimimum;
            _max = maximum; 
        }

        //Operations within the class

        public bool Compare(CircularInt var)
        {
            if ((_val == var._val) && (_max == var._max) && (_min == var._min)) return true;
            return false;
        }

        public bool CompareValue(CircularInt var)
        {
            if (_val == var._val) return true;
            return false;
        }

        public void Equals(int value)
        {
            _val = value;

            int buffer = 0;
            if (_val > _max)
            {
                buffer = _val + _max - _min + 1;
                while ((buffer > _max) || (buffer < _min))
                {
                    buffer = buffer + _max - _min + 1;
                }
            }

            if (_val < _min)
            {
                buffer = _val - _max + _min - 1;
                while ((buffer > _max) || (buffer < _min))
                {
                    buffer = buffer - _max + _min - 1;
                }
            }
        }

        public void Subtract(int decrement)
        {
            if (_val - decrement >= _min)
            {
                _val -= decrement;
                return;
            }
            int buffer = _val - decrement + _max - _min + 1;
            while ((buffer > _max) || (buffer < _min))
            {
                buffer = buffer + _max - _min + 1;
            }
            _val = buffer;
        }

        public void Add(int increment)
        {
            if (_val + increment <= _max)
            {
                _val += increment;
                return;
            }
            int buffer = _val + increment - _max + _min - 1;
            while ((buffer > _max) || (buffer < _min))
            {
                buffer = buffer - _max + _min - 1;
            }
            _val = buffer;
        }

        public override string ToString()
        {
            return _val.ToString();
        }
    }
}
