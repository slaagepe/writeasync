﻿// <copyright file="IfThen.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace GWParse.Test.Statements
{
    using Xunit;

    public sealed class IfThen
    {
        [InlineData("IF 1 THEN 2", "If(NumL(1), Goto(2))")]
        [InlineData("IF X THEN 34", "If(NumV(X), Goto(34))")]
        [InlineData("IF X=1 THEN 56789", "If(Eq(NumV(X), NumL(1)), Goto(56789))")]
        [InlineData("IF A$>\"x\" AND B=1 THEN 56789", "If(And(Gt(StrV(A), StrL(\"x\")), Eq(NumV(B), NumL(1))), Goto(56789))")]
        [Theory]
        public void ValidGoto(string input, string output)
        {
            Test.Good(input, output);
        }

        [InlineData("if 1 then 2", "If(NumL(1), Goto(2))")]
        [InlineData("If 1 TheN 2", "If(NumL(1), Goto(2))")]
        [Theory]
        public void IgnoreCase(string input, string output)
        {
            Test.Good(input, output);
        }

        [InlineData(" IF 1 THEN 2", "If(NumL(1), Goto(2))")]
        [InlineData("IF 1 THEN 2 ", "If(NumL(1), Goto(2))")]
        [InlineData("  IF  1 THEN  2  ", "If(NumL(1), Goto(2))")]
        [Theory]
        public void AllowSpaces(string input, string output)
        {
            Test.Good(input, output);
        }

        [InlineData("IFA")]
        [InlineData("IF1")]
        [InlineData("IF 1")]
        [InlineData("IF 1 THEN")]
        [InlineData("IF THEN")]
        [InlineData("IF 1THEN")]
        [InlineData("IF 1THEN2")]
        [InlineData("IF 1THEN 2")]
        [InlineData("IF 1 THEN2")]
        [Theory]
        public void Invalid(string input)
        {
            Test.Bad(input);
        }
    }
}
