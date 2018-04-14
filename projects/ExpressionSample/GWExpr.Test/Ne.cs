// <copyright file="Ne.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace GWExpr.Test
{
    using Xunit;

    public sealed class Ne
    {
        [InlineData("1<>2", "Ne(NumL(1), NumL(2))")]
        [InlineData("X<>234", "Ne(NumVar(X), NumL(234))")]
        [InlineData("X(234)<>YZ1234", "Ne(Array(NumVar(X), NumL(234)), NumVar(YZ1234))")]
        [Theory]
        public void Numeric(string input, string output)
        {
            Test.Good(input, output);
        }

        [InlineData("\"one\"<>\"two\"", "Ne(StrL(one), StrL(two))")]
        [InlineData("X$<>\"abc\"", "Ne(StrVar(X), StrL(abc))")]
        [InlineData("X$(234)<>YZ1234$", "Ne(Array(StrVar(X), NumL(234)), StrVar(YZ1234))")]
        [Theory]
        public void String(string input, string output)
        {
            Test.Good(input, output);
        }

        [InlineData("2<>\"1\"")]
        [InlineData("234<>X$")]
        [InlineData("X(234)<>YZ1234$")]
        [InlineData("A$<>B$<>\"C\"")]
        [Theory]
        public void TypeMismatch(string input)
        {
            Test.Bad(input);
        }

        [InlineData("(1<>2)", "Ne(NumL(1), NumL(2))")]
        [InlineData("(X<>234)", "Ne(NumVar(X), NumL(234))")]
        [InlineData("(X(234)<>YZ1234)", "Ne(Array(NumVar(X), NumL(234)), NumVar(YZ1234))")]
        [Theory]
        public void WithParens(string input, string output)
        {
            Test.Good(input, output);
        }

        [InlineData("1<>2<>3", "Ne(Ne(NumL(1), NumL(2)), NumL(3))")]
        [InlineData("(1<>2<>3)", "Ne(Ne(NumL(1), NumL(2)), NumL(3))")]
        [InlineData("(1<>2)<>3", "Ne(Ne(NumL(1), NumL(2)), NumL(3))")]
        [InlineData("1<>(2<>3)", "Ne(NumL(1), Ne(NumL(2), NumL(3)))")]
        [Theory]
        public void WithThreeTerms(string input, string output)
        {
            Test.Good(input, output);
        }

        [InlineData("(Z+1)<>(X-Y)", "Ne(Add(NumVar(Z), NumL(1)), Sub(NumVar(X), NumVar(Y)))")]
        [InlineData("Z+1<>X-Y", "Ne(Add(NumVar(Z), NumL(1)), Sub(NumVar(X), NumVar(Y)))")]
        [InlineData("(Z*1)<>(X^Y)", "Ne(Mult(NumVar(Z), NumL(1)), Pow(NumVar(X), NumVar(Y)))")]
        [InlineData("Z*1<>X^Y", "Ne(Mult(NumVar(Z), NumL(1)), Pow(NumVar(X), NumVar(Y)))")]
        [Theory]
        public void WithOtherOperations(string input, string output)
        {
            Test.Good(input, output);
        }
    }
}
