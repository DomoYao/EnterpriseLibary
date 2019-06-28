

using Castle.MicroKernel.Registration;
using Enterprises.Framework.Dependency;
using Enterprises.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Abp.Tests.Dependency
{
    [TestClass()]
    public class IocManager_Override_Tests : TestBaseWithLocalIocManager
    {
        [TestMethod]
        public void Should_Not_Override_As_Default()
        {
            //Arrange
            LocalIocManager.Register<IMyService, MyImpl1>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<IMyService, MyImpl2>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<IMyService, MyImpl3>(DependencyLifeStyle.Transient);

            //Act
            var service = LocalIocManager.Resolve<IMyService>();
            var allServices = LocalIocManager.IocContainer.ResolveAll<IMyService>();

            //Assert
            Assert.IsInstanceOfType(service,typeof(MyImpl1));
            Assert.AreEqual(allServices.Length,3);
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl1)));
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl2)));
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl3)));
        }

        [TestMethod]
        public void Should_Override_When_Using_IsDefault()
        {
            //Arrange
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl1>().LifestyleTransient());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl2>().LifestyleTransient().IsDefault());

            //Act
            var service = LocalIocManager.Resolve<IMyService>();
            var allServices = LocalIocManager.IocContainer.ResolveAll<IMyService>();

            //Assert
            Assert.IsInstanceOfType(service, typeof(MyImpl2));
            Assert.AreEqual(allServices.Length, 2);
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl1)));
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl2)));
        }

        [TestMethod]
        public void Should_Override_When_Using_IsDefault_Twice()
        {
            //Arrange
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl1>().LifestyleTransient());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl2>().LifestyleTransient().IsDefault());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl3>().LifestyleTransient().IsDefault());

            //Act
            var service = LocalIocManager.Resolve<IMyService>();
            var allServices = LocalIocManager.IocContainer.ResolveAll<IMyService>();

            //Assert
            Assert.IsInstanceOfType(service, typeof(MyImpl3));
            Assert.AreEqual(allServices.Length, 3);
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl1)));
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl2)));
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl3)));
        }

        [TestMethod]
        public void Should_Get_Default_Service()
        {
            //Arrange
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl1>().LifestyleTransient());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl2>().LifestyleTransient().IsDefault());
            LocalIocManager.IocContainer.Register(Component.For<IMyService>().ImplementedBy<MyImpl3>().LifestyleTransient());

            //Act
            var service = LocalIocManager.Resolve<IMyService>();
            var allServices = LocalIocManager.IocContainer.ResolveAll<IMyService>();

            //Assert
            Assert.IsInstanceOfType(service, typeof(MyImpl2));
            Assert.AreEqual(allServices.Length, 3);
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl1)));
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl2)));
            Assert.IsTrue(allServices.Any(s => s.GetType() == typeof(MyImpl3)));
        }

        public class MyImpl1 : IMyService
        {
            
        }

        public class MyImpl2 : IMyService
        {

        }

        public class MyImpl3 : IMyService
        {

        }

        public interface IMyService
        {
        }
    }
}
