using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CalculatorAppTest
{
    public partial class MainWindow : Window
    {
        private List<string> expression = new List<string>();
        private bool isNewNumber = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void numBtn_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (isNewNumber)
            {
                ResultTextBox.Text = btn?.Content.ToString();
                isNewNumber = false;
            }
            else
            {
                ResultTextBox.Text += btn?.Content.ToString();
            }
        }

        private void opBtn_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string op = btn?.Content.ToString();

            if (!isNewNumber)
            {
                expression.Add(ResultTextBox.Text);
                expression.Add(op);
                ExpressionTextBox.Text += ResultTextBox.Text + " " + op + " ";
                isNewNumber = true;
            }
            else if (expression.Count > 0)
            {
                expression[expression.Count - 1] = op;
                ExpressionTextBox.Text = ExpressionTextBox.Text.Substring(0, ExpressionTextBox.Text.Length - 2) + op + " ";
            }
        }

        private void equalBtn_click(object sender, RoutedEventArgs e)
        {
            if (!isNewNumber)
            {
                expression.Add(ResultTextBox.Text);
                ExpressionTextBox.Text += ResultTextBox.Text + " =";

                double result = CalculateExpression(expression);

                ResultTextBox.Text = result.ToString();
                expression.Clear();
                isNewNumber = true;
            }
        }

        private double CalculateExpression(List<string> exp)
        {
            List<string> postfix = ConvertToPostfix(exp); // 연산자 우선순위를 적용하기위한 후위 표기법으로의 컨버팅
            Stack<double> stack = new Stack<double>(); // 후위 표기법을 진행하면서 계산값이 계속 저장될 스택 선언


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
                                MessageBox.Show("0으로 나눌 수 없습니다.");
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
                    while (stack.Count > 0 && operatorPrecedence(stack.Peek()) >= operatorPrecedence(token))
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
        private int operatorPrecedence(string op)
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

        private void dotBtn_click(object sender, RoutedEventArgs e)
        {
            if (!ResultTextBox.Text.Contains("."))
            {
                ResultTextBox.Text += ".";
                isNewNumber = false;
            }
        }

        private void plusMinusBtn_click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(ResultTextBox.Text, out double number))
            {
                ResultTextBox.Text = (-number).ToString();
            }
        }

        private void ceBtn_click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text = "0";
            isNewNumber = true;
        }

        private void cBtn_Click(object sender, RoutedEventArgs e)
        {
            ExpressionTextBox.Text = "";
            ResultTextBox.Text = "0";
            expression.Clear();
            isNewNumber = true;
        }

        private void backspaceBtn_click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBox.Text.Length > 1)
            {
                ResultTextBox.Text = ResultTextBox.Text.Substring(0, ResultTextBox.Text.Length - 1);
            }
            else
            {
                ResultTextBox.Text = "0";
                isNewNumber = true;
            }
        }
    }
}