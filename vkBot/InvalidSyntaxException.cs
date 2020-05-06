using System;

namespace VKBot
{
    class InvalidSyntaxException : Exception
    {

        public Expression expression;
        public string error;

        public InvalidSyntaxException(string error, Expression expression)
        {
            this.error = error;
            this.expression = expression;
        }

    }
}
