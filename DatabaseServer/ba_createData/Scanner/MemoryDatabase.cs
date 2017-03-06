using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ba_createData.Scanner
{

    internal class MemoryDatabase
    {

        /// <summary>
        /// Get or set the string
        /// </summary>
        public string TextFile
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set the suffix array
        /// </summary>
        public int[] SuffixArray
        {
            get;
            set;          
        }

        /// <summary>
        /// Get or set the lowest common prefix array
        /// </summary>
        public int[] LcpArray
        {
            get;
            set;
        }


        /// <summary>
        /// Will setup all values and load them into variable
        /// that can be used directly in memory
        /// </summary>
        public MemoryDatabase()
        {
            
        }
    
    }
}
