using System;
using Enterprises.Framework.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprises.Test.Utility
{
    [TestClass]
    public class MapHelperTest
    {
       
        [TestMethod]
        public void IsJsonTest()
        {
            var dis = MapHelper.GetPositionDistance(30.274919135921625, 120.12170905921825, 30.27428, 120.12281);

            Console.WriteLine(dis);
        }
    }
}
