// <copyright file="Division.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace GWExpr.Test
{
    using Xunit;

    public sealed class Division
    {
        [InlineData("1/2", "Div(NumL(1), NumL(2))")]
        [InlineData("X/234", "Div(NumVar(X), NumL(234))")]
        [InlineData("X(234)/YZ1234", "Div(Array(NumVar(X), NumL(234)), NumVar(YZ1234))")]
        [Theory]
        public void Numeric(string input, string output)
        {
            Test.Good(input, output);
        }

        [InlineData("2/\"1\"")]
        [InlineData("234/X$")]
        [InlineData("X(234)/YZ1234$")]
        [Theory]
        public void TypeMismatch(string input)
        {
            Test.Bad(input);
        }

        [InlineData("(1/2)", "Div(NumL(1), NumL(2))")]
        [InlineData("(X/234)", "Div(NumVar(X), NumL(234))")]
        [InlineData("(X(234)/YZ1234)", "Div(Array(NumVar(X), NumL(234)), NumVar(YZ1234))")]
        [Theory]
        public void WithParens(string input, string output)
        {
            Test.Good(input, output);
        }

        [InlineData("1/2/3", "Div(Div(NumL(1), NumL(2)), NumL(3))")]
        [InlineData("(1/2/3)", "Div(Div(NumL(1), NumL(2)), NumL(3))")]
        [InlineData("(1/2)/3", "Div(Div(NumL(1), NumL(2)), NumL(3))")]
        [InlineData("1/(2/3)", "Div(NumL(1), Div(NumL(2), NumL(3)))")]
        [Theory]
        public void WithThreeTerms(string input, string output)
        {
            Test.Good(input, output);
        }
    }
}
