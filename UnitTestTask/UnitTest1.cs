using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestTask
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string str = "4562";
            str = str.PadLeft(12,'0');
            str = str.TrimStart('0');
            //OracleSourceDB oracleSourceDB = new OracleSourceDB();
            //string[] vs= new string[2510];
            //for (int i = 0; i < 2510; i++)
            //{
            //    vs[i] = "测试" + i;
            //}
            //oracleSourceDB.GetAllCheckResultByJZK(vs);
            //NeedSynData needSynData = new NeedSynData();
            //needSynData.GetNeedSynData();
        }
    }
}
