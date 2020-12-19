using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventCode18
{
    class Program
    {
        private const char Add = '+';
        private const char Mult = '*';
        private const char LowerPrecedenceMult = 'X';
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            long solution1 = lines
                .Select(e => EvaluateExpression(e))
                .Sum();
            Console.WriteLine($"Solution 1: {solution1}");

            long solution2 = lines
                .Select(e => e.Replace(Mult, LowerPrecedenceMult))
                .Select(e => EvaluateExpression(e))
                .Sum();
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static long EvaluateExpression(string exp)
        {
            Stack<char> operations = new Stack<char>();
            Stack<long> values = new Stack<long>();
            int i = 0;
            while (i < exp.Length)
            {
                var c = exp[i];
                switch (c)
                {
                    case Add:
                    case Mult:
                    case LowerPrecedenceMult:
                    case '(':
                        operations.Push(c);
                        break;
                    case ' ':
                        break;
                    case ')':
                        Reduce(values, operations);
                        OperateLiteral(values.Pop(), operations, values);
                        break;
                    default:
                        // LITERAL
                        long value = GetLiteral(exp, ref i);
                        OperateLiteral(value, operations, values); ;
                        break;
                };
                i++;
            }

            Reduce(values, operations);
            if (operations.Count() > 0 || values.Count() != 1)
            {
                throw new ArgumentException();
            }
            return values.Pop();
        }

        private static void Reduce(Stack<long> values, Stack<char> operations)
        {
            while (operations.Count() > 0 && !operations.Peek().Equals('('))
            {
                long resultValue = Operate(operations.Pop(), values.Pop(), values.Pop());
                values.Push(resultValue);
            }
            // pop '('
            if (operations.Count() > 0)
            {
                operations.Pop();
            }
        }

        private static void OperateLiteral(long value, Stack<char> operations, Stack<long> values)
        {
            long resultValue;
            char previousOperation = operations.Count() > 0 ? operations.Peek() : '(';
            switch (previousOperation)
            {
                case Add:
                case Mult:
                    resultValue = Operate(previousOperation, values.Pop(), value);
                    operations.Pop();
                    values.Push(resultValue);
                    break;
                case LowerPrecedenceMult:
                    resultValue = value;
                    values.Push(resultValue);
                    break;
                default:
                    resultValue = value;
                    values.Push(resultValue);
                    break;
            }
        }

        private static long GetLiteral(string exp, ref int i)
        {
            StringBuilder valueBuilder = new StringBuilder();
            while (i < exp.Length && char.IsDigit(exp[i]))
            {
                valueBuilder.Append(exp[i]);
                i++;
            }
            i--;
            var value = long.Parse(valueBuilder.ToString());
            return value;
        }

        private static long Operate(char op, long v1, long v2) =>
            op switch
            {
                Add => v1 + v2,
                Mult => v1 * v2,
                LowerPrecedenceMult => v1 * v2,
                _ => throw new ArgumentException(),
            };
    }
}
