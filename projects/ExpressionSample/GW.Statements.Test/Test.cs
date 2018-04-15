// <copyright file="Test.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace GW.Statements.Test
{
    using System;
    using FluentAssertions;

    internal static class Test
    {
        public static void Good(string input, string output)
        {
            BasicStatement.FromString(input).ToString().Should().Be(output, "[{0}]", input);
        }

        public static void Bad(string input)
        {
            Action act = () => BasicStatement.FromString(input);

            act.Should().Throw<FormatException>("[{0}]", input).WithMessage("*'" + input + "'*").WithInnerException<Exception>();
        }
    }
}
