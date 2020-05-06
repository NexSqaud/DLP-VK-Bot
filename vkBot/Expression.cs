/* ЭТОТ КОД РАБОТАЕТ ПЛОХО!!!
 * НЕ ИСПОЛЬЗОВАТЬ!!!
 * 
 * 
 */
using System.Collections.Generic;

namespace VKBot
{
    class Expression
    {
        public static int LastId = 0;

        public int Id;
        public string Token;
        public ExpressionType Type;
        public string Result;
        public List<Expression> SubExpressions = new List<Expression>();
        public bool IsFunction;
        public Expression Parent;
        public string Error;

        public Expression()
        {
            Id = LastId;
            LastId++;
        }

        public Expression(string token, ExpressionType type)
        {
            Token = token;
            Type = type;
            Result = token;
            Id = LastId;
            LastId++;
        }

        public void Add(Expression expression)
        {
            expression.Parent = this;
            SubExpressions.Add(expression);
        }

        public void Delete(int index) => SubExpressions.RemoveAt(index);

        public Expression Copy(Expression parent)
        {
            var expr = new Expression();
            expr.Parent = parent;
            expr.Token = Token;
            expr.Result = Result;
            expr.Type = Type;
            expr.IsFunction = IsFunction;
            foreach (var expression in SubExpressions)
                expr.SubExpressions.Add(expression.Copy(expr));
            return expr;
        }

        public void Clear() => SubExpressions.Clear();

        public static void CompileSc(string brackets, Expression expression)
        {
            int iter = 0;
            Expression beginExpr = expression;
            Expression process;
            while (iter < expression.SubExpressions.Count)
            {
                process = expression.SubExpressions[iter];
                process.Parent = beginExpr;
                if (beginExpr != expression)
                {
                    if (process.Token != brackets.Substring(1, 1))
                        beginExpr.Add(process);
                    expression.Delete(iter);
                    
                }
                else iter++;
                if (process.Token == brackets.Substring(0, 1))
                    beginExpr = process;
                else if (process.Token == brackets.Substring(1, 1))
                    beginExpr = beginExpr.Parent;
            }
        }

        public static void CompileMultDiv(Expression expression)
        {
            for (int i = 0; i < expression.SubExpressions.Count; i++)
                if (expression.SubExpressions[i].SubExpressions.Count != 0)
                    CompileMultDiv(expression.SubExpressions[i]);
            int iter = 1;
            while (iter < expression.SubExpressions.Count - 1)
            {
                if (expression.SubExpressions[iter].Token == "*" || expression.SubExpressions[iter].Token == "/")
                {
                    expression.SubExpressions[iter - 1].Parent = expression;
                    expression.SubExpressions[iter + 1].Parent = expression;
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter - 1]);
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter + 1]);
                    expression.Delete(iter + 1);
                    expression.Delete(iter - 1);
                }
                else iter++;
            }
        }

        public static void CompileSumSub(Expression expression)
        {
            for (int i = 0; i < expression.SubExpressions.Count; i++)
                if (expression.SubExpressions[i].SubExpressions.Count != 0)
                    CompileSumSub(expression.SubExpressions[i]);
            int iter = 1;
            while (iter < expression.SubExpressions.Count - 1)
            {
                if (expression.SubExpressions[iter].Token == "+" || expression.SubExpressions[iter].Token == "-")
                {
                    expression.SubExpressions[iter - 1].Parent = expression;
                    expression.SubExpressions[iter + 1].Parent = expression;
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter - 1]);
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter + 1]);
                    expression.Delete(iter + 1);
                    expression.Delete(iter - 1);
                }
                else iter++;
            }

        }

        public static void CompileCompare(Expression expression)
        {
            for (int i = 0; i < expression.SubExpressions.Count; i++)
                if (expression.SubExpressions[i].SubExpressions.Count != 0)
                    CompileCompare(expression.SubExpressions[i]);
            int iter = 0;
            while (iter < expression.SubExpressions.Count)
            {
                if (expression.SubExpressions[iter].Token == ">" ||
                    expression.SubExpressions[iter].Token == "<" ||
                    expression.SubExpressions[iter].Token == "==" ||
                    expression.SubExpressions[iter].Token == "<=" ||
                    expression.SubExpressions[iter].Token == ">=" ||
                    expression.SubExpressions[iter].Token == "!=" ||
                    expression.SubExpressions[iter].Token == "&&" ||
                    expression.SubExpressions[iter].Token == "||")
                {
                    expression.SubExpressions[iter - 1].Parent = expression;
                    expression.SubExpressions[iter + 1].Parent = expression;
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter - 1]);
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter + 1]);
                    expression.Delete(iter + 1);
                    expression.Delete(iter - 1);
                }
                else iter++;
            }
        }

        public static void CompileAsign(Expression expression)
        {
            for (int i = 0; i < expression.SubExpressions.Count; i++)
                if (expression.SubExpressions[i].SubExpressions.Count != 0)
                    CompileAsign(expression.SubExpressions[i]);
            int iter = 0;
            while (iter < expression.SubExpressions.Count)
            {
                if (expression.SubExpressions[iter].Token == "=")
                {
                    expression.SubExpressions[iter - 1].Parent = expression;
                    expression.SubExpressions[iter + 1].Parent = expression;
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter - 1]);
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter + 1]);
                    expression.Delete(iter + 1);
                    expression.Delete(iter - 1);
                }
                else iter++;
            }
        }

        public static void CompileParams(Expression expression)
        {
            for (int i = 0; i < expression.SubExpressions.Count; i++)
                if (expression.SubExpressions[i].SubExpressions.Count != 0)
                    CompileParams(expression.SubExpressions[i]);
            int iter = expression.SubExpressions.Count - 2;
            while(iter > -1)
            {
                if(expression.SubExpressions[iter].Type == ExpressionType.Operator &&
                    expression.SubExpressions[iter + 1].Token == "(")
                {
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter + 1]);
                    expression.Delete(iter + 1);
                }
                iter--;
            }
            for (int i = 0; i < expression.SubExpressions.Count; i++)
                if (expression.SubExpressions[i].Type == ExpressionType.Operator)
                    if (expression.SubExpressions[i].SubExpressions.Count == 0)
                        expression.SubExpressions[i].Add(new Expression("(", ExpressionType.Char));
        }

        public static void CompileBlock(Expression expression)
        {
            for(int i = 0;i < expression.SubExpressions.Count;i++)
                if (expression.SubExpressions[i].SubExpressions.Count != 0)
                    CompileBlock(expression.SubExpressions[i]);
            int iter = 0;
            while(iter < expression.SubExpressions.Count - 1)
            {
                if (expression.SubExpressions[iter].Type == ExpressionType.Operator &&
                    expression.SubExpressions[iter + 1].Token == "{")
                {
                    if (expression.SubExpressions[iter].Token != "if" ||
                        expression.SubExpressions[iter].Token != "while" ||
                        expression.SubExpressions[iter].Token != "send")
                        expression.SubExpressions[iter].IsFunction = true;
                    expression.SubExpressions[iter].Add(expression.SubExpressions[iter + 1]);
                    expression.Delete(iter + 1);
                }
                else iter++;
            }
            for (int i = 0; i < expression.SubExpressions.Count; i++)
                if (expression.SubExpressions[i].Type == ExpressionType.Operator &&
                    expression.SubExpressions[i].SubExpressions.Count <= 1)
                    expression.SubExpressions[i].Add(new Expression("{", ExpressionType.Char));
        }

        public static void DeleteUS(Expression expression)
        {
            for(int i = 0; i < expression.SubExpressions.Count; i++)
            {
                if (expression.SubExpressions[i].SubExpressions.Count != 0)
                    DeleteUS(expression.SubExpressions[i]);
            }
            int iter = 0;
            while (iter < expression.SubExpressions.Count)
                if (expression.SubExpressions[iter].Token == " " ||
                   expression.SubExpressions[iter].Token == ";" ||
                   expression.SubExpressions[iter].Token == ",")
                    expression.Delete(iter);
                else iter++;
        }

        public static Expression ReadCode(ref int index, string line)
        {
            var expr = new Expression();
            while(index < line.Length)
            {
                if (Parser.isRem(index, line))
                    Parser.readRem(ref index, line);
                else if (Parser.isOperator(index, line))
                    expr.Add(new Expression(Parser.readOperator(ref index, line), ExpressionType.Operator));
                else if (Parser.isNumber(index, line))
                    expr.Add(new Expression(Parser.readNumber(ref index, line), ExpressionType.Number));
                else if (Parser.isQuotes(index, line))
                    expr.Add(new Expression(Parser.readQuotes(ref index, line), ExpressionType.Quotes));
                else if (Parser.isDoubleChar(index, line))
                    expr.Add(new Expression(Parser.readDoubleChar(ref index, line), ExpressionType.DoubleChar));
                else if (Parser.isChar(index, line))
                    expr.Add(new Expression(Parser.readChar(ref index, line), ExpressionType.Char));
                else {
                    expr.Add(new Expression(line.Substring(index, 1), ExpressionType.Unknown));
                    index++;
                }
            }
            return expr;
        }

        public static void CompileCode(Expression code)
        {
            DeleteUS(code);
            CompileSc("()", code);
            CompileSc("{}", code);
            CompileParams(code);
            CompileBlock(code);
            CompileMultDiv(code);
            CompileSumSub(code);
            CompileCompare(code);
            CompileAsign(code);
        }

    }

    public enum ExpressionType
    {
        Operator,
        Number,
        Char,
        DoubleChar,
        Quotes,
        Comments,
        Unknown
    }
}
