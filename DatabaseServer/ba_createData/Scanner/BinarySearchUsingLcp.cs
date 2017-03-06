using System;

namespace ba_createData.Scanner
{
    public class BinarySearchUsingLcp
    {
        private int[] _mLcp;
        /// <summary>
        /// 
        /// </summary>
        public int[] BuildLcpArray(int[] mSa, string mStr)
        {
            _mLcp = new int[mSa.Length + 1];
            _mLcp[0] = _mLcp[mSa.Length] = 0;

            for (int i = 1; i < mSa.Length; i++)
            {
                _mLcp[i] = CalcLcp(mSa[i - 1], mSa[i], mStr);
            }
            return _mLcp;
        }

        private static int CalcLcp(int i, int j, string mStr)
        {
            int lcp;
            int maxIndex = mStr.Length - Math.Max(i, j); // Out of bounds prevention
            for (lcp = 0; (lcp < maxIndex) && (mStr[i + lcp] == mStr[j + lcp]); lcp++)
            {
            }
            return lcp;
        }

        // It works assuming you have builded the concatenated string and
        // computed the suffix and the lcp arrays
        // text.length() ---> tlen
        // pattern.length() ---> plen
        // concatenated string: str

        private bool BinarySearchLcp(string mStr, int[] sa, int[] lcp, string pattern)
        {
            var plen = pattern.Length;
            var tlen = mStr.Length;
            var total = tlen + plen;
            var pos = -1;
            for (var i = 0; i < total; ++i)
                if (total - sa[i] == plen)
                { pos = i; break; }
            if (pos == -1) return false;
            int hi;
            var lo = hi = pos;
            while (lo - 1 >= 0 && lcp[lo - 1] >= plen) lo--;
            while (hi + 1 < tlen && lcp[hi] >= plen) hi++;
            for (var i = lo; i <= hi; ++i)
                if (total - sa[i] >= 2 * plen)
                    return true;
            return false;
        }
    }
}
