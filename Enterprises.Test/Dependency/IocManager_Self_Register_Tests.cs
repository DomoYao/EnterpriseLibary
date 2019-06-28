

using Enterprises.Framework.Dependency;
using Enterprises.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Abp.Tests.Dependency
{
    [TestClass()]
    public class IocManager_Self_Register_Tests : TestBaseWithLocalIocManager
    {
        [TestMethod]
        public void Should_Self_Register_With_All_Interfaces()
        {
            var registrar = LocalIocManager.Resolve<IIocRegistrar>();
            var resolver = LocalIocManager.Resolve<IIocResolver>();
            var managerByInterface = LocalIocManager.Resolve<IIocManager>();
            var managerByClass = LocalIocManager.Resolve<IocManager>();

            Assert.AreEqual(managerByClass,registrar);
            Assert.AreEqual(managerByClass,resolver);
            Assert.AreEqual(managerByClass,managerByInterface);
        }
    }
}