using System;
using System.Collections.Generic;
using System.Text;

namespace ba_createData
{
    namespace ba_createData
    {
        public class Bwt
        {
            public Bwt(string text)
            {

                var bufferIn = Encoding.UTF8.GetBytes(text);
                var bufferOut = new byte[bufferIn.Length];
                var bufferDecode = new byte[bufferIn.Length];

                var bwt = new BwtImplementation();

                var primaryIndex = 0;
                bwt.bwt_encode(bufferIn, bufferOut, bufferIn.Length, ref primaryIndex);
                bwt.bwt_decode(bufferOut, bufferDecode, bufferIn.Length, primaryIndex);

                Console.WriteLine(@"Decoded string: {0}", Encoding.UTF8.GetString(bufferDecode));
            }
        }


        class BwtImplementation
        {
            public void bwt_encode(byte[] bufIn, byte[] bufOut, int size, ref int primaryIndex)
            {
                int[] indices = new int[size];
                for (int i = 0; i < size; i++)
                    indices[i] = i;

                Array.Sort(indices, 0, size, new BwtComparator(bufIn, size));

                for (int i = 0; i < size; i++)
                    bufOut[i] = bufIn[(indices[i] + size - 1) % size];

                for (int i = 0; i < size; i++)
                {
                    if (indices[i] == 1)
                    {
                        primaryIndex = i;
                        return;
                    }
                }
            }

            public void bwt_decode(byte[] bufEncoded, byte[] bufDecoded, int size, int primaryIndex)
            {
                byte[] f = new byte[size];
                int[] buckets = new int[0x100];
                int[] indices = new int[size];

                for (int i = 0; i < 0x100; i++)
                    buckets[i] = 0;

                for (int i = 0; i < size; i++)
                    buckets[bufEncoded[i]]++;

                for (int i = 0, k = 0; i < 0x100; i++)
                {
                    for (int j = 0; j < buckets[i]; j++)
                    {
                        f[k++] = (byte)i;
                    }
                }

                for (int i = 0, j = 0; i < 0x100; i++)
                {
                    while (i > f[j] && j < size - 1)
                    {
                        j++;
                    }
                    buckets[i] = j;
                }

                for (int i = 0; i < size; i++)
                    indices[buckets[bufEncoded[i]]++] = i;

                for (int i = 0, j = primaryIndex; i < size; i++)
                {
                    bufDecoded[i] = bufEncoded[j];
                    j = indices[j];
                }
            }
        }

        internal class BwtComparator : IComparer<int>
        {
            private readonly byte[] _rotlexcmpBuf;
            private readonly int _rottexcmpBufsize;

            public BwtComparator(byte[] array, int size)
            {
                _rotlexcmpBuf = array;
                _rottexcmpBufsize = size;
            }

            public int Compare(int li, int ri)
            {
                int ac = _rottexcmpBufsize;
                while (_rotlexcmpBuf[li] == _rotlexcmpBuf[ri])
                {
                    if (++li == _rottexcmpBufsize)
                        li = 0;
                    if (++ri == _rottexcmpBufsize)
                        ri = 0;
                    if (--ac <= 0)
                        return 0;
                }
                if (_rotlexcmpBuf[li] > _rotlexcmpBuf[ri])
                    return 1;

                return -1;
            }
        }
    }
}
