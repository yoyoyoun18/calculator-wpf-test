using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using CalculatorAppTest.Models;

namespace CalculatorAppTest.ViewModels
{
    /// <summary>
    /// 계산기의 비즈니스 로직을 처리하는 ViewModel 클래스입니다.
    /// MVVM 패턴을 따르며, View와 Model 사이의 중재자 역할을 합니다.
    /// </summary>
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        // 코딩 컨벤션: private 필드를 나타내는 변수는 변수명 앞에 _를 붙였습니다.
        private readonly CalculatorModel _model;
        private string _result = "0";
        private string _expression = "";
        private bool _isNewNumber = true;
        private List<string> _expressionList = new List<string>();

        /// <summary>
        /// 현재 계산 결과를 나타내는 속성입니다.
        /// </summary>
        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        /// <summary>
        /// 현재 입력된 전체 수식을 나타내는 속성입니다.
        /// </summary>
        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                OnPropertyChanged(nameof(Expression));
            }
        }

        // 각 버튼에 대한 Command 속성들
        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand DotCommand { get; }
        public ICommand PlusMinusCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackspaceCommand { get; }

        /// <summary>
        /// CalculatorViewModel의 생성자입니다.
        /// 모든 Command 객체를 초기화합니다.
        /// </summary>
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

        /// <summary>
        /// 숫자 버튼 클릭 시 호출되는 메서드입니다.
        /// </summary>
        /// <param name="number">클릭된 숫자</param>
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

        /// <summary>
        /// 연산자 버튼 클릭 시 호출되는 메서드입니다.
        /// </summary>
        /// <param name="op">클릭된 연산자</param>
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

        /// <summary>
        /// 등호(=) 버튼 클릭 시 호출되는 메서드입니다.
        /// 현재까지의 수식을 계산하고 결과를 표시합니다.
        /// </summary>
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

        /// <summary>
        /// 소수점(.) 버튼 클릭 시 호출되는 메서드입니다.
        /// </summary>
        private void DotButtonClick()
        {
            if (!Result.Contains("."))
            {
                Result += ".";
                _isNewNumber = false;
            }
        }

        /// <summary>
        /// 양수/음수 전환(+/-) 버튼 클릭 시 호출되는 메서드입니다.
        /// </summary>
        private void PlusMinusButtonClick()
        {
            if (double.TryParse(Result, out double number))
            {
                Result = (-number).ToString();
            }
        }

        /// <summary>
        /// CE(Clear Entry) 버튼 클릭 시 호출되는 메서드입니다.
        /// 현재 입력된 숫자만 지웁니다.
        /// </summary>
        private void ClearEntryButtonClick()
        {
            Result = "0";
            _isNewNumber = true;
        }

        /// <summary>
        /// C(Clear) 버튼 클릭 시 호출되는 메서드입니다.
        /// 모든 입력과 계산 내용을 초기화합니다.
        /// </summary>
        private void ClearButtonClick()
        {
            Expression = "";
            Result = "0";
            _expressionList.Clear();
            _isNewNumber = true;
        }

        /// <summary>
        /// Backspace 버튼 클릭 시 호출되는 메서드입니다.
        /// 현재 입력된 숫자의 마지막 자리를 지웁니다.
        /// </summary>
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

        /// <summary>
        /// 속성 변경을 통지하는 메서드입니다.
        /// </summary>
        /// <param name="propertyName">변경된 속성의 이름</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 제네릭 RelayCommand 클래스입니다.
    /// 매개변수가 있는 Command를 구현할 때 사용합니다.
    /// </summary>
    /// <typeparam name="T">Command 매개변수의 타입</typeparam>
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

    /// <summary>
    /// 매개변수가 없는 RelayCommand 클래스입니다.
    /// 매개변수가 필요 없는 Command를 구현할 때 사용합니다.
    /// </summary>
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