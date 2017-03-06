using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ca_createDataTest
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            // 2d75cc1bf8e57872781f9cd04a529256$
            // 2d75c385e296b522b74a82aca8b27f5b$
            // 28e3146742380f9cab6773c61b496832$
            // 28e32fce24aa6fe6ecf816b92c8197d4$
            // 2d75cc10959467729007f87593063663$

            var text = "28e32fce24aa6fe6ecf816b92c8197d4$2d75cc10959467729007f87593063663$28e3146742380f9cab6773c61b496832$2d75c385e296b522b74a82aca8b27f5b$2d75cc1bf8e57872781f9cd04a529256$";
            var suffixArray = new ba_createData.SuffixArray(text, true);
            var lowestCommonPrefix = suffixArray._mLcp;
            var array = suffixArray._mSa;
        }
    }
}
