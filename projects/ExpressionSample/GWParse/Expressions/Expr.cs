﻿// <copyright file="Expr.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace GWParse.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Sprache;

    internal static class Expr
    {
        public static readonly Parser<BasicExpression> Any = Parse.Ref(() => Root);

        public static readonly Parser<BasicExpression> AnyArray = Var.ArrayAny;

        public static readonly Parser<BasicExpression> AnyVar = Var.Any;

        public static readonly Parser<BasicExpression> AnyNumScalar = Var.NumScalar;

        private static readonly Parser<BasicExpression> Paren =
            from lp in Ch.LeftParen.Token()
            from x in Any
            from rp in Ch.RightParen.Token()
            select x;

        private static readonly Parser<BasicExpression> Left =
            from f in Kw.Left
            from d in Ch.Dollar
            from lp in Ch.LeftParen.Token()
            from x in Any
            from c in Ch.Comma.Token()
            from n in Any
            from rp in Ch.RightParen.Token()
            select BasicOperator.Binary("Left", BasicType.Str, A.Str(x), A.Num(n));

        private static readonly Parser<Tuple<BasicExpression, BasicExpression>> MidPrefix =
            from f in Kw.Mid
            from d in Ch.Dollar
            from lp in Ch.LeftParen.Token()
            from x in Any
            from c in Ch.Comma.Token()
            from n in Any
            select Tuple.Create(x, n);

        private static readonly Parser<BasicExpression> Mid3 =
            from t in MidPrefix
            from c in Ch.Comma.Token()
            from m in Any
            from rp in Ch.RightParen.Token()
            select BasicOperator.Ternary("Mid", BasicType.Str, A.Str(t.Item1), A.Num(t.Item2), A.Num(m));

        private static readonly Parser<BasicExpression> Mid2 =
            from t in MidPrefix
            from rp in Ch.RightParen.Token()
            select BasicOperator.Binary("Mid", BasicType.Str, A.Str(t.Item1), A.Num(t.Item2));

        private static readonly Parser<BasicExpression> Mid = Mid3.Or(Mid2);

        private static readonly Parser<BasicExpression> Right =
            from f in Kw.Right
            from d in Ch.Dollar
            from lp in Ch.LeftParen.Token()
            from x in Any
            from c in Ch.Comma.Token()
            from n in Any
            from rp in Ch.RightParen.Token()
            select BasicOperator.Binary("Right", BasicType.Str, A.Str(x), A.Num(n));

        private static readonly Parser<BasicExpression> Exp =
            from f in Kw.Exp
            from x in Paren
            select BasicOperator.Unary("Exp", BasicType.Num, A.Num(x));

        private static readonly Parser<BasicExpression> Len =
            from f in Kw.Len
            from x in Paren
            select BasicOperator.Unary("Len", BasicType.Num, A.Str(x));

        private static readonly Parser<BasicExpression> Sqr =
            from f in Kw.Sqr
            from x in Paren
            select BasicOperator.Unary("Sqrt", BasicType.Num, A.Num(x));

        private static readonly Parser<BasicExpression> Fun =
            Left.Or(Mid).Or(Right).Or(Exp).Or(Len).Or(Sqr);

        private static readonly Parser<BasicExpression> Value = Lit.Any.Or(Fun).Or(Var.Any);

        private static readonly Parser<BasicExpression> Unary =
            Parse.Ref(() => Neg)
            .Or(Parse.Ref(() => Not));

        private static readonly Parser<BasicExpression> Factor = Paren.Or(Value);

        private static readonly Parser<BasicExpression> Operand = Unary.Or(Factor);

        private static readonly Parser<BasicExpression> Pow =
            Parse.ChainOperator(Op.Exponential, Operand, Op.Apply);

        private static readonly Parser<BasicExpression> Neg =
            from m in Ch.Minus.Token()
            from x in Pow
            select BasicOperator.Unary("Neg", BasicType.Num, A.Num(x));

        private static readonly Parser<BasicExpression> Mult =
            Parse.ChainOperator(Op.Multiplicative, Neg.Or(Pow), Op.Apply);

        private static readonly Parser<BasicExpression> Add =
            Parse.ChainOperator(Op.Additive, Mult, Op.Apply);

        private static readonly Parser<BasicExpression> Relational =
            Parse.ChainOperator(Op.Relational, Add, Op.Apply);

        private static readonly Parser<BasicExpression> NotS =
            from k in Kw.Not
            from s in Ch.Space.AtLeastOnce()
            from x in Add
            select x;

        private static readonly Parser<BasicExpression> NotP =
            from k in Kw.Not
            from lp in Ch.LeftParen
            from x in Add
            from rp in Ch.RightParen
            select x;

        private static readonly Parser<BasicExpression> Not =
            from x in NotS.Or(NotP)
            select BasicOperator.Unary("Not", BasicType.Num, A.Num(x));

        private static readonly Parser<BasicExpression> And =
            Parse.ChainOperator(Op.And, Not.Or(Relational), Op.Apply);

        private static readonly Parser<BasicExpression> Or =
            Parse.ChainOperator(Op.Or, And, Op.Apply);

        private static readonly Parser<BasicExpression> Root = Or;

        private interface IOperator
        {
            BasicExpression Apply(BasicExpression x, BasicExpression y);
        }

        public static BasicExpression FromString(string input)
        {
            try
            {
                return Any.Token().End().Parse(input);
            }
            catch (ParseException e)
            {
                throw new FormatException("Bad expression '" + input + "'.", e);
            }
        }

        private static class A
        {
            public static BasicExpression Str(BasicExpression x) => Type(x, BasicType.Str);

            public static BasicExpression Num(BasicExpression x) => Type(x, BasicType.Num);

            private static BasicExpression Type(BasicExpression x, BasicType expected)
            {
                if (x.Type != expected)
                {
                    string error = string.Format(
                        CultureInfo.InvariantCulture,
                        "Type mismatch; expected [{0}] to be of type {1} but was of type {2}.",
                        x,
                        expected,
                        x.Type);
                    throw new ParseException(error);
                }

                return x;
            }
        }

        private static class Ch
        {
            public static readonly Parser<char> Quote = Parse.Char('\"');
            public static readonly Parser<char> Dollar = Parse.Char('$');
            public static readonly Parser<char> LeftParen = Parse.Char('(');
            public static readonly Parser<char> RightParen = Parse.Char(')');
            public static readonly Parser<char> Comma = Parse.Char(',');
            public static readonly Parser<char> Plus = Parse.Char('+');
            public static readonly Parser<char> Minus = Parse.Char('-');
            public static readonly Parser<char> Star = Parse.Char('*');
            public static readonly Parser<char> Slash = Parse.Char('/');
            public static readonly Parser<char> Caret = Parse.Char('^');
            public static readonly Parser<char> Space = Parse.Char(' ');
            public static readonly Parser<char> Equal = Parse.Char('=');
            public static readonly Parser<char> Less = Parse.Char('<');
            public static readonly Parser<char> Greater = Parse.Char('>');
            public static readonly Parser<char> NonQuote = Parse.AnyChar.Except(Quote);
        }

        private static class Kw
        {
            public static readonly HashSet<string> InvalidNum = new HashSet<string>();
            public static readonly HashSet<string> InvalidStr = new HashSet<string>();

            public static readonly Parser<IEnumerable<char>> And = Num("AND");
            public static readonly Parser<IEnumerable<char>> Exp = Num("EXP");
            public static readonly Parser<IEnumerable<char>> Len = Num("LEN");
            public static readonly Parser<IEnumerable<char>> Not = Num("NOT");
            public static readonly Parser<IEnumerable<char>> Or = Num("OR");
            public static readonly Parser<IEnumerable<char>> Sqr = Num("SQR");

            public static readonly Parser<IEnumerable<char>> Left = Str("LEFT");
            public static readonly Parser<IEnumerable<char>> Mid = Str("MID");
            public static readonly Parser<IEnumerable<char>> Right = Str("RIGHT");

            public static string ValidNum(string v)
            {
                if (InvalidNum.Contains(v))
                {
                    throw new ParseException("Invalid numeric identifier '" + v + "'.");
                }

                return v;
            }

            public static string ValidStr(string v)
            {
                if (InvalidStr.Contains(v))
                {
                    throw new ParseException("Invalid string identifier '" + v + "'.");
                }

                return v;
            }

            private static Parser<IEnumerable<char>> Num(string k)
            {
                InvalidNum.Add(k);
                return Str(k);
            }

            private static Parser<IEnumerable<char>> Str(string k)
            {
                InvalidStr.Add(k);
                return P(k);
            }

            private static Parser<IEnumerable<char>> P(string k) => Parse.IgnoreCase(k);
        }

        private static class Lit
        {
            public static readonly Parser<BasicExpression> Num =
                from n in Parse.Number
                select BasicLiteral.Num(n);

            public static readonly Parser<BasicExpression> Str =
                from lq in Ch.Quote
                from s in Ch.NonQuote.Many().Text()
                from rq in Ch.Quote
                select BasicLiteral.Str(s);

            public static readonly Parser<BasicExpression> Any = Num.Or(Str);
        }

        private static class Var
        {
            public static readonly Parser<string> IdPrefix = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

            public static readonly Parser<string> Id =
                from v in IdPrefix
                select v.ToUpperInvariant();

            public static readonly Parser<string> StrId =
                from v in Id
                from d in Ch.Dollar
                select v;

            public static readonly Parser<BasicExpression> Index =
                from x in Expr.Any
                select A.Num(x);

            public static readonly Parser<IEnumerable<BasicExpression>> IndexList =
                from lp in Ch.LeftParen.Token()
                from head in Index.Once()
                from rest in Ch.Comma.Token().Then(_ => Index).Many()
                from rp in Ch.RightParen.Token()
                select head.Concat(rest);

            public static readonly Parser<BasicExpression> NumScalar =
                from v in Id
                select BasicVariable.Num(Kw.ValidNum(v));

            public static readonly Parser<BasicExpression> NumArray =
                from v in Id
                from i in IndexList
                select BasicArray.Num(Kw.ValidNum(v), i);

            public static readonly Parser<BasicExpression> StrScalar =
                from v in StrId
                select BasicVariable.Str(Kw.ValidStr(v));

            public static readonly Parser<BasicExpression> StrArray =
                from v in StrId
                from i in IndexList
                select BasicArray.Str(Kw.ValidStr(v), i);

            public static readonly Parser<BasicExpression> NumAny = NumArray.Or(NumScalar);

            public static readonly Parser<BasicExpression> StrAny = StrArray.Or(StrScalar);

            public static readonly Parser<BasicExpression> ArrayAny = StrArray.Or(NumArray);

            public static readonly Parser<BasicExpression> Any = StrAny.Or(NumAny);
        }

        private static class Op
        {
            public static readonly Parser<IOperator> Or =
                from k in Kw.Or.Token()
                select Binary.Or;

            public static readonly Parser<IOperator> And =
                from k in Kw.And.Token()
                select Binary.And;

            public static readonly Parser<IOperator> Eq =
                from o in Ch.Equal.Token()
                select Binary.Eq;

            public static readonly Parser<IOperator> Ne =
                from o1 in Ch.Less.Token()
                from o2 in Ch.Greater.Token()
                select Binary.Ne;

            public static readonly Parser<IOperator> Le =
                from o1 in Ch.Less.Token()
                from o2 in Ch.Equal.Token()
                select Binary.Le;

            public static readonly Parser<IOperator> Lt =
                from o in Ch.Less.Token()
                select Binary.Lt;

            public static readonly Parser<IOperator> Ge =
                from o1 in Ch.Greater.Token()
                from o2 in Ch.Equal.Token()
                select Binary.Ge;

            public static readonly Parser<IOperator> Gt =
                from o in Ch.Greater.Token()
                select Binary.Gt;

            public static readonly Parser<IOperator> Add =
                from o in Ch.Plus.Token()
                select Binary.Add;

            public static readonly Parser<IOperator> Subtract =
                from o in Ch.Minus.Token()
                select Binary.Sub;

            public static readonly Parser<IOperator> Multiply =
                from o in Ch.Star.Token()
                select Binary.Mult;

            public static readonly Parser<IOperator> Divide =
                from o in Ch.Slash.Token()
                select Binary.Div;

            public static readonly Parser<IOperator> Additive = Add.Or(Subtract);

            public static readonly Parser<IOperator> Multiplicative = Multiply.Or(Divide);

            public static readonly Parser<IOperator> Exponential =
                from o in Ch.Caret.Token()
                select Binary.Pow;

            public static readonly Parser<IOperator> Relational = Eq.Or(Ne).Or(Le).Or(Lt).Or(Ge).Or(Gt);

            public static BasicExpression Apply(IOperator op, BasicExpression x, BasicExpression y)
            {
                return op.Apply(x, y);
            }

            private sealed class Binary : IOperator
            {
                public static readonly IOperator Or = new Binary("Or");
                public static readonly IOperator And = new Binary("And");
                public static readonly IOperator Eq = new Binary("Eq");
                public static readonly IOperator Ne = new Binary("Ne");
                public static readonly IOperator Le = new Binary("Le");
                public static readonly IOperator Lt = new Binary("Lt");
                public static readonly IOperator Ge = new Binary("Ge");
                public static readonly IOperator Gt = new Binary("Gt");
                public static readonly IOperator Add = new Binary("Add", BasicType.None);
                public static readonly IOperator Sub = new Binary("Sub");
                public static readonly IOperator Mult = new Binary("Mult");
                public static readonly IOperator Div = new Binary("Div");
                public static readonly IOperator Pow = new Binary("Pow");

                private readonly string name;
                private readonly BasicType type;

                private Binary(string name, BasicType type = BasicType.Num)
                {
                    this.name = name;
                    this.type = type;
                }

                public BasicExpression Apply(BasicExpression x, BasicExpression y)
                {
                    BasicType result = this.CheckType(x, y);
                    return BasicOperator.Binary(this.name, result, x, y);
                }

                private BasicType CheckType(BasicExpression x, BasicExpression y)
                {
                    if (x.Type != y.Type)
                    {
                        string error = string.Format(
                            CultureInfo.InvariantCulture,
                            "Type mismatch for operator '{0}'; Type of [{1}] is {2} while type of [{3}] is {4}.",
                            this.name,
                            x,
                            x.Type,
                            y,
                            y.Type);
                        throw new ParseException(error);
                    }

                    BasicType result = this.type;
                    if (result == BasicType.None)
                    {
                        result = x.Type;
                    }

                    return result;
                }
            }
        }
    }
}
