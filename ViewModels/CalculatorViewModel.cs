﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CalculatorAppTest.Models;
using System.Collections.ObjectModel;

namespace CalculatorAppTest.ViewModels
{
    /// <summary>
    /// 계산기의 비즈니스 로직을 처리하는 ViewModel 클래스입니다.
    /// MVVM 패턴을 따르며, View와 Model 사이의 중재자 역할을 합니다.
    /// </summary>
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _model;
        private string _result = "0";
        private string _expression = "";
        private bool _isNewNumber = true;
        private List<string> _expressionList = new List<string>();
        private ObservableCollection<string> _history = new ObservableCollection<string>();

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

        /// <summary>
        /// 계산 기록을 나타내는 속성입니다.
        /// </summary>
        public ObservableCollection<string> History
        {
            get => _history;
            set
            {
                _history = value;
                OnPropertyChanged(nameof(History));
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
        public ICommand ParenthesisCommand { get; }
        public ICommand UseHistoryCommand { get; }

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
            ParenthesisCommand = new RelayCommand<string>(ParenthesisButtonClick);
            UseHistoryCommand = new RelayCommand<string>(UseHistoryClick);
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
                string lastItem = _expressionList[_expressionList.Count - 1];
                if (lastItem == ")" || double.TryParse(lastItem, out _))
                {
                    _expressionList.Add(op);
                    Expression += op + " ";
                }
                else if (lastItem != "(")
                {
                    _expressionList[_expressionList.Count - 1] = op;
                    Expression = Expression.Substring(0, Expression.Length - 2) + op + " ";
                }
            }
        }

        /// <summary>
        /// 등호(=) 버튼 클릭 시 호출되는 메서드입니다.
        /// 현재까지의 수식을 계산하고 결과를 표시합니다.
        /// </summary>
        private void EqualsButtonClick()
        {
            if (!_isNewNumber || _expressionList.Count > 0)
            {
                if (!_isNewNumber)
                {
                    _expressionList.Add(Result);
                }
                Expression += Result + " =";

                try
                {
                    double result = _model.CalculateExpression(_expressionList);
                    Result = result.ToString();
                    AddToHistory($"{Expression} {Result}");
                }
                catch (DivideByZeroException)
                {
                    MessageBox.Show("0으로 나눌 수 없습니다.", "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                    Result = "Error";
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                    Result = "Error";
                }
                catch (Exception)
                {
                    MessageBox.Show("잘못된 수식입니다.", "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                    Result = "Error";
                }

                _expressionList.Clear();
                ClearButtonClick();
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

        /// <summary>
        /// 괄호 버튼 클릭 시 호출되는 메서드입니다.
        /// 여는 괄호 또는 닫는 괄호를 추가합니다.
        /// </summary>
        /// <param name="parenthesis">클릭된 괄호 ("(" 또는 ")")</param>
        private void ParenthesisButtonClick(string parenthesis)
        {
            if (_isNewNumber)
            {
                _expressionList.Add(parenthesis);
                Expression += parenthesis + " ";
            }
            else
            {
                _expressionList.Add(Result);
                _expressionList.Add(parenthesis);
                Expression += Result + " " + parenthesis + " ";
                _isNewNumber = true;
            }
        }

        /// <summary>
        /// 계산 기록에 새로운 항목을 추가합니다.
        /// </summary>
        /// <param name="calculation">추가할 계산 기록</param>
        private void AddToHistory(string calculation)
        {
            History.Insert(0, calculation);
            if (History.Count > 5)
            {
                History.RemoveAt(5);
            }
        }

        /// <summary>
        /// 계산 기록 항목을 클릭했을 때 호출되는 메서드입니다.
        /// 선택한 계산 기록을 현재 수식으로 설정합니다.
        /// </summary>
        /// <param name="historyItem">선택된 계산 기록 항목</param>
        private void UseHistoryClick(string historyItem)
        {
            string[] parts = historyItem.Split('=');
            if (parts.Length == 2)
            {
                Expression = parts[0].Trim();
                Result = parts[1].Trim();
                _expressionList.Clear();
                _expressionList.AddRange(Expression.Split(' '));
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