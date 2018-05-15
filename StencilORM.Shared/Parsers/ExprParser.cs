using System;
using StencilORM.Queries;
using System.Text;
using System.Collections.Generic;

namespace StencilORM.Parsers
{
    public class ExprParser : IExprParser
    {
        private static ExprParser instance;

        public static ExprParser Instance
        {
            get
            {
                if (instance == null)
                    instance = new ExprParser();
                return instance;
            }
        }

        private ExprParser()
        {
        }

        public IExpr Parse(string source)
        {
            return new ExprParserState(source).DoFinal();
        }
    }

    class ExprParserState
    {

        private int position = -1, len, currentChar;
        private readonly string source;

        public ExprParserState(string source)
        {
            this.source = source ?? "";
            this.len = this.source.Length;
        }

        private void NextChar()
        {
            if (position < source.Length)
                position++;
            currentChar = (position < source.Length) ? source[position] : -1;
        }

        private void ConsumeWhiteSpace()
        {
            while (currentChar == ' ' || currentChar == '\t' || currentChar == '\n')
                NextChar();
        }

        private string ConsumeString()
        {
            StringBuilder builder = new StringBuilder();
            int start = position;
            bool escaped = false;
            for (;currentChar != -1; NextChar())
            {
                if (!escaped)
                {
                    if (currentChar == '\'')
                        break;
                    if (currentChar == '\\') {
                        escaped = true;
                        builder.Append(source, start, position - start);
                        start = position + 1;
                    }
                } else {
                    escaped = false;
                    switch(currentChar) {
                        case 'n':
                            builder.Append("\n");
                            break;
                        case 't':
                            builder.Append("\t");
                            break;
                        case '\'':
                            builder.Append("'");
                            break;
                        default:
                            builder.Append(currentChar);
                            break;
                            
                    }
                    start++;
                }
            }
            builder.Append(source, start, position - start);
            return builder.ToString();
        }

        private bool Consume(int character)
        {
            ConsumeWhiteSpace();
            if (currentChar == character)
            {
                NextChar();
                return true;
            }
            return false;
        }

        private void Fail()
        {
            if (currentChar == -1)
                throw new Exception("Unexpected end of source");
            throw new Exception(string.Format("Unexpected character: '{0}'", (char)currentChar));
        }

        private bool ConsumeOrFail(char character)
        {
            if (!Consume(character))
                Fail();
            return true;
        }

        private bool ConsumeCurrentOrFail(char character)
        {
            if (currentChar != character)
                Fail();
            NextChar();
            return true;
        }

        public IExpr DoFinal()
        {
            NextChar();
            IExpr result = ParseFromTheTop();
            if (position < source.Length)
                Fail();
            return result;
        }

        private IExpr ParseFromTheTop()
        {
            return ParseIf();
        }

        private IExpr ParseIf()
        {
            IExpr result = ParseLogicalExpression();
            for (;;)
            {
                if (Consume('?'))
                {
                    var thenExpr = ParseFromTheTop();
                    ConsumeOrFail(':');
                    var elseExpr = ParseFromTheTop();
                    result = new If(result, thenExpr, elseExpr);
                }
                else
                    return result;
            }
        }

        protected IExpr ParseLogicalExpression()
        {
            IExpr result = ParseCompareExpression();
            for (;;)
            {
                if (Consume('&') && ConsumeOrFail('&'))
                {
                    IExpr right = ParseCompareExpression();
                    result = Expr.And(result, right);
                }
                else if (Consume('|') && ConsumeOrFail('|'))
                {
                    IExpr right = ParseCompareExpression();
                    result = Expr.Or(result, right);
                }
                else
                    return result;
            }
        }

        private IExpr ParseCompareExpression()
        {
            IExpr result = ParseArithmetic();
            for (;;)
            {
                if (Consume('<'))
                {
                    bool orEqual = Consume('='); 
                    var right = ParseArithmetic();
                    result = (orEqual ? Expr.LtE(result, right) : Expr.Lt(result, right));
                }
                else if (Consume('>'))
                {
                    bool orEqual = Consume('=');
                    var right = ParseArithmetic();
                    result = (orEqual ? Expr.GtE(result, right) : Expr.Gt(result, right));
                }
                else if (Consume('=') && ConsumeOrFail('='))
                {
                    result = Expr.Eq(result, ParseArithmetic());
                }
                else if (Consume('!') && ConsumeOrFail('='))
                {
                    result = Expr.NotEq(result, ParseArithmetic());
                }
                else
                    return result;
            }
        }


        private IExpr ParseArithmetic()
        {
            IExpr result = ParseArithmeticHigherPrecedence();
            for (;;)
            {
                if (Consume('+'))
                    result = Expr.Add(result, ParseArithmeticHigherPrecedence());
                else if (Consume('-'))
                    result = Expr.Sub(result, ParseArithmeticHigherPrecedence());
                else
                    return result;
            }
        }

        private IExpr ParseArithmeticHigherPrecedence()
        {
            IExpr result = ParseValue();
            for (;;)
            {
                if (Consume('*'))
                    result = Expr.Mul(result, ParseValue());
                else if (Consume('/'))
                    result = Expr.Div(result, ParseValue());
                else
                    return result;
            }
        }

        private IExpr ParseValue()
        {
            bool negate = false;
            if (Consume('+'))
                return ParseValue();
            if (Consume('-'))
                negate = true;
            if (Consume('!'))
                return Expr.Not(ParseValue());
            IExpr result = null;
            int startPos = this.position;
            if (Consume('('))
            {
                result = ParseFromTheTop();
                ConsumeOrFail(')');
            }
            else if (Consume('\''))
            { 
                result = (Literal) ConsumeString();
                ConsumeCurrentOrFail('\'');
            }
            else if ((currentChar >= '0' && currentChar <= '9') || currentChar == '.')
            {
                bool isDecimal = false;
                while ((currentChar >= '0' && currentChar <= '9') || currentChar == '.')
                {
                    if (currentChar == '.')
                        isDecimal = true;
                    NextChar();
                }
                var value = source.Substring(startPos, this.position - startPos);
                if (isDecimal)
                    result = (Literal)decimal.Parse(value);
                else
                    result = (Literal)long.Parse(value);
            }
            else if ((currentChar >= 'a' && currentChar <= 'z') 
                     || (currentChar >= 'A' && currentChar <= 'Z')
                     || currentChar == '_'
                     || currentChar == '$')
            {
                bool isParam = false;
                if (currentChar == '$') {
                    startPos++;
                    NextChar();
                    isParam = true;
                }
                    
                while ((currentChar >= 'a' && currentChar <= 'z')
                        || (currentChar >= 'A' && currentChar <= 'Z')
                        || (currentChar >= '0' && currentChar <= '9')
                        || currentChar == '_'
                        || currentChar == '.')
                    NextChar();
                string name = source.Substring(startPos, this.position - startPos);
                if (isParam)
                    result = (Param)name;
                else if (name == "true")
                    result = (Literal)true;
                else if (name == "false")
                    result = (Literal)false;
                else if (name == "null")
                    result = Literal.NULL;
                else if (Consume('(')) {
                    List<IExpr> args = new List<IExpr>();
                    do
                    {
                        args.Add(ParseFromTheTop());
                    } while (Consume(','));
                    ConsumeOrFail(')');
                    result = new Function(name, args.ToArray());
                }  
                else
                    result = (Variable)name;
               
            }
            else
            {
                Fail();
            }
            // if (Consume('^')) result = Math.pow(result, ParseValue());
            if (result != null && negate)
                result = result.Negate();
            return result;
        }
    }
}
