using Castle.MicroKernel.Registration;
using Enterprises.Framework.Dependency;
using Enterprises.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Abp.Tests.Dependency
{
    /// <summary>
    /// 属性注入
    /// </summary>
    [TestClass()]
    public class PropertyInjection_Tests : TestBaseWithLocalIocManager
    {
        [TestMethod]
        public void Should_Inject_Session_For_ApplicationService()
        {
            var session = new AbpSession() { UserId =42};

            LocalIocManager.Register<MyApplicationService>();
            LocalIocManager.IocContainer.Register(
                Component.For<IAbpSession>().Instance(session)
                );

            var myAppService = LocalIocManager.Resolve<MyApplicationService>();
            myAppService.TestSession();
        }

        private class MyApplicationService : ApplicationService
        {
            public void TestSession()
            {
                Assert.IsNotNull(AbpSession);
                Assert.AreEqual(AbpSession.UserId, 42);
            }
        }

        public abstract class ApplicationService
        {
            public IAbpSession AbpSession { get; set; }
        }

        public interface IAbpSession
        {
            long? UserId { get; set; }
        }

        public class AbpSession:IAbpSession
        {
           public  long? UserId { get; set; }
        }
    }
}
