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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void numBtn_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (ResultTextBox.Text == "0")
            {
                ResultTextBox.Text = btn?.Content.ToString();
            }
            else {
                ResultTextBox.Text += btn?.Content.ToString();
            }
        }
    }
}