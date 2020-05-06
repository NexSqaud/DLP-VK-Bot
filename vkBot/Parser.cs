/* ЭТОТ КОД РАБОТАЕТ ПЛОХО!!!
 * НЕ ИСПОЛЬЗОВАТЬ!!!
 * 
 * 
 */
using System.Text.RegularExpressions;

namespace VKBot
{
    class Parser
    {

        #region Strings
        public static string readOperator(ref int index, string line)
        {
            string result = "";
            if (index < line.Length && Regex.IsMatch(line.Substring(index, 1), "[a-z]|[A-Z]"))
            {
                while(index < line.Length && Regex.IsMatch(line.Substring(index, 1), "[a-z]|[A-Z]|[0-9]|_"))
                {
                    result += line[index];
                    index++;
                }
            }
            return result;
        }

        public static bool isOperator(int index, string line) => readOperator(ref index, line) != "";

        public static string readChar(ref int index, string line)
        {
            string result = "";
            if (index < line.Length)
                if (line.Substring(index,1) == "+"||
                    line.Substring(index, 1) == "-"||
                    line.Substring(index, 1) == "\\"||
                    line.Substring(index, 1) == "/"||
                    line.Substring(index, 1) == "*"||
                    line.Substring(index, 1) == "("||
                    line.Substring(index, 1) == ")"||
                    line.Substring(index, 1) == "["||
                    line.Substring(index, 1) == "]"||
                    line.Substring(index, 1) == ">"||
                    line.Substring(index, 1) == "<"||
                    line.Substring(index, 1) == "="||
                    line.Substring(index, 1) == ";"||
                    line.Substring(index, 1) == ":"||
                    line.Substring(index, 1) == ","||
                    line.Substring(index, 1) == ".") 
                { 
                    result = line.Substring(index, 1);
                    index++;
                }
            return result;
        }

        public static bool isChar(int index, string line) => readChar(ref index, line) != "";

        public static string readDoubleChar(ref int index, string line)
        {
            string result = "";
            if (index < line.Length - 1)
                if (line.Substring(index,2)== "<="||
                    line.Substring(index, 2) == ">="||
                    line.Substring(index, 2) == "!="||
                    line.Substring(index, 2) == ">>"||
                    line.Substring(index, 2) == "<<"||
                    line.Substring(index, 2) == "==")
                {
                    result = line.Substring(index, 2);
                    index += 2;
                }
            return result;
        }

        public static bool isDoubleChar(int index, string line) => readDoubleChar(ref index, line) != "";

        public static string readRem(ref int index, string line)
        {
            string result = "";
            if (index < line.Length - 1)
                if (Regex.IsMatch(line.Substring(index, 2), "//"))
                    result = line.Substring(index, line.Length - index);
            return result;
        }

        public static bool isRem(int index, string line) => readRem(ref index, line) != "";

        public static string readQuotes(ref int index, string line)
        {
            string result = "";
            if (index < line.Length)
                if (Regex.IsMatch(line.Substring(index, 1), "\""))
                {
                    index++;
                    while (index < line.Length && !Regex.IsMatch(line.Substring(index, 1), "\""))
                    {
                        result += line.Substring(index, 1);
                        index++;
                    }
                }
            return result;
        }

        public static bool isQuotes(int index, string line) => readQuotes(ref index, line) != "";

        public static string readNumber(ref int index, string line)
        {
            string result = "";
            if(index < line.Length)
                if(Regex.IsMatch(line.Substring(index, 1), "[0-9]"))
                {
                    while(index < line.Length && Regex.IsMatch(line.Substring(index, 1), "[0-9]"))
                    {
                        result += line.Substring(index, 1);
                        index++;
                    }
                }
            return result;
        }

        public static bool isNumber(int index, string line) => readNumber(ref index, line) != "";

        #endregion

        public static void runCode(string code, long? peerId)
        {
            Execute exec = new Execute();
            exec.RunProgamm(code, peerId);
        }

    }
}
