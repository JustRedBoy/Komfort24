using Desktop.Commands;
using System;

namespace Desktop.ViewModels
{
    public class TransitionViewModel : ViewModelBase
    {
        private double _transitionProgressValue = 0.0;
        private string _transitionInfo = "";

        public double TransitionProgressValue
        {
            get { return _transitionProgressValue; }
            set
            {
                _transitionProgressValue = value;
                OnPropertyChanged("TransitionProgressValue");
            }
        }
        public string TransitionInfo
        {
            get { return _transitionInfo; }
            set
            {
                _transitionInfo = value;
                OnPropertyChanged("TransitionInfo");
            }
        }

        private RelayCommand _transitionCommand;
        public RelayCommand TransitionCommand
        {
            get
            {
                return _transitionCommand ??= new RelayCommand(async obj =>
                {
                    try
                    {
                        TransitionToNewMonthCommand.UpdateProgress += UpdateProgress;
                        bool isSuccessful = await TransitionToNewMonthCommand.StartTransitionAsync();
                        TransitionCompleted(isSuccessful ? "Все операции были успешно выполнены!"
                            : "Переход на новый месяц уже был выполнен в этом месяце!");
                    }
                    catch (Exception e)
                    {
                        TransitionCompleted("Произошла ошибка, повторите операцию! " + e.Message);
                    }
                    finally
                    {
                        RelayCommand.RaiseCanExecuteChanged();
                    }
                },                
                obj => !AppViewModel.IsAnyProcessing());
            }
        }

        private void TransitionCompleted(string message)
        {
            TransitionProgressValue = 0.0;
            TransitionInfo = message;     
            TransitionToNewMonthCommand.UpdateProgress -= UpdateProgress;
        }

        private void UpdateProgress(double value, string message)
        {
            TransitionProgressValue = value;
            TransitionInfo = message + $" {Math.Round(value, 2)}%";
        }
    }
}
