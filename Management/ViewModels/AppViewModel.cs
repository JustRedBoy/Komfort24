using Management.Operations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Management.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        #region Fileds and Properties

        private int _generationProgressValue = 0;
        private int _maxGenerationProgressValue = 577;
        private string _generationMessage = "* Во время генерации нельзя пользоваться операциями копировать/вставить";
        private bool _isGenerationEnabled = true;
        private string _generationButtonText = "Начать";
        private bool _isGenerationPartEnabled = true;

        private bool _isTransitionPartEnabled = true;
        private int _maxTransitionProgressValue = 3; //
        private int _transitionProgressValue = 0;
        private bool _isTransitionEnabled = true;
        private string _transitionMessage = "";

        public int GenerationProgressValue
        {
            get { return _generationProgressValue; }
            set
            {
                _generationProgressValue = value;
                OnPropertyChanged("GenerationProgressValue");
            }
        }
        public int MaxGenerationProgressValue
        {
            get { return _maxGenerationProgressValue; }
            set
            {
                _maxGenerationProgressValue = value;
                OnPropertyChanged("MaxGenerationProgressValue");
            }
        }
        public string GenerationMessage
        {
            get { return _generationMessage; }
            set
            {
                _generationMessage = value;
                OnPropertyChanged("GenerationMessage");
            }
        }
        public bool IsGenerationEnabled
        {
            get { return _isGenerationEnabled; }
            set
            {
                _isGenerationEnabled = value;
                OnPropertyChanged("IsGenerationEnabled");
            }
        }
        public string GenerationButtonText
        {
            get { return _generationButtonText; }
            set
            {
                _generationButtonText = value;
                OnPropertyChanged("GenerationButtonText");
            }
        }
        public bool IsGenerationPartEnabled
        {
            get { return _isGenerationPartEnabled; }
            set
            {
                _isGenerationPartEnabled = value;
                OnPropertyChanged("IsGenerationPartEnabled");
            }
        }

        public bool IsTransitionPartEnabled
        {
            get { return _isTransitionPartEnabled; }
            set
            {
                _isTransitionPartEnabled = value;
                OnPropertyChanged("IsTransitionPartEnabled");
            }
        }
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
        public bool IsTransitionEnabled
        {
            get { return _isTransitionEnabled; }
            set
            {
                _isTransitionEnabled = value;
                OnPropertyChanged("IsTransitionEnabled");
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

        #endregion

        #region Commands

        private RelayCommand _generationCommand;
        public RelayCommand GenerationCommand
        {
            get
            {
                return _generationCommand ??
                  (_generationCommand = new RelayCommand(obj =>
                  {
                      if (GenerationFlyers.Processing)
                      {
                          GenerationMessage = "Отмена генерации ...";
                          IsGenerationEnabled = false;
                          GenerationFlyers.CancelGeneration();
                      }
                      else
                      {
                          GenerationButtonText = "Отменить";
                          GenerationMessage = "Подготовка к созданию листовок ...";
                          IsGenerationEnabled = false;
                          IsTransitionPartEnabled = false;
                          GenerationFlyers.UpdateProgress += GenerateFlyers_UpdateProgress;
                          GenerationFlyers.CompletedGeneration += GenerateFlyers_CompletedGeneration;
                          GenerationFlyers.StartGenerationAsync();
                      }
                  }));
            }
        }

        private RelayCommand _transitionCommand;
        public RelayCommand TransitionCommand
        {
            get
            {
                return _transitionCommand ??
                  (_transitionCommand = new RelayCommand(obj =>
                  {
                      TransitionMessage = "Подготовка к переходу на новый месяц ...";
                      IsTransitionEnabled = false;
                      IsGenerationPartEnabled = false;
                      TransitionToNewMonth.UpdateProgress += TransitionToNewMonth_UpdateProgress;
                      TransitionToNewMonth.CompletedTransition += TransitionToNewMonth_CompletedTransition;
                      TransitionToNewMonth.StartTransitionAsync();
                  }));
            }
        }

        #endregion

        #region Methods

        private void GenerateFlyers_CompletedGeneration(int result)
        {
            if (result == 0)
            {
                GenerationCompleted("Генерация листовок завершена");
            }
            else
            {
                GenerationCompleted("Генерация листовок отменена");
            }
        }
        private void GenerateFlyers_UpdateProgress(int value)
        {
            if (!GenerationFlyers.IsCancelled)
            {
                if (!IsGenerationEnabled && value == 1)
                {
                    IsGenerationEnabled = true;
                }
                GenerationProgressValue = value;
                GenerationMessage = $"Создание листовок ... {value} / {MaxGenerationProgressValue}";
            }
        }
        private void GenerationCompleted(string message)
        {
            IsGenerationEnabled = true;
            GenerationButtonText = "Начать";
            GenerationProgressValue = 0;
            GenerationMessage = message;
            IsTransitionPartEnabled = true;
            GenerationFlyers.UpdateProgress -= GenerateFlyers_UpdateProgress;
            GenerationFlyers.CompletedGeneration -= GenerateFlyers_CompletedGeneration;
        }

        private void TransitionToNewMonth_CompletedTransition(int value, string message)
        {
            IsTransitionEnabled = true;
            TransitionProgressValue = 0;
            TransitionMessage = message;
            IsGenerationPartEnabled = true;
            TransitionToNewMonth.UpdateProgress -= TransitionToNewMonth_UpdateProgress;
            TransitionToNewMonth.CompletedTransition -= TransitionToNewMonth_CompletedTransition;
        }
        private void TransitionToNewMonth_UpdateProgress(int value, string message)
        {
            TransitionProgressValue = value;
            TransitionMessage = message;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
