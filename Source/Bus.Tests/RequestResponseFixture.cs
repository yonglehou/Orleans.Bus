﻿using NUnit.Framework;

namespace Orleans.Bus
{
    [TestFixture]
    public class RequestResponseFixture
    {
        IClientMessageBus bus;

        [SetUp]
        public void SetUp()
        {
            bus = MessageBus.Client;
        }

        [Test]
        public void When_sending_command()
        {
            Assert.DoesNotThrow(
                async () => await bus.Send(1, new DoFoo("foo")));
        }

        [Test]
        public async void When_sending_query()
        {
            await bus.Send(1, new DoFoo("foo"));

            var result = await bus.Query<string>(1, new GetFoo());
            Assert.AreEqual("foo-1", result);
        }  
    }
}