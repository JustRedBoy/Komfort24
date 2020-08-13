using Desktop.Commands;

namespace Desktop.ViewModels
{
    public class TransitionViewModel : ViewModelBase
    {
        private int _maxTransitionProgressValue = 3;
        private int _transitionProgressValue = 0;
        private string _transitionMessage = "";

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
        public string TransitionMessage
        {
            get { return _transitionMessage; }
            set
            {
                _transitionMessage = value;
                OnPropertyChanged("TransitionMessage");
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
                      TransitionMessage = "Подготовка к переходу на новый месяц ...";
                      TransitionToNewMonth.UpdateProgress += UpdateProgress;
                      TransitionToNewMonth.TransitionCompleted += TransitionCompleted;
                      await TransitionToNewMonth.StartTransitionAsync();
                      RelayCommand.RaiseCanExecuteChanged();
                  },                
                  obj => !AppViewModel.IsAnyProcessing()));
            }
        }

        private void TransitionCompleted(int value, string message)
        {
            TransitionProgressValue = 0;
            TransitionMessage = message;
            TransitionToNewMonth.UpdateProgress -= UpdateProgress;
            TransitionToNewMonth.TransitionCompleted -= TransitionCompleted;
        }
        private void UpdateProgress(int value, string message)
        {
            TransitionProgressValue = value;
            TransitionMessage = message;
        }
    }
}
