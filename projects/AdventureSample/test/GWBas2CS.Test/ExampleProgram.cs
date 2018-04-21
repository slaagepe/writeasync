// <copyright file="ExampleProgram.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace GWBas2CS.Test
{
    using FluentAssertions;
    using Xunit;

    public sealed class ExampleProgram
    {
        [Fact]
        public void FullClass()
        {
            const string Input = @"10 REM My first BASIC program
20 PRINT ""HELLO, WORLD!""
30 A$=""a string""
40 A$=""same string""
100 GOTO 20";
            const string Expected = @"using System;
using System.IO;

internal sealed class MyProg
{
    private readonly TextReader input;
    private readonly TextWriter output;
    private string A_s;
    public MyProg(TextReader input, TextWriter output)
    {
        this.input = (input);
        this.output = (output);
    }

    public void Run()
    {
        while (this.Main())
        {
        }
    }

    private void Init()
    {
        A_s = ("""");
    }

    private void PRINT(string expression)
    {
        this.output.WriteLine(expression);
    }

    private bool Main()
    {
        this.Init();
        // My first BASIC program
        L20:
            ;
        PRINT(""HELLO, WORLD!"");
        A_s = (""a string"");
        A_s = (""same string"");
        goto L20;
        return false;
    }
}";

            string actual = Test.Translate("MyProg", Input);

            actual.Should().Be(Expected);
        }
    }
}
