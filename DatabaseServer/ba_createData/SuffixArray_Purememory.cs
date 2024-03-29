﻿/*
 Copyright (c) 2012 Eran Meir
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.


 https://github.com/eranmeir/Sufa-Suffix-Array-Csharp d. 11-11-16
*/

using System.Linq;

namespace ba_createData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using C5;

    [Serializable]
    public class SuffixArray
    {
        private const int Eoc = int.MaxValue;
        public int[] _mSa;
        private readonly int[] _mIsa;
        public int[] _mLcp;
        private readonly HashDictionary<char, int> _mChainHeadsDict = new HashDictionary<char, int>(new CharComparer());
        private readonly List<Chain> _mChainStack = new List<Chain>();
        private readonly ArrayList<Chain> _mSubChains = new ArrayList<Chain>();
        private int _mNextRank = 1;
        private readonly string _mStr;
        public string StringRepresentation => GetString(_mSa);

        private void CleanSuffixArray()
        {
            this._mSa = _mSa.Where(index => _mStr.Length - 1 != index && index + 32 <= _mStr.Length - 1 && _mStr[index + 32].Equals('$')).ToArray();
        }

        private static string GetString(IEnumerable<int> bytes)
        {
            var newString = new StringBuilder();
            foreach (var str in bytes)
            {
                newString.Append(str + "; ");
            }
            return newString.ToString();

        }


        /// 
        /// <summary>
        /// Build a suffix array from string str
        /// </summary>
        /// <param name="str">A string for which to build a suffix array with LCP information</param>
        public SuffixArray(string str) : this(str, true) { }

        /// 
        /// <summary>
        /// Build a suffix array from string str
        /// </summary>
        /// <param name="str">A string for which to build a suffix array</param>
        /// <param name="buildLcps">Also calculate LCP information</param>
        public SuffixArray(string str, bool buildLcps)
        {
            _mStr = str;
            _mSa = new int[_mStr.Length];
            _mIsa = new int[_mStr.Length];

            FormInitialChains();
            BuildSuffixArray();
            CleanSuffixArray();
            if (buildLcps)
                BuildLcpArray();
        }

        /// <summary>
        /// Find the index of a substring 
        /// </summary>
        /// <param name="substr">Substring to look for</param>
        /// <returns>First index in the original string. -1 if not found</returns>
        public int IndexOf(string substr)
        {
            var l = 0;
            var r = _mSa.Length;

            if ((substr == null) || (substr.Length == 0))
            {
                return -1;
            }

            // Binary search for substring
            while (r > l)
            {
                var m = (l + r) / 2;
                if (_mStr.Substring(_mSa[m]).CompareTo(substr) < 0)
                {
                    l = m + 1;
                }
                else
                {
                    r = m;
                }
            }
            if ((l == r) && (l < _mStr.Length) && (_mStr.Substring(_mSa[l]).StartsWith(substr)))
            {
                return _mSa[l];
            }
            else
            {
                return -1;
            }
        }


        /// <summary>
        /// Link all suffixes that have the same first character
        /// </summary>
        private void FormInitialChains()
        {
            FindInitialChains();
            SortAndPushSubchains();
        }


        /// <summary>
        /// Scan the string left to right, keeping rightmost occurences of characters as the chain heads
        /// </summary>
        private void FindInitialChains()
        {
            for (var i = 0; i < _mStr.Length; i++)
            {
                if (_mChainHeadsDict.Contains(_mStr[i]))
                {
                    _mIsa[i] = _mChainHeadsDict[_mStr[i]];
                }
                else
                {
                    _mIsa[i] = Eoc;
                }
                _mChainHeadsDict[_mStr[i]] = i;
            }

            // Prepare chains to be pushed to stack
            foreach (var headIndex in _mChainHeadsDict.Values)
            {
                var newChain = new Chain(_mStr)
                {
                    Head = headIndex,
                    Length = 1
                };
                _mSubChains.Add(newChain);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SortAndPushSubchains()
        {
            _mSubChains.Sort();
            for (int i = _mSubChains.Count - 1; i >= 0; i--)
            {
                _mChainStack.Add(_mSubChains[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BuildSuffixArray()
        {
            while (_mChainStack.Count > 0)
            {
                // Pop chain
                var chain = _mChainStack[_mChainStack.Count - 1];
                _mChainStack.RemoveAt(_mChainStack.Count - 1);

                if (_mIsa[chain.Head] == Eoc)
                {
                    // Singleton (A chain that contain only 1 suffix)
                    RankSuffix(chain.Head);
                }
                else
                {
                    //RefineChains(chain);
                    RefineChainWithInductionSorting(chain);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chain"></param>
        private void RefineChains(Chain chain)
        {
            _mChainHeadsDict.Clear();
            _mSubChains.Clear();
            while (chain.Head != Eoc)
            {
                int nextIndex = _mIsa[chain.Head];
                if (chain.Head + chain.Length > _mStr.Length - 1)
                {
                    RankSuffix(chain.Head);
                }
                else
                {
                    ExtendChain(chain);
                }
                chain.Head = nextIndex;
            }
            // Keep stack sorted
            SortAndPushSubchains();
        }

        private void ExtendChain(Chain chain)
        {
            var sym = _mStr[chain.Head + chain.Length];
            if (_mChainHeadsDict.Contains(sym))
            {
                // Continuation of an existing chain, this is the leftmost
                // occurence currently known (others may come up later)
                _mIsa[_mChainHeadsDict[sym]] = chain.Head;
                _mIsa[chain.Head] = Eoc;
            }
            else
            {
                // This is the beginning of a new subchain
                _mIsa[chain.Head] = Eoc;
                var newChain = new Chain(_mStr)
                {
                    Head = chain.Head,
                    Length = chain.Length + 1
                };
                _mSubChains.Add(newChain);
            }
            // Save index in case we find a continuation of this chain
            _mChainHeadsDict[sym] = chain.Head;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chain"></param>
        private void RefineChainWithInductionSorting(Chain chain)
        {
            var notedSuffixes = new ArrayList<SuffixRank>();
            _mChainHeadsDict.Clear();
            _mSubChains.Clear();

            while (chain.Head != Eoc)
            {
                var nextIndex = _mIsa[chain.Head];
                if (chain.Head + chain.Length > _mStr.Length - 1)
                {
                    // If this substring reaches end of string it cannot be extended.
                    // At this point it's the first in lexicographic order so it's safe
                    // to just go ahead and rank it.
                    RankSuffix(chain.Head);
                }
                else if (_mIsa[chain.Head + chain.Length] < 0)
                {
                    var sr = new SuffixRank
                    {
                        Head = chain.Head,
                        Rank = -_mIsa[chain.Head + chain.Length]
                    };
                    notedSuffixes.Add(sr);
                }
                else
                {
                    ExtendChain(chain);
                }
                chain.Head = nextIndex;
            }
            // Keep stack sorted
            SortAndPushSubchains();
            SortAndRankNotedSuffixes(notedSuffixes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="notedSuffixes"></param>
        private void SortAndRankNotedSuffixes(C5.IList<SuffixRank> notedSuffixes)
        {
            notedSuffixes.Sort(new SuffixRankComparer());
            // Rank sorted noted suffixes 
            for (var i = 0; i < notedSuffixes.Count; ++i)
            {
                RankSuffix(notedSuffixes[i].Head);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void RankSuffix(int index)
        {
            // We use the ISA to hold both ranks and chain links, so we differentiate by setting
            // the sign.
            _mIsa[index] = -_mNextRank;
            _mSa[_mNextRank - 1] = index;
            _mNextRank++;
        }

        /// <summary>
        /// 
        /// </summary>
        private void BuildLcpArray()
        {
            _mLcp = new int[_mSa.Length + 1];
            _mLcp[0] = _mLcp[_mSa.Length] = 0;

            for (int i = 1; i < _mSa.Length; i++)
            {
                _mLcp[i] = CalcLcp(_mSa[i - 1], _mSa[i]);
            }
        }

        private int CalcLcp(int i, int j)
        {
            int lcp;
            int maxIndex = _mStr.Length - Math.Max(i, j); // Out of bounds prevention
            for (lcp = 0; (lcp < maxIndex) && (_mStr[i + lcp] == _mStr[j + lcp]); lcp++)
            {
            }
            return lcp;
        }

    }

    #region HelperClasses
    [Serializable]
    internal class Chain : IComparable<Chain>
    {
        public int Head;
        public int Length;
        private readonly string _mStr;

        public Chain(string str)
        {
            _mStr = str;
        }

        public int CompareTo(Chain other)
        {
            return _mStr.Substring(Head, Length).CompareTo(_mStr.Substring(other.Head, other.Length));
        }

        public override string ToString()
        {
            return _mStr.Substring(Head, Length);
        }
    }

    [Serializable]
    internal class CharComparer : System.Collections.Generic.EqualityComparer<char>
    {
        public override bool Equals(char x, char y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(char obj)
        {
            return obj.GetHashCode();
        }
    }

    [Serializable]
    internal struct SuffixRank
    {
        public int Head;
        public int Rank;
    }

    [Serializable]
    internal class SuffixRankComparer : IComparer<SuffixRank>
    {
        public bool Equals(SuffixRank x, SuffixRank y)
        {
            return x.Rank.Equals(y.Rank);
        }

        public int Compare(SuffixRank x, SuffixRank y)
        {
            return x.Rank.CompareTo(y.Rank);
        }
    }
    #endregion
}
