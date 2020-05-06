/* ЭТОТ КОД РАБОТАЕТ ПЛОХО!!!
 * НЕ ИСПОЛЬЗОВАТЬ!!!
 * 
 * 
 */
using System;

namespace VKBot
{
    class Execute
    {

        private Expression findFunction(Expression expression, string name)
        {
            Expression expr = null;
            if (expression.IsFunction && name == expression.Token)
                expr = expression;
            if(expr == null)
                for(int i = expression.SubExpressions.Count; i > 0; i--)
                    if (expression.SubExpressions[i].Type == ExpressionType.Operator &&
                        expression.SubExpressions[i].IsFunction &&
                        expression.SubExpressions[i].Token == name)
                        expr = expression.SubExpressions[i];
            if(expr == null && expression.IsFunction)
                for (int i = 0; i < expression.SubExpressions[0].SubExpressions.Count; i++)
                    if (expression.SubExpressions[0].SubExpressions[i].Token == name)
                        expr = expression.SubExpressions[0].SubExpressions[i];
            if (expr == null && expression.Parent != null)
                expr = findFunction(expression.Parent, name);
            return expr;
        }

        private void runSend(Expression expression, long? peerId)
        {
            string result = "";
            for(int i = 0; i < expression.SubExpressions[0].SubExpressions.Count; i++)
            {
                run(expression.SubExpressions[0].SubExpressions[i], peerId);
                result += expression.SubExpressions[0].SubExpressions[i].Result;
            }
            //Program.sendMessage(peerId, result);
        }

        private void runWhile(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                while (expression.SubExpressions[0].Result == "true")
                {
                    run(expression.SubExpressions[1], peerId);
                    run(expression.SubExpressions[0], peerId);
                }
            }
            else throw new InvalidSyntaxException("Invalid WHILE construstion", expression);
        }

        private void runIf(Expression expression, long? peerId)
        {
            run(expression.SubExpressions[0], peerId);
            if (expression.SubExpressions[0].Result == "true") run(expression.SubExpressions[1], peerId);
            else if (expression.SubExpressions.Count >= 3) run(expression.SubExpressions[2], peerId);
        }

        private void runOr(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (expression.SubExpressions[0].Result == "true" || expression.SubExpressions[1].Result == "true")
                    expression.Result = "true";
                else expression.Result = "false";
            }
            else throw new InvalidSyntaxException("Invalid OR construstion", expression);
        }

        private void runAnd(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (expression.SubExpressions[0].Result == "true" && expression.SubExpressions[1].Result == "true")
                    expression.Result = "true";
                else expression.Result = "false";
            }
            else throw new InvalidSyntaxException("Invalid AND construstion", expression);
        }

        private void runMult(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (!Parser.isNumber(0, expression.SubExpressions[0].Result))
                    expression.SubExpressions[0].Error = "Invalid Value";
                else if (!Parser.isNumber(0, expression.SubExpressions[1].Result))
                    expression.SubExpressions[1].Error = "Invalid Value";
                else
                    expression.Result = $"{int.Parse(expression.SubExpressions[0].Result) * int.Parse(expression.SubExpressions[1].Result)}";
            }
            else throw new InvalidSyntaxException("Invalid MULTIPLY construstion", expression);
        }

        private void runDiv(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (!Parser.isNumber(0, expression.SubExpressions[0].Result))
                    expression.SubExpressions[0].Error = "Invalid Value";
                else if (!Parser.isNumber(0, expression.SubExpressions[1].Result))
                    expression.SubExpressions[1].Error = "Invalid Value";
                else if (int.Parse(expression.SubExpressions[1].Result) == 0)
                    expression.SubExpressions[1].Error = "Divide by zero";
                else
                    expression.Result = $"{int.Parse(expression.SubExpressions[0].Result) / int.Parse(expression.SubExpressions[1].Result)}";
            }
            else throw new InvalidSyntaxException("Invalid DIVIDE construstion", expression);
        }

        private void runSum(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (!Parser.isNumber(0, expression.SubExpressions[0].Result))
                    expression.SubExpressions[0].Error = "Invalid Value";
                else if (!Parser.isNumber(0, expression.SubExpressions[1].Result))
                    expression.SubExpressions[1].Error = "Invalid Value";
                else
                    expression.Result = (int.Parse(expression.SubExpressions[0].Result) + int.Parse(expression.SubExpressions[1].Result)).ToString();
            }
            else throw new InvalidSyntaxException("Invalid SUM construstion", expression);
        }

        private void runSub(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (!Parser.isNumber(0, expression.SubExpressions[0].Result))
                    expression.SubExpressions[0].Error = "Invalid Value";
                else if (!Parser.isNumber(0, expression.SubExpressions[1].Result))
                    expression.SubExpressions[1].Error = "Invalid Value";
                else
                    expression.Result = $"{int.Parse(expression.SubExpressions[0].Result) - int.Parse(expression.SubExpressions[1].Result)}";
            }
            else throw new InvalidSyntaxException("Invalid SUBSTRACTION construstion", expression);
        }

        private void runEQ(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (expression.SubExpressions[0].Result == expression.SubExpressions[1].Result)
                    expression.Result = "true";
                else expression.Result = "false";
            }
            else throw new InvalidSyntaxException("Invalid EQUAL construstion", expression);
        }

        private void runGr(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (!Parser.isNumber(0, expression.SubExpressions[0].Result))
                    expression.SubExpressions[0].Error = "Invalid Value";
                else if (!Parser.isNumber(0, expression.SubExpressions[1].Result))
                    expression.SubExpressions[1].Error = "Invalid Value";
                else expression.Result = $"{(int.Parse(expression.SubExpressions[0].Result) > int.Parse(expression.SubExpressions[1].Result) ? "true" : "false") }";
            }
            else throw new InvalidSyntaxException("Invalid GREAT construstion", expression);
        }

        private void runLess(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (!Parser.isNumber(0, expression.SubExpressions[0].Result))
                    expression.SubExpressions[0].Error = "Invalid Value";
                else if (!Parser.isNumber(0, expression.SubExpressions[1].Result))
                    expression.SubExpressions[1].Error = "Invalid Value";
                else expression.Result = $"{(int.Parse(expression.SubExpressions[0].Result) < int.Parse(expression.SubExpressions[1].Result) ? "true" : "false") }";
            }
            else throw new InvalidSyntaxException("Invalid LESS construstion", expression);
        }

        private void runNQ(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (expression.SubExpressions[0].Result != expression.SubExpressions[1].Result)
                    expression.Result = "true";
                else expression.Result = "false";
            }
            else throw new InvalidSyntaxException("Invalid NOT EQUAL construstion", expression);
        }

        private void runGrEQ(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (!Parser.isNumber(0, expression.SubExpressions[0].Result))
                    expression.SubExpressions[0].Error = "Invalid Value";
                else if (!Parser.isNumber(0, expression.SubExpressions[1].Result))
                    expression.SubExpressions[1].Error = "Invalid Value";
                else expression.Result = $"{(int.Parse(expression.SubExpressions[0].Result) >= int.Parse(expression.SubExpressions[1].Result) ? "true" : "false") }";
            }
            else throw new InvalidSyntaxException("Invalid GREAT EQUAL construstion", expression);
        }

        private void runLessEQ(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[0], peerId);
                run(expression.SubExpressions[1], peerId);
                if (!Parser.isNumber(0, expression.SubExpressions[0].Result))
                    expression.SubExpressions[0].Error = "Invalid Value";
                else if (!Parser.isNumber(0, expression.SubExpressions[1].Result))
                    expression.SubExpressions[1].Error = "Invalid Value";
                else expression.Result = $"{(int.Parse(expression.SubExpressions[0].Result) <= int.Parse(expression.SubExpressions[1].Result) ? "true" : "false") }";
            }
            else throw new InvalidSyntaxException("Invalid LESS EQUAL construstion", expression);
        }

        private void runEqual(Expression expression, long? peerId)
        {
            if (expression.SubExpressions.Count == 2)
            {
                run(expression.SubExpressions[1], peerId);
                expression.SubExpressions[0].Result = expression.SubExpressions[1].Result;
                var function = findFunction(expression.Parent, expression.SubExpressions[0].Token);
                if (function != null) function.Result = expression.SubExpressions[1].Result;
            }
            else throw new InvalidSyntaxException("Invalid construstion", expression);
        }

        private void runBr(Expression expression, long? peerId)
        {
            string result = "";
            for(int i =0; i < expression.SubExpressions.Count; i++)
            {
                run(expression.SubExpressions[i], peerId);
                result += expression.SubExpressions[i].Result;
            }
            expression.Result = result;
        }

        private void run(Expression expression, long? peerId)
        {
            if (expression.Token == "send")
                runSend(expression, peerId);
            else if (expression.Token == "while")
                runWhile(expression, peerId);
            else if (expression.Token == "if")
                runIf(expression, peerId);
            else if (expression.Token == "||")
                runOr(expression, peerId);
            else if (expression.Token == "&&")
                runAnd(expression, peerId);
            else if (expression.Token == "*")
                runMult(expression, peerId);
            else if (expression.Token == "/")
                runDiv(expression, peerId);
            else if (expression.Token == "+")
                runSum(expression, peerId);
            else if (expression.Token == "-")
                runSub(expression, peerId);
            else if (expression.Token == "==")
                runEQ(expression, peerId);
            else if (expression.Token == ">")
                runGr(expression, peerId);
            else if (expression.Token == "<")
                runLess(expression, peerId);
            else if (expression.Token == "!=")
                runNQ(expression, peerId);
            else if (expression.Token == ">=")
                runGrEQ(expression, peerId);
            else if (expression.Token == "<=")
                runLessEQ(expression, peerId);
            else if (expression.Token == "=")
                runEqual(expression, peerId);
            else if (expression.Token == "(")
                runBr(expression, peerId);
            else if (expression.Token == "{")
                runProgramm(expression, peerId);
            else if (expression.Type == ExpressionType.Operator)
            {
                var expr = findFunction(expression.Parent, expression.Token);
                if (expr != null)
                {
                    var copy = expr.Copy(expression.Parent);
                    runProgramm(expression.SubExpressions[0], peerId);
                    for (int i = 0; i < Math.Min(expression.SubExpressions[0].SubExpressions.Count,
                        copy.SubExpressions[0].SubExpressions.Count); i++)
                        copy.SubExpressions[0].SubExpressions[i].Result = expression.SubExpressions[0].SubExpressions[i].Result;
                    runProgramm(copy.SubExpressions[1], peerId);
                    expression.Result = copy.Result;
                    copy.Clear();
                }
                else expression.Error = $"Variable not defined {expression.Token}";
            }
            else if (expression.Type == ExpressionType.Quotes) return;
            else throw new InvalidSyntaxException("SYNTAX PIZDEC", expression);
        }

        private void runProgramm(Expression expression, long? peerId)
        {
            int i = 0;
            while(i < expression.SubExpressions.Count)
            {
                if (!expression.SubExpressions[i].IsFunction)
                    run(expression.SubExpressions[i], peerId);
                i++;
            }
        }

        public void RunProgamm(string code, long? peerId)
        {
            int index = 0;
            Expression expression = Expression.ReadCode(ref index, code);
            Expression.CompileCode(expression);
            runProgramm(expression, peerId);
        }

        public void RunProgamm(string code)
        {
            int index = 0;
            Expression expression = Expression.ReadCode(ref index, code);
            Expression.CompileCode(expression);
            runProgramm(expression, 237912524);
        }

    }
}
