﻿// <copyright file="BasicVisitor.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace GWBas2CS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GWParse.Expressions;
    using GWParse.Statements;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    internal sealed class BasicVisitor : ILineVisitor, IStatementVisitor
    {
        private readonly string name;
        private readonly SyntaxGenerator generator;
        private readonly Methods methods;
        private readonly Lines lines;
        private readonly Variables vars;
        private readonly DataValues data;

        private int lineNumber;

        public BasicVisitor(string name)
        {
            this.name = name;
            this.generator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);
            this.lines = new Lines(this.generator);
            this.methods = new Methods();
            this.vars = new Variables(this.generator);
            this.data = new DataValues(this.generator);
        }

        public override string ToString()
        {
            List<SyntaxNode> declarations = new List<SyntaxNode>();

            this.Usings(declarations);

            this.Class(declarations);

            return this.generator.CompilationUnit(declarations).NormalizeWhitespace().ToString();
        }

        public void Line(int number, BasicStatement[] list)
        {
            this.lineNumber = number;
            foreach (BasicStatement stmt in list)
            {
                stmt.Accept(this);
            }
        }

        public void Assign(BasicExpression left, BasicExpression right)
        {
            ExpressionNode expr = new ExpressionNode(this.generator, this.vars, this.methods);
            left.Accept(expr);
            SyntaxNode lval = expr.Value;
            right.Accept(expr);
            SyntaxNode rval = expr.Value;
            this.lines.Add(this.lineNumber, this.generator.AssignmentStatement(lval, rval));
        }

        public void For(BasicExpression v, BasicExpression start, BasicExpression end, BasicExpression step)
        {
            ExpressionNode expr = new ExpressionNode(this.generator, this.vars, this.methods);
            v.Accept(expr);
            var vx = expr.Value;
            start.Accept(expr);
            var sx = expr.Value;
            end.Accept(expr);
            var ex = expr.Value;
            step.Accept(expr);
            var sp = expr.Value;
            this.lines.For(this.lineNumber, vx, sx, ex, sp);
        }

        public void Go(string name, int dest)
        {
            switch (name)
            {
                case "Goto":
                    this.AddGoto(dest);
                    break;
                case "Gosub":
                    this.AddGosub(dest);
                    break;
                default:
                    throw new NotImplementedException("Go:" + name);
            }
        }

        public void IfThen(BasicExpression cond, BasicStatement[] ifTrue)
        {
            foreach (BasicStatement stmt in ifTrue)
            {
                stmt.Accept(this);
            }

            ExpressionNode expr = new ExpressionNode(this.generator, this.vars, this.methods);
            cond.Accept(expr);
            var nz = this.generator.ValueNotEqualsExpression(expr.Value, this.generator.LiteralExpression(0));
            this.lines.AddIfThen(this.lineNumber, nz);
        }

        public void Input(string prompt, BasicExpression v)
        {
            ExpressionNode expr = new ExpressionNode(this.generator, this.vars, this.methods);
            v.Accept(expr);
            SyntaxNode lval = expr.Value;
            this.AddInput(prompt, lval);
        }

        public void Many(string name, BasicExpression[] list)
        {
            switch (name)
            {
                case "Data":
                    this.AddData(list);
                    break;
                case "Dim":
                    this.AddDim(list);
                    break;
                case "Next":
                    this.AddNext(list[0]);
                    break;
                case "Print":
                    this.AddPrint(list);
                    break;
                case "PrintN":
                    this.AddPrint(list, false);
                    break;
                case "Read":
                    this.AddRead(list);
                    break;
                default:
                    throw new NotImplementedException("Many:" + name);
            }
        }

        public void Remark(string text)
        {
            this.lines.AddComment(this.lineNumber, SyntaxFactory.Comment("// " + text));
        }

        public void Void(string name)
        {
            switch (name)
            {
                case "Cls":
                    this.AddCls();
                    break;
                case "End":
                    this.AddEnd();
                    break;
                case "Return":
                    this.AddReturn();
                    break;
                case "Run":
                    this.AddRun();
                    break;
                default:
                    throw new NotImplementedException("Void:" + name);
            }
        }

        private void AddNext(BasicExpression v)
        {
            ExpressionNode expr = new ExpressionNode(this.generator, this.vars, this.methods);
            v.Accept(expr);
            var vx = expr.Value;
            this.lines.Next(this.lineNumber, vx);
        }

        private void Usings(IList<SyntaxNode> declarations)
        {
            declarations.Add(this.generator.NamespaceImportDeclaration("System"));
            declarations.Add(this.generator.NamespaceImportDeclaration("System.Collections"));
            declarations.Add(this.generator.NamespaceImportDeclaration("System.IO"));
        }

        private void Class(IList<SyntaxNode> declarations)
        {
            List<SyntaxNode> classMembers = new List<SyntaxNode>();
            this.Fields(classMembers);
            this.Constructor(classMembers);
            this.RunMethod(classMembers);
            classMembers.Add(this.vars.Init(this.data));
            this.methods.Declare(classMembers);
            classMembers.AddRange(this.lines.Subroutines());
            this.MainMethod(classMembers);

            var classDecl = this.generator.ClassDeclaration(
                this.name,
                accessibility: Accessibility.Internal,
                modifiers: DeclarationModifiers.Sealed,
                members: classMembers);

            declarations.Add(classDecl);
        }

        private void MainMethod(List<SyntaxNode> classMembers)
        {
            List<SyntaxNode> statements = new List<SyntaxNode>();
            statements.Add(this.generator.InvocationExpression(this.generator.MemberAccessExpression(this.generator.ThisExpression(), "Init")));
            statements.AddRange(this.lines.Main());
            statements.Add(this.generator.ReturnStatement(this.generator.LiteralExpression(2)));

            var ret = this.generator.TypeExpression(SpecialType.System_Int32);
            var mainMethod = this.generator.MethodDeclaration("Main", accessibility: Accessibility.Private, returnType: ret, statements: statements);
            classMembers.Add(mainMethod);
        }

        private void RunMethod(List<SyntaxNode> classMembers)
        {
            var runCoreMember = this.generator.MemberAccessExpression(this.generator.ThisExpression(), "Main");
            var callRunCore = this.generator.InvocationExpression(runCoreMember);
            var cond = this.generator.ValueEqualsExpression(callRunCore, this.generator.LiteralExpression(1));
            List<SyntaxNode> runStatements = new List<SyntaxNode>();
            var whileLoop = this.generator.WhileStatement(cond, null);
            runStatements.Add(whileLoop);
            var runMethod = this.generator.MethodDeclaration("Run", accessibility: Accessibility.Public, statements: runStatements);

            classMembers.Add(runMethod);
        }

        private void Constructor(List<SyntaxNode> classMembers)
        {
            List<SyntaxNode> ctorStatements = new List<SyntaxNode>();
            var thisInput = this.generator.MemberAccessExpression(this.generator.ThisExpression(), "input");
            var assignInput = this.generator.AssignmentStatement(thisInput, this.generator.IdentifierName("input"));
            ctorStatements.Add(assignInput);
            var thisOutput = this.generator.MemberAccessExpression(this.generator.ThisExpression(), "output");
            var assignOutput = this.generator.AssignmentStatement(thisOutput, this.generator.IdentifierName("output"));
            ctorStatements.Add(assignOutput);
            List<SyntaxNode> ctorParams = new List<SyntaxNode>();
            ctorParams.Add(this.generator.ParameterDeclaration("input", type: this.generator.IdentifierName("TextReader")));
            ctorParams.Add(this.generator.ParameterDeclaration("output", type: this.generator.IdentifierName("TextWriter")));
            var ctorMethod = this.generator.ConstructorDeclaration(accessibility: Accessibility.Public, parameters: ctorParams, statements: ctorStatements);

            classMembers.Add(ctorMethod);
        }

        private void Fields(List<SyntaxNode> classMembers)
        {
            classMembers.Add(this.generator.FieldDeclaration("input", this.generator.IdentifierName("TextReader"), accessibility: Accessibility.Private, modifiers: DeclarationModifiers.ReadOnly));
            classMembers.Add(this.generator.FieldDeclaration("output", this.generator.IdentifierName("TextWriter"), accessibility: Accessibility.Private, modifiers: DeclarationModifiers.ReadOnly));
            classMembers.AddRange(this.data.Fields());
            classMembers.AddRange(this.vars.Fields());
        }

        private void AddGoto(int destination)
        {
            this.lines.AddGoto(this.lineNumber, destination);
        }

        private void AddGosub(int destination)
        {
            this.lines.AddGosub(this.lineNumber, destination);
        }

        private void AddData(BasicExpression[] exprs)
        {
            foreach (var expr in exprs)
            {
                this.AddData(expr);
            }
        }

        private void AddData(BasicExpression expr)
        {
            ExpressionNode left = new ExpressionNode(this.generator, this.vars, this.methods);
            expr.Accept(left);
            this.data.Add(left.Value);
        }

        private void AddRead(BasicExpression[] exprs)
        {
            foreach (var expr in exprs)
            {
                this.AddRead(expr);
            }
        }

        private void AddRead(BasicExpression expr)
        {
            ExpressionNode left = new ExpressionNode(this.generator, this.vars, this.methods);
            expr.Accept(left);
            SyntaxNode lval = left.Value;
            string name = "READ_" + (expr.Type == BasicType.Num ? "n" : "s");
            var rval = this.generator.InvocationExpression(this.generator.IdentifierName(name));
            this.lines.Add(this.lineNumber, this.generator.AssignmentStatement(lval, rval));
            var st = expr.Type == BasicType.Num ? SpecialType.System_Single : SpecialType.System_String;
            var type = this.generator.TypeExpression(st);
            var callDequeue = this.generator.MemberAccessExpression(this.generator.IdentifierName("DATA"), "Dequeue");
            var deq = this.generator.CastExpression(type, this.generator.InvocationExpression(callDequeue));
            SyntaxNode[] readStatements = new SyntaxNode[] { this.generator.ReturnStatement(deq) };
            var readMethod = this.generator.MethodDeclaration(name, returnType: type, accessibility: Accessibility.Private, statements: readStatements);
            this.methods.Add(name, readMethod);
        }

        private void AddPrint(BasicExpression[] exprs, bool lineBreak = true)
        {
            ExpressionNode node = new ExpressionNode(this.generator, this.vars, this.methods);
            SyntaxNode arg = this.generator.LiteralExpression(string.Empty);
            foreach (BasicExpression expr in exprs)
            {
                expr.Accept(node);
                arg = this.generator.AddExpression(arg, node.Value);
            }

            this.AddPrint(arg, lineBreak);
        }

        private void AddPrint(SyntaxNode arg, bool lineBreak)
        {
            string name = "PRINT" + (lineBreak ? string.Empty : "_n");
            var callPrint = this.generator.InvocationExpression(SyntaxFactory.IdentifierName(name), arg);
            this.lines.Add(this.lineNumber, callPrint);
            var output = this.generator.MemberAccessExpression(this.generator.ThisExpression(), this.generator.IdentifierName("output"));
            var callWriteLine = this.generator.MemberAccessExpression(output, lineBreak ? "WriteLine" : "Write");
            SyntaxNode[] printStatements = new SyntaxNode[] { this.generator.InvocationExpression(callWriteLine, this.generator.IdentifierName("expression")) };
            SyntaxNode[] parameters = new SyntaxNode[] { this.generator.ParameterDeclaration("expression", type: this.generator.TypeExpression(SpecialType.System_String)) };
            var printMethod = this.generator.MethodDeclaration(name, accessibility: Accessibility.Private, parameters: parameters, statements: printStatements);
            this.methods.Add(name, printMethod);
        }

        private void AddInput(string prompt, SyntaxNode lval)
        {
            string name = "INPUT_n";
            var rval = this.generator.InvocationExpression(this.generator.IdentifierName(name), this.generator.LiteralExpression(prompt));
            this.lines.Add(this.lineNumber, this.generator.AssignmentStatement(lval, rval));
            var output = this.generator.MemberAccessExpression(this.generator.ThisExpression(), this.generator.IdentifierName("output"));
            var callWrite = this.generator.MemberAccessExpression(output, "Write");
            var promptQ = this.generator.AddExpression(this.generator.IdentifierName("prompt"), this.generator.LiteralExpression("? "));
            var writePrompt = this.generator.InvocationExpression(callWrite, promptQ);
            var input = this.generator.MemberAccessExpression(this.generator.ThisExpression(), this.generator.IdentifierName("input"));
            var callReadLine = this.generator.MemberAccessExpression(input, "ReadLine");
            var read = this.generator.InvocationExpression(callReadLine);
            var assignStr = this.generator.LocalDeclarationStatement(this.generator.TypeExpression(SpecialType.System_String), "v", read);
            var declR = this.generator.LocalDeclarationStatement(this.generator.TypeExpression(SpecialType.System_Single), "r");
            var callTryParse = this.generator.MemberAccessExpression(this.generator.TypeExpression(SpecialType.System_Single), "TryParse");
            var argR = this.generator.Argument(RefKind.Out, this.generator.IdentifierName("r"));
            var tryParse = this.generator.InvocationExpression(callTryParse, this.generator.IdentifierName("v"), argR);
            var retR = this.generator.ReturnStatement(this.generator.IdentifierName("r"));
            var ifParse = this.generator.IfStatement(tryParse, new SyntaxNode[] { retR });
            var callWriteLine = this.generator.MemberAccessExpression(output, "WriteLine");
            var writeError = this.generator.InvocationExpression(callWriteLine, this.generator.LiteralExpression("?Redo from start"));

            SyntaxNode[] block = new SyntaxNode[]
            {
                writePrompt,
                assignStr,
                declR,
                ifParse,
                writeError
            };
            SyntaxNode[] inputStatements = new SyntaxNode[]
            {
                this.generator.WhileStatement(this.generator.LiteralExpression(true), block)
            };
            SyntaxNode[] parameters = new SyntaxNode[] { this.generator.ParameterDeclaration("prompt", type: this.generator.TypeExpression(SpecialType.System_String)) };
            var ret = this.generator.TypeExpression(SpecialType.System_Single);
            var inputMethod = this.generator.MethodDeclaration(name, returnType: ret, accessibility: Accessibility.Private, parameters: parameters, statements: inputStatements);
            this.methods.Add(name, inputMethod);
        }

        private void AddReturn()
        {
            this.lines.AddReturn(this.lineNumber);
        }

        private void AddEnd()
        {
            var ret = this.generator.ReturnStatement(this.generator.LiteralExpression(2));
            this.lines.Add(this.lineNumber, ret);
        }

        private void AddRun()
        {
            var ret = this.generator.ReturnStatement(this.generator.LiteralExpression(1));
            this.lines.Add(this.lineNumber, ret);
        }

        private void AddCls()
        {
            string name = "CLS";
            var callCls = this.generator.InvocationExpression(SyntaxFactory.IdentifierName(name));
            this.lines.Add(this.lineNumber, callCls);
            var output = this.generator.MemberAccessExpression(this.generator.ThisExpression(), this.generator.IdentifierName("output"));
            var callWrite = this.generator.MemberAccessExpression(output, "Write");
            var callClear = this.generator.MemberAccessExpression(this.generator.IdentifierName("Console"), "Clear");
            SyntaxNode[] clsStatements = new SyntaxNode[]
            {
                this.generator.InvocationExpression(callWrite, this.generator.LiteralExpression('\f')),
                this.generator.InvocationExpression(callClear),
            };
            var clsMethod = this.generator.MethodDeclaration(name, accessibility: Accessibility.Private, statements: clsStatements);
            this.methods.Add(name, clsMethod);
        }

        private void AddDim(BasicExpression[] exprs)
        {
            foreach (BasicExpression expr in exprs)
            {
                this.AddDim(expr);
            }
        }

        private void AddDim(BasicExpression expr)
        {
            ExpressionNode node = new ExpressionNode(this.generator, this.vars, this.methods, true);
            expr.Accept(node);
            this.lines.Add(this.lineNumber, node.Value);
        }

        private sealed class Methods
        {
            private readonly Dictionary<string, SyntaxNode> methods;

            public Methods()
            {
                this.methods = new Dictionary<string, SyntaxNode>();
            }

            public void Add(string name, SyntaxNode method)
            {
                if (!this.methods.ContainsKey(name))
                {
                    this.methods.Add(name, method);
                }
            }

            public void Declare(IList<SyntaxNode> classMembers)
            {
                foreach (SyntaxNode method in this.methods.Values)
                {
                    classMembers.Add(method);
                }
            }
        }

        private sealed class ExpressionNode : IExpressionVisitor
        {
            private readonly SyntaxGenerator generator;
            private readonly Variables vars;
            private readonly Methods methods;
            private readonly bool dim;

            public ExpressionNode(SyntaxGenerator generator, Variables vars, Methods methods, bool dim = false)
            {
                this.generator = generator;
                this.vars = vars;
                this.methods = methods;
                this.dim = dim;
            }

            public BasicType Type { get; private set; }

            public SyntaxNode Value { get; private set; }

            public void Array(BasicType type, string name, BasicExpression[] subs)
            {
                this.Type = type;
                if (this.dim)
                {
                    this.Value = this.vars.Dim(this.methods, type, name, subs);
                }
                else
                {
                    this.Value = this.vars.Index(this.methods, type, name, subs);
                }
            }

            public void Literal(BasicType type, object o)
            {
                this.Value = this.generator.LiteralExpression(o);
            }

            public void Operator(string name, BasicExpression[] operands)
            {
                operands[0].Accept(this);
                SyntaxNode x = this.Value;
                if (operands.Length == 1)
                {
                    this.Value = this.Unary(name, x);
                }
                else if (operands.Length == 2)
                {
                    operands[1].Accept(this);
                    SyntaxNode y = this.Value;
                    this.Value = this.Binary(name, x, y);
                }
                else if (operands.Length == 3)
                {
                    operands[1].Accept(this);
                    SyntaxNode y = this.Value;
                    operands[2].Accept(this);
                    SyntaxNode z = this.Value;
                    this.Value = this.Ternary(name, x, y, z);
                }
                else
                {
                    throw new NotSupportedException("Operator:" + name);
                }
            }

            public void Variable(BasicType type, string name)
            {
                this.Value = this.vars.Add(type, name);
            }

            private SyntaxNode CastInt(SyntaxNode node)
            {
                return this.generator.CastExpression(this.generator.TypeExpression(SpecialType.System_Int32), node);
            }

            private SyntaxNode Unary(string name, SyntaxNode x)
            {
                switch (name)
                {
                    case "Len": return this.Len(x);
                    default: throw new NotSupportedException("Operator:" + name);
                }
            }

            private SyntaxNode Binary(string name, SyntaxNode x, SyntaxNode y)
            {
                switch (name)
                {
                    case "Eq": return this.Cond(this.generator.ValueEqualsExpression, x, y);
                    case "Ne": return this.Cond(this.generator.ValueNotEqualsExpression, x, y);
                    case "Le": return this.Cond(this.generator.LessThanOrEqualExpression, x, y);
                    case "Lt": return this.Cond(this.generator.LessThanExpression, x, y);
                    case "Ge": return this.Cond(this.generator.GreaterThanOrEqualExpression, x, y);
                    case "Gt": return this.Cond(this.generator.GreaterThanExpression, x, y);
                    case "Or": return this.generator.BitwiseOrExpression(this.CastInt(x), this.CastInt(y));
                    case "And": return this.generator.BitwiseAndExpression(this.CastInt(x), this.CastInt(y));
                    case "Add": return this.generator.AddExpression(x, y);
                    case "Sub": return this.generator.SubtractExpression(x, y);
                    case "Mult": return this.generator.MultiplyExpression(x, y);
                    case "Div": return this.generator.DivideExpression(x, y);
                    case "Left": return this.Left(x, y);
                    default: throw new NotSupportedException("Operator:" + name);
                }
            }

            private SyntaxNode Ternary(string name, SyntaxNode x, SyntaxNode y, SyntaxNode z)
            {
                switch (name)
                {
                    case "Mid": return this.Mid(x, y, z);
                    default: throw new NotSupportedException("Operator:" + name);
                }
            }

            private SyntaxNode Cond(Func<SyntaxNode, SyntaxNode, SyntaxNode> cond, SyntaxNode x, SyntaxNode y)
            {
                var call = this.generator.InvocationExpression(this.generator.MemberAccessExpression(x, "CompareTo"), y);
                var zero = this.generator.LiteralExpression(0);
                var neg1 = this.generator.LiteralExpression(-1);
                return this.generator.ConditionalExpression(cond(call, zero), neg1, zero);
            }

            private SyntaxNode Len(SyntaxNode x)
            {
                return this.generator.MemberAccessExpression(x, "Length");
            }

            private SyntaxNode Left(SyntaxNode x, SyntaxNode n)
            {
                string name = "LEFT_s";
                var callLeft = this.generator.InvocationExpression(this.generator.IdentifierName(name), x, this.CastInt(n));
                var intT = this.generator.TypeExpression(SpecialType.System_Int32);
                var nv = this.generator.IdentifierName("n");
                var xv = this.generator.IdentifierName("x");
                var retX = this.generator.ReturnStatement(xv);
                var len = this.generator.MemberAccessExpression(xv, "Length");
                var gtL = this.generator.GreaterThanExpression(nv, len);
                var ifN = this.generator.IfStatement(gtL, new SyntaxNode[] { retX });
                var zero = this.generator.LiteralExpression(0);
                var callSubstr = this.generator.MemberAccessExpression(xv, "Substring");
                var retSubstr = this.generator.ReturnStatement(this.generator.InvocationExpression(callSubstr, zero, nv));
                SyntaxNode[] leftStatements = new SyntaxNode[]
                {
                    ifN,
                    retSubstr
                };
                var strT = this.generator.TypeExpression(SpecialType.System_String);
                SyntaxNode[] parameters = new SyntaxNode[]
                {
                    this.generator.ParameterDeclaration("x", type: strT),
                    this.generator.ParameterDeclaration("n", type: intT),
                };
                var midMethod = this.generator.MethodDeclaration(name, parameters: parameters, returnType: strT, accessibility: Accessibility.Private, statements: leftStatements);
                this.methods.Add(name, midMethod);

                return callLeft;
            }

            private SyntaxNode Mid(SyntaxNode x, SyntaxNode n, SyntaxNode m)
            {
                string name = "MID_s";
                var callMid = this.generator.InvocationExpression(this.generator.IdentifierName(name), x, this.CastInt(n), this.CastInt(m));
                var intT = this.generator.TypeExpression(SpecialType.System_Int32);
                var retE = this.generator.ReturnStatement(this.generator.LiteralExpression(string.Empty));
                var nv = this.generator.IdentifierName("n");
                var xv = this.generator.IdentifierName("x");
                var len = this.generator.MemberAccessExpression(xv, "Length");
                var gtL = this.generator.GreaterThanExpression(nv, len);
                var ifN = this.generator.IfStatement(gtL, new SyntaxNode[] { retE });
                var sub = this.generator.SubtractExpression(len, nv);
                var one = this.generator.LiteralExpression(1);
                var setL = this.generator.LocalDeclarationStatement(intT, "l", this.generator.AddExpression(sub, one));
                var mv = this.generator.IdentifierName("m");
                var lv = this.generator.IdentifierName("l");
                var gt = this.generator.GreaterThanExpression(mv, lv);
                var ifM = this.generator.IfStatement(gt, new SyntaxNode[] { this.generator.AssignmentStatement(mv, lv) });
                var n1 = this.generator.SubtractExpression(nv, one);
                var callSubstr = this.generator.MemberAccessExpression(xv, "Substring");
                var retSubstr = this.generator.ReturnStatement(this.generator.InvocationExpression(callSubstr, n1, mv));
                SyntaxNode[] midStatements = new SyntaxNode[]
                {
                    ifN,
                    setL,
                    ifM,
                    retSubstr
                };
                var strT = this.generator.TypeExpression(SpecialType.System_String);
                SyntaxNode[] parameters = new SyntaxNode[]
                {
                    this.generator.ParameterDeclaration("x", type: strT),
                    this.generator.ParameterDeclaration("n", type: intT),
                    this.generator.ParameterDeclaration("m", type: intT)
                };
                var midMethod = this.generator.MethodDeclaration(name, parameters: parameters, returnType: strT, accessibility: Accessibility.Private, statements: midStatements);
                this.methods.Add(name, midMethod);

                return callMid;
            }
        }

        private sealed class DataValues
        {
            private readonly SyntaxGenerator generator;
            private readonly List<SyntaxNode> values;

            public DataValues(SyntaxGenerator generator)
            {
                this.generator = generator;
                this.values = new List<SyntaxNode>();
            }

            public IEnumerable<SyntaxNode> Fields()
            {
                yield return this.generator.FieldDeclaration("DATA", this.generator.IdentifierName("Queue"), Accessibility.Private);
            }

            public void Add(SyntaxNode value)
            {
                this.values.Add(value);
            }

            public IEnumerable<SyntaxNode> Init()
            {
                var newQ = this.generator.ObjectCreationExpression(this.generator.IdentifierName("Queue"));
                var d = this.generator.IdentifierName("DATA");
                yield return this.generator.AssignmentStatement(d, newQ);

                var callEnqueue = this.generator.MemberAccessExpression(d, "Enqueue");
                foreach (var value in this.values)
                {
                    yield return this.generator.InvocationExpression(callEnqueue, value);
                }
            }
        }

        private sealed class Variables
        {
            private readonly SyntaxGenerator generator;
            private readonly Dictionary<string, Variable> strArrs;
            private readonly Dictionary<string, Variable> numArrs;
            private readonly Dictionary<string, Variable> strs;
            private readonly Dictionary<string, Variable> nums;

            public Variables(SyntaxGenerator generator)
            {
                this.generator = generator;
                this.strArrs = new Dictionary<string, Variable>();
                this.numArrs = new Dictionary<string, Variable>();
                this.strs = new Dictionary<string, Variable>();
                this.nums = new Dictionary<string, Variable>();
            }

            private IEnumerable<Variable> Scalars => this.strs.Values.Concat(this.nums.Values);

            private IEnumerable<Variable> Arrays => this.strArrs.Values.Concat(this.numArrs.Values);

            public IEnumerable<SyntaxNode> Fields()
            {
                foreach (Variable v in this.Arrays.Concat(this.Scalars))
                {
                    yield return v.Field();
                }
            }

            public SyntaxNode Init(DataValues data)
            {
                List<SyntaxNode> statements = new List<SyntaxNode>();
                statements.AddRange(data.Init());

                foreach (Variable v in this.Scalars)
                {
                    statements.Add(v.Init());
                }

                return this.generator.MethodDeclaration("Init", accessibility: Accessibility.Private, statements: statements);
            }

            public SyntaxNode Dim(Methods methods, BasicType type, string name, BasicExpression[] subs)
            {
                SyntaxNode[] subNodes = this.Subscripts(methods, subs);

                return this.Add(type, name, subNodes.Length).Dim(methods, subNodes);
            }

            public SyntaxNode Index(Methods methods, BasicType type, string name, BasicExpression[] subs)
            {
                SyntaxNode[] subNodes = this.Subscripts(methods, subs);

                return this.Add(type, name, subNodes.Length).Index(subNodes);
            }

            public SyntaxNode Add(BasicType type, string name)
            {
                return this.Add(type, name, 0).Ref();
            }

            private SyntaxNode[] Subscripts(Methods methods, BasicExpression[] subs)
            {
                ExpressionNode node = new ExpressionNode(this.generator, this, methods);
                SyntaxNode[] subNodes = new SyntaxNode[subs.Length];
                for (int i = 0; i < subs.Length; ++i)
                {
                    subs[i].Accept(node);
                    subNodes[i] = node.Value;
                }

                return subNodes;
            }

            private Variable Add(BasicType type, string name, int subs)
            {
                IDictionary<string, Variable> dict;
                if (type == BasicType.Str)
                {
                    dict = (subs > 0) ? this.strArrs : this.strs;
                }
                else
                {
                    dict = (subs > 0) ? this.numArrs : this.nums;
                }

                return this.Add(dict, type, name, subs);
            }

            private Variable Add(IDictionary<string, Variable> vars, BasicType type, string name, int subs)
            {
                Variable v;
                if (!vars.TryGetValue(name, out v))
                {
                    v = new Variable(this.generator, type, name, subs);
                    vars.Add(name, v);
                }

                return v;
            }

            private sealed class Variable
            {
                private readonly SyntaxGenerator generator;
                private readonly BasicType type;
                private readonly string name;
                private readonly int subs;

                public Variable(SyntaxGenerator generator, BasicType type, string name, int subs)
                {
                    this.generator = generator;
                    this.type = type;
                    this.name = name;
                    this.subs = subs;
                }

                private TypeSyntax ElementType
                {
                    get
                    {
                        SyntaxKind kind = SyntaxKind.FloatKeyword;
                        if (this.type == BasicType.Str)
                        {
                            kind = SyntaxKind.StringKeyword;
                        }

                        return SyntaxFactory.PredefinedType(SyntaxFactory.Token(kind));
                    }
                }

                private SyntaxNode Type
                {
                    get
                    {
                        if (this.subs > 0)
                        {
                            return this.ArrayType();
                        }

                        return this.ElementType;
                    }
                }

                private string Name => this.name + this.Suffix;

                private string Suffix
                {
                    get
                    {
                        string suffix = "_n";
                        if (this.type == BasicType.Str)
                        {
                            suffix = "_s";
                        }

                        if (this.subs > 0)
                        {
                            suffix += "a";
                        }

                        return suffix;
                    }
                }

                private SyntaxNode Default
                {
                    get
                    {
                        object lit = 0;
                        if (this.type == BasicType.Str)
                        {
                            lit = string.Empty;
                        }

                        return this.generator.LiteralExpression(lit);
                    }
                }

                public SyntaxNode Ref()
                {
                    return this.generator.IdentifierName(this.Name);
                }

                public SyntaxNode Field()
                {
                    return this.generator.FieldDeclaration(this.Name, this.Type, accessibility: Accessibility.Private);
                }

                public SyntaxNode Init()
                {
                    return this.generator.AssignmentStatement(this.Ref(), this.Default);
                }

                public SyntaxNode Index(SyntaxNode[] sub)
                {
                    return this.generator.ElementAccessExpression(this.Ref(), sub);
                }

                public SyntaxNode Dim(Methods methods, SyntaxNode[] sub)
                {
                    string name = "DIM" + sub.Length + this.Suffix;
                    var arr = this.generator.IdentifierName("a");
                    SyntaxNode[] subNodes = new SyntaxNode[sub.Length];

                    List<SyntaxNode> parameters = new List<SyntaxNode>();
                    parameters.Add(this.generator.ParameterDeclaration("a", type: this.ArrayType(), refKind: RefKind.Out));
                    for (int i = 0; i < sub.Length; ++i)
                    {
                        string n = "d" + (i + 1);
                        var p = this.generator.ParameterDeclaration(n, type: this.generator.TypeExpression(SpecialType.System_Single));
                        parameters.Add(p);

                        var dn = this.generator.IdentifierName(n);
                        var leftS = this.generator.CastExpression(this.generator.TypeExpression(SpecialType.System_Int32), dn);

                        subNodes[i] = this.generator.AddExpression(leftS, this.generator.LiteralExpression(1));
                    }

                    var arrR = SyntaxFactory.ArrayCreationExpression(this.ArrayType(subNodes));

                    List<SyntaxNode> dimStatements = new List<SyntaxNode>();
                    dimStatements.Add(this.generator.AssignmentStatement(arr, arrR));
                    if (this.type == BasicType.Str)
                    {
                        var fill = this.generator.MemberAccessExpression(this.generator.IdentifierName("Array"), "Fill");
                        var callFill = this.generator.InvocationExpression(fill, arr, this.Default);
                        dimStatements.Add(callFill);
                    }

                    var dimMethod = this.generator.MethodDeclaration(name, accessibility: Accessibility.Private, parameters: parameters, statements: dimStatements);
                    methods.Add(name, dimMethod);

                    var method = this.generator.IdentifierName(name);
                    List<SyntaxNode> args = new List<SyntaxNode>();
                    args.Add(this.generator.Argument(RefKind.Out, this.Ref()));
                    args.AddRange(sub);
                    return this.generator.InvocationExpression(method, args);
                }

                private ArrayTypeSyntax ArrayType(params SyntaxNode[] nodes)
                {
                    List<SyntaxNodeOrToken> sizes = new List<SyntaxNodeOrToken>();
                    var omit = SyntaxFactory.OmittedArraySizeExpression();
                    var comma = SyntaxFactory.Token(SyntaxKind.CommaToken);

                    for (int i = 0; i < this.subs; ++i)
                    {
                        if (i != 0)
                        {
                            sizes.Add(comma);
                        }

                        if (nodes.Length == 0)
                        {
                            sizes.Add(omit);
                        }
                        else
                        {
                            sizes.Add(nodes[i]);
                        }
                    }

                    var rank = SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SeparatedList<ExpressionSyntax>(sizes));

                    return SyntaxFactory.ArrayType(this.ElementType, SyntaxFactory.SingletonList(rank));
                }
            }
        }

        private sealed class Lines
        {
            private readonly SyntaxGenerator generator;
            private readonly SortedList<int, Line> statements;
            private readonly HashSet<int> references;
            private readonly HashSet<int> subStarts;
            private readonly HashSet<int> subEnds;
            private readonly Loops loops;

            public Lines(SyntaxGenerator generator)
            {
                this.generator = generator;
                this.statements = new SortedList<int, Line>();
                this.references = new HashSet<int>();
                this.subStarts = new HashSet<int>();
                this.subEnds = new HashSet<int>();
                this.loops = new Loops();
            }

            public void Add(int number, SyntaxNode node)
            {
                this.Get(number).Add(node);
            }

            public void AddComment(int number, SyntaxTrivia comment)
            {
                this.Get(number).Add(comment);
            }

            public void AddIfThen(int number, SyntaxNode cond)
            {
                this.Get(number).Wrap(n => this.generator.IfStatement(cond, n));
            }

            public void AddGoto(int number, int destination)
            {
                var label = SyntaxFactory.IdentifierName(Label(destination));
                var gotoStatement = SyntaxFactory.GotoStatement(SyntaxKind.GotoStatement, label);
                this.Add(number, gotoStatement);
                this.references.Add(destination);
            }

            public void AddGosub(int number, int destination)
            {
                var ret1 = this.generator.ReturnStatement(this.generator.LiteralExpression(1));
                var case1 = this.generator.SwitchSection(this.generator.LiteralExpression(1), new SyntaxNode[] { ret1 });
                var ret2 = this.generator.ReturnStatement(this.generator.LiteralExpression(2));
                var case2 = this.generator.SwitchSection(this.generator.LiteralExpression(2), new SyntaxNode[] { ret2 });
                var call = this.generator.InvocationExpression(this.generator.IdentifierName("Sub_" + destination));
                this.Add(number, this.generator.SwitchStatement(call, case1, case2));
                this.subStarts.Add(destination);
            }

            public void AddReturn(int number)
            {
                var ret = this.generator.ReturnStatement(this.generator.LiteralExpression(0));
                this.Add(number, ret);
                this.subEnds.Add(number);
            }

            public IEnumerable<SyntaxNode> Subroutines()
            {
                string subName = null;
                List<SyntaxNode> subLines = new List<SyntaxNode>();
                foreach (KeyValuePair<int, Line> line in this.statements)
                {
                    if (subName == null)
                    {
                        if (this.subStarts.Contains(line.Key))
                        {
                            subName = "Sub_" + line.Key;
                        }
                    }

                    if (subName != null)
                    {
                        subLines.AddRange(line.Value.Nodes(this.references));
                        if (this.subEnds.Contains(line.Key))
                        {
                            var ret = this.generator.TypeExpression(SpecialType.System_Int32);
                            yield return this.generator.MethodDeclaration(
                                subName,
                                returnType: ret,
                                accessibility: Accessibility.Private,
                                statements: subLines);
                            subName = null;
                            subLines.Clear();
                        }
                    }
                }
            }

            public IEnumerable<SyntaxNode> Main()
            {
                bool readingSub = false;
                foreach (KeyValuePair<int, Line> line in this.statements)
                {
                    if (readingSub)
                    {
                        if (this.subEnds.Contains(line.Key))
                        {
                            readingSub = false;
                        }
                    }
                    else
                    {
                        if (this.subStarts.Contains(line.Key))
                        {
                            readingSub = true;
                        }
                        else
                        {
                            foreach (SyntaxNode node in line.Value.Nodes(this.references))
                            {
                                yield return node;
                            }
                        }
                    }
                }
            }

            public void For(int number, SyntaxNode vx, SyntaxNode sx, SyntaxNode ex, SyntaxNode sp)
            {
                this.loops.Push(number, vx, sx, ex, sp);
            }

            public void Next(int end, SyntaxNode vx)
            {
                Tuple<int, SyntaxNode, SyntaxNode, SyntaxNode, SyntaxNode> t = this.loops.Pop(vx);
                int start = t.Item1;
                List<SyntaxNode> body = new List<SyntaxNode>();
                foreach (int n in this.statements.Keys.ToArray())
                {
                    if (n >= end)
                    {
                        break;
                    }
                    else if (n >= start)
                    {
                        body.AddRange(this.statements[n].Nodes());
                        this.statements.Remove(n);
                    }
                }

                var sx = t.Item3;
                var ex = t.Item4;
                var sp = t.Item5;

                var inc = this.generator.AddExpression(vx, sp);
                body.Add(this.generator.AssignmentStatement(vx, inc));

                var init = this.generator.AssignmentStatement(vx, sx);
                this.Add(start, init);

                var cond = this.generator.LessThanOrEqualExpression(vx, ex);
                var loop = this.generator.WhileStatement(cond, body);
                this.Add(end, loop);
            }

            private static string Label(int number)
            {
                return "L" + number;
            }

            private Line Get(int number)
            {
                Line line;
                if (!this.statements.TryGetValue(number, out line))
                {
                    line = new Line(number);
                    this.statements.Add(number, line);
                }

                return line;
            }

            private sealed class Loops
            {
                private readonly Stack<Tuple<int, SyntaxNode, SyntaxNode, SyntaxNode, SyntaxNode>> loops;

                public Loops()
                {
                    this.loops = new Stack<Tuple<int, SyntaxNode, SyntaxNode, SyntaxNode, SyntaxNode>>();
                }

                public void Push(int number, SyntaxNode vx, SyntaxNode sx, SyntaxNode ex, SyntaxNode sp)
                {
                    this.loops.Push(Tuple.Create(number, vx, sx, ex, sp));
                }

                public Tuple<int, SyntaxNode, SyntaxNode, SyntaxNode, SyntaxNode> Pop(SyntaxNode vx)
                {
                    var t = this.loops.Pop();
                    if (!t.Item2.IsEquivalentTo(vx))
                    {
                        throw new InvalidProgramException("NEXT (" + vx + ") does not match FOR (" + t.Item2 + ").");
                    }

                    return t;
                }
            }

            private sealed class Line
            {
                private readonly int number;
                private readonly List<SyntaxNode> nodes;

                public Line(int number)
                {
                    this.number = number;
                    this.nodes = new List<SyntaxNode>();
                }

                public void Add(SyntaxNode node) => this.nodes.Add(node);

                public void Add(SyntaxTrivia comment)
                {
                    if (this.nodes.Count == 0)
                    {
                        this.Add(SyntaxFactory.EmptyStatement().WithTrailingTrivia(comment));
                    }
                    else
                    {
                        int n = this.nodes.Count - 1;
                        SyntaxNode last = this.nodes[n];
                        this.nodes[n] = last.WithTrailingTrivia(comment);
                    }
                }

                public void Wrap(Func<IEnumerable<SyntaxNode>, SyntaxNode> wrap)
                {
                    SyntaxNode wrapped = wrap(this.nodes);
                    this.nodes.Clear();
                    this.nodes.Add(wrapped);
                }

                public IEnumerable<SyntaxNode> Nodes(ISet<int> references = null)
                {
                    if ((references != null) && references.Contains(this.number))
                    {
                        yield return SyntaxFactory.LabeledStatement(Label(this.number), SyntaxFactory.EmptyStatement());
                    }

                    foreach (SyntaxNode node in this.nodes)
                    {
                        yield return node;
                    }
                }
            }
        }
    }
}
