using System;
using Moq;
using NodaTime.Testing;

namespace Traces.Testing
{
    public abstract class BaseTest : IDisposable
    {
        protected BaseTest()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
        }

        protected static FakeClock FakeClock => NodaTime.Testing.FakeClock.FromUtc(2000, 01, 01);

        protected MockRepository MockRepository { get; }

        public void Dispose()
        {
            MockRepository.VerifyAll();
        }
    }
}