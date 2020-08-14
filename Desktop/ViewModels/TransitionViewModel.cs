using Desktop.Commands;
using System;

namespace Desktop.ViewModels
{
    public class TransitionViewModel : ViewModelBase
    {
        private int _maxTransitionProgressValue = 14;
        private int _transitionProgressValue = 0;
        private string _transitionInfo = "";

        public int TransitionProgressValue
        {
            get { return _transitionProgressValue; }
            set
            {
                _transitionProgressValue = value;
                OnPropertyChanged("TransitionProgressValue");
            }
        }
        public int MaxTransitionProgressValue
        {
            get { return _maxTransitionProgressValue; }
            set
            {
                _maxTransitionProgressValue = value;
                OnPropertyChanged("MaxTransitionProgressValue");
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
                return _transitionCommand ??
                  (_transitionCommand = new RelayCommand(async obj =>
                  {
                      try
                      {
                          TransitionToNewMonth.UpdateProgress += UpdateProgress;
                          bool isSuccessful = await TransitionToNewMonth.StartTransitionAsync();
                          TransitionCompleted(isSuccessful ? "Все операции были успешно выполнены!" 
                              : "Переход на новый месяц уже был выполнен в этом месяце!");
                      }
                      catch (Exception e)
                      {
                          TransitionCompleted(e.Message);
                      }
                      finally
                      {
                          RelayCommand.RaiseCanExecuteChanged();
                      }
                  },                
                  obj => !AppViewModel.IsAnyProcessing()));
            }
        }

        private void TransitionCompleted(string message)
        {
            TransitionProgressValue = 0;
            TransitionInfo = message;     
            TransitionToNewMonth.UpdateProgress -= UpdateProgress;
        }
        private void UpdateProgress(int value, string message)
        {
            TransitionProgressValue = value;
            TransitionInfo = message;
        }
    }
}
