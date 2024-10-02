using System;
using System.Collections.Generic;

namespace CalculatorAppTest.Models
{
    public class CalculatorModel
    {

        //연산자 우선순위를 적용하기위해 후위 표현식으로 계산을 진행하는 메서드
        public double CalculateExpression(List<string> exp)
        {
            List<string> postfix = ConvertToPostfix(exp);// 연산자 우선순위를 적용하기위한 후위 표기법으로의 컨버팅
            Stack<double> stack = new Stack<double>();// 후위 표기법을 진행하면서 계산값이 계속 저장될 스택 선언

            /*
             foreach문으로 요소를 순회하다가 숫자를 만나면 stack에 push 하고 연산자를 만나면 stack에 쌓인 요소 두개를 꺼내어 
             해당 연산자에 따른 계산 진행             
             */

            foreach (string token in postfix)
            {
                if (double.TryParse(token, out double number))
                {
                    stack.Push(number);
                }
                else
                {
                    double b = stack.Pop();
                    double a = stack.Pop();
                    switch (token)
                    {
                        case "+": stack.Push(a + b); break;
                        case "-": stack.Push(a - b); break;
                        case "*": stack.Push(a * b); break;
                        case "/":
                            if (b != 0)
                                stack.Push(a / b);
                            else
                                throw new DivideByZeroException("0으로 나눌 수 없습니다.");
                            break;
                    }
                }
            }

            return stack.Pop();
        }

        //중위 표기법을 후위 표기법으로 변환하는 메서드
        private List<string> ConvertToPostfix(List<string> infix)
        {
            List<string> postfix = new List<string>();
            Stack<string> stack = new Stack<string>();

            foreach (string token in infix)
            {
                if (double.TryParse(token, out _))
                {
                    postfix.Add(token);
                }
                else
                {
                    while (stack.Count > 0 && OperatorPrecedence(stack.Peek()) >= OperatorPrecedence(token))
                    {
                        postfix.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
            }

            while (stack.Count > 0)
            {
                postfix.Add(stack.Pop());
            }

            return postfix;
        }

        //연산자의 우선순위를 정하는 메서드
        private int OperatorPrecedence(string op)
        {
            switch (op)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 0;
            }
        }
    }
}