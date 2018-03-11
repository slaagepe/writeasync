// <copyright file="ExpressionTest.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace Four4.Test
{
    using FluentAssertions;
    using Xunit;

    public sealed class ExpressionTest
    {
        [Fact]
        public void Empty()
        {
            Expression expr = default(Expression);

            expr.ToString().Should().Be(string.Empty);
        }

        [Theory]
        [InlineData("+", "+")]
        [InlineData("-", "-")]
        [InlineData("!", "!")]
        [InlineData("4", "4")]
        public void AppendToEmpty(string input, string result)
        {
            Expression expr = default(Expression);

            TestAppend(expr, input, result);
        }

        [Theory]
        [InlineData("+", "4 +")]
        [InlineData("-", "4 -")]
        [InlineData("!", "4 !")]
        [InlineData("4", "4 4")]
        public void AppendToOne(string input, string result)
        {
            Expression expr = default(Expression).Append("4");

            TestAppend(expr, input, result);
        }

        [Theory]
        [InlineData("+", "44 4 +")]
        [InlineData("-", "44 4 -")]
        [InlineData("!", "44 4 !")]
        [InlineData("4", "44 4 4")]
        public void AppendToTwo(string input, string result)
        {
            Expression expr = default(Expression).Append("44").Append("4");

            TestAppend(expr, input, result);
        }

        [Theory]
        [InlineData("4444", ".4")]
        [InlineData("4444", ".4_")]
        [InlineData("4444", "4")]
        [InlineData("444", "44")]
        [InlineData("44", "444")]
        [InlineData("4", "4444")]
        public void AppendOneTooManyDigits(string x, string y)
        {
            Expression expr = default(Expression).Append(x);

            expr = expr.Append(y);

            expr.IsInRange.Should().BeFalse();
        }

        private static void TestAppend(Expression expr, string input, string result)
        {
            expr.Append(input).ToString().Should().Be(result);
        }
    }
}
