using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CalculatorAppTest
{
    public partial class MainWindow : Window
    {
        private List<double> numbers = new List<double>();
        private List<string> operations = new List<string>();
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
                numbers.Add(double.Parse(ResultTextBox.Text));
                operations.Add(op);
                ExpressionTextBox.Text += ResultTextBox.Text + " " + op + " ";
                isNewNumber = true;
            }
            else if (operations.Count > 0)
            {
                operations[operations.Count - 1] = op;
                ExpressionTextBox.Text = ExpressionTextBox.Text.Substring(0, ExpressionTextBox.Text.Length - 2) + op + " ";
            }
        }

        private void equalBtn_click(object sender, RoutedEventArgs e)
        {
            if (!isNewNumber)
            {
                numbers.Add(double.Parse(ResultTextBox.Text));
                ExpressionTextBox.Text += ResultTextBox.Text + " =";

                double result = numbers[0];
                for (int i = 0; i < operations.Count; i++)
                {
                    switch (operations[i])
                    {
                        case "+":
                            result += numbers[i + 1];
                            break;
                        case "-":
                            result -= numbers[i + 1];
                            break;
                        case "*":
                            result *= numbers[i + 1];
                            break;
                        case "/":
                            if (numbers[i + 1] != 0)
                                result /= numbers[i + 1];
                            else
                                MessageBox.Show("0으로 나눌 수 없습니다.");
                            break;
                    }
                }

                ResultTextBox.Text = result.ToString();
                numbers.Clear();
                operations.Clear();
                isNewNumber = true;
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
            numbers.Clear();
            operations.Clear();
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