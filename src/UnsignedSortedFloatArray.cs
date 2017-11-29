using System.Linq;

namespace TexasHoldEm
{
    /// <summary>
    /// Creates float[] values and int[] index which is are a decending-sorted float array and its corresponding indexes respectively.
    /// </summary>
    class UnsignedSortedFloatArray
    {
        public int[] _index;
        public float[] _values;

        public UnsignedSortedFloatArray(float[] args)
        {
            _index = new int[args.Length];
            _values = args;
            for (int i = 0; i < args.Length; i++)
            {
                _index[i] = i;
            }

            USFA_Sort();
        }

        private void USFA_Sort()
        {
            float[] buffer = new float[_values.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = _values[i];

            } //Need to assign based on values since array assignment operator makes arrays point to the same memory and thus not unique.

            //sort arrays
            for (int k = 0; k < _values.Length; k++)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    float temp = buffer.Max();
                    if (buffer[i] == temp)
                    {
                        _values[k] = buffer.Max();
                        buffer[i] = -1;
                        _index[k] = i;
                        break;
                    }
                }
            }
        }

    }

}
