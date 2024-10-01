using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalculatorAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double currentValue = 0;
        string currentOp = string.Empty;
        bool isNewNumber = true;
        List<string> expressionParts = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void numBtn_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (ResultTextBox.Text == "0" || isNewNumber)
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
            if (!isNewNumber)
            {
                CalculateResult();
            }

            Button btn = sender as Button;
            currentOp = btn?.Content.ToString();
            isNewNumber = true;

            UpdateExpressionTextBox();
        }

        private void equalBtn_click(object sender, RoutedEventArgs e)
        {
            CalculateResult();
            currentOp = string.Empty;
            isNewNumber = true;
            expressionParts.Clear();
            UpdateExpressionTextBox();
        }

        private void CalculateResult()
        {
            double tempNum = double.Parse(ResultTextBox.Text);
            expressionParts.Add(tempNum.ToString());

            if (currentOp != string.Empty)
            {
                expressionParts.Add(currentOp);
            }

            switch (currentOp)
            {
                case "+":
                    currentValue += tempNum;
                    break;
                case "-":
                    currentValue -= tempNum;
                    break;
                case "*":
                    currentValue *= tempNum;
                    break;
                case "/":
                    if (tempNum != 0)
                        currentValue /= tempNum;
                    else
                        MessageBox.Show("0으로 나눌 수 없습니다.");
                    break;
                default:
                    currentValue = tempNum;
                    break;
            }

            ResultTextBox.Text = currentValue.ToString();
        }
        private void UpdateExpressionTextBox()
        {
            ExpressionTextBox.Text = string.Join(" ", expressionParts) + (currentOp != string.Empty ? " " + currentOp : "");
        }
        private void dotBtn_click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBox.Text.Contains("."))
            {
                return;
            }
            else
            {
                ResultTextBox.Text += ".";
            }
        }

        private void plusMinusBtn_click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text = (-double.Parse(ResultTextBox.Text)).ToString();
        }

        // 입력 값 초기화 버튼 ce 클릭 함수
        private void ceBtn_click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text = "0";
        }

        //전체 계산 기록 초기화 버튼 c 클릭 함수
        private void cBtn_Click(object sender, RoutedEventArgs e)
        {
            ExpressionTextBox.Text = "";
            ResultTextBox.Text = "0";
            currentValue = 0;
            currentOp = string.Empty;
            isNewNumber = true;
            expressionParts.Clear();
        }
        //백스페이스 버튼 클릭 함수
        private void backspaceBtn_click(object sender, RoutedEventArgs e)
        {
           ResultTextBox.Text = ResultTextBox.Text.Remove(ResultTextBox.Text.Length - 1);
            if (ResultTextBox.Text.Length == 0) {
                ResultTextBox.Text = "0";
            }
        }
    }
}