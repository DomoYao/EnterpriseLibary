using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enterprises.Framework.EventBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprises.Test.EventBus
{
    [TestClass()]
    public class EventBusTest
    {
        //protected IEventBus EventBus= new Framework.EventBus.EventBus(); 
        protected IEventBus EventBus= Framework.EventBus.EventBus.Default;

        /// <summary>
        /// 简单action 操作
        /// </summary>
        [TestMethod()]
        public void EventBusTest1()
        {
            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    Console.WriteLine(eventData.Value);
                });


            EventBus.Trigger<MySimpleEventData>(null, new MySimpleEventData(1));
        }

        /// <summary>
        /// 简单action 操作
        /// </summary>
        [TestMethod()]
        public void EventBusTest2()
        {
            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    Console.WriteLine("First Time "+eventData.Value);
                });


            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    Console.WriteLine("Second Time "+eventData.Value);
                });

            EventBus.Trigger<MySimpleEventData>(null, new MySimpleEventData(1));
        }


        /// <summary>
        /// Handler 操作
        /// </summary>
        [TestMethod()]
        public void EventBusTest3()
        {
            EventBus.Register(new MySimpleTransientEventHandler());
            EventBus.Trigger<MySimpleEventData>(null, new MySimpleEventData(1));
            EventBus.Trigger(new MySimpleEventData(2));
        }

        /// <summary>
        /// Handler 操作2
        /// </summary>
        [TestMethod()]
        public void Should_Call_Handler_AndDispose()
        {
            EventBus.Register<MySimpleEventData, MySimpleTransientEventHandler>();

            EventBus.Trigger(new MySimpleEventData(1));
            EventBus.Trigger(new MySimpleEventData(2));
            EventBus.Trigger(new MySimpleEventData(3));

            Assert.AreEqual(MySimpleTransientEventHandler.HandleCount, 3);
            Assert.AreEqual(MySimpleTransientEventHandler.DisposeCount, 3);
        }

        /// <summary>
        /// 继承的测试1,父类可以触发子类
        /// </summary>
        [TestMethod()]
        public void Should_Handle_Events_For_Derived_Classes()
        {
            var totalData = 0;

            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                    Assert.AreEqual(this, eventData.EventSource);
                });

            EventBus.Trigger(this, new MySimpleEventData(1)); //Should handle directly registered class
            EventBus.Trigger(this, new MySimpleEventData(2)); //Should handle directly registered class
            EventBus.Trigger(this, new MyDerivedEventData(3)); //Should handle derived class too
            EventBus.Trigger(this, new MyDerivedEventData(4)); //Should handle derived class too

            Assert.AreEqual(10, totalData);
        }

        /// <summary>
        /// 继承的测试2，子类注册不能出发父类
        /// </summary>
        [TestMethod()]
        public void Should_Not_Handle_Events_For_Base_Classes()
        {
            var totalData = 0;

            EventBus.Register<MyDerivedEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                    Assert.AreEqual(this, eventData.EventSource);
                });

            EventBus.Trigger(this, new MySimpleEventData(1)); //Should not handle
            EventBus.Trigger(this, new MySimpleEventData(2)); //Should not handle
            EventBus.Trigger(this, new MyDerivedEventData(3)); //Should handle
            EventBus.Trigger(this, new MyDerivedEventData(4)); //Should handle

            Assert.AreEqual(7, totalData);
        }

    }


    public class MySimpleEventData : EventData
    {
        public int Value { get; set; }

        public MySimpleEventData(int value)
        {
            Value = value;
        }
    }

    public class MySimpleTransientEventHandler : IEventHandler<MySimpleEventData>, IDisposable
    {
        public static int HandleCount { get; set; }

        public static int DisposeCount { get; set; }

        public void HandleEvent(MySimpleEventData eventData)
        {
            ++HandleCount;
            Console.WriteLine("HandleCount="+HandleCount+ " Value=" + eventData.Value);
        }

        public void Dispose()
        {
            ++DisposeCount;
            Console.WriteLine("DisposeCount=" + DisposeCount);
        }
    }


    public class MyDerivedEventData : MySimpleEventData
    {
        public MyDerivedEventData(int value)
            : base(value)
        {
        }
    }
}
