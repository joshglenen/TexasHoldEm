using System;

namespace TexasHoldEm
{
    //TODO: thorough test for bugs
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

        //Operations within the class
        public bool Compare(CircularInt var)
        {
            if ((_val == var._val) && (_max == var._max) && (_min == var._min)) return true;
            return false;
        }
        public void Multiply(int multiplier)
        {
            _val *= multiplier;

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
        public void Divide(int divisor)
        {
             _val = Convert.ToInt32(_val / divisor);
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
        public void Multiply(double multiplier)
        {
            _val = Convert.ToInt32((double)_val * multiplier);

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
        public void Divide(double divisor)
        {
            _val = Convert.ToInt32((double)_val / divisor);

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
            }
            int buffer = _val + increment - _max + _min - 1;
            while ((buffer > _max) || (buffer < _min))
            {
                buffer = buffer - _max + _min - 1;
            }
            _val = buffer;
        }

        //Operators outside the class
        public static int operator *(CircularInt left, CircularInt right)
        {
            return left._val*right._val;
        }
        public static double operator /(CircularInt left, CircularInt right)
        {
            return left._val / right._val;
        }
        public static bool operator >(CircularInt left, CircularInt right)
        {
            if (left._val > right._val)
            {
                return true;
            }
            return false;
        }
        public static bool operator <(CircularInt left, CircularInt right)
        {
            if (left._val < right._val)
            {
                return true;
            }
            return false;
        }
        public static bool operator >=(CircularInt left, CircularInt right)
        {
            if (left._val >= right._val)
            {
                return true;
            }
            return false;
        }
        public static bool operator <=(CircularInt left, CircularInt right)
        {
            if (left._val <= right._val)
            {
                return true;
            }
            return false;
        }
        public static CircularInt operator -(CircularInt left, int decrement)
        {
            if (left._val - decrement >= left._max)
            {
                return new CircularInt(left._min, left._max, left._val - decrement);
            }
            int buffer = left._val - decrement + left._max - left._min + 1;
            while ((buffer > left._max) || (buffer < left._min))
            {
                buffer = buffer + left._max - left._min + 1;
            }
            return new CircularInt(left._min, left._max, buffer);
        }
        public static CircularInt operator +(CircularInt left, int increment)
        {
            if(left._val + increment <= left._max)
            {
                return new CircularInt(left._min, left._max, left._val + increment);
            }
            int buffer = left._val + increment - left._max + left._min -1;
            while ((buffer > left._max) || (buffer < left._min))
            {
                buffer = buffer - left._max + left._min -1;
            }
            return new CircularInt(left._min, left._max, buffer);
        }
    }
}
