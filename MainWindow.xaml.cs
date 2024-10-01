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
        double saved = 0;
        string op = string.Empty;
        bool isOp = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void numBtn_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (ResultTextBox.Text == "0" || isOp == true)
            {
                ResultTextBox.Text = btn?.Content.ToString();
                isOp = false;
            }
            else {
                ResultTextBox.Text += btn?.Content.ToString();
            }
        }

        private void opBtn_click(object sender, RoutedEventArgs e)
        {
            isOp = true;
            saved = double.Parse(ResultTextBox.Text);
            Button btn = sender as Button;  
            op = btn?.Content.ToString();
            ExpressionTextBox.Text = saved + " " + op;
        }

        private void equalBtn_click(object sender, RoutedEventArgs e)
        {
            double tempNum = double.Parse(ResultTextBox.Text);

            switch(op)
            {
                case "+":
                    ResultTextBox.Text = (saved + tempNum).ToString();
                    break;

                case "-":
                    ResultTextBox.Text = (saved - tempNum).ToString();
                    break;

                case "*":
                    ResultTextBox.Text = (saved * tempNum).ToString();
                    break;

                case "/":
                    ResultTextBox.Text = (saved / tempNum).ToString();
                    break;
            }
            ExpressionTextBox.Text = saved + " " + op + " " + tempNum + " =";
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
            ResultTextBox.Text="0";
             saved = 0;
             op = string.Empty;
             isOp = false;
        }
        //백스페이스 버튼 클릭 함수
        private void backspaceBtn_click(object sender, RoutedEventArgs e)
        {
           ResultTextBox.Text = ResultTextBox.Text.Remove(ResultTextBox.Text.Length - 1);
        }
    }
}