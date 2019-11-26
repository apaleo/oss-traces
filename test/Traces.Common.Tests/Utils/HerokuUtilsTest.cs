using FluentAssertions;
using Traces.Common.Utils;
using Traces.Testing;
using Xunit;

namespace Traces.Common.Tests.Utils
{
    public class HerokuUtilsTest : BaseTest
    {
        [Fact]
        public void ConvertConnectionStringIfSet()
        {
            HerokuUtils
                .ConvertConnectionStringIfSet("postgres://user:pw@host.name.local:5432/dbName")
                .Should().Be("Host=host.name.local;Port=5432;Database=dbName;Username=user;Password=pw");
        }
    }
}