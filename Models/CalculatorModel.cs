using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculatorAppTest.Models
{
    public class CalculatorModel
    {
        public double CalculateExpression(List<string> expressionList)
        {
            try
            {
                var postfixExpression = ConvertToPostfix(expressionList);
                return EvaluatePostfix(postfixExpression);
            }
            catch (DivideByZeroException)
            {
                throw new CalculatorException("0으로 나눌 수 없습니다.");
            }
            catch (InvalidOperationException ex)
            {
                throw new CalculatorException(ex.Message);
            }
            catch (Exception)
            {
                throw new CalculatorException("잘못된 수식입니다.");
            }
        }

        private List<string> ConvertToPostfix(List<string> infixExpression)
        {
            var postfix = new List<string>();
            var operatorStack = new Stack<string>();

            foreach (var token in infixExpression)
            {
                if (double.TryParse(token, out _))
                {
                    postfix.Add(token);
                }
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                else if (token == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        postfix.Add(operatorStack.Pop());
                    }
                    if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                    {
                        operatorStack.Pop();
                    }
                    else
                    {
                        throw new CalculatorException("괄호가 맞지 않습니다.");
                    }
                }
                else // Operator
                {
                    while (operatorStack.Count > 0 && Precedence(operatorStack.Peek()) >= Precedence(token))
                    {
                        postfix.Add(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
            }

            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek() == "(")
                {
                    throw new CalculatorException("괄호가 맞지 않습니다.");
                }
                postfix.Add(operatorStack.Pop());
            }

            return postfix;
        }

        private int Precedence(string op)
        {
            switch (op)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                case "%":
                    return 2;
                default:
                    return 0;
            }
        }

        private double EvaluatePostfix(List<string> postfixExpression)
        {
            var stack = new Stack<double>();

            foreach (var token in postfixExpression)
            {
                if (double.TryParse(token, out double number))
                {
                    stack.Push(number);
                }
                else
                {
                    if (stack.Count < 2)
                    {
                        throw new CalculatorException("잘못된 수식입니다.");
                    }
                    double b = stack.Pop();
                    double a = stack.Pop();
                    switch (token)
                    {
                        case "+": stack.Push(a + b); break;
                        case "-": stack.Push(a - b); break;
                        case "*": stack.Push(a * b); break;
                        case "/":
                            if (b == 0) throw new DivideByZeroException();
                            stack.Push(a / b);
                            break;
                        case "%": stack.Push(a % b); break;
                        default:
                            throw new CalculatorException($"알 수 없는 연산자입니다: {token}");
                    }
                }
            }

            if (stack.Count != 1)
            {
                throw new CalculatorException("잘못된 수식입니다.");
            }

            return stack.Pop();
        }
    }
    // 에러 처리 메서드
    public class CalculatorException : Exception
    {
        public CalculatorException(string message) : base(message) { }
    }
}