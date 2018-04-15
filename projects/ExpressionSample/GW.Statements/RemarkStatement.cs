﻿// <copyright file="RemarkStatement.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace GW.Statements
{
    internal sealed class RemarkStatement : BasicStatement
    {
        private readonly string text;

        public RemarkStatement(string text)
        {
            this.text = text;
        }

        public override string ToString() => "Rem(\"" + this.text + "\")";
    }
}