using System;
using Xunit;

namespace Gifter.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Two_Numbers_Should_Equal()
        {
            var num1 = 100;
            var num2 = 100;

            Assert.Equal(num1, num2);
        }
    }
}
