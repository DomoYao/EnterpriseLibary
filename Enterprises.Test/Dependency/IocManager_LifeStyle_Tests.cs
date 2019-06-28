using Castle.MicroKernel.Registration;
using Enterprises.Framework.Dependency;
using Enterprises.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Abp.Tests.Dependency
{
    [TestClass()]
    public class IocManager_LifeStyle_Tests : TestBaseWithLocalIocManager
    {
        [TestMethod]
        public void Should_Call_Dispose_Of_Transient_Dependency_When_Object_Is_Released()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<SimpleDisposableObject>().LifestyleTransient()
                );

            var obj = LocalIocManager.IocContainer.Resolve<SimpleDisposableObject>();

            LocalIocManager.IocContainer.Release(obj);

            Assert.AreEqual(obj.DisposeCount,1);
        }

        [TestMethod]
        public void Should_Call_Dispose_Of_Transient_Dependency_When_IocManager_Is_Disposed()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<SimpleDisposableObject>().LifestyleTransient()
                );

            var obj = LocalIocManager.IocContainer.Resolve<SimpleDisposableObject>();

            LocalIocManager.Dispose();

            Assert.AreEqual(obj.DisposeCount, 1);
        }

        [TestMethod]
        public void Should_Call_Dispose_Of_Singleton_Dependency_When_IocManager_Is_Disposed()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<SimpleDisposableObject>().LifestyleSingleton()
                );

            var obj = LocalIocManager.IocContainer.Resolve<SimpleDisposableObject>();

            LocalIocManager.Dispose();

            Assert.AreEqual(obj.DisposeCount, 1);
        }
    }


    public class SimpleDisposableObject : IDisposable
    {
        public int MyData { get; set; }

        public int DisposeCount { get; set; }

        public SimpleDisposableObject()
        {

        }

        public SimpleDisposableObject(int myData)
        {
            MyData = myData;
        }

        public void Dispose()
        {
            DisposeCount++;
        }

        public int GetMyData()
        {
            return MyData;
        }
    }
}
