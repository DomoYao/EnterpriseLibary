using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace Enterprises.Tests.Dependency
{
    [TestClass()]
    public class IocManager_Tests : TestBaseWithLocalIocManager
    {
        public IocManager_Tests()
        {
            LocalIocManager.Register<IEmpty, EmptyImplOne>();
            LocalIocManager.Register<IEmpty, EmptyImplTwo>();
        }

        [TestMethod]
        public void Should_Get_First_Registered_Class_If_Registered_Multiple_Class_For_Same_Interface()
        {
            Assert.AreEqual(LocalIocManager.Resolve<IEmpty>().GetType(),typeof (EmptyImplOne));
        }

        [TestMethod]
        public void ResolveAll_Test()
        {
            var instances = LocalIocManager.ResolveAll<IEmpty>();
            Assert.AreEqual(instances.Length,2);
            Assert.AreEqual(instances.Any(i => i.GetType() == typeof(EmptyImplOne)),true);
            Assert.AreEqual(instances.Any(i => i.GetType() == typeof(EmptyImplTwo)), true);
        }

        public interface IEmpty
        {
            
        }

        public class EmptyImplOne : IEmpty
        {
            
        }

        public class EmptyImplTwo : IEmpty
        {

        }
    }
}