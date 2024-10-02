using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using CalculatorAppTest.Models;

namespace CalculatorAppTest.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {

        // 코딩 컨벤션: private 필드를 나타내는 변수는 변수명 앞에 _를 붙였습니다.
        private readonly CalculatorModel _model;
        private string _result = "0";
        private string _expression = "";
        private bool _isNewNumber = true;
        private List<string> _expressionList = new List<string>();

        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                OnPropertyChanged(nameof(Expression));
            }
        }

        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand DotCommand { get; }
        public ICommand PlusMinusCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackspaceCommand { get; }

        public CalculatorViewModel()
        {
            _model = new CalculatorModel();
            NumberCommand = new RelayCommand<string>(NumberButtonClick);
            OperatorCommand = new RelayCommand<string>(OperatorButtonClick);
            EqualsCommand = new RelayCommand(EqualsButtonClick);
            DotCommand = new RelayCommand(DotButtonClick);
            PlusMinusCommand = new RelayCommand(PlusMinusButtonClick);
            ClearEntryCommand = new RelayCommand(ClearEntryButtonClick);
            ClearCommand = new RelayCommand(ClearButtonClick);
            BackspaceCommand = new RelayCommand(BackspaceButtonClick);
        }

        private void NumberButtonClick(string number)
        {
            if (_isNewNumber)
            {
                Result = number;
                _isNewNumber = false;
            }
            else
            {
                Result += number;
            }
        }

        private void OperatorButtonClick(string op)
        {
            if (!_isNewNumber)
            {
                _expressionList.Add(Result);
                _expressionList.Add(op);
                Expression += Result + " " + op + " ";
                _isNewNumber = true;
            }
            else if (_expressionList.Count > 0)
            {
                _expressionList[_expressionList.Count - 1] = op;
                Expression = Expression.Substring(0, Expression.Length - 2) + op + " ";
            }
        }

        private void EqualsButtonClick()
        {
            if (!_isNewNumber)
            {
                _expressionList.Add(Result);
                Expression += Result + " =";

                try
                {
                    double result = _model.CalculateExpression(_expressionList);
                    Result = result.ToString();
                }
                catch (DivideByZeroException)
                {
                    Result = "Error: 0으로 나눌 수 없습니다.";
                }

                _expressionList.Clear();
                _isNewNumber = true;
            }
        }

        private void DotButtonClick()
        {
            if (!Result.Contains("."))
            {
                Result += ".";
                _isNewNumber = false;
            }
        }

        private void PlusMinusButtonClick()
        {
            if (double.TryParse(Result, out double number))
            {
                Result = (-number).ToString();
            }
        }

        private void ClearEntryButtonClick()
        {
            Result = "0";
            _isNewNumber = true;
        }

        private void ClearButtonClick()
        {
            Expression = "";
            Result = "0";
            _expressionList.Clear();
            _isNewNumber = true;
        }

        private void BackspaceButtonClick()
        {
            if (Result.Length > 1)
            {
                Result = Result.Substring(0, Result.Length - 1);
            }
            else
            {
                Result = "0";
                _isNewNumber = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}